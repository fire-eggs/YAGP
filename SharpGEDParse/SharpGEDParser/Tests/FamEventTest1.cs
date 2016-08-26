using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class FamEventTest1 : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        private FamRecord parse(string val)
        {
            var res = ReadIt(val);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as FamRecord;
            Assert.IsNotNull(rec);
            return rec;
        }

        public FamRecord EventAddr(string tag)
        {
            string val = string.Format("0 FAM\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(val);
            Assert.AreEqual(1, rec.FamEvents.Count, tag);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date, tag);
            // TODO            Assert.AreEqual(null, rec.FamEvents[0].Age, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Type, tag);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place, tag);
            Assert.AreEqual("Calle de Milaneses 6, tienda", rec.FamEvents[0].Address.Adr, tag);
            Assert.AreEqual(1, rec.FamEvents[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.FamEvents[0].Notes[0].Text);
            return rec;
        }

        // TODO generic address testing?
        public FamRecord EventLongAddr(string tag)
        {
            // This is an unlikely set of tags, but required by standard
            string val = string.Format("0 FAM\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n" +
                "3 CONT Address continue\n3 CITY Nowhere\n3 STAE ZZ\n3 POST 1GN 2YV\n3 CTRY Where\n2 PHON 1-800-555-1212\n" +
            "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(val);
            Assert.AreEqual(1, rec.FamEvents.Count, tag);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date, tag);

            Address addr = rec.FamEvents[0].Address;
            Assert.AreEqual("Calle de Milaneses 6, tienda\nAddress continue", addr.Adr, tag);
            Assert.AreEqual("Nowhere", addr.City, tag);
            Assert.AreEqual("ZZ", addr.Stae, tag);
            Assert.AreEqual("1GN 2YV", addr.Post, tag);
            Assert.AreEqual("Where", addr.Ctry, tag);
            Assert.AreEqual("1-800-555-1212", addr.Phon[0], tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place, tag);
            Assert.AreEqual(1, rec.FamEvents[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.FamEvents[0].Notes[0].Text);
            return rec;
        }

        [Test]
        public void AddrMarr()
        {
            EventAddr("MARR");
            EventLongAddr("MARR");
        }
    }
}
