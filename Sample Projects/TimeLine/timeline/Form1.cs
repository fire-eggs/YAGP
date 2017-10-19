using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using hcwgenericclasses;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;
using editform;


namespace timeline
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Timeline event class

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
        [Serializable]
        public class location
        {
            private string formattedaddress = String.Empty;
            private string lat = String.Empty;
            private string lon = String.Empty;
            private string place = String.Empty;
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
        }
        [Serializable]
        public class preferences
        {
            private string header;
            private string apikey;
            private string browser;
            private int flags;
            public string ApiKey
            {
                get
                {
                    return apikey;
                }
                set
                {
                    apikey = value;
                }
            }
            public string Browser
            {
                get
                {
                    return browser;
                }
                set
                {
                    browser = value;

                }
            }
            public int Flags
            {
                get
                {
                    return flags;
                }
                set
                {
                    flags = value;
                }
            }
            public string FileHeader
            {
                get
                {
                    return header;
                }
                set
                {
                    header=value;
                }
            }
        }
        // Sort Class for Duplicate Entry Removal
        public class sorttimelineentry
        {
            private timelineentry entry;
            private bool? duplicate;
            public timelineentry Entry
            {
                get
                {
                    return entry;
                }
                set
                {
                    entry = value;
                }
            }
            public bool? Duplicate
            {
                get
                {
                    return duplicate;
                }
                set
                {
                    duplicate = value;
                }
            }
        }
        // FOR GED INDIVIDUAL RECORDS
        public class GEDrecord
        {
            public int startingline;
            public int endingline;
        }
        // FOR LONGETIVITY CALCULATIONS
        public class ageentry
        {
            private string birth;
            private string death;
            private string name;
            private int recordnumber;

            public string Birth
            {
                get
                {
                    return birth;
                }
                set
                {
                    birth = value;
                }
            }
            public string Death
            {
                get
                {
                    return death;
                }
                set
                {
                    death = value;
                }
            }
            public int Age
            {
                get
                {
                    return calculateage();  // ADD CODE HERE
                }
                set { }
            }
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
            public int RecordNumber
            {
                get
                {
                    return recordnumber;
                }
                set
                {
                    recordnumber = value;
                }
            }
            private int calculateage()
            {
                int b = 0;
                int d = 0;
                int result = 0;
                try
                {
                    if (birth.Length >= 4)
                    {
                        b = Convert.ToInt16(birth.Substring((birth.Length - 4), 4));
                    }
                    else
                    {
                        b = Convert.ToInt16(birth.Substring((birth.Length - 3), 3));
                    }
                    if (death.Length >= 4)
                    {
                        d = Convert.ToInt16(death.Substring((death.Length - 4), 4));
                    }
                    else
                    {
                        d = Convert.ToInt16(death.Substring((death.Length - 3), 3));
                    }
                    if (d > b)
                    {
                        result = d - b;
                    }

                }
                catch
                {
                    result = 0;
                }
                return result;
            }
          

        }
        // DELEGATES

        public delegate int Rep(string search, string replace, bool match, int startpos, int function); // used for call-back
        public delegate void Down(); // used for call-back

        // SCROLLING

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, IntPtr lParam);

        // DELEGATE HANDLERS

        // REPLACE DELEGATE FUNCTION
        // CAN'T BE STATIC 01-11-2013

        public int ReplaceDelegateMethod(string search, string replace, bool match, int startpos, int function)
        {
            const int FIND = 1;
            const int FINDNEXT = 2;
            const int REPLACE = 3;
            const int REPLACEALL = 4;

            /* DEBUGMessageBox.Show("Search = "+search+" Replace = "+replace+" Match = "+match.ToString()
                , "Delegate Test", MessageBoxButtons.OK,
                MessageBoxIcon.Information);*/
            int currentposition = startpos;
            int stopposition = this.rtbMainFormResults.Text.Length - 1;  /* text or rtf? */
            switch (function)
            {
                case FIND:
                    {
                        this.rtbMainFormResults.Find(search);
                        this.rtbMainFormResults.Focus();
                        return (this.rtbMainFormResults.SelectionStart);
                    }
                case FINDNEXT:
                    {
                        if (search.Length == 0) // ERROR HANDLER EMPTY SEARCH FIELD
                        {
                            GeneralExceptionForm g = new GeneralExceptionForm("Find Text", "Find Field is Empty", "Error(01) - Replace Dialog", false, null);
                            g.ShowDialog();
                            g.Dispose();
                            return currentposition;

                        }
                        if (startpos < (stopposition)) // changed from stopposition-search.length
                        {
                            int searchresult = 0;
                            /*this.rtbMainForm1.SelectionStart = currentposition;*/
                            if (!match)
                            {
                                searchresult = this.rtbMainFormResults.Find(search, currentposition, stopposition, RichTextBoxFinds.None);
                            }
                            else // MATCH CASE
                            {
                                searchresult = this.rtbMainFormResults.Find(search, currentposition, stopposition, RichTextBoxFinds.MatchCase);
                            }

                            if (searchresult > 0)
                            {
                                this.rtbMainFormResults.Focus();
                                return searchresult;
                            }
                            else
                            {
                                return 0;
                            }

                        }
                        return 0;
                    }
                case REPLACE:
                    {

                        if (replace.Length == 0) // ERROR HANDLER EMPTY REPLACE FIELD
                        {
                            GeneralExceptionForm g = new GeneralExceptionForm("Replace Text", "Replace Field is Empty", "Error(02) - Replace Dialog", false, null);
                            g.ShowDialog();
                            g.Dispose();
                            return currentposition;
                        }
                        if (this.rtbMainFormResults.SelectedText.Length > 0) // SKIP IF NONE SELECTED
                        {
                            this.rtbMainFormResults.SelectedText = replace;
                        }
                        return currentposition;
                    }
                case REPLACEALL:
                    {
                        if (search.Length == 0 || replace.Length == 0) // ERROR HANDLER EMPTY SEARCH FIELD
                        {
                            GeneralExceptionForm g = new GeneralExceptionForm("Replace All", "Field(s) empty", "Error(03) - Replace Dialog", false, null);
                            g.ShowDialog();
                            g.Dispose();
                            return 0;

                        }
                        int searchresult = 1;
                        int count = 0;

                        while ((currentposition < stopposition) && searchresult >= 0) // changed from stopposition-search.length
                        {


                            if (!match)
                            {
                                searchresult = this.rtbMainFormResults.Find(search, currentposition, stopposition, RichTextBoxFinds.None);
                            }
                            else // MATCH CASE
                            {
                                searchresult = this.rtbMainFormResults.Find(search, currentposition, stopposition, RichTextBoxFinds.MatchCase);
                            }
                            if (this.rtbMainFormResults.SelectedText.Length > 0)
                            {
                                this.rtbMainFormResults.SelectedText = replace;
                                count++;
                                currentposition = searchresult + replace.Length;
                            }

                        }
                        dt.NotifyDialog(this, "Replaced " + count.ToString() + " items.");

                        return 1;
                    }

                default:
                    {
                        return 0;
                    }
            }

        }

        // SCROLL DELEGATE FUNCTION - SCROLL SELECTION UP INTO MIDDLE OF WINDOW
        // Usage: Scrolls selected text to middle line of current window regardless of size
        // Ver: 11-20-2016
        // REF http://stackoverflow.com/questions/205794/how-to-move-scroll-bar-up-by-one-line-in-c-sharp-richtextbox
        public void ScrollDownMethod()
        {
            int topline = rtbMainFormResults.GetLineFromCharIndex(rtbMainFormResults.GetCharIndexFromPosition(new Point(0, 0)));
            int bottomline = rtbMainFormResults.GetLineFromCharIndex(rtbMainFormResults.GetCharIndexFromPosition(new Point(rtbMainFormResults.ClientSize.Width,
                rtbMainFormResults.ClientSize.Height)));
            int currentline = rtbMainFormResults.GetLineFromCharIndex(rtbMainFormResults.GetFirstCharIndexOfCurrentLine());
            int middleline = topline + ((bottomline - topline) / 2);
            int linestoscroll = currentline - middleline;
            SendMessage(rtbMainFormResults.Handle, (uint)0x00B6, (UIntPtr)0, (IntPtr)(linestoscroll));
            return;

        }

        #region// List Objects

        private static List<timelineentry> TEList = new List<timelineentry>();
        private static List<string> Lines = new List<string>(); // source GED file
        private static List<string> JS = new List<string>(); // Jscript Function call code
        private static List<ageentry> Ages = new List<ageentry>();
        private static List<GEDrecord> Records = new List<GEDrecord>(); // holds individual records from GED file;
        private static List<string> Individuals = new List<string>(); // holds indidiual names
        private static List<location> KnownAddresses = new List<location>(); // stores previously geocoded addresses

        #endregion
        #region// Library Functions
        private FileTools ft = new FileTools();
        private ExceptionHandlerTools eht = new ExceptionHandlerTools();
        private DragDropTools ddt = new DragDropTools();
        private DialogTools dt = new DialogTools();
        private AboutBoxTools abt = new AboutBoxTools();
        private RichTextTools rtt = new RichTextTools();

        #endregion
        #region// Local Exception Details

        Exception exCreatWebPage = new Exception("(Application) Before the WebApp can be created, a GED source file"+
            " must have been loaded, parsed into timeline data and GeoCoded or an existing timeline.dat file with geocoding"+
            " must be loaded. ");
        Exception exNoSourceFile = new Exception("(Application) A source GED export file from Family Tree maker" +
            " must be opened before clicking the Timeline Data button to create Time Line Data for the Web App"+
            " or Clicking the View GED File Button. Note that when a timeline data file is loaded,"+
            " any GED source file opened by the program is closed.");
        Exception exGeoCodeFailed = new Exception("(Application) The Google GeoCode Request Failed. Check the API" +
            " key to be sure it works. Note the number of free geocode requests is limited to 2500 per day.");
        Exception exIncorrectDataFileFormat = new Exception("(Application) File selected is not a valid timeline" +
            " datafile. The default filename is timeline.dat.");
        

        #endregion
        #region// Local variables
        private static string filename = String.Empty;
        private bool fileloadsuccessfully = false;
        private int currentline = 0;
        private byte[] httpresponsebuffer = new byte[2048]; // holds a page of returned data;
        public bool cancelflag = false; // for async operations
        private string datafileheader = "TIMELINE VERSION1 DATAFILE"; // for 
        private string gedsourceversion = String.Empty;
        private static string gedfilename = String.Empty;
        private string googleapijavascript = (" <script type=\"text/javascript\"" +
             "src=\"https://maps.googleapis.com/maps/api/js?ver=3&?key="+
              globals.GeoCodeAPIKey+"\">" +
             "</script>");
        private string webpagetitle = "Timeline Version " + Application.ProductVersion.ToString() + " ";
        private Form SearchReference = null;
        #endregion
        // INJECT TITLE INTO HTML1
        // RETURNS: HTML1 with webpage title in H1
        private string InjectWebPageTitle(string sourcehtml)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(webpagetitle + " - Created: " + System.DateTime.Now.ToLongDateString());
            if (gedfilename.Length > 0)
            {
                sb.Append(" - GED Source File: " + gedfilename);
            }
            string result = String.Empty;
            string[] sarray = sourcehtml.Split('$');
            result = sarray[0] + sb.ToString() + sarray[1];
            return (result + "\n");
        }
        // INJECT API JAVA CODE INTO HTML1
        // Inserts API JS call into HTML at * placeholder
        private string InjectGoogleAPIJava(string sourcehtml)
        {
            string result = String.Empty;
            string[] sarray = sourcehtml.Split('*');
            result = sarray[0] + googleapijavascript + sarray[1];
            return (result + "\n");
        }
        // REMOVES DIACRITICAL MARKS FOR TEXT
        // CREDIT:  http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        // 09-15-2016
        private string NormalizeString(string source)
        {
            string accentedStr = source;
            string result = String.Empty;
            byte[] tempBytes;
            try
            {
                tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
                result = System.Text.Encoding.UTF8.GetString(tempBytes);
            }
            catch
            {
                result = String.Empty;
            }
            return result;
        }
        // STRING CLEANUP
        private string RemoveBadChars(string source)
        {
            string result = String.Empty;
            int x;
            ushort testch;
            // Remove any " characters from the name string, these cause syntax errors
            for (x = 0; x < source.Length; x++)
            {
                //  MODIFIED 09-15-2016if (Char.IsLetter(source[x]) || (source[x] == ' '))
                testch = Convert.ToUInt16(source[x]);
                if (((testch >= 0x30) && (testch <=0x7A)) || (testch == 0x20))
                {
                    if (source[x] == '/')
                    {
                        continue;
                    }
                    result += source[x];
                }
            }
            return result;
        }
        // CLEANS UP NAME STRING 
        private string GetShortName(timelineentry ti)
        {
            string result = String.Empty;
            int x;
            // Remove any " characters from the name string, these cause syntax errors
            for (x = 0; x < ti.Name.Length; x++)
            {
                if (Char.IsLetter(ti.Name[x]) || (ti.Name[x] == ' '))
                {
                    if (ti.Name[x] == '/')
                    {
                        continue;
                    }
                    result += ti.Name[x];
                }
            }
            try
            {
                if (result.Substring(0, 10) == "Record for")
                {
                    result = result.Substring(11);
                }
            }
            catch { };
            return result;
        }
        private bool NameExistsInAgesDatabase(string testname, int record)
        {
            bool result = false;
            foreach (ageentry a in Ages)
            {
                if (testname == a.Name && record == a.RecordNumber)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        // DELEGATE HANDLER FOR SPLASH FORM
        private void GetSettingsHandler()
        {
            LoadPreferenceFile(false);
        }
        // COMPILE AGE DATA FROM TIMELINE
        private void PopulateAgeDatabase()
        {
            int x = 0;
            int y = 0;
            Ages.Clear();
            bool entrycomplete = false;
            string shortname = String.Empty;
            for (x = 0; x < TEList.Count; x++)
            {
                shortname = GetShortName(TEList[x]);
                if (!(NameExistsInAgesDatabase(shortname,x)))
                {
                    entrycomplete = false;
                    if (TEList[x].EventType == "BIRT")
                    {
                        ageentry ag = new ageentry();
                        ag.Name = shortname;
                        ag.Birth = TEList[x].Date;
                        for (y = x; y < TEList.Count; y++)
                        {
                            if ((GetShortName(TEList[y]) == shortname) && (TEList[y].EventType == "DEAT"))
                            {
                                ag.Death = TEList[y].Date;
                                entrycomplete = true;
                                break;
                            }
                        }
                        if (entrycomplete == true)
                        {
                            ag.RecordNumber = x;
                            Ages.Add(ag);
                        }
                    }
                }
            }
        }     
        // UPDATE ACTIVITY LABEL
        private void UpdateLabel(string msg)
        {
            
            lblCurrentView.Text = msg+"                      ";
            lblCurrentView.Refresh();
            System.Threading.Thread.Sleep(500);
            
        }
        // SAVE ADDRESS FILE
        private void SaveAddressFile()
        {
            if (KnownAddresses.Count() == 0)
            {
                Exception saEx = new Exception("You can save geocoded addresses from the current timeline data\r\n" +
                    "to the known address file from the File menu. These can speed up future geocoding, particularly\r\n" +
                    "if you make additions to same Family Tree and re-export to a ged file in the future.");
                eht.GeneralExceptionHandler("No Address Records to Save", "(20) - SaveAddressFile", false, saEx);
                return;
            }
            string filename;
            filename = globals.addressfilename;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = false;
            sfd.CheckFileExists = false;
            sfd.Filter = "Data Files (*.DAT)|*.DAT|All Files (*.*)|*.*";
            sfd.FileName = filename;
            DialogResult Dr = sfd.ShowDialog(this);
            // CANCEL FILE SAVE
            if (Dr == DialogResult.Cancel)
            {
                sfd.Dispose();
                globals.addressfilesaved = false;
                return;
            }
            filename = sfd.FileName;
            sfd.Dispose();
            // check for existing file
            // No If statement needed
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                try
                {

                    globals.datafilesaved = false;
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        // HEADER
                        writer.Write(globals.addressfileheader);
                        Formatter.Serialize(fs, KnownAddresses);
                        globals.addressfilesaved = true;
                        
                    }
                }
                catch (Exception Ex)
                {
                    eht.GeneralExceptionHandler("Unable to save datafile", "(08) - SaveDataFile()", false, Ex);
                    globals.addressfilesaved = false;
                }
                finally
                {
                    fs.Close();
                    Formatter = null;
                    if (globals.addressfilesaved)
                    {
                        dt.NotifyDialog(this, "Address File Saved Successflly");
                    }
                        
                }
            }

        }
        // SAVE DATA FILE
        // SAVES HEADER + TElist parsed file records
        private void SaveDataFile()
        {
            if (TEList.Count() == 0 || (globals.datafilesaved && !globals.datafilechanged))
            {
                eht.GeneralExceptionHandler("No Parsed Records to Save", "(10) - SaveDataFile", false, null);
                return;
            }
            string filename;
            filename = globals.datafilename;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = false;
            sfd.CheckFileExists = false;
            sfd.Filter = "Data Files (*.DAT)|*.DAT|All Files (*.*)|*.*";
            sfd.FileName = filename;
            DialogResult Dr = sfd.ShowDialog(this);
            // CANCEL FILE SAVE
            if (Dr == DialogResult.Cancel)
            {
                sfd.Dispose();
                globals.datafilesaved = false;
                return;
            }
            filename = sfd.FileName;
            sfd.Dispose();
            // check for existing file

            if (globals.datafilechanged || globals.datafileloaded)
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                try
                {

                    globals.datafilesaved = false;
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        // HEADER
                        writer.Write(datafileheader);
                        //writer.Write((string)Globals.CatalogRootDirectory);
                        //writer.Write((string)Globals.CatalogCreateDate);
                        //writer.Write((Int32)ScanResults.Count());
                        // data
                        //writer.Write((string)tbDataTitle.Text);
                        Formatter.Serialize(fs, TEList);
                        globals.datafilesaved = true;
                        globals.datafilechanged = false;

                    }
                }
                catch (Exception Ex)
                {
                    eht.GeneralExceptionHandler("Unable to save datafile", "(08) - SaveDataFile()", false, Ex);
                }
                finally
                {
                    fs.Close();
                    Formatter = null;
                }
            }
            else
            {
                // 
                eht.GeneralExceptionHandler("No datafile to save", "(03) - SaveDataFile()", false, null);
            }

        }
        // LOAD DATA FILE
        private bool LoadDataFile()
        {
            string filename = globals.datafilename;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = filename;
            ofd.Filter = "Data Files (*.DAT)|*.DAT|All Files (*.*)|*.*";
            DialogResult Dr = ofd.ShowDialog(this);
            if (Dr != DialogResult.OK)
            {
                return false; // trap cancel
            }

            filename = ofd.FileName;

            string Header = String.Empty;
            if (ft.FileExists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryFormatter Formatter = new BinaryFormatter();
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    try
                    {
                        // READER HEADER
                        Header = reader.ReadString();
                        if (Header == datafileheader) /* check for correct header string */
                        {


                            // READ DATAFILE
                            TEList.Clear();

                            globals.datafileloaded = false;
                            TEList = (List<timelineentry>)Formatter.Deserialize(fs);
                            globals.datafileloaded = true;
                            globals.datafilechanged = false;
                            globals.datafileautofixperformed = false;
                            fileloadsuccessfully = false; // GED source, if any should be deleted
                            Lines.Clear();
                            globals.sourcefileloaded = false;
                            gedfilename = "";

                        }
                        else
                        {
                            eht.GeneralExceptionHandler("Unable to Load Datafile", "04 - LoadDataFile()", false, exIncorrectDataFileFormat);
                            return false;
                        }

                    }
                    catch (Exception ex)
                    {
                        eht.GeneralExceptionHandler("Unable to Read Data File", "05 - LoadDataFile()", false, ex);
                        globals.datafileloaded = false;
                        return false; ;
                    }
                    finally
                    {
                        fs.Close();
                        Formatter = null;

                    }
                    // RECREATE INDIVIDUALS LIST FROM SAVED DATAFILE
                    // 09-11-2016
                    Individuals.Clear();
                    
                    foreach (timelineentry t in TEList)
                    {
                        if (Individuals.Exists(element => element == t.Name))
                        {
                            continue;  // extract from the list of events only 1st instance of a name
                        }
                        Individuals.Add(t.Name);
                    }
                    // RESORT INDIVIDUALS BASED IND NUMBER THAT BEGINS THE NAME STRING
                    //09-11-2016
                    Individuals.Sort(delegate (string s1, string s2)
                    {
                        if (Convert.ToInt16(s1.Split('-')[0]) > Convert.ToInt16(s2.Split('-')[0]))
                        {
                            return 1;  // x < y, s1 < s2
                        }
                        else
                        {
                            return -1; // x > y, s1 > s2 or they are equal 
                        }
                    });
                    ViewParsedFile();
                    globals.datafileloaded = true;
                    globals.datafilechanged = false;
                    return true;

                }
            }
            else
            {
                // FILE DOESN'T EXIST
                eht.GeneralExceptionHandler("No Timeline data file in this directory", "06 - LoadDataFile()", false, null);
                return false;
            }

        }
        // LOAD USER PREFERENCE FILE
        private bool LoadPreferenceFile(bool allowuserintervention)
        {
            string path = Environment.CurrentDirectory;
            string filename = globals.preffilename;
            int x = 0;
            if (allowuserintervention)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = filename;
                ofd.Filter = "Data Files (*.DAT)|*.DAT|All Files (*.*)|*.*";
                DialogResult Dr = ofd.ShowDialog(this);
                if (Dr != DialogResult.OK)
                {
                    return false; // trap cancel
                }

                filename = ofd.FileName;
            }
            else
            {
                filename = path + "\\" + filename;
            }

            string Header = String.Empty;
            if (ft.FileExists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryFormatter Formatter = new BinaryFormatter();

                try
                {
                    preferences p = new preferences();
                    p = (preferences)Formatter.Deserialize(fs);
                    if (p.FileHeader == globals.preferencefileheader)
                    {
                       
                        globals.GeoCodeAPIKey = p.ApiKey;
                        globals.defaultbrowser = p.Browser;
                        globals.jsincludebirths = ((p.Flags & 1) != 0);
                        globals.jsincludedeaths = ((p.Flags & 2) != 0);
                        globals.jincludemarriages = ((p.Flags & 4) != 0);
                        globals.jsincludebaptisms = ((p.Flags & 8) != 0);
                        globals.jsincluderesidences = ((p.Flags & 16) != 0);
                        globals.jsincludeburials = ((p.Flags & 32) != 0);
                        globals.openhtmlfileaftersaving = ((p.Flags & 64) != 0);
                        globals.autoremovebaddates = ((p.Flags & 128) != 0);
                        globals.autoremovebadgeocodes = ((p.Flags & 256) != 0);
                        globals.autoremoveduplicateentries = ((p.Flags & 512) != 0);
                    }
                    else
                    {
                        eht.GeneralExceptionHandler("Incorrect Preference File Format", "06 - Load preferences", false, null);
                    }
                }

                catch (Exception ex)
                {
                    eht.GeneralExceptionHandler("Unable to Read Preference File", "05 - LoadUserFile()", false, ex);
                    return false;
                }
                finally
                {
                    fs.Close();
                    Formatter = null;

                }

                return true;

            }
            return false;
        }
        // LOAD ADDRESS FILE
        private bool LoadAddressFile()
        {
            bool result = false;
            string filename = globals.addressfilename;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = filename;
            ofd.Filter = "Data Files (*.DAT)|*.DAT|All Files (*.*)|*.*";
            DialogResult Dr = ofd.ShowDialog(this);
            if (Dr != DialogResult.OK)
            {
                return false; // trap cancel
            }

            filename = ofd.FileName;

            string Header = String.Empty;
            if (ft.FileExists(filename))
            {
                
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryFormatter Formatter = new BinaryFormatter();
                using (BinaryReader reader = new BinaryReader(fs))

                {
                    try
                    {
                        // READER HEADER
                        Header = reader.ReadString();
                        if (Header == globals.addressfileheader) /* check for correct header string */
                        {


                            // READ DATAFILE
                            KnownAddresses.Clear();
                            KnownAddresses = (List<location>)Formatter.Deserialize(fs);
                            globals.addressfileloaded = true;
                            result = true;
                            dt.NotifyDialog(this, "Loaded " + KnownAddresses.Count.ToString() + " saved addresses.");
                            

                        }
                        else
                        {
                            Exception laEx = new Exception("The file selected is not a timeline address file.");
                            eht.GeneralExceptionHandler("Unable to Address file", "04 - LoadAddressFile()", false, laEx);
                            result = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        eht.GeneralExceptionHandler("Unable to Read Address File", "05 - LoadAddressFile()", false, ex);
                        globals.datafileloaded = false;
                        return false; ;
                    }
                    finally
                    {
                        fs.Close();
                        Formatter = null;
                        

                    }
                   
                }
                
            }
            return result;
        }
        // CACHE GEOCODED ADDRESSES FROM CURRENT TIMELINE DATA
        private void UpdateSavedAddressList()
        {
            if (TEList.Count() == 0)
            {
                return;
            }
            int count = 0;
            
            foreach (timelineentry te in TEList)
            {

                if (te.Status == "OK")
                {
                    location testlocation = new location();
                    testlocation.Place = te.Place;
                    testlocation.Latitude = te.Latitude;
                    testlocation.Longitude = te.Longitude;
                    testlocation.FormattedAddress = te.FormattedAddress;
                       

                    if (!GetAddressFromCache(ref testlocation))
                    {
                        KnownAddresses.Add(testlocation);
                        count++;
                    }
                }

            }
            if (count > 0)
            {
                dt.NotifyDialog(this, count.ToString() + " locations added to saved addresses.");
            }
            else
            {
                dt.NotifyDialog(this, "No new locations saved");
            }
            return;  // DEBUG
        }
        // TEST IF ADDRESS IS IN KNOWN ADDRESSES
        private bool GetAddressFromCache(ref location test)
        {
            string searchname = string.Empty;
            bool searchresult = false;
            int x = 0;
            searchname = test.Place;
            for (x=0;x<KnownAddresses.Count();x++)
            {
                if (KnownAddresses[x].Place == searchname)
                {
                    searchresult = true;
                    test.FormattedAddress = KnownAddresses[x].FormattedAddress;
                    test.Latitude = KnownAddresses[x].Latitude;
                    test.Longitude = KnownAddresses[x].Longitude;
                    break;

                }

            }
            return searchresult;
        }
        // CREATE WEB PAGE FUNCTION CALL LIST
        private void CreateJSFunctionCalls()
        {
            JS.Clear();
            string functionname = "addMarker";
            string eventtype = string.Empty;
            string javaname = string.Empty;
            int x = 0;
            timelineentry priorentry = new timelineentry(); // to avoid duplicates
            foreach (timelineentry ti in TEList)
            {
                javaname = String.Empty;
                if (ti.Latitude.Length < 3 || ti.Longitude.Length < 3)
                {
                    continue; // trap bad lat or longitude data 03-31-2015
                }
                // BYPASS SINCE REMOVE DUPLICATES CODE HAS BEEN ADDED
                /*if ((ti.Name == priorentry.Name) && (ti.EventType == priorentry.EventType))
                {
                    continue; // trap following duplicates
                }*/
                // Remove any " characters from the name string, these cause syntax errors
                for (x = 0; x < ti.Name.Length; x++)
                {
                    if (Char.IsLetter(ti.Name[x]) || (ti.Name[x] == ' '))
                    {
                        if (ti.Name[x] == '/')
                        {
                            continue;
                        }
                        javaname += ti.Name[x];
                    }
                }
               
                if (ti.EventType == "DEAT" )
                {
                    if (globals.jsincludedeaths)
                    {
                        eventtype = "Death";
                    }
                    else { continue; };
                }
                if (ti.EventType == "BIRT")
                {
                    if (globals.jsincludebirths)
                    {
                        eventtype = "Birth";
                    }
                    else { continue; };
                }
                if (ti.EventType == "RESI" || ti.EventType == "CENS")
                {
                    if (globals.jsincluderesidences)
                    {
                        eventtype = "Residence";
                    }
                    else { continue; };
                }
                if (ti.EventType == "MARR")
                {
                    if (globals.jincludemarriages)
                    {
                        eventtype = "Marriage";
                    }
                    else { continue; };
                }
                if (ti.EventType == "BAPM")
                {
                    if (globals.jsincludebaptisms)
                    {
                        eventtype = "Baptism";
                    }
                    else { continue; };
                }
                if (ti.EventType == "BURI")
                {
                    if (globals.jsincludeburials)
                    {
                        eventtype = "Burial";
                    }
                    else { continue; };
                }
                if (ti.EventType == "CHR")
                {
                    if (globals.jsincludechristenings)
                    {
                        eventtype = "Christened";
                    }
                    else { continue; };
                }
                if (eventtype == String.Empty)
                {
                    continue; // trap other unwanted events
                }
                try
                {
                    if (javaname.Substring(0, 10) == "Record for")
                    {
                        javaname = javaname.Substring(11);
                    }
                }
                catch { };
                    
                string s = functionname + "(" + ti.Latitude.Trim(',') + "," + ti.Longitude.Trim(',') + ",\"" + javaname + @"\n" + 
                     eventtype+": "+ti.Date + " at "+NormalizeString(ti.FormattedAddress)+"\");";  // MOD 09-15-2016 NORMALIZESTRING()
                JS.Add(s);
                priorentry = ti;
            }
        }
        // VIEW JSCRIPT
        private void ShowJScript()
        {
            int count = 0;
            string filters = "Event Filter Includes: ";
            if (globals.jsincludebirths)
            {
                filters += " Births ";
            }
            if (globals.jsincludebaptisms)
            {
                filters += " Baptisms ";
            }
            if (globals.jsincludechristenings)
            {
                filters += " Christenings ";
            }
            if (globals.jincludemarriages)
            {
                filters += " Marriages ";
            }
            if (globals.jsincluderesidences)
            {
                filters += " Residences ";
            }
            if (globals.jsincludedeaths)
            {
                filters += " Deaths ";
            }
            if (globals.jsincludeburials)
            {
                filters += " Burials ";
            }
            StringBuilder sb = new StringBuilder();
            foreach (string s in JS)
            {
                sb.Append(s + "\n");
                count++;
            }
            rtbMainFormResults.Clear();
            rtbMainFormResults.Text = sb.ToString();
            lblCurrentView.Text = "Viewing Javascript - "+ count.ToString()+" events, " + filters;
        }
        // VIEW ADDRESS CACHE
        private void ViewSavedAddresses()
        {
            rtbMainFormResults.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (location l in KnownAddresses)
            {
                sb.Append("Place: " + l.Place + "\r\n" + "Formatted Address: " + l.FormattedAddress + "\r\n" + "Lat: " + l.Latitude + "\r\n" + "Long: " +
                    l.Longitude + "\r\n\n");
            }
            rtbMainFormResults.Text = sb.ToString();
            lblCurrentView.Text = "Viewing "+KnownAddresses.Count.ToString()+" Saved Addresses";
            
        }
        // DETECT DATAFILE WITH NO GEOCODES
        // If true, file has never been geocoded and autofix should not remove miscoded entries
        private bool DataFileContainsNoGecodes()
        {
            bool result = true;
            foreach (timelineentry te in TEList)
            {
                if (te.Status != "")
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        // CONVERT ADDRESS STRING TO GECODE REQUEST
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
        // GETGEOCODE
        // CALL WITH: formatted request string and byte[] buffer for response
        // RETURNS: true if request succeeded, result in buffer
        private bool GetGeoCode(string request, ref byte[] returnbuffer)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] =
                "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                "(compatible; MSIE 6.0; Windows NT 5.1; " +
                ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                bool result = true;
                // Download data.
                try
                {
                    returnbuffer = client.DownloadData(request);

                }
                catch (Exception ex)
                {
                    eht.GeneralExceptionHandler("Http Request failed", "(04) - GetGeoCode", false, exGeoCodeFailed);
                    result = false;
                }
                finally
                {
                    client.Dispose();
                }
                return result;
                
             }
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
                    eht.GeneralExceptionHandler("Http Request failed", "(04) - GetGeoCode", false, exGeoCodeFailed);
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
        // GETGEOCODEASYNC (Background Worker)
        // CALL WITH: A List Object of timelineentry classes (by ref)
        // RETURNS: fills in geocode data in list objects
        private void GetGeoCodesAsync(ref List<timelineentry> telist, ref bool cancel)
        {
            int count = telist.Count();
            int x = 0;
            for (x = 0; x < count; x++)
            {
                if (cancel || backgroundWorker1.CancellationPending)
                {
                    break;
                }
                backgroundWorker1.ReportProgress(x);
                if (telist[x].Status != "OK") // don't resubmit complete entries
                {
                    // TEST FOR ADDRESS IN CACHE

                    if (KnownAddresses.Count > 0 )
                    {
                        location loc = new location();
                        loc.Place = telist[x].Place;
                        if (GetAddressFromCache(ref loc))
                        {
                            telist[x].FormattedAddress = loc.FormattedAddress;
                            telist[x].Latitude = loc.Latitude;
                            telist[x].Longitude = loc.Longitude;
                            telist[x].Status = "OK";
                            continue;  // AVOID UNECESSARY GEOCODING IF ADDRESS IS IN CACHE
                        }
                        
                    }

                    // IF NOT CACHED, GEOCODE THE ENTRY

                    timelineentry te = new timelineentry();
                    te.Place = telist[x].Place;
                    GetGeoCode2(ref te);
                    telist[x].FormattedAddress = te.FormattedAddress;
                    telist[x].Latitude = te.Latitude;
                    telist[x].Longitude = te.Longitude;
                    telist[x].Status = te.Status;
                    
                }
            }
           
        }
        // ADD ENTRY TO LIST
        private bool AddEntry(string AEname, string AEeventtype, string AEdate)
        {
            timelineentry te = new timelineentry();
            te.Name = AEname;
            te.EventType = AEeventtype;
            te.Date = AEdate;
            TEList.Add(te);
            return true;
        }
        // VIEW PARSEDFILE
        private void ViewParsedFile()
        {
            int countvalidgeocodes = 0;
            int x;
            rtbMainFormResults.Clear();
            FixLatLong(); // MOD 03-28-2015
            StringBuilder sb = new StringBuilder();
            sb.Append("Total Events: " + TEList.Count().ToString() + "\n");
            for (x = 0; x < TEList.Count(); x++)
            {
                if (TEList[x].Status == "OK")
                {
                    countvalidgeocodes++;
                }
            }
            sb.Append("Correctly Geocoded: " + countvalidgeocodes.ToString() + "\n \n ");
            x = 1; // reuse variable
            foreach (timelineentry te in TEList)
            {
                sb.Append("RECORD: "+ x.ToString()+"\n Name: " + te.Name + "\n Type: " + te.EventType + "\n Date: " + te.Date + "\n Place: " + te.Place +
                    "\n Formatted Address: " + te.FormattedAddress + "\n Latitude: " + te.Latitude + "\n Longitude: " + te.Longitude +
                    "\n GeoCode Status: " + te.Status + "\n \n ");
                x++;
            }
            rtbMainFormResults.Text += sb;
            lblCurrentView.Text = "Viewing Parsed TimeLine Data";
            
        }
        // VIEW SOURCE FILE
        private void ViewSourceFile()
        {
            if (!fileloadsuccessfully)
            {
                eht.GeneralExceptionHandler("Source File Not Loaded", "03 - ViewSourceFile", false, exNoSourceFile);
                return;
            }
            rtbMainFormResults.Clear();
            int x;
            StringBuilder sb = new StringBuilder();
            for (x = 0; x < Lines.Count(); x++)
            {
                sb.Append(Lines[x] + "\n ");

            }
            rtbMainFormResults.Text = sb.ToString();
            lblCurrentView.Text = "Viewing GED Source";
        }
        // CREATE LIST OF INDIVIDUAL RECORDS IN THE GED FILE;
        private void CreateRecordList()
        {
            int count = 0,  x = 0,y = 0;
            bool done = false;
            Records.Clear();
            while (!done)
            {
               done = true;
               if (FindNextIndividual( ref x))
                {
                    count++;
                    GEDrecord ge = new GEDrecord();
                    ge.startingline = x;
                    y = x + 1;
                    if (FindNextIndividual(ref y))
                    {
                        ge.endingline = y - 1;
                        done = false;
                        x = y - 1;
                    }
                    else
                    {
                        ge.endingline = Lines.Count - 1; // use end of file if no more records;
                        done = true;
                    }
                    Records.Add(ge);
                }

            }
            rtbMainFormResults.Clear();
            rtbMainFormResults.Text = "\n Found " + count.ToString() + " individual records.\r\n ";
            return;

        }
        // RETURNS NEXT LINE BY REFERENCE THAT HOLDS A NAME FIELD
        // TRUE IF FIELD IDENTIFIER FOUND
        private bool FindNextName(ref int startposition)
        {
            bool result = false;
            int x = startposition;
            while (x < Lines.Count())
            {
                string[] words = Lines[x].Split(' ');
                if (words[1] == "NAME" || words[1] == "TEXT")
               
                {
                    result = true;
                    break;
                }
                
                x++;
            }
            startposition = x; // update reference variable in caller
            return result;
        }
        // RETURNS NEXT 0 @INDI@ RECORD START LINE
        // REV 04-30-2017 Ancestry.com GEDCOMs include a space after the last word in the line
        private bool FindNextIndividual(ref int startposition)
        {
            bool result = false;
            int x = startposition;
            while (x < Lines.Count)
            {
                string[] words = Lines[x].Split(' ');
                    if ((words[words.Length - 1] == "INDI") || (words[words.Length - 2] == "INDI")) // REV 04-30-2017
                    {
                        result = true;
                        break;
                    }
                    x++;
            }
            startposition = x;
            return result;
        }
        // RETURNS FIELD NAME FROM LINE SPECIFIED
        private string GetFieldType(int linetoread)
        {
            string[] words = Lines[linetoread].Split(' ');
            return words[1];
        }
        // EXTRACTS DATA FIELD FROM A SELECTED LINE
        // BUILDS A STRING FROM ALL WORDS AFTER THE FIRST 2 IN THE LINE
        private string LineData(int linetoread)
        {
            string s = string.Empty;
            int x;
            string[] words = Lines[linetoread].Split(' ');
            for (x = 2; x < words.Length; x++)
            {
                s += (words[x]+" ");
            }
            return s;
        }
        // EXTRACT PARTICULAR DATA TYPE IF PRESENT
        private string ExtractData(int linetoread, string type)
        {
            int x;
            string result = string.Empty;
            string[] words = Lines[linetoread].Split(' ');
            if (words[1] == type)
            {
                for (x = 2; x < words.Length; x++)
                {
                    result += (words[x]+" ");
                }
            }
            return result;
        }
        // INSURE LATITUDE AND LONGITUDE FORMATS ARE OK
        // Removes trailing spaces, commas, \n
        private void FixLatLong()
        {
            foreach (timelineentry te in TEList)
            {
                /*string[] s = te.Latitude.Split(',');
                te.Latitude = s[0];
                s = te.Longitude.Split(',');
                te.Longitude = s[0];*/
                string ts = te.Latitude;
                ts = ts.Trim(',');
                ts = ts.Trim(' ');
                ts = ts.Trim('\n');
                te.Latitude = ts;
                ts = te.Longitude;
                ts = ts.Trim(',');
                ts = ts.Trim(' ');
                ts = ts.Trim('\n');
                te.Longitude = ts;

            }
        }
        // STANDARDIZE DATE FIELDS
        private void FixDates()
        {
            int count = 0;
            foreach (timelineentry te in TEList)
            {
                count++;
                lblCurrentView.Text = "Standardizing Dates...Record -> " + count.ToString();
                lblCurrentView.Refresh();
                if (te.Date == "UNKNOWN" || te.Date == "NOWN" || te.Date == "NOWN " || te.Date == "UNKNOWN ") // trap unknown dates
                {
                    te.Date = "0000";
                    continue;
                }
                try
                {
                    te.Date = Convert.ToDateTime(te.Date).ToShortDateString();
                }
                catch // extract year if conversion fails
                {
                    string s = te.Date;
                    if (s.Length > 4) // if its already a year only, leave it alone
                    {
                        s = s.Substring((s.Length - 1) - 4, 4);
                        te.Date = s;
                    }
                }

            }
        }
        // CompareTo Method for Timeline Entries
        // Compares Years Only
        // Returns: - if t1<t2, + if t2<t1, 0 if equal
        private int CompareDates(timelineentry t1, timelineentry t2)
        {
            string input1, input2;
            int result = 0;
            if (t1.Date.Length > 4)
            {
                input1 = t1.Date.Substring(t1.Date.Length - 4, 4);
            }
            else
            {
                input1 = t1.Date;
            }
            if (t2.Date.Length > 4)
            {
                input2 = t2.Date.Substring(t2.Date.Length - 4, 4);
            }
            else
            {
                input2 = t2.Date;
            }

            int num1 = 0, num2 = 0;
            try
            {
                num1 = Convert.ToInt16(input1);
                num2 = Convert.ToInt16(input2);
                if (num1 < num2)
                {
                    result = -1;
                }
                else
                {
                    if (num1 > num2)
                    {
                        result = +1;
                    }

                }
            }
            catch // Conversion Failed
            {
            }
            return result;
        }   
        // SORT TELIST BY DATE
        // Uses: CompareDates as delegate
        private void SortListByDate()
        {
            TEList.Sort(delegate(timelineentry t1, timelineentry t2)
            {
                return CompareDates(t1, t2);
            });
        }
        // REMOVE ENTRIES WITH MISSING DATES
        private void RemoveEntriesMissingDates()
        {
            int x = 0;
            int count = 0;
            while (x < TEList.Count())
            {
                if (TEList[x].Date == "0000")
                { 
                    TEList.Remove(TEList[x]);
                    count++;
                    continue;
                }
                else
                {
                    x++;
                }
            }
            dt.NotifyDialog(this, count.ToString() + " entries without dates deleted");
            globals.datafilechanged = true;
            ViewParsedFile();
        }
        // REMOVE BAD GEOCODE ENTRIES
        private void RemoveBadGeocodes()
        {
            int x = 0;
            int count = 0;
            while (x < TEList.Count())
            {
                if (TEList[x].Status != "OK")
                {
                    TEList.Remove(TEList[x]);
                    count++;
                    continue;
                }
                else
                {
                    x++;
                }
            }
            dt.NotifyDialog(this, count.ToString() + " entries without geocodes deleted");
            globals.datafilechanged = true;
            ViewParsedFile();
        }
        // REMOVE DUPLICATE ENTRIES
        private void RemoveDuplicateEntries(bool displayonly)
        {
            if (TEList.Count == 0)
            {
                return;
            }
            List<sorttimelineentry> sortlist = new List<sorttimelineentry>();
            int count=0;
            int duplicatesfound =0;
            foreach (timelineentry te in TEList)
            {
                sorttimelineentry s = new sorttimelineentry();
                s.Entry = te;
                s.Duplicate = null;
                sortlist.Add(s);
            }
            foreach (sorttimelineentry s2 in sortlist)
            {
                int x;
                if (s2.Duplicate != null)
                {
                    continue;
                }
                s2.Duplicate = false; // first occurence is chosen as preferred by default
                for (x = 0; x < sortlist.Count; x++)
                {
                    if ((sortlist[x].Entry.Name == s2.Entry.Name) && (sortlist[x].Entry.EventType == s2.Entry.EventType) &&
                        (sortlist[x].Duplicate != false))
                    {
                        sortlist[x].Duplicate = true;
                        count++;
                    }
                }
             }
            duplicatesfound = count;
            if ((!displayonly)&&((globals.autoremoveduplicateentries) || (dt.QueryDialog(this, "Remove Duplicate Events?", "Change Time Line Data"))))
            {
                count = 0;
                TEList.Clear();
                foreach (sorttimelineentry s in sortlist)
                {
                    if (s.Duplicate == false)
                    {
                        timelineentry te = new timelineentry();
                        te = s.Entry;
                        TEList.Add(te); // reconstitute list with non-duplicates only
                        count++;
                    }
                }
                dt.NotifyDialog(this, "Removed " + duplicatesfound.ToString() +" duplicate events");
                ViewParsedFile();
            }
            else
            {
                if (duplicatesfound > 0)
                {
                    rtbMainFormResults.Clear();
                    StringBuilder sb = new StringBuilder();
                    count = 0;
                    sb.Append("Duplicate Events Found \n\n");
                    foreach (sorttimelineentry s in sortlist)
                    {
                        count++;
                        sb.Append("( " + count.ToString() + ")" + s.Entry.Name + " " + s.Entry.EventType + " " + s.Entry.Date + "\n");
                    }
                    rtbMainFormResults.Text = sb.ToString();
                    lblCurrentView.Text = duplicatesfound.ToString() + " Duplicate Events Found";
                }
            }
            
            return;
            
        }
        // CLOSE BUTTON
        private void btnmainFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // EVENT HANDLERS - Filename tb
        private void tbMainFormFilename_DragEnter(object sender, DragEventArgs e)
        {
            ddt.GenericDragEnterEventHandler(sender, e);
        }
        private void tbMainFormFilename_DragDrop(object sender, DragEventArgs e)
        {
            tbMainFormFilename.Text = ddt.GenericDragDropEventHandler(sender, e)[0];
        }
        // FORM LOAD
        private void Form1_Load(object sender, EventArgs e)
        {
            eht.ApplicationName = "Time Line";
            btnCancel.Enabled = false;
            progressBar1.Visible = false;
            btnMainFormEdit.Visible = false; // USED FOR DEBUGGING ONLY
            btnMainFormEdit.Enabled = false;
            SplashScreen();
            //LoadPreferenceFile(false);


        }
        // SPLASH SCREEN
        private void SplashScreen()
        {
            GetSet handler = GetSettingsHandler;
            SplashForm sf = new SplashForm(handler);
            sf.ShowDialog(this);
            sf.Dispose();
            return;
        }
        // OPEN SOURCE FILE BUTTON
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            // CHECK FOR DRAG AND DROP OR TYPED FILENAME

            filename = tbMainFormFilename.Text;
            if (filename.Length > 0)
            {
                if (!ft.FileExists(filename))
                {
                    eht.GeneralExceptionHandler("Source File Not Found", "01 - OpenFile", false, null);
                    return;
                }
                
            }

            // IF NO FILENAME IN TEXT BOX OPEN FILE DIALOG

            if (filename.Length == 0)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.FileName = filename;
                ofd.Title = "Open a GEDCOM Source File";
                ofd.Filter = "GED Files (*.ged)|*.GED|All Files (*.*)|*.*";
                DialogResult Dr = ofd.ShowDialog(this);
                if (Dr != DialogResult.OK)
                {
                    return; // trap cancel
                }

                filename = ofd.FileName;
            };
            rtbMainFormResults.Text += "Reading File... \n";
            Lines.Clear();
            globals.sourcefileloaded = false;
            StreamReader file = new StreamReader(filename);
            try
            {
                string line = String.Empty;

                while ((line = file.ReadLine()) != null)
                {
                    Lines.Add(line);
                }
                file.Close();
                fileloadsuccessfully = true;
                globals.sourcefileloaded = true;
                TEList.Clear(); // remove any preexisting timeline data;
                globals.datafileloaded = false;
                globals.datafilechanged = false;
                gedfilename = ft.GetShortFilename(filename);
                

            }
            catch (Exception ex)
            {
                eht.GeneralExceptionHandler("Error Reading Source File", "02 - FileRead", false, ex);
                fileloadsuccessfully = false;
                gedfilename = "";
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    tbMainFormFilename.Text = filename;
                }
                
            }
            if (globals.sourcefileloaded)
            {
                ViewSourceFile();
                this.btnMainFormViewFile.Focus();
                this.btnMainFormViewFile.Refresh();
            }

        }
        // PARSE FILE
        // REFLECTED TO PARSGEDFILE() 11-30-2015
        private void btnMainFormParse_Click(object sender, EventArgs e)
        {
            // MOD 11-30-2015
           
            ParseGEDFile();
            return;
            // END MOD
            if (!globals.sourcefileloaded && TEList.Count == 0)
            {
                eht.GeneralExceptionHandler("No Source File Loaded", "(11) - ParseFile", false, exNoSourceFile);
                ViewParsedFile();
                return;
            }
            else
            {
                if (TEList.Count > 0)
                {
                    ViewParsedFile();
                    return;
                }
            }
            int lastline = Lines.Count() - 1;
            int nextname = 0;

            // Intialize MOD 03-28-2015
            currentline = 0;
            TEList.Clear();

            // FLUSH HEADER ENTRY
            FindNextName(ref currentline);

            // MAIN LOOP
            while (currentline <= lastline)
            {
                string namefound = string.Empty;
                string eventfound = string.Empty;
                string datefound = string.Empty;
                string placefound = string.Empty;
                // UNUSED AS OF 09-15-2016 > string[] words;
                int x = 0;
                if (!FindNextName(ref currentline))
                {
                    break; // done 
                }
                x = currentline;
                namefound = LineData(currentline);
                eventfound = Lines[x + 2].Split(' ')[1];
                datefound = ExtractData((x + 3), "DATE");
                placefound = ExtractData((x + 4), "PLAC");
                if ((datefound != string.Empty) && (placefound != string.Empty))
                {
                    timelineentry te = new timelineentry();
                    te.Name = namefound;
                    te.EventType = eventfound;
                    te.Date = datefound;
                    te.Place = placefound;
                    TEList.Add(te);
                }

                currentline++;
               
             }

            rtbMainFormResults.Text += "\n Stardardizing Dates (may take several minutes large files) ...";
            FixDates();
            rtbMainFormResults.Text += "\n Sorting By Date...";
            SortListByDate(); // sort
            globals.datafilechanged = true;
            ViewParsedFile();
            
            
        }
        // PARSE GED
        // ADDED: 11-30-2015
        private void ParseGEDFile()
        {
            int x = 0;
            int recordstart, recordend = 0;
            int individualnumber = 0;
            string individualname = String.Empty;
            string _event = String.Empty;
            string _date = String.Empty;
            string _place = String.Empty;

            

            rtbMainFormResults.Clear();
            UpdateLabel("Parsing GED File");
            if (!globals.sourcefileloaded && TEList.Count == 0)
            {
                eht.GeneralExceptionHandler("No Source File Loaded", "(11) - ParseFile", false, exNoSourceFile);
                ViewParsedFile();
                return;
            }
            else
            {
                if (TEList.Count > 0)
                {
                    ViewParsedFile();
                    return;
                }
            }
            // 09-12-2016

            Individuals.Clear(); 
            
            // holds names found

            TEList.Clear();
            UpdateLabel("Finding Individuals...");
            rtbMainFormResults.Refresh();
            CreateRecordList();
            UpdateLabel("Found " + Records.Count.ToString() + " Individuals");
            foreach (GEDrecord record in Records)
            {
                recordstart = record.startingline;
                recordend = record.endingline;
                
                // 1st get Individual Name
                // REVISED 09-11-2016 Added Number to Name as Unique Identifier
                if (GetFieldType(recordstart+1) == "NAME")
                {
                    individualnumber++;
                    individualname = individualnumber.ToString()+"-"+RemoveBadChars(LineData(recordstart + 1));
                    // 09-11-2016
                    Individuals.Add(individualname);
                    //
                    for (x = (recordstart + 2) ;x < recordend;x++)
                    {
                        _event = GetFieldType(x);
                        if (_event == "TYPE" || _event == "CONC")
                        {
                            continue; // exclude these fields
                        }
                        
                        if ((GetFieldType(x+1) == "DATE") && (GetFieldType(x+2) == "PLAC"))
                        {
                            _date = LineData(x + 1);
                            _place = LineData(x + 2);
                            timelineentry te = new timelineentry();
                            te.Name = individualname;
                            te.Date = _date;
                            te.Place = _place;
                            te.EventType = _event;
                            TEList.Add(te);
                        }

                    }
                        
                 } 
            }
            UpdateLabel("Stardardizing Dates...");
            FixDates();
            UpdateLabel("Sorting By Date...");
            SortListByDate(); // sort
            globals.datafilechanged = true;
            globals.datafileloaded = true;
            ViewParsedFile();


        }
        // VIEW FILE
        private void btnMainFormViewFile_Click(object sender, EventArgs e)
        {
            ViewSourceFile();
            
        }
        // EDIT FILE
        private void btnMainFormEdit_Click(object sender, EventArgs e)
        {
            EditForm ef = new EditForm();
            ef.AllowRtf = true;
            ef.DocumentText = rtbMainFormResults.Rtf;
            ef.ShowDialog(this);
            rtbMainFormResults.Rtf = ef.DocumentText;
            ef.Dispose();
        }
        // LINK CLICKED EVENT HANDLER
        private void rtbMainFormResults_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
        // FILE MENU - QUIT
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // HELP MENU - ABOUT
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abt.Progam = "TimeLine";
            abt.Version = Application.ProductVersion.ToString();
            abt.Build = "April 2017";
            abt.Compiler = "Visual C# 2015";
            abt.Copyright = "HC Williams MD";
            System.Reflection.Assembly SRA = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream imagedata = SRA.GetManifestResourceStream("timeline.Resources.time_machine.png");
            abt.AboutBoxImage = imagedata;
            abt.UseCustomLicense(timeline.Properties.Resources.TIMELINELICENSE);
            abt.DisplayAboutBox();

        }
        // SETTINGS MENU OPTION
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog(this);
            sf.Dispose();

        }
        // GEOCODE BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            // rev 12-29-2015
            // trap missing key
            if (globals.GeoCodeAPIKey == "")
            {
                Exception keyexception = new Exception("Application Exception: The Google Maps API key is missing. Go to Options->Settings and " +
                    "enter a valid key or get a new one for free. Follow the link at the bottom of the settings form to obtain "+
                    "your own key, then fill in the key textbox and save your configuration. SEE THE HELP MENU.");
                eht.GeneralExceptionHandler("Missing Google Maps API Key", "(30) GeoCode()", false, keyexception);
                return;

            }
            // debugging code
            #region
            /* string result1 = string.Empty;
            string result2 = string.Empty;

            rtbMainFormResults.Clear();
            result1 = FormatGeoCodeRequest(TEList[1].Place);
            if (GetGeoCode(result1, ref httpresponsebuffer) == true)
            {
                foreach (byte b in httpresponsebuffer)
                {
                    result2 += (Char)b;
                }
                rtbMainFormResults.Clear();
                string[] parsedresult = result2.Split('"');
                StringBuilder sb = new StringBuilder();
                int x = 0;
                foreach (string s in parsedresult)
                {
                    sb.Append("String "+x.ToString()+" = "+s + "\n ");
                    x++;
                }
                rtbMainFormResults.Text = sb.ToString();

            }
            else
            {
                rtbMainFormResults.Text = "Https request failed";
            }
            return;
          
            timelineentry tetest = new timelineentry();
            tetest.Place = TEList[1].Place;
            if (GetGeoCode2(ref tetest))
            {
                ViewParsedFile();
            }
            else
            {
                rtbMainFormResults.Text = "GeoCode2 Call failed";
            }*/
            #endregion
            rtbMainFormResults.Clear();
            rtbMainFormResults.Text = "Geocoding " + TEList.Count().ToString() + " Addresses...";
            // NON ASYNC FORM
            #region
            /*
            int x=0;
            for (x=0;x<TEList.Count();x++)
            {
                rtbMainFormResults.Text = "GeoCoding Entry # " + x.ToString();
                timelineentry te = new timelineentry();
                te.Place = TEList[x].Place;
                GetGeoCode2(ref te);
                TEList[x].FormattedAddress = te.FormattedAddress;
                TEList[x].Latitude = te.Latitude;
                TEList[x].Longitude = te.Longitude;
                TEList[x].Status = te.Status;
            }
            ViewParsedFile();*/
            #endregion
            // SYNC FORM
            btnCancel.Enabled = true;
            progressBar1.Visible = true;
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            };
           
        }
        // BGWORKER 1 - DOWORK()
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
          
                BackgroundWorker worker = sender as BackgroundWorker;
                cancelflag = false;
                GetGeoCodesAsync(ref TEList, ref cancelflag);
            
        }
        // CANCEL BUTTON
        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancelflag = true;
            if (backgroundWorker1.WorkerSupportsCancellation)
            {
                backgroundWorker1.CancelAsync();
            }
        }
        // BACKGROUNDWORKER 1 COMPLETED
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cancelflag = false;
            btnCancel.Enabled = false;
            progressBar1.Visible = false;
            ViewParsedFile();

        }
        // BG1 PROGRESS CHANGED
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Maximum = TEList.Count();
            progressBar1.Value = e.ProgressPercentage; 
            progressBar1.Update();
            
        }
        // SAVE FILE MENU ITEM
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDataFile();
        }
        // LOAD DATA FILE MENU ITEM
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDataFile();
        }
        // SHOW JAVASCRIPT BUTTON
        private void btnJscript_Click(object sender, EventArgs e)
        {
            CreateJSFunctionCalls();
            ShowJScript();

        }
        // CREATE WEB PAGE BUTTON
        private void btnMainFormCreatWebPage_Click(object sender, EventArgs e)
        {
            string result = String.Empty;
            
            if (JS.Count == 0 && (TEList.Count > 0)) // if js has not been generated
            {
                if (!globals.datafileautofixperformed)
                {
                    if (dt.QueryDialog(this, "Check Data For Errors First", "Create Web Application"))
                    {
                        this.btnMainFormAutoFix_Click(sender, EventArgs.Empty);
                    }
                }
                CreateJSFunctionCalls();
            }

            if (JS.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                string part1 = InjectWebPageTitle(timeline.Properties.Resources.HTMLPART1);
                sb.Append(InjectGoogleAPIJava(part1));
                foreach (string s in JS)
                {
                    sb.Append(s + "\n");
                }
                sb.Append(timeline.Properties.Resources.HTMLPART2);
                rtbMainFormResults.Clear();
                rtbMainFormResults.Text = sb.ToString();
                lblCurrentView.Text = "Viewing HTML of TimeLine Web Application";
                result = SaveWorkingFile(globals.webappfilename);
            }
            else
            {
                eht.GeneralExceptionHandler("Incomplete or missing Time Line data", "(12)- Create Web Page", false, exCreatWebPage);
            }
            if (globals.openhtmlfileaftersaving && (result != String.Empty))
            {
                string filename = result;
                /*ProcessStartInfo PSI = new ProcessStartInfo();
                if (globals.defaultbrowser.Length > 0)
                {
                    PSI.FileName = globals.defaultbrowser;
                    if (!ft.FileExists(PSI.FileName))
                    {
                        eht.GeneralExceptionHandler("Invalid Web Browser Path", PSI.FileName, false, null);
                        return;
                    }
                    PSI.Arguments = result;
                }*/
                try
                {
                    System.Diagnostics.Process.Start(result); // Use Default Browser
                }
                catch (Exception Ex)
                {
                    eht.GeneralExceptionHandler("Unable to open " + filename, "(09) - Open File Context Menu",false, Ex);
                    return;
                }
            }
        }
        // SAVE HTML FILE FROM RTBMAIN
        // Returns: actual filename saved, or string.empty if failed
        private string SaveWorkingFile(string DefaultFileName)
        {
            
            string FileName = String.Empty;
            saveFileDialog1MainForm.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog1MainForm.DefaultExt = "html";
            saveFileDialog1MainForm.Title = "Save Working File";
            saveFileDialog1MainForm.Filter = "HTML Files (*.html)|*.html;*.HTML";
            saveFileDialog1MainForm.FilterIndex = 1;
            if (DefaultFileName.Length != 0)
            {
                saveFileDialog1MainForm.FileName = DefaultFileName;
            }
            bool FileSaved = false;
            if (saveFileDialog1MainForm.ShowDialog(this) != DialogResult.Cancel &&
                saveFileDialog1MainForm.FileName.Length > 0)
            {
                FileName = saveFileDialog1MainForm.FileName;
                try{
                    rtbMainFormResults.SaveFile(FileName, RichTextBoxStreamType.PlainText);
                    FileSaved = true;

                   }
                catch( Exception ex)
                {
                    eht.GeneralExceptionHandler("Unable to Save html file","(11) Save Web Page",false,ex);
                    FileSaved = false;
                }
            }
            if (FileSaved)
            {
                dt.NotifyDialog(this, "WebApp Saved");
            }
            if (FileSaved)
            {
                return FileName;
            }
            else
            {
                return String.Empty;
            }
        }
        // SHOW BAD GEOCODE ENTRIES
        private void showMissingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countinvalidgeocodes = 0;
            int x;
            rtbMainFormResults.Clear();
            FixLatLong(); // MOD 03-28-2015
            StringBuilder sb = new StringBuilder();
            sb.Append("Total Events: " + TEList.Count().ToString() + "\n");
            for (x = 0; x < TEList.Count(); x++)
            {
                if (TEList[x].Status != "OK")
                {
                    countinvalidgeocodes++;
                }
            }
            sb.Append("Incorrectly Geocoded: " + countinvalidgeocodes.ToString() + "\n \n");
            x = 1;
            foreach (timelineentry te in TEList)
            {
                if (te.Status != "OK")
                {
                    sb.Append("RECORD: "+ x.ToString()+"\n Name: " + te.Name + "\n Type: " + te.EventType + "\n Date: " + te.Date + "\n Place: " + te.Place +
                    "\n Formatted Address: " + te.FormattedAddress + "\n Latitude: " + te.Latitude + "\n Longitude: " + te.Longitude +
                    "\n GeoCode Status: " + te.Status + "\n \n ");
                }
                x++;
            }
            rtbMainFormResults.Text += sb;
        }
        // REMOVE DUPLICATES TOOLS MENU ITEM
        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveDuplicateEntries(false);
        }
        // REMOVE BAD GEOCODES TOOLS MENU ITEM
        private void removeEntriesWithBadGeocodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TEList.Count() > 0 && (dt.QueryDialog(this, "Remove Bad Geocodes?", "Modify Timeline Data")))
            {
                RemoveBadGeocodes();
            }

        }
        // REMOVE MISSING DATES TOOLS MENU ITEM
        private void removeEntriesWithMissingDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TEList.Count() > 0 && (dt.QueryDialog(this, "Remove Missing Dates?", "Modify Timeline Data")))
            {
                RemoveEntriesMissingDates();
            }
        }
        // DISPLAY MISSING DATES TOOLS ITEM
        private void showMissingDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countinvalidgeocodes = 0;
            int x;
            rtbMainFormResults.Clear();
            FixLatLong(); // MOD 03-28-2015
            StringBuilder sb = new StringBuilder();
            sb.Append("Total Events: " + TEList.Count().ToString() + "\n");
            for (x = 0; x < TEList.Count(); x++)
            {
                if (TEList[x].Date == "0000")
                {
                    countinvalidgeocodes++;
                }
            }
            sb.Append("Missing Dates: " + countinvalidgeocodes.ToString() + "\n \n");
            foreach (timelineentry te in TEList)
            {
                if (te.Date == "0000")
                {
                    sb.Append("Name: " + te.Name + "\n Type: " + te.EventType + "\n Date: " + te.Date + "\n Place: " + te.Place +
                    "\n Formatted Address: " + te.FormattedAddress + "\n Latitude: " + te.Latitude + "\n Longitude: " + te.Longitude +
                    "\n GeoCode Status: " + te.Status + "\n \n ");
                }
            }
            rtbMainFormResults.Text += sb;
            lblCurrentView.Text = "Entries with Missing dates";
        }
        // DISPLAY BAD GEOCODE TOOLS MENU ITEM
        private void showBadGeocodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countinvalidgeocodes = 0;
            int x;
            rtbMainFormResults.Clear();
            FixLatLong(); // MOD 03-28-2015
            StringBuilder sb = new StringBuilder();
            sb.Append("Total Events: " + TEList.Count().ToString() + "\n");
            for (x = 0; x < TEList.Count(); x++)
            {
                if (TEList[x].Status != "OK")
                {
                    countinvalidgeocodes++;
                }
            }
            sb.Append("Incorrectly Geocoded: " + countinvalidgeocodes.ToString() + "\n \n");
            x = 1;
            foreach (timelineentry te in TEList)
            {
                if (te.Status != "OK")
                {
                    sb.Append("RECORD: "+ x.ToString()+"\n Name: " + te.Name + "\n Type: " + te.EventType + "\n Date: " + te.Date + "\n Place: " + te.Place +
                    "\n Formatted Address: " + te.FormattedAddress + "\n Latitude: " + te.Latitude + "\n Longitude: " + te.Longitude +
                    "\n GeoCode Status: " + te.Status + "\n \n ");
                }
                x++;
            }
            rtbMainFormResults.Text += sb;
            lblCurrentView.Text="Entries with Bad/Missing GeoCodes";
        }
        // DISPLAY DUPLICATES TOOLS MENU ITEM
        private void showDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveDuplicateEntries(true);
        }
        // AUTOFIX BUTTON
        private void btnMainFormAutoFix_Click(object sender, EventArgs e)
        {
            if (TEList.Count == 0)
            {
                return;
            }
            if (globals.autoremoveduplicateentries)
            {
                RemoveDuplicateEntries(false);
            }
            if (globals.autoremovebaddates)
            {
                RemoveEntriesMissingDates();
            }
            if (globals.autoremovebadgeocodes && !DataFileContainsNoGecodes())
            {
                RemoveBadGeocodes(); // Don't Do this if the file has never been coded
            }
            globals.datafileautofixperformed = true;
            ViewParsedFile();


        }
        // RTB SELECT-ON-CLICK HANDLER
        private void rtbMainFormResults_MouseClick(object sender, MouseEventArgs e)
        {
            if (lblCurrentView.Text == "Viewing Parsed TimeLine Data" || lblCurrentView.Text == "Entries with Bad/Missing GeoCodes" ||
                lblCurrentView.Text.Split(' ')[0] == "Display")
            {
             rtt.SelectOnClick(sender, e);
            }
        }
        // EDIT RECORD CONTEXT MENU ITEM
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.SelectedText.Split(' ')[1] != "RECORD:")
            {
                return;
            }
            string s = rtbMainFormResults.SelectedText.Split(' ')[2];
            int x = Convert.ToInt16(s);
            x--;
            EditRecordForm erf = new EditRecordForm();
            erf.Name = TEList[x].Name;
            erf.Place = TEList[x].Place;
            erf.Date = TEList[x].Date;
            erf.EventType = TEList[x].EventType;
            erf.Address = TEList[x].FormattedAddress;
            erf.Latitude = TEList[x].Latitude;
            erf.Longitude = TEList[x].Longitude;
            erf.Status = TEList[x].Status;
            erf.RecordNumber = x;
            erf.ShowDialog(this);
            if (erf.SaveChanges)
            {
                TEList[x].Name = erf.Name;
                TEList[x].Place = erf.Place;
                TEList[x].Date = erf.Date;
                TEList[x].EventType = erf.EventType;
                TEList[x].FormattedAddress = erf.Address;
                TEList[x].Latitude = erf.Latitude;
                TEList[x].Longitude = erf.Longitude;
                TEList[x].Status = erf.Status;
                dt.NotifyDialog(this, "Changes Saved");
                globals.datafilechanged = true;
            }
            erf.Dispose();
            ViewParsedFile();

        }
        // LONGETIVITY TOOLS MENU ITEM
        private void averageLongetivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            PopulateAgeDatabase();
            rtbMainFormResults.Clear();
            int x =1, total = 0;
            int averageage = 0;
            foreach (ageentry a in Ages)
            {
                sb.Append("( " + a.RecordNumber.ToString() + " ) " + a.Name + " " + a.Birth + " - " + a.Death + " Lived " +
                    a.Age.ToString() + " years\n");
                total += a.Age;
                x++;
            }
            averageage = total / x;
            sb.Append("\n\n Average Longetivity of " + x.ToString() + " relatives is " +
                averageage.ToString() + " years.\n");
            rtbMainFormResults.Text = sb.ToString();
            lblCurrentView.Text = "Longetivity Analysis";

        }
        // SAVE USER PREFERENCES
        private void saveConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            string filename;
            int x = 0;
            // BITWISE ENCODE SETTINGS
            if (globals.jsincludebirths) { x += 1; };
            if (globals.jsincludedeaths) { x += 2; };
            if (globals.jincludemarriages) { x += 4; };
            if (globals.jsincludebaptisms) { x += 8; };
            if (globals.jsincluderesidences) { x += 16; };
            if (globals.jsincludeburials) { x += 32; };
            if (globals.openhtmlfileaftersaving) { x += 64; };
            if (globals.autoremovebaddates) { x += 128; };
            if (globals.autoremovebadgeocodes) { x += 256; };
            if (globals.autoremoveduplicateentries) { x += 512; };
            
            filename =  globals.preffilename;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckPathExists = false;
            sfd.CheckFileExists = false;
            sfd.Filter = "CFG Files (*.CFG)|*.CFG|All Files (*.*)|*.*";
            sfd.FileName = filename;
            DialogResult Dr = sfd.ShowDialog(this);
            // CANCEL FILE SAVE
            if (Dr == DialogResult.Cancel)
            {
                sfd.Dispose();
                globals.userpreferencessaved = false;
                return;
            }
            filename = sfd.FileName;
            sfd.Dispose();
            preferences p = new preferences();
            p.FileHeader = globals.preferencefileheader;
            p.ApiKey = globals.GeoCodeAPIKey;
            p.Browser = globals.defaultbrowser;
            p.Flags = x;
                BinaryFormatter Formatter = new BinaryFormatter();
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                try
                {

                    globals.userpreferencessaved = false;
                    Formatter.Serialize(fs, p);
                    globals.userpreferencessaved = true;
                    dt.NotifyDialog(this, "User Preferences Saved");
                        
                }
                catch (Exception Ex)
                {
                    eht.GeneralExceptionHandler("Unable to save Preferences", "(08) - SaveDataFile()", false, Ex);
                }
                finally
                {
                    fs.Close();
                    Formatter = null;
                }
            
        }
        // LOAD USER PREFERENCE FILE
        private void loadSavedConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadPreferenceFile(true))
            {
                dt.NotifyDialog(this, "User Preferences Loaded");
            }

        }
        // HELP FORM
        private void usingTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.ShowDialog(this);
            hf.Dispose();

        }
        // CLEAR TEXT BOX BUTTON
        private void btnMainFormClearTextBox_Click(object sender, EventArgs e)
        {
            tbMainFormFilename.Clear();
            tbMainFormFilename.Focus();
        }
        // DISPLAY LIST OF INDIVIDUALS FOUND
        private void displayListOfIndividualsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Individuals.Count == 0)
            {
                return; // no info to display
            }
            rtbMainFormResults.Clear();
            StringBuilder sbl = new StringBuilder();
            foreach (string s in Individuals)
            {
                sbl.Append(s + "\r\n");
            }
            
            rtbMainFormResults.Text = sbl.ToString();
            UpdateLabel("Display List of Individuals - " + Individuals.Count.ToString());
            
        }
        // PRINT WHATEVER IS IN THE RTB
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.Lines.Count() > 0)
            {
                Exception ex = new Exception("A problem occurred while printing");
                rtt.GeneralPrintForm("Print", rtbMainFormResults.Rtf, ref ex);
                
            }
            else
            {
                dt.NotifyDialog(this, "Nothing to Print");
            }
            return;
        }
        // DELETE RECORD CONTEXT MENU ITEM
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.SelectedText.Split(' ')[1] != "RECORD:")
            {
                return;
            }
            string s = rtbMainFormResults.SelectedText.Split(' ')[2];
            int x = Convert.ToInt16(s);
            x--;
            if (dt.QueryDialog(this,"Delete This Record?","Delete Selected Record"))
            {
                TEList.RemoveAt(x);
                dt.NotifyDialog(this, "Record Deleted");

            }
            ViewParsedFile();
        }
        // SHOW INDIVIDUAL'S EVENTS CONTEXT MENU ITEM
        private void showEventsForIndividualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.SelectedText.Length == 0)
            {
                return; // exit if no selection
            }
            if (rtbMainFormResults.SelectedText.Split(' ')[1] == "RECORD:")
            {
                return;
            }
            string s = rtbMainFormResults.SelectedText.Split('-')[0];
            int x = 0;
            // CREATE EVENT LIST
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            for (x=0;x<TEList.Count();x++)
            {
                if (TEList[x].Name.Split('-')[0] == s)
                {
                    timelineentry te = new timelineentry();
                    te = TEList[x];
                    sb.Append("RECORD: " + (x+1).ToString() + "\n Name: " + te.Name + "\n Type: " + te.EventType + "\n Date: " + te.Date + "\n Place: " + te.Place +
                   "\n Formatted Address: " + te.FormattedAddress + "\n Latitude: " + te.Latitude + "\n Longitude: " + te.Longitude +
                   "\n GeoCode Status: " + te.Status + "\n \n ");
                }
            }

            frmShowEvents fse = new frmShowEvents(sb.ToString(), ref TEList);
            fse.ShowDialog(this);
            if (fse.EventsChanged)
            {
                TEList.Clear();
                foreach (timelineentry t in fse.LocalEventsList)
                {
                    timelineentry tee = new timelineentry();
                    tee.Name = t.Name;
                    tee.Place = t.Place;
                    tee.FormattedAddress = t.FormattedAddress;
                    tee.Latitude = t.Latitude;
                    tee.Longitude = t.Longitude;
                    tee.Status = t.Status;
                    tee.Date = t.Date;
                    tee.EventType = t.EventType;
                    TEList.Add(tee);

                }
                dt.NotifyDialog(this, "Saved Changes to Events List");
                globals.datafilechanged = true;
            }
            fse.Dispose();
            
            return;
        }
        // CONTEXT MENU OPENING EVENT HANDLER - CUSTOMIZE CONTEXT MENU
        private void cmenu1_Opening(object sender, CancelEventArgs e)
        {
           
            this.toolStripMenuItem1.Visible = false;
            this.toolStripMenuItem2.Visible = false;
            showEventsForIndividualToolStripMenuItem.Visible = false;
            try
            {
                if (rtbMainFormResults.SelectedText.Split(' ')[1] == "RECORD:")
                {
                    this.toolStripMenuItem1.Visible = true;
                    this.toolStripMenuItem2.Visible = true;

                }
                else
                {
                    string s = rtbMainFormResults.SelectedText.Split('-')[0];
                    int test;
                    if (int.TryParse(s, out test)) // Is it an integer
                    {
                        showEventsForIndividualToolStripMenuItem.Visible = true;


                    }
                }
            } catch 
            {
            }
        }
        // SAVE ADDRESS FILE MENU ITEM
        private void saveAddressFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAddressFile();
        }
        // LOAD ADDRESS FILE MENU ITEM
        private void loadAddressFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAddressFile();
        }
        // UPDATE SAVED ADDRESS CACHE MENU ITEM
        private void updateSavedAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateSavedAddressList();
        }
        // VIEW SAVED ADDRESSES MENU ITEM
        private void viewSavedAddresesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewSavedAddresses();
        }
        // CLEAR SAVED ADDRESSES
        private void clearSavedAddressListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dt.QueryDialog(this,"Erase Saved Addresses?","Clear Saved Address List"))
            {
                KnownAddresses.Clear();
                globals.addressfileloaded = false;
                dt.NotifyDialog(this, "Saved Address List cleared.");
                ViewSavedAddresses();
            }
        }
        // PRINT CURRENT VIEW
        private void currentViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.Lines.Count() > 0)
            {
                Exception ex = new Exception("A problem occurred while printing");
                rtt.GeneralPrintForm("Print", rtbMainFormResults.Rtf, ref ex);

            }
            else
            {
                dt.NotifyDialog(this, "Nothing to Print");
            }
            return;
        }
        // OPEN CURRENT VIEW IN EDITOR BEFORE PRINTING
        private void editBeforePrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbMainFormResults.Lines.Count() > 0)
            {
                editor ed = new editor();
                ed.Document = rtbMainFormResults.Rtf;
                ed.WindowTitle = "Timeline Editor - Edit Current View Before Printing";
                ed.AllowRtf = true;
                ed.AllowDiscAccess = true;
                ed.UseSaveFileDialogWhenClosing = false;
                ed.DisplayEditForm(this);
                
            }
            else
            {
                dt.NotifyDialog(this, "Current View is Empty");
            }
            return;
        }
        // FORM CLOSING EVENT HANDLER
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (globals.datafilechanged && globals.datafilechanged)
            {
                if (dt.QueryDialog(this,"Save Timeline Data before Closing?","Datafile Changed"))
                {
                    SaveDataFile();
                }
            }
        }
        // SEARCH BUTTON CLICK HANDLER
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (this.SearchReference != null) // 11-05-2016 prevent multiple instances
            {
                this.SearchReference.Close();
                this.SearchReference = null;
            }
            Form1.Rep handler = ReplaceDelegateMethod;
            Form1.Down dhandler = ScrollDownMethod;
            ReplaceForm rf = new ReplaceForm(handler, dhandler);
            this.SearchReference = rf;  // CREATE REFERENCE FOR CLOSURE
            rf.Show();
        }
    }
  }
           
   










