using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DrawAnce;  // TODO common helpers into utility DLL
using GEDWrap;


// Ancestry tree diagram
// Using code from Rachel Lim, and fixes from the blog comments.
// https://rachel53461.wordpress.com/2014/04/20/algorithm-for-drawing-trees/
//

// TODO scale factor
// TODO text boxes sized by text
// TODO marriages as double boxes, showing spouse
// TODO tree into a control
// TODO drag to pan?

// TODO a person might appear more than once (see I80 in kev.ged). Impact if has multiple marriages?

namespace DrawTreeTest
{
    public partial class Form1 : Form
    {
        private const int NODE_HEIGHT = 40;
        private const int NODE_WIDTH = 120;
        private const int NODE_MARGIN_X = 40;
        private const int NODE_MARGIN_Y = 50;

        private static Pen NODE_PEN = Pens.Gray;
        private static Pen DUPL_PEN = new Pen(Color.CornflowerBlue) {DashStyle = DashStyle.Dash};
        private static Pen MMARG_PEN = new Pen(Color.Coral) { DashStyle = DashStyle.Dash };
        
        readonly List<object> _cmbItems = new List<object>();
        protected MruStripMenu mnuMRU;

        private DataSet _data2;
        private TreeNodeModel<UnionData> _tree2;

        public Form1()
        {
            InitializeComponent();

            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            try { personSel.DataSource = _cmbItems; } catch { }

            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;

            LoadSettings(); // NOTE: must go after mnuMRU init

            DoubleBuffered = true;
            treePanel.Paint += treePanel_Paint;
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

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        #region Custom Painting

        private float _zoom = 1.0f;

        private void treePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Clear(Color.AntiqueWhite);

            g.ScaleTransform(_zoom, _zoom);

            if (_tree2 != null)
                DrawNode2(_tree2, e.Graphics);
        }
        
        private void CalculateControlSize2()
        {
            // tree sizes are 0-based, so add 1
            var treeWidth = _tree2.Width + 1;
            var treeHeight = _tree2.Height + 1;

            Size calcSize0 = new Size(
                Convert.ToInt32((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X)),
                (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y));

            Size calcSize = new Size((int)(calcSize0.Width*_zoom), (int)(calcSize0.Height*_zoom));

            treePanel.ClientSize = calcSize;
        }

        private void DrawNode2(TreeNodeModel<UnionData> node, Graphics g)
        {
            // rectangle where node will be positioned
            var nodeRect = this.nodeRect(node);

            // draw box
            g.DrawRectangle(node.Item.Link != -1 ? DUPL_PEN : NODE_PEN, nodeRect);

            // draw content
            {
                string txt = node.ToString();
#if NOTTEST
                SizeF txtSz = g.MeasureString(txt, Font, NODE_WIDTH, StringFormat.GenericTypographic);
#else
                SizeF txtSz = g.MeasureString(txt, Font, 1000, StringFormat.GenericTypographic); // multi-line
                SizeF txtSz2 = g.MeasureString("W", Font, 5, StringFormat.GenericTypographic);  // single-line
#endif
                float txtX = nodeRect.X + nodeRect.Width / 2.0f - txtSz.Width / 2.0f;
                float txtY = nodeRect.Y + nodeRect.Height / 2.0f - txtSz.Height / 2.0f;
                g.DrawString(node.ToString(), Font, Brushes.Black, txtX, txtY, StringFormat.GenericTypographic);

                if (node.Item.CurrentMarriage != -1)
                    g.DrawString("M", Font, Brushes.Black, nodeRect.Right + 1, nodeRect.Top + 1, StringFormat.GenericTypographic);

                if (node.Item.CurrentParents != -1)
                    g.DrawString("A", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz2.Height - 1, StringFormat.GenericTypographic);
                else if (node.Item.Parents.Count > 0)
                    g.DrawString("P", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz2.Height - 1, StringFormat.GenericTypographic);

            }

            // draw line to parent
            if (node.Parent != null && node.Item.DrawParentLink)
            {
                var nodeTopMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y);
                g.DrawLine(NODE_PEN, nodeTopMiddle, new Point(nodeTopMiddle.X, nodeTopMiddle.Y - (NODE_MARGIN_Y / 2)));
            }

            // draw line to other marriage
            if (!node.Item.DrawParentLink)
            {
                var sib = node.GetPreviousSibling();
                var sibRect = this.nodeRect(sib);
                var myLeftMiddle = new Point(nodeRect.X, nodeRect.Y + (nodeRect.Height/2));
                var sibRightMiddle = new Point(sibRect.X + sibRect.Width, sibRect.Y + (sibRect.Height/2));
                g.DrawLine(MMARG_PEN, myLeftMiddle, sibRightMiddle); // TODO 3rd marriage etc not have distinct line
            }

            // draw line to children
            if (node.Children.Count > 0)
            {
                var nodeBottomMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y + nodeRect.Height);
                g.DrawLine(NODE_PEN, nodeBottomMiddle, new Point(nodeBottomMiddle.X, nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2)));


