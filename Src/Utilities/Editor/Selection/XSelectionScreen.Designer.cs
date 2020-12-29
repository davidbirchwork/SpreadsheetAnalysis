using Utilities.Tree;

namespace Utilities.Editor.Selection {
    partial class XSelectionScreen {
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.selectionTree = new Utilities.Tree.XVirtualTreeView();
            this.btnDeslectChildren = new System.Windows.Forms.Button();
            this.btnSelectChidlren = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectionTree)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.selectionTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnDeslectChildren);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectChidlren);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnAccept);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectNone);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectAll);
            this.splitContainer1.Size = new System.Drawing.Size(239, 270);
            this.splitContainer1.SplitterDistance = 194;
            this.splitContainer1.TabIndex = 0;
            // 
            // selectionTree
            // 
            this.selectionTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectionTree.Location = new System.Drawing.Point(0, 0);
            this.selectionTree.Name = "selectionTree";
            this.selectionTree.Size = new System.Drawing.Size(239, 194);
            this.selectionTree.TabIndex = 0;
            this.selectionTree.XTree = null;
            // 
            // btnDeslectChildren
            // 
            this.btnDeslectChildren.Location = new System.Drawing.Point(78, 27);
            this.btnDeslectChildren.Name = "btnDeslectChildren";
            this.btnDeslectChildren.Size = new System.Drawing.Size(75, 44);
            this.btnDeslectChildren.TabIndex = 5;
            this.btnDeslectChildren.Text = "De-Select Children";
            this.btnDeslectChildren.UseVisualStyleBackColor = true;
            this.btnDeslectChildren.Click += new System.EventHandler(this.BtnDeslectChildrenClick);
            // 
            // btnSelectChidlren
            // 
            this.btnSelectChidlren.Location = new System.Drawing.Point(3, 27);
            this.btnSelectChidlren.Name = "btnSelectChidlren";
            this.btnSelectChidlren.Size = new System.Drawing.Size(75, 44);
            this.btnSelectChidlren.TabIndex = 4;
            this.btnSelectChidlren.Text = "Select Children";
            this.btnSelectChidlren.UseVisualStyleBackColor = true;
            this.btnSelectChidlren.Click += new System.EventHandler(this.BtnSelectChidlrenClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(159, 36);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(159, 12);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 28);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.BtnAcceptClick);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(78, 2);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(75, 28);
            this.btnSelectNone.TabIndex = 1;
            this.btnSelectNone.Text = "Select None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.BtnSelectNoneClick);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(3, 2);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 28);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.BtnSelectAllClick);
            // 
            // XSelectionScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 270);
            this.Controls.Add(this.splitContainer1);
            this.Name = "XSelectionScreen";
            this.Text = "XSelectionScreen";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectionTree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private XVirtualTreeView selectionTree;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeslectChildren;
        private System.Windows.Forms.Button btnSelectChidlren;
    }
}