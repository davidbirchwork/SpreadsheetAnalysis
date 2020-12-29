using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Utilities.Loggers;
using System.Linq;

namespace Utilities.XmlSerialisation {

    /// <summary>
    /// Wrappers for the xml serialisation
    /// if you add ___ to the name of a node it will be split into another element
    /// </summary>
    public static class SerialisationController {

        #region UTF8 encoding        

        //Pasted from <http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp> 

        private static String UTF8ByteArrayToString(Byte[] characters) {
            UTF8Encoding encoding = new UTF8Encoding(false);
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static Byte[] StringToUTF8ByteArray(String pXmlString) {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        #endregion

        /// <summary>
        /// Method to convert an Object to XML string
        /// using UTF8
        /// code upgraded from http://www.dotnetjohn.com/articles.aspx?articleid=173
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object that is to be serialized to XML</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>XML string</returns>
        public static String Serialize<T>(T obj, Type[] extraTypes = null) where T : class {
            return SerializeToXElement<T>(obj, extraTypes).ToString();
        }

        public static XElement SerializeToXElement<T>(T obj, Type[] extraTypes = null) where T : class {
            GC.Collect();
            try {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = extraTypes == null ? new XmlSerializer(obj.GetType()) : new XmlSerializer(obj.GetType(), extraTypes);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xs.Serialize(xmlTextWriter, obj);

                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                string encoded = UTF8ByteArrayToString(memoryStream.ToArray());

                memoryStream = new MemoryStream(StringToUTF8ByteArray(encoded));
                XmlTextReader xmlTextReader = new XmlTextReader(memoryStream);


                XDocument doc = XDocument.Load(xmlTextReader);
                xmlTextReader = null;
                foreach (XElement child in doc.Elements()) {
                    FormatDocument(child);
                }

                if (typeof(T) != typeof(Type) && doc.Root != null && doc.Root.Element("TYPEDEFINITION") == null) {
                    doc.Root.Add(new XElement("TYPEDEFINITION", obj.GetType().AssemblyQualifiedName));
                    // ReSharper disable PossibleNullReferenceException
                    doc.Root.Attribute(XNamespace.Xmlns + "xsi").Remove();
                    doc.Root.Attribute(XNamespace.Xmlns + "xsd").Remove();
                    // ReSharper restore PossibleNullReferenceException
                }

                return doc.Root;
            } catch (Exception e) {
                Logger.ERROR("Xml serialisation failed for type " + typeof(T).FullName + " error was: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Formats the document - takes nodes with ___ in their name and splits them into other nodes
        /// </summary>
        /// <param name="element">The element.</param>
        private static void FormatDocument(XElement element) {

            // first we recurse as we will likely change the document structure so we cant iterate through it - thus we use an array which will not change

            XElement[] array = element.Elements().ToArray();

            foreach (XElement child in array) {
                FormatDocument(child);
            }

            if (element.Name.ToString().Contains(ChildChar)) {
                string[] path = element.Name.ToString().Split(new[] {ChildChar},
                                                              StringSplitOptions.RemoveEmptyEntries);
                XElement parent = element.Parent;
                element.Remove();

                if (parent != null) {
                    int i = 0;

                    while (i < path.Length - 1) {

                        if (parent.Element(path[i]) != null) { // if the path exists                             
                            parent = parent.Element(path[i]);
                            // ensure that it contains a property group attribute.
// ReSharper disable PossibleNullReferenceException
                            if (parent.Attribute("PropertyGroup") == null) {
// ReSharper restore PossibleNullReferenceException
                                parent.Add(new XAttribute("PropertyGroup", "true"));
                            }
                        } else { // if the path does not exist create it. 
                            XElement addElem = new XElement(path[i],new XAttribute("PropertyGroup","true"));
                            parent.Add(addElem);
                            parent = addElem;
                        }

                        i++;
                    }

                    element.Name = path[i]; // should be the last in the array
                    parent.Add(element);
                }
            }          

        }

        public static string ChildChar {
            get { return "___"; }
        }

        /// <summary>
        /// Method to reconstruct an Object from XML string
        /// using UTF8
        /// code upgraded from http://www.dotnetjohn.com/articles.aspx?articleid=173
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(String xmlString) {
            string formatedxml = "";
            try {
                XElement doc = XElement.Parse(xmlString);
                foreach (XElement child in doc.Elements()) {
                    UnFormatDocument(child);
                }

                formatedxml = doc.ToString();

                XmlSerializer xs = new XmlSerializer(typeof(T));
                MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(formatedxml));
                XmlTextReader xmlTextWriter = new XmlTextReader(memoryStream);                


                return (T) xs.Deserialize(xmlTextWriter);
            } catch (Exception e) {
                Logger.INFO(formatedxml);
                Logger.INFO(e.Message);
                Logger.ERROR("Xml deserialisation failed for type " + typeof(T).FullName + " error was: " + e.Message);
                return default(T);
            }

        }

        private static void UnFormatDocument(XElement elem) {
            if (elem.Parent != null) {
                if (elem.Attribute("PropertyGroup") != null) {
                    foreach (XElement child in elem.Elements()) {
                        child.Name = elem.Name + ChildChar + child.Name;
                        child.Remove();
                        if (elem.Parent == null) {
                            Logger.ERROR("cannot ungroup the root item");
                        } else {
                            elem.Parent.Add(child);
                        }
                    }
                    elem.Remove();
                }

                // and recurse 
                foreach (XElement child in elem.Elements()) {
                    UnFormatDocument(child);
                }
            }
        }

        public static object Deserialize(string xmlString, Type type) {
            MethodInfo method = typeof(SerialisationController).GetMethod("Deserialize", new [] {typeof(String)}).MakeGenericMethod(new[] { type });
            return method.Invoke(null, new object[] { xmlString });                    
        }

        /// <summary>
        /// Deserializes the specified XML string to the type specified by its FullName in a TYPEDEFINITION element
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>a hydrated object</returns>
        public static object DeserializeFromXmlWithType(string xmlString) {
            XElement elem = XElement.Parse(xmlString);
            XElement typeDefn = elem.Element("TYPEDEFINITION");
            if (typeDefn == null) {
                 Logger.FAILURE("Could not edit object as no TYPEDEFINITION was found");
                return null;
            }
            //Debugger.Log(1,"ERROR","Serialiser is trying to find type: " + typeDefn.Value);
            Type t = Type.GetType(typeDefn.Value,true,true);
            
            object obj = Deserialize(xmlString, t);
            
            return obj;
        }
    }
}
