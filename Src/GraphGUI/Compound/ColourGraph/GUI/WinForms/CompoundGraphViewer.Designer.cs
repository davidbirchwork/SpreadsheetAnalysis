namespace GraphGUI.Compound.ColourGraph.GUI.WinForms {
    partial class CompoundGraphViewer {
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
            this.GraphDisplay = new System.Windows.Forms.Integration.ElementHost();
            this.GraphViewer = new GraphGUI.Compound.ColourGraph.GUI.WPF.CompoundGraphController();
            this.SuspendLayout();
            // 
            // GraphDisplay
            // 
            this.GraphDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphDisplay.Location = new System.Drawing.Point(0, 0);
            this.GraphDisplay.Name = "GraphDisplay";
            this.GraphDisplay.Size = new System.Drawing.Size(984, 712);
            this.GraphDisplay.TabIndex = 0;
            this.GraphDisplay.Text = "elementHost1";
            this.GraphDisplay.Child = this.GraphViewer;
            // 
            // CompoundGraphViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 712);
            this.Controls.Add(this.GraphDisplay);
            this.Name = "CompoundGraphViewer";
            this.Text = "CompoundGraphViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost GraphDisplay;
        private GraphGUI.Compound.ColourGraph.GUI.WPF.CompoundGraphController GraphViewer;
    }
}