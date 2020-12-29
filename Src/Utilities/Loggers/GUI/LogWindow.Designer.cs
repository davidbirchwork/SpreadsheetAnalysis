using System;

namespace Utilities.Loggers.GUI {
    partial class LogWindow {
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
            this.LogBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LogBox
            // 
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogBox.Location = new System.Drawing.Point(0, 0);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(362, 153);
            this.LogBox.TabIndex = 0;
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 153);
            this.Controls.Add(this.LogBox);
            this.Name = "LogWindow";
            this.Text = "Log Box - Message Appear Here!";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogWindowFormClosed);
            this.Load += new System.EventHandler(this.LogWindowLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LogBox;

    }
}

