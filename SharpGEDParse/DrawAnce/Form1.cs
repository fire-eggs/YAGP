using System.Diagnostics;
using BuildTree;
using PrintPreview;
using SharpGEDParser;
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
        private const int MAX_AHNEN = 32;

        readonly List<object> _cmbItems = new List<object>();
        readonly List<object> _cmbPedItems = new List<object>();

        private Pedigrees _pedigrees;
        private IndiWrap[] _ancIndi;

        private readonly FamilyTreeBuild _treeBuild;

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

            _treeBuild = new FamilyTreeBuild();

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
            var fr = new FileRead();
            fr.ReadGed(LastFile); // TODO Using LastFile is a hack... pass path in args? not as event?
            //logit("LoadGed 2");
            _treeBuild.BuildTree(fr.Data.ToList(), false, false);
            //logit("LoadGed 3");

            // populate combobox with individuals
            // www.ahnenbuch.de-AMMON has multiple individuals with the same name. Need to distinguish
            // them somehow for the combobox.
            // TODO is there a better way? unique thing for combobox selection?
            HashSet<string> comboNames = new HashSet<string>();
            foreach (var indiId in _treeBuild.IndiIds)
            {
                IndiWrap p = _treeBuild.IndiFromId(indiId);

                // for each person, show the # of individuals available in (first) pedigree
                Pedigrees pd = new Pedigrees(p, firstOnly:true);
                p.Ahnen = pd.GetPedigreeMax(0);

                var text = string.Format("{0} [{1}] ({2})", p.Name, indiId, p.Ahnen);
                comboNames.Add(text);
                _cmbItems.Add(new { Text=text, Value=p } );
            }
            cmbPerson.DisplayMember = "Text";
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
                cmbPedigree.DisplayMember = "Text";
                cmbPedigree.ValueMember = "Value";
                cmbPedigree.DataSource = _cmbPedItems;
                cmbPedigree.SelectedIndex = 0;
                cmbPedigree.Enabled = true;
            }
            _noUpdate = false;
        }

        private void TreePerson(IndiWrap val)
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
            var val = cmbPerson.SelectedValue as IndiWrap;
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

#if false
        /// <summary>
        /// The GEDCOM file has been parsed. This method pieces the tree together, creating FamilyUnit
        /// objects to contain Father/Mother/Children.
        /// </summary>
        /// <param name="gedRecs"></param>
        private void BuildTree(IEnumerable<GEDCommon> gedRecs)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            _indiHash = new Dictionary<string, IndiWrap>();
            var famHash = new Dictionary<string, FamRecord>();
            string first = null;
            foreach (var kbrGedRec in gedRecs)
            {
                if (kbrGedRec is IndiRecord)
                {
                    var ident = (kbrGedRec as IndiRecord).Ident;

                    IndiWrap iw = new IndiWrap();
                    iw.Indi = kbrGedRec as IndiRecord;
                    iw.Ahnen = 0;
                    iw.ChildOf = null;
                    _indiHash.Add(ident, iw);

                    if (first == null)
                        first = ident;
                }
                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (kbrGedRec is FamRecord)
                {
                    var ident = (kbrGedRec as FamRecord).Ident;
                    if (!famHash.ContainsKey(ident))
                        famHash.Add(ident, kbrGedRec as FamRecord);
                }
            }

            // hash: child ids -> familyunit
            _childHash = new Dictionary<string, FamilyUnit>();

            // Accumulate family units
            // TODO indi with no fam
            // TODO indi with fams/famc only : see GEDCOM_Amssoms
            foreach (var kbrGedFam in famHash.Values)
            {
                var famU = new FamilyUnit(kbrGedFam);
                if (kbrGedFam.Dad != null)
                {
                    // TODO GEDCOM_Amssoms has a family with reference to non-existant individual. Needs to be caught by validate and 'fixed' there.
                    if (_indiHash.ContainsKey(kbrGedFam.Dad))
                    {
                        var iw = _indiHash[kbrGedFam.Dad];
                        famU.Husband = iw.Indi;
                        iw.SpouseIn = famU;
                    }
                    else
                    {
                        IndiWrap hack0 = new IndiWrap();

                        // TODO need a library method to do this!!!
                        IndiRecord hack = new IndiRecord(null,kbrGedFam.Dad,null);
                        var hack2 = new SharpGEDParser.Model.NameRec();
                        hack2.Surname = "Missing";
                        hack.Names.Add(hack2);
                        famU.Husband = hack;
                        hack0.Indi = hack;
                        hack0.Ahnen = -1;
                        hack0.SpouseIn = famU;
                        _indiHash.Add(kbrGedFam.Dad, hack0);
                    }
                }
                if (kbrGedFam.Mom != null)
                {
                    var iw = _indiHash[kbrGedFam.Mom];
                    famU.Wife = iw.Indi; // TODO handle mom as non-existant individual.
                    iw.SpouseIn = famU;
                }
                foreach (var child in kbrGedFam.Childs)
                {
                    famU.Childs.Add(_indiHash[child].Indi);

                    // TODO punting on adoption where a child could be part of more than one family see allged.ged
                    if (!_childHash.ContainsKey(child))
                        _childHash.Add(child, famU);
                }
                families.Add(famU);
            }

            // Connect family units
            foreach (var familyUnit in families)
            {
                if (_childHash.ContainsKey(familyUnit.DadId))
                    familyUnit.DadFam = _childHash[familyUnit.DadId];
                if (_childHash.ContainsKey(familyUnit.MomId))
                    familyUnit.MomFam = _childHash[familyUnit.MomId];
            }

            famHash = null;
            families = null;
        }
#endif

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
