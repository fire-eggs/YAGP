using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using hcwgenericclasses;


namespace timeline
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        [Serializable]
        public class timelineentry
        {
            private string name = String.Empty;
            private string eventtype = String.Empty;
            private string date = String.Empty;
            private string place = String.Empty;
            // Results From Google
            private string status = String.Empty;
            private string formattedaddress = String.Empty;
            private string lat = String.Empty;
            private string lon = String.Empty;

            // Accessors
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
            public string FormattedAddress
            {
                get
                {
                    return formattedaddress;
                }
                set
                {
                    formattedaddress = value;
                }
            }
            public string Latitude
            {
                get
                {
                    return lat;
                }
                set
                {
                    lat = value;
                }
            }
            public string Longitude
            {
                get
                {
                    return lon;
                }
                set
                {
                    lon = value;
                }
            }


        }
        private ExceptionHandlerTools sfeht = new ExceptionHandlerTools();
        private DragDropTools sfddt = new DragDropTools();

        // CONVERT ADDRESS STRING TO GECODE REQUEST
        // LOCAL COPY
        public string FormatGeoCodeRequest(string address)
        {
            string result = string.Empty;
            string part1 = "https://maps.googleapis.com/maps/api/geocode/json?address=";
            string part2 = "&key=" + globals.GeoCodeAPIKey;
            string[] source = address.Split(' ');
            foreach (string s in source)
            {
                part1 += s + "+";
            }
            result = part1 + part2;
            return result;
        }
        // GETGEOCODE2
        // CALL WITH: a timelineemtry class object
        // RETURNS: fills in geocode relevant fields using callobject.Place
        private bool GetGeoCode2(ref timelineentry te)
        {
            byte[] buffer = new byte[2048];
            const string ADDRESS_FIELD = "formatted_address";
            const string LOCATION_FIELD = "location";
            string httprequest = String.Empty;

            httprequest = FormatGeoCodeRequest(te.Place);
            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] =
                "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                "(compatible; MSIE 6.0; Windows NT 5.1; " +
                ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                bool result = true;
                string result2 = String.Empty;

                // Download data.
                try
                {
                    buffer = client.DownloadData(httprequest);

                }
                catch (Exception ex)
                {
                    
                    sfeht.GeneralExceptionHandler("Http Request failed", "(10) - Test API Key", false, ex);
                    result = false;
                }
                finally
                {
                    client.Dispose();
                }
                if (result)
                {
                    foreach (byte b in buffer)
                    {
                        result2 += (Char)b;
                    }

                    string[] parsedresult = result2.Split('"');
                    te.Status = parsedresult[parsedresult.Length - 2]; // get status code
                    int x = 0;
                    // EXTRACT FORMATTED ADDRESS
                    for (x = 0; x < parsedresult.Length; x++)
                    {
                        if (parsedresult[x] == ADDRESS_FIELD)
                        {
                            te.FormattedAddress = parsedresult[x + 2];
                            te.Latitude = parsedresult[x + 5];
                            te.Longitude = parsedresult[x + 6];
                            break;
                        }
                    }
                    // EXTRACT LATITUDE AND LONGITUDE
                    for (x = 0; x < parsedresult.Length; x++)
                    {
                        if (parsedresult[x] == LOCATION_FIELD)
                        {
                            te.Latitude = parsedresult[x + 3].Substring(2, 10);
                            te.Longitude = parsedresult[x + 5].Substring(2, 10);
                            break;
                        }
                    }

                    return true;
                }
                return false;

            }

        }  
        // SAVE SETTINGS BUTTON
        private void btnSettingsFormOk_Click(object sender, EventArgs e)
        {
            globals.GeoCodeAPIKey = tbAPIKey.Text;
            globals.jsincludebirths = cbIncludeBirths.Checked;
            globals.jsincludedeaths = cbDeaths.Checked;
            globals.jincludemarriages = cbMarriages.Checked;
            globals.jsincludebaptisms = cbBaptisms.Checked;
            globals.jsincluderesidences = cbResidences.Checked;
            globals.jsincludeburials = cbBurials.Checked;
            globals.openhtmlfileaftersaving = cbAutoOpenHtml.Checked;
            globals.defaultbrowser = tbBrowserName.Text;
            globals.autoremovebaddates = cbRemoveEntriesWithBadDates.Checked;
            globals.autoremovebadgeocodes = cbRemoveBadGeoCodes.Checked;
            globals.autoremoveduplicateentries = cbAutoRemoveDuplicates.Checked;
            this.Close();
        }
        // FORM LOAD
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            tbAPIKey.Text = globals.GeoCodeAPIKey;
            cbIncludeBirths.Checked = globals.jsincludebirths;
            cbDeaths.Checked = globals.jsincludedeaths;
            cbMarriages.Checked = globals.jincludemarriages;
            cbBurials.Checked = globals.jsincludeburials;
            cbResidences.Checked = globals.jsincluderesidences;
            cbBaptisms.Checked = globals.jsincludebaptisms;
            cbAutoOpenHtml.Checked = globals.openhtmlfileaftersaving;
            tbBrowserName.Text = globals.defaultbrowser;
            cbRemoveBadGeoCodes.Checked = globals.autoremovebadgeocodes;
            cbRemoveEntriesWithBadDates.Checked = globals.autoremovebaddates;
            cbAutoRemoveDuplicates.Checked = globals.autoremoveduplicateentries;

            
        }
        // CANCEL BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // TEST KEY BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            btnTestApi.BackColor = button1.BackColor; // clear color to default
            timelineentry te = new timelineentry();
            te.Place = "1600 Pennsylvania Avenue, Washington D.C";
            te.Name = "President Lincoln";
            if (GetGeoCode2(ref te))
            {
                btnTestApi.BackColor = Color.Lime;
                toolTip1.SetToolTip(btnTestApi, "Google Maps API Key is Valid");
            }
            else
            {
                btnTestApi.BackColor = Color.Red;
                toolTip1.SetToolTip(btnTestApi, "Invalid API Key");
            }
        }
        // BROWSER NAME TB DRAG ENTER HANDLER
        private void tbBrowserName_DragEnter(object sender, DragEventArgs e)
        {
            sfddt.GenericDragEnterEventHandler(sender, e);
        }
        // BROWSER NAME TB DRAG DROP HANDLER
        private void tbBrowserName_DragDrop(object sender, DragEventArgs e)
        {
            tbBrowserName.Text = sfddt.GenericDragDropEventHandler(sender, e)[0];
        }
        // LINK TO INFORMATION ABOUT API KEYS
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start(globals.googleapikeyinfowebpage);
            }
            catch (Exception Ex)
            {
                sfeht.GeneralExceptionHandler("Unable to Open Web Page", "(14) - Settings Form", false, Ex);
            }
        }
    }
}
