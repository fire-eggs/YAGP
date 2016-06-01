using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

// TODO consider outputting in human-readable format. See http://www.limilabs.com/blog/json-net-formatter

namespace DrawAnce
{
    public class DASettings : AppSettings<DASettings>
    {
        public bool Fake = true;
        public int WinLeft = -1;
        public int WinTop = -1;
        public int WinHigh = -1;
        public int WinWide = -1;
        public string LastPath = null;
        public List<string> PathHistory = null;
    }

    public class AppSettings<T> where T : new()
    {
        private const string DEFAULT_FILENAME = "DrawAnce_settings.jsn";

        public void Save(string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
        }

        public static void Save(T pSettings, string fileName = DEFAULT_FILENAME)
        {
            File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
        }

        public static T Load(string fileName = DEFAULT_FILENAME)
        {
            T t = new T();
            if(File.Exists(fileName))
                t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
            return t;
        }
    }
}