using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("timeline")]
[assembly: AssemblyDescription("Family Tree Mapping Utility")]
[assembly: AssemblyConfiguration(" Version 2.0.0.1")]
[assembly: AssemblyCompany("HC Williams MD")]
[assembly: AssemblyProduct("timeline")]
[assembly: AssemblyCopyright("Copyright © April 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c1e33791-47c9-40ea-8ecd-aca6bb2804c5")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
// 03-28-2015: basic geocode of file entries working, ?change parser to json object serializer
// 03-29-2015: Added Save, Load file; fixed parsing, date correction and sort-by-date
// 03-31-2015: Javascript generator working
//             Current issues: allow selection of event types to include eg. births only
//             duplicate births, dates, etc are a problem; consider a database of people with one
//             birth, death, marriage etc. eg sort the events by person, remove duplicates
// 04-03-2015: Added API key test, option to remove entries with incomplete dates
// 04-05-2015: Added - html output file creation
// 04-06-2015: Added InfoBox support to WebApp
//             ToDo = api key must be added during HTML construction (now its hard coded)
//             Drag and Drop for browser path tb in settings; file browse for mainform file tb
//             Save cfg file for API key, settings.
//             Add program version and source file info to html during construction
// 04-08-2015: Added apikey insertion, drag drop for web browser selection, additional options
// 04-09-2015: Multiple improvements, added autofix feature
// 04-13-2015: Edit Record function added
// 04-20-2015: Added longetivity report.
// 05-14-2015: Added Save/Load user preferences
// 06-05-2015: Added Help Form
// 06-07-2015: Fixed WebApp
// 08-12-2015: Added Heatmap, custom icons to Webapp
// 11-30-2015: New GED Parser created
// 12-29-2015: Added check for missing API key, updated help file
// 08-08-2016: Fixed Jscript so webapp uses default icon if person.png is missing
// 09-11-2016: Added record number to name string as unique individual identifier
//             Avoid counting individuals with the same name as a duplicate
// 09-15-2016: Modified Name cleanup to remove non-standard chars in names; Added NormalizeString() to
//             remove diacriticals from addresses before creating javascript, results in some ?s in addresses
// 09-19-2016: Added ability to print main window contents
// 03-27-2017: Added context menu function for list of individuals
// 04-10-2017: Improved Web Page
// 04-15-2017: Allow event editing from Individuals List
// 04-25-2017: Update Web Page HTML / jscript, Add Search Window
// 04-30-2017: Fixed Parse errors related to GEDCOMs downloaded from Ancestry.com due to trailing spaces

[assembly: AssemblyVersion("2.0.0.1")]
[assembly: AssemblyFileVersion("2.0.0.1")]
