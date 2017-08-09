using System;
using System.IO;
using System.Text;

// ReSharper disable InconsistentNaming

// Helper class when exercising the physical reading of files

namespace SharpGEDParser.Tests
{
    public class ReadingUtil
    {
        public FileRead BuildAndRead(string[] lines, GedReader.LB term, bool bom, bool trailTerm=true)
        {
            StringBuilder sb = new StringBuilder();
            int len = !trailTerm ? lines.Length - 1 : lines.Length;
            for (int i = 0; i < len; i++)
            {
                sb.Append(lines[i]);
                switch (term)
                {
                    case GedReader.LB.MAC:
                        sb.Append('\r');
                        break;
                    case GedReader.LB.UNIX:
                        sb.Append('\n');
                        break;
                    case GedReader.LB.DOS:
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

        public FileRead ReadFile(string contents, bool bom = false)
        {
            string path = MakeFile(contents, bom);
            FileRead fr = new FileRead();
            fr.ReadGed(path);
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
            }
            return fr;
        }
    }
}
