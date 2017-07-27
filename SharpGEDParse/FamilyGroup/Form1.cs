using DrawAnce;
using GEDWrap;
using PrintPreview;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

// ReSharper disable InconsistentNaming
// ReSharper disable LocalizableElement

namespace FamilyGroup
{
    public partial class Form1 : Form
    {
        protected MruStripMenu mnuMRU;
        public event EventHandler LoadGed;
        private Forest gedtrees;
        readonly List<object> _cmbItems = new List<object>();

        public Form1()
        {
            InitializeComponent();

            mnuMRU = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMRU);
            mnuMRU.MaxEntries = 7;

            LoadSettings(); // NOTE: must go after mnuMRU init

            LoadGed += Form1_LoadGed;
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

        private void ProcessGED(string gedPath)
        {
            cmbFamilies.SelectedIndex = -1;
            cmbFamilies.DataSource = null;
            cmbFamilies.Enabled = false;
            cmbFamilies.Refresh();
            _cmbItems.Clear();

            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed(this, new EventArgs());
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        void Form1_LoadGed(object sender, EventArgs e)
        {

            gedtrees = new Forest();
            gedtrees.LoadGEDCOM(LastFile);
            DateEstimator.Estimate(gedtrees);

            foreach (var union in gedtrees.AllUnions)
            {
                var text = union.Id; // TODO add spous names
                _cmbItems.Add(new {Text=text, Value=union});
            }

            cmbFamilies.DisplayMember = "Text";
            cmbFamilies.ValueMember = "Value";
            cmbFamilies.DataSource = _cmbItems;
            cmbFamilies.SelectedIndex = 0;
            cmbFamilies.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Union _family;
        private List<Child> _childs;

        private void cmbFamilies_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = cmbFamilies.SelectedValue as Union;
            if (val == null)
                return;

            _family = val;
            _childs = new List<Child>();
            int i = 1;
            foreach (var person in val.Childs)
            {
                Child aCh = new Child(person, i);
                _childs.Add(aCh);
                i++;
            }
            fillWeb();
        }

        public PrintDocument PrintAncTree()
        {
            // TODO how/when is this disposed? 
            // TODO can this be pre-created somehow?

            PrintDocument pdoc = new PrintDocument();
            pdoc.DocumentName = "foobar";
            pdoc.BeginPrint += BeginPrint;
            pdoc.PrintPage += PrintPage;
            pdoc.QueryPageSettings += QueryPageSettings;
            pdoc.EndPrint += EndPrint;
            return pdoc;
        }

        void BeginPrint(object sender, PrintEventArgs e)
        {
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            e.HasMorePages = false; // TODO calculate
        }
        private void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
        }
        private void EndPrint(object sender, PrintEventArgs e)
        {
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnhancedPrintPreviewDialog newPreview = new EnhancedPrintPreviewDialog();
            newPreview.Owner = this;
            newPreview.Document = PrintAncTree();
            newPreview.ShowDialog();
        }

        private void printerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void printItToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // TODO need to escape strings for HTML

        // TODO move these to Person?
        private string GetWhat(Person who, string what)
        {
            string val = " ";
            var gedEvent = who.GetEvent(what);
            if (gedEvent != null && gedEvent.Descriptor != null)
                val = gedEvent.Descriptor;
            return WebUtility.HtmlEncode(val);
        }
        private string GetDate(Person who, string what)
        {
            string val = " ";
            var gedEvent = who.GetEvent(what);
            if (gedEvent != null && gedEvent.GedDate != null)
                val = gedEvent.GedDate.ToString(); // TODO provide format
            return WebUtility.HtmlEncode(val);
        }
        private string GetPlace(Person who, string what)
        {
            string val = " ";
            var gedEvent = who.GetEvent(what);
            if (gedEvent != null && !string.IsNullOrEmpty(gedEvent.Place))
                val = gedEvent.Place;
            return WebUtility.HtmlEncode(val);
        }

        private string GetParent(Person who, bool dad)
        {
            string val = " ";
            if (who.ChildIn != null && who.ChildIn.Count > 0)
            {
                // TODO adoption etc
                Union onion = who.ChildIn.ToArray()[0]; // TODO might not get the "right" one
                var val0 = dad ? onion.Husband : onion.Wife;
                val = val0 == null ? " " : val0.Name;
            }
            return WebUtility.HtmlEncode(val);
        }

