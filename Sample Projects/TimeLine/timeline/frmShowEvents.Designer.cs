namespace timeline
{
    partial class frmShowEvents
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShowEvents));
            this.btnShowEventsClose = new System.Windows.Forms.Button();
            this.rtbShowEvents = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnShowEventsClose
            // 
            this.btnShowEventsClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowEventsClose.Location = new System.Drawing.Point(657, 504);
            this.btnShowEventsClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowEventsClose.Name = "btnShowEventsClose";
            this.btnShowEventsClose.Size = new System.Drawing.Size(162, 35);
            this.btnShowEventsClose.TabIndex = 1;
            this.btnShowEventsClose.Text = "&Close";
            this.btnShowEventsClose.UseVisualStyleBackColor = true;
            this.btnShowEventsClose.Click += new System.EventHandler(this.btnShowEventsClose_Click);
            // 
            // rtbShowEvents
            // 
            this.rtbShowEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbShowEvents.ContextMenuStrip = this.contextMenuStrip1;
            this.rtbShowEvents.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbShowEvents.Location = new System.Drawing.Point(12, 12);
            this.rtbShowEvents.Name = "rtbShowEvents";
            this.rtbShowEvents.Size = new System.Drawing.Size(807, 472);
            this.rtbShowEvents.TabIndex = 2;
            this.rtbShowEvents.Text = "";
            this.rtbShowEvents.MouseClick += new System.Windows.Forms.MouseEventHandler(this.rtbShowEvents_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRecordToolStripMenuItem,
            this.deleteRecordToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(212, 97);
            // 
            // editRecordToolStripMenuItem
            // 
            this.editRecordToolStripMenuItem.Name = "editRecordToolStripMenuItem";
            this.editRecordToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.editRecordToolStripMenuItem.Text = "&Edit Record";
            this.editRecordToolStripMenuItem.Click += new System.EventHandler(this.editRecordToolStripMenuItem_Click);
            // 
            // deleteRecordToolStripMenuItem
            // 
            this.deleteRecordToolStripMenuItem.Name = "deleteRecordToolStripMenuItem";
            this.deleteRecordToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.deleteRecordToolStripMenuItem.Text = "&Delete Record";
            this.deleteRecordToolStripMenuItem.Click += new System.EventHandler(this.deleteRecordToolStripMenuItem_Click);
            // 
            // frmShowEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 553);
            this.Controls.Add(this.rtbShowEvents);
            this.Controls.Add(this.btnShowEventsClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(854, 609);
            this.Name = "frmShowEvents";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Timeline Events";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmShowEvents_FormClosing);
            this.Load += new System.EventHandler(this.frmShowEvents_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowEventsClose;
        private System.Windows.Forms.RichTextBox rtbShowEvents;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRecordToolStripMenuItem;
    }
}