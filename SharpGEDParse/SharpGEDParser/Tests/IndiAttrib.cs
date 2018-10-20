using NUnit.Framework;
using SharpGEDParser.Model;

// TODO test NOTE
// TODO test NOTE details
// TODO test SOUR
// TODO test SOUR details
// TODO test OBJE
// TODO test OBJE details
// TODO test multiple, mixed attributes

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiAttrib : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        private IndiRecord TestAttrib1(string tag)
        {
            string indi = string.Format("0 INDI\n1 {0} attrib_value\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual(tag, rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value", rec.Attribs[0].Descriptor);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);

            return rec;
        }

        [Test]
        public void TestCAST()
        {
            string tag = "CAST";
            TestAttrib1(tag);
        }
        [Test]
        public void TestDSCR()
        {
            string tag = "DSCR";
            TestAttrib1(tag);
        }

        [Test]
        public void LongDSCR()
        {
            string indi = "0 @I1@ INDI\n1 DSCR attrib_value\n2 CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual("DSCR", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_valuea big man\nI don't know the\nsecret handshake", rec.Attribs[0].Descriptor);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);
        }

        [Test]
        public void LongDSCR2()
        {
            string indi = "0 @I1@ INDI\n1 DSCR attrib_value \n2 CONC a big man \n2 CONT I don't know the \n2 CONC secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual("DSCR", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value a big man \nI don't know the secret handshake", rec.Attribs[0].Descriptor);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);
        }

        [Test]
        public void ErrorCont()
        {
            // CONC/CONT was invalid for any but DSCR
            string indi = "0 @I1@ INDI\n1 EDUC attrib_value\n2 CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Attribs.Count);
            // 20180106 Allow CONC/CONT for any event/attribute
            Assert.AreEqual("attrib_valuea big man\nI don't know the\nsecret handshake", rec.Attribs[0].Descriptor);
            Assert.AreEqual(0, rec.Errors.Count);
        }

        [Test]
        public void TestEDUC()
        {
            string tag = "EDUC";
            TestAttrib1(tag);
        }
        [Test]
        public void TestIDNO()
        {
            string tag = "IDNO";
            TestAttrib1(tag);
        }
        [Test]
        public void TestNATI()
        {
            string tag = "NATI";
            TestAttrib1(tag);
        }
        [Test]
        public void TestNCHI()
        {
            string tag = "NCHI";
            TestAttrib1(tag);
        }
        [Test]
        public void TestNMR()
        {
            string tag = "NMR";
            TestAttrib1(tag);
        }
        [Test]
        public void TestOCCU()
        {
            string tag = "OCCU";
            TestAttrib1(tag);
        }
        [Test]
        public void TestPROP()
        {
            string tag = "PROP";
            TestAttrib1(tag);
        }
        [Test]
        public void TestRELI()
        {
            string tag = "RELI";
            TestAttrib1(tag);
        }
        [Test]
        public void TestSSN()
        {
            string tag = "SSN";
            TestAttrib1(tag);
        }
        [Test]
        public void TestTITL()
        {
            string tag = "TITL";
            TestAttrib1(tag);
        }
        [Test]
        public void TestFACT()
        {
            string tag = "FACT";
            TestAttrib1(tag);
        }

        [Test]
        public void AttribObjeEmbed()
        {
            // multimedia reference off an attribute
            string indi = "0 @I1@ INDI\n1 FACT attrib_value\n2 DATE 1774\n2 OBJE gibber\n3 FILE refn\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual(1, rec.Attribs.Count);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Attribs[0].OtherLines.Count);  // From mutation testing: verify sub-record parsing

            Assert.AreEqual("FACT", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value", rec.Attribs[0].Descriptor);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);

            Assert.AreEqual(1, rec.Attribs[0].Media.Count);
            Assert.AreEqual(1, rec.Attribs[0].Media[0].Files.Count);
            Assert.AreEqual("refn", rec.Attribs[0].Media[0].Files[0].FileRefn);
            Assert.IsEmpty(rec.Attribs[0].Media[0].Xref);
        }

        [Test]
        public void AttribObjeXref()
        {
            // multimedia reference off an attribute
            string indi = "0 @I1@ INDI\n1 FACT attrib_value\n2 DATE 1774\n2 OBJE @O1@\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual(1, rec.Attribs.Count);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Attribs[0].OtherLines.Count);  // From mutation testing: verify sub-record parsing

            Assert.AreEqual("FACT", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value", rec.Attribs[0].Descriptor);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);

            Assert.AreEqual(1, rec.Attribs[0].Media.Count);
            Assert.AreEqual("O1", rec.Attribs[0].Media[0].Xref);
            Assert.AreEqual(0, rec.Attribs[0].Media[0].Files.Count);
        }

    }
}

