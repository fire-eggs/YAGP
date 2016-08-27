using NUnit.Framework;
using SharpGEDParser.Model;
using System.Collections.Generic;
using System.Linq;

// TODO generic address testing?


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
            string val =
                string.Format(
                    "0 FAM\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                    tag);
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

        public FamRecord EventLongAddr(string tag)
        {
            // This is an unlikely set of tags, but required by standard
            string val = string.Format("0 FAM\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n" +
                                       "3 CONT Address continue\n3 CITY Nowhere\n3 STAE ZZ\n3 POST 1GN 2YV\n3 CTRY Where\n2 PHON 1-800-555-1212\n" +
                                       "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                tag);
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

        public void EventSimpleSour(string tag)
        {
            string txt = string.Format("0 @F1@ FAM\n1 {0}\n2 SOUR @S1@" +
                                       "\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                tag);
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident, tag);
            Assert.AreEqual(1, rec.FamEvents.Count, tag);

            Assert.AreEqual(0, rec.Errors.Count, "No error " + tag);

            var evt = rec.FamEvents[0];
            Assert.AreEqual(tag, rec.FamEvents[0].Tag, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date, tag);

            Assert.AreEqual(1, evt.Cits.Count, tag);
            Assert.AreEqual("S1", evt.Cits[0].Xref, tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", evt.Place, tag);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", evt.Notes[0].Text);

        }

        public void EventMultSour(string tag)
        {
            string txt = string.Format("0 @F1@ FAM\n1 {0}\n" +
                "2 SOUR out of bed\n3 TEXT fumbar ex\n3 CONC tended\n3 QUAY nope\n" +
                "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng\n" +
                "2 SOUR inbed\n3 TEXT foebar \n3 CONC extended\n3 QUAY yup",
                tag);
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident, tag);
            Assert.AreEqual(1, rec.FamEvents.Count, tag);

            Assert.AreEqual(0, rec.Errors.Count, "No error " + tag);

            var evt = rec.FamEvents[0];
            Assert.AreEqual(tag, rec.FamEvents[0].Tag, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date, tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", evt.Place, tag);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", evt.Notes[0].Text);

            Assert.AreEqual("out of bed", evt.Cits[0].Desc);
            Assert.AreEqual(1, evt.Cits[0].Text.Count);
            Assert.AreEqual("fumbar extended", evt.Cits[0].Text[0]);
            Assert.AreEqual("nope", evt.Cits[0].Quay);

            Assert.AreEqual("inbed", evt.Cits[1].Desc);
            Assert.AreEqual(1, evt.Cits[1].Text.Count);
            Assert.AreEqual("foebar extended", evt.Cits[1].Text[0]);
            Assert.AreEqual("yup", evt.Cits[1].Quay);
        }

        public void EventSimpleSourErr(string tag)
        {
            string txt = string.Format("0 @F1@ FAM\n1 {0}\n2 SOUR @S1@\n3 TEXT blah\n" +
                                       "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                tag);
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident, tag);
            Assert.AreEqual(1, rec.FamEvents.Count, tag);

            Assert.AreEqual(1, rec.Errors.Count, "Error expected " + tag);

            var evt = rec.FamEvents[0];
            Assert.AreEqual(tag, rec.FamEvents[0].Tag, tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date, tag);

            Assert.AreEqual(1, evt.Cits.Count, tag);
            Assert.AreEqual("S1", evt.Cits[0].Xref, tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", evt.Place, tag);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", evt.Notes[0].Text);

        }

        [Test]
        public void MarrSour()
        {
            EventSimpleSour("MARR");
            EventMultSour("MARR");
            EventSimpleSourErr("MARR");
        }
    }
}
