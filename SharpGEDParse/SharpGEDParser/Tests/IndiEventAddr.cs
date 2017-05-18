using NUnit.Framework;
using SharpGEDParser.Model;

// Crashes during changes showed INDI.<event>.ADDR and related were not unit tested

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// TODO consider merging INDI.Events and INDI.Attribs in the model?
// TODO pass and verify Attrib text?

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiEventAddr : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        private IndiRecord TestEventAddr(string tag)
        {
            var indi = string.Format("0 INDI\n1 {0}\n2 ADDR This is a test", tag);
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("This is a test", rec.Events[0].Address.Adr);
            return rec;
        }

        [Test]
        public void TestAddr()
        {
            TestEventAddr("BIRT");
        }

        private IndiRecord TestEventPhon(string tag)
        {
            var indi = string.Format("0 INDI\n1 {0}\n2 PHON This is a test", tag);
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual(1, rec.Events[0].Address.Phon.Count);
            Assert.AreEqual("This is a test", rec.Events[0].Address.Phon[0]);
            return rec;
        }

        [Test]
        public void TestPhon()
        {
            TestEventPhon("BIRT");
        }

        public IndiRecord EventAddr(string tag)
        {
            var val = string.Format(
                    "0 INDI\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                    tag);
            var rec = parse(val);

            Assert.AreEqual(1, rec.Events.Count, tag);
            Assert.AreEqual(tag, rec.Events[0].Tag, tag);
            Assert.AreEqual(null, rec.Events[0].Date, tag);
            Assert.AreEqual(null, rec.Events[0].Age, tag);
            Assert.AreEqual(null, rec.Events[0].Type, tag);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place, tag);
            Assert.AreEqual("Calle de Milaneses 6, tienda", rec.Events[0].Address.Adr, tag);
            Assert.AreEqual(1, rec.Events[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.Events[0].Notes[0].Text);
            return rec;
        }


        private readonly string[] AllEventTags =
        {
            "BIRT", "DEAT", "CHR",  "CREM", "BURI", 
            "ADOP", "BAPM", "BARM", "BASM", "BLES",
            "CHRA", "CONF", "FCOM", "ORDN", "NATU",
            "EMIG", "IMMI", "CENS", "PROB", "WILL",
            "GRAD", "RETI", "EVEN"
        };

        private readonly string[] AllAttribTags =
        {
            "CAST", "DSCR", "EDUC", "IDNO", "NATI",
            "NCHI", "NMR", "OCCU", "PROP", "RELI",
            "RESI", "SSN", "TITL"
        };

        [Test]
        public void AllEventAddr()
        {
            foreach (var eventTag in AllEventTags)
            {
                EventAddr(eventTag);
            }
        }

        public IndiRecord AttribAddr(string tag, string extra)
        {
            var val = string.Format(
                    "0 INDI\n1 {0} {1}\n2 ADDR Calle de Milaneses 6, tienda\n2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                    tag, extra);
            var rec = parse(val);

            Assert.AreEqual(1, rec.Attribs.Count, tag);
            Assert.AreEqual(tag, rec.Attribs[0].Tag, tag);
            Assert.AreEqual(null, rec.Attribs[0].Date, tag);
            Assert.AreEqual(null, rec.Attribs[0].Age, tag);
            Assert.AreEqual(null, rec.Attribs[0].Type, tag);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place, tag);
            Assert.AreEqual("Calle de Milaneses 6, tienda", rec.Attribs[0].Address.Adr, tag);
            Assert.AreEqual(1, rec.Attribs[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.Attribs[0].Notes[0].Text);
            return rec;
        }

        [Test]
        public void AllAttribAddr()
        {
            foreach (var eventTag in AllAttribTags)
            {
                AttribAddr(eventTag, "");
            }
        }

        public IndiRecord EventLongAddr(string tag)
        {
            // This is an unlikely set of tags, but required by standard
            string val = string.Format("0 INDI\n1 {0}\n2 ADDR Calle de Milaneses 6, tienda\n" +
                                       "3 CONT Address continue\n3 CITY Nowhere\n3 STAE ZZ\n3 POST 1GN 2YV\n3 CTRY Where\n2 PHON 1-800-555-1212\n" +
                                       "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                tag);
            var rec = parse(val);
            Assert.AreEqual(1, rec.Events.Count, tag);
            Assert.AreEqual(tag, rec.Events[0].Tag, tag);
            Assert.AreEqual(null, rec.Events[0].Date, tag);

            Address addr = rec.Events[0].Address;
            Assert.AreEqual("Calle de Milaneses 6, tienda\nAddress continue", addr.Adr, tag);
            Assert.AreEqual("Nowhere", addr.City, tag);
            Assert.AreEqual("ZZ", addr.Stae, tag);
            Assert.AreEqual("1GN 2YV", addr.Post, tag);
            Assert.AreEqual("Where", addr.Ctry, tag);
            Assert.AreEqual("1-800-555-1212", addr.Phon[0], tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place, tag);
            Assert.AreEqual(1, rec.Events[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.Events[0].Notes[0].Text);
            return rec;
        }

        [Test]
        public void AllEventAddr2()
        {
            foreach (var eventTag in AllEventTags)
            {
                EventLongAddr(eventTag);
            }
        }

        public IndiRecord AttribLongAddr(string tag, string extra)
        {
            // This is an unlikely set of tags, but required by standard
            string val = string.Format("0 INDI\n1 {0} {1}\n2 ADDR Calle de Milaneses 6, tienda\n" +
                                       "3 CONT Address continue\n3 CITY Nowhere\n3 STAE ZZ\n3 POST 1GN 2YV\n3 CTRY Where\n2 PHON 1-800-555-1212\n" +
                                       "2 NOTE Blah blah this is a note con\n3 CONC tinued on a second line.\n2 PLAC Sands, Oldham, Lncshr, Eng",
                tag, extra);
            var rec = parse(val);
            Assert.AreEqual(1, rec.Attribs.Count, tag);
            Assert.AreEqual(tag, rec.Attribs[0].Tag, tag);
            Assert.AreEqual(null, rec.Attribs[0].Date, tag);

            Address addr = rec.Attribs[0].Address;
            Assert.AreEqual("Calle de Milaneses 6, tienda\nAddress continue", addr.Adr, tag);
            Assert.AreEqual("Nowhere", addr.City, tag);
            Assert.AreEqual("ZZ", addr.Stae, tag);
            Assert.AreEqual("1GN 2YV", addr.Post, tag);
            Assert.AreEqual("Where", addr.Ctry, tag);
            Assert.AreEqual("1-800-555-1212", addr.Phon[0], tag);

            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place, tag);
            Assert.AreEqual(1, rec.Attribs[0].Notes.Count);
            Assert.AreEqual("Blah blah this is a note continued on a second line.", rec.Attribs[0].Notes[0].Text);
            return rec;
        }

        [Test]
        public void AllAttribAddr2()
        {
            foreach (var eventTag in AllAttribTags)
            {
                AttribLongAddr(eventTag, "");
            }
        }

    }
}
