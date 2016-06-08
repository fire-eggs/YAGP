﻿using BuildTree;
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

        public Form1()
        {
            InitializeComponent();
            cmbPerson.DisplayMember = "Text";
            cmbPerson.ValueMember = "Value";
            cmbPerson.DataSource = _cmbItems;
            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            LoadGed += Form1_LoadGed;
            LoadSettings(); // must go after mnuMRU init
        }

        private void Form1_LoadGed(object sender, EventArgs e)
        {
            var fr = new FileRead();
            fr.ReadGed(LastFile); // TODO Using LastFile is a hack... pass path in args? not as event?
            BuildTree(fr.Data);

            ResetContext();

            // populate combobox with individuals
            // www.ahnenbuch.de-AMMON has multiple individuals with the same name. Need to distinguish
            // them somehow for the combobox.
            // TODO is there a better way? unique thing for combobox selection?
            HashSet<string> comboNames = new HashSet<string>();
            foreach (var indiId in _indiHash.Keys)
            {
                IndiWrap p = _indiHash[indiId];
                int count = 1;
                if (_childHash.ContainsKey(p.Indi.Ident))
                {
                    FamilyUnit firstFam = _childHash[p.Indi.Ident];
                    p.ChildOf = firstFam;
                    count = CalcAnce(firstFam, 1);
                }
                p.Ahnen = count;
                var text = p.Name + "(" + count + ")";
                var test = text;
                int suffix = 1;
                while (comboNames.Contains(test))
                {
                    test = text + "[" + suffix + "]";
                    suffix++;
                }
                comboNames.Add(test);
                _cmbItems.Add(new { Text = test, Value = p });
            }
            cmbPerson.DisplayMember = "Text";
            cmbPerson.DataSource = _cmbItems;
            cmbPerson.Enabled = true;
        }

        private void OnMRU(int number, string filename)
        {
            if (!File.Exists(filename))
            {
                mnuMRU.RemoveFile(number);
                MessageBox.Show("The file no longer exists:" + filename);
                return;
            }

            // TODO process could fail for some reason, in which case remove the file from the MRU list
            LastFile = filename;
            mnuMRU.SetFirstFile(number);
            ProcessGED(filename);
        }

        private void ResetContext()
        {
            _ancIndi = new IndiWrap[MAX_AHNEN];
            for (int i = 0; i < MAX_AHNEN; i++)
            {
                _ancIndi[i] = new IndiWrap();
            }
        }

        private void TreePerson(IndiWrap val)
        {
            ResetContext();
            _ancIndi[1] = val;
            if (_childHash.ContainsKey(val.Indi.Ident)) // Is there any ancestry?
            {
                FamilyUnit firstFam = _childHash[val.Indi.Ident];
                CalcAnce(firstFam, 1);
            }

            DoAncTree();
        }

        private IDrawGen d4;
        private int MAX_AHNEN = 16;

        private void DoAncTree()
        {
            if (d4 == null)
                d4 = new Draw4Gen();
            d4.AncData = _ancIndi;
            picTree.Image = d4.MakeAncTree();
        }

        private void cmbPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var item = cmbPerson.SelectedItem;
            var val = cmbPerson.SelectedValue as IndiWrap;
            if (val == null)
                return;
            TreePerson(val);
        }

        /// <summary>
        /// Determine a person's ancestry. Does two things:
        /// 1. Populates the _ancIndi array for use when drawing the tree. Only fills in those
        ///    entries supported by the tree limit.
        /// 2. Drills down the entire ancestry, returning the largest Ahnen number found.
        /// </summary>
        /// <param name="firstFam">The root person's family unit</param>
        /// <param name="myNum">The root person's Ahnen number</param>
        /// <returns>The largest Ahnen number encountered</returns>
        private int CalcAnce(FamilyUnit firstFam, int myNum)
        {
            int numRet = myNum;

            // From http://www.tamurajones.net/AhnenNumbering.xhtml : the Ahnen number 
            // of the father is double that of the current person. Mom's Ahnen number
            // is Dad's plus 1.

            int dadnum = myNum * 2;
            if (firstFam.Husband != null)
            {
                numRet = Math.Max(numRet, dadnum);
                if (dadnum < MAX_AHNEN)
                {
                    IndiWrap hack = _indiHash[firstFam.Husband.Ident];
                    _ancIndi[dadnum] = hack;
                }
                if (firstFam.DadFam != null)
                    numRet = Math.Max(numRet,CalcAnce(firstFam.DadFam, dadnum));
            }
            if (firstFam.Wife != null)
            {
                numRet = Math.Max(numRet, dadnum+1);
                if (dadnum + 1 < MAX_AHNEN)
                {
                    IndiWrap hack = _indiHash[firstFam.Wife.Ident];
                    _ancIndi[dadnum+1] = hack;
                }
                if (firstFam.MomFam != null)
                    numRet = Math.Max(numRet, CalcAnce(firstFam.MomFam, dadnum+1));
            }
            return numRet;
        }

        private void openGEDCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "GEDCOM files (*.ged)|*.ged";
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

        readonly List<object> _cmbItems = new List<object>();
        Dictionary<string, IndiWrap> _indiHash ;
        Dictionary<string, FamilyUnit> _childHash ;
        private IndiWrap[] _ancIndi;

        public event EventHandler LoadGed;

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


        /// <summary>
        /// The GEDCOM file has been parsed. This method pieces the tree together, creating FamilyUnit
        /// objects to contain Father/Mother/Children.
        /// </summary>
        /// <param name="gedRecs"></param>
        private void BuildTree(List<KBRGedRec> gedRecs)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            _indiHash = new Dictionary<string, IndiWrap>();
            var famHash = new Dictionary<string, KBRGedFam>();
            string first = null;
            foreach (var kbrGedRec in gedRecs)
            {
                if (kbrGedRec is KBRGedIndi)
                {
                    IndiWrap iw = new IndiWrap();
                    iw.Indi = kbrGedRec as KBRGedIndi;
                    iw.Ahnen = 0;
                    iw.ChildOf = null;
                    _indiHash.Add(kbrGedRec.Ident, iw);

                    if (first == null)
                        first = kbrGedRec.Ident;
                }
                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (kbrGedRec is KBRGedFam && !famHash.ContainsKey(kbrGedRec.Ident))
                    famHash.Add(kbrGedRec.Ident, kbrGedRec as KBRGedFam);
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
                        KBRGedIndi hack = new KBRGedIndi(null,kbrGedFam.Dad);
                        NameRec hack2 = new NameRec();
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

            //// For each person, dump their ancestry
            //foreach (var indiId in indiHash.Keys)
            //{
            //    KBRGedIndi firstP = indiHash[indiId];
            //    Console.WriteLine("First person:" + firstP.Names[0]);
            //    if (childHash.ContainsKey(indiId))
            //    {
            //        FamilyUnit firstFam = childHash[indiId];
            //        DumpAnce(firstFam, childHash, firstP, 1);
            //    }
            //    else
            //    {
            //        Console.WriteLine(" No ancestry");
            //    }
            //    Console.WriteLine("==========================================================");
            //}
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
                int index = d4.HitIndex(e.Location);

                picTree.Cursor = !HavePerson(index) ?
                    Cursors.Arrow :
                    Cursors.Hand;
            }
        }

        private void picTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawer == null)
                return;
            int index = d4.HitIndex(e.Location);
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
    }
}