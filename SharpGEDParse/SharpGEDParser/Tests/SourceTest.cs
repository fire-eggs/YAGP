﻿using NUnit.Framework;
using SharpGEDParser.Model;
using System;

// TODO common testing REFN, RIN ... should move to own test, exercised for all records

// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    // TODO DATA/EVEN/DATE requires additional parsing/validation
    // TODO REPO/CALN/MEDI requires additional parsing/validation
    
    // TODO missing newline confused parsing: a legit, necessary test
    // TODO : var txt = "0 @S1@ SOUR\n1 OBJE @obje1\n1 AUTH Fred\n1 OBJE @obje2@"; : missing trailing '@', was not caught?

    // TODO multimedia link(s) - 5.5 syntax [also NOTEs]

    // TODO xref style note
    // 0 @S1@ SOUR\n1 NOTE @
    // 0 @S1@ SOUR\n1 NOTE @ @
    // 0 @S1@ SOUR\n1 NOTE @@@



    // Testing for SOURCE records
    [TestFixture]
    class SourceTest : GedParseTest
    {
        public new static SourceRecord ReadOne(string teststring)
        {
            return parse<SourceRecord>(teststring);
        }

        [Test]
        public void TestBasic()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred";
            var rec = ReadOne(txt);

            Assert.AreEqual("SOUR", rec.Tag);
            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
        }

        [Test]
        public void TestAbbr()
        {
            var txt = "0 @S1@ SOUR\n1 ABBR Fred";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Abbreviation);
        }

        [Test]
        public void TestMissingIdent()
        {
            var txt = "0 SOUR\n1 ABBR Fred";
            var rec = ReadOne(txt);

            Assert.AreNotEqual(0, rec.Errors.Count); // TODO error details
            Assert.AreEqual("Fred", rec.Abbreviation);
        }

        // Common code for sub-tags which use CONC/CONT
        private SourceRecord TestExtSubTag(string tag, string propName)
        {
            var txt = string.Format("0 @S1@ SOUR\n1 {0} Fred \n2 CONC Flintstone\n2 CONT yabba dabba doo", tag);
            var rec = ReadOne(txt);
            Assert.AreEqual("S1", rec.Ident);
            var val = rec.GetType().GetProperty(propName).GetValue(rec, null);
            Assert.AreEqual("Fred Flintstone\nyabba dabba doo", val);
            return rec;
        }

        [Test]
        public void TestAuth()
        {
            var rec = TestExtSubTag("AUTH", "Author");
        }

        [Test]
        public void TestTitle()
        {
            var rec = TestExtSubTag("TITL", "Title");
        }

        [Test]
        public void TestPubl()
        {
            var rec = TestExtSubTag("PUBL", "Publication");
        }

        [Test]
        public void TestText()
        {
            var rec = TestExtSubTag("TEXT", "Text");
        }

        [Test]
        public void TestReal()
        {
            // A real source record taken from a GED file... Note the missing trailing space in the TEXT tag!
            var txt = "0 @S7@ SOUR\n1 ABBR Truelove-Newton, Alice\n1 TITL Ms Alice [Truelove] Newton, her info abt Lourena Vaughan\n1 TEXT Estelle Truelove's daughter--born in vacinity of New Hill/Aplex, Wake\n2 CONC Co., North Carloina";
            var rec = ReadOne(txt);
            Assert.AreEqual("S7", rec.Ident);
            Assert.AreEqual("Truelove-Newton, Alice", rec.Abbreviation);
            Assert.IsNotNull(rec.Title);
            Assert.IsNotNull(rec.Text);
        }

        #region Custom
        [Test]
        public void TestCust1()
        {
            var txt = "0 @N1@ SOUR\n1 _CUST foobar\n1 RIN foobar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @N1@ SOUR\n1 _CUST foobar\n2 CONC foobar2";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestCust3()
        {
            // custom tag at the end of the record
            var txt = "0 @N1@ SOUR\n1 ABBR foobar\n1 _CUST foobar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
            Assert.AreEqual("foobar", rec.Abbreviation);
        }
        #endregion

        #region REFN
        [Test]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @N1@ SOUR\n1 REFN 001\n1 ABBR fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual("fumbar", rec.Abbreviation);
        }
        [Test]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @N1@ SOUR\n1 REFN 001\n1 TITL fumbar\n1 REFN 002";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual("002", rec.REFNs[1].Value);
            Assert.AreEqual("fumbar", rec.Title);
        }

        [Test]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @N1@ SOUR\n1 REFN 001\n2 TYPE blah\n1 AUTH fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual(1, rec.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Author);
        }

        [Test]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @N1@ SOUR\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 TEXT fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.REFNs.Count);
            Assert.AreEqual("001", rec.REFNs[0].Value);
            Assert.AreEqual(2, rec.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }
        #endregion REFN

        [Test]
        public void TestData()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(0, rec.Data.Events.Count);
        }

        [Test]
        public void TestDataEvent()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happened\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("things happened", rec.Data.Events[0].Text);
        }

        [Test]
        public void TestDataEvent2()
        {
            // multi events; multi data notes
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happened\n2 NOTE a note\n2 EVEN more happenings\n2 NOTE another\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(2, rec.Data.Events.Count);
            Assert.AreEqual("things happened", rec.Data.Events[0].Text);
            Assert.AreEqual("more happenings", rec.Data.Events[1].Text);
            Assert.AreEqual(2, rec.Data.Notes.Count);
            Assert.AreEqual("a note", rec.Data.Notes[0].Text);
            Assert.AreEqual("another", rec.Data.Notes[1].Text);
        }

        [Test]
        public void TestDataAgnc()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 AGNC fred\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual("fred", rec.Data.Agency);
        }

        [Test]
        public void TestEvenAgnc()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happen\n2 AGNC barney\n1 TEXT Not on your nelly";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Not on your nelly", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual("barney", rec.Data.Agency);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("things happen", rec.Data.Events[0].Text);
            
        }

        [Test]
        public void TestAgncEven()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 AGNC barney\n2 EVEN things happen\n1 TEXT Not on your nelly";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Not on your nelly", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual("barney", rec.Data.Agency);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("things happen", rec.Data.Events[0].Text);
        }

        [Test]
        public void TestDataNote()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 NOTE A note\n3 CONT continued - the word TE\n3 CONC ST not busted\n1 TEXT blah blah";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Notes.Count);
            Assert.AreEqual("A note\ncontinued - the word TEST not busted", rec.Data.Notes[0].Text);
        }

        [Test]
        public void TestDataNote2()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 NOTE A note\n3 CONT continued - the word TE\n3 CONC ST not busted\n2 NOTE another\n1 TEXT blah blah";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(2, rec.Data.Notes.Count);
            Assert.AreEqual("A note\ncontinued - the word TEST not busted", rec.Data.Notes[0].Text);
            Assert.AreEqual("another", rec.Data.Notes[1].Text);
        }

        [Test]
        public void TestDataEventNote()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN happens\n3 DATE whatever\n3 PLAC whereever\n2 NOTE A note\n3 CONT continued - the word TE\n3 CONC ST not busted\n2 AGNC whoever\n1 TEXT blah blah";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Notes.Count);
            Assert.AreEqual("A note\ncontinued - the word TEST not busted", rec.Data.Notes[0].Text);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("happens", rec.Data.Events[0].Text);
            Assert.AreEqual("whatever", rec.Data.Events[0].Date);
            Assert.AreEqual("whereever", rec.Data.Events[0].Place);
            Assert.AreEqual("whoever", rec.Data.Agency);
        }

        [Test]
        public void TestEventDate()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happened\n3 DATE period\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("things happened", rec.Data.Events[0].Text);
            Assert.AreEqual("period", rec.Data.Events[0].Date);
        }

        [Test]
        public void TestEventPlace()
        {
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happened\n3 PLAC period\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("things happened", rec.Data.Events[0].Text);
            Assert.AreEqual("period", rec.Data.Events[0].Place);
        }

        [Test]
        public void DataNoEven()
        {
            // TODO not sure if this should be accepted... 5.5/5.5.1 diff?
            var txt = "0 @S1@ SOUR\n1 DATA\n3 PLAC period\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("period", rec.Data.Events[0].Place);
        }

        #region Custom under sub-tags
        [Test]
        public void TestDataCust()
        {
            // custom under SOUR
            var txt = "0 @S1@ SOUR\n1 DATA\n1 _CUST what?\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual(1, rec.Unknowns.Count); // TODO validate details
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(0, rec.Data.Events.Count);
        }

        [Test]
        public void TestDataCust2()
        {
            // custom under DATA
            var txt = "0 @S1@ SOUR\n1 DATA\n2 _CUST what?\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.OtherLines.Count); // TODO validate details
            Assert.AreEqual(0, rec.Data.Events.Count);
        }

        [Test]
        public void TestDataCust3()
        {
            // Custom under DATA\EVEN
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN stuff\n3 _CUST what?\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.OtherLines.Count); // TODO validate details
            Assert.AreEqual(1, rec.Data.Events.Count);
        }

        [Test]
        public void TestDataCust4()
        {
            // Custom under DATA\EVEN\PLAC
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN stuff\n3 PLAC where?\n4 _CUST what?\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.OtherLines.Count); // TODO validate details
            Assert.AreEqual(1, rec.Data.Events.Count);
        }

        [Test]
        public void TestDataCust5()
        {
            // Custom under DATA\AGNC
            var txt = "0 @S1@ SOUR\n1 DATA\n2 AGNC who\n3 _CUST what?\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(1, rec.Data.OtherLines.Count); // TODO validate details
        }
        #endregion

        [Test]
        public void TestDeep1()
        {
            // NOTE, DATA+NOTE, NOTE, CHAN+NOTE, NOTE
            var txt = "0 @S1@ SOUR\n" +
                      "1 NOTE A note on source\n3 CONT continued - the word TE\n3 CONC ST not broken\n" +
                      "1 DATA\n2 EVEN happens\n3 DATE whatever\n3 PLAC whereever\n2 AGNC whoever\n" +
                      "2 NOTE A data note\n3 CONT continued - the word TE\n3 CONC ST not busted\n" +
                      "1 NOTE Note two on source\n2 CONT continued - the word TE\n2 CONC ST not broken\n" +
                      "1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000\n" +
                      "1 NOTE Note three on source\n2 CONT continued - the word TE\n2 CONC ST not broken\n" +
                      "1 TEXT blah blah";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual("blah blah", rec.Text);

            Assert.AreEqual(1, rec.Data.Events.Count);
            Assert.AreEqual("happens", rec.Data.Events[0].Text);
            Assert.AreEqual("whatever", rec.Data.Events[0].Date);
            Assert.AreEqual("whereever", rec.Data.Events[0].Place);
            Assert.AreEqual("whoever", rec.Data.Agency);
            Assert.AreEqual(1, rec.Data.Notes.Count);
            Assert.AreEqual("A data note\ncontinued - the word TEST not busted", rec.Data.Notes[0].Text);

            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text);

            Assert.AreEqual(3, rec.Notes.Count);
            Assert.AreEqual("A note on source\ncontinued - the word TEST not broken", rec.Notes[0].Text);
            Assert.AreEqual("Note two on source\ncontinued - the word TEST not broken", rec.Notes[1].Text);
            Assert.AreEqual("Note three on source\ncontinued - the word TEST not broken", rec.Notes[2].Text);

        }

        // SOUR/1 DATA/NOTE/1 NOTE/1 OBJE/1 NOTE/1 REPO/2 NOTE/1 NOTE/1 CHAN/2 NOTE

        [Test]
        public void ExtraText1()
        {
            var txt = "0 @S1@ SOUR blah blah blah\n1 AUTH Fred";
            var rec = ReadOne(txt);

            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
        }

        [Test]
        public void ExtraDataText1()
        {
            var txt = "0 @S1@ SOUR\n1 DATA blah blah blah\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(0, rec.Data.Events.Count);
            Assert.AreEqual(1, rec.Data.Notes.Count);
            Assert.AreEqual("blah blah blah", rec.Data.Notes[0].Text);
        }

        [Test]
        public void ExtraRepoText1()
        {
            // simple repo xref, interspersed
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO @R1@ no more\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);

            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual(" no more", rec.Cits[0].Notes[0].Text);
        }

        #region Multimedia link
        [Test]
        public void Obje1()
        {
            // basic OBJE - taken from ALLGED.GED (5.5.1 syntax)
            var txt = "0 @S1@ SOUR\n1 OBJE\n2 FILE file name.bmp\n3 FORM bmp\n2 TITL A bmp picture\n2 NOTE A note\n3 CONT Note continued here. The word TE\n3 CONC ST should not be broken!";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);

            var rec2 = rec as MediaHold;  // TODO useful for common media link parsing - SOUR, FAM, INDI records; Event, SOUR sub-records
            Assert.AreEqual(1, rec2.Media.Count);
            var med = rec2.Media[0];

            Assert.AreEqual("A bmp picture", med.Title);
            Assert.AreEqual(1, med.Files.Count);
            var fil = med.Files[0];
            Assert.AreEqual("file name.bmp", fil.FileRefn);
            Assert.AreEqual("bmp", fil.Form);

            // TODO ALLGED.GED implies a NOTE is possible on a OBJE sub-tag; not according to GED standard? YES: GEDCOM 5.5
        }

        [Test]
        public void Obje2()
        {
            // Simple xref multimedia links; multiple
            var txt = "0 @S1@ SOUR\n1 OBJE @obje1@\n1 AUTH Fred\n1 OBJE @obje2@\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Media.Count);
            Assert.AreEqual("obje1", rec.Media[0].Xref);
            Assert.AreEqual("obje2", rec.Media[1].Xref);
        }

        [Test]
        public void Obje3()
        {
            // Embedded multimedia links; multiple
            var txt = "0 @S1@ SOUR\n1 OBJE\n2 FILE blah\n3 FORM wav\n2 FILE flint\n3 FORM bmp\n1 AUTH Fred\n1 OBJE\n2 FILE foe\n3 FORM jpg\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Media.Count);
            var med = rec.Media[0];

            Assert.AreEqual(2, med.Files.Count);
            var fil = med.Files[0];
            Assert.AreEqual("blah", fil.FileRefn);
            Assert.AreEqual("wav", fil.Form);
            fil = med.Files[1];
            Assert.AreEqual("flint", fil.FileRefn);
            Assert.AreEqual("bmp", fil.Form);

            med = rec.Media[1];
            Assert.AreEqual(1, med.Files.Count);
            fil = med.Files[0];
            Assert.AreEqual("foe", fil.FileRefn);
            Assert.AreEqual("jpg", fil.Form);

        }

        [Test]
        public void Obje4()
        {
            // basic object - taken from ALLGED.GED - 5.5 syntax. Note in particular that the FORM tag *precedes* the FILE tag.
            var txt = "0 @S1@ SOUR\n1 OBJE\n2 FORM bmp\n2 TITL A bmp picture\n2 FILE file name.bmp\n2 NOTE A note\n3 CONT Note continued here. The word TE\n3 CONC ST should not be broken!";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);

            var rec2 = rec as MediaHold;  // TODO useful for common media link parsing - SOUR, FAM, INDI records; Event, SOUR sub-records
            Assert.AreEqual(1, rec2.Media.Count);
            var med = rec2.Media[0];

            Assert.AreEqual("A bmp picture", med.Title);
            Assert.AreEqual(1, med.Files.Count);
            var fil = med.Files[0];
            Assert.AreEqual("file name.bmp", fil.FileRefn);
            Assert.AreEqual("bmp", fil.Form);

            Assert.AreEqual(1, med.Notes.Count);
        }

        // TODO multi-OBJE testing - 5.5.1 syntax, 5.5 syntax

        #endregion

        #region Repository Citation

        [Test]
        public void RepoBasic()
        {
            // simple repo xref, interspersed
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO @R1@\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);
        }

        [Test]
        public void RepoBasic2()
        {
            // two simple repo xref, interspersed
            var txt = "0 @S1@ SOUR\n1 REPO @R2@\n1 AUTH Fred\n1 REPO @R1@\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("R2", rec.Cits[0].Xref);
            Assert.AreEqual("R1", rec.Cits[1].Xref);
        }

        [Test]
        public void RepoEmbed()
        {
            // simple embedded repo, interspersed
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO\n2 CALN number\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("number", rec.Cits[0].CallNums[0].Number);
        }

        [Test]
        public void RepoEmbed2()
        {
            // simple embedded repo, interspersed
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO\n2 CALN number\n3 MEDI type\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(0, rec.Unknowns.Count); // From mutation testing: verify struct parsing correctly points to end of sub-record

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("number", rec.Cits[0].CallNums[0].Number);
            Assert.AreEqual("type", rec.Cits[0].CallNums[0].Media);
        }

        [Test]
        public void Repos2()
        {
            // two repo, interspersed
            var txt = "0 @S1@ SOUR\n1 REPO @R1@\n1 AUTH Fred\n1 REPO\n2 CALN number\n3 MEDI type\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);
            Assert.AreEqual("number", rec.Cits[1].CallNums[0].Number);
            Assert.AreEqual("type", rec.Cits[1].CallNums[0].Media);
        }

        [Test]
        public void RepoNote()
        {
            // two repo with notes, interspersed
            var txt = "0 @S1@ SOUR\n1 REPO @R1@\n" +
                      "2 NOTE A note\n3 CONT continued - the word TE\n3 CONC ST not broken\n" +
                      "1 AUTH Fred\n1 REPO\n2 CALN number\n3 MEDI type\n" +
                      "2 NOTE Note two\n3 CONT continued - the word TE\n3 CONC ST not borked\n" +
                      "1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);

            Assert.AreEqual("R1", rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual("A note\ncontinued - the word TEST not broken", rec.Cits[0].Notes[0].Text);

            Assert.AreEqual("number", rec.Cits[1].CallNums[0].Number);
            Assert.AreEqual("type", rec.Cits[1].CallNums[0].Media);
            Assert.AreEqual(1, rec.Cits[1].Notes.Count);
            Assert.AreEqual("Note two\ncontinued - the word TEST not borked", rec.Cits[1].Notes[0].Text);
        }

        [Test]
        public void RepoCustom()
        {
            // simple repo xref, interspersed
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO @R1@\n2 _CUST custom\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Cits[0].OtherLines.Count);
        }

        #endregion

        public void HasErrors(GEDCommon rec, int count)
        {
            Assert.AreEqual(count, rec.Errors.Count);
            Assert.AreNotEqual(0, rec.Errors[0].Error);
        }

        [Test]
        public void InvalidRepoId()
        {
            var txt = "0 @S1@ SOUR\n1 REPO @   @";
            var rec = ReadOne(txt);

            HasErrors(rec, 1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.IsNull(rec.Cits[0].Xref);

            txt = "0 @S1@ SOUR\n1 REPO @ gibberish";
            rec = ReadOne(txt);

            HasErrors(rec, 1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.IsNull(rec.Cits[0].Xref);
        }

        [Test]
        public void InvalidRepoId2()
        {
            var txt = "0 @S1@ SOUR\n1 REPO @@@";
            var rec = ReadOne(txt);

            HasErrors(rec, 1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("@", rec.Cits[0].Xref); // TODO is this correct???
        }

        [Test]
        public void Crash()
        {
            // MEDI without a CALN - would crash at one point
            var txt = "0 @S1@ SOUR\n1 REPO\n2 MEDI blah";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(1, rec.Cits[0].CallNums.Count);
            Assert.AreEqual("blah", rec.Cits[0].CallNums[0].Media);

            // TODO should this be an error? (MEDI without CALN)
        }

        [Test]
        public void MultMedi()
        {
            // MEDI without a CALN - would crash at one point
            var txt = "0 @S1@ SOUR\n1 REPO\n2 MEDI blah\n2 MEDI foo";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(1, rec.Cits[0].CallNums.Count);

            // TODO should this be an error - last one 'wins'
            Assert.AreEqual("foo", rec.Cits[0].CallNums[0].Media);

            // TODO should this be an error? (MEDI without CALN)
        }

        [Test]
        public void MultCaln()
        {
            // multiple CALN (issues from mutation testing)
            var txt = "0 @S1@ SOUR\n1 REPO @R1@\n1 AUTH Fred\n1 REPO\n2 CALN number1\n3 MEDI type\n2 NOTE repo note\n2 CALN number2\n2 CALN number3\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);
            Assert.AreEqual(0, rec.Cits[0].CallNums.Count);
            Assert.AreEqual(3, rec.Cits[1].CallNums.Count);
            Assert.AreEqual("number1", rec.Cits[1].CallNums[0].Number);
            Assert.AreEqual("type", rec.Cits[1].CallNums[0].Media);
            Assert.AreEqual("number2", rec.Cits[1].CallNums[1].Number);
            Assert.IsNull(rec.Cits[1].CallNums[1].Media);
            Assert.AreEqual("number3", rec.Cits[1].CallNums[2].Number);
            Assert.IsNull(rec.Cits[1].CallNums[2].Media);
            Assert.AreEqual(1, rec.Cits[1].Notes.Count);
            Assert.AreEqual("repo note", rec.Cits[1].Notes[0].Text);
        }

        [Test]
        public void MultMediNoCaln()
        {
            // multiple MEDI w/o CALLN (issues from mutation testing)
            var txt = "0 @S1@ SOUR\n1 REPO @R1@\n1 AUTH Fred\n1 REPO\n2 MEDI type\n" +
                      "2 CALN number1\n3 MEDI type\n2 NOTE repo note\n2 MEDI foob\n" +
                      "2 CALN number2\n2 CALN number3\n1 RIN rin-chan";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count); // mutation test: verify no error w/ valid xref

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Fred", rec.Author);
            Assert.AreEqual("rin-chan", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("R1", rec.Cits[0].Xref);
            Assert.AreEqual(0, rec.Cits[0].CallNums.Count);
            Assert.AreEqual(4, rec.Cits[1].CallNums.Count);

            Assert.IsNull(rec.Cits[1].CallNums[0].Number);
            Assert.AreEqual("type", rec.Cits[1].CallNums[0].Media);

            Assert.AreEqual("number1", rec.Cits[1].CallNums[1].Number);
            Assert.AreEqual("foob", rec.Cits[1].CallNums[1].Media); // TODO is this correct?

            Assert.AreEqual("number2", rec.Cits[1].CallNums[2].Number);
            Assert.IsNull(rec.Cits[1].CallNums[2].Media);
            Assert.AreEqual("number3", rec.Cits[1].CallNums[3].Number);
            Assert.IsNull(rec.Cits[1].CallNums[3].Media);
            Assert.AreEqual(1, rec.Cits[1].Notes.Count);
            Assert.AreEqual("repo note", rec.Cits[1].Notes[0].Text);
        }

        [Test]
        public void Sour_Rfn()
        {
            // TODO Non-standard tag. Apparently exists from FamilyTreeMaker? See Gene.Genie.
            // 20181025 Current behavior is to store as an 'unknown'.
            var txt = "0 @S1@ SOUR\n1 RFN blah\n";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            SourceRecord rec = res[0] as SourceRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].Beg);
        }

        [Test]
        public void Repo_Caln()
        {
            // TODO Non-standard tag. Apparently exists from FamilyTreeMaker? See Gene.Genie. Should convert to standard?
            // 20181025 Current behavior is to store as an 'unknown'.
            var txt = "0 @S1@ SOUR\n1 CALN blah\n";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            SourceRecord rec = res[0] as SourceRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].Beg);

            // TODO consider?
            //Assert.AreEqual(1,rec.Cits.Count);
            //Assert.AreEqual(1, rec.Cits[0].CallNums.Count);
            //Assert.AreEqual("blah", rec.Cits[0].CallNums[0].Number);
        }

    }
}
