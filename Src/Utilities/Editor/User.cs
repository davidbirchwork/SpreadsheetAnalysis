using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using LinqExtensions.Extensions;
using Utilities.Command;
using Utilities.Editor.Editors;
using Utilities.Editor.Editors.Controllers;
using Utilities.Loggers;
using Utilities.Tree;
using Utilities.Tree.Columns;
using Utilities.Windowing;
using Utilities.XmlSerialisation;
using Utilities.Editor.GUI;

namespace Utilities.Editor {
    /// <summary>
    /// static class for interacting with the editor system
    /// </summary>
    public static class User {
        /// <summary>
        /// Edits the specified obj.
        /// </summary>
        /// <param name="name">name of the object being edited</param>
        /// <param name="obj">The obj to edit</param>
        /// <param name="windowController">Parent object to become a child of - can be null</param>
        /// <param name="setObject">function to set the updated object back into where it belongs</param>
        /// <param name="commandHistory">command history to add the edit command to</param>
        /// <param name="returnXml">return a serialised version of the object or not</param>
        /// <param name="isreadonly">is the object read only?</param>
        /// <returns>true on success</returns>
        public static bool Edit(string name, object obj, WindowController windowController, Func<object, bool> setObject, CommandHistory commandHistory, bool returnXml, bool isreadonly = false ) {

            /*objectiveS:
             * serialise object for editing
             * get a correct list of the columns for editing
             * get a list of validators and displayers.  - ie XPath and validator methods... 
             * Qmark - how do we do the column that shows the value??             
             * Display the object
             * make sure it validates
             * then return the object.              
             */

            //serialise object for editing
            XDocument xml = XDocument.Parse(SerialisationController.Serialize(obj));

            return Edit(name, xml.Root, obj, windowController, setObject, commandHistory, returnXml, isreadonly);

        }

        public static bool Edit(string name, XElement elem, WindowController windowController, Func<object, bool> setObject, CommandHistory commandHistory, bool returnXml, bool isreadonly = false) {            
            object obj = SerialisationController.DeserializeFromXmlWithType(elem.ToString());
            if (obj == null) {
                Logger.FAILURE("Deserialisation failed");
            }
            return Edit(name, XElement.Parse(elem.ToString()), obj, windowController, setObject, commandHistory, returnXml, isreadonly); // tostring so we only get the actual element
        }

