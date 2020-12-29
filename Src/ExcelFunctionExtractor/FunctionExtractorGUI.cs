using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ExcelExtractor;
using ExcelExtractor.Analyses;
using ExcelExtractor.Analyses.Graph;
using ExcelExtractor.Domain;
using ExcelInterop;
using Excel_Interop_ClosedXML;
using Excel_Interop_COM;
using log4net.Config;
using Utilities.Loggers;
using Utilities.Tree;
using Utilities.Tree.Columns;

namespace ExcelFunctionExtractor {
    public partial class FunctionExtractorGUI : Form {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ExtractionController ExtractionController = new ExtractionController();
        private readonly Func<string, IExcelReader> _readerFactor = ExcelReaderClosedXml.Factory; //ExcelCOMFile.Factory;// ExcelReaderClosedXml.Factory;
        private readonly Func<string, IExcelWholeReader> _readerFactorWhole = ExcelReaderClosedXml.FactoryWhole;

        private XElement _configXml;

        public FunctionExtractorGUI() {
            InitializeComponent();

        }

        private delegate void BtnRefreshClickCallback(object sender, EventArgs e);

        private void BtnRefreshClick(object sender, EventArgs e) {
            if (InvokeRequired) {
                 BtnRefreshClickCallback d = new BtnRefreshClickCallback(BtnRefreshClick);
                 this.Invoke(d, new object[] { sender, e });
            } else {
                this.Tree.UpdateRows(true);
            }
        }

        private void BtnExtractClick(object sender, EventArgs e)
        {
            UpdateConfigXML();
            var shims = this._configXml.Elements().Select(elem =>
                new Tuple<string, string>(elem.Attribute("Input").Value, elem.Attribute("Output").Value)).ToList();
            var tree = ExtractionController.BeginExtraction(this.ExcelFileAddress.Text,
                (int) this.numThreadsPicker.Value, this.edtRootCell.Text, this.DisplayResults, shims, _readerFactor);
            SetUpTreeGUI(tree);
        }

        private void btn_Extract_whole_Click(object sender, EventArgs e) {
            UpdateConfigXML();
            var shims = this._configXml.Elements().Select(elem =>
                new Tuple<string, string>(elem.Attribute("Input").Value, elem.Attribute("Output").Value)).ToList();
            var tree = ExtractionController.BeginWholeExtraction(this.ExcelFileAddress.Text,
                (int)this.numThreadsPicker.Value, this.DisplayResults, shims, _readerFactorWhole);
            SetUpTreeGUI(tree);
        }

        private void DisplayResults() {
            if (!InvokeRequired) {
                BtnSaveResultsAsClick(this, new EventArgs());
                this.Tree.DataSource =  this.btnDisplayList.Checked ? this.ExtractionController.GetListXml() : this.ExtractionController.GetRootXml();
                BtnRefreshClick(this, new EventArgs());                

            } else {
                DisplayResultsCallback d = DisplayResults;
                this.Invoke(d);
            }
        }

        private delegate void DisplayResultsCallback();

        private void SetUpTreeGUI(XElement rootElement)
        {
            XTreeView treeview = new XTreeView("formula tree", "shows the formula tree", () => rootElement,
                x => x.Elements().ToList(), // display the whole tree                
                new AttributeColumn("Name", "ERROR") // read only with default value                
                , new List<AxTreeColumn>
                {
                    new AttributeColumn("Formula", "none!"),
                    new AttributeColumn("KnownAs", " "),
                    new AttributeColumn("Value", "???"),
                    new AttributeColumn("CalculatedAs", "double click to calc")
                });

            this.Tree.XTree = treeview;
        }
        
        #region graph save        

