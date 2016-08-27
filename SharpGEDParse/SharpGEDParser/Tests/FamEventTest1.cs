using NUnit.Framework;
using SharpGEDParser.Model;
using System.Collections.Generic;
using System.Linq;

// TODO generic address testing?

// TODO "EVEN" special
// TODO "Y" trailing for "MARR"

// TODO trailing "Y" for 5.5
// TODO comprehensive Source Citation

// TODO FAM.AGE for 5.5; what else different 5.5 vs 5.5.1?

// TODO FAM.MARR.OBJE et al

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

        // TODO trailing 'Y' for MARR

        [Test]
        public void EventSourErr()
        {
            // TODO don't have a container for errors when parsing structure+SOUR
            EventSimpleSourErr("MARR");
        }

        public FamRecord TestEventTag(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0}\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date);
            //            Assert.AreEqual(null, rec.FamEvents[0].Age);
            Assert.AreEqual(null, rec.FamEvents[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place);
            return rec;
        }

        public FamRecord TestEventTag2(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0} Y\n2 DATE 1774\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            //            Assert.AreEqual("Y", rec.FamEvents[0].Detail); // TODO 'MARR' specific? NOTE also occurs with ENGA in "ged complete"
            Assert.AreEqual("1774", rec.FamEvents[0].Date);
            Assert.AreEqual("suspicious", rec.FamEvents[0].Type);
            Assert.AreEqual(null, rec.FamEvents[0].Place);
            Assert.AreEqual("church", rec.FamEvents[0].Agency);
            Assert.AreEqual("pregnancy", rec.FamEvents[0].Cause);
            Assert.AreEqual("atheist", rec.FamEvents[0].Religion);
            Assert.AreEqual("locked", rec.FamEvents[0].Restriction);
            return rec;
        }

        public FamRecord TestEventAge(string tag, string spouse)
        {
            string fam = string.Format("0 FAM\n1 {0}\n2 {1}\n3 AGE 42\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 99", tag, spouse);
            var rec = parse(fam);
            Assert.AreEqual(1, rec.FamEvents.Count);
            var famEvent = rec.FamEvents[0];
            Assert.AreEqual(tag, famEvent.Tag);
            Assert.AreEqual(null, famEvent.Date);
            // TODO            Assert.AreEqual("99", famEvent.Age);
            Assert.AreEqual(null, famEvent.Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", famEvent.Place);
            if (spouse == "HUSB")
            {
                Assert.AreEqual("42", famEvent.HusbDetail.Age);
                Assert.AreEqual(null, famEvent.WifeDetail);
            }
            else
            {
                Assert.AreEqual(null, famEvent.HusbDetail);
                Assert.AreEqual("42", famEvent.WifeDetail.Age);
            }
            return rec;
        }

        public FamRecord TestEventNote(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0}\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);

            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date);
            // TODO            Assert.AreEqual(null, rec.FamEvents[0].Age);
            Assert.AreEqual(null, rec.FamEvents[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place);
            Assert.AreEqual(1, rec.FamEvents[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.FamEvents[0].Notes[0].Text);
            return rec;
        }


        private string[] AllEventTags =
        {
            "ANUL", "CENS", "DIV",  "DIVF", "ENGA", 
            "EVEN", "MARB", "MARC", "MARR", "MARL", 
            "MARS", "RESI"
        };

        [Test]
        public void AllEventTag()
        {
            foreach (var eventTag in AllEventTags)
            {
                TestEventTag(eventTag);
            }
        }
        [Test]
        public void AllEventTag2()
        {
            foreach (var eventTag in AllEventTags)
            {
                TestEventTag2(eventTag);
            }
        }
        [Test]
        public void AllEventAge()
        {
            foreach (var eventTag in AllEventTags)
            {
                foreach (string spouse in new [] {"HUSB", "WIFE"})
                {
                    TestEventAge(eventTag, spouse);
                }
            }
        }

        [Test]
        public void AllEventNote()
        {
            foreach (var eventTag in AllEventTags)
            {
                TestEventNote(eventTag);
            }
        }

        [Test]
        public void AllEventAddr()
        {
            foreach (var eventTag in AllEventTags)
            {
                EventAddr(eventTag);
                EventLongAddr(eventTag);
            }
        }

        [Test]
        public void AllEventSour()
        {
            foreach (var eventTag in AllEventTags)
            {
                EventSimpleSour(eventTag);
                EventMultSour(eventTag);
            }
        }

        [Test]
        public void TestSpouseDetail()
        {
            string indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 HUSB blah blah\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].HusbDetail.Detail);
            Assert.AreEqual(null, rec.FamEvents[0].HusbDetail.Age);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 WIFE blah blah\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].WifeDetail.Detail);
            Assert.AreEqual(null, rec.FamEvents[0].WifeDetail.Age);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 HUSB blah blah\n3 AGE 87\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].HusbDetail.Detail);
            Assert.AreEqual("87", rec.FamEvents[0].HusbDetail.Age);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 WIFE bloh bloh\n3 AGE 23\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("bloh bloh", rec.FamEvents[0].WifeDetail.Detail);
            Assert.AreEqual("23", rec.FamEvents[0].WifeDetail.Age);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked\n2 WIFE bloh bloh\n3 AGE 23";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("bloh bloh", rec.FamEvents[0].WifeDetail.Detail);
            Assert.AreEqual("23", rec.FamEvents[0].WifeDetail.Age);

        }

        [Test]
        public void TestAddr()
        {
            string fam = "0 @F33877@ FAM\n1 HUSB @I97095@\n1 WIFE @I97096@\n1 CHIL @I97091@\n1 MARR\n2 DATE 5 JUN 830\n2 PLAC Constantinople, Byzantium\n2 ADDR Hagia Sophia\n2 NOTE Even in the 9th century the church was almost 300 years old, built on t\n3 CONC he ruins of two earlier buildings and known as the Μεγάλη Ἐκκλησία (Gre\n3 CONC at Church).            ";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.IsNotNull(rec.FamEvents[0].Address); // TODO verify details
        }

    }
}
