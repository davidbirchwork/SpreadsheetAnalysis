using Infralution.Controls.VirtualTree;
using Utilities.Tree;
using Utilities.Tree.Columns;

namespace ExcelFunctionExtractor {
    partial class FunctionExtractorGUI {
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
            this.ExcelOpener = new System.Windows.Forms.OpenFileDialog();
            this.splitterVert = new System.Windows.Forms.SplitContainer();
            this.Splitter_Hoz = new System.Windows.Forms.SplitContainer();
            this.grpbox_Config = new System.Windows.Forms.GroupBox();
            this.shimbox = new System.Windows.Forms.GroupBox();
            this.btnDeleteShim = new System.Windows.Forms.Button();
            this.btnAddShim = new System.Windows.Forms.Button();
            this.shimTree = new Utilities.Tree.XVirtualTreeView();
            this.btnsaveConfigAs = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.edtRootCell = new System.Windows.Forms.TextBox();
            this.btnExcelSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ExcelFileAddress = new System.Windows.Forms.TextBox();
            this.grpBoxCommands = new System.Windows.Forms.GroupBox();
            this.btnSaveSubGraphs = new System.Windows.Forms.Button();
            this.btnSaveWholeGraph = new System.Windows.Forms.Button();
            this.btn_Extract_whole = new System.Windows.Forms.Button();
            this.btnSaveEvalGraph = new System.Windows.Forms.Button();
            this.IncludeBlanks = new System.Windows.Forms.CheckBox();
            this.btnDisplayList = new System.Windows.Forms.CheckBox();
            this.threadscaption = new System.Windows.Forms.Label();
            this.numThreadsPicker = new System.Windows.Forms.NumericUpDown();
            this.btnSaveResultsAS = new System.Windows.Forms.Button();
            this.btnSaveResults = new System.Windows.Forms.Button();
            this.btnLoadResults = new System.Windows.Forms.Button();
            this.EvaluateALL = new System.Windows.Forms.Button();
            this.btnevaluate = new System.Windows.Forms.Button();
            this.formulatestbox = new System.Windows.Forms.TextBox();
            this.chkSheetPrefix = new System.Windows.Forms.CheckBox();
            this.checkboxKnownNames = new System.Windows.Forms.CheckBox();
            this.IncludeUnMappedCells = new System.Windows.Forms.CheckBox();
            this.btnSaveSA = new System.Windows.Forms.Button();
            this.btnSaveGraph = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExtract = new System.Windows.Forms.Button();
            this.taberror = new System.Windows.Forms.TabControl();
            this.tablog = new System.Windows.Forms.TabPage();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.taberrors = new System.Windows.Forms.TabPage();
            this.errorlogbox = new System.Windows.Forms.TextBox();
            this.tabtree = new System.Windows.Forms.TabPage();
            this.Tree = new Utilities.Tree.XVirtualTreeView();
            this.saveGMLFile = new System.Windows.Forms.SaveFileDialog();
            this.TreeOpener = new System.Windows.Forms.OpenFileDialog();
            this.SAMappingOpener = new System.Windows.Forms.OpenFileDialog();
            this.SASaver = new System.Windows.Forms.SaveFileDialog();
            this.configOpener = new System.Windows.Forms.OpenFileDialog();
            this.configsaver = new System.Windows.Forms.SaveFileDialog();
            this.unitTestopener = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.utilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanExcelNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAnalysePartition = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitterVert)).BeginInit();
            this.splitterVert.Panel1.SuspendLayout();
            this.splitterVert.Panel2.SuspendLayout();
            this.splitterVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Splitter_Hoz)).BeginInit();
            this.Splitter_Hoz.Panel1.SuspendLayout();
            this.Splitter_Hoz.Panel2.SuspendLayout();
            this.Splitter_Hoz.SuspendLayout();
            this.grpbox_Config.SuspendLayout();
            this.shimbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shimTree)).BeginInit();
            this.grpBoxCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreadsPicker)).BeginInit();
            this.taberror.SuspendLayout();
            this.tablog.SuspendLayout();
            this.taberrors.SuspendLayout();
            this.tabtree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tree)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ExcelOpener
            // 
            this.ExcelOpener.DefaultExt = "*.xls";
            this.ExcelOpener.Filter = "Excel file |*.xls;*.xlsx";
            this.ExcelOpener.Title = "Select an Excel Spreadsheet";
            // 
            // splitterVert
            // 
            this.splitterVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitterVert.Location = new System.Drawing.Point(0, 24);
            this.splitterVert.Name = "splitterVert";
            // 
            // splitterVert.Panel1
            // 
            this.splitterVert.Panel1.Controls.Add(this.Splitter_Hoz);
            // 
            // splitterVert.Panel2
            // 
            this.splitterVert.Panel2.Controls.Add(this.taberror);
            this.splitterVert.Size = new System.Drawing.Size(1196, 1003);
            this.splitterVert.SplitterDistance = 394;
            this.splitterVert.TabIndex = 0;
            // 
            // Splitter_Hoz
            // 
            this.Splitter_Hoz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Splitter_Hoz.Location = new System.Drawing.Point(0, 0);
            this.Splitter_Hoz.Name = "Splitter_Hoz";
            this.Splitter_Hoz.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Splitter_Hoz.Panel1
            // 
            this.Splitter_Hoz.Panel1.Controls.Add(this.grpbox_Config);
            // 
            // Splitter_Hoz.Panel2
            // 
            this.Splitter_Hoz.Panel2.Controls.Add(this.grpBoxCommands);
            this.Splitter_Hoz.Size = new System.Drawing.Size(394, 1003);
            this.Splitter_Hoz.SplitterDistance = 364;
            this.Splitter_Hoz.TabIndex = 1;
            // 
            // grpbox_Config
            // 
            this.grpbox_Config.Controls.Add(this.shimbox);
            this.grpbox_Config.Controls.Add(this.btnsaveConfigAs);
            this.grpbox_Config.Controls.Add(this.btnSaveConfig);
            this.grpbox_Config.Controls.Add(this.btnLoadConfig);
            this.grpbox_Config.Controls.Add(this.label2);
            this.grpbox_Config.Controls.Add(this.edtRootCell);
            this.grpbox_Config.Controls.Add(this.btnExcelSelect);
            this.grpbox_Config.Controls.Add(this.label1);
            this.grpbox_Config.Controls.Add(this.ExcelFileAddress);
            this.grpbox_Config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpbox_Config.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpbox_Config.Location = new System.Drawing.Point(0, 0);
            this.grpbox_Config.Name = "grpbox_Config";
            this.grpbox_Config.Size = new System.Drawing.Size(394, 364);
            this.grpbox_Config.TabIndex = 0;
            this.grpbox_Config.TabStop = false;
            this.grpbox_Config.Text = "Configuration";
            // 
            // shimbox
            // 
            this.shimbox.Controls.Add(this.btnDeleteShim);
            this.shimbox.Controls.Add(this.btnAddShim);
            this.shimbox.Controls.Add(this.shimTree);
            this.shimbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.shimbox.Location = new System.Drawing.Point(3, 211);
            this.shimbox.Name = "shimbox";
            this.shimbox.Size = new System.Drawing.Size(388, 150);
            this.shimbox.TabIndex = 13;
            this.shimbox.TabStop = false;
            this.shimbox.Text = "Formula Shims";
            // 
            // btnDeleteShim
            // 
            this.btnDeleteShim.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteShim.Location = new System.Drawing.Point(128, 19);
            this.btnDeleteShim.Name = "btnDeleteShim";
            this.btnDeleteShim.Size = new System.Drawing.Size(121, 28);
            this.btnDeleteShim.TabIndex = 11;
            this.btnDeleteShim.Text = "Delete Selected";
            this.btnDeleteShim.UseVisualStyleBackColor = true;
            // 
            // btnAddShim
            // 
            this.btnAddShim.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddShim.Location = new System.Drawing.Point(3, 19);
            this.btnAddShim.Name = "btnAddShim";
            this.btnAddShim.Size = new System.Drawing.Size(121, 28);
            this.btnAddShim.TabIndex = 10;
            this.btnAddShim.Text = "Add Shim";
            this.btnAddShim.UseVisualStyleBackColor = true;
            this.btnAddShim.Click += new System.EventHandler(this.BtnAddShimClick);
            // 
            // shimTree
            // 
            this.shimTree.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.shimTree.Location = new System.Drawing.Point(3, 47);
            this.shimTree.Name = "shimTree";
            this.shimTree.Size = new System.Drawing.Size(382, 100);
            this.shimTree.TabIndex = 9;
            this.shimTree.XTree = null;
            // 
            // btnsaveConfigAs
            // 
            this.btnsaveConfigAs.Location = new System.Drawing.Point(172, 19);
            this.btnsaveConfigAs.Name = "btnsaveConfigAs";
            this.btnsaveConfigAs.Size = new System.Drawing.Size(80, 26);
            this.btnsaveConfigAs.TabIndex = 12;
            this.btnsaveConfigAs.Text = "Save As";
            this.btnsaveConfigAs.UseVisualStyleBackColor = true;
            this.btnsaveConfigAs.Click += new System.EventHandler(this.BtnsaveConfigAsClick);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Location = new System.Drawing.Point(88, 19);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(80, 26);
            this.btnSaveConfig.TabIndex = 11;
            this.btnSaveConfig.Text = "Save";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.BtnSaveConfigClick);
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(3, 19);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(80, 26);
            this.btnLoadConfig.TabIndex = 9;
            this.btnLoadConfig.Text = "Load";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            this.btnLoadConfig.Click += new System.EventHandler(this.BtnLoadConfigClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Root Cell:";
            // 
            // edtRootCell
            // 
            this.edtRootCell.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edtRootCell.Location = new System.Drawing.Point(63, 97);
            this.edtRootCell.Name = "edtRootCell";
            this.edtRootCell.Size = new System.Drawing.Size(179, 23);
            this.edtRootCell.TabIndex = 6;
            this.edtRootCell.Text = "\'Carbon Output Comparison\'!Q6";
            // 
            // btnExcelSelect
            // 
            this.btnExcelSelect.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExcelSelect.Location = new System.Drawing.Point(74, 45);
            this.btnExcelSelect.Name = "btnExcelSelect";
            this.btnExcelSelect.Size = new System.Drawing.Size(81, 26);
            this.btnExcelSelect.TabIndex = 5;
            this.btnExcelSelect.Text = "Select...";
            this.btnExcelSelect.UseVisualStyleBackColor = true;
            this.btnExcelSelect.Click += new System.EventHandler(this.BtnExcelSelectClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Excel File:";
            // 
            // ExcelFileAddress
            // 
            this.ExcelFileAddress.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExcelFileAddress.Location = new System.Drawing.Point(5, 74);
            this.ExcelFileAddress.Name = "ExcelFileAddress";
            this.ExcelFileAddress.Size = new System.Drawing.Size(237, 23);
            this.ExcelFileAddress.TabIndex = 3;
            this.ExcelFileAddress.Text = "sample.xlsx";
            // 
            // grpBoxCommands
            // 
            this.grpBoxCommands.Controls.Add(this.btnAnalysePartition);
            this.grpBoxCommands.Controls.Add(this.btnSaveSubGraphs);
            this.grpBoxCommands.Controls.Add(this.btnSaveWholeGraph);
            this.grpBoxCommands.Controls.Add(this.btn_Extract_whole);
            this.grpBoxCommands.Controls.Add(this.btnSaveEvalGraph);
            this.grpBoxCommands.Controls.Add(this.IncludeBlanks);
            this.grpBoxCommands.Controls.Add(this.btnDisplayList);
            this.grpBoxCommands.Controls.Add(this.threadscaption);
            this.grpBoxCommands.Controls.Add(this.numThreadsPicker);
            this.grpBoxCommands.Controls.Add(this.btnSaveResultsAS);
            this.grpBoxCommands.Controls.Add(this.btnSaveResults);
            this.grpBoxCommands.Controls.Add(this.btnLoadResults);
            this.grpBoxCommands.Controls.Add(this.EvaluateALL);
            this.grpBoxCommands.Controls.Add(this.btnevaluate);
            this.grpBoxCommands.Controls.Add(this.formulatestbox);
            this.grpBoxCommands.Controls.Add(this.chkSheetPrefix);
            this.grpBoxCommands.Controls.Add(this.checkboxKnownNames);
            this.grpBoxCommands.Controls.Add(this.IncludeUnMappedCells);
            this.grpBoxCommands.Controls.Add(this.btnSaveSA);
            this.grpBoxCommands.Controls.Add(this.btnSaveGraph);
            this.grpBoxCommands.Controls.Add(this.btnRefresh);
            this.grpBoxCommands.Controls.Add(this.btnExtract);
            this.grpBoxCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBoxCommands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBoxCommands.Location = new System.Drawing.Point(0, 0);
            this.grpBoxCommands.Name = "grpBoxCommands";
            this.grpBoxCommands.Size = new System.Drawing.Size(394, 635);
            this.grpBoxCommands.TabIndex = 1;
            this.grpBoxCommands.TabStop = false;
            this.grpBoxCommands.Text = "Commands";
            // 
            // btnSaveSubGraphs
            // 
            this.btnSaveSubGraphs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSubGraphs.Location = new System.Drawing.Point(263, 75);
            this.btnSaveSubGraphs.Name = "btnSaveSubGraphs";
            this.btnSaveSubGraphs.Size = new System.Drawing.Size(122, 27);
            this.btnSaveSubGraphs.TabIndex = 25;
            this.btnSaveSubGraphs.Text = "Save Sub Graphs";
            this.btnSaveSubGraphs.UseVisualStyleBackColor = true;
            this.btnSaveSubGraphs.Click += new System.EventHandler(this.btnSaveSubGraphs_Click);
            // 
            // btnSaveWholeGraph
            // 
            this.btnSaveWholeGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveWholeGraph.Location = new System.Drawing.Point(135, 73);
            this.btnSaveWholeGraph.Name = "btnSaveWholeGraph";
            this.btnSaveWholeGraph.Size = new System.Drawing.Size(122, 27);
            this.btnSaveWholeGraph.TabIndex = 24;
            this.btnSaveWholeGraph.Text = "Save Whole Graph";
            this.btnSaveWholeGraph.UseVisualStyleBackColor = true;
            this.btnSaveWholeGraph.Click += new System.EventHandler(this.btnSaveWholeGraph_Click);
            // 
            // btn_Extract_whole
            // 
            this.btn_Extract_whole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Extract_whole.Location = new System.Drawing.Point(8, 73);
            this.btn_Extract_whole.Name = "btn_Extract_whole";
            this.btn_Extract_whole.Size = new System.Drawing.Size(122, 27);
            this.btn_Extract_whole.TabIndex = 23;
            this.btn_Extract_whole.Text = "Extract Whole!";
            this.btn_Extract_whole.UseVisualStyleBackColor = true;
            this.btn_Extract_whole.Click += new System.EventHandler(this.btn_Extract_whole_Click);
            // 
            // btnSaveEvalGraph
            // 
            this.btnSaveEvalGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveEvalGraph.Location = new System.Drawing.Point(156, 128);
            this.btnSaveEvalGraph.Name = "btnSaveEvalGraph";
            this.btnSaveEvalGraph.Size = new System.Drawing.Size(105, 24);
            this.btnSaveEvalGraph.TabIndex = 22;
            this.btnSaveEvalGraph.Text = "Save Eval Graph";
            this.btnSaveEvalGraph.UseVisualStyleBackColor = true;
            this.btnSaveEvalGraph.Click += new System.EventHandler(this.BtnSaveEvalGraphClick);
            // 
            // IncludeBlanks
            // 
            this.IncludeBlanks.AutoSize = true;
            this.IncludeBlanks.Location = new System.Drawing.Point(23, 128);
            this.IncludeBlanks.Name = "IncludeBlanks";
            this.IncludeBlanks.Size = new System.Drawing.Size(110, 17);
            this.IncludeBlanks.TabIndex = 21;
            this.IncludeBlanks.Text = "Include Blanks";
            this.IncludeBlanks.UseVisualStyleBackColor = true;
            // 
            // btnDisplayList
            // 
            this.btnDisplayList.AutoSize = true;
            this.btnDisplayList.Checked = true;
            this.btnDisplayList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnDisplayList.Location = new System.Drawing.Point(16, 244);
            this.btnDisplayList.Name = "btnDisplayList";
            this.btnDisplayList.Size = new System.Drawing.Size(73, 17);
            this.btnDisplayList.TabIndex = 19;
            this.btnDisplayList.Text = "ListCells";
            this.btnDisplayList.UseVisualStyleBackColor = true;
            // 
            // threadscaption
            // 
            this.threadscaption.AutoSize = true;
            this.threadscaption.Location = new System.Drawing.Point(103, 23);
            this.threadscaption.Name = "threadscaption";
            this.threadscaption.Size = new System.Drawing.Size(53, 13);
            this.threadscaption.TabIndex = 17;
            this.threadscaption.Text = "Threads";
            // 
            // numThreadsPicker
            // 
            this.numThreadsPicker.Location = new System.Drawing.Point(7, 19);
            this.numThreadsPicker.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numThreadsPicker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreadsPicker.Name = "numThreadsPicker";
            this.numThreadsPicker.Size = new System.Drawing.Size(93, 20);
            this.numThreadsPicker.TabIndex = 16;
            this.numThreadsPicker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnSaveResultsAS
            // 
            this.btnSaveResultsAS.Location = new System.Drawing.Point(177, 270);
            this.btnSaveResultsAS.Name = "btnSaveResultsAS";
            this.btnSaveResultsAS.Size = new System.Drawing.Size(80, 26);
            this.btnSaveResultsAS.TabIndex = 15;
            this.btnSaveResultsAS.Text = "Save As";
            this.btnSaveResultsAS.UseVisualStyleBackColor = true;
            this.btnSaveResultsAS.Click += new System.EventHandler(this.BtnSaveResultsAsClick);
            // 
            // btnSaveResults
            // 
            this.btnSaveResults.Location = new System.Drawing.Point(93, 270);
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.Size = new System.Drawing.Size(80, 26);
            this.btnSaveResults.TabIndex = 14;
            this.btnSaveResults.Text = "Save";
            this.btnSaveResults.UseVisualStyleBackColor = true;
            this.btnSaveResults.Click += new System.EventHandler(this.BtnSaveResultsClick);
            // 
            // btnLoadResults
            // 
            this.btnLoadResults.Location = new System.Drawing.Point(8, 270);
            this.btnLoadResults.Name = "btnLoadResults";
            this.btnLoadResults.Size = new System.Drawing.Size(80, 26);
            this.btnLoadResults.TabIndex = 13;
            this.btnLoadResults.Text = "Load";
            this.btnLoadResults.UseVisualStyleBackColor = true;
            this.btnLoadResults.Click += new System.EventHandler(this.BtnLoadResultsClick);
            // 
            // EvaluateALL
            // 
            this.EvaluateALL.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EvaluateALL.Location = new System.Drawing.Point(131, 41);
            this.EvaluateALL.Name = "EvaluateALL";
            this.EvaluateALL.Size = new System.Drawing.Size(119, 26);
            this.EvaluateALL.TabIndex = 12;
            this.EvaluateALL.Text = "Evaluate All!";
            this.EvaluateALL.UseVisualStyleBackColor = true;
            this.EvaluateALL.Click += new System.EventHandler(this.EvaluateSelectedClick);
            // 
            // btnevaluate
            // 
            this.btnevaluate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnevaluate.Location = new System.Drawing.Point(136, 209);
            this.btnevaluate.Name = "btnevaluate";
            this.btnevaluate.Size = new System.Drawing.Size(81, 26);
            this.btnevaluate.TabIndex = 11;
            this.btnevaluate.Text = "Evaluate!";
            this.btnevaluate.UseVisualStyleBackColor = true;
            this.btnevaluate.Click += new System.EventHandler(this.BtnEvaluateClick);
            // 
            // formulatestbox
            // 
            this.formulatestbox.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formulatestbox.Location = new System.Drawing.Point(25, 209);
            this.formulatestbox.Name = "formulatestbox";
            this.formulatestbox.Size = new System.Drawing.Size(112, 23);
            this.formulatestbox.TabIndex = 10;
            // 
            // chkSheetPrefix
            // 
            this.chkSheetPrefix.AutoSize = true;
            this.chkSheetPrefix.Checked = true;
            this.chkSheetPrefix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSheetPrefix.Location = new System.Drawing.Point(23, 113);
            this.chkSheetPrefix.Name = "chkSheetPrefix";
            this.chkSheetPrefix.Size = new System.Drawing.Size(133, 17);
            this.chkSheetPrefix.TabIndex = 7;
            this.chkSheetPrefix.Text = "IncludeSheetPrefix";
            this.chkSheetPrefix.UseVisualStyleBackColor = true;
            // 
            // checkboxKnownNames
            // 
            this.checkboxKnownNames.AutoSize = true;
            this.checkboxKnownNames.Checked = true;
            this.checkboxKnownNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkboxKnownNames.Location = new System.Drawing.Point(23, 98);
            this.checkboxKnownNames.Name = "checkboxKnownNames";
            this.checkboxKnownNames.Size = new System.Drawing.Size(132, 17);
            this.checkboxKnownNames.TabIndex = 6;
            this.checkboxKnownNames.Text = "Use Known Names";
            this.checkboxKnownNames.UseVisualStyleBackColor = true;
            // 
            // IncludeUnMappedCells
            // 
            this.IncludeUnMappedCells.AutoSize = true;
            this.IncludeUnMappedCells.Location = new System.Drawing.Point(56, 186);
            this.IncludeUnMappedCells.Name = "IncludeUnMappedCells";
            this.IncludeUnMappedCells.Size = new System.Drawing.Size(190, 17);
            this.IncludeUnMappedCells.TabIndex = 5;
            this.IncludeUnMappedCells.Text = "Include only cells in mapping";
            this.IncludeUnMappedCells.UseVisualStyleBackColor = true;
            // 
            // btnSaveSA
            // 
            this.btnSaveSA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveSA.Location = new System.Drawing.Point(23, 151);
            this.btnSaveSA.Name = "btnSaveSA";
            this.btnSaveSA.Size = new System.Drawing.Size(206, 29);
            this.btnSaveSA.TabIndex = 4;
            this.btnSaveSA.Text = "Create Sensitivity Analysis";
            this.btnSaveSA.UseVisualStyleBackColor = true;
            this.btnSaveSA.Click += new System.EventHandler(this.BtnSaveSAClick);
            // 
            // btnSaveGraph
            // 
            this.btnSaveGraph.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveGraph.Location = new System.Drawing.Point(156, 102);
            this.btnSaveGraph.Name = "btnSaveGraph";
            this.btnSaveGraph.Size = new System.Drawing.Size(105, 28);
            this.btnSaveGraph.TabIndex = 2;
            this.btnSaveGraph.Text = "Save Graph";
            this.btnSaveGraph.UseVisualStyleBackColor = true;
            this.btnSaveGraph.Click += new System.EventHandler(this.BtnSaveGraphClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Location = new System.Drawing.Point(95, 238);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(122, 27);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefreshClick);
            // 
            // btnExtract
            // 
            this.btnExtract.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtract.Location = new System.Drawing.Point(6, 40);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(122, 27);
            this.btnExtract.TabIndex = 0;
            this.btnExtract.Text = "Extract!";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.BtnExtractClick);
            // 
            // taberror
            // 
            this.taberror.Controls.Add(this.tablog);
            this.taberror.Controls.Add(this.taberrors);
            this.taberror.Controls.Add(this.tabtree);
            this.taberror.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taberror.Location = new System.Drawing.Point(0, 0);
            this.taberror.Name = "taberror";
            this.taberror.SelectedIndex = 0;
            this.taberror.Size = new System.Drawing.Size(798, 1003);
            this.taberror.TabIndex = 3;
            // 
            // tablog
            // 
            this.tablog.Controls.Add(this.LogBox);
            this.tablog.Location = new System.Drawing.Point(4, 22);
            this.tablog.Name = "tablog";
            this.tablog.Padding = new System.Windows.Forms.Padding(3);
            this.tablog.Size = new System.Drawing.Size(790, 977);
            this.tablog.TabIndex = 1;
            this.tablog.Text = "Log";
            this.tablog.UseVisualStyleBackColor = true;
            // 
            // LogBox
            // 
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogBox.Location = new System.Drawing.Point(3, 3);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.Size = new System.Drawing.Size(784, 971);
            this.LogBox.TabIndex = 10;
            // 
            // taberrors
            // 
            this.taberrors.Controls.Add(this.errorlogbox);
            this.taberrors.Location = new System.Drawing.Point(4, 22);
            this.taberrors.Name = "taberrors";
            this.taberrors.Padding = new System.Windows.Forms.Padding(3);
            this.taberrors.Size = new System.Drawing.Size(790, 977);
            this.taberrors.TabIndex = 2;
            this.taberrors.Text = "Error Log";
            this.taberrors.UseVisualStyleBackColor = true;
            // 
            // errorlogbox
            // 
            this.errorlogbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorlogbox.Location = new System.Drawing.Point(3, 3);
            this.errorlogbox.Multiline = true;
            this.errorlogbox.Name = "errorlogbox";
            this.errorlogbox.Size = new System.Drawing.Size(784, 971);
            this.errorlogbox.TabIndex = 11;
            // 
            // tabtree
            // 
            this.tabtree.Controls.Add(this.Tree);
            this.tabtree.Location = new System.Drawing.Point(4, 22);
            this.tabtree.Name = "tabtree";
            this.tabtree.Padding = new System.Windows.Forms.Padding(3);
            this.tabtree.Size = new System.Drawing.Size(790, 977);
            this.tabtree.TabIndex = 0;
            this.tabtree.Text = "Tree";
            this.tabtree.UseVisualStyleBackColor = true;
            // 
            // Tree
            // 
            this.Tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tree.Location = new System.Drawing.Point(3, 3);
            this.Tree.Name = "Tree";
            this.Tree.Size = new System.Drawing.Size(784, 971);
            this.Tree.TabIndex = 3;
            this.Tree.XTree = null;
            // 
            // saveGMLFile
            // 
            this.saveGMLFile.DefaultExt = "*.graphml";
            this.saveGMLFile.Filter = "GraphML|*.graphml";
            this.saveGMLFile.Title = "Save GraphML File";
            // 
            // TreeOpener
            // 
            this.TreeOpener.Filter = "XML|*.xml";
            // 
            // SAMappingOpener
            // 
            this.SAMappingOpener.Filter = "tab seperated |*.txt;*.csv";
            // 
            // configOpener
            // 
            this.configOpener.FileName = "config.xml";
            this.configOpener.Filter = "xml | *.xml";
            // 
            // configsaver
            // 
            this.configsaver.Filter = "XML | *.xml";
            // 
            // unitTestopener
            // 
            this.unitTestopener.FileName = "config.csv";
            this.unitTestopener.Filter = "csv | *.csv";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.utilitiesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1196, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // utilitiesToolStripMenuItem
            // 
            this.utilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cleanExcelNamesToolStripMenuItem});
            this.utilitiesToolStripMenuItem.Name = "utilitiesToolStripMenuItem";
            this.utilitiesToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.utilitiesToolStripMenuItem.Text = "Utilities";
            // 
            // cleanExcelNamesToolStripMenuItem
            // 
            this.cleanExcelNamesToolStripMenuItem.Name = "cleanExcelNamesToolStripMenuItem";
            this.cleanExcelNamesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.cleanExcelNamesToolStripMenuItem.Text = "CleanExcelNames";
            this.cleanExcelNamesToolStripMenuItem.Click += new System.EventHandler(this.cleanExcelNamesToolStripMenuItem_Click);
            // 
            // btnAnalysePartition
            // 
            this.btnAnalysePartition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalysePartition.Location = new System.Drawing.Point(263, 108);
            this.btnAnalysePartition.Name = "btnAnalysePartition";
            this.btnAnalysePartition.Size = new System.Drawing.Size(122, 27);
            this.btnAnalysePartition.TabIndex = 26;
            this.btnAnalysePartition.Text = "AnalysePartition";
            this.btnAnalysePartition.UseVisualStyleBackColor = true;
            this.btnAnalysePartition.Click += new System.EventHandler(this.btnAnalysePartition_Click);
            // 
            // FunctionExtractorGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 1027);
            this.Controls.Add(this.splitterVert);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FunctionExtractorGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Excel Graph Extractor by David Birch";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FunctionExtractorGUI_FormClosed);
            this.Load += new System.EventHandler(this.FunctionExtractorGUI_Load);
            this.splitterVert.Panel1.ResumeLayout(false);
            this.splitterVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitterVert)).EndInit();
            this.splitterVert.ResumeLayout(false);
            this.Splitter_Hoz.Panel1.ResumeLayout(false);
            this.Splitter_Hoz.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Splitter_Hoz)).EndInit();
            this.Splitter_Hoz.ResumeLayout(false);
            this.grpbox_Config.ResumeLayout(false);
            this.grpbox_Config.PerformLayout();
            this.shimbox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.shimTree)).EndInit();
            this.grpBoxCommands.ResumeLayout(false);
            this.grpBoxCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreadsPicker)).EndInit();
            this.taberror.ResumeLayout(false);
            this.tablog.ResumeLayout(false);
            this.tablog.PerformLayout();
            this.taberrors.ResumeLayout(false);
            this.taberrors.PerformLayout();
            this.tabtree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Tree)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ExcelOpener;
        private System.Windows.Forms.SplitContainer splitterVert;
        private System.Windows.Forms.SplitContainer Splitter_Hoz;
        private System.Windows.Forms.GroupBox grpbox_Config;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox edtRootCell;
        private System.Windows.Forms.Button btnExcelSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ExcelFileAddress;
        private System.Windows.Forms.GroupBox grpBoxCommands;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSaveGraph;
        private System.Windows.Forms.SaveFileDialog saveGMLFile;
        private System.Windows.Forms.Button btnSaveSA;
        private System.Windows.Forms.OpenFileDialog TreeOpener;
        private System.Windows.Forms.OpenFileDialog SAMappingOpener;
        private System.Windows.Forms.SaveFileDialog SASaver;
        private System.Windows.Forms.CheckBox IncludeUnMappedCells;
        private System.Windows.Forms.CheckBox checkboxKnownNames;
        private System.Windows.Forms.CheckBox chkSheetPrefix;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.Button EvaluateALL;
        private System.Windows.Forms.Button btnevaluate;
        private System.Windows.Forms.TextBox formulatestbox;
        private System.Windows.Forms.OpenFileDialog configOpener;
        private System.Windows.Forms.SaveFileDialog configsaver;
        private System.Windows.Forms.Button btnsaveConfigAs;
        private System.Windows.Forms.GroupBox shimbox;
        private System.Windows.Forms.Button btnDeleteShim;
        private System.Windows.Forms.Button btnAddShim;
        private XVirtualTreeView shimTree;
        private System.Windows.Forms.Button btnSaveResultsAS;
        private System.Windows.Forms.Button btnSaveResults;
        private System.Windows.Forms.Button btnLoadResults;
        private System.Windows.Forms.NumericUpDown numThreadsPicker;
        private System.Windows.Forms.Label threadscaption;
        private System.Windows.Forms.OpenFileDialog unitTestopener;
        private System.Windows.Forms.CheckBox btnDisplayList;
        private System.Windows.Forms.CheckBox IncludeBlanks;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem utilitiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanExcelNamesToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveEvalGraph;
        private System.Windows.Forms.Button btn_Extract_whole;
        private System.Windows.Forms.TabControl taberror;
        private System.Windows.Forms.TabPage tabtree;
        private XVirtualTreeView Tree;
        private System.Windows.Forms.TabPage tablog;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button btnSaveWholeGraph;
        private System.Windows.Forms.TabPage taberrors;
        private System.Windows.Forms.TextBox errorlogbox;
        private System.Windows.Forms.Button btnSaveSubGraphs;
        private System.Windows.Forms.Button btnAnalysePartition;
    }
}

