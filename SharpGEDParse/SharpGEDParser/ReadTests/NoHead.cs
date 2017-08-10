using NUnit.Framework;

// No 0 HEAD - no read

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class NoHead : TestUtil
    {
        private readonly string[] lines0 =
        {
            "garbage",
            "1 SOUR 0",
            "0 TRLR",
        };

        [Test]
        public void NoHeadLFBom()
        {
            GedReader r = BuildAndRead(lines0, LB.LF, true);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.LineCount);
        }

        [Test]
        public void NoHeadLFNoBom()
        {
            GedReader r = BuildAndRead(lines0, LB.LF, false);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.LineCount);
        }

        [Test]
        public void NoHeadCRLFBom()
        {
            GedReader r = BuildAndRead(lines0, LB.CRLF, true);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.LineCount);
        }

        [Test]
        public void NoHeadCRLFNoBom()
        {
            GedReader r = BuildAndRead(lines0, LB.CRLF, false);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.LineCount);
        }

    }
}
