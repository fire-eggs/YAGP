﻿using System.Text;
using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// Testing for 'Note structure': not a note record

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

        private NoteHold SimpleXrefSourCit(string tag)
        {
            string format = "0 {0}\n1 NOTE @N123@\n2 SOUR @S1@";
            var txt = string.Format(format, tag);
            var rec = NoteCommon(txt, 1, tag);
            Assert.AreEqual("N123", rec.Notes[0].Xref, "Note ref "+tag);
            Assert.AreEqual(1, rec.Notes[0].Cits.Count, "Cit count " + tag);
            Assert.AreEqual("S1", rec.Notes[0].Cits[0].Xref, "Cit val " + tag);
            return rec;
        }

        [Test]
        public void SourceCit1()
        {
            // GEDCOM 5.5 - source cit under a note structure (note ref)
            var rec = SimpleXrefSourCit(_INDI);
            rec = SimpleXrefSourCit(_FAM);
            rec = SimpleXrefSourCit(_SOUR);
            rec = SimpleXrefSourCit(_REPO);
            rec = SimpleXrefSourCit(_OBJE);
        }

        public NoteHold EmbNoteSourCit(string tag)
        {
            var txt = string.Format("0 {0}\n1 NOTE notes\n2 CONT more detail\n2 SOUR @S1@", tag);
            var rec = NoteCommon(txt, 1, tag);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text, "Note text " + tag);
            Assert.AreEqual(1, rec.Notes[0].Cits.Count, "Cit count "+ tag);
            Assert.AreEqual("S1", rec.Notes[0].Cits[0].Xref, "Cit val " + tag);
            return rec;
        }
        [Test]
        public void SourceCit2()
        {
            // GEDCOM 5.5 - source cit under a note structure (embedded note)
            var rec = EmbNoteSourCit(_INDI);
            rec = EmbNoteSourCit(_FAM);
            rec = EmbNoteSourCit(_SOUR);
            rec = EmbNoteSourCit(_REPO);
            rec = EmbNoteSourCit(_OBJE);
        }

        // TODO push to common test utility
        public string MakeInput(string[] recs)
        {
            StringBuilder inp = new StringBuilder();
            foreach (var rec in recs)
            {
                inp.Append(rec);
                inp.Append("\n");
            }
            return inp.ToString();
        }


        [Test]
        public void FamNoteSour()
        {
            // Hambleden-Tasmania_update.ged [via zDoAll551] showed a crash not caught by any other test
            // FAM.NOTE.SOUR
            string[] lines =
            {
                "0 @F1@ FAM",
                "1 HUSB @I40@",
                "1 WIFE @I39@",
                "1 MARR",
                "2 DATE 01 JAN 1828",
                "2 PLAC St George Hanover Square, Middlesex, England",
                "2 SOUR @S2@",
                "3 PAGE Email message 21 Apr 2011",
                "3 QUAY 3",
                "2 NOTE @NF1@",
                "3 SOUR @S2@",
                "4 PAGE Email message 21 Apr 2011",
                "4 QUAY 3",
                "2 SOUR @S10@",
                "1 CHAN",
                "2 DATE 15 NOV 2015",
                "3 TIME 00:00:13"
            };
            var inp = MakeInput(lines);
            var res = ReadIt(inp);
            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(0, res[0].Errors.Count);
            Assert.IsNotNull(res[0] as FamRecord);
        }
    }
}
