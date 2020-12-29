using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ExcelExtractor;
using Excel_Interop_ClosedXML;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExtractor_Test
{
    [TestClass]
    public class WholeExtractionTests   
    {
        private MemoryAppender memoryAppender;

        [TestInitialize]
        public void SetUp() {
            this.memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(this.memoryAppender);
        }


        [TestMethod]
        public void ExtractWhole() {
            var fname = Directory.GetCurrentDirectory() + @"\TestFiles\sumtest.xlsx";
            
            SemaphoreSlim waiter = new SemaphoreSlim(0);

            ExtractionController c = new ExtractionController();
            var e = c.BeginWholeExtraction(fname, 1, () => {
                    c.EvaluateAll();

                waiter.Release();

                },
                new List<Tuple<string, string>>(), ExcelReaderClosedXml.FactoryWhole);

            waiter.Wait();


            Assert.IsFalse(
                this.memoryAppender.GetEvents().Any(le => le.Level == Level.Error),
                this.memoryAppender.GetEvents().Aggregate("Did not expect any error messages in the logs " ,
                    (acc,next)=> acc+Environment.NewLine+ "["+next.Level+"] "+next.RenderedMessage));




        }
    }
}
