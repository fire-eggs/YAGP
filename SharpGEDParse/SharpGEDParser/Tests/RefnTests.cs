using NUnit.Framework;
using SharpGEDParser.Model;

// TODO refn in sub-record?

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class RefnTests : GedParseTest
    {
        // 'Common' testing for REFN. A little tricky as each record type has a different 
        // "minimum detail required" to prevent not-related-to-REFN errors from appearing.
        // TODO would be solved by verifying error details?

        private GEDCommon Common(string recB)
        {
            var res = ReadIt(recB);
            Assert.AreEqual(1, res.Count);
            var rec = res[0];
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("I1", rec.Ident);
            return rec;
        }

        private GEDCommon TestSingle(string tag, string recExtra)
        {
            var txt = string.Format("0 @I1@ {0}\n1 REFN 001\n{1}", tag, recExtra);
            var rec = Common(txt);
            Assert.AreEqual(1, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            return rec;
        }

        private GEDCommon TestMulti(string tag, string recExtra)
        {
            // Multiple REFNs - 2d appended to record
            var txt = string.Format("0 @I1@ {0}\n1 REFN 001\n{1}\n1 REFN number2", tag, recExtra);
            var rec = Common(txt);
            Assert.AreEqual(2, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual("number2", rec.REFNs[1].Value);
            return rec;
        }

        private GEDCommon RefnExtra(string tag, string recExtra)
        {
            // Extra on REFN
            var txt = string.Format("0 @I1@ {0}\n1 REFN 001\n2 TYPE blah\n{1}\n1 REFN number2", tag, recExtra);
            var rec = Common(txt);
            Assert.AreEqual(2, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual("number2", rec.REFNs[1].Value);
            Assert.AreEqual(1, rec.REFNs[0].Extra.LineCount);
            return rec;
        }

        private GEDCommon RefnExtra2(string tag, string recExtra)
        {
            var txt = string.Format("0 @I1@ {0}\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n{1}\n1 REFN number2", tag, recExtra);
            var rec = Common(txt);
            Assert.AreEqual(2, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual("number2", rec.REFNs[1].Value);

            Assert.AreEqual(2, rec.REFNs[0].Extra.LineCount);
            return rec;
        }

        private delegate GEDCommon Atest(string tag, string txt);

        private void VerifyIndi(Atest torun)
        {
            // Verify INDI specific details
            GEDCommon res = torun("INDI", "1 SEX U");
            var rec = res as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual('U', rec.Sex);
        }

        private void VerifyFam(Atest torun)
        {
            // Verify FAM specific details
            GEDCommon res = torun("FAM", "1 HUSB @P1@");
            var rec = res as FamRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual("P1", rec.Dads[0]);
        }

        private void VerifyNote(Atest torun)
        {
            // Verify NOTE specific details
            GEDCommon res = torun("NOTE", "1 CONC fumbar");
            var rec = res as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("fumbar", rec.Text);
        }

        private void VerifyRepo(Atest torun)
        {
            // Verify REPO specific details
            GEDCommon res = torun("REPO", "1 NAME fumbar");
            var rec = res as Repository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("fumbar", rec.Name);
        }

        private void VerifyObje(Atest torun)
        {
            GEDCommon res = torun("OBJE", "1 FILE ref\n2 FORM tif");
            var rec = (res as MediaRecord);
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("ref", rec.Files[0].FileRefn);
            Assert.AreEqual("tif", rec.Files[0].Form);
        }

        private void VerifySour(Atest torun)
        {
            // Verify REPO specific details
            GEDCommon res = torun("SOUR", "1 ABBR fumbar");
            var rec = res as SourceRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("fumbar", rec.Abbreviation);
        }

        [Test]
        public void IndiRefn()
        {
            VerifyIndi(TestSingle);
            VerifyIndi(TestMulti);
            VerifyIndi(RefnExtra);
            VerifyIndi(RefnExtra2);
        }

        [Test]
        public void FamRefn()
        {
            VerifyFam(TestSingle);
            VerifyFam(TestMulti);
            VerifyFam(RefnExtra);
        }

        [Test]
        public void NoteRefn()
        {
            VerifyNote(TestSingle);
            VerifyNote(TestMulti);
            VerifyNote(RefnExtra);
        }

        [Test]
        public void RepoRefn()
        {
            VerifyRepo(TestSingle);
            VerifyRepo(TestMulti);
            VerifyRepo(RefnExtra);
        }

        [Test]
        public void ObjeRefn()
        {
            VerifyObje(TestSingle);
            VerifyObje(TestMulti);
            VerifyObje(RefnExtra);
        }

        [Test]
        public void SourRefn()
        {
            VerifySour(TestSingle);
            VerifySour(TestMulti);
            VerifySour(RefnExtra);
        }
    }
}
