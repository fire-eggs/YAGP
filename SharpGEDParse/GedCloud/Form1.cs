using GEDWrap;
using PrintPreview;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SharpGEDParser.Model;
using WordCloud;

namespace GedCloud
{
    public partial class Form1 : Form
    {
        protected MruStripMenu mnuMRU;

        public event EventHandler LoadGed;

        private List<Word> _surnames;
        private List<Word> _givennames;
        private List<Word> _locations;

        public Form1()
        {
            InitializeComponent();
            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;
            LoadGed += Form1_LoadGed;
            LoadSettings(); // must go after mnuMRU init
        }

#if LOGGING
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
#endif
        private static void incr(Dictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                int val = dict[key];
                val++;
                dict[key] = val;
            }
            else
            {
                dict.Add(key, 1);
            }
        }

        private void Form1_LoadGed(object sender, EventArgs e)
        {
            Forest gedtrees = new Forest();
            gedtrees.LoadGEDCOM(LastFile);

            Dictionary<string, int> surCount = new Dictionary<string, int>();
            Dictionary<string, int> givenCount = new Dictionary<string, int>();

            // 1. Gather surnames
            // 2. Gather given names
            foreach (var indi in gedtrees.AllPeople)
            {
                incr(surCount, indi.Surname.ToLower());
                incr(givenCount, indi.Given.ToLower());
            }

            _surnames = new List<Word>(surCount.Count);
            foreach (var name in surCount)
            {
                _surnames.Add(new Word(name));
            }
            _givennames = new List<Word>(givenCount.Count);
            foreach (var name in givenCount)
            {
                _givennames.Add(new Word(name));
            }

            // 3. Locations?
            ScanIt(gedtrees);
            Dictionary<string, int> locCount = new Dictionary<string, int>();
            foreach (var one in dataSet)
            {
                incr(locCount, one.Location.ToLower());
            }
            _locations = new List<Word>(locCount.Count);
            foreach (var i in locCount)
            {
                _locations.Add(new Word(i));
            }

            // 4. Update the cloud
            ChangeCloud();
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
            cloudControl1.WeightedWords = null;
            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed(this, new EventArgs());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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

        private void ChangeCloud()
        {
            if (rad4Gen.Checked)
                // surnames cloud
                cloudControl1.WeightedWords = _surnames.OrderByDescending(word => word.Occurrences);
            else if (rad5Gen.Checked)
                // given names cloud
                cloudControl1.WeightedWords = _givennames.OrderByDescending(word => word.Occurrences);
            else if (radCirc.Checked)
                // locations cloud
                cloudControl1.WeightedWords = _locations.OrderByDescending(word => word.Occurrences);
        }

        private void rad4Gen_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked || _surnames == null)
                return;
            ChangeCloud();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnhancedPrintPreviewDialog newPreview = new EnhancedPrintPreviewDialog();
            newPreview.Owner = this;
            newPreview.ShowDialog();
        }

        private void btnPal_Click(object sender, EventArgs e)
        {
#if PALETTE
            //SwatchPick dlg = new SwatchPick();
            //dlg.StartPosition = FormStartPosition.CenterParent;
            //dlg.SwatchIndex = _paletteIndex;
            //DialogResult res = dlg.ShowDialog(this);
            //if (res == DialogResult.Cancel)
            //    return;
            //_paletteIndex = dlg.SwatchIndex;
            //drawer.Palette = _paletteIndex;
            //DoAncTree(); // TODO the 'control' should redraw itself
#endif
        }

        private class One // TODO copy-pasta from LocationsList
        {
            public string Location;
            public string Tag;
            public string PersonId;
            public Person Indi; // TODO either keep reference to Forest data or keep instance of Forest
            public string FamId;
            public Union Fam; // TODO either keep reference to Forest data or keep instance of Forest
        }

        private List<One> dataSet;

        private void ScanIt(Forest f) // TODO copy-pasta from LocationsList
        {
            dataSet = new List<One>();

            foreach (var person in f.AllPeople)
            {
                IndiRecord ged = person.Indi;
                foreach (var familyEvent in ged.Events)
                {
                    string tag = familyEvent.Tag.ToString(); // TODO use GedTag?
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One { Location = familyEvent.Place, Tag = tag, PersonId = ged.Ident, Indi = person });
                    }
                }
                foreach (var familyEvent in ged.Attribs)
                {
                    string tag = familyEvent.Tag.ToString(); // TODO use GedTag?
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One { Location = familyEvent.Place, Tag = tag, PersonId = ged.Ident, Indi = person });
                    }
                }
            }

            foreach (var union in f.AllUnions)
            {
                FamRecord fam = union.FamRec;
                foreach (var familyEvent in fam.FamEvents)
                {
                    string tag = familyEvent.Tag.ToString(); // TODO use gedtag?
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One { Location = familyEvent.Place, Tag = tag, FamId = fam.Ident, Fam = union });
                    }
                }
            }
        }

    }
}
