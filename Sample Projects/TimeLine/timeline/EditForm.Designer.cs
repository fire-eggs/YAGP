namespace timeline
{
    partial class EditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditForm));
            this.btnMainFormQuit = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.rtbMainForm1 = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentFont = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.lblCurrentColor = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblMainFormChars = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblMainFormCharCount = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblBackgroundColor = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.cbMainFormBold = new System.Windows.Forms.CheckBox();
            this.cbmainFormItalic = new System.Windows.Forms.CheckBox();
            this.cbMainFormUnderline = new System.Windows.Forms.CheckBox();
            this.cb2MainFormUnderline = new System.Windows.Forms.CheckBox();
            this.panMainFormJustify = new System.Windows.Forms.Panel();
            this.rbMainFormRight = new System.Windows.Forms.RadioButton();
            this.rbMainFormLeft = new System.Windows.Forms.RadioButton();
            this.rbMainFormCenter = new System.Windows.Forms.RadioButton();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panMainFormJustify.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMainFormQuit
            // 
            this.btnMainFormQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMainFormQuit.Location = new System.Drawing.Point(572, 5);
            this.btnMainFormQuit.Name = "btnMainFormQuit";
            this.btnMainFormQuit.Size = new System.Drawing.Size(86, 30);
            this.btnMainFormQuit.TabIndex = 0;
            this.btnMainFormQuit.Text = "Quit";
            this.btnMainFormQuit.UseVisualStyleBackColor = true;
            this.btnMainFormQuit.Click += new System.EventHandler(this.btnMainFormQuit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(695, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.printToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.loadToolStripMenuItem.Text = "File";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem1
            // 
            this.loadToolStripMenuItem1.Name = "loadToolStripMenuItem1";
            this.loadToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.loadToolStripMenuItem1.Text = "&Open";
            this.loadToolStripMenuItem1.Click += new System.EventHandler(this.loadToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Enabled = false;
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.printToolStripMenuItem.Text = "&Print";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pastToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteAllToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pastToolStripMenuItem
            // 
            this.pastToolStripMenuItem.Enabled = false;
            this.pastToolStripMenuItem.Name = "pastToolStripMenuItem";
            this.pastToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.pastToolStripMenuItem.Text = "&Paste";
            this.pastToolStripMenuItem.Click += new System.EventHandler(this.pastToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.selectAllToolStripMenuItem.Text = "&Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.deleteAllToolStripMenuItem.Text = "C&lear";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.colorToolStripMenuItem,
            this.backgroundColorToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.optionsToolStripMenuItem.Text = "Format";
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.colorToolStripMenuItem.Text = "Text Color";
            this.colorToolStripMenuItem.Click += new System.EventHandler(this.colorToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // rtbMainForm1
            // 
            this.rtbMainForm1.AcceptsTab = true;
            this.rtbMainForm1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbMainForm1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbMainForm1.ContextMenuStrip = this.contextMenuStrip1;
            this.rtbMainForm1.HideSelection = false;
            this.rtbMainForm1.Location = new System.Drawing.Point(12, 27);
            this.rtbMainForm1.Name = "rtbMainForm1";
            this.rtbMainForm1.Size = new System.Drawing.Size(671, 359);
            this.rtbMainForm1.TabIndex = 7;
            this.rtbMainForm1.Text = "";
            this.rtbMainForm1.SelectionChanged += new System.EventHandler(this.rtbMainForm1_SelectionChanged);
            this.rtbMainForm1.TextChanged += new System.EventHandler(this.rtbMainForm1_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textAttributesToolStripMenuItem,
            this.textColorToolStripMenuItem,
            this.undoToolStripMenuItem1,
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 158);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // textAttributesToolStripMenuItem
            // 
            this.textAttributesToolStripMenuItem.Name = "textAttributesToolStripMenuItem";
            this.textAttributesToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.textAttributesToolStripMenuItem.Text = "Text Attributes";
            this.textAttributesToolStripMenuItem.Click += new System.EventHandler(this.textAttributesToolStripMenuItem_Click);
            // 
            // textColorToolStripMenuItem
            // 
            this.textColorToolStripMenuItem.Name = "textColorToolStripMenuItem";
            this.textColorToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.textColorToolStripMenuItem.Text = "Text Color";
            this.textColorToolStripMenuItem.Click += new System.EventHandler(this.textColorToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem1
            // 
            this.undoToolStripMenuItem1.Name = "undoToolStripMenuItem1";
            this.undoToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.undoToolStripMenuItem1.Text = "Undo";
            this.undoToolStripMenuItem1.Click += new System.EventHandler(this.undoToolStripMenuItem1_Click);
            // 
            // cutToolStripMenuItem1
            // 
            this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            this.cutToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.cutToolStripMenuItem1.Text = "Cut";
            this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem1_Click);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // lblCurrentFont
            // 
            this.lblCurrentFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentFont.AutoSize = true;
            this.lblCurrentFont.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.lblCurrentFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFont.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCurrentFont.Location = new System.Drawing.Point(17, 1);
            this.lblCurrentFont.MinimumSize = new System.Drawing.Size(60, 16);
            this.lblCurrentFont.Name = "lblCurrentFont";
            this.lblCurrentFont.Size = new System.Drawing.Size(60, 16);
            this.lblCurrentFont.TabIndex = 7;
            this.lblCurrentFont.Text = "label1";
            // 
            // lblCurrentColor
            // 
            this.lblCurrentColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentColor.AutoSize = true;
            this.lblCurrentColor.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblCurrentColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCurrentColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentColor.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblCurrentColor.Location = new System.Drawing.Point(441, 12);
            this.lblCurrentColor.MaximumSize = new System.Drawing.Size(40, 25);
            this.lblCurrentColor.MinimumSize = new System.Drawing.Size(40, 25);
            this.lblCurrentColor.Name = "lblCurrentColor";
            this.lblCurrentColor.Size = new System.Drawing.Size(40, 25);
            this.lblCurrentColor.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel1.Controls.Add(this.lblMainFormChars);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lblBackgroundColor);
            this.panel1.Controls.Add(this.btnMainFormQuit);
            this.panel1.Controls.Add(this.lblCurrentColor);
            this.panel1.Location = new System.Drawing.Point(0, 392);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(695, 39);
            this.panel1.TabIndex = 11;
            // 
            // lblMainFormChars
            // 
            this.lblMainFormChars.AutoSize = true;
            this.lblMainFormChars.Location = new System.Drawing.Point(295, 7);
            this.lblMainFormChars.Name = "lblMainFormChars";
            this.lblMainFormChars.Size = new System.Drawing.Size(34, 13);
            this.lblMainFormChars.TabIndex = 1;
            this.lblMainFormChars.Text = "Chars";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.lblMainFormCharCount);
            this.panel3.Location = new System.Drawing.Point(336, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(63, 21);
            this.panel3.TabIndex = 8;
            // 
            // lblMainFormCharCount
            // 
            this.lblMainFormCharCount.AutoSize = true;
            this.lblMainFormCharCount.Location = new System.Drawing.Point(8, 1);
            this.lblMainFormCharCount.MaximumSize = new System.Drawing.Size(40, 13);
            this.lblMainFormCharCount.MinimumSize = new System.Drawing.Size(40, 13);
            this.lblMainFormCharCount.Name = "lblMainFormCharCount";
            this.lblMainFormCharCount.Size = new System.Drawing.Size(40, 13);
            this.lblMainFormCharCount.TabIndex = 0;
            this.lblMainFormCharCount.Text = "chars";
            this.lblMainFormCharCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblCurrentFont);
            this.panel2.Location = new System.Drawing.Point(10, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(264, 21);
            this.panel2.TabIndex = 10;
            // 
            // lblBackgroundColor
            // 
            this.lblBackgroundColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBackgroundColor.AutoSize = true;
            this.lblBackgroundColor.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblBackgroundColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBackgroundColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackgroundColor.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblBackgroundColor.Location = new System.Drawing.Point(428, 5);
            this.lblBackgroundColor.MaximumSize = new System.Drawing.Size(40, 25);
            this.lblBackgroundColor.MinimumSize = new System.Drawing.Size(40, 25);
            this.lblBackgroundColor.Name = "lblBackgroundColor";
            this.lblBackgroundColor.Size = new System.Drawing.Size(40, 25);
            this.lblBackgroundColor.TabIndex = 9;
            // 
            // cbMainFormBold
            // 
            this.cbMainFormBold.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMainFormBold.AutoSize = true;
            this.cbMainFormBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMainFormBold.Location = new System.Drawing.Point(162, 0);
            this.cbMainFormBold.MinimumSize = new System.Drawing.Size(26, 25);
            this.cbMainFormBold.Name = "cbMainFormBold";
            this.cbMainFormBold.Size = new System.Drawing.Size(28, 26);
            this.cbMainFormBold.TabIndex = 10;
            this.cbMainFormBold.Text = "B";
            this.cbMainFormBold.UseVisualStyleBackColor = true;
            this.cbMainFormBold.CheckedChanged += new System.EventHandler(this.cbMainFormBold_CheckedChanged);
            // 
            // cbmainFormItalic
            // 
            this.cbmainFormItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbmainFormItalic.AutoSize = true;
            this.cbmainFormItalic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbmainFormItalic.Location = new System.Drawing.Point(194, 0);
            this.cbmainFormItalic.MaximumSize = new System.Drawing.Size(26, 23);
            this.cbmainFormItalic.MinimumSize = new System.Drawing.Size(26, 25);
            this.cbmainFormItalic.Name = "cbmainFormItalic";
            this.cbmainFormItalic.Size = new System.Drawing.Size(26, 25);
            this.cbmainFormItalic.TabIndex = 12;
            this.cbmainFormItalic.Text = "I";
            this.cbmainFormItalic.UseVisualStyleBackColor = true;
            this.cbmainFormItalic.CheckedChanged += new System.EventHandler(this.cbmainFormItalic_CheckedChanged);
            // 
            // cbMainFormUnderline
            // 
            this.cbMainFormUnderline.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMainFormUnderline.AutoSize = true;
            this.cbMainFormUnderline.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMainFormUnderline.Location = new System.Drawing.Point(226, 0);
            this.cbMainFormUnderline.Name = "cbMainFormUnderline";
            this.cbMainFormUnderline.Size = new System.Drawing.Size(29, 26);
            this.cbMainFormUnderline.TabIndex = 13;
            this.cbMainFormUnderline.Text = "U";
            this.cbMainFormUnderline.UseVisualStyleBackColor = true;
            this.cbMainFormUnderline.CheckedChanged += new System.EventHandler(this.cbMainFormUnderline_CheckedChanged);
            // 
            // cb2MainFormUnderline
            // 
            this.cb2MainFormUnderline.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb2MainFormUnderline.AutoSize = true;
            this.cb2MainFormUnderline.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb2MainFormUnderline.Location = new System.Drawing.Point(226, 0);
            this.cb2MainFormUnderline.MaximumSize = new System.Drawing.Size(26, 25);
            this.cb2MainFormUnderline.MinimumSize = new System.Drawing.Size(26, 25);
            this.cb2MainFormUnderline.Name = "cb2MainFormUnderline";
            this.cb2MainFormUnderline.Size = new System.Drawing.Size(26, 25);
            this.cb2MainFormUnderline.TabIndex = 14;
            this.cb2MainFormUnderline.Text = "U";
            this.cb2MainFormUnderline.UseVisualStyleBackColor = true;
            this.cb2MainFormUnderline.CheckedChanged += new System.EventHandler(this.cb2MainFormUnderline_CheckedChanged);
            // 
            // panMainFormJustify
            // 
            this.panMainFormJustify.BackColor = System.Drawing.SystemColors.MenuBar;
            this.panMainFormJustify.Controls.Add(this.rbMainFormRight);
            this.panMainFormJustify.Controls.Add(this.rbMainFormLeft);
            this.panMainFormJustify.Controls.Add(this.rbMainFormCenter);
            this.panMainFormJustify.Location = new System.Drawing.Point(268, 0);
            this.panMainFormJustify.Name = "panMainFormJustify";
            this.panMainFormJustify.Size = new System.Drawing.Size(113, 25);
            this.panMainFormJustify.TabIndex = 15;
            // 
            // rbMainFormRight
            // 
            this.rbMainFormRight.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMainFormRight.AutoSize = true;
            this.rbMainFormRight.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rbMainFormRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbMainFormRight.BackgroundImage")));
            this.rbMainFormRight.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbMainFormRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbMainFormRight.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.rbMainFormRight.Location = new System.Drawing.Point(79, 0);
            this.rbMainFormRight.Name = "rbMainFormRight";
            this.rbMainFormRight.Size = new System.Drawing.Size(32, 25);
            this.rbMainFormRight.TabIndex = 2;
            this.rbMainFormRight.TabStop = true;
            this.rbMainFormRight.Text = "     ";
            this.rbMainFormRight.UseVisualStyleBackColor = false;
            this.rbMainFormRight.CheckedChanged += new System.EventHandler(this.rbMainFormRight_CheckedChanged);
            // 
            // rbMainFormLeft
            // 
            this.rbMainFormLeft.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMainFormLeft.AutoSize = true;
            this.rbMainFormLeft.BackColor = System.Drawing.SystemColors.GrayText;
            this.rbMainFormLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbMainFormLeft.BackgroundImage")));
            this.rbMainFormLeft.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbMainFormLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbMainFormLeft.Location = new System.Drawing.Point(41, 0);
            this.rbMainFormLeft.Name = "rbMainFormLeft";
            this.rbMainFormLeft.Size = new System.Drawing.Size(32, 25);
            this.rbMainFormLeft.TabIndex = 1;
            this.rbMainFormLeft.TabStop = true;
            this.rbMainFormLeft.Text = "     ";
            this.rbMainFormLeft.UseVisualStyleBackColor = false;
            this.rbMainFormLeft.CheckedChanged += new System.EventHandler(this.rbMainFormLeft_CheckedChanged);
            // 
            // rbMainFormCenter
            // 
            this.rbMainFormCenter.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbMainFormCenter.AutoSize = true;
            this.rbMainFormCenter.BackColor = System.Drawing.SystemColors.GrayText;
            this.rbMainFormCenter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rbMainFormCenter.BackgroundImage")));
            this.rbMainFormCenter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbMainFormCenter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbMainFormCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbMainFormCenter.Location = new System.Drawing.Point(0, 0);
            this.rbMainFormCenter.Name = "rbMainFormCenter";
            this.rbMainFormCenter.Size = new System.Drawing.Size(35, 25);
            this.rbMainFormCenter.TabIndex = 0;
            this.rbMainFormCenter.TabStop = true;
            this.rbMainFormCenter.Text = "      ";
            this.rbMainFormCenter.UseVisualStyleBackColor = false;
            this.rbMainFormCenter.CheckedChanged += new System.EventHandler(this.rbMainFormCenter_CheckedChanged);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 443);
            this.Controls.Add(this.panMainFormJustify);
            this.Controls.Add(this.cb2MainFormUnderline);
            this.Controls.Add(this.rtbMainForm1);
            this.Controls.Add(this.cbMainFormBold);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbmainFormItalic);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.cbMainFormUnderline);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Line Source File Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panMainFormJustify.ResumeLayout(false);
            this.panMainFormJustify.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMainFormQuit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.RichTextBox rtbMainForm1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem textAttributesToolStripMenuItem;
        private System.Windows.Forms.Label lblCurrentFont;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label lblCurrentColor;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.Label lblBackgroundColor;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox cbMainFormBold;
        private System.Windows.Forms.CheckBox cbmainFormItalic;
        private System.Windows.Forms.CheckBox cbMainFormUnderline;
        private System.Windows.Forms.CheckBox cb2MainFormUnderline;
        private System.Windows.Forms.Panel panMainFormJustify;
        private System.Windows.Forms.RadioButton rbMainFormRight;
        private System.Windows.Forms.RadioButton rbMainFormLeft;
        private System.Windows.Forms.RadioButton rbMainFormCenter;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblMainFormCharCount;
        private System.Windows.Forms.Label lblMainFormChars;
    }
}

