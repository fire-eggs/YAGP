using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Documents;
using System.Windows.Controls;

namespace timeline
{
    
    
    
    public partial class EditForm : Form
    {
        // EditForm ver 2.0  12-14-2012
        // Calling Using SHOWDIALOG to access DocumentText, as in
        // ef.ShowDialog();
        // text = ef.DocumentText;
        // ef.Dispose(); - caller must dispose of form if called this way
        // Call using SHOW() to use independently of caller
        //
        // REVISIONS:
        // 12-14-12 Ver 2 various bugs fixed
        

        // Base Constructor
        public EditForm()
        {
            InitializeComponent();
            
        }
        
        // Accessor for RTB Text or RichText, depending on AllowRtf
        // Don't return Rtf if document is empty (returns default formatting codes)
        public string DocumentText
        {
            get
            {
                if (ReturnRtf == true && (this.rtbMainForm1.Text.Length > 0))
                {
                    return this.rtbMainForm1.Rtf;
                }
                else
                {
                    return this.rtbMainForm1.Text;
                }
            
            }
            set
            {
                if (ReturnRtf == true)
                {
                    this.rtbMainForm1.Clear();
                    this.rtbMainForm1.Rtf = value;
                }
                else
                {
                    this.rtbMainForm1.Text = value;
                }
            }
        }
        // Accessor for Title
        public string Title
        {
            
            set
            {
                this.Text=value;
            }
        }
        // Enable/Disable File Load & Save
        public bool AllowDiscAccess
        {
            get
            {
                return AllowDisc;
            }
            set
            {
                AllowDisc = value;
            }
        }// Controls text or RTF returned to Global Variable
        public bool AllowRtf
        {
            get
            {
                return ReturnRtf;
            }
            set
            {
                ReturnRtf = value;
            }

        }
        
             
        // DEFAULT SETTINGS & FLAGS
        private bool AllowDisc = true; // Allow saving & loading files
        private bool ReturnRtf = true; // return Rich Text if true
        private string FileName = "";
        private bool FileSaved = false;
        private bool DocumentChanged = false;
        private Font StartingFont = new Font("Microsoft San Serif", 12.0f, FontStyle.Regular);

       
        // Loads Working File
        private void LoadText(string filename)
        {
            string fileextension = GetFileExtension(filename);
            if (fileextension == "rtf")
            {
                this.rtbMainForm1.Clear();
                this.rtbMainForm1.LoadFile(filename, RichTextBoxStreamType.RichText);
                UpdateDisplay();
            }
            else
            {
                if (fileextension == "txt" || fileextension == "tex")
                {
                    this.rtbMainForm1.Clear();
                    this.rtbMainForm1.LoadFile(filename, RichTextBoxStreamType.PlainText);
                    UpdateDisplay();
                   
                }
                else
                {
                    MessageBox.Show("Invalid Filename", "The filename and/or extension is incorrect",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

        }
        // RETURNS 3 LETTER FILE EXTENSION 
        private string GetFileExtension(string filename)
        {
            string tempstr = null;
            int x,y;

            if (filename[filename.Length - 4] == '.')
            {
                y =0;
                for (x = 1; x <= 3; x++)
                {
                    y = filename.Length - (4 - x);
                    tempstr += filename[y];
                }
            }
            tempstr = tempstr.ToLower();
            return tempstr;
        }
        // Change Working Font
        private void ChangeFont()
        {
            DialogResult result = fontDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.rtbMainForm1.SelectionFont = fontDialog1.Font;
                UpdateDisplay();
            };
           
         // Change Text Color  
        }
        private void ChangeColor()
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.rtbMainForm1.SelectionColor = colorDialog1.Color;
            };
            
            UpdateDisplay();
        }
        // Change background Color
        private void ChangeBackground()
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.rtbMainForm1.SelectionBackColor = colorDialog1.Color;
            };
            UpdateDisplay();
            
            
        }
        // Maintain Correct Button Settings depending on text selected
        private void UpdateDisplay()
        {
            
            System.Drawing.FontStyle newstyle = new FontStyle(); /* stop null exception */
            if (rtbMainForm1.SelectionFont == null) /* null if more than 1 font is included*/
            {
                return;
            }
            newstyle = rtbMainForm1.SelectionFont.Style;
            if ((newstyle & FontStyle.Bold) > 0)
            {
                this.cbMainFormBold.Checked = true;
            }
            else
            {
                this.cbMainFormBold.Checked = false;
            }
            if ((newstyle & FontStyle.Italic) > 0)
            {
                this.cbmainFormItalic.Checked = true;
            }
            else
            {
                this.cbmainFormItalic.Checked = false;
            }
            if (((byte)newstyle & (byte)FontStyle.Underline) > 0)
            {
                this.cb2MainFormUnderline.Checked = true;
            }
            else
            {
                this.cb2MainFormUnderline.Checked = false;
            }
            if (this.rtbMainForm1.SelectionAlignment == HorizontalAlignment.Left)
            {
                rbMainFormLeft.Checked = true;
            }
            else
            {
                if (this.rtbMainForm1.SelectionAlignment == HorizontalAlignment.Center)
                {
                    rbMainFormCenter.Checked = true;
                }
                else
                {
                    rbMainFormRight.Checked = true;
                }
            }
            lblCurrentFont.Text = this.rtbMainForm1.SelectionFont.Name.ToString() + " ("+
                this.rtbMainForm1.SelectionFont.SizeInPoints.ToString()+ " Pts)";
            // reversed labels to make overlap work correctly 12-13-12
            lblBackgroundColor.ForeColor = this.rtbMainForm1.SelectionColor;
            lblBackgroundColor.BackColor = this.rtbMainForm1.SelectionColor;
            lblCurrentColor.ForeColor = this.rtbMainForm1.SelectionBackColor;
            lblCurrentColor.BackColor = this.rtbMainForm1.SelectionBackColor;
            lblMainFormCharCount.Text = this.rtbMainForm1.Text.Length.ToString();
                    
        }


        // Match Current Style and Justification to Button Settings
        private void UpdateFontStyle()
        {
            System.Drawing.Font currentfont = this.rtbMainForm1.SelectionFont;
            System.Drawing.FontStyle newstyle = FontStyle.Regular;
            
            if (cbMainFormBold.Checked)
            {
                newstyle = newstyle | FontStyle.Bold;
            }
            if (cbmainFormItalic.Checked)
            {
                newstyle = newstyle | FontStyle.Italic;
            }
            if (cb2MainFormUnderline.Checked)
            {
                newstyle = newstyle | FontStyle.Underline;
            }
            if (rbMainFormLeft.Checked)
            {
                rtbMainForm1.SelectionAlignment = HorizontalAlignment.Left;
            }
            else
            {
                if (rbMainFormCenter.Checked)
                {
                    rtbMainForm1.SelectionAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    rtbMainForm1.SelectionAlignment = HorizontalAlignment.Right;
                }
            }
            try
            {
                rtbMainForm1.SelectionFont = new System.Drawing.Font(currentfont.FontFamily,
                    currentfont.Size, newstyle);  // add test to be sure font supports the current style*/
            }
            catch (Exception e)
            {
            }

                this.rtbMainForm1.Focus();
        }
        // CUT
        private void CutText()
        {
            this.rtbMainForm1.Cut();
        }
        // COPY
        private void CopyText()
        {
            this.rtbMainForm1.Copy();
        }
        // PASTE
        private void PasteText()
        {
            this.rtbMainForm1.Paste();
        }
        // CLEAR ALL TEXT
        private void ClearText()
        {
            this.rtbMainForm1.Clear();
            this.rtbMainForm1.Focus();
        }
       
        
        // SAVE FILE HANDLER
        private bool SaveWorkingFile(string DefaultFileName)
        {
            saveFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog1.DefaultExt = "rtf";
            saveFileDialog1.Title = "Save Working File";
            saveFileDialog1.Filter = "Rich text Files (*.rtf)|*.rtf;*.RTF|Text Files (*.txt)|*.txt;*.TXT";
            saveFileDialog1.FilterIndex = 1;
            if (DefaultFileName.Length != 0)
            {
                saveFileDialog1.FileName = DefaultFileName;
            }
            bool FileSaved = false;
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel && 
                saveFileDialog1.FileName.Length > 0)
            {
                FileName=saveFileDialog1.FileName;
                if (GetFileExtension(FileName) == "rtf")
               {
                rtbMainForm1.SaveFile(FileName,RichTextBoxStreamType.RichText);
                FileSaved = true;
                  
               }
                else
                {
                    if (GetFileExtension(FileName) == "txt")
                    {
                        rtbMainForm1.SaveFile(FileName,RichTextBoxStreamType.PlainText);
                        FileSaved = true;
                        
                    }
                }
            }
            return FileSaved;
        }


        // Form Control Handlers

        private void btnMainFormQuit_Click(object sender, EventArgs e)
        {
            
                    
            if (FileSaved == false && rtbMainForm1.Text.Length != 0 &&
                DocumentChanged == true && AllowDisc == true)
            {
               
                if (MessageBox.Show("Do you want to save the working file?", "File Changed",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveWorkingFile(FileName);
                        
                }
                
            }
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            if (!(this.DocumentText.Length > 0))
            {
                this.rtbMainForm1.Font = StartingFont;
            }
            this.lblCurrentColor.Text = "current color";
            this.lblCurrentColor.BackColor = this.rtbMainForm1.SelectionColor;
            this.lblCurrentColor.ForeColor = this.rtbMainForm1.SelectionColor;
            this.lblBackgroundColor.ForeColor = this.rtbMainForm1.SelectionBackColor;
            this.lblBackgroundColor.BackColor = this.rtbMainForm1.SelectionBackColor;
            FileSaved = false;
            DocumentChanged = false;
            this.rbMainFormLeft.Checked = true;
            if (!AllowDisc)
            {
                loadToolStripMenuItem1.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                this.btnMainFormQuit.Text = "&Done";
                
            }
           
            
            
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
        }

        private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Select A File";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Rich text Files (*.rtf)|*.rtf;*.RTF|Text Files (*.txt, *.tex)|*.txt;*.TXT;*.tex;*.TEX";
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                FileName = openFileDialog1.FileName;
                LoadText(FileName);
                this.Text = "Editing - " + FileName;

            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeFont();

        }

        private void btnMainFormErase_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.Clear();
            this.Text = "Simple Editor";
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void textAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeFont();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeColor();
        }

        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeColor();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.Undo();

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutText();
        }

        private void pastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                PasteText();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                pastToolStripMenuItem.Enabled = true;
            }
            else { pastToolStripMenuItem.Enabled = false; }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.SelectedText = "";
        }

        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.Undo();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutText();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyText();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                PasteText();
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.SelectedText = "";
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rtbMainForm1.SelectionStart = 0;
            this.rtbMainForm1.SelectionLength = this.rtbMainForm1.Text.Length;

        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBackground();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _defaultfilename = "";
            if (FileName.Length != 0)
            {
                _defaultfilename=FileName;
            }
            if (rtbMainForm1.Text.Length == 0)
            {
                MessageBox.Show("The Edit Window is empty", "File Not Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                FileSaved = false;
                return;   
            }
            if (SaveWorkingFile(_defaultfilename) == true)
            {
                MessageBox.Show("The working file has been saved","File Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                FileSaved = true;
                DocumentChanged = false;
            }
            else
            {
                MessageBox.Show("The File was not saved.","Error Saving File",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                FileSaved = false;
            }
         
        }

        private void rtbMainForm1_TextChanged(object sender, EventArgs e)
        {
            DocumentChanged = true;
        }

        private void cbMainFormBold_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void cbmainFormItalic_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void cbMainFormUnderline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void cb2MainFormUnderline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void rbMainFormCenter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void rbMainFormLeft_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void rbMainFormRight_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFontStyle();
        }

        private void rtbMainForm1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear everything and start over?", "Delete All",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                ClearText();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Start over with a blank document", "New Document",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                ClearText();
            }
        }
        

       
    }
    
}
