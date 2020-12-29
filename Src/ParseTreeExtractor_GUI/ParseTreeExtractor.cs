using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net.Config;

namespace ParseTreeExtractor {
    public partial class ParseTreeExtractor : Form {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ParseTreeExtractor() {
            InitializeComponent();
        }

        private void ParseTreeExtractor_Load(object sender, EventArgs e) {
            XmlConfigurator.Configure();
            Log.Info("Hello World");
        }

        private void btnStart_Click_1(object sender, EventArgs e) {
            if (File.Exists(this.txtFileName.Text)) {
                Task.Factory.StartNew(() => {
                    var p = new ExcelExtractor();
                    var extraction = p.Extract(this.txtFileName.Text);
                    NeoExporter.ExportToNeo(extraction);
                });
            }
            else {
                Log.Error("No File Provided");
            }


        }
    }
}