        private static bool Edit(string name,XElement elem,object obj, WindowController windowController, Func<object, bool> setObject, CommandHistory commandHistory, bool returnXml, bool isreadonly) {

        //* get a correct list of the columns for editing
             //* get a list of validators and displayers.  - ie XPath and validator methods...             
            Dictionary<string, List<AEditor>> editors = new Dictionary<string, List<AEditor>>();
            Dictionary<string, Func<XElement, string>> displayers = new Dictionary<string, Func<XElement, string>>();
            Dictionary<string, Func<object, string>> validators = new Dictionary<string, Func<object, string>>();
            Dictionary<string, AElementController> controllers = new Dictionary<string, AElementController>();

            GetColumns("", "", elem, obj, editors, displayers, validators, controllers,false);

            List<AxTreeColumn> columns = new List<AxTreeColumn>();

            //create the display column
            columns.Add(new ReadOnlyColumn("Value", element => MatchesPath(element,
                                                                           path => displayers.ContainsKey(path)
                                                                                       ? displayers[path].Invoke(element)
                                                                                       : "-",
                                                                           controller => displayers.ContainsKey(controller.Key)
                                                                                           ? displayers[controller.Key].Invoke(element) 
                                                                                           : "-",
                                                                           controllers))); 

            //add the editor columns - having added a generic constraint on which rows they can display data for
            foreach (KeyValuePair<string, List<AEditor>> aEditors in editors) {
                string parentpath = aEditors.Key;
                foreach (AEditor aeditor in aEditors.Value) {
                    aeditor.AppliesToRow = element => MatchesPath(element,
                                                             path => parentpath.Equals(path),
                                                             controller => false,
                                                             controllers);                    
                }
                columns.AddRange(aEditors.Value);
            }           
 
            // add controller columns
            foreach (KeyValuePair<string, AElementController> controller in controllers) {
                string parentpath = controller.Key;
                foreach (AEditor aeditor in controller.Value.ControlColumns) { 
                    aeditor.AppliesToRow = element => MatchesPath(element,
                                                             path => path.Contains(parentpath) && path.Replace(parentpath,"").Occurrences(Seperator)<=1,
                                                             controllerr => true,
                                                             controllers);
                }
                editors.AddToDictionaryList(controller.Key + Seperator + ListElement, controller.Value.ControlColumns); // now add them to the editors list so they can be displayed
                columns.AddRange(controller.Value.ControlColumns);
            }


            // add custom validation

            foreach (var validator in validators) {
                foreach (AEditor aEditor in editors[validator.Key]) { // todo this is not always right...
                    aEditor.Validators.Add(validator.Value);
                }                
            }

            // create the treeview

            XTreeView treeview = new XTreeView("Editor", "This form allows editing of a given node - in this case a " + obj.GetType(), () => elem, 
                element => MatchesPath(element,                                  path => editors.Any(edtr => path.Contains(edtr.Key))
                                                                                        ? (controllers.Any(ctrlr => path.Contains(ctrlr.Key)) ?
                                                                                            element.Elements().ToList().Where(child =>
                                                                                                MatchesPath<bool>(child,
                                                                                                cpath =>
                                                                                                    editors.ContainsKey(cpath) || displayers.Keys.Any(displayerpath => displayerpath.Contains(cpath)), 
                                                                                                    cpath => true,
                                                                                                controllers)
                                                                                            ).ToList()
                                                                                            : new List<XElement>())                                                                                        
                                                                                        : element.Elements().Where(child => {
                                                                                                                       string childpath = GetPath(child);
                                                                                                                       return displayers.Keys.Any( displayerpath => displayerpath.Contains(childpath) );
                                                                                                                   }
                                                                                              ).ToList(),

                                                                            controller => element.Elements().ToList(),
                                                                            controllers),
                                               new NodeNameColumn("Object", true), columns);
            
            treeview.ShowColumn = (col, row) => {
                                      if (col.Name == "Value") {
                                          return true;
                                      }
                                      XElement element = row.Item as XElement;
                                      if (element != null) {

                                          return MatchesPath(element,
                                                             path => 
                                                                 editors.ContainsKey(path) && editors[path].Contains(col),
                                                             controller => 
                                                                 controller.Value.ControlColumns.Contains(col),
                                                             controllers);
                                      }
                                      return true;
                                  };

            // now show the GUI...

            windowController.Bind("EditWindow", typeof(EditorWindow),
                                  new Dictionary<string, object> {
                                                                     {"Tree", treeview},
                                                                     {"EditController", new EditController(name,(elem as XElement).ToString(),setObject,commandHistory,returnXml, isreadonly)}
                                                                 });                    
            return true;
        }

        private static T MatchesPath<T>(XElement element, Func<string,T> matchPath, Func<KeyValuePair<string,AElementController>,T> matchesController, Dictionary<string, AElementController> controllers) {
            string path = GetPath(element);                                       
            List<KeyValuePair<string, AElementController>> matchingControllers = controllers.Where(ctrler => path.Contains(ctrler.Key)).ToList();

            List<AElementController> processedController =
                new List<AElementController>();

            while (matchingControllers.Count() > 0) {
                path = path.Replace(matchingControllers.First().Key, "");
                if (string.IsNullOrEmpty(path)) { // we've found the root element for list - so just display the controller
                    return matchesController.Invoke(matchingControllers.First());                        
                }
                path = path.Substring(Seperator.Length);
                if (path.Contains(Seperator)) {
                    path = path.Substring(path.IndexOf(Seperator));
                } else {
                    path = string.Empty;
                }
                path = matchingControllers.First().Key + Seperator +
                       ListElement + path;
                processedController.Add(matchingControllers.First().Value);
                matchingControllers =
                    controllers.Where(ctrler => path.Contains(ctrler.Key) &&
                                                !processedController.Contains(ctrler.Value)).ToList();
            }

            return matchPath.Invoke(path);            
        }

