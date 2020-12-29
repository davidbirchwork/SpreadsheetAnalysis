using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Tree.Columns {

    /// <summary>
    /// An abstract class to implement to provide a column for viewing
    /// </summary>
    public abstract class AxTreeColumn : Column {

        /// <summary>
        /// Gets or sets the description to display as a tooltip
        /// </summary>
        /// <value>The description.</value>
        public string Description {
            get {
                return this._description;
            }
            set {
                this.ToolTip = value;                
                this._description = value;
            }
        }

        private string _description;

        public void SetName(string name) {
            this.Caption = name;
            this.Name = name;
        }

        /// <summary>
        /// Gets the data for a cell
        /// </summary>
        /// <param name="xElement">The x element (ie ROW)</param>
        /// <param name="cellData">The cell data event - should be able to set anything</param>
        /// <returns>success or failure</returns>
        public abstract bool GetData(XElement xElement, CellData cellData);

        /// <summary>
        /// Sets the value for a cell on an XElement
        /// </summary>
        /// <param name="xElement">The XElement (ie ROW).</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>ERROR MESSAGE - null if no error</returns>
        public abstract string SetValue(XElement xElement, object oldValue, object newValue);        
    }
    
}
