namespace Utilities.Editor.Xml {
    partial class XmlEditor {
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
            this.TextBoxSplitter = new System.Windows.Forms.SplitContainer();
            this.textbox = new System.Windows.Forms.TextBox();
            this.btnAccept = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxSplitter)).BeginInit();
            this.TextBoxSplitter.Panel1.SuspendLayout();
            this.TextBoxSplitter.Panel2.SuspendLayout();
            this.TextBoxSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBoxSplitter
            // 
            this.TextBoxSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxSplitter.Location = new System.Drawing.Point(0, 0);
            this.TextBoxSplitter.Name = "TextBoxSplitter";
            this.TextBoxSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // TextBoxSplitter.Panel1
            // 
            this.TextBoxSplitter.Panel1.Controls.Add(this.textbox);
            // 
            // TextBoxSplitter.Panel2
            // 
            this.TextBoxSplitter.Panel2.Controls.Add(this.btnAccept);
            this.TextBoxSplitter.Size = new System.Drawing.Size(284, 262);
            this.TextBoxSplitter.SplitterDistance = 182;
            this.TextBoxSplitter.TabIndex = 0;
            // 
            // textbox
            // 
            this.textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textbox.Location = new System.Drawing.Point(0, 0);
            this.textbox.Multiline = true;
            this.textbox.Name = "textbox";
            this.textbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textbox.Size = new System.Drawing.Size(284, 182);
            this.textbox.TabIndex = 1;
            // 
            // btnAccept
            // 
            this.btnAccept.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAccept.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAccept.Location = new System.Drawing.Point(0, 0);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(284, 76);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.BtnAcceptClick);
            // 
            // XmlEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TextBoxSplitter);
            this.Name = "XmlEditor";
            this.Text = "TextViewer";
            this.TextBoxSplitter.Panel1.ResumeLayout(false);
            this.TextBoxSplitter.Panel1.PerformLayout();
            this.TextBoxSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TextBoxSplitter)).EndInit();
            this.TextBoxSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer TextBoxSplitter;
        private System.Windows.Forms.TextBox textbox;
        private System.Windows.Forms.Button btnAccept;

    }
}