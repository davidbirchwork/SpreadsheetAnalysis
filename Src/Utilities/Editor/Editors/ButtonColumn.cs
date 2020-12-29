using System;
using System.Windows.Forms;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// A Button column which enables custom code
    /// </summary>
    public class ButtonColumn : AEditor {

        private readonly Func<XElement, string> _getText;
        private readonly Func<XElement, bool> _btnOnClick;

        #region ctors

        public ButtonColumn (string name, string description, Func<XElement, string> getText, Func<XElement, bool> btnOnClick) {
            SetName(name);
            this.Description = description;
            this._getText = getText;
            this._btnOnClick = btnOnClick;
        }

        #endregion

        #region Overrides of AxTreeColumn
        
        protected override bool GetEditorValue(XElement xElement, CellData cellData) {
            Button button = new Button { Text = this._getText.Invoke(xElement) };
            cellData.Editor = new CellEditor(button) {
                DisplayMode = CellEditorDisplayMode.Always,
                ValueProperty = button.GetType().GetProperty("Tag")
            };
            cellData.Editor.InitializeControl += EditorInitializeControl;
            return true;
        }

        void EditorInitializeControl(object sender, CellEditorInitializeEventArgs e) {
            if (e.NewControl) {
                Button button = (e.Control) as Button;
                XElement element = e.CellWidget.Row.Item as XElement;
                if (button != null) {
                    button.Click += (clicksender, clickevents) => {
                                        if (this._btnOnClick.Invoke(element)) {
                                            this.Tree.UpdateRows();
                                            Row focusRow = this.Tree.FocusRow;
                                            Row parentRow = focusRow.ParentRow;
                                            focusRow.Expand();
                                            if (parentRow != null) {
                                                Row siblingRow = this.Tree.GetRow(parentRow.LastChildRowIndex);
                                                if (siblingRow != null) {
                                                    siblingRow.Expand();
                                                }
                                            } 
                                        }
                                    };
                    
                }
            }
        }

        protected override string SetEditorValue(XElement xElement, object oldValue, object newValue) {
            return "A button column is read only";
        }

        #endregion        
    }
}
