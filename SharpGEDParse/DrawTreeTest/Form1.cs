using System;
using System.Collections.Generic;
using System.Drawing;
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
// TODO marriages as larger boxes
// TODO tree into a control
// TODO multi-marriage toggle
// TODO navigation: go to parent [M / F separate]
// TODO drag to pan?

namespace DrawTreeTest
{
    public partial class Form1 : Form
    {
        private const int NODE_HEIGHT = 30;
        private const int NODE_WIDTH = 40;
        private const int NODE_MARGIN_X = 50;
        private const int NODE_MARGIN_Y = 40;

        private static Pen NODE_PEN = Pens.Gray;
        
        private List<SampleDataModel> _data;
        private TreeNodeModel<SampleDataModel> _tree;

        readonly List<object> _cmbItems = new List<object>();
        protected MruStripMenu mnuMRU;

        public Form1()
        {
            InitializeComponent();

            personSel.DisplayMember = "Text";
            personSel.ValueMember = "Value";
            personSel.DataSource = _cmbItems;

            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;

            LoadSettings(); // NOTE: must go after mnuMRU init

            _data = GetSampleData();
            _tree = GetSampleTree(_data);
            TreeHelpers<SampleDataModel>.CalculateNodePositions(_tree);

            CalculateControlSize();

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

        private List<SampleDataModel> GetSampleData2()
        {
            var sampleTree = new List<SampleDataModel>();

            sampleTree.Add(new SampleDataModel {Id = "TO", ParentId = string.Empty});
            sampleTree.Add(new SampleDataModel {Id = "JW", ParentId = "TO"});
            sampleTree.Add(new SampleDataModel {Id = "BK", ParentId = "JW"});
            sampleTree.Add(new SampleDataModel {Id = "WH", ParentId = "BK"});
            sampleTree.Add(new SampleDataModel {Id = "SE", ParentId = "BK"});
            sampleTree.Add(new SampleDataModel {Id = "QI", ParentId = "BK"});
            sampleTree.Add(new SampleDataModel {Id = "KX", ParentId = "BK"});
            sampleTree.Add(new SampleDataModel {Id = "KA", ParentId = "KX"});
            sampleTree.Add(new SampleDataModel {Id = "HH", ParentId = "JW"});
            sampleTree.Add(new SampleDataModel {Id = "DN", ParentId = "HH"});
            sampleTree.Add(new SampleDataModel {Id = "KT", ParentId = "HH"});
            sampleTree.Add(new SampleDataModel {Id = "JB", ParentId = "KT"});
            sampleTree.Add(new SampleDataModel {Id = "UM", ParentId = "KT"});
            sampleTree.Add(new SampleDataModel {Id = "AL", ParentId = "KT"});
            sampleTree.Add(new SampleDataModel {Id = "FR", ParentId = "KT"});
            sampleTree.Add(new SampleDataModel {Id = "WE", ParentId = "HH"});
            sampleTree.Add(new SampleDataModel {Id = "CO", ParentId = "WE"});
            sampleTree.Add(new SampleDataModel {Id = "LE", ParentId = "WE"});
            sampleTree.Add(new SampleDataModel {Id = "LO", ParentId = "WE"});
            sampleTree.Add(new SampleDataModel {Id = "YI", ParentId = "HH"});
            sampleTree.Add(new SampleDataModel {Id = "EI", ParentId = "YI"});
            sampleTree.Add(new SampleDataModel {Id = "DJ", ParentId = "YI"});
            sampleTree.Add(new SampleDataModel {Id = "SH", ParentId = "YI"});
            sampleTree.Add(new SampleDataModel {Id = "BS", ParentId = "JW"});
            sampleTree.Add(new SampleDataModel {Id = "SP", ParentId = "BS"});
            sampleTree.Add(new SampleDataModel {Id = "SB", ParentId = "JW"});
            sampleTree.Add(new SampleDataModel {Id = "GQ", ParentId = "SB"});
            sampleTree.Add(new SampleDataModel {Id = "JS", ParentId = "GQ"});
            sampleTree.Add(new SampleDataModel {Id = "HT", ParentId = "SB"});
            sampleTree.Add(new SampleDataModel {Id = "MB", ParentId = "HT"});
            sampleTree.Add(new SampleDataModel {Id = "MF", ParentId = "HT"});
            sampleTree.Add(new SampleDataModel {Id = "FW", ParentId = "SB"});
            sampleTree.Add(new SampleDataModel {Id = "GM", ParentId = "FW"});
            sampleTree.Add(new SampleDataModel {Id = "XT", ParentId = "FW"});
            sampleTree.Add(new SampleDataModel {Id = "VQ", ParentId = "FW"});         
            return sampleTree;
        }

        // returns a list of sample data items
        private List<SampleDataModel> GetSampleData()
        {
            var sampleTree = new List<SampleDataModel>();

            // Root Node
            sampleTree.Add(new SampleDataModel {Id = "O", ParentId = string.Empty, Name = "Test GP O"});
            sampleTree.Add(new SampleDataModel { Id = "Z", ParentId = string.Empty});

            // 1st Level
            sampleTree.Add(new SampleDataModel {Id = "E", ParentId = "O", Name = "Test Node E"});
            sampleTree.Add(new SampleDataModel {Id = "F", ParentId = "O", Name = "Test Node F"});
            sampleTree.Add(new SampleDataModel {Id = "N", ParentId = "O", Name = "Test Node N"});

            // 2nd Level
            sampleTree.Add(new SampleDataModel {Id = "A", ParentId = "E", Name = "Test Node A"});
            sampleTree.Add(new SampleDataModel {Id = "D", ParentId = "E", Name = "Test Node D"});

            sampleTree.Add(new SampleDataModel {Id = "G", ParentId = "N", Name = "Test Node G"});
            sampleTree.Add(new SampleDataModel {Id = "M", ParentId = "N", Name = "Test Node M"});
            sampleTree.Add(new SampleDataModel {Id = "P", ParentId = "N", Name = "KBR P"});

            // 3rd Level
            sampleTree.Add(new SampleDataModel {Id = "B", ParentId = "D", Name = "Test Node B"});
            sampleTree.Add(new SampleDataModel {Id = "C", ParentId = "D", Name = "Test Node C"});

            sampleTree.Add(new SampleDataModel {Id = "H", ParentId = "M", Name = "Test Node H"});
            sampleTree.Add(new SampleDataModel {Id = "I", ParentId = "M", Name = "Test Node I"});
            sampleTree.Add(new SampleDataModel {Id = "J", ParentId = "M", Name = "Test Node J"});
            sampleTree.Add(new SampleDataModel {Id = "K", ParentId = "M", Name = "Test Node K"});
            sampleTree.Add(new SampleDataModel {Id = "L", ParentId = "M", Name = "Test Node L"});

            sampleTree.Add(new SampleDataModel {Id="Q",ParentId="G" });
            sampleTree.Add(new SampleDataModel { Id = "R", ParentId = "G" });

            // 4th level
            sampleTree.Add(new SampleDataModel { Id = "A1", ParentId = "H" });
            sampleTree.Add(new SampleDataModel { Id = "A2", ParentId = "H" });
            sampleTree.Add(new SampleDataModel { Id = "A3", ParentId = "H" });

            // 5th level
            sampleTree.Add(new SampleDataModel { Id = "B1", ParentId = "A1"});
            sampleTree.Add(new SampleDataModel { Id = "B2", ParentId = "A1" });
            sampleTree.Add(new SampleDataModel { Id = "B3", ParentId = "A1" });
            sampleTree.Add(new SampleDataModel { Id = "B4", ParentId = "A1" });
            sampleTree.Add(new SampleDataModel { Id = "B5", ParentId = "A1" });
            sampleTree.Add(new SampleDataModel { Id = "B6", ParentId = "A1" });
            sampleTree.Add(new SampleDataModel { Id = "B7", ParentId = "A1" });

            sampleTree.Add(new SampleDataModel { Id = "B8", ParentId = "A2" });
            sampleTree.Add(new SampleDataModel { Id = "B9", ParentId = "A2" });

            sampleTree.Add(new SampleDataModel { Id = "B10", ParentId = "A3" });
            sampleTree.Add(new SampleDataModel { Id = "B11", ParentId = "A3" });
            sampleTree.Add(new SampleDataModel { Id = "B12", ParentId = "A3" });
            sampleTree.Add(new SampleDataModel { Id = "B13", ParentId = "A3" });

            return sampleTree;
        }

        // converts list of sample items to hierarchial list of TreeNodeModels
        private TreeNodeModel<SampleDataModel> GetSampleTree(List<SampleDataModel> data)
        {
            var root = data.FirstOrDefault(p => p.ParentId == string.Empty);
            var rootTreeNode = new TreeNodeModel<SampleDataModel>(root, null);

            // add tree node children recursively
            rootTreeNode.Children = GetChildNodes(data, rootTreeNode);

            return rootTreeNode;
        }

        private static List<TreeNodeModel<SampleDataModel>> GetChildNodes(List<SampleDataModel> data, TreeNodeModel<SampleDataModel> parent)
        {
            var nodes = new List<TreeNodeModel<SampleDataModel>>();

            foreach (var item in data.Where(p => p.ParentId == parent.Item.Id))
            {
                var treeNode = new TreeNodeModel<SampleDataModel>(item, parent);
                treeNode.Children = GetChildNodes(data, treeNode);
                nodes.Add(treeNode);
            }

            return nodes;
        }

        #endregion

        #region Custom Painting

        private void treePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.AntiqueWhite);
            DrawNode(_tree, e.Graphics);
        }
        
