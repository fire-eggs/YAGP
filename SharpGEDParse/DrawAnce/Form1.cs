using GEDWrap;
using PrintPreview;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DrawAnce
{
    public partial class Form1 : Form
    {
        protected MruStripMenu mnuMRU;
        private IDrawGen draw4gen;
        private IDrawGen draw5gen;
        private IDrawGen drawer;

        readonly List<object> _cmbItems = new List<object>();
        readonly List<object> _cmbPedItems = new List<object>();

        private Pedigrees _pedigrees;
        private Person[] _ancIndi;

        public event EventHandler LoadGed;

        private bool _noUpdate;  // TODO stupid GUI hack: sometimes the pedigree combo update causes a redraw, and sometimes it doesn't

        public Form1()
        {
            InitializeComponent();
            cmbPerson.DisplayMember = "Text";
            cmbPerson.ValueMember = "Value";
            cmbPerson.DataSource = _cmbItems;
            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;
            LoadGed += Form1_LoadGed;
            LoadSettings(); // must go after mnuMRU init

            draw4gen = new Draw4Gen();
            draw5gen = new Draw5gen();
            rad4Gen.Checked = true;
            drawer = draw4gen;
        }

        private int tick;
        private void logit(string msg, bool first = false)
        {
            int delta = 0;
            if (first)
                tick = Environment.TickCount;
            else
                delta = Environment.TickCount - tick;
            Console.WriteLine(msg + "|" + delta);
        }

        private void Form1_LoadGed(object sender, EventArgs e)
        {
            //logit("LoadGed 1", true);
            Forest gedtrees = new Forest();
            // TODO Using LastFile is a hack... pass path in args? not as event?            
            gedtrees.LoadGEDCOM(LastFile);
            //logit("LoadGed 2");

            // populate combobox with individuals
            // www.ahnenbuch.de-AMMON has multiple individuals with the same name. Need to distinguish
            // them somehow for the combobox.
            // TODO is there a better way? unique thing for combobox selection?
            HashSet<string> comboNames = new HashSet<string>();
            foreach (var indiId in gedtrees.AllIndiIds)
            {
                Person p = gedtrees.PersonById(indiId);

                // for each person, show the # of individuals available in (first) pedigree
                Pedigrees pd = new Pedigrees(p, firstOnly:true);
                p.Ahnen = pd.GetPedigreeMax(0);

                var text = string.Format("{0} [{1}] ({2})", p.Name, indiId, p.Ahnen);
                comboNames.Add(text);
                _cmbItems.Add(new { Text=text, Value=p } );
            }
            cmbPerson.DisplayMember = "Text";
            cmbPerson.ValueMember = "Value";
            cmbPerson.DataSource = _cmbItems;
            cmbPerson.Enabled = true;
            //logit("LoadGed 4");
        }

        private void OnMRU(int number, string filename)
        {
            if (!File.Exists(filename))
            {
                mnuMRU.RemoveFile(number);
                MessageBox.Show("The file no longer exists: " + filename);
                return;
            }

            // TODO process could fail for some reason, in which case remove the file from the MRU list
            LastFile = filename;
            mnuMRU.SetFirstFile(number);
            ProcessGED(filename);
        }

        private void updatePedigreeList(Pedigrees p)
        {
            _noUpdate = true;
            int count = p.PedigreeCount;
            cmbPedigree.DataSource = null; // force rebuild when pedigree count changes
            if (count < 2)
            {
                cmbPedigree.Enabled = false;
            }
            else
            {
                // Establish list of pedigrees
                _cmbPedItems.Clear();
                for (int i = 1; i <= count; i++)
                {
                    string text = string.Format("Pedigree {0}", i);
                    _cmbPedItems.Add(new { Text=text, Value=(i-1) } );
                }
                cmbPedigree.DataSource = _cmbPedItems;
                cmbPedigree.DisplayMember = "Text";
                cmbPedigree.ValueMember = "Value";
                cmbPedigree.SelectedIndex = 0;
                cmbPedigree.Enabled = true;
            }
            _noUpdate = false;
        }

        private void TreePerson(Person val)
        {
            _pedigrees = new Pedigrees(val, firstOnly:false); // TODO currently re-calculating - any benefit from caching these?
            updatePedigreeList(_pedigrees);
            _ancIndi = _pedigrees.GetPedigree(0);
            DoAncTree();
        }

        private void DoAncTree()
        {
            if (drawer == null)
                drawer = new Draw5gen();
            drawer.AncData = _ancIndi;
            var oldImage = picTree.Image;
            picTree.Image = drawer.MakeAncTree();
            if (oldImage != null)
                oldImage.Dispose();
            drawer.AncData = null;
        }

        private void cmbPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = cmbPerson.SelectedValue as Person;
            if (val == null)
                return;
            TreePerson(val);
        }

        private void openGEDCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
			ofd.Filter = "GEDCOM files|*.ged;*.GED";
            ofd.FilterIndex = 1;
            ofd.DefaultExt = "ged";
            ofd.CheckFileExists = true;
            if (DialogResult.OK != ofd.ShowDialog(this))
            {
                return;
            }
            mnuMRU.AddFile(ofd.FileName);
            LastFile = ofd.FileName; // TODO invalid ged file
            ProcessGED(ofd.FileName);
        }

        private void ProcessGED(string gedPath)
        {
            cmbPerson.SelectedIndex = -1;
            cmbPerson.DataSource = null;
            cmbPerson.Enabled = false;
            _cmbItems.Clear();
            picTree.Image = null;
            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed(this, new EventArgs());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool HavePerson(int index)
        {
            if (index == -1 || _ancIndi[index] == null)
                return false;
            return !string.IsNullOrWhiteSpace(_ancIndi[index].Name);
        }

        private void picTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawer == null )
                picTree.Cursor = Cursors.Arrow;
            else
            {
                int index = drawer.HitIndex(e.Location);

                picTree.Cursor = !HavePerson(index) ?
                    Cursors.Arrow :
                    Cursors.Hand;
            }
        }

        private void picTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawer == null)
                return;
            int index = drawer.HitIndex(e.Location);
            if (!HavePerson(index))
                return;
            TreePerson(_ancIndi[index]);
            cmbPerson.SelectedIndex = -1;
        }

        #region Settings
        private DASettings _mysettings;

        private List<string> _fileHistory = new List<string>();

        private string LastFile
        {
            get
            {
                if (_fileHistory == null || _fileHistory.Count < 1)
                    return null;
                return _fileHistory[0]; // First entry is the most recent
            }
            set
            {
                // Make sure to wipe any older instance
                _fileHistory.Remove(value);
                _fileHistory.Insert(0, value); // First entry is the most recent
            }
        }

        private void LoadSettings()
        {
            _mysettings = DASettings.Load();

            // No existing settings. Use default.
            if (_mysettings.Fake)
            {
                StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                // restore windows position
                StartPosition = FormStartPosition.Manual;
                Top = _mysettings.WinTop;
                Left = _mysettings.WinLeft;
                Height = _mysettings.WinHigh;
                Width = _mysettings.WinWide;
                _fileHistory = _mysettings.PathHistory ?? new List<string>();
                _fileHistory.Remove(null);
                mnuMRU.SetFiles(_fileHistory.ToArray());

                LastFile = _mysettings.LastPath;
            }
        }

        private void SaveSettings()
        {
            // TODO check minimized
            var bounds = DesktopBounds;
            _mysettings.WinTop = Location.Y;
            _mysettings.WinLeft = Location.X; 
            _mysettings.WinHigh = bounds.Height;
            _mysettings.WinWide = bounds.Width;
            _mysettings.Fake = false;
            _mysettings.LastPath = LastFile;
            _mysettings.PathHistory = mnuMRU.GetFiles().ToList();
            _mysettings.Save();
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void rad4Gen_CheckedChanged(object sender, EventArgs e)
        {
            drawer = rad4Gen.Checked ? draw4gen : draw5gen;
            cmbPerson_SelectedIndexChanged(null,null);
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawer.AncData = _ancIndi;
            EnhancedPrintPreviewDialog newPreview = new EnhancedPrintPreviewDialog();
            newPreview.Owner = this;
            newPreview.Document = drawer.PrintAncTree();
            newPreview.ShowDialog();
            drawer.AncData = null;
        }

        private void printSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void cmbPedigree_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO stupid GUI hack: sometimes the pedigree combo update causes a redraw, and sometimes it doesn't
            if (cmbPedigree.SelectedIndex < 0 || _noUpdate)
                return;
            int val = (int) cmbPedigree.SelectedValue;
            if (val < 0 || val >= _pedigrees.PedigreeCount)
                return;
            _ancIndi = _pedigrees.GetPedigree(val);
            DoAncTree();
        }
    }
}
