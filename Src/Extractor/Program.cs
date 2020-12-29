using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Excel_Interop_ClosedXML.SpreadUtil;
using ExcelInterop.Domain;
using GraphAnalysis.InputPartitioner;
using log4net;
using log4net.Config;
using ParseTreeExtractor;
using ParseTreeExtractor.Graph;

namespace Extractor {
    class Program {
        static void Main(string[] args) {
            SetUpLogging();

            string fName =
                args.Length > 0
                    ? args[0]
                    : 
                    @"B:\enron\spreadsheets\susan_bailey__39391__mercedconfirms.xlsx"; //<< all inputs
                    //@"B:\enron\tom_donohoe__39951__enron1lng_fixed.xlsx"; << nice example ;

            var p = new ExcelExtractor();

            var extraction = p.Extract(fName);

            var graph = SpreadGraphCreator.Create(extraction);

            SpreadGraphExporter.Export(graph, Path.ChangeExtension(fName, "graphml"));

            var inputs = InputPartitioner.PartitionInputs(graph, extraction);
            InputPartitioner.PrintInputs(inputs, fName);
            
            var foundVectors = IsoMorph.Partition(graph, extraction);

            SpreadColourer.Colour(fName, MakeSubFile(fName, "_input.xlsx"), inputs.ToDictionary(i => i.Name??"unknown"+Guid.NewGuid(), i => i.LabelCells.Union(i.Cells).Select(l => new ExcelAddress(l)).ToList()));
            SpreadColourer.Colour(fName, MakeSubFile(fName, "_iso.xlsx"), foundVectors.GetColourings());

            if (!(args.Length > 1)) {
                Console.WriteLine($"Finished Export Process{Environment.NewLine} press any key to exit");
                Console.ReadKey();
            }
        }

        private static string MakeSubFile(string fName, string name) {
            return Path.ChangeExtension(fName, "._"+name).Replace("._", "_");
        }

        private static void SetUpLogging() {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            var logger = LogManager.GetLogger(typeof(Program));

            logger.Info("Parse Tree Extractor");
        }
    }
}