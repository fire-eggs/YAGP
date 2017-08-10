using NUnit.Framework;

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class SmallValid : TestUtil
    {

        // Smallest valid GEDCOM
        private readonly string[] lines1 =
        {
            "0 HEAD",
            "1 SOUR 0",
            "1 SUBM @U@",
            "1 GEDC",
            "2 VERS 5.5.1",
            "2 FORM LINEAGE-LINKED",
            "1 CHAR ASCII",
            "0 @U@ SUBM",
            "1 NAME X",
            "0 TRLR",
        };

        [Test]
        public void SmallLFNoBom()
        {
            GedReader r = BuildAndRead(lines1, LB.LF, false);
            Assert.AreEqual("UNIX", r.LineBreaks);
            Assert.AreEqual(lines1.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
            Assert.AreEqual(0, r.Spurious.Count);
        }

        [Test]
        public void SmallCRLFNoBom()
        {
            GedReader r = BuildAndRead(lines1, LB.CRLF, false);
            Assert.AreEqual("DOS", r.LineBreaks);
            Assert.AreEqual(lines1.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
            Assert.AreEqual(0, r.Spurious.Count);
        }

        [Test]
        public void SmallCRNoBom()
        {
            GedReader r = BuildAndRead(lines1, LB.CR, false);
            Assert.AreEqual("ERR", r.LineBreaks);
            Assert.AreEqual(1, r.Errors.Count); // TODO validate error string?
        }

        [Test]
        public void SmallLFBom()
        {
            GedReader r = BuildAndRead(lines1, LB.LF, true);
            Assert.AreEqual("UNIX", r.LineBreaks);
            Assert.AreEqual(lines1.Length, r.LineCount);
            Assert.AreEqual(0, r.Spurious.Count);

            // Error: bom/head.char mismatch
            Assert.AreEqual(1, r.Errors.Count);
        }

        [Test]
        public void SmallCRLFBom()
        {
            GedReader r = BuildAndRead(lines1, LB.CRLF, true);
            Assert.AreEqual("DOS", r.LineBreaks);
            Assert.AreEqual(lines1.Length, r.LineCount);
            Assert.AreEqual(0, r.Spurious.Count);

            // Error: bom/head.char mismatch
            Assert.AreEqual(1, r.Errors.Count);
        }

        [Test]
        public void SmallCRBom()
        {
            GedReader r = BuildAndRead(lines1, LB.CR, true);
            Assert.AreEqual("ERR", r.LineBreaks);
            Assert.AreEqual(1, r.Errors.Count); // TODO validate error string?
        }

        // Tiny test
        private readonly string[] lines0 =
        {
            "0 HEAD",
            "1 SOUR 0",
            "0 TRLR",
        };

        [Test]
        public void TinyNoBom()
        {
            GedReader r = BuildAndRead(lines0, LB.LF, false);
            
        }
    }
}