        private void BtnSaveGraphClick(object sender, EventArgs e) {

            string friendlynamesfile = (this.SAMappingOpener.ShowDialog() == DialogResult.OK)? this.SAMappingOpener.FileName: null;

            var includeBlanksChecked = this.IncludeBlanks.Checked;
            var usesheetPrefix = this.chkSheetPrefix.Checked;
            
            if (this.saveGMLFile.ShowDialog() == DialogResult.OK) {
                var ext = new ExtractRefGraph();
                ext.ExtractGraph(friendlynamesfile, includeBlanksChecked, usesheetPrefix, this.saveGMLFile.FileName, this.checkboxKnownNames.Checked,this.ExtractionController.Extractor,this.ExtractionController.Factory);
            }
        }

        private void BtnSaveEvalGraphClick(object sender, EventArgs e) {
            if (this.saveGMLFile.ShowDialog() == DialogResult.OK) {
                ExtractionController.SaveGraph(this.saveGMLFile.FileName, this.chkSheetPrefix.Checked);
            }
        }

        private void btnSaveWholeGraph_Click(object sender, EventArgs e)
        {
            if (this.saveGMLFile.ShowDialog() == DialogResult.OK)
            {
                ExtractionController.SaveWholeGraph(this.saveGMLFile.FileName);
            }
        }

        private void btnSaveSubGraphs_Click(object sender, EventArgs e)
        {
            if (this.saveGMLFile.ShowDialog() == DialogResult.OK)
            {
                ExtractionController.SaveGraphComponents(this.saveGMLFile.FileName);
            }
        }

        private void btnAnalysePartition_Click(object sender, EventArgs e)
        {
            if (this.saveGMLFile.ShowDialog() == DialogResult.OK)
            {
             var graphs = ExtractionController.SaveGraphComponents(this.saveGMLFile.FileName);

                var tablegraphs = ExtractionController.SaveTables(Path.ChangeExtension(this.saveGMLFile.FileName, "_tables.graphml").Replace("._", "_"));

            }
        }

        #endregion       

        #region SA Save                 

        private void BtnSaveSAClick(object sender, EventArgs e) {
            if (this.SAMappingOpener.ShowDialog() == DialogResult.OK) {
                if (this.SASaver.ShowDialog() == DialogResult.OK)  {
                    SAGeneration.CreateSArun(this.SAMappingOpener.FileName, this.ExcelFileAddress.Text, this.SASaver.FileName);
                }
            }
        }

        #endregion        

        #region logger

        private delegate void LogMessageCallback(int id);

        private void LogUpdateDelegate(int id) {
            if (!InvokeRequired) {
                IEnumerable<LogMessage> messages = Logger.GetMessagesForLogger(id, false);
                int i = messages.Count();
                foreach (var logMessage in messages) {
                    this.LogBox.AppendText("\r\n" +
                                           string.Format("{0}:{1:000}: [{2}]: {3}", logMessage.DateTime,
                                                         logMessage.DateTime.Millisecond, logMessage.Level,
                                                         logMessage.Message));
                }
                Logger.FinishProcessingMessages(id);
            } else {                  
                LogMessageCallback d = LogUpdateDelegate;
                this.Invoke(d, new object[] { id });
            }
        }

        private void FunctionExtractorGUI_Load(object sender, EventArgs e) {
         //   this.LogId = Logger.RegisterThreadedLogger(this.LogUpdateDelegate);
            this._configXml = CreateShimTree();
            XmlConfigurator.Configure();
            Log.Info("Hello World");
        }

        private void FunctionExtractorGUI_FormClosed(object sender, FormClosedEventArgs e) {
         //   Logger.DeRegisterThreadedLogger(this.LogId);
        }

        #endregion

        #region cell evaluation        

        private void TreeDoubleClick(object sender, EventArgs e) {
            EvaluateThisCell((this.Tree.FocusItem as XElement));
        }

        private void EvaluateThisCell(XElement selectedCell) {
            Task.Factory.StartNew(() =>
            {
                this.ExtractionController.EvaluateAll();
                UpdateCalcValues(selectedCell, this.ExtractionController.Extractor);              
            })
            ;           
        }

        private delegate void UpdateCalcValuesCallback(XElement selectedCell, FunctionExtractor extractor);

