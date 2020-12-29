using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Tree.Columns {
    /// <summary>
    /// A column which reads the value of a given child element of the node.
    /// </summary>
    public class ElementValueColumn : AxTreeColumn {
        private readonly string _elementName;
        private readonly string _defaultValue;
        private readonly bool _readOnly = true;


        public ElementValueColumn(string columnName, string elementName,string defaultValue, bool readOnly) {
            this.Name = columnName;
            this.Caption = this.Name;
            this._elementName = elementName;
            this._defaultValue = defaultValue;
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
            XElement child = xElement.Element(this._elementName);
            if (child == null) {
                cellData.Value = this._defaultValue;
            } else {
                cellData.Value = child.Value;
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

            string newName = newValue as string;

            if (newName == null || string.IsNullOrEmpty(newName)) {
                return "An XML node name must be a string with no spaces and may not be blank";
            }

            XElement child = xElement.Element(this._elementName);
            if (child == null) {
                xElement.Add(new XElement(this._elementName, newValue));
            } else {
                child.Value = newValue.ToString();
            }

            return null;
        }

        #endregion
    }
}
