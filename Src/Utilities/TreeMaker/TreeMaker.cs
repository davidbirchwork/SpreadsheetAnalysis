using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Utilities.TreeMaker {
    public static class TreeMaker {
        // todo this could be more generic
        /// <summary>
        /// Makes a tree from a set of related XElements 
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="nameExtractor">The name extractor for each element.</param>
        /// <param name="seperator">The seperator character - normally _.</param>
        /// <returns>a tree :) accessible using chidfunc </returns>
        public static XElement MakeTree(IEnumerable<XElement> elements, Func<XElement,string> nameExtractor,string seperator) {

            SortedDictionary<string, XElement> namedElements = new SortedDictionary<string,XElement>(elements.ToDictionary(nameExtractor));
            XElement root = new XElement("Variables");
            Dictionary<string, XElement> addedNodes = new Dictionary<string, XElement>();

            string[] names = namedElements.Keys.ToArray();

            for (int e = names.Length- 1; e >= 0; e--) { // going backwards is more efficient
                string name = names[e];
                string workingName = name;
                string nameSoFar = "";
                XElement parent = root;

                if (addedNodes.ContainsKey(name)) { // handle case where we've already added a non-data node to the same 
                    addedNodes[name].Add(new XElement(Data, namedElements[name]));
                } else {

                    while (workingName.Contains(seperator)) {

                        string nextpeice = workingName.Substring(0, workingName.IndexOf(seperator));

                        workingName = workingName.Remove(0, workingName.IndexOf(seperator) + seperator.Length);

                        if (nameSoFar != "") {
                            nameSoFar += seperator;
                        }
                        nameSoFar += nextpeice;

                        if (addedNodes.ContainsKey(nameSoFar)) {
                            parent = addedNodes[nameSoFar];
                        } else {
                            // create a new non-data node
                            XElement newnode = new XElement(new XElement("TreeNode",new XAttribute("Name",nextpeice)));
                            parent.Add(newnode);
                            parent = newnode;
                            addedNodes.Add(nameSoFar, newnode);
                        }                       
                    }

                    parent.Add(new XElement("TreeNode",new XAttribute("Name",workingName), new XElement(Data, namedElements[name])));

                }
            }

            return root;
        }

        public const string Data = "Data";
        public const string Name = "Name";

        public static bool HasData(this XElement element) {
            return element.GetData() != null;
        }

        public static XElement GetData(this XElement element) {
            if (element == null) {
                return null;
            }
            XElement data = element.Element(Data);
            return data == null ? null : 
                data.HasElements ?
                    data.Elements().First() :
                    data;
        }

        private static string NameExtractor(XElement xelem)  {
            var nameattr = xelem.Attribute("Name");
            return nameattr == null ? null : nameattr.Value;
        }

        public static readonly Func<XElement, List<XElement>> ChildFunc = element => {
                                                                              var elems = element.Elements().Where(
                                                                                  child =>
                                                                                      child.Name.ToString() != Data &&
                                                                                      child.Name.ToString() != Name).ToList();
                                                                              elems.Sort(comparison:Comparer);
                                                                              return elems;
                                                                          };

        private static int Comparer(XElement x, XElement y) {
            var xname = NameExtractor(x);
            var yname = NameExtractor(y);
            return xname.CompareTo(yname);
        }
    }
}
