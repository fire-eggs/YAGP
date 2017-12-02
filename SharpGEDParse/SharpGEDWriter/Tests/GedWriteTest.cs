using SharpGEDParser;
using System.IO;
using System.Text;

namespace SharpGEDWriter.Tests
{
    class GedWriteTest
    {

        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        public static FileRead ReadItHigher(string testString)
        {
            // TODO as implemented, trailing newline in original string will cause an "empty line" error record to be generated
            FileRead fr = new FileRead();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                fr.ReadGed(null, stream);
            }
            return fr;
        }

        public static string Write(FileRead fr, bool noHead = true, bool unix = true)
        {
            MemoryStream mem = new MemoryStream();
            FileWrite.WriteRecs(mem, fr.Data, noHead, unix);
            return Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
        }

        public static string ParseAndWrite(string testString, bool noHead = true, bool unix=true)
        {
            FileRead fr = ReadItHigher(testString);
            return Write(fr, noHead, unix);
        }

        // Take a list of strings and create a combined,NL terminated string
        public string MakeInput(string[] recs)
        {
            StringBuilder inp = new StringBuilder();
            foreach (var rec in recs)
            {
                inp.Append(rec);
                inp.Append("\n");
            }
            return inp.ToString();
        }

        // Take a list of strings and create a combined,NL terminated string
        // in a specified order
        public string MakeInput(string[] recs, int [] order)
        {
            StringBuilder inp = new StringBuilder();
            foreach (var dex in order)
            {
                inp.Append(recs[dex]);
                inp.Append("\n");
            }
            return inp.ToString();
        }

    }
}
