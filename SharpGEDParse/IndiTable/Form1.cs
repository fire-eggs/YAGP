﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using GEDWrap;
using System;
using System.Windows.Forms;

// ReSharper disable LocalizableElement
// ReSharper disable InconsistentNaming

// TODO neat if nav bar could deal with dates (i.e. date ranges)

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

            InitNavBar();
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
        private List<Person> _sortedData;

        private int tick1;
        private int tick2;
        private int tick3;

        private void Form1_LoadGed(object sender, EventArgs e)
        {
            EmptyGrid();

            tick1 = Environment.TickCount;

            _gedtrees = new Forest();
            _gedtrees.LoadGEDCOM(LastFile);

            tick2 = Environment.TickCount;

            _sortedData = _gedtrees.AllPeople.ToList();
            _sortedData.OrderBy(p => p.Id);

            fillingGrid = true;
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

            dataGridView1.Invalidated += dataGridView1_Invalidated;

            // double-buffer the grid for repaint performance
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                dataGridView1,
                new object[] { true });

            LoadColumns();
        }

        private bool fillingGrid = false;
        void dataGridView1_Invalidated(object sender, InvalidateEventArgs e)
        {
            // This is a hack ... I can find no mechanism to detect when the datagridview has
            // finished loading when in virtual mode. My attempt here is to use the first
            // repaint of the grid after the filling process has started.
            if (fillingGrid)
            {
                fillingGrid = false;
                gridLoadComplete();
            }
        }

        private void gridLoadComplete()
        {
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;

            tick3 = Environment.TickCount;
            showState();
        }

        private void showState()
        {
            int count = _sortedData.Count;
            int time1 = tick2 - tick1;
            int time2 = tick3 - tick2;
            var s = string.Format("{0:n0} INDI - Load:{1:n0} Grid:{2:n0}", count, time1, time2);
            lblStatus.Text = s;
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
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            bool sortChange = false;
            if (sortCol.Trim().ToUpper() != dataGridView1.Columns[e.ColumnIndex].Name.Trim().ToUpper())
            {
                dataGridView1.Columns[sortCol].HeaderCell.SortGlyphDirection = SortOrder.None;
                sortAscending = true;
                sortChange = true;
            }
            else
            {
                sortAscending = (dataGridView1.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection != SortOrder.Ascending);
            }
            sortCol = dataGridView1.Columns[e.ColumnIndex].Name;

            EmptyGrid(false, false);

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
            if (sortChange)
                updateNavBar();
        }

        private void EmptyGrid(bool reset = true, bool clear=true)
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            if (sortCol != null)
                dataGridView1.Columns[sortCol].HeaderCell.SortGlyphDirection = SortOrder.None;

            if (clear)
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
            if (dataGridView1.RowCount > 1)
                dataGridView1.Rows.Clear();

            // Populate datagridview with individuals
            dataGridView1.RowCount = _sortedData.Count(); // TODO IEnumerable is not useful!
            dataGridView1.Columns[sortCol].HeaderCell.SortGlyphDirection = sortAscending ? SortOrder.Ascending : SortOrder.Descending;
        }

        private void InitNavBar()
        {
            flowLayoutPanel1.WrapContents = false;
            for (char l = 'A'; l <= 'Z'; l++) // TODO localization
            {
                Label lbl = new Label();
                lbl.Text = l.ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;

                // scrunch as small as possible ... AutoSize leaves a huge gap w/ height
                lbl.AutoSize = false;
                using (Graphics g = CreateGraphics())
                {
                    var msr = g.MeasureString(lbl.Text, lbl.Font);
                    lbl.Width = (int) msr.Width+10; // allow a little slop, especially for 'I'
                    lbl.Height = (int) msr.Height+2;
                }
                lbl.Cursor = Cursors.Hand;
                lbl.Click += lbl_Click;
                flowLayoutPanel1.Controls.Add(lbl);
            }
        }

        void lbl_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl == null || CharMap == null)
                return;
            var val = lbl.Text[0];
            var dex = CharMap[val];
            dataGridView1.FirstDisplayedScrollingRowIndex = dex;
        }

        private Dictionary<char, int> CharMap;

        private void updateNavBar()
        {
            // TODO hack: nav bar not visible if sorting column isn't string
            flowLayoutPanel1.Visible = sortCol == "Name";

            // TODO must build in reverse order if sorted in reverse!!!

            // User has changed sorting column. 
            CharMap = new Dictionary<char, int>();
            for (char l = 'A'; l <= 'Z'; l++)
                CharMap.Add(l,0);
            char l2 = 'A';
            for (int i = 0; i < _sortedData.Count; i++)
            {
                var name = _sortedData[i].Name;
                if (string.IsNullOrEmpty(name))
                    continue;
                char first = Char.ToUpper(name[0]);
                if (first > 'Z')
                    continue;

                while (first > l2)
                    l2++;
                if (first == l2)
                {
                    CharMap[l2] = i;
                    l2++;
                }
                if (l2 > 'Z')
                    break;
            }

            foreach (var control in flowLayoutPanel1.Controls)
            {
                var lbl = control as Label;
                if (lbl == null)
                    continue;
                int dex = CharMap[lbl.Text[0]];
                lbl.Visible = dex > 0;
            }
        }
    }
}
