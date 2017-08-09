using NUnit.Framework;

// ReSharper disable InconsistentNaming

// Exercise missing "0 HEAD" record, and non-reading of file
namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class ReadingNoHead : ReadingUtil
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
            var r = BuildAndRead(lines0, GedReader.LB.UNIX, true);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.NumberLines);
        }

        [Test]
        public void NoHeadLFNoBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.UNIX, false);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.NumberLines);
        }

        [Test]
        public void NoHeadCRLFBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.DOS, true);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.NumberLines);
        }

        [Test]
        public void NoHeadCRLFNoBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.DOS, false);
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(0, r.NumberLines);
        }
    }
}
