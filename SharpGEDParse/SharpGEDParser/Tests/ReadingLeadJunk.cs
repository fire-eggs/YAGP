using NUnit.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCommaInArrayInitializer

// Exercise junk before "0 HEAD", with line terminator variants

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class ReadingLeadJunk : ReadingUtil
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
            var r = BuildAndRead(junkNoLF, GedReader.LB.UNIX, false);
            Assert.AreEqual(junkNoLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkLF2NoBom()
        {
            var r = BuildAndRead(junkLF, GedReader.LB.UNIX, false);
            Assert.AreEqual(junkLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkLF3NoBom()
        {
            var r = BuildAndRead(junkCRLF, GedReader.LB.UNIX, false);
            Assert.AreEqual(junkCRLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS1NoBom()
        {
            var r = BuildAndRead(junkNoLF, GedReader.LB.DOS, false);
            Assert.AreEqual(junkNoLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS2NoBom()
        {
            var r = BuildAndRead(junkLF, GedReader.LB.DOS, false);
            Assert.AreEqual(junkLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
        [Test]
        public void JunkDOS3NoBom()
        {
            var r = BuildAndRead(junkCRLF, GedReader.LB.DOS, false);
            Assert.AreEqual(junkCRLF.Length, r.NumberLines);
            Assert.AreEqual(0, r.Errors.Count);
        }
    }

}
