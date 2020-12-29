using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExcelInterop.Domain;
using Excel_Interop_ClosedXML;
using Irony.Parsing;
using log4net;
using ParseTreeExtractor.AST;
using ParseTreeExtractor.Domain;
using XLParser;

namespace ParseTreeExtractor  {
    public class ExcelExtractor {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Extraction Extract(string excelFilename) {
            var extraction = new Extraction(); // todo it may be clearer to revert to building this at the end... 

            Log.Info("About to parse "+excelFilename);
            // get the named ranges
            extraction.NamedRangeMap = FindAllNamedRanges(excelFilename);

            // find cells
            extraction.SheetRanges = FindSheetRanges(excelFilename);
            ExpandSheetRangesbyNamedRanges(extraction);
            extraction.Cells = FindAllUsedCells(extraction.SheetRanges);
            Log.Info("we have "+ extraction.Cells.Count+" cells to parse");
            if (extraction.Cells.Count > 1000000) {
                throw new InsufficientMemoryException("more than a million cells");
            }
            
            // find formulas
            extraction.CellFormulas = GetFormulas(extraction, excelFilename);
            Log.Info("we have collected "+ extraction.CellFormulas.Count+" cell formulas");
            
            // parse them
            extraction.ParseTrees = CreateParseTrees(extraction);
            LabelParseTrees(extraction.ParseTrees);
            Log.Info("We have parsed "+extraction.ParseTrees.Count+" parse trees");

            // clean parse trees
            extraction.Grammar = ParseTreeCleaner.CleanParseTrees(extraction);

            // figure out what in the AST references WHO
            GenerateReferences(extraction);

            Log.Info("Finished Extraction :)");

            return extraction;
        }

        private void ExpandSheetRangesbyNamedRanges(Extraction extraction) {
            // sometimes the named ranges reference things outside of the size of the reported used range
            foreach (var map in extraction.NamedRangeMap.Where(nvm => nvm.Value.Any())) {
                var workSheet = map.Value.First().WorkSheet;
                var maxRow = map.Value.Max(r => r.IntRow);
                var maxCol = map.Value.Max(r => r.IntCol);
                var extents = extraction.SheetRanges[workSheet].RangeBottomRightCell();
                if (maxRow > extents.IntRow) {
                    extraction.SheetRanges[workSheet] = new ExcelAddress(extents.WorkSheet, "A1:"+extents.Column()+maxRow);
                    extents = extraction.SheetRanges[workSheet].RangeBottomRightCell();
                }
                if (maxCol > extents.IntCol) {
                    extraction.SheetRanges[workSheet] = new ExcelAddress(extents.WorkSheet, "A1:" + CellName.IntToColumn(maxCol) + extents.IntRow);                   
                }
            }            
        }

        /// <summary>
        /// Give parse tree nodes a consistent id. 
        /// </summary>
        /// <param name="parseTrees"></param>
        private void LabelParseTrees(ConcurrentDictionary<ExcelAddress, ParseTreeNode> parseTrees) {
            // recurse down the tree and build a list of nodes with relationships
            Queue<ParseTreeNode> todo = new Queue<ParseTreeNode>();

            foreach (var tree in parseTrees) {
                if (tree.Value == null) continue;

                todo.Enqueue(tree.Value);

                int id = 0;

                while (todo.Any()) {
                    var next = todo.Dequeue();

                    id++;
                    var nodeId = tree.Key.FullName + "_" + id;

                    next.Tag = new Tuple<string, object>(nodeId, next.Tag);

                    foreach (var childNode in next.ChildNodes) {
                        todo.Enqueue(childNode);
                    }
                }
            }
        }

