using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Model;

namespace UnitTestProject1
{
    public class GedParseTest
    {
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
        }

        // For those tests which need to verify errors at the topmost level
        public static FileRead ReadItHigher(string testString)
        {
            // TODO as implemented, trailing newline in original string will cause an "empty line" error record to be generated
            FileRead fr = new FileRead();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                fr.ReadLines(stream);
            }
            return fr;
        }

        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        public T parse<T>(string testString, string tagN) where T: class
        {
            var res = ReadIt(testString);
            Assert.AreEqual(1, res.Count, "record count");
//            Assert.AreEqual(tagN, res[0].Tag, "Tag:"+tagN);
            Assert.IsNotNull(res[0]);
            var rec = res[0] as T;
            Assert.AreNotEqual(null, rec, "wrong record type:"+tagN);
            return rec;
        }
    }
}
