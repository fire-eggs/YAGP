using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class MiscIdTests : GedParseTest
    {
        public void TestIndiId(string id)
        {
            var indi = string.Format("0 @I1@ INDI\n1 {0} number\n1 SEX M", id);
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.IsTrue(rec.Ids.HasId(id));
        }
        public void TestFamId(string id)
        {
            var indi = string.Format("0 @I1@ FAM\n1 {0} number\n1 HUSB @p1@", id);
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dad);
            Assert.IsTrue(rec.Ids.HasId(id));
        }

        [Test]
        public void AFN()
        {
            TestIndiId("AFN");
        }

        [Test]
        public void RFN()
        {
            TestIndiId("RFN");
        }

        [Test]
        public void UID()
        {
            TestIndiId("UID");
            TestIndiId("_UID");
            TestFamId("UID");
            TestFamId("_UID");
        }

        public void TestIndiMultiId(string id)
        {
            var indi = string.Format("0 @I1@ INDI\n1 {0} number\n1 SEX M\n1 {0} number42", id);
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.IsTrue(rec.Ids.HasId(id));
            // TODO need to be able to get id value and verify
        }
        public void TestFamMultiId(string id)
        {
            var indi = string.Format("0 @I1@ FAM\n1 {0} number\n1 HUSB @p1@\n1 {0} number42", id);
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dad);
            Assert.IsTrue(rec.Ids.HasId(id));
            // TODO need to be able to get id value and verify
        }

        [Test]
        public void MultiAFN()
        {
            TestIndiMultiId("AFN");
        }
        [Test]
        public void MultiRFN()
        {
            TestIndiMultiId("RFN");
        }
        [Test]
        public void MultiUID()
        {
            TestIndiMultiId("UID");
            TestIndiMultiId("_UID");
            TestFamMultiId("UID");
            TestFamMultiId("_UID");
        }

        [Test]
        public void IndiREFN()
        {
            var indi = "0 @I1@ INDI\n1 REFN number\n1 SEX M";
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
        }
        [Test]
        public void IndiMultiREFN()
        {
            var indi = "0 @I1@ INDI\n1 REFN number\n1 SEX M\n1 REFN number2";
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.AreEqual(2, rec.Ids.REFNs.Count); // TODO validate contents
        }
        [Test]
        public void FamREFN()
        {
            var indi = "0 @I1@ FAM\n1 REFN number\n1 HUSB @p1@";
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
        }
        [Test]
        public void FamMultREFN()
        {
            var indi = "0 @I1@ FAM\n1 REFN number\n1 HUSB @p1@\n1 REFN number42";
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual(2, rec.Ids.REFNs.Count);
        }

    }
}
