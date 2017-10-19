using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace timeline
{
    
    public partial class ReplaceForm : Form
    {
        public ReplaceForm()
        {
            InitializeComponent();
        }
        // DELEGATES

        
        // Overload with delegate - prototype def in program.cs
        // Callback Delegate for ViewForm to initiate search/replace code
        public ReplaceForm(Form1.Rep r,Form1.Down d)

        {
            InitializeComponent();
            ReplaceDelegate = r; // transer a copy of the delegate to local object
            ScrollDown = d;
        }
        public string searchstring
        {
            get
            {
                return SearchString;
            }
            set
            {
                SearchString = value;
            }
        }
        public string replacestring
        {
            get
            {
                return ReplaceString;
            }
            set
            {
                ReplaceString = value;
            }

        }
        public bool matchcase
        {
            get
            {
                return MatchCase;
            }
            set
            {
                MatchCase = value;
            }
        }
        private Form1.Rep ReplaceDelegate; // a private copy of the delegate in the constructor
        private Form1.Down ScrollDown;

        
        private void btnReplaceFormCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private string SearchString = String.Empty;
        private string ReplaceString = String.Empty;
        private bool MatchCase = false;
        private int position = 0; // CHANGED FROM 1, 01-24-2013 missed 1st word
        private const int FINDNEXT = 2;
        private const int REPLACE = 3;
        private const int REPLACEALL = 4;
        private const int SCRLINES = 10;
      
      

        private void ReplaceForm_Load(object sender, EventArgs e)
        {
            if (SearchString != String.Empty)
            {
                tbFindWhat.Text = SearchString;
            }
            if (ReplaceString != String.Empty)
            {
                tbReplaceWith.Text = ReplaceString;
            }
            cbMatchCase.Checked = MatchCase;
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            
            int placeholder=0;
            SearchString = this.tbFindWhat.Text;
            placeholder = ReplaceDelegate(SearchString, ReplaceString, MatchCase, position, FINDNEXT);
            // Scroll Selection 11-19-2016
            ScrollDown();
            lblposition.Text = placeholder.ToString() + " " + SearchString;
            if (placeholder != 0)
            {
                position = placeholder+ SearchString.Length;
            }
            else
            {
                position = 0;
                MessageBox.Show("Finished searching through document.", "Search Complete", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            
        }

        private void tbFindWhat_TextChanged(object sender, EventArgs e)
        {
            SearchString = tbFindWhat.Text;
        }

        private void tbReplaceWith_TextChanged(object sender, EventArgs e)
        {
            ReplaceString = tbReplaceWith.Text;
        }

        private void cbMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            MatchCase = cbMatchCase.Checked;

        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            int placeholder = 0;
            SearchString = this.tbFindWhat.Text;
            placeholder = ReplaceDelegate(SearchString, ReplaceString, MatchCase, position, REPLACE);
            lblposition.Text = placeholder.ToString() + " " + SearchString;
            if (placeholder != 0)
            {
                position = placeholder + SearchString.Length;
            }
            else
            {
                position = 0;
                MessageBox.Show("Finished searching through document.", "Search Complete", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            if (ReplaceDelegate(SearchString, ReplaceString, MatchCase, 1, REPLACEALL) == 1)
            {
                this.Close(); // RETURNS 1 if successful, 0 if field(s) are missing
            }
        }
        // SHORTCUTS FOR REPLACE FORM
        private void ReplaceForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            const int CTRLR = 18; // NOTE CTRL: R = 18, L =12, D = 4
            const int CTRLL = 12;
            const int CTRLD = 4;
            const int CTRLA = 1;

            if (System.Windows.Forms.Control.ModifierKeys.ToString() == "Control")
            {
                int result = e.KeyChar;
                switch (result) {

                    case CTRLR:
                        this.tbFindWhat.Text = "<side>";
                        this.tbReplaceWith.Text = "right";
                        break;
                    case CTRLL:
                        this.tbFindWhat.Text = "<side>";
                        this.tbReplaceWith.Text = "left";
                        break;
                    case CTRLD:
                        this.tbFindWhat.Text = "<date>";
                        this.tbReplaceWith.Text = System.DateTime.Today.ToShortDateString();
                        break;
                    case CTRLA:
                        this.tbFindWhat.Text = "*";
                        break;
                    default:
                        break;
                }
                

            }
        }
        //ENTER KEY EVENT HANDLERS
        private void tbFindWhat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.KeyChar = (char)Keys.Tab;
                e.Handled = true;
                SendKeys.Send(e.KeyChar.ToString());
            }
        }

        private void tbReplaceWith_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.KeyChar = (char)Keys.Tab;
                e.Handled = true;
                SendKeys.Send(e.KeyChar.ToString());
            }
        }

        private void cbMatchCase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.KeyChar = (char)Keys.Tab;
                e.Handled = true;
                SendKeys.Send(e.KeyChar.ToString());
            }
        }
    }
}
