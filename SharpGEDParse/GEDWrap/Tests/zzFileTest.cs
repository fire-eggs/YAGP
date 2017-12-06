using NUnit.Framework;
using SharpGEDParser.Model;

namespace GEDWrap.Tests
{
    class zzFileTest
    {
        [Test]
        public void SimpleGed()
        {
            var path = @"E:\projects\YAGP\Sample GED\export_ged_919.ged";// TODO project-relative path

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
            var path = @"E:\projects\YAGP\Sample GED\ege.ged"; // TODO project-relative path
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
            var path = @"Z:\HOST_E\proj\GED\all_ged\01\pallanezf.ged"; // TODO project-relative path
            Forest ged = new Forest();
            ged.ParseGEDCOM(path);
            Assert.AreEqual(0, ged.Errors.Count);
            Assert.AreEqual(0, ged.Unknowns.Count);

            var indi = ged.FindIndiByIdent("I30");
            Assert.IsNotNull(indi);
        }

    }
}
