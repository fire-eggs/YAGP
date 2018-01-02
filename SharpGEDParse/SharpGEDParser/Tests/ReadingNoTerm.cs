using NUnit.Framework;

// ReSharper disable InconsistentNaming

// Exercise files where last line has no line terminator
namespace SharpGEDParser.Tests
{
    class ReadingNoTerm : ReadingUtil
    {

        // Tiny test
        private readonly string[] lines0 =
        {
            "0 HEAD",
            "1 SOUR 0",
            "0 TRLR"
        };

        [Test]
        public void TinyLFNoBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.UNIX, false, false);
            Assert.AreEqual(3, r.NumberLines);
            //Assert.AreEqual(0, r.Errors.Count);
            //Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyDOSNoBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.DOS, false, false);
            Assert.AreEqual(3, r.NumberLines);
            //Assert.AreEqual(0, r.Errors.Count);
            //Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyLFBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.UNIX, true, false);
            Assert.AreEqual(3, r.NumberLines);
            //Assert.AreEqual(0, r.Errors.Count);
            //Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

        [Test]
        public void TinyDOSBom()
        {
            var r = BuildAndRead(lines0, GedReader.LB.DOS, true, false);
            Assert.AreEqual(3, r.NumberLines);
            //Assert.AreEqual(0, r.Errors.Count);
            //Assert.AreEqual("0 TRLR", r.Lines[2]);
        }

    }
}
