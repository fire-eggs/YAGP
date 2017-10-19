namespace timeline
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnmainFormClose = new System.Windows.Forms.Button();
            this.tbMainFormFilename = new System.Windows.Forms.TextBox();
            this.lblMainFormFilename = new System.Windows.Forms.Label();
            this.rtbMainFormResults = new System.Windows.Forms.RichTextBox();
            this.cmenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.showEventsForIndividualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnMainFormParse = new System.Windows.Forms.Button();
            this.btnMainFormViewFile = new System.Windows.Forms.Button();
            this.gbMainForm = new System.Windows.Forms.GroupBox();
            this.btnMainFormAutoFix = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnJscript = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnMainFormEdit = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAddressFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAddressFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSavedConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editBeforePrintingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntriesWithMissingDatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntriesWithBadGeocodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMissingDatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBadGeocodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.averageLongetivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayListOfIndividualsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSavedAddressesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSavedAddresesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSavedAddressListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usingTimelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnMainFormCreatWebPage = new System.Windows.Forms.Button();
            this.saveFileDialog1MainForm = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblCurrentView = new System.Windows.Forms.Label();
            this.btnMainFormClearTextBox = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmenu1.SuspendLayout();
            this.gbMainForm.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnmainFormClose
            // 
            this.btnmainFormClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnmainFormClose.Location = new System.Drawing.Point(1035, 800);
            this.btnmainFormClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnmainFormClose.Name = "btnmainFormClose";
            this.btnmainFormClose.Size = new System.Drawing.Size(162, 35);
            this.btnmainFormClose.TabIndex = 0;
            this.btnmainFormClose.Text = "&Close";
            this.btnmainFormClose.UseVisualStyleBackColor = true;
            this.btnmainFormClose.Click += new System.EventHandler(this.btnmainFormClose_Click);
            // 
            // tbMainFormFilename
            // 
            this.tbMainFormFilename.AllowDrop = true;
            this.tbMainFormFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMainFormFilename.Location = new System.Drawing.Point(214, 115);
            this.tbMainFormFilename.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbMainFormFilename.Name = "tbMainFormFilename";
            this.tbMainFormFilename.Size = new System.Drawing.Size(918, 26);
            this.tbMainFormFilename.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbMainFormFilename, "Drag and Drop a GED source file to get started");
            this.tbMainFormFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbMainFormFilename_DragDrop);
            this.tbMainFormFilename.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbMainFormFilename_DragEnter);
            // 
            // lblMainFormFilename
            // 
            this.lblMainFormFilename.AutoSize = true;
            this.lblMainFormFilename.Location = new System.Drawing.Point(72, 126);
            this.lblMainFormFilename.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMainFormFilename.Name = "lblMainFormFilename";
            this.lblMainFormFilename.Size = new System.Drawing.Size(133, 20);
            this.lblMainFormFilename.TabIndex = 2;
            this.lblMainFormFilename.Text = "GED Source File:";
            // 
            // rtbMainFormResults
            // 
            this.rtbMainFormResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbMainFormResults.BackColor = System.Drawing.Color.LightYellow;
            this.rtbMainFormResults.ContextMenuStrip = this.cmenu1;
            this.rtbMainFormResults.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMainFormResults.Location = new System.Drawing.Point(76, 225);
            this.rtbMainFormResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbMainFormResults.Name = "rtbMainFormResults";
            this.rtbMainFormResults.Size = new System.Drawing.Size(854, 609);
            this.rtbMainFormResults.TabIndex = 3;
            this.rtbMainFormResults.Text = "";
            this.rtbMainFormResults.WordWrap = false;
            this.rtbMainFormResults.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbMainFormResults_LinkClicked);
            this.rtbMainFormResults.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rtbMainFormResults_MouseClick);
            // 
            // cmenu1
            // 
            this.cmenu1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.showEventsForIndividualToolStripMenuItem});
            this.cmenu1.Name = "cmenu1";
            this.cmenu1.Size = new System.Drawing.Size(298, 94);
            this.cmenu1.Opening += new System.ComponentModel.CancelEventHandler(this.cmenu1_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(297, 30);
            this.toolStripMenuItem1.Text = "Edit Record";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(297, 30);
            this.toolStripMenuItem2.Text = "Delete Record";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // showEventsForIndividualToolStripMenuItem
            // 
            this.showEventsForIndividualToolStripMenuItem.Name = "showEventsForIndividualToolStripMenuItem";
            this.showEventsForIndividualToolStripMenuItem.Size = new System.Drawing.Size(297, 30);
            this.showEventsForIndividualToolStripMenuItem.Text = "Show Events For Individual";
            this.showEventsForIndividualToolStripMenuItem.Click += new System.EventHandler(this.showEventsForIndividualToolStripMenuItem_Click);
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(33, 29);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(162, 35);
            this.btnOpenFile.TabIndex = 4;
            this.btnOpenFile.Text = "&Get GED File";
            this.toolTip1.SetToolTip(this.btnOpenFile, "Load the GED file in the Source File TextBox\r\n\r\n\r\n");
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnMainFormParse
            // 
            this.btnMainFormParse.Location = new System.Drawing.Point(33, 118);
            this.btnMainFormParse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormParse.Name = "btnMainFormParse";
            this.btnMainFormParse.Size = new System.Drawing.Size(162, 35);
            this.btnMainFormParse.TabIndex = 5;
            this.btnMainFormParse.Text = "&TimeLine Data";
            this.toolTip1.SetToolTip(this.btnMainFormParse, "Create Time Line Data from the GED source\r\nfile or display data aready created or" +
        " loaded.\r\n");
            this.btnMainFormParse.UseVisualStyleBackColor = true;
            this.btnMainFormParse.Click += new System.EventHandler(this.btnMainFormParse_Click);
            // 
            // btnMainFormViewFile
            // 
            this.btnMainFormViewFile.Location = new System.Drawing.Point(33, 74);
            this.btnMainFormViewFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormViewFile.Name = "btnMainFormViewFile";
            this.btnMainFormViewFile.Size = new System.Drawing.Size(162, 35);
            this.btnMainFormViewFile.TabIndex = 6;
            this.btnMainFormViewFile.Text = "&View GED File";
            this.toolTip1.SetToolTip(this.btnMainFormViewFile, "Display Current GED source file");
            this.btnMainFormViewFile.UseVisualStyleBackColor = true;
            this.btnMainFormViewFile.Click += new System.EventHandler(this.btnMainFormViewFile_Click);
            // 
            // gbMainForm
            // 
            this.gbMainForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMainForm.Controls.Add(this.btnMainFormAutoFix);
            this.gbMainForm.Controls.Add(this.btnCancel);
            this.gbMainForm.Controls.Add(this.btnJscript);
            this.gbMainForm.Controls.Add(this.button1);
            this.gbMainForm.Controls.Add(this.btnMainFormEdit);
            this.gbMainForm.Controls.Add(this.btnOpenFile);
            this.gbMainForm.Controls.Add(this.btnMainFormParse);
            this.gbMainForm.Controls.Add(this.btnMainFormViewFile);
            this.gbMainForm.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gbMainForm.Location = new System.Drawing.Point(1000, 225);
            this.gbMainForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbMainForm.Name = "gbMainForm";
            this.gbMainForm.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gbMainForm.Size = new System.Drawing.Size(226, 408);
            this.gbMainForm.TabIndex = 7;
            this.gbMainForm.TabStop = false;
            // 
            // btnMainFormAutoFix
            // 
            this.btnMainFormAutoFix.Location = new System.Drawing.Point(33, 163);
            this.btnMainFormAutoFix.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormAutoFix.Name = "btnMainFormAutoFix";
            this.btnMainFormAutoFix.Size = new System.Drawing.Size(162, 35);
            this.btnMainFormAutoFix.TabIndex = 11;
            this.btnMainFormAutoFix.Text = "&AutoFix";
            this.toolTip1.SetToolTip(this.btnMainFormAutoFix, "Fix an errors in the Time Line data\r\nsuch as duplicates, missing dates or geocode" +
        "s\r\n");
            this.btnMainFormAutoFix.UseVisualStyleBackColor = true;
            this.btnMainFormAutoFix.Click += new System.EventHandler(this.btnMainFormAutoFix_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(34, 345);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(162, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "C&ancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Stop Background Geocoding operation.\r\nEntires already coded will be saved.\r\n");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnJscript
            // 
            this.btnJscript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJscript.Location = new System.Drawing.Point(33, 252);
            this.btnJscript.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnJscript.Name = "btnJscript";
            this.btnJscript.Size = new System.Drawing.Size(162, 35);
            this.btnJscript.TabIndex = 10;
            this.btnJscript.Text = "Show JavaScript";
            this.toolTip1.SetToolTip(this.btnJscript, "Displays Custom javascript generated from\r\ntimeline data which includes events se" +
        "lected\r\nin the Settings window.\r\n");
            this.btnJscript.UseVisualStyleBackColor = true;
            this.btnJscript.Click += new System.EventHandler(this.btnJscript_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(33, 208);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(162, 35);
            this.button1.TabIndex = 9;
            this.button1.Text = "&GeoCode";
            this.toolTip1.SetToolTip(this.button1, "Get GeoCodes from google for\r\ntimeline entries that don\'t have them already.\r\nThi" +
        "s may take several minutes for a large file.\r\nThere is a query limit of 2500 req" +
        "uests per day\r\nper API key.\r\n");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMainFormEdit
            // 
            this.btnMainFormEdit.Location = new System.Drawing.Point(34, 297);
            this.btnMainFormEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormEdit.Name = "btnMainFormEdit";
            this.btnMainFormEdit.Size = new System.Drawing.Size(162, 35);
            this.btnMainFormEdit.TabIndex = 7;
            this.btnMainFormEdit.Text = "&Edit";
            this.btnMainFormEdit.UseVisualStyleBackColor = true;
            this.btnMainFormEdit.Click += new System.EventHandler(this.btnMainFormEdit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1281, 35);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveAddressFileToolStripMenuItem,
            this.loadAddressFileToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.printToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.saveToolStripMenuItem.Text = "&Save Data File";
            this.saveToolStripMenuItem.ToolTipText = "Save Time Line Data File";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.loadToolStripMenuItem.Text = "&Open Data File";
            this.loadToolStripMenuItem.ToolTipText = "Load Time Line Data file";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveAddressFileToolStripMenuItem
            // 
            this.saveAddressFileToolStripMenuItem.Name = "saveAddressFileToolStripMenuItem";
            this.saveAddressFileToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.saveAddressFileToolStripMenuItem.Text = "Save &Address File";
            this.saveAddressFileToolStripMenuItem.Click += new System.EventHandler(this.saveAddressFileToolStripMenuItem_Click);
            // 
            // loadAddressFileToolStripMenuItem
            // 
            this.loadAddressFileToolStripMenuItem.Name = "loadAddressFileToolStripMenuItem";
            this.loadAddressFileToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.loadAddressFileToolStripMenuItem.Text = "&Load Address File";
            this.loadAddressFileToolStripMenuItem.Click += new System.EventHandler(this.loadAddressFileToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveConfigurationToolStripMenuItem,
            this.loadSavedConfigurationToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.preferencesToolStripMenuItem.Text = "&Preferences";
            // 
            // saveConfigurationToolStripMenuItem
            // 
            this.saveConfigurationToolStripMenuItem.Name = "saveConfigurationToolStripMenuItem";
            this.saveConfigurationToolStripMenuItem.Size = new System.Drawing.Size(245, 30);
            this.saveConfigurationToolStripMenuItem.Text = "Save &User Settings";
            this.saveConfigurationToolStripMenuItem.Click += new System.EventHandler(this.saveConfigurationToolStripMenuItem_Click);
            // 
            // loadSavedConfigurationToolStripMenuItem
            // 
            this.loadSavedConfigurationToolStripMenuItem.Name = "loadSavedConfigurationToolStripMenuItem";
            this.loadSavedConfigurationToolStripMenuItem.Size = new System.Drawing.Size(245, 30);
            this.loadSavedConfigurationToolStripMenuItem.Text = "&Load User Settings";
            this.loadSavedConfigurationToolStripMenuItem.Click += new System.EventHandler(this.loadSavedConfigurationToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentViewToolStripMenuItem,
            this.editBeforePrintingToolStripMenuItem});
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.printToolStripMenuItem.Text = "P&rint";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.printToolStripMenuItem_Click);
            // 
            // currentViewToolStripMenuItem
            // 
            this.currentViewToolStripMenuItem.Name = "currentViewToolStripMenuItem";
            this.currentViewToolStripMenuItem.Size = new System.Drawing.Size(249, 30);
            this.currentViewToolStripMenuItem.Text = "Current View";
            this.currentViewToolStripMenuItem.Click += new System.EventHandler(this.currentViewToolStripMenuItem_Click);
            // 
            // editBeforePrintingToolStripMenuItem
            // 
            this.editBeforePrintingToolStripMenuItem.Name = "editBeforePrintingToolStripMenuItem";
            this.editBeforePrintingToolStripMenuItem.Size = new System.Drawing.Size(249, 30);
            this.editBeforePrintingToolStripMenuItem.Text = "Edit Before Printing";
            this.editBeforePrintingToolStripMenuItem.Click += new System.EventHandler(this.editBeforePrintingToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(237, 30);
            this.quitToolStripMenuItem.Text = "&Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(161, 30);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeDuplicatesToolStripMenuItem,
            this.removeEntriesWithMissingDatesToolStripMenuItem,
            this.removeEntriesWithBadGeocodesToolStripMenuItem,
            this.showMissingDatesToolStripMenuItem,
            this.showBadGeocodesToolStripMenuItem,
            this.showDuplicatesToolStripMenuItem,
            this.averageLongetivityToolStripMenuItem,
            this.displayListOfIndividualsToolStripMenuItem,
            this.updateSavedAddressesToolStripMenuItem,
            this.viewSavedAddresesToolStripMenuItem,
            this.clearSavedAddressListToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // removeDuplicatesToolStripMenuItem
            // 
            this.removeDuplicatesToolStripMenuItem.Name = "removeDuplicatesToolStripMenuItem";
            this.removeDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.removeDuplicatesToolStripMenuItem.Text = "Remove &Duplicate Events";
            this.removeDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.removeDuplicatesToolStripMenuItem_Click);
            // 
            // removeEntriesWithMissingDatesToolStripMenuItem
            // 
            this.removeEntriesWithMissingDatesToolStripMenuItem.Name = "removeEntriesWithMissingDatesToolStripMenuItem";
            this.removeEntriesWithMissingDatesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.removeEntriesWithMissingDatesToolStripMenuItem.Text = "Remove Entries with &Missing Dates";
            this.removeEntriesWithMissingDatesToolStripMenuItem.Click += new System.EventHandler(this.removeEntriesWithMissingDatesToolStripMenuItem_Click);
            // 
            // removeEntriesWithBadGeocodesToolStripMenuItem
            // 
            this.removeEntriesWithBadGeocodesToolStripMenuItem.Name = "removeEntriesWithBadGeocodesToolStripMenuItem";
            this.removeEntriesWithBadGeocodesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.removeEntriesWithBadGeocodesToolStripMenuItem.Text = "Remove Entries with &Bad Geocodes";
            this.removeEntriesWithBadGeocodesToolStripMenuItem.Click += new System.EventHandler(this.removeEntriesWithBadGeocodesToolStripMenuItem_Click);
            // 
            // showMissingDatesToolStripMenuItem
            // 
            this.showMissingDatesToolStripMenuItem.Name = "showMissingDatesToolStripMenuItem";
            this.showMissingDatesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.showMissingDatesToolStripMenuItem.Text = "Show Missing D&ates";
            this.showMissingDatesToolStripMenuItem.Click += new System.EventHandler(this.showMissingDatesToolStripMenuItem_Click);
            // 
            // showBadGeocodesToolStripMenuItem
            // 
            this.showBadGeocodesToolStripMenuItem.Name = "showBadGeocodesToolStripMenuItem";
            this.showBadGeocodesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.showBadGeocodesToolStripMenuItem.Text = "Show Bad &Geocodes";
            this.showBadGeocodesToolStripMenuItem.Click += new System.EventHandler(this.showBadGeocodesToolStripMenuItem_Click);
            // 
            // showDuplicatesToolStripMenuItem
            // 
            this.showDuplicatesToolStripMenuItem.Name = "showDuplicatesToolStripMenuItem";
            this.showDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.showDuplicatesToolStripMenuItem.Text = "Show Duplicate &Events";
            this.showDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.showDuplicatesToolStripMenuItem_Click);
            // 
            // averageLongetivityToolStripMenuItem
            // 
            this.averageLongetivityToolStripMenuItem.Name = "averageLongetivityToolStripMenuItem";
            this.averageLongetivityToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.averageLongetivityToolStripMenuItem.Text = "&Calculate Average Longevity";
            this.averageLongetivityToolStripMenuItem.Click += new System.EventHandler(this.averageLongetivityToolStripMenuItem_Click);
            // 
            // displayListOfIndividualsToolStripMenuItem
            // 
            this.displayListOfIndividualsToolStripMenuItem.Name = "displayListOfIndividualsToolStripMenuItem";
            this.displayListOfIndividualsToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.displayListOfIndividualsToolStripMenuItem.Text = "Display &List of Individuals";
            this.displayListOfIndividualsToolStripMenuItem.Click += new System.EventHandler(this.displayListOfIndividualsToolStripMenuItem_Click);
            // 
            // updateSavedAddressesToolStripMenuItem
            // 
            this.updateSavedAddressesToolStripMenuItem.Name = "updateSavedAddressesToolStripMenuItem";
            this.updateSavedAddressesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.updateSavedAddressesToolStripMenuItem.Text = "&Update Saved Addresses";
            this.updateSavedAddressesToolStripMenuItem.Click += new System.EventHandler(this.updateSavedAddressesToolStripMenuItem_Click);
            // 
            // viewSavedAddresesToolStripMenuItem
            // 
            this.viewSavedAddresesToolStripMenuItem.Name = "viewSavedAddresesToolStripMenuItem";
            this.viewSavedAddresesToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.viewSavedAddresesToolStripMenuItem.Text = "&View Saved Addresses";
            this.viewSavedAddresesToolStripMenuItem.Click += new System.EventHandler(this.viewSavedAddresesToolStripMenuItem_Click);
            // 
            // clearSavedAddressListToolStripMenuItem
            // 
            this.clearSavedAddressListToolStripMenuItem.Name = "clearSavedAddressListToolStripMenuItem";
            this.clearSavedAddressListToolStripMenuItem.Size = new System.Drawing.Size(375, 30);
            this.clearSavedAddressListToolStripMenuItem.Text = "Clear Saved &Address List";
            this.clearSavedAddressListToolStripMenuItem.Click += new System.EventHandler(this.clearSavedAddressListToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.usingTimelineToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // usingTimelineToolStripMenuItem
            // 
            this.usingTimelineToolStripMenuItem.Name = "usingTimelineToolStripMenuItem";
            this.usingTimelineToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.usingTimelineToolStripMenuItem.Text = "Using Timeline";
            this.usingTimelineToolStripMenuItem.Click += new System.EventHandler(this.usingTimelineToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(76, 57);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1120, 35);
            this.progressBar1.TabIndex = 9;
            // 
            // btnMainFormCreatWebPage
            // 
            this.btnMainFormCreatWebPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMainFormCreatWebPage.Image = ((System.Drawing.Image)(resources.GetObject("btnMainFormCreatWebPage.Image")));
            this.btnMainFormCreatWebPage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMainFormCreatWebPage.Location = new System.Drawing.Point(1000, 674);
            this.btnMainFormCreatWebPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormCreatWebPage.Name = "btnMainFormCreatWebPage";
            this.btnMainFormCreatWebPage.Size = new System.Drawing.Size(226, 63);
            this.btnMainFormCreatWebPage.TabIndex = 11;
            this.btnMainFormCreatWebPage.Text = "&Create WEB Page";
            this.btnMainFormCreatWebPage.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnMainFormCreatWebPage, "Create The Web Application from\r\nthe current TimeLine data. Display it automatica" +
        "lly\r\nif that option is set in Settings Page.\r\n");
            this.btnMainFormCreatWebPage.UseVisualStyleBackColor = true;
            this.btnMainFormCreatWebPage.Click += new System.EventHandler(this.btnMainFormCreatWebPage_Click);
            // 
            // lblCurrentView
            // 
            this.lblCurrentView.AutoSize = true;
            this.lblCurrentView.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblCurrentView.Location = new System.Drawing.Point(72, 183);
            this.lblCurrentView.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentView.Name = "lblCurrentView";
            this.lblCurrentView.Size = new System.Drawing.Size(224, 20);
            this.lblCurrentView.TabIndex = 0;
            this.lblCurrentView.Text = "Current View                               ";
            // 
            // btnMainFormClearTextBox
            // 
            this.btnMainFormClearTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMainFormClearTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMainFormClearTextBox.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.btnMainFormClearTextBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainFormClearTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnMainFormClearTextBox.Location = new System.Drawing.Point(1132, 115);
            this.btnMainFormClearTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnMainFormClearTextBox.Name = "btnMainFormClearTextBox";
            this.btnMainFormClearTextBox.Size = new System.Drawing.Size(44, 31);
            this.btnMainFormClearTextBox.TabIndex = 12;
            this.btnMainFormClearTextBox.Text = "X";
            this.btnMainFormClearTextBox.UseVisualStyleBackColor = false;
            this.btnMainFormClearTextBox.Click += new System.EventHandler(this.btnMainFormClearTextBox_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(862, 191);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(65, 29);
            this.btnSearch.TabIndex = 14;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1281, 926);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnMainFormClearTextBox);
            this.Controls.Add(this.lblCurrentView);
            this.Controls.Add(this.btnMainFormCreatWebPage);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.gbMainForm);
            this.Controls.Add(this.rtbMainFormResults);
            this.Controls.Add(this.lblMainFormFilename);
            this.Controls.Add(this.tbMainFormFilename);
            this.Controls.Add(this.btnmainFormClose);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(1294, 954);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Time Line";
            this.toolTip1.SetToolTip(this, "Load a GED File Exported  from\r\nFamily Tree maker\r\n");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.cmenu1.ResumeLayout(false);
            this.gbMainForm.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnmainFormClose;
        private System.Windows.Forms.TextBox tbMainFormFilename;
        private System.Windows.Forms.Label lblMainFormFilename;
        private System.Windows.Forms.RichTextBox rtbMainFormResults;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnMainFormParse;
        private System.Windows.Forms.Button btnMainFormViewFile;
        private System.Windows.Forms.GroupBox gbMainForm;
        private System.Windows.Forms.Button btnMainFormEdit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.Button btnJscript;
        private System.Windows.Forms.Button btnMainFormCreatWebPage;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1MainForm;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblCurrentView;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeDuplicatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEntriesWithMissingDatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEntriesWithBadGeocodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMissingDatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBadGeocodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDuplicatesToolStripMenuItem;
        private System.Windows.Forms.Button btnMainFormAutoFix;
        private System.Windows.Forms.ContextMenuStrip cmenu1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem averageLongetivityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSavedConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usingTimelineToolStripMenuItem;
        private System.Windows.Forms.Button btnMainFormClearTextBox;
        private System.Windows.Forms.ToolStripMenuItem displayListOfIndividualsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showEventsForIndividualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAddressFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAddressFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateSavedAddressesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewSavedAddresesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSavedAddressListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editBeforePrintingToolStripMenuItem;
        private System.Windows.Forms.Button btnSearch;
    }
}