        private void CalculateControlSize()
        {
            // tree sizes are 0-based, so add 1
            var treeWidth = _tree.Width + 1;
            var treeHeight = _tree.Height + 1;

            Size calcSize = new Size(
                Convert.ToInt32((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X)),
                (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y));
//            treePanel.Size = calcSize;
            treePanel.ClientSize = calcSize;
        }

        private void DrawNode(TreeNodeModel<SampleDataModel> node, Graphics g)
        {
            // rectangle where node will be positioned
            var nodeRect = new Rectangle(
                Convert.ToInt32(NODE_MARGIN_X + (node.X * (NODE_WIDTH + NODE_MARGIN_X))),
                NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y))
                , NODE_WIDTH, NODE_HEIGHT);

            // draw box
            g.DrawRectangle(NODE_PEN, nodeRect);

            // draw content
            g.DrawString(node.ToString(), Font, Brushes.Black, nodeRect.X + 10, nodeRect.Y + 10);

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


            foreach (var item in node.Children)
            {
                DrawNode(item, g);
            }
        }

        #endregion

        private void personSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = personSel.SelectedValue as Person;
            if (val == null)
                return;
            _data = GetAncestors(val);
            _tree = GetSampleTree(_data);
            TreeHelpers<SampleDataModel>.CalculateNodePositions(_tree);
            CalculateControlSize();
            treePanel.Invalidate();
        }

        private void GetAncestors(List<SampleDataModel> tree, Person root, string parentId)
        {
            SampleDataModel node = new SampleDataModel() {Id = root.Id, ParentId = parentId};
            tree.Add(node);

            // TODO add spouse as expanded node
            // TODO if root has more than one marriage, provide a toggle

            // children of first marriage the root is spouse in are added
            if (root.SpouseIn.Count < 1)
                return;

            if (root.SpouseIn.Count > 1)
                node.Name = root.Id + "(*)";

            var union = root.SpouseIn.First();
            foreach (var achild in union.Childs)
            {
                GetAncestors(tree, achild, root.Id);
            }
        }

        private List<SampleDataModel> GetAncestors(Person person)
        {
            List<SampleDataModel> tree = new List<SampleDataModel>();
            GetAncestors(tree, person, "");
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
            personSel.DataSource = _cmbItems;
            personSel.Enabled = true;
        }

    }
}
