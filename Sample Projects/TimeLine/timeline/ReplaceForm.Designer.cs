namespace timeline
{
    partial class ReplaceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceForm));
            this.lblFindWhat = new System.Windows.Forms.Label();
            this.lblReplaceWith = new System.Windows.Forms.Label();
            this.tbFindWhat = new System.Windows.Forms.TextBox();
            this.tbReplaceWith = new System.Windows.Forms.TextBox();
            this.cbMatchCase = new System.Windows.Forms.CheckBox();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.btnReplaceFormCancel = new System.Windows.Forms.Button();
            this.lblposition = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblFindWhat
            // 
            this.lblFindWhat.AutoSize = true;
            this.lblFindWhat.Location = new System.Drawing.Point(18, 34);
            this.lblFindWhat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFindWhat.Name = "lblFindWhat";
            this.lblFindWhat.Size = new System.Drawing.Size(91, 20);
            this.lblFindWhat.TabIndex = 0;
            this.lblFindWhat.Text = "Find What?";
            // 
            // lblReplaceWith
            // 
            this.lblReplaceWith.AutoSize = true;
            this.lblReplaceWith.Location = new System.Drawing.Point(18, 126);
            this.lblReplaceWith.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReplaceWith.Name = "lblReplaceWith";
            this.lblReplaceWith.Size = new System.Drawing.Size(108, 20);
            this.lblReplaceWith.TabIndex = 1;
            this.lblReplaceWith.Text = "Replace With:";
            this.lblReplaceWith.Visible = false;
            // 
            // tbFindWhat
            // 
            this.tbFindWhat.Location = new System.Drawing.Point(22, 60);
            this.tbFindWhat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbFindWhat.Name = "tbFindWhat";
            this.tbFindWhat.Size = new System.Drawing.Size(538, 26);
            this.tbFindWhat.TabIndex = 2;
            this.toolTip1.SetToolTip(this.tbFindWhat, "Enter search text here");
            this.tbFindWhat.TextChanged += new System.EventHandler(this.tbFindWhat_TextChanged);
            this.tbFindWhat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFindWhat_KeyPress);
            // 
            // tbReplaceWith
            // 
            this.tbReplaceWith.Location = new System.Drawing.Point(152, 120);
            this.tbReplaceWith.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbReplaceWith.Name = "tbReplaceWith";
            this.tbReplaceWith.Size = new System.Drawing.Size(265, 26);
            this.tbReplaceWith.TabIndex = 3;
            this.toolTip1.SetToolTip(this.tbReplaceWith, "Enter text to replace search word(s)  here");
            this.tbReplaceWith.Visible = false;
            this.tbReplaceWith.TextChanged += new System.EventHandler(this.tbReplaceWith_TextChanged);
            this.tbReplaceWith.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbReplaceWith_KeyPress);
            // 
            // cbMatchCase
            // 
            this.cbMatchCase.AutoSize = true;
            this.cbMatchCase.Location = new System.Drawing.Point(22, 169);
            this.cbMatchCase.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbMatchCase.Name = "cbMatchCase";
            this.cbMatchCase.Size = new System.Drawing.Size(120, 24);
            this.cbMatchCase.TabIndex = 4;
            this.cbMatchCase.Text = "Match Case";
            this.toolTip1.SetToolTip(this.cbMatchCase, "Require exact match with search word case");
            this.cbMatchCase.UseVisualStyleBackColor = true;
            this.cbMatchCase.CheckedChanged += new System.EventHandler(this.cbMatchCase_CheckedChanged);
            this.cbMatchCase.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbMatchCase_KeyPress);
            // 
            // btnFindNext
            // 
            this.btnFindNext.Location = new System.Drawing.Point(450, 120);
            this.btnFindNext.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(112, 35);
            this.btnFindNext.TabIndex = 5;
            this.btnFindNext.Text = "&Find Next";
            this.toolTip1.SetToolTip(this.btnFindNext, "Find the next instance of the search word");
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(306, 160);
            this.btnReplace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(112, 35);
            this.btnReplace.TabIndex = 6;
            this.btnReplace.Text = "&Replace";
            this.toolTip1.SetToolTip(this.btnReplace, "Replace the current word found");
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Visible = false;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(306, 205);
            this.btnReplaceAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(112, 35);
            this.btnReplaceAll.TabIndex = 7;
            this.btnReplaceAll.Text = "Replace &All";
            this.toolTip1.SetToolTip(this.btnReplaceAll, "Replace all instances of the search word in the document");
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Visible = false;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // btnReplaceFormCancel
            // 
            this.btnReplaceFormCancel.Location = new System.Drawing.Point(450, 163);
            this.btnReplaceFormCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReplaceFormCancel.Name = "btnReplaceFormCancel";
            this.btnReplaceFormCancel.Size = new System.Drawing.Size(112, 35);
            this.btnReplaceFormCancel.TabIndex = 8;
            this.btnReplaceFormCancel.Text = "&Cancel";
            this.toolTip1.SetToolTip(this.btnReplaceFormCancel, "Cancel search and replace");
            this.btnReplaceFormCancel.UseVisualStyleBackColor = true;
            this.btnReplaceFormCancel.Click += new System.EventHandler(this.btnReplaceFormCancel_Click);
            // 
            // lblposition
            // 
            this.lblposition.AutoSize = true;
            this.lblposition.Location = new System.Drawing.Point(147, 215);
            this.lblposition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblposition.Name = "lblposition";
            this.lblposition.Size = new System.Drawing.Size(51, 20);
            this.lblposition.TabIndex = 9;
            this.lblposition.Text = "label1";
            this.lblposition.Visible = false;
            // 
            // ReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 215);
            this.Controls.Add(this.lblposition);
            this.Controls.Add(this.btnReplaceFormCancel);
            this.Controls.Add(this.btnReplaceAll);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnFindNext);
            this.Controls.Add(this.cbMatchCase);
            this.Controls.Add(this.tbReplaceWith);
            this.Controls.Add(this.tbFindWhat);
            this.Controls.Add(this.lblReplaceWith);
            this.Controls.Add(this.lblFindWhat);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximumSize = new System.Drawing.Size(594, 271);
            this.MinimumSize = new System.Drawing.Size(594, 271);
            this.Name = "ReplaceForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Current View";
            this.Load += new System.EventHandler(this.ReplaceForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReplaceForm_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFindWhat;
        private System.Windows.Forms.Label lblReplaceWith;
        private System.Windows.Forms.TextBox tbFindWhat;
        private System.Windows.Forms.TextBox tbReplaceWith;
        private System.Windows.Forms.CheckBox cbMatchCase;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.Button btnReplaceFormCancel;
        private System.Windows.Forms.Label lblposition;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}