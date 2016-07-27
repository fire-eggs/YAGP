// TODO common testing CHAN, REFN, RIN ... should move to own test, exercised for all records

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
    // TODO custom under sub-tag (DATA, OBJE, NOTE, REPO, TEXT, TITL, PUBL, ABBR)
    // TODO DATA/EVEN/DATE requires additional parsing/validation
    // TODO extra text "0 @S1@ SOUR\n1 DATA blah blah\n"
    
    // TODO missing newline confused parsing: a legit, necessary test

    // TODO source repository citation(s)
    // TODO multiple EVEN
    // TODO multimedia link(s)

    // TODO deep test: SOUR, DATA+NOTE, NOTE, REPO+NOTE, NOTE, CHAN+NOTE, NOTE

    // Testing for SOURCE records
    [TestFixture]
    class SourceTest : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public new static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        public static SourceRecord ReadOne(string teststring)
        {
            var res = ReadIt(teststring);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as SourceRecord;
            Assert.IsNotNull(rec);
            return rec;
        }

        [Test]
        public void TestBasic()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred";
            var rec = ReadOne(txt);

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
            var txt = "0 @N1@ SOUR blah blah blah\n1 _CUST foobar\n1 RIN foobar";
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
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
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
            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
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
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
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
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }
        #endregion REFN

        // DATA/EVEN/EVEN
        // DATA/NOTE/NOTE
        // DATA/EVEN/NOTE/EVEN/NOTE
        // SOUR/NOTE
        // SOUR/OBJE
        // SOUR/1 DATA/NOTE/1 NOTE/1 OBJE
        // SOUR/1 DATA/NOTE/1 NOTE/1 OBJE/1 NOTE/1 OBJE/1 CHAN/2 NOTE

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
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN things happened\n2 EVEN more happenings\n1 TEXT Use your loaf";
            var rec = ReadOne(txt);

            Assert.AreEqual("S1", rec.Ident);
            Assert.AreEqual("Use your loaf", rec.Text);
            Assert.IsNotNull(rec.Data);
            Assert.AreEqual(2, rec.Data.Events.Count);
            Assert.AreEqual("things happened", rec.Data.Events[0].Text);
            Assert.AreEqual("more happenings", rec.Data.Events[1].Text);
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
    }
}