        private static ConcurrentDictionary<ExcelAddress, ParseTreeNode> CreateParseTrees(Extraction extraction) {
            ConcurrentBag<string> errors = new ConcurrentBag<string>();

            var parseTreeNodes = new ConcurrentDictionary<ExcelAddress, ParseTreeNode>();
            Parallel.ForEach(extraction.CellFormulas, cellFormula => {
                try {
                    if (String.IsNullOrWhiteSpace(cellFormula.Value)) {
                        parseTreeNodes.AddOrUpdate(cellFormula.Key, k => null,
                            (k, v) => null); // todo figure out what blank space is 
                    }
                    else if (cellFormula.Value.StartsWith("=") ) {
                            var parseTreeNode = ExcelFormulaParser.Parse(cellFormula.Value);
                            parseTreeNodes.AddOrUpdate(cellFormula.Key, k => parseTreeNode, (k, v) => parseTreeNode);
                    } else {
                        var val = cellFormula.Value;
                        if (!double.TryParse(val, out _)) {
                            // its a stringish value
                            if (val.Contains("\"")) {
                                val = val.Replace("\"", "''");
                            }
                            val = "\"" + val + "\"";
                            val = val.Replace("\"'", "\"");// remove prefix '
                        }
                        var parseTreeNode = ExcelFormulaParser.Parse(val);
                        parseTreeNodes.AddOrUpdate(cellFormula.Key, k => parseTreeNode, (k, v) => parseTreeNode);
                    }

                }
                catch (Exception e) {
                    errors.Add(e.ToString());
                    if (cellFormula.Value.EndsWith("}")) {
                        Log.Error("Array Formulas are Not Supported");
                    } else {
                        Log.Error("failed to Parse " + cellFormula.Key + " |" + cellFormula.Value + "|", e);
                    }

                    parseTreeNodes.AddOrUpdate(cellFormula.Key, k => null, (k, v) => null);
                }
            });
            return parseTreeNodes;
        }

        private ConcurrentDictionary<ExcelAddress, string> GetFormulas(Extraction extraction, string excelFilename) {
            using (var reader = new ExcelReaderClosedXml(excelFilename, true)) {
                // not sure if this is thread safe...
                var d = extraction.Cells.ToDictionary(c => c, c => reader.ReadFormula(c));

                var cd = new ConcurrentDictionary<ExcelAddress, string>();
                foreach (var f in d) {
                    cd.TryAdd(f.Key, f.Value);
                }
                return cd;
            }
        }

        private Dictionary<string, List<ExcelAddress>> FindAllNamedRanges(string excelFilename) {
            using (var reader = new ExcelReaderClosedXml(excelFilename, true)) {
                var res =  reader.FindAllNamedRanges();
                Log.Info("we have "+res.Count+" named ranges");
                return res;
            }
        }

        private static Dictionary<string, ExcelAddress> FindSheetRanges(string excelFilename) {
            Log.Info("Starting extraction of " + excelFilename);
            Dictionary<string, ExcelAddress> sheetRanges = new Dictionary<string, ExcelAddress>();

            using (var reader = new ExcelReaderClosedXml(excelFilename, true)) {
                var sheetNames = reader.GetSheetNames();
                Log.Info("we found " + sheetNames.Count + " worksheets");
                foreach (var sheet in sheetNames) {
                    var range = reader.GetUsedCells(sheet);
                    if (range == null) {
                        Log.Info($"Worksheet {sheet} is blank");
                        continue;
                    }

                    Log.Info("Worksheet " + sheet + " has used cells");
                    sheetRanges.Add(sheet, range);
                }
            }

            Log.Info("in total we have " + sheetRanges.Count + " cells to extract");
            return sheetRanges;
        }

        private static List<ExcelAddress> FindAllUsedCells(Dictionary<string, ExcelAddress> sheetRanges) {
            List<ExcelAddress> addresses = new List<ExcelAddress>();

            foreach (var sheet in sheetRanges) {
                var cells = ExcelAddress.ExpandRangeToExcelAddresses(sheet.Value);
                Log.Info("Worksheet " + sheet.Key + " has " + cells.Count + " used cells");
                addresses.AddRange(cells);
            }

            Log.Info("in total we have " + addresses.Count + " cells to extract");
            return addresses;
        }

