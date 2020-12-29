using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExcelInterop;
using ExcelInterop.Domain;
using LinqExtensions.Extensions;
using NCalcExcel;
using Utilities.Loggers;

namespace ExcelFunctionExtractor {
    class XmlFormulaTreeMethods {
        private static void ValidateTree(XElement xElement) {
            if (xElement == null) {
                throw new ArgumentNullException("null element");
            }

            if (xElement.Attribute("NameGivenByParent") == null) {
                throw new ArgumentNullException("null NameGivenByParent");
            }

            if (xElement.Attribute("Unprocessed") != null) {
                throw new ArgumentNullException("found unprocessed node");
            }

            if (xElement.Attribute("Name") == null) {
                throw new ArgumentNullException("null Name");
            }

            if (xElement.Attribute("Formula") == null) {
                throw new ArgumentNullException("null Formula");
            }

            string formula = xElement.Attribute("Formula").Value;

            if (formula == "BLANK") {
                if (xElement.Elements().Count() != 0) {
                    throw new ArgumentException("Blank cells should not have children");
                }
            }

            if (formula == "RANGE") {
                if (!xElement.Elements().Any()) {
                    throw new ArgumentException("non-expanded range");
                }
            }
            else {

                if (xElement.Attribute("IsFormula") == null) {
                    if (xElement.Elements().Count() != 0) {
                        throw new ArgumentException("Non formula cells should not have children");
                    }
                }
            }

            foreach (XElement child in xElement.Elements()) {
                ValidateTree(child);
            }

        }

        private int gccount = 0;

        /// <summary>
        /// This is only called recursively 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="level"></param>
        /// <param name="evaling"></param>
        /// <param name="factory"></param>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        private string GetFormula(XElement root, int level, bool evaling, ExpressionFactory factory,
            ConcurrentDictionary<string, object> variableMap) {
            gccount++;
            //  replace this.knownbyparentas by the formula if its not null or blank or numeric or a RANGE
            //      if it is a RANGE then we should replace the knownbyparentas by the list of children [get the correct name]

            string name = root.Attribute("Name").Value;
            //         if (root.Attribute("KnownAs") != null) {
            //            name = root.Attribute("KnownAs").Value;
            //       }            
            string formula = "ERROR!!!";
            try {
                formula = root.Attribute("Formula").Value;
            }
            catch (Exception) {
                formula = "ERROR!!!";
            }

            if (formula == "BLANK") {
                variableMap.AddIfNotExistant(name, "\"\"");
                if (root.Attribute("KnownAs") != null) {
                    variableMap.AddIfNotExistant(root.Attribute("KnownAs").Value, "\"\"");
                }

                return "[" + name + "]"; // wrap the name
            }

            if (formula == "RANGE") {
                // recurse for children then concat their returned formulas together...
                string result = root.Elements().Aggregate("",
                    (res, element) => res + ",(" + GetFormula(element, level, evaling, factory, variableMap) + ")");
                if (result[0] == ',') {
                    result = result.Substring(1); // remove first , ...
                }

                return result;
            }

            if (root.Attribute("IsFormula") == null) {
                variableMap.AddIfNotExistant(name, formula);
                if (root.Attribute("KnownAs") != null) {
                    variableMap.AddIfNotExistant(root.Attribute("KnownAs").Value, formula);
                }

                return "([" + name + "])"; // wrap the name 
            }

            // now recurse...
            string theresult = formula;
            int i = 0;

            Dictionary<string, string> replacements = new Dictionary<string, string>();
            ConcurrentBag<Tuple<string, string>>
                pchildren =
                    new ConcurrentBag<Tuple<string, string>>(); // doing this should avoid problems replacing parts of child formulas by mistake
            Parallel.ForEach(root.Elements(),
                child => pchildren.Add(new Tuple<string, string>(child.Attribute("NameGivenByParent").Value,
                    GetFormula(child, level, evaling, factory, variableMap)))); // get the list of what to replace)

            List<Tuple<string, string>> sorted = new List<Tuple<string, string>>();
            List<Tuple<string, string>> children = pchildren.ToList();
            while (children.Count > 0) {
                sorted.AddRange(children.Where(tuple =>
                    !children.Any(other => !other.Item1.Equals(tuple.Item1) && other.Item1.Contains(tuple.Item1))));
                children.RemoveAll(child => sorted.Contains(child));
            }

            children = null;
            // sort replacememnts..

            //  if (level % 10 == 0 && evaling) {
            //     pchildren = new ConcurrentBag<Tuple<string, string>>();
            //    Parallel.ForEach<Tuple<string, string>>(sorted, tuple => pchildren.Add(new Tuple<string, string>(tuple.Item1, this.Factory.Evaluate(tuple.Item2, variableMap).ToString())));
            //    sorted = pchildren.ToList();
            //}

            foreach (Tuple<string, string> child in sorted) { // do dummy replacements
                replacements.Add("REPLACEMENTERING" + i + "GNIRETNEMECALPER", child.Item2); // + BODMAS
                if (!theresult.Contains(child.Item1)) {
                    throw new Exception("pants");
                }

                theresult = theresult.Replace(child.Item1, "REPLACEMENTERING" + i + "GNIRETNEMECALPER");
                i++;
            }

            sorted = null;
            if (gccount % 20000 == 0) {
                GC.Collect();
            }

            foreach (KeyValuePair<string, string> replacement in replacements) { // do actual replacements
                string replacementValue = replacement.Value;
                // here we need to be careful of inserting unescaped strings
                if (!factory.TryParse(replacementValue)) {
                    if (!factory.TryParse("Sum(" + replacementValue + ")")) { // handle arrays
                        replacementValue = "\"" + replacementValue + "\"";
                    }
                }

                theresult = theresult.Replace(replacement.Key, replacementValue);
            }

            //    if (!this.Factory.TryParse(theresult)) {
            //       throw new ArgumentException("bad formula "+ theresult);
            //  }
            //GC.Collect();
            if (level % 10 == 0 && evaling) {
                theresult = factory.Evaluate(theresult,
                    variableMap.ToDictionary(valuePair => valuePair.Key, valuePair => valuePair.Value)).ToString();
            }

            if (!factory.TryParse(theresult)) {
                if (!factory.TryParse("Sum(" + theresult + ")")) { // handle arrays
                    return "\"" + theresult + "\"";
                }
            }

            return "(" + theresult + ")";
        }


#pragma warning disable 649
        private Dictionary<String, XElement> KnownCells;
        private Dictionary<int, Tuple<XElement, string, string>> ReferencesToAdd;
#pragma warning restore 649

