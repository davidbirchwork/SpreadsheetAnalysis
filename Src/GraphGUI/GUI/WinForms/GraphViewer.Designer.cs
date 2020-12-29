namespace GraphGUI.GUI.WinForms {
    partial class GraphViewer {
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
            this.graphHost = new System.Windows.Forms.Integration.ElementHost();
            this.viewer = new GraphGUI.GUI.WPF.Viewer();
            this.SuspendLayout();
            // 
            // graphHost
            // 
            this.graphHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphHost.Location = new System.Drawing.Point(0, 0);
            this.graphHost.Name = "graphHost";
            this.graphHost.Size = new System.Drawing.Size(1148, 638);
            this.graphHost.TabIndex = 0;
            this.graphHost.Text = "elementHost1";
            this.graphHost.Child = this.viewer;
            // 
            // GraphViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 638);
            this.Controls.Add(this.graphHost);
            this.DoubleBuffered = true;
            this.Name = "GraphViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GraphViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost graphHost;
        private GraphGUI.GUI.WPF.Viewer viewer;
    }
}