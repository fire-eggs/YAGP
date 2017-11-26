using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser;

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

        public static string ParseAndWrite(string testString, bool noHead = true)
        {
            FileRead fr = ReadItHigher(testString);
            MemoryStream mem = new MemoryStream();
            FileWrite.WriteRecs(mem, fr.Data, noHead);
            return Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
        }
    }
}
