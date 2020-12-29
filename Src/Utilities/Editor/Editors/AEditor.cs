using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;
using Utilities.Tree.Columns;

namespace Utilities.Editor.Editors {

    /// <summary>
    /// This class is the base of the editors which are tree columns of the TreeView
    /// </summary>
    public abstract class AEditor : AxTreeColumn {

        /// <summary>
        /// A List of validators to call
        /// </summary>
        public List<Func<object, string>> Validators = new List<Func<object,string>>();        

        private string Validate(object obj) {
            string error = null;
            for (int i = 0; i < this.Validators.Count && error == null; i++) {
                error = this.Validators[i].Invoke(obj);
            }
            return error;
        }

        /// <summary>
        /// Does this editor apply to this row - to not to use this functionality leave this null
        /// </summary>
        public Func<XElement, bool> AppliesToRow;

        /// <summary>
        /// Gets the data for a cell
        /// </summary>
        /// <param name="xElement">The x element (ie ROW)</param>
        /// <param name="cellData">The cell data event - should be able to set anything</param>
        /// <returns>success or failure</returns>
        public sealed override bool GetData(XElement xElement, CellData cellData) {           
            return this.AppliesToRow == null
                       ? this.GetEditorValue(xElement, cellData)
                       : this.AppliesToRow.Invoke(xElement) ? 
                            this.GetEditorValue(xElement, cellData) 
                            : true; // ie this row doesn't accept this xElement however we have handled the event
        }

        public sealed override string SetValue(XElement xElement, object oldValue, object newValue) {
            return this.AppliesToRow == null || this.AppliesToRow.Invoke(xElement)
                       ? Validate(newValue) ?? this.SetEditorValue(xElement, oldValue, newValue)
                       : null;
        }

        protected abstract bool GetEditorValue(XElement xElement, CellData cellData);
        protected abstract string SetEditorValue(XElement xElement, object oldValue, object newValue);
    }
}
