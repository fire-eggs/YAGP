using System;
using System.IO;
using System.Text;

// ReSharper disable InconsistentNaming

namespace GEDReadTest.Tests
{
    public class TestUtil
    {
        public enum LB
        {
            CR,
            CRLF,
            LF
        };

        public GedReader BuildAndRead(string[] lines, LB term, bool bom, bool trailTerm=true)
        {
            StringBuilder sb = new StringBuilder();
            int len = !trailTerm ? lines.Length - 1 : lines.Length;
            for (int i = 0; i < len; i++)
            {
                sb.Append(lines[i]);
                switch (term)
                {
                    case LB.CR:
                        sb.Append('\r');
                        break;
                    case LB.LF:
                        sb.Append('\n');
                        break;
                    case LB.CRLF:
                        sb.Append('\r');
                        sb.Append('\n');
                        break;
                }
            }
            if (!trailTerm)
                sb.Append(lines[len]);
            return ReadFile(sb.ToString(), bom);
        }

        public string MakeFile(string contents, bool bom = false)
        {
            string path = Path.GetTempFileName();
            TextWriter tw = new StreamWriter(path, false, new UTF8Encoding(bom));
            tw.Write(contents);
            tw.Close();
            return path;
        }

        public GedReader ReadFile(string contents, bool bom = false)
        {
            string path = MakeFile(contents, bom);
            GedReader r = new GedReader();
            r.ReadFile(path);
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
            }
            return r;
        }
    }
}
