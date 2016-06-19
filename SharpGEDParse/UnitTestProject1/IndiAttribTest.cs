using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [TestClass]
    public class IndiAttribTest : GedParseTest
    {
        // TODO test NOTE
        // TODO test NOTE details
        // TODO test SOUR
        // TODO test SOUR details
        // TODO test CHAN
        // TODO test CHAN details
        // TODO test multiple, mixed attributes

        private KBRGedIndi parse(string val)
        {
            return parse<KBRGedIndi>(val, "INDI");
        }

        private KBRGedIndi TestAttrib1(string tag)
        {
            string indi = string.Format("0 INDI\n1 {0} attrib_value\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious", tag);
            KBRGedIndi rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual(tag, rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value", rec.Attribs[0].Detail);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);

            return rec;
        }

        [TestMethod]
        public void TestCAST()
        {
            string tag = "CAST";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestDSCR()
        {
            string tag = "DSCR";
            TestAttrib1(tag);
        }

        [TestMethod]
        public void LongDSCR()
        {
            string indi = "0 INDI\n1 DSCR attrib_value\n2 CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            KBRGedIndi rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual("DSCR", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_valuea big man\nI don't know the\nsecret handshake", rec.Attribs[0].Detail);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);
        }

        [TestMethod]
        public void LongDSCR2()
        {
            string indi = "0 INDI\n1 DSCR attrib_value \n2 CONC a big man \n2 CONT I don't know the \n2 CONC secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            KBRGedIndi rec = parse(indi);

            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreEqual("DSCR", rec.Attribs[0].Tag);
            Assert.AreEqual("attrib_value a big man \nI don't know the secret handshake", rec.Attribs[0].Detail);
            Assert.AreEqual("17", rec.Attribs[0].Age);
            Assert.AreEqual("1774", rec.Attribs[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Attribs[0].Place);
            Assert.AreEqual("suspicious", rec.Attribs[0].Type);
        }

        [TestMethod]
        public void ErrorCont()
        {
            // NOTE: CONC/CONT invalid for any but DSCR
            string indi = "0 INDI\n1 EDUC attrib_value\n2 CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            KBRGedIndi rec = parse(indi);
            Assert.AreEqual(1, rec.Attribs.Count);
            Assert.AreNotEqual(0, rec.Attribs[0].Errors.Count);
        }

        [TestMethod]
        public void TestEDUC()
        {
            string tag = "EDUC";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestIDNO()
        {
            string tag = "IDNO";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestNATI()
        {
            string tag = "NATI";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestNCHI()
        {
            string tag = "NCHI";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestNMR()
        {
            string tag = "NMR";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestOCCU()
        {
            string tag = "OCCU";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestPROP()
        {
            string tag = "PROP";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestRELI()
        {
            string tag = "RELI";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestSSN()
        {
            string tag = "SSN";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestTITL()
        {
            string tag = "TITL";
            TestAttrib1(tag);
        }
        [TestMethod]
        public void TestFACT()
        {
            string tag = "FACT";
            TestAttrib1(tag);
        }
    }
}