        /// <summary>
        /// Gets the path to an object from the root node down to the node
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>the path</returns>
        private static string GetPath(XElement element) {
            string path = string.Empty;
            while (element != null) {
                if (string.IsNullOrEmpty(path)) {
                    path = element.Name.ToString();
                } else {
                    path = element.Name + Seperator + path;
                }
                element = element.Parent;
            }
            return path;
        }

        private static void GetColumns(string path, string name, XElement elem, object obj, Dictionary<string, List<AEditor>> editors,
            Dictionary<string, Func<XElement, string>> displayers, Dictionary<string, Func<object, string>> validators,
            Dictionary<string, AElementController> controllers, bool isListElement) {

            // firstly get the name of what we are trying to find
            if (elem.Attribute("PropertyGroup") != null) {
                if (string.IsNullOrEmpty(path)) {
                    path += elem.Name;
                } else {
                    path += Seperator + elem.Name;
                }
                name += elem.Name + SerialisationController.ChildChar;

                foreach (XElement child in elem.Elements()) {
                    GetColumns(path, name, child, obj, editors, displayers, validators, controllers, false);
                }

                return;
            }

            if (!isListElement) {
                if (string.IsNullOrEmpty(path)) {
                    path += elem.Name;
                } else {
                    path += Seperator + elem.Name;
                }
            }
            name += elem.Name;

            #region now find the item
            // it may be the class itself...
            // which may have been renamed. 
            // if it is a class we should just recurse

            if (obj.GetType().Name == name) {
                foreach (XElement child in elem.Elements()) { // recurse
                    GetColumns(path, "", child, obj, editors, displayers, validators, controllers, false);
                }
                return;
            }
            // its been renamed
            XmlRootAttribute[] rootAttr = (XmlRootAttribute[])obj.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true);
            if (rootAttr != null && rootAttr.Length > 0) {
                if (rootAttr[0].ElementName.Equals(name)) {
                    foreach (XElement child in elem.Elements()) { // recurse
                        GetColumns(path, "", child, obj, editors, displayers, validators, controllers, false);
                    }

                    return;
                }
            }

            if (name == "TYPEDEFINITION") return; // we dont count this

            /*rules             
             * > otherwise it must be a property
             * > it may have been renamed with an XmlArrayAttribute (or its derivatives)
             * > it may have been renamed with an XmlElementAttribute (or its derivatives)
             */

            PropertyInfo property = null;

            PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < propertyInfos.Length && property == null; i++) {
                if (propertyInfos[i].Name.Equals(name)) { // a property
                    property = propertyInfos[i];
                } else {

                    object[] attribs = propertyInfos[i].GetCustomAttributes(true);

                    for (int a = 0; a < attribs.Length && property == null; a++) {
                        {
                            XmlArrayAttribute xmlarrayAttribute = attribs[a] as XmlArrayAttribute; // an XmlArrayAttribute
                            if (xmlarrayAttribute != null && xmlarrayAttribute.ElementName == name) {
                                property = propertyInfos[i];
                            }
                        }
                        {
                            XmlElementAttribute xmlElementAttribute = attribs[a] as XmlElementAttribute; // an XmlElementAttribute
                            if (xmlElementAttribute != null && xmlElementAttribute.ElementName == name) {
                                property = propertyInfos[i];
                            }
                        }
                    }

                }
            }

            if (property == null) {
                Logger.ERROR("Could not find a property called " + name);
                return;
            }

            {
                XmlIgnoreAttribute[] xmlignores =
                    (XmlIgnoreAttribute[]) property.GetCustomAttributes(typeof (XmlIgnoreAttribute), true);
                if (xmlignores.Length > 0) {
                    return;
                }
            }
            {
                EditorIgnoreAttribute[] editorignores =
                    (EditorIgnoreAttribute[])property.GetCustomAttributes(typeof(EditorIgnoreAttribute), true);
                if (editorignores.Length > 0) {
                    return;
                }
            }

