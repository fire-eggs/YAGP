using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using hcwgenericclasses;

namespace timeline
{
    public partial class frmShowEvents : Form
    {
        public frmShowEvents()
        {
            InitializeComponent();
        }
        // OVERRIDE 1
        public frmShowEvents(string events)
        {
            
            InitializeComponent();
            EventsList = events;
            rtbShowEvents.Text = EventsList;
        }
        // OVERRIDE 2
        public frmShowEvents(string events, ref List <Form1.timelineentry> TEList)
        {
            InitializeComponent();
            EventsList = events;
            rtbShowEvents.Text = EventsList;
            foreach (Form1.timelineentry t in TEList)
            {
                Form1.timelineentry x = new Form1.timelineentry();
                x.Name = t.Name;
                x.Place = t.Place;
                x.Longitude = t.Longitude;
                x.Latitude = t.Latitude;
                x.FormattedAddress = t.FormattedAddress;
                x.EventType = t.EventType;
                x.Date = t.Date;
                x.Status = t.Status;
                LocalTEList.Add(x);
            }
            
        }
        public bool EventsChanged
        {
            get
            {
                return eventschanged;
            }
        }
        private bool eventschanged = false;
        public List<Form1.timelineentry> LocalEventsList
        {
            get
            {
                return LocalTEList;
            }
        }
        private RichTextTools rtt = new RichTextTools();
        private DialogTools dt = new DialogTools();
        private string EventsList = string.Empty;
        private List<Form1.timelineentry> LocalTEList = new List<Form1.timelineentry>();

        private void btnShowEventsClose_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        // SELECT ON CLICK HANDLER
        private void rtbShowEvents_MouseClick(object sender, MouseEventArgs e)
        {
            rtt.SelectOnClick(sender, e);
        }
        // EDIT SELECTED RECORD
        private void editRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbShowEvents.SelectedText.Split(' ')[1] != "RECORD:")
            {
                return;
            }
            string s = rtbShowEvents.SelectedText.Split(' ')[2];
            int x = Convert.ToInt16(s);
            x--;
            EditRecordForm erf = new EditRecordForm();
            erf.Name = LocalTEList[x].Name;
            erf.Place = LocalTEList[x].Place;
            erf.Date = LocalTEList[x].Date;
            erf.EventType = LocalTEList[x].EventType;
            erf.Address = LocalTEList[x].FormattedAddress;
            erf.Latitude = LocalTEList[x].Latitude;
            erf.Longitude = LocalTEList[x].Longitude;
            erf.Status = LocalTEList[x].Status;
            erf.RecordNumber = x;
            erf.ShowDialog(this);
            if (erf.SaveChanges)
            {
                LocalTEList[x].Name = erf.Name;
                LocalTEList[x].Place = erf.Place;
                LocalTEList[x].Date = erf.Date;
                LocalTEList[x].EventType = erf.EventType;
                LocalTEList[x].FormattedAddress = erf.Address;
                LocalTEList[x].Latitude = erf.Latitude;
                LocalTEList[x].Longitude = erf.Longitude;
                LocalTEList[x].Status = erf.Status;
                eventschanged = true;
            }
            erf.Dispose();
            this.Close();
            
        }
        // FORM LOAD
        private void frmShowEvents_Load(object sender, EventArgs e)
        {

        }
        // FORM CLOSING
        private void frmShowEvents_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        // DELETE RECORD
        private void deleteRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbShowEvents.SelectedText.Split(' ')[1] != "RECORD:")
            {
                return;
            }
            string s = rtbShowEvents.SelectedText.Split(' ')[2];
            int x = Convert.ToInt16(s);
            x--;
            if (dt.QueryDialog(this, "Delete This Record?", "Delete Selected Record"))
            {
                LocalTEList.RemoveAt(x);
                dt.NotifyDialog(this, "Record Deleted");
                eventschanged = true;

            }
            this.Close();
        }
    }
}
