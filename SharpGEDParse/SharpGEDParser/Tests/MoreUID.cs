using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToConstant.Local

// Added _UID/UID support to NOTE, OBJE, REPO and SOUR.
// Technically not required: FamilySearch recommends INDI/FAM only. 
// Used by MyHeritage.
// Supported, so must be tested.

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class MoreUID : GedParseTest
    {

        [Test]
        public void NOTE1()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 _UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("blah blah blah", rec.Text);
            Assert.AreEqual("N1", rec.Ident);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["_UID"].Value);
        }

        [Test]
        public void NOTE2()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("blah blah blah", rec.Text);
            Assert.AreEqual("N1", rec.Ident);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["UID"].Value);
        }

        [Test]
        public void OBJE1()
        {
            var txt = "0 @M1@ OBJE\n1 FORM gif\n1 BLOB\n2 CONT not real\n1 _UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count); // treat BLOB as unknown

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["_UID"].Value);
        }

        [Test]
        public void OBJE2()
        {
            var txt = "0 @M1@ OBJE\n1 FORM gif\n1 BLOB\n2 CONT not real\n1 UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count); // treat BLOB as unknown

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["UID"].Value);
        }

        [Test]
        public void REPO1()
        {
            var txt = "0 @R1@ REPO\n1 NAME foobar\n1 _UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("REPO", rec.Tag);
            Assert.AreEqual("foobar", rec.Name);
            Assert.AreEqual("R1", rec.Ident);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["_UID"].Value);
        }

        [Test]
        public void REPO2()
        {
            var txt = "0 @R1@ REPO\n1 NAME foobar\n1 UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("REPO", rec.Tag);
            Assert.AreEqual("foobar", rec.Name);
            Assert.AreEqual("R1", rec.Ident);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["UID"].Value);
        }

        [Test]
        public void SOUR1()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 _UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as SourceRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("SOUR", rec.Tag);
            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["_UID"].Value);
        }

        [Test]
        public void SOUR2()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 UID blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as SourceRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("SOUR", rec.Tag);
            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);

            Assert.AreEqual(1, rec.Ids.Others.Count);
            Assert.AreEqual("blah", rec.Ids.Others["UID"].Value);
        }


    }
}
