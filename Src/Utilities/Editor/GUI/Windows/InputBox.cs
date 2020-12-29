using System;
using System.Windows.Forms;

namespace Utilities.Editor.GUI.Windows {
    public sealed partial class InputBox : Form {
        private bool NotNull { get; set; }
        private Func<string, string> Validator { get; set; }

        public InputBox(string caption, string message, string value,bool notNull, Func<string,string> validator = null) {            
            InitializeComponent();
            this.Text = caption;
            this.Label.Text = message;
            this.TextBox.Text = value;
            NotNull = notNull;
            this.Validator = validator;
        }

        private void InputBoxFormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel =false;
            if (this.DialogResult == DialogResult.OK) {
                string errorText = this.Validator == null ? "" : this.Validator.Invoke(this.TextBox.Text);
                if (this.NotNull && string.IsNullOrWhiteSpace(this.TextBox.Text)) {
                    errorText += "You must enter a value";
                }
                if (errorText != "") {
                    e.Cancel = true;
                    MessageBox.Show(this, errorText, "Validation Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Focus();
                }
            }

        }

        public string Result {get { return this.TextBox.Text; }}

    }
}
