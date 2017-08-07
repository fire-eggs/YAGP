using DrawAnce;
using GEDWrap;
using PrintPreview;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
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

            Filler = "-";
            textBox1.Text = Filler;
            radioButton1.Checked = true;
            TopLabel = "Person";
            BotLabel = "Spouse";
            radioButton2.Checked = false;

            cmbWebFont.DataSource = _webFonts;
            cmbTheme.DataSource = _themes;
            cmbFontSize.DataSource = _fontSizes;
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
                var text = union.Id; // TODO add spouse names
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
        private FamSheet _famDraw ;
        private Pedigree _pedDraw;
        private Pedigree5 _ped5Draw;

        private void cmbFamilies_SelectedIndexChanged(object sender, EventArgs e)
        {
            var val = cmbFamilies.SelectedValue as Union;
            if (val == null)
                return;

            _famDraw = new FamSheet();
            _famDraw.Base = val;
            _famDraw.Trees = gedtrees;

            _family = val;

            _pedDraw = new Pedigree();
            _ped5Draw = new Pedigree5();

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

        private void fillWeb()
        {
            if (_family == null) // Invoked from event handlers
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");

            _pedDraw.DrawTo = sb;
            _ped5Draw.DrawTo = sb;

            _famDraw.DrawTo = sb;
            _famDraw.Filler = Filler;
            _famDraw.Spouse1Text = TopLabel;
            _famDraw.Spouse2Text = BotLabel;

            var fontFamily = cmbWebFont.SelectedItem as string;
            _famDraw.FontFam = fontFamily;
            var theme = cmbTheme.SelectedItem as string;
            _famDraw.Theme = theme;
            var fSize = cmbFontSize.SelectedItem as string;
            _famDraw.FontSize = fSize;

            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendFormat("body{{font-family: {0};font-size: 14px;padding: 30px 50px 50px 50px;margin: 0;}}",
                fontFamily).AppendLine();

            //_pedDraw.FillStyle();

            //_ped5Draw.FillStyle();

            _famDraw.FillStyle();

            sb.AppendLine("</style>");
            sb.AppendLine("<body>");

            //_ped5Draw.DrawChart();
            //_pedDraw.DrawChart();
            _famDraw.DrawChart();

            sb.AppendLine("</body></html>");
            webBrowser1.DocumentText = sb.ToString();
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                TopLabel = "Person";
                BotLabel = "Spouse";
            }
            else
            {
                TopLabel = "Husband";
                BotLabel = "Wife";
            }
            fillWeb();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Filler = textBox1.Text.Trim();
            if (_famDraw == null)
                return;
            _famDraw.Filler = Filler;
            fillWeb();
        }

        private string Filler { get; set; }
        private string TopLabel { get; set; }
        private string BotLabel { get; set; }

        private void cmbWebFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillWeb();
        }

        private static readonly string[] _webFonts =
        {
            "Arial,sans-serif",
            "Arial Black,sans-serif",
            "Courier New,monospace",
            "Garamond,serif",
            "Georgia,serif",
            "Helvetica,sans-serif",
            "Impact,sans-serif",
            "Palatino,serif",
            "Papyrus,fantasy",
            "Times New Roman,serif",
            "Trebuchet MS,sans-serif",
            "Verdana,sans-serif",
        };

        private static string[] _themes =
        {
            "blue",
            "grey",
            "green",
        };

        private static string[] _fontSizes =
        {
            "10",
            "12",
            "14",
            "16",
            "18",
            "20",
        };

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillWeb();
        }

        private void cmbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillWeb();
        }
    }
}
