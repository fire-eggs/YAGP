using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GEDWrap;
using System;
using System.Windows.Forms;

// ReSharper disable LocalizableElement

namespace IndiTable
{
    public partial class Form1 : Form
    {
        public event EventHandler LoadGed;

        private string LastFile { get; set; }

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            LoadGed += Form1_LoadGed;
        }

        private void loadGEDCOMToolStripMenuItem_Click(object sender, EventArgs e)
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
            LastFile = ofd.FileName; // TODO invalid ged file
            ProcessGED(ofd.FileName);
        }

        private void ProcessGED(string gedPath)
        {
            Text = gedPath;
            Application.DoEvents(); // Cycle events so image updates in case GED load/process takes a while
            LoadGed(this, new EventArgs());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Forest _gedtrees;
        //private int _visibleRows;

        private List<Person> _sortedData;

        private void Form1_LoadGed(object sender, EventArgs e)
        {
            EmptyGrid();

            _gedtrees = new Forest();
            _gedtrees.LoadGEDCOM(LastFile);

            _sortedData = _gedtrees.AllPeople.ToList();
            _sortedData.OrderBy(p => p.Id);

            FillGrid();
        }

        private bool sortAscending;
        private string sortCol;

        // Set datagridview columns
        private void LoadColumns()
        {
            var idCol = new DataGridViewTextBoxColumn();
            idCol.HeaderText = "Id";
            idCol.Name = "Id";
            var nameCol = new DataGridViewTextBoxColumn();
            nameCol.HeaderText = "Name";
            nameCol.Name = "Name";

            var bdateCol = new DataGridViewTextBoxColumn();
            bdateCol.HeaderText = "Birth Date";
            bdateCol.Name = "bdate";

            dataGridView1.Columns.Add(idCol);
            dataGridView1.Columns.Add(nameCol);
            dataGridView1.Columns.Add(bdateCol);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.VirtualMode = true;
            dataGridView1.CellValueNeeded += dataGridView1_CellValueNeeded;
            dataGridView1.ColumnHeaderMouseClick += dataGridView1_ColumnHeaderMouseClick;

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                dataGridView1,
                new object[] { true });

            LoadColumns();
        }

        private void loadComplete()
        {
            if (dataGridView1.RowHeadersVisible) // Done it already
                return; 

            dataGridView1.RowHeadersVisible = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        }

        void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (_sortedData == null) return;

            int dex = e.RowIndex;
            var name = dataGridView1.Columns[e.ColumnIndex].Name;

            Person p = _sortedData.ElementAt(dex);
            switch (name)
            {
                case "Id":
                    e.Value = p.Id;
                    break;
                case "Name":
                    e.Value = p.Name;
                    break;
                case "bdate":
                    var evt = p.GetEvent("BIRT");
                    e.Value = evt == null ? "" : evt.Date;
                    break;
            }

            if (dex >= dataGridView1.RowCount)
                loadComplete();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sortCol.Trim().ToUpper() != dataGridView1.Columns[e.ColumnIndex].Name.Trim().ToUpper())
            {
                dataGridView1.Columns[sortCol].HeaderCell.SortGlyphDirection = SortOrder.None;
                sortAscending = true;
            }
            else
            {
                sortAscending = (dataGridView1.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection != SortOrder.Ascending);
            }
            sortCol = dataGridView1.Columns[e.ColumnIndex].Name;

            EmptyGrid(false);

            // TODO *only* works if column name matches Person property name. Need comparators!!!
            // TODO comparer for id: sort numerically, not alphabetically
            // TODO track column, comparer, column header, etc : see GedKeeper listmanager

            PropertyInfo prop = typeof(Person).GetProperty(sortCol);
            if (prop != null)
            {
                if (sortAscending)
                    _sortedData = _sortedData.OrderBy(p => prop.GetValue(p)).ToList();
                else
                    _sortedData = _sortedData.OrderByDescending(p => prop.GetValue(p)).ToList();
            }
            FillGrid();
        }

        // TODO letter navigator

        private void EmptyGrid(bool reset = true)
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView1.Rows.Clear();

            if (reset)
            {
                _gedtrees = null;
                sortAscending = true;
                sortCol = "Id";
            }
            Application.DoEvents(); // Cycle events so grid updates in case GED load/process takes a while
        }

        private void FillGrid()
        {
            // Populate datagridview with individuals
            dataGridView1.RowCount = _sortedData.Count(); // TODO IEnumerable is not useful!
            dataGridView1.Columns[sortCol].HeaderCell.SortGlyphDirection = sortAscending ? SortOrder.Ascending : SortOrder.Descending;
        }
    }
}
