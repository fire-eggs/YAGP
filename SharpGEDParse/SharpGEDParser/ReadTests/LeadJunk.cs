using NUnit.Framework;

// Leading junk before 0 HEAD
// including line terminator variants

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class LeadJunk : TestUtil
    {
        // lines with junk with no LF
        private readonly string[] junkNoLF =
        {
            "gibberish junk and so on0 HEAD",
            "1 CHAR ASCII",
            "1 SOUR 0",
            "0 TRLR",
        };

        // lines with junk with LF
        private readonly string[] junkLF =
        {
            "gibberish junk and so on\n0 HEAD",
            "1 CHAR ASCII",
            "1 SOUR 0",
            "0 TRLR",
        };

        // lines with junk with CRLF
        private readonly string[] junkCRLF =
        {
            "gibberish junk and so on\r\n0 HEAD",
            "1 CHAR ASCII",
            "1 SOUR 0",
            "0 TRLR",
        };

        [Test]
        public void JunkLF1NoBom()
        {
            GedReader r = BuildAndRead(junkNoLF, LB.LF, false);
            Assert.AreEqual(junkNoLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkLF2NoBom()
        {
            GedReader r = BuildAndRead(junkLF, LB.LF, false);
            Assert.AreEqual(junkLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkLF3NoBom()
        {
            GedReader r = BuildAndRead(junkCRLF, LB.LF, false);
            Assert.AreEqual(junkCRLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS1NoBom()
        {
            GedReader r = BuildAndRead(junkNoLF, LB.CRLF, false);
            Assert.AreEqual(junkNoLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS2NoBom()
        {
            GedReader r = BuildAndRead(junkLF, LB.CRLF, false);
            Assert.AreEqual(junkLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS3NoBom()
        {
            GedReader r = BuildAndRead(junkCRLF, LB.CRLF, false);
            Assert.AreEqual(junkCRLF.Length, r.LineCount);
            Assert.AreEqual(0, r.Errors.Count);
        }
    }
}