            #endregion

            // now decide if it requires recursion
            if (property.PropertyType.IsArray || property.PropertyType.HasElementType || property.PropertyType.IsGenericType) {
                bool isEditable = true;
                Type listType = GetListOrArrayType(property.PropertyType);
                if (listType == null) {
                    Logger.ERROR("unknown array/list type: " + property.PropertyType);
                    return;
                }


                // lists and dictionaries are generic :)                

                ArrayEditorAttribute arrayEditorAttribute = GetAttribute<ArrayEditorAttribute>(property);

                if (arrayEditorAttribute == null) {                    
                    Logger.DEBUG("The following property has no ArrayEditor definition: " + property.Name +
                                    " in class " +
                                    obj.GetType().FullName);
                    return;                
                }

                ReadOnlyAttribute readOnlyAttribute = GetAttribute<ReadOnlyAttribute>(property);

                if (readOnlyAttribute != null) {
                    isEditable = false;
                } 

                //add a list controller                
                AElementController controller = arrayEditorAttribute.GetController();
                controllers.Add(path, controller);

                //add controller editors
                // editors.AddToDictionaryList(path, controller.ControlColumns); - dont do this as they mess with the editor list

                //add a custom displayer
                displayers.Add(path, element => {
                    string text = "Array of type " + property.PropertyType.Name + " {";
                    int numchildren = element.Elements().Count();
                    XElement[] children = element.Elements().ToArray();
                    for (int i = 0; i < 5 && i < numchildren; i++) {
                        if (text.EndsWith(",") || text.EndsWith("{")) {
                            text += children[i].Value;
                        } else {
                            text += "," + children[i].Value;
                        }
                    }
                    if (numchildren > 5) {
                        text += "...";
                    }

                    return text + "}";
                });

                

                 // now consider if the type is a value type or not? 

                if (listType.IsPrimitive || listType.Equals(typeof(string))) {
                    if (isEditable) {
                        //now lets add an editor 
                        AEditorAttribute editor = GetAttribute<AEditorAttribute>(property);
                        if (editor == null) {
                            Logger.DEBUG("The following property has no editor definition: " + property.Name +
                                         " in class " +
                                         obj.GetType().FullName);
                            return;
                        }
                        editors.AddToDictionaryList(path + Seperator + ListElement, editor.GetEditors());
                    }
                    // also add a displayer for the element 
                    displayers.Add(path + Seperator + ListElement, e => e.Value);

                } else {
                    bool classReadonly = listType.GetCustomAttributes(typeof(ReadOnlyClassAttribute), true).Length > 0;

                      if (classReadonly) {
                          displayers.Add(path + Seperator + ListElement, element => {
                                                                             XElement elementWithRightName =
                                                                                 new XElement(element) { Name = listType.Name };
                                                                             if (elementWithRightName.Attribute(XNamespace.Xmlns + "xsi") == null) {
                                                                                 elementWithRightName.Add(new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
                                                                             }

                                                                             if (elementWithRightName.Attribute(XNamespace.Xmlns + "xsd") == null) {
                                                                                 elementWithRightName.Add(new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));
                                                                             }
                                                                             object dispobj =
                                                                                 SerialisationController.Deserialize(
                                                                                     elementWithRightName.ToString(),
                                                                                     listType);
                                                                             if (dispobj == null) {
                                                                                 Logger.FAILURE(
                                                                                     "deserialisation failed for type " +
                                                                                     listType.FullName +
                                                                                     " xml was " +
                                                                                     elementWithRightName.ToString());
                                                                                 return null;
                                                                             }
                                                                             return dispobj.ToString();
                                                                         });
                      } else {
                          // now we choose a default instance of the array/list to work with..
                          object listobj = Activator.CreateInstance(listType);
                          XElement listXml = XElement.Parse(SerialisationController.Serialize(listobj));
                          // this may not serialise correctly...
                          GetColumns(path + Seperator + ListElement, "", listXml, listobj, editors, displayers,
                                     validators,
                                     controllers, true);
                      }
                }


            } else if (property.PropertyType.IsPrimitive || property.PropertyType.Equals(typeof(string))) { // it a primitive
                bool isEditable = true;
                AEditorAttribute editor = GetAttribute<AEditorAttribute>(property);
                if (editor == null) {

                    ReadOnlyAttribute readOnlyAttribute = GetAttribute<ReadOnlyAttribute>(property);

                    if (readOnlyAttribute != null) {
                        isEditable = false;
                    } else {

                        Logger.DEBUG("The following property has no editor definition: " + property.Name +
                                     " in class " +
                                     obj.GetType().FullName);
                        return;
                    }
                }
                if (isEditable) {
                    editors.Add(path, editor.GetEditors());
                }
                displayers.Add(path, e => e.Value);
            } else if (property.PropertyType.IsCustomValueType() || property.PropertyType.IsClass) {

                bool classReadonly = obj.GetType().GetCustomAttributes(typeof (ReadOnlyClassAttribute), true).Length > 0;

                if (classReadonly) {

                    // add a custom displayer - the toString method - need first to De-serialise it - so need to wrap that...                
                    displayers.Add(path, element => {
                        XElement elementWithRightName = new XElement(element) { Name = property.PropertyType.Name };
                        object dispobj =
                            SerialisationController.Deserialize(elementWithRightName.ToString(),
                                                                property.PropertyType);
                        if (dispobj == null) {
                            Logger.FAILURE("deserialisation failed for type " +
                                           property.PropertyType.FullName + " xml was " +
                                           elementWithRightName.ToString());
                        }
                        return dispobj.ToString();
                    });
                } else {

                    // todo add a row validator 

                    // now recurse
                    foreach (XElement child in elem.Elements()) {
                        GetColumns(path, "", child, property.GetValue(obj, new object[] {}), editors, displayers,
                                   validators, controllers, false);
                    }
                }
            } else {
                Logger.ERROR("unknown type: " + property.PropertyType.FullName);
            }

