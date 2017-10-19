using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace timeline
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // EMBEDDED DLL HANDLER tested OK 01-15-2014
            // Must run in Program Class (where exception occurs
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.Run(new Form1());
        }
        // EMBEDDED DLL LOADER 
        // VERSION 2.0 01-15-2014 derives resourcename from args and application namespace
        // assumes resource is a DLL
        // this should load any missing DLL that is properly embedded
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string appname = Application.ProductName + "."; // gets Application Namespace
            string[] dll = args.Name.ToString().Split(','); // separates args.Name string
            string resourcename = appname + dll[0] + ".dll"; // element [0] contains the missing resource name
            Assembly MyAssembly = Assembly.GetExecutingAssembly();
            Stream AssemblyStream = MyAssembly.GetManifestResourceStream(resourcename);
            byte[] raw = new byte[AssemblyStream.Length];
            AssemblyStream.Read(raw, 0, raw.Length);
            return Assembly.Load(raw);

        }
        
        
    }
    // DELEGATES

    public delegate void GetSet();

    // GLOBAL VARIABLES
    [Serializable]
    static class globals
    {
       //public static string GeoCodeAPIKey = "AIzaSyCs9PpLS5UWryENiGP7R5a31yWDNcnPXpU";
       public static string GeoCodeAPIKey = "AIzaSyBEN7j_iF9mjSKOrsI6xNiGH_GmflvW30g"; //06-07-2015
       public static bool datafileloaded = false;
       public static bool datafilechanged = false;
       public static bool datafilesaved = false;
       public static string datafilename = "timelinedatafile.dat";
       public static string webappfilename = "timelinewebpage.html";
       public static bool sourcefileloaded = false;
       public static bool jsincludebirths = true; // include BIRTHS in javascript
       public static bool jsincludedeaths = false; // include DEATHs in javascript
       public static bool jincludemarriages = false; 
       public static bool jsincludebaptisms = false;
       public static bool jsincludeburials = false;
       public static bool jsincluderesidences = false;
       public static bool jsincludechristenings = false;
       public static bool openhtmlfileaftersaving = true;
       public static string defaultbrowser = "C:\\Program Files (x86)\\Mozilla FireFox\\firefox.exe";
       public static bool autoremovebaddates = true;
       public static bool autoremovebadgeocodes = true;
       public static bool autoremoveduplicateentries = true;
       public static bool datafileautofixperformed = false;
       public static string googleapikeyinfowebpage = "https://developers.google.com/maps/documentation/javascript/get-api-key";
       public static bool userpreferencessaved = false;
       public static string preffilename = "timeline.cfg";
       public static string preferencefileheader = "TIMELINE CONFIGURATION FILE VER 1";
       public static string addressfilename = "timelineaddressfile.dat";
       public static string addressfileheader = "TIMELINE ADDRESS FILE VER 1";
       public static bool addressfileloaded = false;
       public static bool addressfilesaved = false;
    }
    
}
