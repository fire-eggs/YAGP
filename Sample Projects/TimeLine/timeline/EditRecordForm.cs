using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace timeline
{
    public partial class EditRecordForm : Form
    {
        public EditRecordForm()
        {
            InitializeComponent();
        }
       
       
      
        private int _recordnumber;
        private bool savechanges = false;
        private string name = String.Empty;
        private string date = String.Empty;
        private string eventtype = String.Empty;
        private string address = String.Empty;
        private string place = String.Empty;
        private string latitude = String.Empty;
        private string longitude = String.Empty;
        private string status = String.Empty;
        // ACCESSORS
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }
        public string EventType
        {
            get
            {
                return eventtype;
            }
            set
            {
                eventtype = value;
            }
        }
        public string Place
        {
            get
            {
                return place;
            }
            set
            {
                place = value;
            }
        }
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }
        public string Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }
        public string Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        public bool SaveChanges
        {
            get
            {
                return savechanges;
            }
            set { }
        }
        public int RecordNumber
        {
            get
            {
                return _recordnumber;
            }
            set
            {
                _recordnumber = value;
            }
        }


        // FORM LOAD
        private void EditRecordForm_Load(object sender, EventArgs e)
        {
            this.Text += " " + (_recordnumber+1).ToString();
            tbAddress.Text = address;
            tbPlace.Text = place;
            tbDate.Text = date;
            tbName.Text = name;
            tbEvent.Text = eventtype;
            tbLatitude.Text = latitude;
            tbLongitude.Text = longitude;
            tbStatus.Text = status;

        }
        // CANCEL BUTTON
        private void btnCancel_Click(object sender, EventArgs e)
        {
            savechanges = false;
            this.Close();
        }
        // SAVE CHANGES
        private void button1_Click(object sender, EventArgs e)
        {
            savechanges = true;
            address = tbAddress.Text;
            name = tbName.Text;
            date = tbDate.Text;
            eventtype = tbEvent.Text;
            place = tbPlace.Text;
            latitude = tbLatitude.Text;
            longitude = tbLongitude.Text;
            status = tbStatus.Text;
            this.Close();

        }

    }
}
