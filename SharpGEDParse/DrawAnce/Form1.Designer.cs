﻿namespace DrawAnce
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGEDCOMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPerson = new System.Windows.Forms.ComboBox();
            this.rad4Gen = new System.Windows.Forms.RadioButton();
            this.rad5Gen = new System.Windows.Forms.RadioButton();
            this.radCirc = new System.Windows.Forms.RadioButton();
            this.cmbPedigree = new System.Windows.Forms.ComboBox();
            this.picTree = new System.Windows.Forms.PictureBox();
            this.btnPal = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTree)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.printToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1016, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGEDCOMToolStripMenuItem,
            this.recentFilesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openGEDCOMToolStripMenuItem
            // 
            this.openGEDCOMToolStripMenuItem.Name = "openGEDCOMToolStripMenuItem";
            this.openGEDCOMToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.openGEDCOMToolStripMenuItem.Text = "Open GEDCOM...";
            this.openGEDCOMToolStripMenuItem.Click += new System.EventHandler(this.openGEDCOMToolStripMenuItem_Click);
            // 
            // recentFilesToolStripMenuItem
            // 
            this.recentFilesToolStripMenuItem.Name = "recentFilesToolStripMenuItem";
            this.recentFilesToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.recentFilesToolStripMenuItem.Text = "Recent Files";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printPreviewToolStripMenuItem,
            this.printSettingsToolStripMenuItem,
            this.printToolStripMenuItem1});
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.printToolStripMenuItem.Text = "Print";
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Preview";
            this.printPreviewToolStripMenuItem.Click += new System.EventHandler(this.printPreviewToolStripMenuItem_Click);
            // 
            // printSettingsToolStripMenuItem
            // 
            this.printSettingsToolStripMenuItem.Name = "printSettingsToolStripMenuItem";
            this.printSettingsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.printSettingsToolStripMenuItem.Text = "Print Settings";
            this.printSettingsToolStripMenuItem.Click += new System.EventHandler(this.printSettingsToolStripMenuItem_Click);
            // 
            // printToolStripMenuItem1
            // 
            this.printToolStripMenuItem1.Name = "printToolStripMenuItem1";
            this.printToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.printToolStripMenuItem1.Text = "Print";
            this.printToolStripMenuItem1.Click += new System.EventHandler(this.printToolStripMenuItem1_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.picTree, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1016, 390);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.cmbPerson);
            this.flowLayoutPanel1.Controls.Add(this.rad4Gen);
            this.flowLayoutPanel1.Controls.Add(this.rad5Gen);
            this.flowLayoutPanel1.Controls.Add(this.radCirc);
            this.flowLayoutPanel1.Controls.Add(this.cmbPedigree);
            this.flowLayoutPanel1.Controls.Add(this.btnPal);
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(920, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a Person:";
            // 
            // cmbPerson
            // 
            this.cmbPerson.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPerson.FormattingEnabled = true;
            this.cmbPerson.Location = new System.Drawing.Point(115, 3);
            this.cmbPerson.Name = "cmbPerson";
            this.cmbPerson.Size = new System.Drawing.Size(310, 24);
            this.cmbPerson.TabIndex = 1;
            this.cmbPerson.SelectedIndexChanged += new System.EventHandler(this.cmbPerson_SelectedIndexChanged);
            // 
            // rad4Gen
            // 
            this.rad4Gen.AutoSize = true;
            this.rad4Gen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rad4Gen.Location = new System.Drawing.Point(431, 3);
            this.rad4Gen.Name = "rad4Gen";
            this.rad4Gen.Size = new System.Drawing.Size(61, 20);
            this.rad4Gen.TabIndex = 2;
            this.rad4Gen.TabStop = true;
            this.rad4Gen.Text = "4 Gen";
            this.rad4Gen.UseVisualStyleBackColor = true;
            this.rad4Gen.CheckedChanged += new System.EventHandler(this.rad4Gen_CheckedChanged);
            // 
            // rad5Gen
            // 
            this.rad5Gen.AutoSize = true;
            this.rad5Gen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rad5Gen.Location = new System.Drawing.Point(498, 3);
            this.rad5Gen.Name = "rad5Gen";
            this.rad5Gen.Size = new System.Drawing.Size(61, 20);
            this.rad5Gen.TabIndex = 3;
            this.rad5Gen.TabStop = true;
            this.rad5Gen.Text = "5 Gen";
            this.rad5Gen.UseVisualStyleBackColor = true;
            this.rad5Gen.CheckedChanged += new System.EventHandler(this.rad4Gen_CheckedChanged);
            // 
            // radCirc
            // 
            this.radCirc.AutoSize = true;
            this.radCirc.Location = new System.Drawing.Point(565, 3);
            this.radCirc.Name = "radCirc";
            this.radCirc.Size = new System.Drawing.Size(49, 20);
            this.radCirc.TabIndex = 4;
            this.radCirc.TabStop = true;
            this.radCirc.Text = "Circ";
            this.radCirc.UseVisualStyleBackColor = true;
            this.radCirc.CheckedChanged += new System.EventHandler(this.rad4Gen_CheckedChanged);
            // 
            // cmbPedigree
            // 
            this.cmbPedigree.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPedigree.Enabled = false;
            this.cmbPedigree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPedigree.FormattingEnabled = true;
            this.cmbPedigree.Location = new System.Drawing.Point(620, 3);
            this.cmbPedigree.Name = "cmbPedigree";
            this.cmbPedigree.Size = new System.Drawing.Size(196, 24);
            this.cmbPedigree.TabIndex = 5;
            this.cmbPedigree.SelectedIndexChanged += new System.EventHandler(this.cmbPedigree_SelectedIndexChanged);
            // 
            // picTree
            // 
            this.picTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picTree.Location = new System.Drawing.Point(3, 38);
            this.picTree.Name = "picTree";
            this.picTree.Size = new System.Drawing.Size(1010, 349);
            this.picTree.TabIndex = 1;
            this.picTree.TabStop = false;
            this.picTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picTree_MouseClick);
            this.picTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTree_MouseMove);
            // 
            // btnPal
            // 
            this.btnPal.Location = new System.Drawing.Point(822, 3);
            this.btnPal.Name = "btnPal";
            this.btnPal.Size = new System.Drawing.Size(75, 23);
            this.btnPal.TabIndex = 6;
            this.btnPal.Text = "Pal";
            this.btnPal.UseVisualStyleBackColor = true;
            this.btnPal.Click += new System.EventHandler(this.btnPal_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 414);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGEDCOMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPerson;
        private System.Windows.Forms.PictureBox picTree;
        private System.Windows.Forms.RadioButton rad4Gen;
        private System.Windows.Forms.RadioButton rad5Gen;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem1;
        private System.Windows.Forms.ComboBox cmbPedigree;
        private System.Windows.Forms.RadioButton radCirc;
        private System.Windows.Forms.Button btnPal;
    }
}

