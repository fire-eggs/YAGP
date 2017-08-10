using NUnit.Framework;

// Exercise spurious LF/CR scenarios

// ReSharper disable InconsistentNaming

namespace GEDReadTest.Tests
{
    [TestFixture]
    class SmallSpurious : TestUtil
    {
        private readonly string[] linesLF =
        {
            "0 HEAD",
            "1 CHAR ASCII",
            "1 SO\nUR 0",
            "0 TRLR",
        };

        private readonly string[] linesCR =
        {
            "0 HEAD",
            "1 CHAR ASCII",
            "1 SO\rUR 0",
            "0 TRLR",
        };

        [Test]
        public void LFinDOS()
        {
            var r = BuildAndRead(linesLF, LB.CRLF, false, true);
            Assert.AreEqual(linesLF.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);
            Assert.AreEqual(0, r.Errors.Count);
        }

        [Test]
        public void CRinUnix()
        {
            var r = BuildAndRead(linesCR, LB.LF, false, true);
            Assert.AreEqual(linesCR.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);
            Assert.AreEqual(0, r.Errors.Count);
        }

        [Test]
        public void LFinDOSBom()
        {
            var r = BuildAndRead(linesLF, LB.CRLF, true, true);
            Assert.AreEqual(linesLF.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);

            // Error: BOM / Head.Char mismatch
            Assert.AreEqual(1, r.Errors.Count); // TODO eliminate
        }

        [Test]
        public void CRinUnixBom()
        {
            var r = BuildAndRead(linesCR, LB.LF, true, true);
            Assert.AreEqual(linesCR.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);

            // Error: BOM / Head.Char mismatch
            Assert.AreEqual(1, r.Errors.Count); // TODO eliminate
        }

        [Test]
        public void LFinDOSNoEnd()
        {
            var r = BuildAndRead(linesLF, LB.CRLF, false, false);
            Assert.AreEqual(linesLF.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);
            Assert.AreEqual("0 TRLR", r.Lines[3]);
            Assert.AreEqual(0, r.Errors.Count);
        }

        [Test]
        public void CRinUnixNoEnd()
        {
            var r = BuildAndRead(linesCR, LB.LF, false, false);
            Assert.AreEqual(linesCR.Length, r.LineCount);
            Assert.AreEqual("1 SOUR 0", r.Lines[2]);
            Assert.AreEqual("0 TRLR", r.Lines[3]);
            Assert.AreEqual(0, r.Errors.Count);
        }

    }
}
