using Utilities.Tree;

namespace Utilities.Editor.GUI {
    partial class EditorWindow {
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
            this.HozSplitter = new System.Windows.Forms.SplitContainer();
            this.XTreeView = new Utilities.Tree.XVirtualTreeView();
            this.StringEditor = new Infralution.Controls.VirtualTree.CellEditor();
            this.universalEditBox1 = new Infralution.Controls.UniversalEditBox();
            this.rowBindingXElement = new Infralution.Controls.VirtualTree.ObjectRowBinding();
            this.CommandBox = new System.Windows.Forms.GroupBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.HozSplitter)).BeginInit();
            this.HozSplitter.Panel1.SuspendLayout();
            this.HozSplitter.Panel2.SuspendLayout();
            this.HozSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XTreeView)).BeginInit();
            this.CommandBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // HozSplitter
            // 
            this.HozSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HozSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.HozSplitter.Location = new System.Drawing.Point(0, 0);
            this.HozSplitter.Name = "HozSplitter";
            this.HozSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // HozSplitter.Panel1
            // 
            this.HozSplitter.Panel1.Controls.Add(this.XTreeView);
            this.HozSplitter.Panel1.Controls.Add(this.universalEditBox1);
            // 
            // HozSplitter.Panel2
            // 
            this.HozSplitter.Panel2.Controls.Add(this.CommandBox);
            this.HozSplitter.Size = new System.Drawing.Size(336, 312);
            this.HozSplitter.SplitterDistance = 168;
            this.HozSplitter.TabIndex = 0;
            // 
            // XTreeView
            // 
            this.XTreeView.AllowMultiSelect = false;
            this.XTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XTreeView.EditOnDoubleClick = true;
            this.XTreeView.Editors.Add(this.StringEditor);
            this.XTreeView.Location = new System.Drawing.Point(0, 0);
            this.XTreeView.Name = "XTreeView";
            this.XTreeView.RowBindings.Add(this.rowBindingXElement);
            this.XTreeView.Size = new System.Drawing.Size(336, 168);
            this.XTreeView.TabIndex = 5;
            this.XTreeView.XTree = null;
            // 
            // StringEditor
            // 
            this.StringEditor.Control = this.universalEditBox1;
            // 
            // universalEditBox1
            // 
            this.universalEditBox1.Location = new System.Drawing.Point(0, 0);
            this.universalEditBox1.Name = "universalEditBox1";
            this.universalEditBox1.Size = new System.Drawing.Size(195, 20);
            this.universalEditBox1.TabIndex = 4;
            this.universalEditBox1.Visible = false;
            // 
            // rowBindingXElement
            // 
            this.rowBindingXElement.Name = "rowBindingXElement";
            this.rowBindingXElement.ParentProperty = "Parent";
            this.rowBindingXElement.TypeName = "System.Xml.Linq.XElement";
            // 
            // CommandBox
            // 
            this.CommandBox.Controls.Add(this.btnApply);
            this.CommandBox.Controls.Add(this.logBox);
            this.CommandBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommandBox.Location = new System.Drawing.Point(0, 0);
            this.CommandBox.Name = "CommandBox";
            this.CommandBox.Size = new System.Drawing.Size(336, 140);
            this.CommandBox.TabIndex = 0;
            this.CommandBox.TabStop = false;
            this.CommandBox.Text = "Editor Options:";
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.logBox.Location = new System.Drawing.Point(3, 16);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(330, 88);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "Validation Messages Will Appear Here";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(3, 108);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(332, 31);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApplyClick);
            // 
            // EditorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 312);
            this.Controls.Add(this.HozSplitter);
            this.Name = "EditorWindow";
            this.Text = "Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditorWindowFormClosed);
            this.Load += new System.EventHandler(this.EditorWindowLoad);
            this.HozSplitter.Panel1.ResumeLayout(false);
            this.HozSplitter.Panel1.PerformLayout();
            this.HozSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HozSplitter)).EndInit();
            this.HozSplitter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.XTreeView)).EndInit();
            this.CommandBox.ResumeLayout(false);
            this.CommandBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer HozSplitter;
        private System.Windows.Forms.GroupBox CommandBox;
        private XVirtualTreeView XTreeView;
        private Infralution.Controls.VirtualTree.CellEditor StringEditor;
        private Infralution.Controls.UniversalEditBox universalEditBox1;
        private Infralution.Controls.VirtualTree.ObjectRowBinding rowBindingXElement;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button btnApply;
    }
}