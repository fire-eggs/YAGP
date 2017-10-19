namespace timeline
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tbAPIKey = new System.Windows.Forms.TextBox();
            this.btnSettingsFormOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblSfoptions = new System.Windows.Forms.Label();
            this.cbIncludeBirths = new System.Windows.Forms.CheckBox();
            this.cbDeaths = new System.Windows.Forms.CheckBox();
            this.cbMarriages = new System.Windows.Forms.CheckBox();
            this.cbBaptisms = new System.Windows.Forms.CheckBox();
            this.cbResidences = new System.Windows.Forms.CheckBox();
            this.cbBurials = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestApi = new System.Windows.Forms.Button();
            this.cbAutoOpenHtml = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbBrowserName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbAutoRemoveDuplicates = new System.Windows.Forms.CheckBox();
            this.cbRemoveBadGeoCodes = new System.Windows.Forms.CheckBox();
            this.cbRemoveEntriesWithBadDates = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Google GeoCode API Key";
            // 
            // tbAPIKey
            // 
            this.tbAPIKey.Location = new System.Drawing.Point(162, 41);
            this.tbAPIKey.Name = "tbAPIKey";
            this.tbAPIKey.Size = new System.Drawing.Size(596, 20);
            this.tbAPIKey.TabIndex = 1;
            // 
            // btnSettingsFormOk
            // 
            this.btnSettingsFormOk.Location = new System.Drawing.Point(709, 497);
            this.btnSettingsFormOk.Name = "btnSettingsFormOk";
            this.btnSettingsFormOk.Size = new System.Drawing.Size(112, 23);
            this.btnSettingsFormOk.TabIndex = 2;
            this.btnSettingsFormOk.Text = "&Save Settings";
            this.btnSettingsFormOk.UseVisualStyleBackColor = true;
            this.btnSettingsFormOk.Click += new System.EventHandler(this.btnSettingsFormOk_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(709, 526);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "&Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSfoptions
            // 
            this.lblSfoptions.AutoSize = true;
            this.lblSfoptions.Location = new System.Drawing.Point(20, 31);
            this.lblSfoptions.Name = "lblSfoptions";
            this.lblSfoptions.Size = new System.Drawing.Size(111, 13);
            this.lblSfoptions.TabIndex = 3;
            this.lblSfoptions.Text = "Include These Events";
            // 
            // cbIncludeBirths
            // 
            this.cbIncludeBirths.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbIncludeBirths.BackColor = System.Drawing.SystemColors.Control;
            this.cbIncludeBirths.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbIncludeBirths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbIncludeBirths.Location = new System.Drawing.Point(147, 23);
            this.cbIncludeBirths.Name = "cbIncludeBirths";
            this.cbIncludeBirths.Size = new System.Drawing.Size(104, 24);
            this.cbIncludeBirths.TabIndex = 4;
            this.cbIncludeBirths.Text = "Births";
            this.cbIncludeBirths.UseVisualStyleBackColor = false;
            // 
            // cbDeaths
            // 
            this.cbDeaths.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbDeaths.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbDeaths.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbDeaths.Location = new System.Drawing.Point(147, 53);
            this.cbDeaths.Name = "cbDeaths";
            this.cbDeaths.Size = new System.Drawing.Size(104, 24);
            this.cbDeaths.TabIndex = 5;
            this.cbDeaths.Text = "Deaths";
            this.cbDeaths.UseVisualStyleBackColor = true;
            // 
            // cbMarriages
            // 
            this.cbMarriages.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMarriages.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbMarriages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMarriages.Location = new System.Drawing.Point(147, 83);
            this.cbMarriages.Name = "cbMarriages";
            this.cbMarriages.Size = new System.Drawing.Size(104, 24);
            this.cbMarriages.TabIndex = 6;
            this.cbMarriages.Text = "Marriages";
            this.cbMarriages.UseVisualStyleBackColor = true;
            // 
            // cbBaptisms
            // 
            this.cbBaptisms.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbBaptisms.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbBaptisms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBaptisms.Location = new System.Drawing.Point(147, 113);
            this.cbBaptisms.Name = "cbBaptisms";
            this.cbBaptisms.Size = new System.Drawing.Size(104, 24);
            this.cbBaptisms.TabIndex = 7;
            this.cbBaptisms.Text = "Baptisims";
            this.cbBaptisms.UseVisualStyleBackColor = true;
            // 
            // cbResidences
            // 
            this.cbResidences.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbResidences.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbResidences.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbResidences.Location = new System.Drawing.Point(147, 143);
            this.cbResidences.Name = "cbResidences";
            this.cbResidences.Size = new System.Drawing.Size(104, 24);
            this.cbResidences.TabIndex = 8;
            this.cbResidences.Text = "Residences";
            this.cbResidences.UseVisualStyleBackColor = true;
            // 
            // cbBurials
            // 
            this.cbBurials.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbBurials.FlatAppearance.CheckedBackColor = System.Drawing.Color.Lime;
            this.cbBurials.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBurials.Location = new System.Drawing.Point(147, 173);
            this.cbBurials.Name = "cbBurials";
            this.cbBurials.Size = new System.Drawing.Size(104, 24);
            this.cbBurials.TabIndex = 9;
            this.cbBurials.Text = "Burials";
            this.cbBurials.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbMarriages);
            this.groupBox1.Controls.Add(this.cbBurials);
            this.groupBox1.Controls.Add(this.lblSfoptions);
            this.groupBox1.Controls.Add(this.cbResidences);
            this.groupBox1.Controls.Add(this.cbIncludeBirths);
            this.groupBox1.Controls.Add(this.cbBaptisms);
            this.groupBox1.Controls.Add(this.cbDeaths);
            this.groupBox1.Location = new System.Drawing.Point(15, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(743, 218);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Javascript Options";
            // 
            // btnTestApi
            // 
            this.btnTestApi.Location = new System.Drawing.Point(646, 67);
            this.btnTestApi.Name = "btnTestApi";
            this.btnTestApi.Size = new System.Drawing.Size(112, 23);
            this.btnTestApi.TabIndex = 11;
            this.btnTestApi.Text = "&Test Key";
            this.toolTip1.SetToolTip(this.btnTestApi, "Click to Test API Key ");
            this.btnTestApi.UseVisualStyleBackColor = true;
            this.btnTestApi.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbAutoOpenHtml
            // 
            this.cbAutoOpenHtml.AutoSize = true;
            this.cbAutoOpenHtml.Location = new System.Drawing.Point(147, 31);
            this.cbAutoOpenHtml.Name = "cbAutoOpenHtml";
            this.cbAutoOpenHtml.Size = new System.Drawing.Size(240, 17);
            this.cbAutoOpenHtml.TabIndex = 12;
            this.cbAutoOpenHtml.Text = "Automatically Open WebApp When  Created,";
            this.cbAutoOpenHtml.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(390, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Using:";
            this.label2.Visible = false;
            // 
            // tbBrowserName
            // 
            this.tbBrowserName.AllowDrop = true;
            this.tbBrowserName.Enabled = false;
            this.tbBrowserName.Location = new System.Drawing.Point(433, 29);
            this.tbBrowserName.Name = "tbBrowserName";
            this.tbBrowserName.Size = new System.Drawing.Size(304, 20);
            this.tbBrowserName.TabIndex = 14;
            this.toolTip1.SetToolTip(this.tbBrowserName, "Drag and Drop Browser Filename");
            this.tbBrowserName.Visible = false;
            this.tbBrowserName.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbBrowserName_DragDrop);
            this.tbBrowserName.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbBrowserName_DragEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbAutoRemoveDuplicates);
            this.groupBox2.Controls.Add(this.cbRemoveBadGeoCodes);
            this.groupBox2.Controls.Add(this.cbRemoveEntriesWithBadDates);
            this.groupBox2.Controls.Add(this.cbAutoOpenHtml);
            this.groupBox2.Controls.Add(this.tbBrowserName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(15, 327);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(743, 153);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other Options";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "When AutoFix Clicked";
            // 
            // cbAutoRemoveDuplicates
            // 
            this.cbAutoRemoveDuplicates.AutoSize = true;
            this.cbAutoRemoveDuplicates.Location = new System.Drawing.Point(147, 112);
            this.cbAutoRemoveDuplicates.Name = "cbAutoRemoveDuplicates";
            this.cbAutoRemoveDuplicates.Size = new System.Drawing.Size(137, 17);
            this.cbAutoRemoveDuplicates.TabIndex = 18;
            this.cbAutoRemoveDuplicates.Text = "Purge Duplicate Entries\r\n";
            this.cbAutoRemoveDuplicates.UseVisualStyleBackColor = true;
            // 
            // cbRemoveBadGeoCodes
            // 
            this.cbRemoveBadGeoCodes.AutoSize = true;
            this.cbRemoveBadGeoCodes.Location = new System.Drawing.Point(147, 89);
            this.cbRemoveBadGeoCodes.Name = "cbRemoveBadGeoCodes";
            this.cbRemoveBadGeoCodes.Size = new System.Drawing.Size(186, 17);
            this.cbRemoveBadGeoCodes.TabIndex = 16;
            this.cbRemoveBadGeoCodes.Text = "Purge Entries with Bad GeoCodes\r\n";
            this.cbRemoveBadGeoCodes.UseVisualStyleBackColor = true;
            // 
            // cbRemoveEntriesWithBadDates
            // 
            this.cbRemoveEntriesWithBadDates.AutoSize = true;
            this.cbRemoveEntriesWithBadDates.Location = new System.Drawing.Point(147, 66);
            this.cbRemoveEntriesWithBadDates.Name = "cbRemoveEntriesWithBadDates";
            this.cbRemoveEntriesWithBadDates.Size = new System.Drawing.Size(190, 17);
            this.cbRemoveEntriesWithBadDates.TabIndex = 15;
            this.cbRemoveEntriesWithBadDates.Text = "Remove Entries with Missing dates";
            this.cbRemoveEntriesWithBadDates.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(159, 502);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(190, 13);
            this.linkLabel1.TabIndex = 16;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Creating A  Free Google Maps API Key";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 502);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "For More Information...";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 582);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnTestApi);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSettingsFormOk);
            this.Controls.Add(this.tbAPIKey);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TimeLine Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAPIKey;
        private System.Windows.Forms.Button btnSettingsFormOk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblSfoptions;
        private System.Windows.Forms.CheckBox cbIncludeBirths;
        private System.Windows.Forms.CheckBox cbDeaths;
        private System.Windows.Forms.CheckBox cbMarriages;
        private System.Windows.Forms.CheckBox cbBaptisms;
        private System.Windows.Forms.CheckBox cbResidences;
        private System.Windows.Forms.CheckBox cbBurials;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTestApi;
        private System.Windows.Forms.CheckBox cbAutoOpenHtml;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbBrowserName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbRemoveEntriesWithBadDates;
        private System.Windows.Forms.CheckBox cbRemoveBadGeoCodes;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbAutoRemoveDuplicates;
        private System.Windows.Forms.Label label4;
    }
}