        private void UpdateCalcValues(XElement selectedCell, FunctionExtractor extractor) {
            if (!InvokeRequired) {
                UpdateCellWithNewValue(selectedCell, extractor);
                this.Tree.UpdateRowData();
            } else {
                UpdateCalcValuesCallback d = UpdateCalcValues;
                this.Invoke(d, new object[] { selectedCell, extractor });
            }
        }

        private void UpdateCellWithNewValue(XElement selectedCell, FunctionExtractor extractor) {
            XAttribute attribute = selectedCell.Attribute("Name");
            if (attribute != null && extractor.ProcessedCells.ContainsKey(attribute.Value)) {
                object res = extractor.ProcessedCells[attribute.Value].EvaluatedValue;
                XAttribute valueAttr = selectedCell.Attribute("CalculatedAs");
                if (valueAttr != null) {
                    valueAttr.Remove();
                }
                selectedCell.Add(new XAttribute("CalculatedAs", res == null ? "": res.ToString()));
            }
            foreach (var child in selectedCell.Elements()) {
                UpdateCellWithNewValue(child, extractor);
            }
        }

        private void EvaluateSelectedClick(object sender, EventArgs e) {
            EvaluateThisCell((this.Tree.DataSource as XElement));
        }

        #endregion

        #region xml config

        private void UpdateConfigXML() {
            // ReSharper disable PossibleNullReferenceException
            //            this.ConfigXml.Attribute("ConfigFilename").Value = this.XXX.Text;
            this._configXml.Attribute("ExcelFile").Value = this.ExcelFileAddress.Text;
            this._configXml.Attribute("ExcelCell").Value = this.edtRootCell.Text;
            // ReSharper restore PossibleNullReferenceException
        }

        private XElement CreateShimTree() {
            var rootElement = ExcelExtractor.ExtractionController.CreateConfig();
            XTreeView treeview = new XTreeView("Formula Shims", "shows the shim tree",
                                               () => rootElement,
                                               x => x.Elements().ToList(), // display the whole tree
                                               new AttributeColumn("Input", "ERROR", isreadonly: false)                                               
                                               , new List<AxTreeColumn> {
                                                                            new AttributeColumn("Output", "ERROR", isreadonly: false)
                                                                        }) {ShowRootNode = false};

            this.shimTree.XTree = treeview;
            return rootElement;
        }

        private void BtnSaveConfigClick(object sender, EventArgs e) {
            // ReSharper disable PossibleNullReferenceException
            string fname = this._configXml.Attribute("ConfigFilename").Value;
            // ReSharper restore PossibleNullReferenceException
            if (File.Exists(fname)) {
                this._configXml.Save(fname);
            } else {
                BtnsaveConfigAsClick(sender, e);
            }
        }

        private void BtnsaveConfigAsClick(object sender, EventArgs e) {
            if (this.configsaver.ShowDialog() == DialogResult.OK) {
                UpdateConfigXML();
                // ReSharper disable PossibleNullReferenceException
                this._configXml.Attribute("ConfigFilename").Value = this.configsaver.FileName;
                // ReSharper restore PossibleNullReferenceException
                this._configXml.Save(this.configsaver.FileName);
            }
        }        

        private void BtnLoadConfigClick(object sender, EventArgs e) {
            if (this.configOpener.ShowDialog() == DialogResult.OK) {
                
                this._configXml = XElement.Parse(File.ReadAllText(this.configOpener.FileName));
                this.shimTree.DataSource = this._configXml;
                this.shimTree.UpdateRows();
                this.shimTree.UpdateRowData();
                // ReSharper disable PossibleNullReferenceException
                this.ExcelFileAddress.Text = this._configXml.Attribute("ExcelFile").Value;
                this.edtRootCell.Text = this._configXml.Attribute("ExcelCell").Value;
                this._configXml.Attribute("ConfigFilename").Value = this.configOpener.FileName;
                // ReSharper restore PossibleNullReferenceException
            }
        }

