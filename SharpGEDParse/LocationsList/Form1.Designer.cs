namespace LocationsList
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
            this.LoadGED = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.AllEvents = new System.Windows.Forms.RadioButton();
            this.SelectedEvents = new System.Windows.Forms.RadioButton();
            this.EventsPick = new System.Windows.Forms.TreeView();
            this.LocOnly = new System.Windows.Forms.RadioButton();
            this.LocSurname = new System.Windows.Forms.RadioButton();
            this.LocPeople = new System.Windows.Forms.RadioButton();
            this.eventsHolder = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.OutFileSelect = new System.Windows.Forms.Button();
            this.OutFileDisplay = new System.Windows.Forms.Label();
            this.DoIt = new System.Windows.Forms.Button();
            this.CloseIt = new System.Windows.Forms.Button();
            this.eventsHolder.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadGED
            // 
            this.LoadGED.Location = new System.Drawing.Point(13, 13);
            this.LoadGED.Name = "LoadGED";
            this.LoadGED.Size = new System.Drawing.Size(109, 23);
            this.LoadGED.TabIndex = 0;
            this.LoadGED.Text = "Load GEDCOM";
            this.LoadGED.UseVisualStyleBackColor = true;
            this.LoadGED.Click += new System.EventHandler(this.LoadGED_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // AllEvents
            // 
            this.AllEvents.AutoSize = true;
            this.AllEvents.Checked = true;
            this.AllEvents.Location = new System.Drawing.Point(3, 3);
            this.AllEvents.Name = "AllEvents";
            this.AllEvents.Size = new System.Drawing.Size(72, 17);
            this.AllEvents.TabIndex = 4;
            this.AllEvents.TabStop = true;
            this.AllEvents.Text = "All Events";
            this.AllEvents.UseVisualStyleBackColor = true;
            // 
            // SelectedEvents
            // 
            this.SelectedEvents.AutoSize = true;
            this.SelectedEvents.Location = new System.Drawing.Point(3, 27);
            this.SelectedEvents.Name = "SelectedEvents";
            this.SelectedEvents.Size = new System.Drawing.Size(106, 17);
            this.SelectedEvents.TabIndex = 5;
            this.SelectedEvents.TabStop = true;
            this.SelectedEvents.Text = "Selected Events:";
            this.SelectedEvents.UseVisualStyleBackColor = true;
            // 
            // EventsPick
            // 
            this.EventsPick.CheckBoxes = true;
            this.EventsPick.Location = new System.Drawing.Point(3, 51);
            this.EventsPick.Name = "EventsPick";
            this.EventsPick.Size = new System.Drawing.Size(204, 211);
            this.EventsPick.TabIndex = 6;
            this.EventsPick.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.EventsPick_AfterCheck);
            // 
            // LocOnly
            // 
            this.LocOnly.AutoSize = true;
            this.LocOnly.Checked = true;
            this.LocOnly.Location = new System.Drawing.Point(3, 3);
            this.LocOnly.Name = "LocOnly";
            this.LocOnly.Size = new System.Drawing.Size(95, 17);
            this.LocOnly.TabIndex = 7;
            this.LocOnly.TabStop = true;
            this.LocOnly.Text = "Locations Only";
            this.LocOnly.UseVisualStyleBackColor = true;
            // 
            // LocSurname
            // 
            this.LocSurname.AutoSize = true;
            this.LocSurname.Location = new System.Drawing.Point(3, 37);
            this.LocSurname.Name = "LocSurname";
            this.LocSurname.Size = new System.Drawing.Size(142, 17);
            this.LocSurname.TabIndex = 8;
            this.LocSurname.TabStop = true;
            this.LocSurname.Text = "Locations and Surnames";
            this.LocSurname.UseVisualStyleBackColor = true;
            // 
            // LocPeople
            // 
            this.LocPeople.AutoSize = true;
            this.LocPeople.Location = new System.Drawing.Point(3, 72);
            this.LocPeople.Name = "LocPeople";
            this.LocPeople.Size = new System.Drawing.Size(128, 17);
            this.LocPeople.TabIndex = 9;
            this.LocPeople.TabStop = true;
            this.LocPeople.Text = "Locations and People";
            this.LocPeople.UseVisualStyleBackColor = true;
            // 
            // eventsHolder
            // 
            this.eventsHolder.Controls.Add(this.AllEvents);
            this.eventsHolder.Controls.Add(this.SelectedEvents);
            this.eventsHolder.Controls.Add(this.EventsPick);
            this.eventsHolder.Location = new System.Drawing.Point(13, 59);
            this.eventsHolder.Name = "eventsHolder";
            this.eventsHolder.Size = new System.Drawing.Size(215, 268);
            this.eventsHolder.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LocOnly);
            this.panel1.Controls.Add(this.LocSurname);
            this.panel1.Controls.Add(this.LocPeople);
            this.panel1.Location = new System.Drawing.Point(247, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 185);
            this.panel1.TabIndex = 11;
            // 
            // OutFileSelect
            // 
            this.OutFileSelect.Location = new System.Drawing.Point(13, 334);
            this.OutFileSelect.Name = "OutFileSelect";
            this.OutFileSelect.Size = new System.Drawing.Size(109, 23);
            this.OutFileSelect.TabIndex = 12;
            this.OutFileSelect.Text = "Write to...";
            this.OutFileSelect.UseVisualStyleBackColor = true;
            this.OutFileSelect.Click += new System.EventHandler(this.OutFileSelect_Click);
            // 
            // OutFileDisplay
            // 
            this.OutFileDisplay.AutoSize = true;
            this.OutFileDisplay.Location = new System.Drawing.Point(137, 339);
            this.OutFileDisplay.Name = "OutFileDisplay";
            this.OutFileDisplay.Size = new System.Drawing.Size(0, 13);
            this.OutFileDisplay.TabIndex = 13;
            // 
            // DoIt
            // 
            this.DoIt.Location = new System.Drawing.Point(164, 376);
            this.DoIt.Name = "DoIt";
            this.DoIt.Size = new System.Drawing.Size(75, 23);
            this.DoIt.TabIndex = 14;
            this.DoIt.Text = "Do It";
            this.DoIt.UseVisualStyleBackColor = true;
            this.DoIt.Click += new System.EventHandler(this.DoIt_Click);
            // 
            // CloseIt
            // 
            this.CloseIt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseIt.Location = new System.Drawing.Point(427, 376);
            this.CloseIt.Name = "CloseIt";
            this.CloseIt.Size = new System.Drawing.Size(75, 23);
            this.CloseIt.TabIndex = 15;
            this.CloseIt.Text = "Close";
            this.CloseIt.UseVisualStyleBackColor = true;
            this.CloseIt.Click += new System.EventHandler(this.CloseIt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 411);
            this.Controls.Add(this.CloseIt);
            this.Controls.Add(this.DoIt);
            this.Controls.Add(this.OutFileDisplay);
            this.Controls.Add(this.OutFileSelect);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.eventsHolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LoadGED);
            this.Name = "Form1";
            this.Text = "GEDCOM Location Lister";
            this.eventsHolder.ResumeLayout(false);
            this.eventsHolder.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadGED;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton AllEvents;
        private System.Windows.Forms.RadioButton SelectedEvents;
        private System.Windows.Forms.TreeView EventsPick;
        private System.Windows.Forms.RadioButton LocOnly;
        private System.Windows.Forms.RadioButton LocSurname;
        private System.Windows.Forms.RadioButton LocPeople;
        private System.Windows.Forms.Panel eventsHolder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button OutFileSelect;
        private System.Windows.Forms.Label OutFileDisplay;
        private System.Windows.Forms.Button DoIt;
        private System.Windows.Forms.Button CloseIt;
    }
}

