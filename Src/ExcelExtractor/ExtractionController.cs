using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExcelExtractor.Analyses.Graph;
using ExcelExtractor.Domain;
using ExcelInterop;
using ExcelInterop.Domain;
using NCalcExcel;

namespace ExcelExtractor {
    /// <summary>
    /// control the extraction of a spreadsheet based on the formulas within it
    /// </summary>
    public class ExtractionController {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExpressionFactory Factory;
        public FunctionExtractor Extractor;

        public XElement BeginWholeExtraction(string excelFilename, int threads, Action onFinish,
            List<Tuple<string, string>> shims, Func<string, IExcelWholeReader> readerFactor) {

            Log.Info("Starting extraction of " +excelFilename);
            List<string> addresses = new List<string>();

            using (var reader = readerFactor(excelFilename)) {
                var sheetNames = reader.GetSheetNames();
                Log.Info("we found "+sheetNames.Count+" worksheets");
                foreach (var sheet in sheetNames) {
                    var range = reader.GetUsedCells(sheet);
                    if (range == null) {
                        Log.Info($"Worksheet {sheet} is blank");
                        continue;
                    }
                    var cells = ExcelAddress.ExpandRangeToExcelAddresses(range);
                    Log.Info("Worksheet "+sheet+" has "+cells.Count+" used cells");
                    addresses.AddRange(cells.Select(c=> c.ToString()));
                }
            }

            Log.Info("in total we have "+addresses.Count+" cells to extract");

            XElement rootElement = SetUpTree(addresses.First());


            this.Factory = CreateExpressionFactory();
            this.Extractor = new FunctionExtractor(this.Factory, excelFilename, shims, threads, readerFactor);

            Log.Info("Launching Extraction");
            this.Extractor.BeginExtractionFrom(addresses, onFinish);
            return rootElement;
        }

        public XElement BeginExtraction(string excelFilename, int threads, string startExcelAddress, Action onFinish,
            List<Tuple<string, string>> shims, Func<string, IExcelReader> readerFactor) {
            // set up the output 
            XElement rootElement = SetUpTree(startExcelAddress);

            this.Factory = CreateExpressionFactory();
            this.Extractor = new FunctionExtractor(this.Factory, excelFilename, shims, threads, readerFactor);

            Log.Info("Launching Extraction");
            this.Extractor.BeginExtractionFrom(startExcelAddress, onFinish);
            return rootElement;
        }

        /// <summary>
        /// Creates a root node for the extracted tree
        /// </summary>
        /// <param name="rootCell"></param>
        /// <returns></returns>
        public static XElement SetUpTree(string rootCell) {
            XElement rootElement = new XElement("Cell",
                new XAttribute("Name", rootCell),
                new XAttribute("NameGivenByParent", rootCell)
            );

            return rootElement;
        }

        public static ExpressionFactory CreateExpressionFactory() {
            var currentDirectory = Directory.GetCurrentDirectory();
            return new ExpressionFactory(
                new AggregateCatalog( //new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()),
                    new DirectoryCatalog(currentDirectory))) {
                Options =
                    EvaluateOptions.IgnoreCase | EvaluateOptions.BuiltInFunctionsFirst |
                    EvaluateOptions.ReduceTo15Sigfig |
                    EvaluateOptions.DontUseStringConcat //  EvaluateOptions.NoCache | << speed improvement
            };
        }

        public static XElement CreateConfig() {
            XElement rootElement = new XElement("FunctionExtractorConfig",
                new XAttribute("ConfigFilename", "none"),
                new XAttribute("ExcelFile", "none"),
                new XAttribute("ExcelCell", "none"),
                new XElement("FormulaShim",
                    new XAttribute("Input", "from this"),
                    new XAttribute("Output", "to this"))
            );
            return rootElement;
        }

        public XElement GetListXml() {
            return this.Extractor.GetListXML();
        }

        public void EvaluateAll() {
            this.Extractor.EvaluateAll();
        }

        public XElement GetRootXml() {
            return this.Extractor.GetRootXML();
        }

        public void SaveResults(string baseFileName) {
            Extractor.ReturnExtractedTree().Serialize(baseFileName);
            Log.Info("Saved Results: " + baseFileName);

            string all = Path.ChangeExtension(baseFileName, "_all.tsv").Replace("._","_");
            File.WriteAllText(all, Extractor.GetCellsAsTSV());
            Log.Info("Saved Results: " + all);

            string allbutranges = Path.ChangeExtension(baseFileName, "_allbutRanges.tsv").Replace("._", "_");
            File.WriteAllText(allbutranges, Extractor.GetCellsAsTSV(isRange: false));
            Log.Info("Saved Results: " + allbutranges);

            string inputs = Path.ChangeExtension(baseFileName, "_inputs.tsv").Replace("._", "_");
            File.WriteAllText(inputs, Extractor.GetCellsAsTSV(isRange: false, hasReferences: false));
            Log.Info("Saved Results: " + inputs);

            string nonblankinputs = Path.ChangeExtension(baseFileName, "_nonblankinputs.tsv").Replace("._", "_");
            File.WriteAllText(nonblankinputs,
                Extractor.GetCellsAsTSV(isRange: false, hasReferences: false, isBlank: false));
            Log.Info("Saved Results: " + nonblankinputs);

            string blankinputs = Path.ChangeExtension(baseFileName, "_blankinputs.tsv").Replace("._", "_");
            File.WriteAllText(blankinputs,
                Extractor.GetCellsAsTSV(isRange: false, hasReferences: false, isBlank: true));
            Log.Info("Saved Results: " + blankinputs);

            string numericnonblankinputs = Path.ChangeExtension(baseFileName, "_numericnonblankinputs.tsv").Replace("._", "_");
            File.WriteAllText(numericnonblankinputs,
                Extractor.GetCellsAsTSV(isRange: false, hasReferences: false, isBlank: false, isNumeric: true));
            Log.Info("Saved Results: " + numericnonblankinputs);

        }

        // todo these methods should be refactored so that the ExtractionClass takes an ExtractionController

        public void SaveGraph(string fileName, bool useSheetPrefix) {
            this.Extractor.SaveGraph(fileName,useSheetPrefix,this.Factory);
        }

        public void SaveWholeGraph(string fileName) {
            ExtractWholeGraph.Save(this.Extractor, fileName);
        }

        public List<ExcelGraph> SaveGraphComponents(string fileName) {
            var extractor = new ExtractGraphComponents();
            return extractor.PartitionGraph(this.Extractor,fileName);
        }

        public List<ExcelGraph> SaveTables(string fileName) {
            var extractor = new ExtractGraphTables();
            return extractor.PartitionGraph(this.Extractor, fileName);
            
        }
    }
}
