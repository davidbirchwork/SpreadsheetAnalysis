using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Utilities.Config {
    
    /// <summary>
    /// Wrapper class for extracting data from the .config file for the exe
    /// 
    /// Example call:
    /// MessageBox.Show(AppConfig.GetValue string ("Test"));
    /// </summary>
    /// // Singleton Design Pattern from http://www.yoda.arachsys.com/csharp/singleton.html 
    public sealed class AppConfig {
// ReSharper disable InconsistentNaming
        private static readonly AppConfig instance = new AppConfig();
// ReSharper restore InconsistentNaming

        private AppSettingsReader _reader;

// ReSharper disable UnusedMember.Global
        public static AppConfig Instance {
// ReSharper restore UnusedMember.Global
            get {
                return instance;
            }
        }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
// ReSharper disable EmptyConstructor
        static AppConfig() {
// ReSharper restore EmptyConstructor
        }

        private AppConfig() {
            this._reader = new AppSettingsReader();            
        }
        
        private T _GetValue<T>(string name) {
            try {
                return (T) this._reader.GetValue(name, typeof(T));
            } catch (Exception e) {                
                throw new FileNotFoundException("Configurator failed with error "+e.Message+" it was trying to find: ",name);
            }            
        }

        private List<string> _GetValues(string name) {
            List<string> values = new List<string>();
            XDocument xmlDoc = XDocument.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            if (xmlDoc.Root != null) {
                XElement appSettings = xmlDoc.Root.Element("appSettings");

                if (appSettings != null) {
                    foreach (XElement element in appSettings.Elements("add")) {
                        XAttribute elemName = element.Attribute("key");
                        if (elemName != null && elemName.Value == name) {
                            XAttribute elemvalue = element.Attribute("value");
                            if (elemvalue != null) {
                                values.Add(elemvalue.Value);
                            }
                        }
                    }
                }
            }

            return values;
        }

        private void _WriteValue<T>(string key, T value) {            
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            if (xmlDoc.DocumentElement != null)
                foreach (XmlElement element in xmlDoc.DocumentElement.ChildNodes) {
                    if (!element.Name.Equals("appSettings")) continue;
                    bool updated = false;
                    foreach (XmlNode node in
                        element.ChildNodes.Cast<XmlNode>().Where(node => node != null && node.Attributes != null && node.Attributes.Count>1 && node.Attributes[0].Value.Equals(key))) {
                        if (node.Attributes != null) {
                            node.Attributes[0].Value = value.ToString();
                            updated = true;
                        }                        
                    }
                    if (updated) continue;
                    XmlElement elem = xmlDoc.CreateElement("", "add", "");
                    elem.SetAttribute(key, "", value.ToString());                
                    element.AppendChild(elem);
                }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);           
            this._reader = new AppSettingsReader(); // i hope that this refreshes things ;-/
        }

        /// <summary>
        /// Read value of a given type T in the .config file of the application
        /// </summary>
        /// <typeparam name="T">Type which you wish to read</typeparam>
        /// <param name="name">name (ie key value) of the elem to read in the .config value</param>
        /// <returns>value specified - or null/default</returns>
        public static T GetValue<T>(string name) {
            return instance._GetValue<T>(name);
        }

        public static IEnumerable<string> GetValues(string name) {
            return instance._GetValues(name);
        }        

        public static string GetExeDirectory() {            
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", ""))+"\\";
        }

        /// <summary>
        /// Add/update Key with Value in the config file
        /// </summary>
        /// <typeparam name="T">Type - must cast to/from string</typeparam>
        /// <param name="key">string key for elem in the xml config file</param>
        /// <param name="value">value which must cast to/from string</param>
        public static void WriteValue<T>(string key, T value) {
            instance._WriteValue(key,value);
        }

        public static AggregateCatalog GetAggregateCatalog() {
            AggregateCatalog catalog = new AggregateCatalog();
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly!= null) {catalog.Catalogs.Add(new AssemblyCatalog(assembly));}
            //catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)));
            foreach (string directory in AppConfig.GetValues("MEF_Directory")) {
                catalog.Catalogs.Add(new DirectoryCatalog(Path.GetFullPath(directory)));
            }            

            return catalog;
        }

        public static CompositionContainer GetCompositionContainer() {
            return new CompositionContainer(GetAggregateCatalog());
        }
    }
}