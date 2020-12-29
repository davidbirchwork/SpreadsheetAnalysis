using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Utilities.Windowing {

    [Export(typeof(IWindow))]
    [WindowAttribute(WindowTypeName = "Text Window",
        WindowTypeDescription = "A Window to display arbitrary text.",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class TextViewer : Form, IWindow{
        public TextViewer() {
            InitializeComponent();
        }

        #region Implementation of IWindow

        public string GetInstanceName() {
            return "Text Window";
        }

        public string GetInstanceDescription() {
            return "A Window to display arbitrary text.";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }

        private Func<String> _caption;
        private Func<string> _textFunc;

        public void BindTo(Dictionary<string, object> objectList) {
            this._caption = objectList["Caption"] as Func<String>;
            if (this._caption == null) {
                this.Text = (objectList["Caption"] as string) ?? "Text Window - pass a 'Caption' string to display";
            } else {
                this.Text = this._caption.Invoke();
            }

            this._textFunc = objectList["Text"] as Func<String>;
            if (this._textFunc == null) {
                this.textbox.Text = (objectList["Text"] as string) ?? "Pass a 'Text' object to be displayed";
            } else {
                this.textbox.Text = this._textFunc.Invoke();
            }            
        }        

        public void RefreshView() {
            if (this._caption != null) {
                this.Text = this._caption.Invoke();
            }
            if (this._textFunc != null)  {
                this.textbox.Text = this._textFunc.Invoke();
            }    
        }

        #endregion
    }
}
