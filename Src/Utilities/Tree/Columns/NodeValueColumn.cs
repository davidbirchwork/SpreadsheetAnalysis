using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Tree.Columns {
    /// <summary>
    /// A column which reads the value of the node if it has no children.
    /// </summary>
    public class NodeValueColumn : AxTreeColumn {
        public string NonTerminalValue { get; set; }

        private readonly bool _readOnly = true;

        public NodeValueColumn(string name, string nonTerminalValue, bool readOnly) {            
            this.Name = name;
            this.Caption = this.Name;
            this.NonTerminalValue = nonTerminalValue;
            this._readOnly = readOnly;
        }

        #region Overrides of AxTreeColumn

        /// <summary>
        /// Gets the data for a cell
        /// </summary>
        /// <param name="xElement">The x element (ie ROW)</param>
        /// <param name="cellData">The cell data event - should be able to set anything</param>
        /// <returns>success or failure</returns>
        public override bool GetData(XElement xElement, CellData cellData) {
            if (xElement.HasElements) {
                cellData.Value = this.NonTerminalValue;
            } else {
                cellData.Value = xElement.Value;
            }
            return true;
        }

        /// <summary>
        /// Sets the value for a cell on an XElement
        /// </summary>
        /// <param name="xElement">The XElement (ie ROW).</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>ERROR MESSAGE - null if no error</returns>
        public override string SetValue(XElement xElement, object oldValue, object newValue) {
            if (this._readOnly) {
                return "This column is read only";
            }

            string newvalue = newValue as string;

            if (newvalue == null) {
                return "An XML node name must be a string with no spaces and may not be blank";
            }
            if (xElement.HasElements) {
                return "Cannot set Node value as this node has children";
            }

            xElement.Value = newvalue;
            return null;
        }

        #endregion
    }
}