        /// <summary>
        /// objective here is to generate the links between reference nodes and cell nodes
        /// references have been pruned and placed in the their tag node
        /// we will need to make NEW range reference nodes where they are encountered
        /// </summary>
        /// <param name="extraction"></param>
        private static void GenerateReferences(Extraction extraction) {
            /* steps:
             1) find the reference nodes & extract their tags which will be the reference tree
             2) find simple cell references in the AST
             3) work out from the context of the reference which cell is being referenced 
             4) record the need to create a link
             5) for range references << contain a ReferenceFunction 
                - record the need to create a range node
                - record the need to link to it 
                - expand out the range and record the links it will need 
             6) Export to Neo4J
                - create new range nodes
                - create reference links 
            */
            var grammar = extraction.Grammar;

            Log.Info("about to generate references between Parse Trees");


            #region 1) find the reference nodes & extract their tags which will be the reference tree

            var referencesToProcess = new List<Tuple<ExcelAddress, ParseTreeNode>>();

            foreach (var cell in extraction.ParseTrees) {
                var queue = new Queue<ParseTreeNode>();
                if (cell.Value != null) {
                    queue.Enqueue(cell.Value);
                }

                while (queue.Any()) {
                    var node = queue.Dequeue();
                    if (node.Term?.Name == extraction.Grammar.Reference.Name) {
                        referencesToProcess.Add(new Tuple<ExcelAddress, ParseTreeNode>(cell.Key, node));
                    }

                    if (node.ChildNodes.Any()) {
                        foreach (var child in node.ChildNodes) {
                            queue.Enqueue(child);
                        }
                    }
                }
            }

            #endregion

            List<Tuple<string, string>> refsToMake = new List<Tuple<string, string>>();
            List<Tuple<string, string>> namedRangeRefs = new List<Tuple<string, string>>();
            List<Tuple<string, string>> rangeRefs = new List<Tuple<string, string>>();
            
            #region  2-5) find references and 

            foreach (var reference in referencesToProcess) {
                if (reference.Item1.ToString().Contains("C2")) {
                    Log.Info("breakpoint");
                }

                var refTag = reference.Item2.Tag as Tuple<string, object>; // id / parse tree
                var refTree = refTag.Item2 as ParseTreeNode[];
                if (refTree == null) {
                    Log.Error("failed to find reference sub tree for cell " + reference.Item1);
                    continue;
                }

                // deal with "Prefix" nodes appearing here
                var sheet = reference.Item1.WorkSheet;
                var prefixNode = refTree.FirstOrDefault(t => t?.Term.Name == grammar.Prefix.Name);
                if (prefixNode != null && prefixNode.ChildNodes.Any(c => c?.Term.Name == grammar.QuotedFileSheet.Name
                                                                         || c?.Term.Name == grammar.File.Name)) {
                    Log.Info("Ignoring Reference to a worksheet in another file");
                    continue;
                }
                var token = prefixNode?.ChildNodes.FirstOrDefault(t => t?.Term.Name == grammar.SheetToken.Name) ??
                            prefixNode?.ChildNodes.FirstOrDefault(t => t?.Term.Name == grammar.SheetQuotedToken.Name);
                if (token != null) {
                    sheet = token.Token.ValueString.Replace("!", "").Replace("'","");
                }
                if (string.IsNullOrWhiteSpace(sheet)) {
                    Log.Warn("Bad sheet reference");
                }

                if (refTree.Length > 2) {
                    Log.Error("found a strange multiple reference" + reference.Item1);
                    continue;
                }

                // now start dealing with different types of reference 
                if (refTree.Any(t => t?.Term.Name == grammar.NamedRange.Name)) {
                    List<ParseTreeNode> nameToken = refTree.First()?.GetDescendentsOfType(grammar.NameToken.Name);
                    var combined = refTree.First()?.GetDescendentsOfType(grammar.NamedRangeCombinationToken.Name);
                    if (combined != null && combined.Any()) {
                        nameToken.AddRange(combined);
                    }

                    if (nameToken == null || nameToken.Count != 1) {
                        Log.Error("found a weird named range" + reference.Item1);
                        continue;
                    }

                    var name = nameToken.First()?.Token.ValueString;
                    if (string.IsNullOrWhiteSpace(name)) {
                        Log.Error("bad name" + reference.Item1);
                    }
                    namedRangeRefs.Add(new Tuple<string, string>(refTag.Item1, name));

                }
                else if (refTree.Any(t => t?.Term.Name == grammar.ReferenceFunctionCall.Name)) {
                    // its a range reference 
                    List<ParseTreeNode> callRoots = refTree
                        .SelectMany(c => c.GetDescendentsOfType(grammar.ReferenceFunctionCall.Name)).ToList();
                    if (callRoots.Count != 1) {
                        Log.Error("found a strange reference function call"+reference.Item1);
                        continue;
                    }

                    var callRoot = callRoots.First();
                    if (callRoot.ChildNodes.Count!= 3){
                        Log.Error("found a strange reference function call" + reference.Item1);
                        continue;
                    }
                    var leftNode = callRoot.ChildNodes[0].GetDescendentsOfType(grammar.CellToken.Name).FirstOrDefault();
                    var rightNode = callRoot.ChildNodes[2].GetDescendentsOfType(grammar.CellToken.Name)
                        .FirstOrDefault();

                    // if there is a sheet prefix to  the reference 
                    if (callRoot.ChildNodes[0].ChildNodes.Count > 1) {
                        // ref node > prefix node > prefix name
                        var childPrefixNode = callRoot.ChildNodes[0].ChildNodes.FirstOrDefault();
                        if (childPrefixNode == null) {
                            Log.Error("bad prefix node");
                            continue;
                        }

                        if (childPrefixNode?.ChildNodes?.Any(c => c?.Term?.Name == grammar.File.Name) == true) {
                            Log.Info("Skipping external file reference");
                            continue;
                        }

                        var sheetName = childPrefixNode.ChildNodes.Select(c => c?.Token?.ValueString)
                            .Aggregate("", (acc, next) => next == null ? acc : acc + next);

                        sheetName = sheetName?.Replace("!", "").Replace("'",""); //todo null reference exception being thrown here... 
                        if (string.IsNullOrWhiteSpace(sheetName)) {
                            Log.Error("Bad sheet reference " + reference.Item1);
                            continue;
                        }

                        sheet = sheetName;
                    }

                    if (callRoot.ChildNodes.Count != 3
                        || leftNode == null
                        || callRoot.ChildNodes[1].Term.Name != grammar.KeyTerms[":"].Text
                        || rightNode == null) {
                        Log.Error("malformed reference" + reference.Item1);
                        continue;

                    }

                    string leftText = leftNode.Token.ValueString.Replace("$", "");
                    string rightText = rightNode.Token.ValueString.Replace("$", "");

                    
                    if (leftText.Contains("!")) {
                        var s = leftText.Split('!');
                        sheet = s[0];
                        leftText = s[1];
                    }

                    if (string.IsNullOrWhiteSpace(sheet)) {
                        Log.Warn("Bad sheet reference");
                    }

                    rangeRefs.Add(new Tuple<string, string>(refTag.Item1, sheet + "!" + leftText + ":" + rightText));


                } else if (refTree.Any(t => t?.Term.Name == grammar.VRange.Name)) {
                    // change a whole column reference to a normal range reference using the used cells range of each worksheet. 
                    var refString = refTree.Last().ChildNodes.First()?.Token?.Text;
                    if (string.IsNullOrWhiteSpace(refString)) {
                        Log.Error("failed to find VRange reference string "+ reference.Item1);
                        continue;
                    }
                    var cols = refString.Replace("$", "").Split(':');
                    if (!extraction.SheetRanges.TryGetValue(sheet, out var sheetRange)) {
                        Log.Error("failed to find sheet "+sheet);
                        continue;
                    }

                    var rangeRef = new ExcelAddress(sheet,
                        cols[0] + "1" + ":" + cols[1] + sheetRange.RangeDimensions().Item2);
                    var rangeString = rangeRef.ToString();
                    rangeRefs.Add(new Tuple<string, string>(refTag.Item1, rangeString));
                } else if (refTree.Any(t => t?.Term.Name == grammar.HRange.Name)) {
                    // change a whole row reference to a normal range reference using the used cells range of each worksheet. 
                    var refString = refTree.Last().ChildNodes.First()?.Token?.Text;
                    if (string.IsNullOrWhiteSpace(refString)) {
                        Log.Error("failed to find HRange reference string " + reference.Item1);
                        continue;
                    }
                    var rows = refString.Replace("$", "").Split(':');
                    if (!extraction.SheetRanges.TryGetValue(sheet, out var sheetRange)) {
                        Log.Error("failed to find sheet " + sheet);
                        continue;
                    }

                    var rangeRef = new ExcelAddress(sheet,
                        "A" + rows[0] + ":" + sheetRange.RangeBottomRight().Column + rows[1]);
                    var rangeString = rangeRef.ToString();
                    rangeRefs.Add(new Tuple<string, string>(refTag.Item1, rangeString));
                } else {

                    if (refTree.Any(c => c.GetDescendentsOfType(grammar.RefError.Name).Any())) {
                        Log.Info("Found a reference error in a spreadsheet " + reference.Item1);
                        continue;
                    }

                    // loop down to the CellToken node 

                    var refs = refTree.Last().GetDescendentsOfType(grammar.CellToken.Name);
                    if (refTree.Length > 2 || refs.Count != 1) {
                        Log.Error("found a simple reference that is more complex " + reference.Item1);
                        continue;
                    }

                    // its a simple reference 

                    // figure out where we are referencing to 
                    ParseTreeNode refNode = refs.FirstOrDefault();
                    if (refNode == null) {
                        Log.Error("bad reference" + reference.Item1);
                        continue;
                    }
                    string refString = refNode.Token.ValueString.Replace("$", "");
                    try {
                        var refCell = refString.Contains("!")
                            ? new ExcelAddress(refString)
                            : new ExcelAddress(sheet, refString);

                        // record the need to make the reference 
                        // between refToken and the cell node 

                        refsToMake.Add(new Tuple<string, string>(refTag.Item1, refCell.ToString()));
                    }
                    catch (Exception e) {
                        Log.Error("could not parse reference " + refString, e);
                    }

                }
            }

            #endregion
            
            // link between AST's and the range nodes
            refsToMake.AddRange(namedRangeRefs);
            refsToMake.AddRange(rangeRefs);
            // here we find the list of all ranges pointed at by AST nodes and then expand them out
            // we then register the refs between the range node and its expanded version
            var uniqueRanges = rangeRefs.Select(r => r.Item2).Distinct().ToList();
            extraction.Ranges = uniqueRanges.Select(r => {
                ExcelAddress range = new ExcelAddress(r);
                var expanded = ExcelAddress.ExpandRangeToCellList(range);
                // expand out the range to its links
                refsToMake.AddRange(expanded.Select(l =>
                    new Tuple<string, string>(r, new ExcelAddress(range.WorkSheet,l).ToString())));
                return new RangeNode( r, range.WorkSheet, expanded.Count);
            }).ToList();

            // check for any weird named ranges
            if (namedRangeRefs.Any()) {
                var odd = namedRangeRefs.Distinct()
                    .Where(nr => extraction.NamedRanges.FirstOrDefault(r => r.Label == nr.Item2) == null).ToList();
                if (odd.Any()) {
                    Log.Warn(odd.Aggregate("found a named ranged which isn't known by the Excel Reader! ",
                        (acc, next) => acc + " " + next.Item1));
                }
            }

            // expand out the named range nodes
            refsToMake.AddRange(extraction.NamedRangeMap.SelectMany(r =>
                r.Value.Select(d => new Tuple<string, string>(r.Key, d.ToString()))));

            Log.Info($"we found {refsToMake.Count} references between Parse Trees");

            extraction.References = refsToMake;
        }
    }
}