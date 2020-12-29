namespace Utilities.Command {
    partial class HistoryViewer {
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
            this.Splitter = new System.Windows.Forms.SplitContainer();
            this.CommandHistoryGroup = new System.Windows.Forms.GroupBox();
            this.CommandHistoryBox = new System.Windows.Forms.TextBox();
            this.UndoItemsSplitter = new System.Windows.Forms.SplitContainer();
            this.UndoStackBoxGroup = new System.Windows.Forms.GroupBox();
            this.UndoStackBox = new System.Windows.Forms.TextBox();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnDeleteHistory = new System.Windows.Forms.Button();
            this.checkFullHistory = new System.Windows.Forms.CheckBox();
            this.BtnClean = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).BeginInit();
            this.Splitter.Panel1.SuspendLayout();
            this.Splitter.Panel2.SuspendLayout();
            this.Splitter.SuspendLayout();
            this.CommandHistoryGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UndoItemsSplitter)).BeginInit();
            this.UndoItemsSplitter.Panel1.SuspendLayout();
            this.UndoItemsSplitter.Panel2.SuspendLayout();
            this.UndoItemsSplitter.SuspendLayout();
            this.UndoStackBoxGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // Splitter
            // 
            this.Splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.Splitter.Location = new System.Drawing.Point(0, 0);
            this.Splitter.Name = "Splitter";
            this.Splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Splitter.Panel1
            // 
            this.Splitter.Panel1.Controls.Add(this.CommandHistoryGroup);
            // 
            // Splitter.Panel2
            // 
            this.Splitter.Panel2.Controls.Add(this.UndoItemsSplitter);
            this.Splitter.Size = new System.Drawing.Size(431, 284);
            this.Splitter.SplitterDistance = 128;
            this.Splitter.TabIndex = 0;
            // 
            // CommandHistoryGroup
            // 
            this.CommandHistoryGroup.Controls.Add(this.CommandHistoryBox);
            this.CommandHistoryGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommandHistoryGroup.Location = new System.Drawing.Point(0, 0);
            this.CommandHistoryGroup.Name = "CommandHistoryGroup";
            this.CommandHistoryGroup.Size = new System.Drawing.Size(431, 128);
            this.CommandHistoryGroup.TabIndex = 1;
            this.CommandHistoryGroup.TabStop = false;
            this.CommandHistoryGroup.Text = "Command History";
            // 
            // CommandHistoryBox
            // 
            this.CommandHistoryBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommandHistoryBox.Location = new System.Drawing.Point(3, 16);
            this.CommandHistoryBox.Multiline = true;
            this.CommandHistoryBox.Name = "CommandHistoryBox";
            this.CommandHistoryBox.Size = new System.Drawing.Size(425, 109);
            this.CommandHistoryBox.TabIndex = 1;
            // 
            // UndoItemsSplitter
            // 
            this.UndoItemsSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndoItemsSplitter.Location = new System.Drawing.Point(0, 0);
            this.UndoItemsSplitter.Name = "UndoItemsSplitter";
            this.UndoItemsSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // UndoItemsSplitter.Panel1
            // 
            this.UndoItemsSplitter.Panel1.Controls.Add(this.UndoStackBoxGroup);
            // 
            // UndoItemsSplitter.Panel2
            // 
            this.UndoItemsSplitter.Panel2.Controls.Add(this.btnRedo);
            this.UndoItemsSplitter.Panel2.Controls.Add(this.btnUndo);
            this.UndoItemsSplitter.Panel2.Controls.Add(this.btnDeleteHistory);
            this.UndoItemsSplitter.Panel2.Controls.Add(this.checkFullHistory);
            this.UndoItemsSplitter.Panel2.Controls.Add(this.BtnClean);
            this.UndoItemsSplitter.Size = new System.Drawing.Size(431, 152);
            this.UndoItemsSplitter.SplitterDistance = 100;
            this.UndoItemsSplitter.TabIndex = 5;
            // 
            // UndoStackBoxGroup
            // 
            this.UndoStackBoxGroup.Controls.Add(this.UndoStackBox);
            this.UndoStackBoxGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndoStackBoxGroup.Location = new System.Drawing.Point(0, 0);
            this.UndoStackBoxGroup.Name = "UndoStackBoxGroup";
            this.UndoStackBoxGroup.Size = new System.Drawing.Size(431, 100);
            this.UndoStackBoxGroup.TabIndex = 0;
            this.UndoStackBoxGroup.TabStop = false;
            this.UndoStackBoxGroup.Text = "Undo Stack (last = most recent)";
            // 
            // UndoStackBox
            // 
            this.UndoStackBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UndoStackBox.Location = new System.Drawing.Point(3, 16);
            this.UndoStackBox.Multiline = true;
            this.UndoStackBox.Name = "UndoStackBox";
            this.UndoStackBox.Size = new System.Drawing.Size(425, 81);
            this.UndoStackBox.TabIndex = 0;
            // 
            // btnRedo
            // 
            this.btnRedo.Location = new System.Drawing.Point(362, 9);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(57, 23);
            this.btnRedo.TabIndex = 9;
            this.btnRedo.Text = "REDO";
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.BtnRedoClick);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(307, 9);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(49, 23);
            this.btnUndo.TabIndex = 8;
            this.btnUndo.Text = "UNDO";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.BtnUndoClick);
            // 
            // btnDeleteHistory
            // 
            this.btnDeleteHistory.Location = new System.Drawing.Point(205, 9);
            this.btnDeleteHistory.Name = "btnDeleteHistory";
            this.btnDeleteHistory.Size = new System.Drawing.Size(96, 23);
            this.btnDeleteHistory.TabIndex = 7;
            this.btnDeleteHistory.Text = "Delete History";
            this.btnDeleteHistory.UseVisualStyleBackColor = true;
            this.btnDeleteHistory.Click += new System.EventHandler(this.BtnDeleteHistoryClick);
            // 
            // checkFullHistory
            // 
            this.checkFullHistory.AutoSize = true;
            this.checkFullHistory.Checked = true;
            this.checkFullHistory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkFullHistory.Location = new System.Drawing.Point(3, 13);
            this.checkFullHistory.Name = "checkFullHistory";
            this.checkFullHistory.Size = new System.Drawing.Size(107, 17);
            this.checkFullHistory.TabIndex = 6;
            this.checkFullHistory.Text = "Show Full History";
            this.checkFullHistory.UseVisualStyleBackColor = true;
            this.checkFullHistory.CheckedChanged += new System.EventHandler(this.CheckFullHistoryCheckedChanged);
            // 
            // BtnClean
            // 
            this.BtnClean.Location = new System.Drawing.Point(116, 9);
            this.BtnClean.Name = "BtnClean";
            this.BtnClean.Size = new System.Drawing.Size(83, 23);
            this.BtnClean.TabIndex = 5;
            this.BtnClean.Text = "Clean History";
            this.BtnClean.UseVisualStyleBackColor = true;
            this.BtnClean.Click += new System.EventHandler(this.BtnCleanClick);
            // 
            // HistoryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 284);
            this.Controls.Add(this.Splitter);
            this.Name = "HistoryViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ViewHistory";
            this.Splitter.Panel1.ResumeLayout(false);
            this.Splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Splitter)).EndInit();
            this.Splitter.ResumeLayout(false);
            this.CommandHistoryGroup.ResumeLayout(false);
            this.CommandHistoryGroup.PerformLayout();
            this.UndoItemsSplitter.Panel1.ResumeLayout(false);
            this.UndoItemsSplitter.Panel2.ResumeLayout(false);
            this.UndoItemsSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UndoItemsSplitter)).EndInit();
            this.UndoItemsSplitter.ResumeLayout(false);
            this.UndoStackBoxGroup.ResumeLayout(false);
            this.UndoStackBoxGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer Splitter;
        private System.Windows.Forms.GroupBox CommandHistoryGroup;
        private System.Windows.Forms.TextBox CommandHistoryBox;
        private System.Windows.Forms.SplitContainer UndoItemsSplitter;
        private System.Windows.Forms.GroupBox UndoStackBoxGroup;
        private System.Windows.Forms.TextBox UndoStackBox;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnDeleteHistory;
        private System.Windows.Forms.CheckBox checkFullHistory;
        private System.Windows.Forms.Button BtnClean;
    }
}