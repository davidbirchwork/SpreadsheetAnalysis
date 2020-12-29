using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utilities.XmlSerialisation;

namespace Utilities.Editor.Editors.Controllers {
    /// <summary>
    /// Controls a list / array by providing column with add new, delete copy and delete all?     
    /// </summary>
    public class ArrayController : AElementController {
        private readonly bool _ordered;
        private readonly string _name;
        private readonly string _description;
        private readonly Type _type;
        private readonly bool _allowNewItems;
        private readonly bool _allowDelete;
        private readonly int _minItems;
        private readonly int _maxItems;

        public ArrayController(string name, string description, Type type, bool allowNewItems, bool allowDelete, int minItems, int maxItems, bool ordered) {            
            this._name = name;
            this._description = description;
            this._type = type;
            this._allowNewItems = allowNewItems;
            this._allowDelete = allowDelete;
            this._minItems = minItems;
            this._maxItems = maxItems;
            this._ordered = ordered;
        }

        #region Overrides of AElementController

        protected override List<AEditor> CreateColumns() {
            List<AEditor> editors = new List<AEditor>();
            if (this._allowDelete) {
                editors.Add(new ButtonColumn("Delete", "Delete the current List object",
                                             element =>
                                             element.Name.ToString().Equals(this._name) ? "Delete All" : "Delete",
                                             element => {
                                                 if (element.Parent == null) {
                                                     return false;
                                                 }
                                                 if (element.Name.ToString().Equals(this._name)) {
                                                     if (this._minItems <= 0) {
                                                         element.RemoveNodes();
                                                         return true;
                                                     } else {
                                                         return false;
                                                     }
                                                 } else {
                                                     if (element.Parent.Elements().Count() > this._minItems) {
                                                         element.Remove();
                                                         return true;
                                                     } else {
                                                         return false;
                                                     }
                                                 }
                                             }));
            }
            if (this._allowNewItems) {
                editors.Add(new ButtonColumn("Clone", "Copy the element, inserting the copy at the end of the list.",
                                             element => element.Name.ToString().Equals(this._name) ? "Add New" : "Clone",
                                             element => {
                                                 if (element.Parent == null) {
                                                     return false;
                                                 }
                                                 if (element.Name.ToString().Equals(this._name)) {
                                                     if (element.Elements().Count() < this._maxItems) {

                                                         object newitem;
                                                         XElement newXML;
                                                         if (this._type.IsValueType) {
                                                             newitem = Activator.CreateInstance(this._type);
                                                             string name = this._type.Name;
                                                             if (name == "Int32") name = "int";
                                                             newXML = new XElement(name, newitem.ToString());
                                                         } else if (this._type.Equals(typeof(string))) {
                                                             newXML = new XElement(this._type.Name, "new item");
                                                         } else {
                                                             newitem = Activator.CreateInstance(this._type);
                                                             newXML =
                                                                 XElement.Parse(
                                                                     SerialisationController.Serialize(newitem));
                                                         }


                                                         element.Add(newXML);
                                                         return true;
                                                     } else {
                                                         return false;
                                                     }
                                                 } else {
                                                     if (element.Parent.Elements().Count() < this._maxItems) {
                                                         element.Parent.Add(XElement.Parse(element.ToString()));
                                                         return true;
                                                     } else {
                                                         return false;
                                                     }
                                                 }
                                             }));
            }
            if (this._ordered) {
                editors.Add(new ButtonColumn("MoveUp", "Moves an item up", e => "Move Up", element => {
                                    XElement parent =
                                        element.Parent;
                                    if (parent == null) {
                                        return false;
                                    }
                                    int insertPos = Math.Max(element.ElementsBeforeSelf().Count() - 1,0);
                                    element.Remove();
                                    var siblings = parent.Elements().ToList();

                                    foreach (XElement sibling in siblings) {
                                        sibling.Remove();
                                    }
                                    siblings.Insert(insertPos, element);

                                    foreach (XElement sibling in siblings) {
                                        parent.Add(sibling);
                                    }

                                    return true;
                }));

                editors.Add(new ButtonColumn("MoveDown", "Moves an item down", e => "Move Down", element => {
                    XElement parent =
                        element.Parent;
                    if (parent == null) {
                        return false;
                    }
                    int insertPos = element.ElementsBeforeSelf().Count() +1;                    
                    element.Remove();
                    var siblings = parent.Elements().ToList();
                    if (insertPos >= siblings.Count) {
                        insertPos = siblings.Count;
                    }
                    foreach (XElement sibling in siblings) {
                        sibling.Remove();
                    }
                    siblings.Insert(insertPos, element);

                    foreach (XElement sibling in siblings) {
                        parent.Add(sibling);
                    }

                    return true;
                }));
            }
            return editors;
        }

        #endregion
    }
}
