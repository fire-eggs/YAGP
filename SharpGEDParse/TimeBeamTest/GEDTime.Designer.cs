namespace TimeBeamTest
{
    partial class GEDTime
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ScrollbarH = new System.Windows.Forms.HScrollBar();
            this.ScrollbarV = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // ScrollbarH
            // 
            this.ScrollbarH.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ScrollbarH.LargeChange = 1;
            this.ScrollbarH.Location = new System.Drawing.Point(0, 419);
            this.ScrollbarH.Maximum = 0;
            this.ScrollbarH.Name = "ScrollbarH";
            this.ScrollbarH.Size = new System.Drawing.Size(415, 10);
            this.ScrollbarH.TabIndex = 0;
            // 
            // ScrollbarV
            // 
            this.ScrollbarV.Dock = System.Windows.Forms.DockStyle.Right;
            this.ScrollbarV.LargeChange = 1;
            this.ScrollbarV.Location = new System.Drawing.Point(405, 0);
            this.ScrollbarV.Maximum = 0;
            this.ScrollbarV.Name = "ScrollbarV";
            this.ScrollbarV.Size = new System.Drawing.Size(10, 419);
            this.ScrollbarV.TabIndex = 1;
            // 
            // GEDTime
            // 
            this.Controls.Add(this.ScrollbarV);
            this.Controls.Add(this.ScrollbarH);
            this.Name = "GEDTime";
            this.Size = new System.Drawing.Size(415, 429);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.HScrollBar ScrollbarH;
        private System.Windows.Forms.VScrollBar ScrollbarV;
    }
}
