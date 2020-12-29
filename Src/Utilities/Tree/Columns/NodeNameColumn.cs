using System.Linq;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Tree.Columns {
    /// <summary>
    /// A column which reads the name of the node.
    /// </summary>
    public class NodeNameColumn : AxTreeColumn {

        private readonly bool _readOnly = true;

        public NodeNameColumn(string name,bool readOnly, bool autoSize = true) {
            this.Name = name;
            this.Caption = this.Name;
            this._readOnly = readOnly;
            if (autoSize) {
                this.AutoSizePolicy = ColumnAutoSizePolicy.AutoSize;
                this.MaxAutoSizeWidth = 200;
            }
        }

        #region Overrides of AxTreeColumn

        /// <summary>
        /// Gets the data for a cell
        /// </summary>
        /// <param name="xElement">The x element (ie ROW)</param>
        /// <param name="cellData">The cell data event - should be able to set anything</param>
        /// <returns>success or failure</returns>
        public override bool GetData(XElement xElement, CellData cellData) {
            cellData.Value = xElement.Name;
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

            string newName = newValue as string;

            if (newName == null || string.IsNullOrEmpty(newName) || string.IsNullOrWhiteSpace(newName) || newName.Contains(' ') ) {
                return "An XML node name must be a string with no spaces and may not be blank";
            }

            xElement.Name = newName;
            return null;
        }

        #endregion
    }
}
