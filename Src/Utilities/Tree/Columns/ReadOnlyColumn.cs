using System;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Tree.Columns {
    /// <summary>
    /// a read only column with a custom reader function
    /// </summary>
    public class ReadOnlyColumn : AxTreeColumn {
        private readonly Func<XElement, string> _reader;

        public ReadOnlyColumn(string name,Func<XElement,string> reader, bool autoSize = true) {
            this.Name = name;
            this.Caption = this.Name;
            this.Description = "displays the "+name + " of each tree item";
            this._reader = reader;
            if (autoSize) {
                this.AutoSizePolicy = ColumnAutoSizePolicy.AutoSize;
                this.MaxAutoSizeWidth = 200;
            }
        }        

        public override bool GetData(XElement xElement, CellData cellData) {
            cellData.Value = this._reader.Invoke(xElement);
            return true;
        }

        public override string SetValue(XElement xElement, object oldValue, object newValue) {
            return "This Column is read only";
        }
    }
}