        // TODO consistent styling w/ child table
        private void fillPerson(StringBuilder sb, string caption, Person who, bool inclMarr = false)
        {
            // Name-full Occupation
            // Born Place
            // Opt: Christened Place
            // inclMarr: Married Place
            // inclMarr: Divorced Place
            // Died Place
            // Opt: Buried Place
            // Parent Parent
            // Other spouses?
            sb.AppendLine("<table id=\"t1\">");
            sb.AppendFormat("<caption class=\"tt\">{0}</caption>", caption);
            if (who == null)
                sb.AppendLine("<tr><td>None</td></tr>");
            else
            {
                sb.AppendFormat("<tr><th width=\"15%\";></th><td id=\"d0\">{0}</td><th width=\"15%\";>Occupation</th><td id=\"d0\">{1}</td></tr>", who.Name, GetWhat(who, "OCCU")).AppendLine();
                sb.AppendFormat("<tr><th width=\"15%\";>Born</th><td id=\"d0\"; width=\"30%\";>{0}</td><th width=\"15%\";>Place</th><td id=\"d0\"; width=\"30%\";>{1}</td></tr>", GetDate(who, "BIRT"), GetPlace(who, "BIRT")).AppendLine();
                if (inclMarr)
                {
                    sb.AppendFormat("<tr><th width=\"15%\";>Married</th><td id=\"d0\"; width=\"30%\";>{0}</td><th width=\"15%\";>Place</th><td id=\"d0\"; width=\"30%\";>{1}</td></tr>", GetDate(who, "MARR"), GetPlace(who, "MARR")).AppendLine();
                }
                sb.AppendFormat("<tr><th width=\"15%\";>Died</th><td id=\"d0\">{0}</td><th width=\"15%\";>Place</th><td id=\"d0\">{1}</td></tr>", GetDate(who, "DEAT"), GetPlace(who, "DEAT")).AppendLine();
                sb.AppendFormat("<tr><th width=\"15%\";>Parent</th><td id=\"d0\">{0}</td><th width=\"15%\";>Parent</th><td id=\"d0\">{1}</td></tr>", GetParent(who, true), GetParent(who, false)).AppendLine();
            }
            sb.AppendLine("</table>");
        }

        private void fillWeb()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            fillStyle(sb);

            sb.AppendLine("<body><h3>Family Group Report</h3>");

            Person who = _family.Husband ?? _family.Wife;
            fillPerson(sb, "Person", who, true);

            sb.AppendLine("&nbsp;");

            who = _family.Spouse(who);
            fillPerson(sb, "Spouse", who);

            sb.AppendLine("&nbsp;");

            fillChildren(sb);

            sb.AppendLine("</body></html>");
            webBrowser1.DocumentText = sb.ToString();
        }

        // TODO consider placing in external file?
        private static readonly string[] STYLE_STRINGS =
        {
        ".tt{color:#333; background-color:#fff;font-family:Arial, sans-serif;font-size:14px;font-weight:bold;}",
        ".tg{width:100%;border-collapse:collapse;border-spacing:0;border-color:#ccc;}",
        ".tg td{font-family:Arial, sans-serif;font-size:13px;padding:4px 3px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;}",
        ".tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:5px 4px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;background-color:#aaa;}",
        ".tg .tg-9hbo{font-weight:bold;vertical-align:top}",
        ".tg .tg-9hboN{font-weight:bold;vertical-align:top;width:5%;}",
        ".tg .tg-dzk6{background-color:#f0f0f0;text-align:center;vertical-align:top}",
        ".tg .tg-b7b8{background-color:#f0f0f0;vertical-align:top}",
        ".tg .tg-yw4l{vertical-align:top}",
        ".tg .tg-baqh{text-align:center;vertical-align:top}",
        ".tNest {width:100%;border-collapse:collapse;}",
        ".tNest td{font-family:Arial, sans-serif;font-size:13px;padding:1px 1px;border-width:0px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;}",
        ".tNest .tNt{border-style:none none dashed none; border-width:1px; border-color:#ccc;}",
        ".tNest .tN-b7b8{background-color:#f0f0f0;vertical-align:top}",
        ".tNest .tNt-b7b8{border-style:none none dashed none; border-width:1px; border-color:#ccc;background-color:#f0f0f0;vertical-align:top}",
        };

