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
        private const int NODE_HEIGHT = 30;
        private const int NODE_WIDTH = 40;
        private const int NODE_MARGIN_X = 50;
        private const int NODE_MARGIN_Y = 40;

        private static Pen NODE_PEN = Pens.Gray;
        private static Pen DUPL_PEN = Pens.LawnGreen;
        
        private List<SampleDataModel> _data;
        private TreeNodeModel<SampleDataModel> _tree;

        readonly List<object> _cmbItems = new List<object>();
        protected MruStripMenu mnuMRU;

        private DataSet _data2;
        private TreeNodeModel<UnionData> _tree2;

        public Form1()
        {
            InitializeComponent();

            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            personSel.DataSource = _cmbItems;

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

        #region Tree Setup Helpers

        //private List<SampleDataModel> GetSampleData2()
        //{
        //    var sampleTree = new List<SampleDataModel>();

        //    sampleTree.Add(new SampleDataModel {Id = "TO", ParentId = string.Empty});
        //    sampleTree.Add(new SampleDataModel {Id = "JW", ParentId = "TO"});
        //    sampleTree.Add(new SampleDataModel {Id = "BK", ParentId = "JW"});
        //    sampleTree.Add(new SampleDataModel {Id = "WH", ParentId = "BK"});
        //    sampleTree.Add(new SampleDataModel {Id = "SE", ParentId = "BK"});
        //    sampleTree.Add(new SampleDataModel {Id = "QI", ParentId = "BK"});
        //    sampleTree.Add(new SampleDataModel {Id = "KX", ParentId = "BK"});
        //    sampleTree.Add(new SampleDataModel {Id = "KA", ParentId = "KX"});
        //    sampleTree.Add(new SampleDataModel {Id = "HH", ParentId = "JW"});
        //    sampleTree.Add(new SampleDataModel {Id = "DN", ParentId = "HH"});
        //    sampleTree.Add(new SampleDataModel {Id = "KT", ParentId = "HH"});
        //    sampleTree.Add(new SampleDataModel {Id = "JB", ParentId = "KT"});
        //    sampleTree.Add(new SampleDataModel {Id = "UM", ParentId = "KT"});
        //    sampleTree.Add(new SampleDataModel {Id = "AL", ParentId = "KT"});
        //    sampleTree.Add(new SampleDataModel {Id = "FR", ParentId = "KT"});
        //    sampleTree.Add(new SampleDataModel {Id = "WE", ParentId = "HH"});
        //    sampleTree.Add(new SampleDataModel {Id = "CO", ParentId = "WE"});
        //    sampleTree.Add(new SampleDataModel {Id = "LE", ParentId = "WE"});
        //    sampleTree.Add(new SampleDataModel {Id = "LO", ParentId = "WE"});
        //    sampleTree.Add(new SampleDataModel {Id = "YI", ParentId = "HH"});
        //    sampleTree.Add(new SampleDataModel {Id = "EI", ParentId = "YI"});
        //    sampleTree.Add(new SampleDataModel {Id = "DJ", ParentId = "YI"});
        //    sampleTree.Add(new SampleDataModel {Id = "SH", ParentId = "YI"});
        //    sampleTree.Add(new SampleDataModel {Id = "BS", ParentId = "JW"});
        //    sampleTree.Add(new SampleDataModel {Id = "SP", ParentId = "BS"});
        //    sampleTree.Add(new SampleDataModel {Id = "SB", ParentId = "JW"});
        //    sampleTree.Add(new SampleDataModel {Id = "GQ", ParentId = "SB"});
        //    sampleTree.Add(new SampleDataModel {Id = "JS", ParentId = "GQ"});
        //    sampleTree.Add(new SampleDataModel {Id = "HT", ParentId = "SB"});
        //    sampleTree.Add(new SampleDataModel {Id = "MB", ParentId = "HT"});
        //    sampleTree.Add(new SampleDataModel {Id = "MF", ParentId = "HT"});
        //    sampleTree.Add(new SampleDataModel {Id = "FW", ParentId = "SB"});
        //    sampleTree.Add(new SampleDataModel {Id = "GM", ParentId = "FW"});
        //    sampleTree.Add(new SampleDataModel {Id = "XT", ParentId = "FW"});
        //    sampleTree.Add(new SampleDataModel {Id = "VQ", ParentId = "FW"});         
        //    return sampleTree;
        //}

        // returns a list of sample data items
        //private List<SampleDataModel> GetSampleData()
        //{
        //    var sampleTree = new List<SampleDataModel>();

        //    // Root Node
        //    sampleTree.Add(new SampleDataModel {Id = "O", ParentId = string.Empty, Name = "Test GP O"});
        //    sampleTree.Add(new SampleDataModel { Id = "Z", ParentId = string.Empty});

        //    // 1st Level
        //    sampleTree.Add(new SampleDataModel {Id = "E", ParentId = "O", Name = "Test Node E"});
        //    sampleTree.Add(new SampleDataModel {Id = "F", ParentId = "O", Name = "Test Node F"});
        //    sampleTree.Add(new SampleDataModel {Id = "N", ParentId = "O", Name = "Test Node N"});

        //    // 2nd Level
        //    sampleTree.Add(new SampleDataModel {Id = "A", ParentId = "E", Name = "Test Node A"});
        //    sampleTree.Add(new SampleDataModel {Id = "D", ParentId = "E", Name = "Test Node D"});

        //    sampleTree.Add(new SampleDataModel {Id = "G", ParentId = "N", Name = "Test Node G"});
        //    sampleTree.Add(new SampleDataModel {Id = "M", ParentId = "N", Name = "Test Node M"});
        //    sampleTree.Add(new SampleDataModel {Id = "P", ParentId = "N", Name = "KBR P"});

        //    // 3rd Level
        //    sampleTree.Add(new SampleDataModel {Id = "B", ParentId = "D", Name = "Test Node B"});
        //    sampleTree.Add(new SampleDataModel {Id = "C", ParentId = "D", Name = "Test Node C"});

        //    sampleTree.Add(new SampleDataModel {Id = "H", ParentId = "M", Name = "Test Node H"});
        //    sampleTree.Add(new SampleDataModel {Id = "I", ParentId = "M", Name = "Test Node I"});
        //    sampleTree.Add(new SampleDataModel {Id = "J", ParentId = "M", Name = "Test Node J"});
        //    sampleTree.Add(new SampleDataModel {Id = "K", ParentId = "M", Name = "Test Node K"});
        //    sampleTree.Add(new SampleDataModel {Id = "L", ParentId = "M", Name = "Test Node L"});

        //    sampleTree.Add(new SampleDataModel {Id="Q",ParentId="G" });
        //    sampleTree.Add(new SampleDataModel { Id = "R", ParentId = "G" });

        //    // 4th level
        //    sampleTree.Add(new SampleDataModel { Id = "A1", ParentId = "H" });
        //    sampleTree.Add(new SampleDataModel { Id = "A2", ParentId = "H" });
        //    sampleTree.Add(new SampleDataModel { Id = "A3", ParentId = "H" });

        //    // 5th level
        //    sampleTree.Add(new SampleDataModel { Id = "B1", ParentId = "A1"});
        //    sampleTree.Add(new SampleDataModel { Id = "B2", ParentId = "A1" });
        //    sampleTree.Add(new SampleDataModel { Id = "B3", ParentId = "A1" });
        //    sampleTree.Add(new SampleDataModel { Id = "B4", ParentId = "A1" });
        //    sampleTree.Add(new SampleDataModel { Id = "B5", ParentId = "A1" });
        //    sampleTree.Add(new SampleDataModel { Id = "B6", ParentId = "A1" });
        //    sampleTree.Add(new SampleDataModel { Id = "B7", ParentId = "A1" });

        //    sampleTree.Add(new SampleDataModel { Id = "B8", ParentId = "A2" });
        //    sampleTree.Add(new SampleDataModel { Id = "B9", ParentId = "A2" });

        //    sampleTree.Add(new SampleDataModel { Id = "B10", ParentId = "A3" });
        //    sampleTree.Add(new SampleDataModel { Id = "B11", ParentId = "A3" });
        //    sampleTree.Add(new SampleDataModel { Id = "B12", ParentId = "A3" });
        //    sampleTree.Add(new SampleDataModel { Id = "B13", ParentId = "A3" });

        //    return sampleTree;
        //}

        // converts list of sample items to hierarchial list of TreeNodeModels

        private Dictionary<string, TreeNodeModel<SampleDataModel>> _idsToNodes;

        private TreeNodeModel<SampleDataModel> GetSampleTree(List<SampleDataModel> data)
        {
            var root = data.FirstOrDefault(p => p.ParentId == string.Empty);
            var rootTreeNode = new TreeNodeModel<SampleDataModel>(root, null);

            _idsToNodes = new Dictionary<string, TreeNodeModel<SampleDataModel>>();
            _idsToNodes.Add(root.Id, rootTreeNode);

            // add tree node children recursively
            rootTreeNode.Children = GetChildNodes(data, rootTreeNode);

            return rootTreeNode;
        }

        private List<TreeNodeModel<SampleDataModel>> GetChildNodes(List<SampleDataModel> data, TreeNodeModel<SampleDataModel> parent)
        {
            var nodes = new List<TreeNodeModel<SampleDataModel>>();

            foreach (var item in data.Where(p => p.ParentId == parent.Item.Id))
            {
                var treeNode = new TreeNodeModel<SampleDataModel>(item, parent);

                if (_idsToNodes.ContainsKey(item.Id))
                {
                    var tnm = _idsToNodes[item.Id];
                    tnm.Item.IsDup = true;

                    item.IsDup = true;
                }
                else
                    _idsToNodes.Add(item.Id, treeNode);

                treeNode.Children = GetChildNodes(data, treeNode);
                nodes.Add(treeNode);
            }

            return nodes;
        }

        #endregion

        #region Custom Painting

        private void treePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.Clear(Color.AntiqueWhite);

            //if (_tree != null)
            //    DrawNode(_tree, e.Graphics);
            if (_tree2 != null)
                DrawNode2(_tree2, e.Graphics);
        }
        
