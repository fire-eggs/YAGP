using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming
// ReSharper disable once RedundantCommaInArrayInitializer

// TODO ged read: line format as property?
// TODO ged read: spurious cr/lf locations as property?

// Tests to exercise reading of physical files - BOM, linebreak variants

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class Reading : ReadingUtil
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

        private FileRead common(GedReader.LB format, bool bom)
        {
            var r = BuildAndRead(lines1, format, bom);
            Assert.AreEqual(lines1.Length, r.NumberLines);
            return r;
        }

        [Test]
        public void SmallLFNoBom()
        {
            var r = common(GedReader.LB.UNIX, false);
            Assert.AreEqual(0, r.Errors.Count);
            // TODO additional checks?
        }

        [Test]
        public void SmallCRLFNoBom()
        {
            var r = common(GedReader.LB.DOS, false);
            Assert.AreEqual(0, r.Errors.Count);
            // TODO additional checks?
        }

        [Test]
        public void SmallCRNoBom()
        {
            var r = BuildAndRead(lines1, GedReader.LB.MAC, false);
            Assert.AreEqual(0, r.NumberLines);

            //Assert.AreEqual("ERR", r.LineBreaks);
            // Error: invalid line break format
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.UnsuppLB, r.Errors[0].Error);
        }

        [Test]
        public void SmallLFBom()
        {
            var r = common(GedReader.LB.UNIX, true);

            // Error: bom/head.char mismatch
            Assert.AreEqual(1, r.Errors.Count);
        }

        [Test]
        public void SmallCRLFBom()
        {
            var r = common(GedReader.LB.DOS, true);

            // Error: bom/head.char mismatch
            Assert.AreEqual(1, r.Errors.Count);
        }

        [Test]
        public void SmallCRBom()
        {
            var r = BuildAndRead(lines1, GedReader.LB.MAC, true);
            Assert.AreEqual(0, r.NumberLines);

            //Assert.AreEqual("ERR", r.LineBreaks);
            // Error: invalid line break format
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.UnsuppLB, r.Errors[0].Error);
        }
    }
}
