using System;
using System.Windows.Forms;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;
using Utilities.Command;

namespace Utilities.Editor.Editors {
    public class BooleanEditor : AEditor {

        public CommandHistory History { get; set; }
        public Action<XElement> OnSetTrue;
        public Action<XElement> OnSetFalse;
        public Func<XElement, bool> GetValue;
        private readonly string _name;
        private readonly bool _defaultValue;

        #region ctors

        public BooleanEditor(string name, string description, bool defaultValue) {
            SetName(name);
            this.Description = description;
            this._name = name;
            this._defaultValue = defaultValue;
        }

        #endregion

        #region Overrides of AEditor

        protected override bool GetEditorValue(XElement xElement, CellData cellData) {
            CheckBox checkbox = new CheckBox() { Checked = _defaultValue};
            if (this.GetValue != null) {
                checkbox.Checked = this.GetValue.Invoke(xElement);
            } else {
                checkbox.Checked = Boolean.Parse(xElement.Value);
            }
            cellData.Editor = new CellEditor(checkbox) {
                DisplayMode = CellEditorDisplayMode.Always,
                ValueProperty = checkbox.GetType().GetProperty("Tag")
            };
            cellData.Editor.InitializeControl += EditorInitializeControl;
            return true;
        }

        void EditorInitializeControl(object sender, CellEditorInitializeEventArgs e) {
            if (e.NewControl) {
                CheckBox checkbox = (e.Control) as CheckBox;
                XElement element = e.CellWidget.Row.Item as XElement;
                if (checkbox != null) {
                    checkbox.CheckedChanged += (clicksender, clickevents) => {
                                                    
                                                   if (this.History != null) {
                                                       this.History.Execute(new EditFieldCommand<bool>(this._name,
                                                                                                       element, 
                                                                                                       !checkbox.Checked,
                                                                                                       checkbox.Checked,
                                                                                                       CallInvokers));
                                                   } else {
                                                       CallInvokers(checkbox.Checked, element);
                                                   }
                                                   this.Tree.UpdateRowData(this.Tree.FocusRow); // refresh row
                                               };

                }
            }
        }        

        private string CallInvokers(bool value,XElement element) {
            if (value) {
                if (this.OnSetTrue != null) {
                    this.OnSetTrue.Invoke(element);
                    return null;
                } else {
                    if (element != null) {
                        element.SetValue("true");
                        return null;
                    } 
                }
            } else {
                if (this.OnSetFalse != null) {
                    this.OnSetFalse.Invoke(element);
                    return null;
                } else {
                    if (element != null) {
                        element.SetValue("false");
                        return null;
                    }
                }
            }

            return "ERROR could not set bool value";
        }

        protected override string SetEditorValue(XElement xElement, object oldValue, object newValue) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
