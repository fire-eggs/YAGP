using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class NoteLevel1 : GedParseTest
    {
        private NoteHold NoteCommon(string txt, int count, string tag)
        {
            var res = ReadOne(txt);
            Assert.IsNotNull(res, tag);
            Assert.AreEqual(0, res.Errors.Count, tag);
            var rec = res as NoteHold;
            Assert.IsNotNull(rec, tag);
            Assert.AreEqual(count, rec.Notes.Count, tag);
            return rec;
        }

        private void SimpleXrefCommon(string tag)
        {
            var txt = string.Format("0 {0}\n1 NOTE @N123@", tag);
            var rec = NoteCommon(txt, 1, tag);
            Assert.AreEqual("N123", rec.Notes[0].Xref, tag);
        }

        private const string _INDI = "@I1@ INDI";
        private const string _FAM = "@F1@ FAM";
        private const string _OBJE = "@O1@ OBJE\n1 FILE ref\n2 FORM wav";
        private const string _REPO = "@R1@ REPO\n1 NAME foo";
        private const string _SOUR = "@S1@ SOUR";

        [Test]
        public void SimpleXrefM()
        {
            // Single, simple xref note
            SimpleXrefCommon(_INDI);
            SimpleXrefCommon(_FAM);
            SimpleXrefCommon(_SOUR);
            SimpleXrefCommon(_REPO);
            SimpleXrefCommon(_OBJE);
        }

        private void SimpleText(string tag)
        {
            var txt = string.Format("0 {0}\n1 NOTE notes\n2 CONT more detail", tag);
            var rec = NoteCommon(txt, 1, tag);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
        }

        [Test]
        public void SimpleTextM()
        {
            // Single, simple text note
            SimpleText(_INDI);
            SimpleText(_FAM);
            SimpleText(_OBJE);
            SimpleText(_SOUR);
            SimpleText(_REPO);
        }

        private GEDCommon TwoNotesSplit(string part1, string part2)
        {
            var txt = string.Format("0 {0}\n1 NOTE notes\n2 CONT more detail\n{1}\n1 NOTE notes2", part1, part2);
            var rec = NoteCommon(txt, 2, part1);
            //var res = ReadOne(txt);
            //Assert.IsNotNull(res, part1);
            //Assert.AreEqual(0, res.Errors.Count, part1);
            //var rec = res as NoteHold;
            //Assert.IsNotNull(rec, part1);
            //Assert.AreEqual(2, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
            Assert.AreEqual("notes2", rec.Notes[1].Text);
            return rec as GEDCommon;
        }

        [Test]
        public void TwoNotesSplitM()
        {
            // Two notes, with sub-details between [verifies that note parsing doesn't gobble up record details]
            var res = TwoNotesSplit(_INDI, "1 SEX F");
            var indi = res as IndiRecord;
            Assert.IsNotNull(indi, "INDI");
            Assert.AreEqual('F', indi.Sex, "INDI");

            res = TwoNotesSplit(_FAM, "1 HUSB @I1@");
            var fam = res as FamRecord;
            Assert.IsNotNull(fam, "FAM");
            Assert.AreEqual("I1", fam.Dads[0], "FAM");

            res = TwoNotesSplit("@R1@ REPO", "1 NAME foo");
            var repo = res as Repository;
            Assert.IsNotNull(repo, "REPO");
            Assert.AreEqual("foo", repo.Name, "REPO");

            res = TwoNotesSplit("@01@ OBJE", "1 FILE ref\n2 FORM wav");
            var obje = res as MediaRecord;
            Assert.IsNotNull(obje, "OBJE");
            Assert.AreEqual("ref", obje.Files[0].FileRefn, "OBJE");

            res = TwoNotesSplit(_SOUR, "1 AUTH Fred");
            var src = res as SourceRecord;
            Assert.IsNotNull(src, "SOUR");
            Assert.AreEqual("Fred", src.Author, "SOUR");
        
        }

        [Test]
        public void LeadSpaceConc()
        {
            // Leading spaces, trailing spaces are to be preserved for notes
            var txt = "0 @I1@ INDI\n1  NOTE\n2 CONC    Line";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);

            Assert.AreEqual("   Line", rec.Notes[0].Text);
        }

        [Test]
        public void LeadSpaceConc2()
        {
            // Leading spaces, trailing spaces are to be preserved for notes
            var txt = "0 @I1@ INDI\n1  NOTE    A \n2 CONC    Line";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);

            Assert.AreEqual("   A    Line", rec.Notes[0].Text);
        }

        [Test]
        public void LeadSpaceMult()
        {
            // Leading spaces, trailing spaces are to be preserved for notes
            var txt = "0 @I1@ INDI\n1 NOTE\n2 CONT    Line \n2 CONC   more \n2 CONT       and";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);

            Assert.AreEqual("\n   Line   more \n      and", rec.Notes[0].Text);
        }

        [Test]
        public void DoubleAt()
        {
            // Doubled '@'s are supposed to be replaced with single
            var txt = "0 @I1@ INDI\n1 NOTE Where it's @@";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);

            Assert.AreEqual("Where it's @", rec.Notes[0].Text);

        }

    }
}