                // draw line over children
                if (node.Children.Count > 1)
                {
                    var rmc = node.GetRightMostChild(); // take multi-marriage into account
                    while (!rmc.Item.DrawParentLink)
                        rmc = rmc.GetPreviousSibling();

                    var childrenLineStart = new Point(
                        Convert.ToInt32(NODE_MARGIN_X + (rmc.X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
                        nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2));
                    var childrenLineEnd = new Point(
                        Convert.ToInt32(NODE_MARGIN_X + (node.GetLeftMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
                        nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2));

                    g.DrawLine(NODE_PEN, childrenLineStart, childrenLineEnd);
                }
            }

            DrawDupLink(node, g);

            foreach (var item in node.Children)
            {
                DrawNode2(item, g);
            }

        }

        private TreeNodeModel<UnionData> FindNodeById(TreeNodeModel<UnionData> thisroot, int id)
        {
            if (thisroot.Item.Id == id)
                return thisroot;
            foreach (var child in thisroot.Children)
            {
                var res = FindNodeById(child, id);
                if (res != null)
                    return res;
            }
            return null;
        }

        private void DrawDupLink(TreeNodeModel<UnionData> node, Graphics g)
        {
            if (node.Item.Link == -1)
                return;

            TreeNodeModel<UnionData> destNode = FindNodeById(_tree2, node.Item.Link);
            Debug.Assert(destNode != null);

            var thisRect = this.nodeRect(node);
            var destRect = this.nodeRect(destNode);

            int midXDest = destRect.Left + (destRect.Right - destRect.Left)/2;
            int midXthis = thisRect.Left + (thisRect.Right - thisRect.Left) / 2;
            int midmidX = midXDest + (midXthis - midXDest)/2;

            int midY = Math.Max(thisRect.Bottom, destRect.Bottom) + (NODE_MARGIN_Y/2)-5; // TODO tweak/make constant

            Point p1 = new Point(midXthis, thisRect.Bottom);
            Point p2 = new Point(midmidX, midY);
            Point p3 = new Point(midXDest, destRect.Bottom);

            g.DrawCurve(DUPL_PEN, new Point[]{p1,p2,p3});
        }

        #endregion

        private void TreePerson(Person val)
        {
            _data2 = new DataSet(val);
            _data2.GetDescendants();
            RebuildTree();
        }

        private void RebuildTree()
        {
            toolTip1.SetToolTip(treePanel, "");
            _tree2 = _data2.GetTree();
            TreeHelpers<UnionData>.CalculateNodePositions(_tree2);
            CalculateControlSize2();
            treePanel.Invalidate();
        }

