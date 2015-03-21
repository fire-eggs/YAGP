using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    public class GedParseTest
    {
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
        }

        public static List<KBRGedRec> ReadIt(string testString)
        {
            FileRead fr = new FileRead();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                fr.ReadLines(stream);
            }
            return fr.Data;
        }

        public KBRGedIndi parse(string testString)
        {
            if (!testString.EndsWith("0 KLUDGE"))
                testString += "\n0 KLUDGE";
            var res = ReadIt(testString);
            Assert.AreEqual(1, res.Count, "record count");
            Assert.AreEqual("INDI", res[0].Tag, "Indi Tag");
            var rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(null, rec, "not INDI record");
            return rec;
        }
    }
}
