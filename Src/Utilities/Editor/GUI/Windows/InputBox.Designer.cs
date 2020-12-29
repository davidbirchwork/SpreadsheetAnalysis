namespace Utilities.Editor.GUI.Windows {
    sealed partial class InputBox {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnCancel = new System.Windows.Forms.Button();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.Label = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(3, 48);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(122, 24);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // TextBox
            // 
            this.TextBox.Location = new System.Drawing.Point(3, 22);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(256, 20);
            this.TextBox.TabIndex = 1;
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(0, 6);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(35, 13);
            this.Label.TabIndex = 2;
            this.Label.Text = "label1";
            // 
            // btnAccept
            // 
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(131, 48);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(128, 24);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // InputBox
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(260, 73);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.btnCancel);
            this.Name = "InputBox";
            this.Text = "InputBox";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputBoxFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox TextBox;
        private System.Windows.Forms.Label Label;
        private System.Windows.Forms.Button btnAccept;
    }
}