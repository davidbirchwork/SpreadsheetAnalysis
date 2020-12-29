namespace Utilities.Tree {
    partial class TreeGui {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeGui));
            this.TreeHeaderMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sortAscendingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortDescendingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator1MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.bestFitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bestFitAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoFitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator2MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.pinnedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separator3MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.showColumnsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.XTreeView = new Utilities.Tree.XVirtualTreeView();
            this.universalEditBox1 = new Infralution.Controls.UniversalEditBox();
            this.StringEditor = new Infralution.Controls.VirtualTree.CellEditor();
            this.rowBindingXElement = new Infralution.Controls.VirtualTree.ObjectRowBinding();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TreeHeaderMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XTreeView)).BeginInit();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeHeaderMenu
            // 
            this.TreeHeaderMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortAscendingMenuItem,
            this.sortDescendingMenuItem,
            this.separator1MenuItem,
            this.bestFitMenuItem,
            this.bestFitAllMenuItem,
            this.autoFitMenuItem,
            this.separator2MenuItem,
            this.pinnedMenuItem,
            this.separator3MenuItem,
            this.showColumnsMenuItem,
            this.customizeMenuItem});
            this.TreeHeaderMenu.Name = "TreeHeaderMenu";
            this.TreeHeaderMenu.Size = new System.Drawing.Size(185, 198);
            // 
            // sortAscendingMenuItem
            // 
            this.sortAscendingMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sortAscendingMenuItem.Image")));
            this.sortAscendingMenuItem.Name = "sortAscendingMenuItem";
            this.sortAscendingMenuItem.Size = new System.Drawing.Size(184, 22);
            this.sortAscendingMenuItem.Tag = "sortAscendingMenuItem";
            this.sortAscendingMenuItem.Text = "Sort Ascending";
            // 
            // sortDescendingMenuItem
            // 
            this.sortDescendingMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sortDescendingMenuItem.Image")));
            this.sortDescendingMenuItem.Name = "sortDescendingMenuItem";
            this.sortDescendingMenuItem.Size = new System.Drawing.Size(184, 22);
            this.sortDescendingMenuItem.Tag = "sortDescendingMenuItem";
            this.sortDescendingMenuItem.Text = "Sort Descending";
            // 
            // separator1MenuItem
            // 
            this.separator1MenuItem.Name = "separator1MenuItem";
            this.separator1MenuItem.Size = new System.Drawing.Size(181, 6);
            // 
            // bestFitMenuItem
            // 
            this.bestFitMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("bestFitMenuItem.Image")));
            this.bestFitMenuItem.Name = "bestFitMenuItem";
            this.bestFitMenuItem.Size = new System.Drawing.Size(184, 22);
            this.bestFitMenuItem.Tag = "bestFitMenuItem";
            this.bestFitMenuItem.Text = "Best Fit";
            // 
            // bestFitAllMenuItem
            // 
            this.bestFitAllMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("bestFitAllMenuItem.Image")));
            this.bestFitAllMenuItem.Name = "bestFitAllMenuItem";
            this.bestFitAllMenuItem.Size = new System.Drawing.Size(184, 22);
            this.bestFitAllMenuItem.Tag = "bestFitAllMenuItem";
            this.bestFitAllMenuItem.Text = "Best Fit All";
            // 
            // autoFitMenuItem
            // 
            this.autoFitMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("autoFitMenuItem.Image")));
            this.autoFitMenuItem.Name = "autoFitMenuItem";
            this.autoFitMenuItem.Size = new System.Drawing.Size(184, 22);
            this.autoFitMenuItem.Tag = "autoFitMenuItem";
            this.autoFitMenuItem.Text = "Auto Fit (No Scroll)";
            // 
            // separator2MenuItem
            // 
            this.separator2MenuItem.Name = "separator2MenuItem";
            this.separator2MenuItem.Size = new System.Drawing.Size(181, 6);
            // 
            // pinnedMenuItem
            // 
            this.pinnedMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pinnedMenuItem.Image")));
            this.pinnedMenuItem.Name = "pinnedMenuItem";
            this.pinnedMenuItem.Size = new System.Drawing.Size(184, 22);
            this.pinnedMenuItem.Tag = "pinnedMenuItem";
            this.pinnedMenuItem.Text = "Pinned";
            // 
            // separator3MenuItem
            // 
            this.separator3MenuItem.Name = "separator3MenuItem";
            this.separator3MenuItem.Size = new System.Drawing.Size(181, 6);
            // 
            // showColumnsMenuItem
            // 
            this.showColumnsMenuItem.Name = "showColumnsMenuItem";
            this.showColumnsMenuItem.Size = new System.Drawing.Size(184, 22);
            this.showColumnsMenuItem.Tag = "showColumnsMenuItem";
            this.showColumnsMenuItem.Text = "Show/Hide Columns";
            // 
            // customizeMenuItem
            // 
            this.customizeMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("customizeMenuItem.Image")));
            this.customizeMenuItem.Name = "customizeMenuItem";
            this.customizeMenuItem.Size = new System.Drawing.Size(184, 22);
            this.customizeMenuItem.Tag = "customizeMenuItem";
            this.customizeMenuItem.Text = "Column Chooser";
            // 
            // XTreeView
            // 
            this.XTreeView.AllowMultiSelect = false;
            this.XTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.XTreeView.EditOnDoubleClick = true;
            this.XTreeView.Location = new System.Drawing.Point(0, 24);
            this.XTreeView.Name = "XTreeView";
            this.XTreeView.Size = new System.Drawing.Size(251, 117);
            this.XTreeView.TabIndex = 3;
            this.XTreeView.XTree = null;
            // 
            // universalEditBox1
            // 
            this.universalEditBox1.Location = new System.Drawing.Point(0, 0);
            this.universalEditBox1.Name = "universalEditBox1";
            this.universalEditBox1.Size = new System.Drawing.Size(195, 20);
            this.universalEditBox1.TabIndex = 2;
            this.universalEditBox1.Visible = false;
            // 
            // StringEditor
            // 
            this.StringEditor.Control = this.universalEditBox1;
            // 
            // rowBindingXElement
            // 
            this.rowBindingXElement.Name = "rowBindingXElement";
            this.rowBindingXElement.ParentProperty = "Parent";
            this.rowBindingXElement.TypeName = "System.Xml.Linq.XElement";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(251, 24);
            this.MainMenu.TabIndex = 4;
            this.MainMenu.Text = "menuStrip1";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItemClick);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItemClick);
            // 
            // TreeGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 141);
            this.Controls.Add(this.XTreeView);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.universalEditBox1);
            this.MainMenuStrip = this.MainMenu;
            this.Name = "TreeGui";
            this.Text = "TreeGui";
            this.TreeHeaderMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.XTreeView)).EndInit();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip TreeHeaderMenu;
        private System.Windows.Forms.ToolStripMenuItem sortAscendingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortDescendingMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestFitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bestFitAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoFitMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem pinnedMenuItem;
        private System.Windows.Forms.ToolStripSeparator separator3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColumnsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeMenuItem;
        private XVirtualTreeView XTreeView;
        private Infralution.Controls.UniversalEditBox universalEditBox1;
        private Infralution.Controls.VirtualTree.CellEditor StringEditor;
        private Infralution.Controls.VirtualTree.ObjectRowBinding rowBindingXElement;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;

    }
}