        private void personSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = personSel.SelectedValue as Person;
            if (val == null)
                return;
            TreePerson(val);
        }

        private void ProcessGED(string gedPath)
        {
            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed();
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

        private Forest gedtrees;

        void LoadGed()
        {
            gedtrees = new Forest();
            // TODO Using LastFile is a hack... pass path in args? not as event?            
            gedtrees.LoadGEDCOM(LastFile);
//            DateEstimator.Estimate(gedtrees);

            // TODO people should be distinguished by date range [as seen on tamurajones.net... where?]

            personSel.SelectedIndexChanged -= personSel_SelectedIndexChanged;
            personSel.Enabled = false;
            personSel.BeginUpdate();
            personSel.DataSource = null;
            _cmbItems.Clear();

            HashSet<string> comboNames = new HashSet<string>();
            Dictionary<string, Person> comboPersons = new Dictionary<string, Person>();
            foreach (var indiId in gedtrees.AllIndiIds)
            {
                Person p = gedtrees.PersonById(indiId);

                var text = string.Format("{0},{1} [{2}]", p.Surname, p.Given, indiId);
                comboNames.Add(text);
                comboPersons.Add(text, p);
            }

            var nameSort = comboNames.ToArray();
            Array.Sort(nameSort);
            foreach (var s in nameSort)
            {
                _cmbItems.Add(new {Text=s,Value=comboPersons[s]});
            }
            
            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            personSel.DataSource = _cmbItems;
            personSel.EndUpdate();
            personSel.Enabled = true;
            personSel.SelectedIndexChanged += personSel_SelectedIndexChanged;
            personSel.SelectedIndex = 0;
        }

        private enum HitType { Person, Parent, Marriage, Adoptive, None };

        private void treePanel_Click(object sender, EventArgs eventArgs)
        {
            MouseEventArgs e = eventArgs as MouseEventArgs;

            TreeNodeModel<UnionData> node = null;
            HitType what = TreeIntersect(e.X, e.Y, ref node, _tree2);
            if (what == HitType.None)
                return;

            Person p = gedtrees.PersonById(node.Item.PersonId);
            switch (what)
            {
            case HitType.Person:
                personSel.SelectedIndex = -1;
                TreePerson(p);
                break;
            case HitType.Parent:
                {
                Union u = p.ChildIn.FirstOrDefault();
                Debug.Assert(p.ChildIn.Count < 2); // TODO adoptive parents for root
                personSel.SelectedIndex = -1;
                TreePerson(u.Husband ?? u.Wife);
                }
                break;
            case HitType.Adoptive:
                {
                int parentDex = node.Item.CurrentParents + 1;
                if (parentDex >= p.ChildIn.Count)
                    parentDex = 0;
                // TODO why is ChildIn a HashSet?
                Union u = p.ChildIn.ToArray()[parentDex];
                personSel.SelectedIndex = -1;
                TreePerson(u.Husband ?? u.Wife);
                // TODO update CurrentParents value in new node!!
                }
                break;
            case HitType.Marriage:
                RebuildChildren(node.Item, p);
                RebuildTree();
                break;
            }
        }

        private void RebuildChildren(UnionData node, Person p)
        {
            // Marriage has been changed. 
            // 1. Existing node, children AND descendants need to be removed.
            // 2. Sub-tree with selected marriage needs to be added.

            int newUnionNum = node.CurrentMarriage + 1;
            if (newUnionNum >= p.SpouseIn.Count)
                newUnionNum = 0;

            UnionData newItem = node.ShallowCopy();
            newItem.CurrentMarriage = newUnionNum;

            Union u = p.SpouseIn.ToArray()[newUnionNum];
            newItem.UnionId = u.Id;

            _data2.Replace(newItem, u);
        }

        private Point oldLocation = Point.Empty;

        private void treePanel_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Location == oldLocation || _tree2 == null) // TODO tooltip causing infinite mouse move events?
                return;
            oldLocation = e.Location;

            TreeNodeModel<UnionData> node = null;
            HitType what = TreeIntersect(e.X, e.Y, ref node, _tree2);
            string ttxt = "";
            treePanel.Cursor = Cursors.Hand;
            switch (what)
            {
                case HitType.Person:
                    Person p = gedtrees.PersonById(node.Item.PersonId);
                    string bdate = p.Birth == null ? "" : "\n" + p.Birth.Date;
                    ttxt = String.Format("{0}{1}", p.Name, bdate);
                    break;
                case HitType.Marriage:
                    ttxt = "Another marriage";
                    break;
                case HitType.Parent:
                    ttxt = "View parent";
                    break;
                case HitType.Adoptive:
                    ttxt = "Another parent";
                    break;
                case HitType.None:
                    treePanel.Cursor = Cursors.Arrow;
                    break;
            }
            toolTip1.Show(ttxt, treePanel, e.X + 15, e.Y + 15); // TODO use cursor size for offset
        }

        private Rectangle nodeRect(TreeNodeModel<UnionData> node)
        {
            // TODO This changes only on paint - store in data
            int x = (int)(NODE_MARGIN_X + (node.X * (NODE_WIDTH + NODE_MARGIN_X)));
            int w = NODE_WIDTH;
            //if (node.Item.IsUnion)
            //{
            //    w += NODE_WIDTH;
            //    x -= (int)(NODE_WIDTH * 0.5);
            //}
            var rect = new Rectangle(x,
                NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y))
                , w, NODE_HEIGHT);
            return rect;
        }

        private HitType TreeIntersect(int x, int y, ref TreeNodeModel<UnionData> node, TreeNodeModel<UnionData> thisroot)
        {
            var rect = nodeRect(thisroot);
            if (rect.Contains(x, y))
            {
                node = thisroot;
                return HitType.Person;
            }

            // alternate marriage
            if (thisroot.Item.CurrentMarriage != -1)
            {
                // TODO calc and store during draw when we have actual sizes
                var marrRect = new Rectangle(rect.Right + 1, rect.Top + 1, 10, 19);
                if (marrRect.Contains(x, y))
                {
                    node = thisroot;
                    return HitType.Marriage;
                }
            }

            // Root, parents
            if (thisroot.Parent == null && thisroot.Item.CurrentParents != -1)
            {
                // TODO root adoptive parents
                // TODO calc and store during draw with actual sizes
                var parRect = new Rectangle(rect.Left + 1, rect.Top - 20, 10, 19);
                if (parRect.Contains(x, y))
                {
                    node = thisroot;
                    return HitType.Parent;
                }
            }

            // Alternate parents
            if (thisroot.Item.Parents.Count > 0)
            {
                // TODO calc and store during draw when we have actual sizes
                var parRect = new Rectangle(rect.Left + 1, rect.Top - 20, 10, 19);
                if (parRect.Contains(x, y))
                {
                    node = thisroot;
                    return HitType.Adoptive;
                }
            }

            // No hit on this node, try its children
            foreach (var child in thisroot.Children)
            {
                var res = TreeIntersect(x, y, ref node, child);
                if (res != HitType.None)
                    return res;
            }
            return HitType.None;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
//            Console.WriteLine("{0}|{1}", e.KeyCode, e.Control);
            if (!e.Control)
                return;
            if (e.KeyCode == Keys.Oemplus)
            {
                btnZoomIn_Click(null,null);
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                btnZoomOut_Click(null,null);
            }
        }

        private void btn100Percent_Click(object sender, EventArgs e)
        {
            _zoom = 1.0f;
            CalculateControlSize2();
            treePanel.Invalidate();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            _zoom += 0.1f;
            CalculateControlSize2();
            treePanel.Invalidate();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            _zoom -= 0.1f;
            CalculateControlSize2();
            treePanel.Invalidate();
        }
    }
}
