using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class MiscIdTests : GedParseTest
    {
        public IndiRecord TestIndiId(string id)
        {
            var indi = string.Format("0 @I1@ INDI\n1 {0} number\n1 SEX M", id);
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            return rec;
        }

        public FamRecord TestFamId(string id)
        {
            var indi = string.Format("0 @I1@ FAM\n1 {0} number\n1 HUSB @p1@", id);
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            return rec;
        }

        [Test]
        public void AFN()
        {
            var rec = TestIndiId("AFN");
            Assert.AreEqual("number", rec.AFN.Value);
        }

        [Test]
        public void RFN()
        {
            var rec = TestIndiId("RFN");
            Assert.AreEqual("number", rec.RFN.Value);
        }

        [Test]
        public void UID()
        {
            var rec = TestIndiId("UID");
            Assert.AreEqual("number", rec.UID);
            rec = TestIndiId("_UID");
            Assert.AreEqual("number", rec.UID);
            var rec2 = TestFamId("UID");
            Assert.AreEqual("number", rec2.UID);
            rec2 = TestFamId("_UID");
            Assert.AreEqual("number", rec2.UID);
        }

        public IndiRecord TestIndiMultiId(string id)
        {
            var indi = string.Format("0 @I1@ INDI\n1 {0} number\n1 SEX M\n1 {0} number42", id);
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual('M', rec.Sex);
            return rec;
        }

        public FamRecord TestFamMultiId(string id)
        {
            var indi = string.Format("0 @I1@ FAM\n1 {0} number\n1 HUSB @p1@\n1 {0} number42", id);
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            return rec;
        }

        public NoteRecord TestNoteMultiId(string id)
        {
            var indi = string.Format("0 @I1@ NOTE Text\n1 {0} number\n1 CONC text2\n1 {0} number42", id);
            var rec = parse<NoteRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual("Texttext2", rec.Text);
            return rec;
        }

        public MediaRecord TestObjeMultiId(string id)
        {
            var indi = string.Format("0 @I1@ OBJE\n1 {0} number\n1 FILE 111-222-333\n2 FORM floppy\n1 {0} number42", id);
            var rec = parse<MediaRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual("111-222-333", rec.Files[0].FileRefn);
            return rec;
        }

        public Repository TestRepoMultiId(string id)
        {
            var indi = string.Format("0 @I1@ REPO\n1 {0} number\n1 NAME Diseases\n1 {0} number42", id);
            var rec = parse<Repository>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual("Diseases", rec.Name);
            return rec;
        }

        public SourceRecord TestSourMultiId(string id)
        {
            var indi = string.Format("0 @I1@ SOUR\n1 {0} number\n1 AUTH anon\n1 {0} number42", id);
            var rec = parse<SourceRecord>(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MultId, rec.Errors[0].Error);
            Assert.AreEqual(id, rec.Errors[0].Tag);
            Assert.AreEqual("anon", rec.Author);
            return rec;
        }

        [Test]
        public void MultiAFN()
        {
            var rec = TestIndiMultiId("AFN");
            Assert.AreEqual("number", rec.AFN.Value);
        }

        [Test]
        public void MultiRFN()
        {
            var rec = TestIndiMultiId("RFN");
            Assert.AreEqual("number", rec.RFN.Value);
        }

        [Test]
        public void MultiINDI_UID()
        {
            IndiRecord recI = TestIndiMultiId("UID");
            Assert.IsNotNull(recI.UID);
            Assert.AreEqual("number", recI.UID);
            recI = TestIndiMultiId("_UID");
            Assert.IsNotNull(recI.UID);
            Assert.AreEqual("number", recI.UID);
        }

        [Test]
        public void MultiFAM_UID()
        {
            FamRecord recF = TestFamMultiId("UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
            recF = TestFamMultiId("_UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
        }

        [Test]
        public void MultiOBJE_UID()
        {
            MediaRecord recF = TestObjeMultiId("UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
            recF = TestObjeMultiId("_UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
        }

        [Test]
        public void MultiNOTE_UID()
        {
            NoteRecord recF = TestNoteMultiId("UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
            recF = TestNoteMultiId("_UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
        }

        [Test]
        public void MultiREPO_UID()
        {
            Repository recF = TestRepoMultiId("UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
            recF = TestRepoMultiId("_UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
        }

        [Test]
        public void MultiSOUR_UID()
        {
            SourceRecord recF = TestSourMultiId("UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
            recF = TestSourMultiId("_UID");
            Assert.IsNotNull(recF.UID);
            Assert.AreEqual("number", recF.UID);
        }

        [Test]
        public void IndiREFN()
        {
            var indi = "0 @I1@ INDI\n1 REFN number\n1 SEX M";
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.AreEqual(1, rec.REFNs.Count);
        }
        [Test]
        public void IndiMultiREFN()
        {
            var indi = "0 @I1@ INDI\n1 REFN number\n1 SEX M\n1 REFN number2";
            var rec = parse<IndiRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('M', rec.Sex);
            Assert.AreEqual(2, rec.REFNs.Count); // TODO validate contents
        }
        [Test]
        public void FamREFN()
        {
            var indi = "0 @I1@ FAM\n1 REFN number\n1 HUSB @p1@";
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            Assert.AreEqual(1, rec.REFNs.Count);
        }
        [Test]
        public void FamMultREFN()
        {
            var indi = "0 @I1@ FAM\n1 REFN number\n1 HUSB @p1@\n1 REFN number42";
            var rec = parse<FamRecord>(indi);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            Assert.AreEqual(2, rec.REFNs.Count);
        }

    }
}
