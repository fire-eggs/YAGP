using System.Text;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

// Tests where the final line has no terminator

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class SmallNoFinalTerm : TestUtil
    {
        // Tiny test
        private readonly string[] lines0 =
        {
            "0 HEAD",
            "1 SOUR 0",
            "0 TRLR",
        };

        [Test]
        public void TinyLFNoBom()
        {
            GedReader r = BuildAndRead(lines0, LB.LF, false, false);
            Assert.AreEqual(3, r.LineCount);
            Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyDOSNoBom()
        {
            GedReader r = BuildAndRead(lines0, LB.CRLF, false, false);
            Assert.AreEqual(3, r.LineCount);
            Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyLFBom()
        {
            GedReader r = BuildAndRead(lines0, LB.LF, true, false);
            Assert.AreEqual(3, r.LineCount);
            Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyDOSBom()
        {
            GedReader r = BuildAndRead(lines0, LB.CRLF, true, false);
            Assert.AreEqual(3, r.LineCount);
            Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

    }
}
