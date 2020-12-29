namespace ParseTreeExtractor
{
    partial class ParseTreeExtractor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.taberror = new System.Windows.Forms.TabControl();
            this.tablog = new System.Windows.Forms.TabPage();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.taberrors = new System.Windows.Forms.TabPage();
            this.errorlogbox = new System.Windows.Forms.TextBox();
            this.tabtree = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.taberror.SuspendLayout();
            this.tablog.SuspendLayout();
            this.taberrors.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFileName);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 554);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Commands";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(7, 30);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(178, 22);
            this.txtFileName.TabIndex = 2;
            this.txtFileName.Text = "sample.xlsx";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(39, 59);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 28);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "start Parsing";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click_1);
            // 
            // taberror
            // 
            this.taberror.Controls.Add(this.tablog);
            this.taberror.Controls.Add(this.taberrors);
            this.taberror.Controls.Add(this.tabtree);
            this.taberror.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taberror.Location = new System.Drawing.Point(200, 0);
            this.taberror.Margin = new System.Windows.Forms.Padding(4);
            this.taberror.Name = "taberror";
            this.taberror.SelectedIndex = 0;
            this.taberror.Size = new System.Drawing.Size(867, 554);
            this.taberror.TabIndex = 4;
            // 
            // tablog
            // 
            this.tablog.Controls.Add(this.LogBox);
            this.tablog.Location = new System.Drawing.Point(4, 25);
            this.tablog.Margin = new System.Windows.Forms.Padding(4);
            this.tablog.Name = "tablog";
            this.tablog.Padding = new System.Windows.Forms.Padding(4);
            this.tablog.Size = new System.Drawing.Size(859, 525);
            this.tablog.TabIndex = 1;
            this.tablog.Text = "Log";
            this.tablog.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogBox.Location = new System.Drawing.Point(4, 4);
            this.LogBox.Margin = new System.Windows.Forms.Padding(4);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(851, 517);
            this.LogBox.TabIndex = 10;
            // 
            // taberrors
            // 
            this.taberrors.Controls.Add(this.errorlogbox);
            this.taberrors.Location = new System.Drawing.Point(4, 25);
            this.taberrors.Margin = new System.Windows.Forms.Padding(4);
            this.taberrors.Name = "taberrors";
            this.taberrors.Padding = new System.Windows.Forms.Padding(4);
            this.taberrors.Size = new System.Drawing.Size(859, 525);
            this.taberrors.TabIndex = 2;
            this.taberrors.Text = "Error Log";
            this.taberrors.UseVisualStyleBackColor = true;
            // 
            // errorlogbox
            // 
            this.errorlogbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorlogbox.Location = new System.Drawing.Point(4, 4);
            this.errorlogbox.Margin = new System.Windows.Forms.Padding(4);
            this.errorlogbox.Multiline = true;
            this.errorlogbox.Name = "errorlogbox";
            this.errorlogbox.Size = new System.Drawing.Size(851, 517);
            this.errorlogbox.TabIndex = 11;
            // 
            // tabtree
            // 
            this.tabtree.Location = new System.Drawing.Point(4, 25);
            this.tabtree.Margin = new System.Windows.Forms.Padding(4);
            this.tabtree.Name = "tabtree";
            this.tabtree.Padding = new System.Windows.Forms.Padding(4);
            this.tabtree.Size = new System.Drawing.Size(859, 525);
            this.tabtree.TabIndex = 0;
            this.tabtree.Text = "Tree";
            this.tabtree.UseVisualStyleBackColor = true;
            // 
            // ParseTreeExtractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.taberror);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ParseTreeExtractor";
            this.Text = "Parse Tree Extractor";
            this.Load += new System.EventHandler(this.ParseTreeExtractor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.taberror.ResumeLayout(false);
            this.tablog.ResumeLayout(false);
            this.tablog.PerformLayout();
            this.taberrors.ResumeLayout(false);
            this.taberrors.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabControl taberror;
        private System.Windows.Forms.TabPage tablog;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.TabPage taberrors;
        private System.Windows.Forms.TextBox errorlogbox;
        private System.Windows.Forms.TabPage tabtree;
        private System.Windows.Forms.TextBox txtFileName;
    }
}

