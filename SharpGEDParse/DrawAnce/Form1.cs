using System.IO;
using System.Linq;
using BuildTree;
using SharpGEDParser;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            fr.ReadGed(LastFile); // this is a hack...
            BuildTree(fr.Data);

            ResetContext();

            // populate combobox with individuals
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
                _cmbItems.Add(new { Text = p.Name + "(" + count + ")", Value = p });
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
            _ancIndi = new IndiWrap[32];
            _hitRect = new Rectangle[32];
            for (int i = 0; i < 32; i++)
            {
                _ancIndi[i] = new IndiWrap();
                _hitRect[i] = new Rectangle();
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

            MakeAncTree();
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
                if (dadnum < 16)
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
                if (dadnum + 1 < 16)
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

        public class IndiWrap
        {
            public KBRGedIndi Indi;
            public int Ahnen;
            public FamilyUnit ChildOf;

            public string Name
            {
                get { return Indi == null ? "" : Indi.Names[0].ToString(); }
            }

            public string Text
            {
                get
                {
                    if (Indi == null)
                        return "";
                    string val = string.IsNullOrEmpty(Indi.Birth) ? "" : "B: " + Indi.Birth + "\r\n";
                    string val4 = string.IsNullOrEmpty(Indi.Christening) ? "" : "C: " + Indi.Christening + "\r\n";
                    string val3 = string.IsNullOrWhiteSpace(Marriage) ? "" : "M: " + Marriage + "\r\n";
                    string val2 = string.IsNullOrEmpty(Indi.Death) ? "" : "D: " + Indi.Death + "\r\n";
                    string val5 = string.IsNullOrEmpty(Indi.Occupation) ? "" : "O: " + Indi.Occupation + "\r\n";
                    return val + val4 + val3 + val2 + val5;
                }
            }

            public FamilyUnit SpouseIn { get; set; }

            // TODO divorce?

            public string Marriage
            {
                get
                {
                    if (SpouseIn == null)
                        return "";
                    var fam = SpouseIn.FamRec;
                    return fam.Marriage;
                }
            }
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

        #region Drawing Constants
        private const int MoreGenW = 25;

        private const int BOXH = 80;
        private const int BOXW = 250;
        private const int GEN3VM = 15; // vertical margin between boxes for Gen 3
        private const int OuterMargin = 5;
        private const int GEN2HM = 75;

        private const int TEXTSTEP = 6;
        private const int TEXTINDENT = 2;

        private const string MORE_GEN = "►";
        #endregion

        private Pen _boxPen;
        private Font _nameFont;
        private Font _textFont;
        private Brush _textBrush;
        private StringFormat _sf;
        private Rectangle[] _hitRect;

        private void MakeAncTree()
        {
            Point boxSz0, boxSz1, boxSz2, boxSz3;

            // 1. calc h/w. Assume 3 gen back. therefore, 8 boxes high plus margin. Width is calculated based on contents.
            int maxH = 8 * BOXH + 7 * GEN3VM + 2 * OuterMargin;
            int maxW; // = 2 * OuterMargin + GEN2HM + 2 * BOXW + (int)(BOXW * 1.5);

            _boxPen = new Pen(Color.Chocolate, 2.0f);
            Pen connPen = new Pen(Color.Black, 2.0f);
            _nameFont = new Font("Arial", 12);
            _textFont = new Font("Arial", 9);
            _textBrush = new SolidBrush(Color.Black);

            // A. Calculate the actual box widths. This requires creating a temp. bitmap
            // and graphics context to measure against.
            using (Bitmap tmpBmp = new Bitmap(500, 500))
            {
                using (Graphics gr = Graphics.FromImage(tmpBmp))
                {
                    boxSz0 = CalcBoxDims(gr, 1, 1);
                    boxSz1 = CalcBoxDims(gr, 2, 3);
                    boxSz2 = CalcBoxDims(gr, 4, 7);
                    boxSz3 = CalcBoxDims(gr, 8, 15);
                }
            }

            maxW = MoreGenW + 2 * OuterMargin + GEN2HM + boxSz3.X + boxSz2.X + 2 * boxSz1.X / 3 + 3 * boxSz0.X / 4;

            // 2. make bitmap to draw on.
            Bitmap bmp = new Bitmap(maxW, maxH);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.Cornsilk);

                _sf = new StringFormat();
                //                _sf.Trimming = StringTrimming.EllipsisCharacter;
                _sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;

                //Point boxSz = calcBoxDims(gr, 8, 15);

                // 3. draw gen 3 on right side
                int right = maxW - OuterMargin - MoreGenW;
                int left = right - boxSz3.X;
                int top = OuterMargin;
                Rectangle box3Rect = new Rectangle(left, top, boxSz3.X, BOXH);
                int vLineTop = 0;
                for (int i = 8; i <= 15; i++)
                {
                    DrawAnc(i, gr, box3Rect);
                    // partial connector, gen3 to gen2
                    gr.DrawLine(connPen, left, top + BOXH / 2, left - GEN2HM / 2, top + BOXH / 2);

                    // draw vertical connector line between pairs of boxes
                    if (vLineTop != 0)
                        gr.DrawLine(connPen, left - GEN2HM / 2, vLineTop - 1, left - GEN2HM / 2, top + BOXH / 2 + 1);
                    vLineTop = i % 2 == 0 ? top + BOXH / 2 : 0;

                    // Does this individual have ancestors? If so, draw a marker
                    if (_ancIndi[i] != null && _ancIndi[i].Ahnen > 1)
                        gr.DrawString(MORE_GEN, _nameFont, _textBrush, box3Rect.Right+2, box3Rect.Top + BOXH / 2 - 10); // TODO how to calc. location?

                    top += BOXH + GEN3VM;
                    box3Rect.Location = new Point(left, top);
                }

                // 4. draw gen 2. each box is centered vertically against
                // the two boxes to the right in gen 3. The boxes are stepped
                // left as a separate column to allow drawing connectors.
                int gen3step = 2 * BOXH + GEN3VM;
                right = left - GEN2HM;
                left = right - boxSz2.X;
                int gen3top = OuterMargin;
                Rectangle box2Rect = new Rectangle(left, top, boxSz2.X, BOXH);
                int gen2Togen1LineLeft = left - boxSz1.X/3;
                for (int i = 4; i <= 7; i++)
                {
                    top = gen3top + gen3step / 2 - BOXH / 2;
                    box2Rect.Location = new Point(left, top);

                    // complete the connector, gen3 to gen2
                    gr.DrawLine(connPen, right, top + BOXH / 2, right + GEN2HM / 2, top + BOXH / 2);

                    // partial connector, gen2 to gen1 [horz. line]
                    gr.DrawLine(connPen, left, top + BOXH / 2, gen2Togen1LineLeft, top + BOXH / 2);

                    DrawAnc(i, gr, box2Rect);
                    gen3top += gen3step + GEN3VM;
                }

                // TODO refactor drawing gen 1: duplicated code [at least one copy-pasta error has occurred]

                // 5. draw gen 1. The boxes are inset between the two boxes of
                // gen 2.
                right = left + boxSz1.X / 3;
                left = right - boxSz1.X;

                int top1 = OuterMargin + gen3step / 2 - BOXH / 2; // top of 1st gen2
                int high = BOXH + gen3step + GEN3VM;
                int cent1 = top1 + high / 2;
                top = cent1 - BOXH / 2;
                // TODO verify there is enough vertical room? at what time?
                Rectangle box1Rect = new Rectangle(left, top, boxSz1.X, BOXH);
                DrawAnc(2, gr, box1Rect);
                // complete connectors, gen2 to gen1 [vertical line]
                gr.DrawLine(connPen, gen2Togen1LineLeft, top1 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top1 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn2Y = top + BOXH / 2;
                int conn2X = left - boxSz1.X / 4;
                gr.DrawLine(connPen, left, conn2Y, conn2X, conn2Y);

                int top2 = top1 + high + gen3step + GEN3VM - BOXH;
                top = top2 + high / 2 - BOXH / 2;
                box1Rect.Location = new Point(left, top);
                DrawAnc(3, gr, box1Rect);
                // complete connectors, gen2 to gen1
                gr.DrawLine(connPen, gen2Togen1LineLeft, top2 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top2 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn3X = left - boxSz1.X / 4;
                int conn3Y = top + BOXH / 2;
                gr.DrawLine(connPen, left, conn3Y, conn3X, conn3Y);

                // 6. Draw gen 0.
                int top0 = maxH / 2 - BOXH / 2;
                int left0 = left - (int)(0.75 * boxSz0.X);
                Rectangle box0Rect = new Rectangle(left0, top0, boxSz0.X, BOXH);
                DrawAnc(1, gr, box0Rect);
                // finish connectors, gen1 to gen0
                gr.DrawLine(connPen, conn2X, top0 - 1, conn2X, conn2Y - 1);
                gr.DrawLine(connPen, conn3X, top0 + BOXH + 1, conn3X, conn3Y + 1);

            }

            picTree.Image = bmp;
        }

        /// <summary>
        /// Common code drawing an ancestor box. Draws the box, name, text.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="gr"></param>
        /// <param name="boxRect"></param>
        private void DrawAnc(int i, Graphics gr, Rectangle boxRect)
        {
            _hitRect[i] = boxRect;

            gr.DrawRectangle(_boxPen, boxRect);
            int left = boxRect.Left;
            int top = boxRect.Top;

            RectangleF textRect = boxRect;
            var nameLoc = new PointF(left, top + 3);
            textRect.Location = nameLoc;
            var nameSize = gr.MeasureString(_ancIndi[i].Name, _nameFont);
            gr.DrawString(_ancIndi[i].Name, _nameFont, _textBrush, nameLoc);
            var textLoc = new PointF(left + 2, top + 6 + nameSize.Height);
            textRect.Location = textLoc;
            gr.DrawString(_ancIndi[i].Text, _textFont, _textBrush, textLoc);
        }

        /// <summary>
        /// Calculate how big the boxes must be to fit the text of a generation.
        /// I.e. for a set of ancestors, determine how wide the name/text would
        /// draw and return the widest, highest.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="ancL"></param>
        /// <param name="ancH"></param>
        /// <returns></returns>
        private Point CalcBoxDims(Graphics gr, int ancL, int ancH)
        {
            int maxW = 0;
            int maxH = 0;
            for (int i = ancL; i <= ancH; i++)
            {
                var nameSize = gr.MeasureString(_ancIndi[i].Name, _nameFont);
                maxW = Math.Max(maxW, (int)nameSize.Width);
                var textSize = gr.MeasureString(_ancIndi[i].Text, _textFont);
                maxW = Math.Max(maxW, (int)textSize.Width + TEXTINDENT);
                maxW = Math.Max(maxW, 150); // prevent collapsed boxes
                int totH = (int)(nameSize.Height + textSize.Height + TEXTSTEP);
                maxH = Math.Max(maxH, totH);
            }
            return new Point(maxW, maxH);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Determine which rectangle a Point intersects.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns>The index within the rectangle array; -1 if no intersection.</returns>
        private int HitIndex(Point hit)
        {
            if (_hitRect == null)
                return -1;
            for (int i=0; i<_hitRect.Length; i++)
                if (_hitRect[i].Contains(hit))
                    return i;
            return -1;
        }

        private void picTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (HitIndex(e.Location) == -1)
                picTree.Cursor = Cursors.Arrow;
            else
                picTree.Cursor = Cursors.Hand;
        }

        private void picTree_MouseClick(object sender, MouseEventArgs e)
        {
            int index = HitIndex(e.Location);
            if (index == -1 || _ancIndi[index] == null)
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
