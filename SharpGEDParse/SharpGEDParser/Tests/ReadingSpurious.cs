using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

// Exercise various spurious line break scenarios
namespace SharpGEDParser.Tests
{
    [TestFixture]
    class ReadingSpurious : ReadingUtil
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
            var r = BuildAndRead(linesLF, GedReader.LB.DOS, false, true);
            Assert.AreEqual(linesLF.Length, r.NumberLines-1);
            Assert.AreEqual(0, r.Errors.Count);

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

        [Test]
        public void CRinUnix()
        {
            var r = BuildAndRead(linesCR, GedReader.LB.UNIX, false, true);
            Assert.AreEqual(0, r.Errors.Count);
            Assert.AreEqual(linesCR.Length, r.NumberLines - 1);

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

        [Test]
        public void LFinDOSBom()
        {
            var r = BuildAndRead(linesLF, GedReader.LB.DOS, true, true);
            Assert.AreEqual(linesLF.Length, r.NumberLines - 1);

            // Error: BOM / Head.Char mismatch
            Assert.AreEqual(1, r.Errors.Count); // TODO eliminate?

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

        [Test]
        public void CRinUnixBom()
        {
            var r = BuildAndRead(linesCR, GedReader.LB.UNIX, true, true);
            Assert.AreEqual(linesCR.Length, r.NumberLines - 1);

            // Error: BOM / Head.Char mismatch
            Assert.AreEqual(1, r.Errors.Count); // TODO eliminate?

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

        [Test]
        public void LFinDOSNoEnd()
        {
            // No final line terminator
            var r = BuildAndRead(linesLF, GedReader.LB.DOS, false, false);
            Assert.AreEqual(linesLF.Length, r.NumberLines - 1);
            Assert.AreEqual(0, r.Errors.Count);

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

        [Test]
        public void CRinUnixNoEnd()
        {
            // No final line terminator
            var r = BuildAndRead(linesCR, GedReader.LB.UNIX, false, false);
            Assert.AreEqual(linesCR.Length, r.NumberLines - 1);
            Assert.AreEqual(0, r.Errors.Count);

            Assert.AreEqual(1, r.Data.Count);
            var rec = r.Data[0] as HeadRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("0", rec.Source);
        }

    }
}
