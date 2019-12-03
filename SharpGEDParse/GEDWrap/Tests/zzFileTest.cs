using System.ComponentModel.Design.Serialization;
using System.IO;
using NUnit.Framework;
using SharpGEDParser.Model;

namespace GEDWrap.Tests
{
    class zzFileTest
    {
        private string rootPath = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            @"..\..\..\..\Sample GED\");

        [Test]
        public void SimpleGed()
        {
            var path = Path.Combine(rootPath, "export_ged_919.ged");

            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);

            var indis = ged.Indi;
            Assert.AreEqual(2, indis.Count);

            var indi = ged.FindIndiByIdent("I919");
            Assert.IsNotNull(indi);

            Assert.AreEqual('M', indi.Sex);
            Assert.IsNotNull(indi.CHAN.Date);
            Assert.AreEqual(1, indi.Names.Count);
        }

        [Test]
        public void SimpleGed2()
        {
            var path = Path.Combine(rootPath, "ege.ged");
            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(1, ged.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.CustTagsSeen, ged.Errors[0].Error);
            Assert.AreEqual(0, ged.Unknowns.Count);

            var indi = ged.FindIndiByIdent("I26");
            Assert.IsNotNull(indi);
        }

        [Test]
        public void SimpleGed3()
        {
            // A simple GED file downloaded from the internet
            var path = Path.Combine(rootPath, "pallanezf.ged");

            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);
            Assert.AreEqual(0, ged.Unknowns.Count);

            var indi = ged.FindIndiByIdent("I30");
            Assert.IsNotNull(indi);
        }

    }
}
