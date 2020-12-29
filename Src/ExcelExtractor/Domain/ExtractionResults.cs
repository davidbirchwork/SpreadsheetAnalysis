using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ExcelExtractor.Domain {
    public class ExtractionResults {
        public List<ExtractedCell> ProcessedCells { get; set; }
        public string rootCell { get; set; }

        public static T LoadConfig<T>(string xmltext) {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            StringReader sr = new StringReader(xmltext);
            XmlTextReader tw = new XmlTextReader(sr);

            return (T)xs.Deserialize(tw);
        }

        public void Serialize(string fname) {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            using (TextWriter sw = new StreamWriter(fname,false) ) {
                using (XmlTextWriter tw = new XmlTextWriter(sw) {Formatting = Formatting.Indented, Indentation = 4}) {

                    xs.Serialize(tw, this);

                    tw.Close();
                    sw.Close();
                }
            }
        }
    }
}