        private void fillStyle(StringBuilder sb)
        {
            sb.AppendLine("<style type=\"text/css\">");

            //font-family: arial, sans-serif;
            sb.AppendLine("table#t1 { border-collapse: collapse; width: 100%; } ");
            sb.AppendLine("table#t2 { border-spacing: 0px; width: 100%; } ");
            //padding: 8px;
            sb.AppendLine("td#d0, th { border: 1px solid #dddddd; text-align: left; padding: 1px; }");
            sb.AppendLine("td#d1 {border: 1px solid #dddddd; text-align: center;}");
            sb.AppendLine("td#d2 {border-bottom: 1px dashed #dddddd;}");

            foreach (var s in STYLE_STRINGS)
            {
                sb.AppendLine(s);
            }
            sb.AppendLine("</style>");
        }

        // TODO consider placing in an external file?
        private static string[] childHeader =
        {
            "<tr>",
            "<th class=\"tg-9hboN\">No</th>",
            "<th class=\"tg-9hboN\">Id</th>",
            "<th class=\"tg-9hbo\">Name</th>",
            "<th class=\"tg-9hboN\">Sex</th>",
            "<th class=\"tg-9hbo\">BDate/DDate</th>",
            "<th class=\"tg-9hbo\">BPlace/DPlace</th>",
            "<th class=\"tg-9hbo\">Spouse / Details</th>",
            "</tr>",
        };

        private void fillChildren(StringBuilder sb)
        {
            sb.AppendLine("<table class=\"tg\">");
            sb.AppendLine("<caption class=\"tt\">Children</caption>");

            if (_childs.Count < 1)
            {
                sb.AppendLine("<tr><td>None</td></tr>");
            }
            else
            {
                foreach (var s in childHeader)
                {
                    sb.AppendLine(s);
                }
                int i = 1;
                foreach (var child in _childs)
                {
                    fillChild(sb, i, child);
                    i++;
                }
            }

            sb.AppendLine("</table>");
        }

        // TODO different style for alternate rows!
        private static string[] CHILD_ROW =
        {
"<tr>",
"<td class=\"tg-dzk6\">{0}</td>",
"<td class=\"tg-b7b8\">{0}</td>",
"<td class=\"tg-b7b8\">{0}</td>",
"<td class=\"tg-dzk6\">{0}</td>",
"<td class=\"tg-b7b8\">",
"<table class=\"tNest\">",
"<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
"<tr><td class=\"tN-b7b8\">{0}</td></tr>",
"</table>",
"</td>",
"<td class=\"tg-b7b8\"> <!-- bplace/dplace -->",
"<table class=\"tNest\">",
"<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
"<tr><td class=\"tN-b7b8\">{0}</td></tr>",
"</table>",
"</td>",
"<td class=\"tg-b7b8\"> <!-- Marriage details -->",
"<table class=\"tNest\">",
"<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
"<tr><td class=\"tN-b7b8\">{0};{1}</td></tr>",
"</table>",
"</td>",
"</tr>",
        };

        private void fillChild(StringBuilder sb, int num, Child child)
        {
            int dex = 0;
            foreach (var s in CHILD_ROW)
            {
                switch (dex)
                {
                    case 1:
                        sb.AppendFormat(s, num);
                        break;
                    case 2:
                        sb.AppendFormat(s, child.Id);
                        break;
                    case 3:
                        sb.AppendFormat(s, child.Name);
                        break;
                    case 4:
                        sb.AppendFormat(s, child.Sex);
                        break;
                    case 7:
                        sb.AppendFormat(s, child.BDate);
                        break;
                    case 8:
                        sb.AppendFormat(s, child.DDate);
                        break;
                    case 13:
                        sb.AppendFormat(s, child.BPlace);
                        break;
                    case 14:
                        sb.AppendFormat(s, child.DPlace);
                        break;
                    case 19:
                        sb.AppendFormat(s, child.MSpouse);
                        break;
                    case 20:
                        sb.AppendFormat(s, child.MDate, child.MPlace);
                        break;
                    default:
                        sb.AppendLine(s);
                        break;
                }
                dex++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void btnAutoSave_Click(object sender, EventArgs e)
        {
            var f = File.Open(@"z:\host_e\fam_grp_sht.html", FileMode.Create);
            var str = webBrowser1.DocumentText;
            var bytes = Encoding.UTF8.GetBytes(str);
            f.Write(bytes,0,bytes.Length);
            f.Close();
        }
    }
}