            // now find validators... 

            foreach (MethodInfo m in obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static).Where(
                    mi => mi.Name.EndsWith(property.Name, StringComparison.CurrentCultureIgnoreCase)).Where(
                        mi => mi.GetCustomAttributes(typeof(ValidatorAttribute), true).Length > 0)) {
                MethodInfo validation = m;
                validators.Add(path, objectToValidate => validation.Invoke(null, new[] { objectToValidate }) as string);
            }
        }

        private const string Seperator = "|||";
        private const string ListElement = "LISTELEMENT";

        private static T GetAttribute<T>(PropertyInfo property) where T : Attribute {
            object[] attributes = property.GetCustomAttributes(true);
            if (attributes.Length == 0) {                
                return null;
            }
            List<object> attrs = attributes.Where(attrib => (attrib as T) != null).ToList();
            if ( attrs.Count == 0) {
                return null;
            }
            return attrs[0] as T;
        }

        /// <summary>
        /// Makes a new element of the given type - call default and then edit it
        /// </summary>
        /// <param name="name">name of the type</param>
        /// <param name="type">The type.</param>
        /// <param name="windowController">MDI parent form to add the editor form to</param>
        /// <param name="setObject">function to set the updated object back into where it belongs</param>
        /// <param name="commandHistory">command history to add the edit command to</param>
        /// <param name="returnXml">return a serialised version of the object or not</param>
        /// <returns>an object of the given type or null on failure</returns>
        public static object MakeNew(string name, Type type, WindowController windowController, Func<object, bool> setObject, CommandHistory commandHistory, bool returnXml) {            
            object obj = Activator.CreateInstance(type);
            return Edit(name, obj, windowController, setObject, commandHistory, returnXml);
        }

        private static Type GetListOrArrayType(Type at) {
            if (at.IsArray) {
                return at.GetElementType();
            }
            if (at.IsGenericType) {
                return at.GetGenericArguments()[0];
            }
            return null;
        }

    }

    public static class ReflectionExtensions {
        public static bool IsCustomValueType(this Type type) {            
               return type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System.");
        }
    }

}
