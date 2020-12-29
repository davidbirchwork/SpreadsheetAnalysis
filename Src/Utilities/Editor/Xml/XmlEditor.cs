using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Utilities.Command;
using Utilities.Loggers;
using Utilities.Windowing;

namespace Utilities.Editor.Xml {

    [Export(typeof(IWindow))]
    [WindowAttribute(WindowTypeName = "Xml Editor Window",
        WindowTypeDescription = "A Window to Edit XML.",
        InstanceRequired = false,
        AllowMultiple = true)]
    public partial class XmlEditor : Form, IWindow{

        public XmlEditor() {
            InitializeComponent();
        }

        private Func<String> _caption;
        private Func<String> _textFunc;
        private string _originalText;
        private bool _returnXml = true;
        private CommandHistory _commandHistory;
        private Func<object, bool> _onSetObjectAction;

        #region Implementation of IWindow

        public string GetInstanceName() {
            return this.Text;
        }

        public string GetInstanceDescription() {
            return "A Window to edit XML / other text";
        }

        public XElement SaveAsXML() {
            throw new NotImplementedException();
        }

        public XElement LoadFromXML() {
            throw new NotImplementedException();
        }       

        public void BindTo(Dictionary<string, object> objectList) {
            this._caption = objectList["Caption"] as Func<String>;
            if (this._caption == null) {
                this.Text = (objectList["Caption"] as string) ?? "Text Window - pass a 'Caption' string to display";
            } else {
                this.Text = this._caption.Invoke();
            }            

            this._textFunc = objectList["Xml"] as Func<string>;
            if (this._textFunc == null) {
                this.textbox.Text = FormatXML(objectList["Xml"] as string) ?? "Pass an 'Xml' object to be displayed";
            } else {
                this.textbox.Text = FormatXML(this._textFunc.Invoke());
            }
            this._originalText = this.textbox.Text;
            
            if (!objectList.ContainsKey("ReturnXml")) {
                Logger.FAILURE("You must pass a boolean ReturnXml to the XMLEditor");                
            } else {
                this._returnXml = (bool) objectList["ReturnXml"];
            }

            if (!objectList.ContainsKey("CommandHistory")) {
                Logger.FAILURE("You must pass a CommandHistory object to the XMLEditor");
            } else {
                this._commandHistory = (CommandHistory)objectList["CommandHistory"];
            }

            if (!objectList.ContainsKey("OnSetObjectAction")) {
                Logger.FAILURE("You must pass a Func<object, bool> OnSetObjectAction object to the XMLEditor");
            } else {
                this._onSetObjectAction = (Func<object, bool>) objectList["OnSetObjectAction"];
            }           
        }

        private static string FormatXML(string unformattedXml) {
            // first read the xml ignoring whitespace
            XmlReaderSettings readeroptions= new XmlReaderSettings {IgnoreWhitespace = true};
            XmlReader reader = XmlReader.Create(new StringReader(unformattedXml),readeroptions);
           
            // then write it out with indentation
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xmlSettingsWithIndentation = new XmlWriterSettings {
                                                                                     Indent = true,
                                                                                     OmitXmlDeclaration = true
                                                                                 };
            using (XmlWriter writer = XmlWriter.Create(sb, xmlSettingsWithIndentation)) {
                writer.WriteNode(reader, true);
            }
            
            return sb.ToString();
        }

        public void RefreshView() {
            if (this._caption != null) {
                this.Text = this._caption.Invoke();
            }
            if (this._textFunc != null)  {
                this.textbox.Text = FormatXML(this._textFunc.Invoke());
                this._originalText = this.textbox.Text;
            }    
        }

        #endregion

        private void BtnAcceptClick(object sender, EventArgs e) {
            EditCommand editCommand = new EditCommand(this.Text,
                                                       this._originalText,
                                                       this.textbox.Text,
                                                       this._onSetObjectAction,
                                                       this._returnXml);            
            if (this._commandHistory == null) {                
                    Task.Factory.StartNew(() =>
                                          editCommand.Execute());
                    Logger.DEBUG("just launched an edit command without a history - did you mean to provide a history?");                
            } else {
                Task.Factory.StartNew(() =>
                this._commandHistory.Execute(editCommand));
            }
            this.Close();
        }
    }
}
