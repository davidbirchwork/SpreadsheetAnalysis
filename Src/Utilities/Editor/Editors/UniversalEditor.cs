using System;
using System.Xml.Linq;
using Infralution.Controls;
using Infralution.Controls.VirtualTree;
using Utilities.Command;
using Utilities.Loggers;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// Enables use of the UniversalEditBox which
    /// Defines an inplace editor control that allows editing of any object type that supports a UITypeEditor.     
    /// </summary>
    /// <typeparam name="T">a type supporting a UITypeEditor</typeparam>
    public class UniversalEditor<T> : AEditor {
        private readonly string _name;
        public Func<XElement, T> Converter;        
        private readonly Func<T, string> _validator;
        public Func<T, XElement, string> Setter;
        private readonly CommandHistory _history;

        private readonly CellEditor _cellEditor;

        #region ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalEditor&lt;T&gt;"/> class.
        /// this enables editing of any type implementing a UITypeEditor
        /// </summary>
        /// <param name="name">The name of the property being edited</param>
        /// <param name="description">A description of the field</param>
        /// <param name="converter">Converter to read the type from an XElement</param>
        /// <param name="validator">A validation function returning a string containing error messages or null if no errors</param>
        /// <param name="setter">The setter writes the value back to the XElement replacing the old value. Validation will have succeeded at this point</param>
        /// <param name="history"></param>
        public UniversalEditor(string name, string description, Func<XElement, T> converter, Func<T, string> validator, Func<T, XElement, string> setter, CommandHistory history) {
            SetName(name);
            this.Description = description;
            if (converter == null) throw new ArgumentNullException("converter");            
            if (validator == null) throw new ArgumentNullException("validator");
            if (setter == null) throw new ArgumentNullException("setter");
            this._name = name;
            this.Converter = converter;            
            this._validator = validator;
            this.Setter = setter;
            this._history = history;
            this._cellEditor = new CellEditor(new UniversalEditBox {ValueType = typeof (T)});
        }

        #endregion

        #region Overrides of AEditor

        protected override bool GetEditorValue(XElement xElement, CellData cellData) {            
            cellData.Editor = this._cellEditor;
            try {
                cellData.Value = this.Converter.Invoke(xElement);
            } catch (Exception e) {
                Logger.DEBUG("Universal Converter Failed for type " + typeof (T).FullName + " on XElement " + xElement + " error: "+e.Message);
                cellData.Value = e.Message;
            }            
            return true;
        }

        protected override string SetEditorValue(XElement xElement, object oldValue, object newValue) {
// ReSharper disable RedundantAssignment
            T newValueT = default(T);
            T oldValueT = default(T);
// ReSharper restore RedundantAssignment
            try {
                newValueT = (T) newValue;
            } catch(Exception e) {
                return "Type conversion failed for type " + typeof (T).FullName + " Error was " + e.Message;
            }
            try {
                oldValueT = (T) oldValue;
            } catch(Exception e) {
                return "Type conversion failed for type " + typeof (T).FullName + " Error was " + e.Message;
            }


            string validatormessage = this._validator.Invoke(newValueT);
            if ( validatormessage != null) {
                return validatormessage;
            }
            if (this._history != null) {
                this._history.Execute(new EditFieldCommand<T>(this._name, xElement, oldValueT, newValueT, this.Setter));
                return null; //we'll assume everything is going ok
            } 
            return this.Setter(newValueT,xElement);
        }

        #endregion
    }
}