//        private void CalculateControlSize()
//        {
//            // tree sizes are 0-based, so add 1
//            var treeWidth = _tree.Width + 1;
//            var treeHeight = _tree.Height + 1;

//            Size calcSize = new Size(
//                Convert.ToInt32((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X)),
//                (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y));
////            treePanel.Size = calcSize;
//            treePanel.ClientSize = calcSize;
//        }

        private void CalculateControlSize2()
        {
            // tree sizes are 0-based, so add 1
            var treeWidth = _tree2.Width + 1;
            var treeHeight = _tree2.Height + 1;

            Size calcSize = new Size(
                Convert.ToInt32((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X)),
                (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y));
            //            treePanel.Size = calcSize;
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
                SizeF txtSz = g.MeasureString(txt, Font, 1000, StringFormat.GenericTypographic);
                float txtX = nodeRect.X + nodeRect.Width / 2.0f - txtSz.Width / 2.0f;
                float txtY = nodeRect.Y + nodeRect.Height / 2.0f - txtSz.Height / 2.0f;
                g.DrawString(node.ToString(), Font, Brushes.Black, txtX, txtY, StringFormat.GenericTypographic);

                if (node.Item.CurrentMarriage != -1)
                    g.DrawString("M", Font, Brushes.Black, nodeRect.Right + 1, nodeRect.Top + 1, StringFormat.GenericTypographic);

                if (node.Item.CurrentParents != -1)
                    g.DrawString("A", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz.Height - 1, StringFormat.GenericTypographic);
                else if (node.Item.Parents.Count > 0)
                    g.DrawString("P", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz.Height - 1, StringFormat.GenericTypographic);

            }

            // draw line to parent
            if (node.Parent != null)
            {
                var nodeTopMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y);
                g.DrawLine(NODE_PEN, nodeTopMiddle, new Point(nodeTopMiddle.X, nodeTopMiddle.Y - (NODE_MARGIN_Y / 2)));
            }

            // draw line to children
            if (node.Children.Count > 0)
            {
                var nodeBottomMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y + nodeRect.Height);
                g.DrawLine(NODE_PEN, nodeBottomMiddle, new Point(nodeBottomMiddle.X, nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2)));


                // draw line over children
                if (node.Children.Count > 1)
                {
                    var childrenLineStart = new Point(
                        Convert.ToInt32(NODE_MARGIN_X + (node.GetRightMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
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

            int midY = Math.Max(thisRect.Bottom, destRect.Bottom) + 10;

            Point p1 = new Point(midXthis, thisRect.Bottom);
            Point p2 = new Point(midmidX, midY);
            Point p3 = new Point(midXDest, destRect.Bottom);

            g.DrawCurve(DUPL_PEN, new Point[]{p1,p2,p3});
        }

        //private void DrawNode(TreeNodeModel<SampleDataModel> node, Graphics g)
        //{
        //    // rectangle where node will be positioned
        //    var nodeRect = this.nodeRect(node);

        //    // draw box
        //    g.DrawRectangle(node.Item.IsDup ? DUPL_PEN : NODE_PEN, nodeRect);

        //    // draw content
        //    {
        //        string txt = node.ToString();
        //        SizeF txtSz = g.MeasureString(txt, Font, 1000, StringFormat.GenericTypographic);
        //        float txtX = nodeRect.X + nodeRect.Width/2.0f - txtSz.Width/2.0f;
        //        float txtY = nodeRect.Y + nodeRect.Height / 2.0f - txtSz.Height / 2.0f;
        //        g.DrawString(node.ToString(), Font, Brushes.Black, txtX, txtY, StringFormat.GenericTypographic);

        //        if (node.Item.CurrentMarriage != -1)
        //            g.DrawString("M", Font, Brushes.Black, nodeRect.Right + 1, nodeRect.Top+1, StringFormat.GenericTypographic);
        //        if (node.Item.Parents.Count > 0)
        //            g.DrawString("P", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz.Height - 1, StringFormat.GenericTypographic);
        //        if (node.Item.CurrentParents != -1)
        //            g.DrawString("A", Font, Brushes.Black, nodeRect.Left + 1, nodeRect.Top - txtSz.Height - 1, StringFormat.GenericTypographic);

        //    }

        //    // draw line to parent
        //    if (node.Parent != null)
        //    {
        //        var nodeTopMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y);
        //        g.DrawLine(NODE_PEN, nodeTopMiddle, new Point(nodeTopMiddle.X, nodeTopMiddle.Y - (NODE_MARGIN_Y / 2)));
        //    }

        //    // draw line to children
        //    if (node.Children.Count > 0)
        //    {
        //        var nodeBottomMiddle = new Point(nodeRect.X + (nodeRect.Width / 2), nodeRect.Y + nodeRect.Height);
        //        g.DrawLine(NODE_PEN, nodeBottomMiddle, new Point(nodeBottomMiddle.X, nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2)));


        //        // draw line over children
        //        if (node.Children.Count > 1)
        //        {
        //            var childrenLineStart = new Point(
        //                Convert.ToInt32(NODE_MARGIN_X + (node.GetRightMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
        //                nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2));
        //            var childrenLineEnd = new Point(
        //                Convert.ToInt32(NODE_MARGIN_X + (node.GetLeftMostChild().X * (NODE_WIDTH + NODE_MARGIN_X)) + (NODE_WIDTH / 2)),
        //                nodeBottomMiddle.Y + (NODE_MARGIN_Y / 2));

        //            g.DrawLine(NODE_PEN, childrenLineStart, childrenLineEnd);
        //        }
        //    }


        //    foreach (var item in node.Children)
        //    {
        //        DrawNode(item, g);
        //    }
        //}

        #endregion

        private void TreePerson(Person val)
        {
            _data2 = new DataSet(val);
            _data2.GetDescendants();

            //_data = GetDescendants(val);
            RebuildTree();
        }

        private void RebuildTree()
        {
            toolTip1.SetToolTip(treePanel, "");
            //_tree = GetSampleTree(_data);
            _tree2 = _data2.GetTree();

            //TreeHelpers<SampleDataModel>.CalculateNodePositions(_tree);
            TreeHelpers<UnionData>.CalculateNodePositions(_tree2);

            //CalculateControlSize();
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

        private void GetDescendants(List<SampleDataModel> tree, Person root, string parentId)
        {
            // TODO intermediate nodes may have been toggled to show a different marriage

            SampleDataModel node = new SampleDataModel {Id = root.Id, ParentId = parentId};
            tree.Add(node);

            // TODO navigation to parent(s) for spouse
            // TODO add spouse as expanded node
            
            //Debug.Assert(!(root.ChildIn.Count > 1 && parentId != "")); // alternate set of parents

            // Navigation to parents - for root only
            if (parentId == "" && root.ChildIn.Count > 0)
            {
                // TODO a non-root node might have an alternate parent!
                foreach (var union1 in root.ChildIn)
                {
                    node.Parents.Add(union1.DadId);
                    node.Parents.Add(union1.MomId);
                }
            }

            if (root.ChildIn.Count > 1 && parentId != "")
            {
                // Child node has multiple parents possible
                node.CurrentParents = 0;
                // TODO actually implement this!
            }

            // children of first marriage are added
            if (root.SpouseIn.Count < 1)
                return;

            if (root.SpouseIn.Count > 1)
                node.CurrentMarriage = 0; // Alternate marriages possible

            var union = root.SpouseIn.First();
            foreach (var achild in union.Childs)
            {
                GetDescendants(tree, achild, root.Id);
            }

            // TODO need a 'get spouse' method in Union!
            // spousein.count > 0 but may not have a spouse!
            if (union.DadId == node.Id)
            {
                node.HasSpouse = !string.IsNullOrEmpty(union.MomId);
                node.SpouseId = union.MomId;
            }
            if (union.MomId == node.Id)
            {
                node.HasSpouse = !string.IsNullOrEmpty(union.DadId);
                node.SpouseId = union.DadId;
            }
        }

        private List<SampleDataModel> GetDescendants(Person person)
        {
            List<SampleDataModel> tree = new List<SampleDataModel>();
            GetDescendants(tree, person, "");
            return tree;
        }

        private void ProcessGED(string gedPath)
        {
            personSel.SelectedIndex = -1;
            personSel.DataSource = null;
            personSel.Enabled = false;
            _cmbItems.Clear();

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
            personSel.SelectedIndex = -1;
            personSel.DataSource = _cmbItems;
            personSel.Enabled = true;
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
                Union u = p.ChildIn.First();
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

        //private void DeleteChildrenOf(string id)
        //{
        //    List<string> toDelete = new List<string>();
        //    foreach (var sampleDataModel in _data)
        //    {
        //        if (sampleDataModel.ParentId == id)
        //            toDelete.Add(sampleDataModel.Id);
        //    }
        //    foreach (var todel in toDelete)
        //    {
        //        DeleteChildrenOf(todel);
        //    }
        //    _data.RemoveAll(x => x.ParentId == id);
        //}

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

        //private void RebuildChildren(SampleDataModel node, Person person)
        //{
        //    // TODO rebuild spouse

        //    // Marriage has been changed. 
        //    // 1. Existing children AND descendants need to be removed.
        //    // 2. Children from selected marriage need to be added.
        //    DeleteChildrenOf(person.Id);

        //    int index = 0;
        //    foreach (var union1 in person.SpouseIn)
        //    {
        //        if (index == node.CurrentMarriage)
        //        {
        //            foreach (var achild in union1.Childs)
        //            {
        //                GetDescendants(_data, achild, node.Id);
        //            }
        //        }
        //        index++;
        //    }
        //}

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
            if (node.Item.IsUnion)
            {
                w += NODE_WIDTH;
                x -= NODE_WIDTH / 2;
            }
            var rect = new Rectangle(x,
                NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y))
                , w, NODE_HEIGHT);
            return rect;
        }

        private Rectangle nodeRect(TreeNodeModel<SampleDataModel> node)
        {
            // TODO This changes only on paint - store in data?
            int x = (int)(NODE_MARGIN_X + (node.X*(NODE_WIDTH + NODE_MARGIN_X)));
            int w = NODE_WIDTH;
            if (node.Item.HasSpouse)
            {
                w += NODE_WIDTH;
                x -= NODE_WIDTH/2;
            }

            var rect = new Rectangle( x,
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
            if (thisroot.Parent == null)
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

        //private bool treeIntersect(int x, int y, ref string who, ref HitType what, TreeNodeModel<SampleDataModel> tree )
        //{
        //    who = tree.Item.Id;
        //    what = HitType.None;

        //    var rect = nodeRect(tree);
        //    if (rect.Contains(x, y))
        //    {
        //        what = HitType.Person;
        //        return true;
        //    }

        //    // Marriage toggle
        //    if (tree.Item.CurrentMarriage != -1)
        //    {
        //        // TODO calc and store during draw when we have actual sizes
        //        var marrRect = new Rectangle(rect.Right + 1, rect.Top + 1, 10, 19);
        //        if (marrRect.Contains(x, y))
        //        {
        //            what = HitType.Marriage;
        //            return true;
        //        }
        //    }

        //    // Parent(s)
        //    if (tree.Item.Parents.Count > 0)
        //    {
        //        // TODO calc and store during draw when we have actual sizes
        //        var parRect = new Rectangle(rect.Left + 1, rect.Top - 20, 10, 19);
        //        if (parRect.Contains(x, y))
        //        {
        //            what = HitType.Parent;
        //            return true;
        //        }
        //    }

        //    foreach (var child in tree.Children)
        //    {
        //        if (treeIntersect(x, y, ref who, ref what, child))
        //            return true;
        //    }
        //    return false;
        //}

        //private bool treeIntersect(int x, int y, out string who, out HitType what)
        //{
        //    who = "";
        //    what = HitType.Person;
        //    return treeIntersect(x, y, ref who, ref what, _tree);
        //}
    }
}
