using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqExtensions.Extensions {
    public static class XElementExtensions {

        /// <summary>
        /// Performs a deep equality test on two XNodes using the XNodeEqualityComparer
        /// <see cref="http://msdn.microsoft.com/en-us/library/bb348073.aspx"/>
        /// </summary>
        /// <param name="elem1">The elem1.</param>
        /// <param name="elem2">The elem2.</param>
        /// <returns>true if they are deeply equal</returns>
        public static bool DeepEquals(this XNode elem1, XNode elem2) {
            XNodeEqualityComparer comparer = new XNodeEqualityComparer();
            return comparer.Equals(elem1, elem2);
        }

        public static bool AddOrSetAttribute(this XElement element, XName name,string value) {
            if (element == null) return false;

            XAttribute attr = element.Attribute(name);
            if (attr == null) {
                element.Add(new XAttribute(name, value));
            } else {
                attr.Value = value;
            }
            return true;
        }

        /// <summary>
        /// Attributes the value or null if it doesnt exist
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string AttributeValue(this XElement element, string attributeName) {
            return element?.Attribute(attributeName)?.Value;
        }

        /// <summary>
        /// Updates the root element by ensureing the attributes match the update node. then copying all elements which are not in the ignorable list from the the update element and deleting any which dont appear
        /// DOES NOT RECURSE
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="updates">The updates.</param>
        /// <param name="ignorableElements">The ignorable elements.</param>
        /// <param name="ignorableAttributes">The ignorable attributes.</param>
        public static void UpdateRootElement(this XElement element, XElement updates, IEnumerable<string> ignorableElements, string[] ignorableAttributes) {
            if (element == null) return;

            if (ignorableElements.Contains(element.Name.ToString())) return;

            // update attributes on the element
            List<XAttribute> attribsToRemove = new List<XAttribute>();
            foreach (XAttribute attribute in element.Attributes()) {
                if (!ignorableAttributes.Contains(attribute.Name.ToString())) {
                    XAttribute updateAttrib = updates.Attribute(attribute.Name);
                    if (updateAttrib != null) {
                        attribute.SetValue(updateAttrib.Value);
                    } else {
                        attribsToRemove.Add(attribute);
                    }
                }
            }

            // remove attributes not within the new element
            foreach (XAttribute attribute in attribsToRemove) {
                attribute.Remove();
            }

            // add attributes in the new element but not in the old
            foreach (XAttribute newAttribute in updates.Attributes()) {
                if (!ignorableAttributes.Contains(newAttribute.Name.ToString())) {
                    XAttribute oldAttrib = element.Attribute(newAttribute.Name);
                    if (oldAttrib == null) {
                        element.Add(new XAttribute(newAttribute.Name, newAttribute.Value));
                    } 
                }
            }
            
            // now handle case where there is a string as the elements value

            if (!element.HasElements) {
                if (!updates.HasElements) {
                    element.Value = updates.Value;
                    return;
                } else {
                    element.Value = string.Empty;
                    foreach (XElement child in updates.Elements()) {
                        if (!ignorableElements.Contains(child.Name.ToString())) {
                            element.Add(XElement.Parse(child.ToString())); // note might get ignorable stuff here... 
                        }
                    }
                }
            }
            
            // now recurse if the elements are not within the ignore list

            List<XElement> elemToRemove = new List<XElement>();
            List<XElement> elemToAdd = new List<XElement>();

            List<XName> elemNames = element.Elements().Select(elem => elem.Name).Distinct().ToList();

            foreach (XName elemName in elemNames) {
                if (!ignorableElements.Contains(elemName.ToString())) {
                    List<XElement> elementsElements = element.Elements(elemName).ToList();
                    List<XElement> updateElements = updates.Elements(elemName).ToList();
                    if (updateElements.Count == 0) {
                        elemToRemove.AddRange(elementsElements);
                    } else {
                        elemToAdd.AddRange(updateElements);
                        elemToRemove.AddRange(elementsElements);
                    }
                }
            }

            foreach (XElement xElement in elemToRemove) {
                xElement.Remove();
            }

            // add elements in updates but not in element
            List<XName> updateNewNames = updates.Elements().Select(elem => elem.Name).Where(name => element.Elements().Any(elem => elem.Name == name)).Distinct().ToList();
            foreach (XName updateNewName in updateNewNames) {
                elemToAdd.AddRange(updates.Elements(updateNewName));
            }            

            foreach (XElement xElement in elemToAdd) {
                XElement newChild = XElement.Parse(xElement.ToString());
                element.Add(newChild); // note we might be getting extra stuff?  <- recursion point but against what??!
                newChild.RemoveAttribute(XNamespace.Xmlns +"xsl"); // little bit of a hack
            }            
            
        }

        /// <summary>
        /// Determines whether the specified element has an attribute of the given name
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>
        /// 	<c>true</c> if the specified element has an attribute of the given name; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement element, XName attributeName) {
            return element.Attribute(attributeName) != null;
        }

        /// <summary>
        /// Removes attributes with a given name
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attributes to remove.</param>
        public static void RemoveAttribute(this XElement element, XName attributeName) {
            XAttribute[] attribsToRemove = element.Attributes(attributeName).ToArray();
            foreach (XAttribute xAttribute in attribsToRemove) {
                xAttribute.Remove();
            }
        }

        public static string ElementValueorNull(this XElement element,XName elementName) {
            XElement child = element.Element(elementName);
            return child == null ? null : child.Value;
        }
    }
}
