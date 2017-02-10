using System.Diagnostics;
using System.IO;
using System.Text;
using GEDWrap;
using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LocationsList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoItState();
        }

        private void LoadGED_Click(object sender, EventArgs e)
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
            label1.Text = ofd.FileName;

            LoadIt(ofd.FileName); // TODO background load
        }

        private void LoadIt(string path)
        {
            // Load a GED file.
            using (Forest f = new Forest())
            {
                f.LoadGEDCOM(path);
                ScanIt(f);
            }
            // Scan for events w/ locations
            // Update events tree
            BuildTagTree();
            // Enable controls
            DoItState();
        }

        private class One
        {
            public string Location;
            public string Tag;
            public string PersonId;
            public Person Indi; // TODO either keep reference to Forest data or keep instance of Forest
            public string FamId;
            public Union Fam; // TODO either keep reference to Forest data or keep instance of Forest
        }

        private List<One> dataSet;

        private void ScanIt(Forest f)
        {
            dataSet = new List<One>();

            foreach (var person in f.AllPeople)
            {
                IndiRecord ged = person.Indi;
                foreach (var familyEvent in ged.Events)
                {
                    string tag = familyEvent.Tag;
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One {Location = familyEvent.Place, Tag = tag, PersonId = ged.Ident, Indi=person});
                    }
                }
                foreach (var familyEvent in ged.Attribs)
                {
                    string tag = familyEvent.Tag;
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One { Location = familyEvent.Place, Tag = tag, PersonId = ged.Ident, Indi = person});
                    }
                }
            }

            foreach (var union in f.AllUnions)
            {
                FamRecord fam = union.FamRec;
                foreach (var familyEvent in fam.FamEvents)
                {
                    string tag = familyEvent.Tag;
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                    {
                        dataSet.Add(new One { Location = familyEvent.Place, Tag = tag, FamId = fam.Ident, Fam=union });
                    }
                }
            }
        }

        #region Event Tree Management

        private class treeHier
        {
            public int Group;
            public string Tag;
            public string Label;
        }

        private List<treeHier> displayHier = new List<treeHier>
        {
            new treeHier {Group=1, Tag=null, Label = "INDI Events"},
            new treeHier {Group=1, Tag="BIRT", Label = "Birth"},
            new treeHier {Group=1, Tag="CHR", Label = "Christening"},
            new treeHier {Group=1, Tag="DEAT", Label = "Death"},
            new treeHier {Group=1, Tag="BURI", Label = ""},
            new treeHier {Group=1, Tag="CREM", Label = ""},
            new treeHier {Group=1, Tag="ADOP", Label = ""},
            new treeHier {Group=1, Tag="BAPM", Label = ""},
            new treeHier {Group=1, Tag="BARM", Label = ""},
            new treeHier {Group=1, Tag="BASM", Label = ""},
            new treeHier {Group=1, Tag="BLES", Label = ""},
            new treeHier {Group=1, Tag="CHRA", Label = ""},
            new treeHier {Group=1, Tag="CONF", Label = ""},
            new treeHier {Group=1, Tag="FCOM", Label = ""},
            new treeHier {Group=1, Tag="ORDN", Label = ""},
            new treeHier {Group=1, Tag="NATU", Label = ""},
            new treeHier {Group=1, Tag="EMIG", Label = ""},
            new treeHier {Group=1, Tag="IMMI", Label = ""},
            new treeHier {Group=1, Tag="CENS", Label = ""},
            new treeHier {Group=1, Tag="PROB", Label = ""},
            new treeHier {Group=1, Tag="WILL", Label = ""},
            new treeHier {Group=1, Tag="GRAD", Label = ""},
            new treeHier {Group=1, Tag="RETI", Label = ""},
            new treeHier {Group=1, Tag="EVEN", Label = ""}, // conflict
            new treeHier {Group=2, Tag=null, Label = "INDI Attributes"},
            new treeHier {Group=2, Tag="CAST", Label = "Caste"},
            new treeHier {Group=2, Tag="DSCR", Label = "Description"},
            new treeHier {Group=2, Tag="EDUC", Label = ""},
            new treeHier {Group=2, Tag="IDNO", Label = ""},
            new treeHier {Group=2, Tag="NATI", Label = ""},
            new treeHier {Group=2, Tag="NCHI", Label = ""},
            new treeHier {Group=2, Tag="NMR", Label = ""},
            new treeHier {Group=2, Tag="OCCU", Label = ""},
            new treeHier {Group=2, Tag="PROP", Label = ""},
            new treeHier {Group=2, Tag="RELI", Label = ""},
            new treeHier {Group=2, Tag="RESI", Label = ""}, // conflict
            new treeHier {Group=2, Tag="SSN", Label = ""},
            new treeHier {Group=2, Tag="TITL", Label = ""},
            new treeHier {Group=2, Tag="FACT", Label = ""},
            new treeHier {Group=3, Tag=null, Label = "FAM Events"},
            new treeHier {Group=3, Tag="MARR", Label = ""},
            new treeHier {Group=3, Tag="ANUL", Label = ""},
            new treeHier {Group=3, Tag="CENS", Label = ""},
            new treeHier {Group=3, Tag="DIV", Label = ""},
            new treeHier {Group=3, Tag="DIVF", Label = ""},
            new treeHier {Group=3, Tag="ENGA", Label = ""},
            new treeHier {Group=3, Tag="MARB", Label = ""},
            new treeHier {Group=3, Tag="MARC", Label = ""},
            new treeHier {Group=3, Tag="MARL", Label = ""},
            new treeHier {Group=3, Tag="MARS", Label = ""},
            new treeHier {Group=3, Tag="RESI", Label = ""}, // conflict
            new treeHier {Group=3, Tag="EVEN", Label = ""}, // conflict
        };

        private void BuildTagTree()
        {
            var uniqueTags = (from one in dataSet select one.Tag).Distinct(); //.OrderBy(name => name);
            var roots = (from hier in displayHier where hier.Tag == null select hier);
            var ents = (from hier in displayHier where uniqueTags.Contains(hier.Tag) select hier).ToArray();

            List<TreeNode> nodes = new List<TreeNode>();
            foreach (var treeHier in roots)
            {
                treeHier hier1 = treeHier;
                TreeNode node = new TreeNode(hier1.Label) { Tag = hier1, Checked = true };
                var childs = (from hier in ents where hier.Tag != null && hier.Group == hier1.Group select hier);
                foreach (var child in childs)
                {
                    TreeNode cNode = new TreeNode(child.Label == "" ? child.Tag : child.Label) {Tag = child, Checked = true};
                    node.Nodes.Add(cNode);
                }
                nodes.Add(node);
            }

            EventsPick.SuspendLayout();
            EventsPick.Nodes.Clear();
            EventsPick.Nodes.AddRange(nodes.ToArray());
            EventsPick.ResumeLayout();
        }

        private List<string> GetCheckedEvents()
        {
            List<string> checkedEvents = new List<string>();
            foreach (TreeNode node in EventsPick.Nodes)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (!child.Checked)
                        continue;
                    treeHier data = child.Tag as treeHier;
                    if (data != null && !string.IsNullOrEmpty(data.Tag))
                        checkedEvents.Add(data.Tag);
                }
            }
            return checkedEvents;
        }
        #endregion

        #region Tree Checkbox State
        private bool busy = false; // prevent recursive check events

        // TODO uncheck of child changes parent state

        private void EventsPick_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (busy) return;
            busy = true;
            try
            {
                checkNodes(e.Node, e.Node.Checked);
            }
            finally
            {
                busy = false;
            }

            SelectedEvents.Checked = true; // user changes to events implies they want to use 'selected events'
        }

        private void checkNodes(TreeNode node, bool check)
        {
            foreach (TreeNode node1 in node.Nodes)
            {
                node1.Checked = check;
                checkNodes(node1, check);
            }
        }
        #endregion

        private void OutFileSelect_Click(object sender, EventArgs e)
        {
            var ofd = new SaveFileDialog();
            ofd.Filter = "Text files|*.txt;*.TXT";
            ofd.FilterIndex = 1;
            ofd.DefaultExt = "txt";
            if (DialogResult.OK != ofd.ShowDialog(this))
            {
                return;
            }
            OutFileDisplay.Text = ofd.FileName;
            DoItState();
        }

        private void DoItState()
        {
            DoIt.Enabled = !string.IsNullOrWhiteSpace(OutFileDisplay.Text);
        }

        private void DoIt_Click(object sender, EventArgs e)
        {
            string outfile = OutFileDisplay.Text;
            if (string.IsNullOrWhiteSpace(outfile))
                return;

            // Filter data based on selected events
            List<One> filteredData = dataSet;
            if (!AllEvents.Checked)
            {
                List<string> checkedEvents = GetCheckedEvents();
                if (checkedEvents.Count != 0)
                {
                    filteredData = (from one in dataSet where checkedEvents.Contains(one.Tag) select one).ToList();
                }
            }

            if (LocOnly.Checked)
            {
                WriteLocationsOnly(filteredData, outfile);
            }
            if (LocSurname.Checked)
            {
                WriteLocationsNames(filteredData, outfile, surnamesonly: true);
            }
            if (LocPeople.Checked)
            {
                WriteLocationsNames(filteredData, outfile, surnamesonly: false);
            }

            Process.Start("notepad.exe", outfile);
        }

        private void CloseIt_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WriteLocationsOnly(List<One> filteredData, string path)
        {
            var sb = new StringBuilder();
            //foreach (var one in filteredData)
            //{
            //    sb.AppendFormat("{0}:{1}", one.Tag, one.Location);
            //    sb.AppendLine();
            //}
            var uniqueLocs = (from one in filteredData select one.Location).Distinct().OrderBy(name => name);
            foreach (var loc in uniqueLocs)
            {
                sb.AppendLine(loc);
            }
            File.WriteAllText(path, sb.ToString());
        }

        // TODO include person id when writing persons?

        private void WriteLocationsNames(List<One> filteredData, string path, bool surnamesonly)
        {
            var sb = new StringBuilder();
            MultiMap<string, string> locToSurname = new MultiMap<string, string>();
            foreach (var one in filteredData)
            {
                if (one.Indi != null) // INDI event
                {
                    string name = surnamesonly ? one.Indi.Surname : one.Indi.Name;
                    locToSurname.Add(one.Location, name);
                }
                if (one.Fam != null) // FAM event
                {
                    // TODO husband or wife not specified
                    string name1 = surnamesonly ? one.Fam.Husband.Surname : one.Fam.Husband.Name;
                    string name2 = surnamesonly ? one.Fam.Wife.Surname : one.Fam.Wife.Name;
                    locToSurname.Add(one.Location, name1);
                    locToSurname.Add(one.Location, name2);
                }
            }

            foreach (var loc in locToSurname.Keys.OrderBy(name=>name))
            {
                var names = locToSurname[loc];
                sb.Append(loc);
                sb.Append(" | ");
                foreach (var name in names.Distinct().OrderBy(name=>name))
                {
                    sb.Append(name);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2); // erase trailing comma
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString());
        }
    }
}