        private void AddChildren(XElement xElement) {
            if (xElement.Name == "Ref2Sort") {
                Tuple<XElement, string, string> refToAdd = this.ReferencesToAdd[int.Parse(xElement.Value)];

                xElement.Parent.Add(this.KnownCells[refToAdd.Item2]);
                Tuple<XElement, string, string> add = refToAdd;
                List<XElement> nodes = xElement.Parent.Elements().Where(elem =>
                    elem.Name != "Ref2Sort" && elem.Attribute("Name").Value.Equals(add.Item2)).ToList();
                if (nodes.Count() != 1) {
                    throw new Exception("node has duplicate names");
                }

                foreach (XElement node in nodes) {
                    node.SetAttributeValue("NameGivenByParent",
                        refToAdd.Item3); // this should sort out problems with cells being known by different names
                    AddChildren(node);
                }

                xElement.Remove();

            }
            else {
                XElement[] children = xElement.Elements().ToArray(); // we make a static copy
                foreach (XElement child in children) {
                    AddChildren(child);
                }
            }
        }

        private int count;

#pragma warning disable 649
#pragma warning disable 169
        private Stack<XElement> WorkElements;
        private ConcurrentDictionary<string, object> VariableMap;
#pragma warning restore 169
#pragma warning restore 649


        private void AddReferences(XElement element, ExcelAddress address, List<string> references,
            IExcelReader excelObject) {
            for (int r = 0; r < references.Count; r++) {
                string knownbyParent = references[r];
                XElement child = new XElement("Cell", new XAttribute("NameGivenByParent", references[r]),
                    new XAttribute("Unprocessed", "true"));

                if (!references[r].Contains("!")) {
                    // its a local reference to append the current sheet?
                    references[r] = address.WorkSheet + "!" + references[r];
                }

                //done what if a range is passed this way?
                if (!references[r].Contains(":")) {
                    ExcelAddress newname = new ExcelAddress(references[r]);
                    try {
                        // test we can read it
                        string formula = excelObject.ReadFormula(newname);
                        // test it is a valid cell name
                        new CellName(newname.CellReference);
                    }
                    catch (Exception e) {
                        string error = "bad name " + newname + " exception " + e;
                        Logger.DEBUG(error);
                        references[r] = excelObject.FindName(newname.CellReference).ToString();
                        child.Add(new XAttribute("KnownAs", newname.CellReference));
                    }
                }

                // lets remove all $'s 
                references[r] = references[r].Replace("$", "");

                child.Add(new XAttribute("Name", references[r]));

                if (!this.KnownCells.ContainsKey(references[r])) {
                    this.KnownCells.Add(references[r], child);
                    int r1 = r;
                    if (!this.WorkElements.ToList().Any(elem => elem.Attribute("Name").Value.Equals(references[r1]))) {
                        this.WorkElements.Push(child);
                    }
                    else {
                        Logger.ERROR("named a cell twice");
                    }

                }

                element.Add(new XElement("Ref2Sort", this.count));
                this.ReferencesToAdd.Add(this.count,
                    new Tuple<XElement, string, string>(element, references[r], knownbyParent));
                this.count++;
            }
        }

    }
}