        private void BtnAddShimClick(object sender, EventArgs e) {
            this._configXml.Add(new XElement("FormulaShim",
                                                             new XAttribute("Input", "from this"),
                                                             new XAttribute("Output", "to this")));
            this.shimTree.UpdateRows();
        }

        private void BtnDeleteShimClick(object sender, EventArgs e) {
            XElement selected = this.shimTree.SelectedItem as XElement;
            if (selected != null) {
                selected.Remove();
            }
        }

        private void BtnExcelSelectClick(object sender, EventArgs e) {
            if (this.ExcelOpener.ShowDialog() == DialogResult.OK) {
                this.ExcelFileAddress.Text = this.ExcelOpener.FileName;
                UpdateConfigXML();
            }
        }

        #endregion

        #region Load/Save Results

        private static string SaveConfig<T>(T obj) {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw) { Formatting = Formatting.Indented, Indentation = 4 };

            xs.Serialize(tw, obj);
            string s = sw.ToString();

            tw.Close();
            sw.Close();

            return s;
        }

        private static T LoadConfig<T>(string xmltext) {
            XmlSerializer xs = new XmlSerializer(typeof(T));            

            StringReader sr = new StringReader(xmltext);
            XmlTextReader tw = new XmlTextReader(sr);

            return (T)xs.Deserialize(tw);
        }        

        private string _resultsFName;
        

        private void BtnLoadResultsClick(object sender, EventArgs e) {
            if (this.configOpener.ShowDialog() == DialogResult.OK) {
                this._resultsFName = this.configOpener.FileName;
                var factory = ExcelExtractor.ExtractionController.CreateExpressionFactory();
                var shims = this._configXml.Elements().Select(elem => new Tuple<string, string>(elem.Attribute("Input").Value, elem.Attribute("Output").Value)).ToList();
                var extractor = new FunctionExtractor(factory,this.ExcelFileAddress.Text,shims,(int) this.numThreadsPicker.Value,ExcelCOMFile.Factory);
                extractor.LoadResultsFrom(this._resultsFName);
                SetUpTreeGUI(ExcelExtractor.ExtractionController.SetUpTree(this.edtRootCell.Text));
                this.Tree.ShowRootRow = !this.btnDisplayList.Checked;
                this.Tree.DataSource = this.btnDisplayList.Checked ? extractor.GetListXML() : extractor.GetRootXML();
                BtnRefreshClick(this, new EventArgs());                
            }
        }

        private void BtnSaveResultsClick(object sender, EventArgs e) {
            if (File.Exists(this._resultsFName)) {
                Task.Factory.StartNew(() => { ExtractionController.SaveResults(this._resultsFName); });
            } else {
                BtnSaveResultsAsClick(sender, e);
            }
        }

        private void BtnSaveResultsAsClick(object sender, EventArgs e) {
            if (this.configsaver.ShowDialog() == DialogResult.OK) {
                this._resultsFName =this.configsaver.FileName;
                Task.Factory.StartNew(() => { ExtractionController.SaveResults(this._resultsFName); });
            }
        }

        #endregion

        #region debug

        
        private void BtnEvaluateClick(object sender, EventArgs e) {
            var factory = ExcelExtractor.ExtractionController.CreateExpressionFactory();
            MessageBox.Show(
                $@"Result is: {factory.Evaluate(this.formulatestbox.Text, new Dictionary<string, object>())}");
        }

        #endregion

        private void cleanExcelNamesToolStripMenuItem_Click(object sender, EventArgs e) {

            var excelObject = new ExcelCOMFile(this.ExcelFileAddress.Text);
            List<string> cleanedExcelNames = excelObject.CleanExcelNames();
            Logger.SUCCESS("Found "+cleanedExcelNames.Count+" bad names and cleaned them");
            foreach (var name in cleanedExcelNames) {
                Logger.INFO("cleaned "+name);
            }
            if (cleanedExcelNames.Count > 0) {
                excelObject.Close(true);
                Logger.SUCCESS("Saved Excel Book");
            }
        }

    }
}
