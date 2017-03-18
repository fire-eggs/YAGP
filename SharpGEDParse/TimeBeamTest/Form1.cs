using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DrawAnce; // TODO need to start collecting utility classes
using GEDWrap;
using SharpGEDParser.Model;
using TimeBeam;

// ReSharper disable LocalizableElement

namespace TimeBeamTest
{
    public partial class Form1 : Form
    {
        readonly List<object> _cmbItems = new List<object>();
        protected MruStripMenu mnuMRU;
        public event EventHandler LoadGed;

        public Form1()
        {
            InitializeComponent();

            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            personSel.DataSource = _cmbItems;

            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;

            LoadSettings(); // NOTE: must go after mnuMRU init

            LoadGed += Form1_LoadGed;

            timeline1.TrackBorderSize = 2; // TODO KBR
            timeline1.TrackLabelWidth = 200; // TODO should adjust automagically
            timeline1.TrackSpacing = 5; // TODO primary/secondary spacing
            timeline1.TrackHeight = 20;
            timeline1.DecadeLabelHigh = 16;
        }

        private void loadGEDCOMToolStripMenuItem_Click(object sender, System.EventArgs e)
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

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void ProcessGED(string gedPath)
        {
            personSel.SelectedIndex = -1;
            personSel.DataSource = null;
            personSel.Enabled = false;
            _cmbItems.Clear();

            timeline1.Visible = false;
            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed(this, new EventArgs());
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

        void Form1_LoadGed(object sender, EventArgs e)
        {
            Forest gedtrees = new Forest();
            // TODO Using LastFile is a hack... pass path in args? not as event?            
            gedtrees.LoadGEDCOM(LastFile);

            // TODO people should be distinguished by date range [as seen on tamurajones.net... where?]

            HashSet<string> comboNames = new HashSet<string>();
            foreach (var indiId in gedtrees.AllIndiIds)
            {
                Person p = gedtrees.PersonById(indiId);

                //// for each person, show the # of individuals available in (first) pedigree
                //Pedigrees pd = new Pedigrees(p, firstOnly: true);
                //p.Ahnen = pd.GetPedigreeMax(0);
                //var text = string.Format("{0} [{1}] ({2})", p.Name, indiId, p.Ahnen);

                var text = string.Format("{0} [{1}]", p.Name, indiId);
                comboNames.Add(text);
                _cmbItems.Add(new { Text = text, Value = p });
            }
            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            personSel.DataSource = _cmbItems;
            personSel.Enabled = true;
        }

        private const long JDN1800_01_01 = 2378497;
        private const long JDN2017_01_01 = 2457755;

        private class AdjustMyLength : ITimelineTrack
        {
            public float Start { get; set; }
            public float? End { get; set; }
            public string Name { get; set; }

            public override string ToString() // TODO not used?
            {
                return string.Format("Name: {0}, End: {1}, Start: {2}", Name, End, Start);
            }
        }

        public class MyTrack : Track
        {
        }

        private void MakeTrack(Person val, string mark)
        {
            var birt = val.Birth;
            var deat = val.Death;
            var name = val.Name;

            float yrs = JDN1800_01_01;
            float secs;
            float start = 60.0f;
            if (birt == null || birt.GedDate == null || birt.GedDate.Type==GEDDate.Types.Unknown)
            {
                ; // TODO need extrapolated
                GEDDate birt2 = val.ExtrapolateBirth();
                if (birt2 != null && birt2.Type != GEDDate.Types.Unknown)
                {
                    name = "*" + name;
                    yrs = birt2.JDN;
                }
            }
            else
            {
                yrs = birt.GedDate.JDN;
            }
            yrs = (yrs - JDN1800_01_01) / 365.0f;
            int yr1 = (int)yrs + 1800;
            secs = yrs * 6; // six seconds equals one year
            start = 60.0f + secs;

            if (deat == null || val.Indi.Living || deat.GedDate == null)
                yrs = JDN2017_01_01;
            else
                yrs = deat.GedDate.JDN;
            yrs = (yrs - JDN1800_01_01) / 365.0f;
            int yr2 = (int) yrs + 1800;
            secs = yrs * 6;
            float end = 60.0f + secs;

            int? showEnd = null;
            if (deat != null && !val.Indi.Living && deat.GedDate != null)
                showEnd = yr2;

            var outName = string.Format("{0}({1})[{2}-{3}]", name, mark, yr1, showEnd.HasValue ? yr2.ToString() : "");

            var aml = new AdjustMyLength();
            aml.Start = yr1;
            if (deat != null && !val.Indi.Living && deat.GedDate != null)
                aml.End = yr2;
            aml.Name = outName;

            timeline1.AddTrack(aml);

            Track person = new MyTrack();
            person.Start = yr1;
            if (deat != null && !val.Indi.Living && deat.GedDate != null)
                person.End = yr2;
            person.Name = outName;
            gedTime1.AddTrack(person);
        }

        private void personSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = personSel.SelectedValue as Person;
            if (val == null)
                return;

            timeline1.ClearTracks();
            gedTime1.ClearAllTracks();

            MakeTrack(val, "P");

            var marriages = val.SpouseIn;
            if (marriages.Count != 0)
            {
                foreach (var marriage in marriages)
                {
                    // determine who is the other spouse
                    var other = marriage.Husband == val ? marriage.Wife : marriage.Husband;
                    if (other != null)
                        MakeTrack(other, "S");

                    foreach (var child in marriage.Childs)
                    {
                        MakeTrack(child, "C");
                    }
                }
            }

            timeline1.Visible = true;
        }

        private void zoomIn_Click(object sender, EventArgs e)
        {
            var scale = timeline1.RenderingScale;
            scale.X += 0.1f;
            scale.Y += 0.1f;
            timeline1.RenderingScale = scale;
            gedTime1.RenderingScale = scale;
        }

        private void zoomOut_Click(object sender, EventArgs e)
        {
            var scale = timeline1.RenderingScale;
            scale.X -= 0.1f;
            scale.Y -= 0.1f;
            timeline1.RenderingScale = scale;
            gedTime1.RenderingScale = scale;
        }

    }
}
