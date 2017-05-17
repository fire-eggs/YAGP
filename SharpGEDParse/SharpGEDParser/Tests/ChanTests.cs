using NUnit.Framework;
using SharpGEDParser.Model;
using System;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    // Common testing for CHAN - applies to all 'top-level' records with the same syntax
    [TestFixture]
    public class ChanTests : GedParseTest
    {
        // Simple CHAN test for a given record
        private GEDCommon TestChanM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));

            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date), tag);
            return rec;
        }

        [Test]
        public void TestChan()
        {
            var txt = "0 @A1@ {0}\n1 CHAN\n2 DATE 1 APR 2000";
            TestChanM(txt, "NOTE");
            TestChanM(txt, "SOUR");
            TestChanM(txt, "REPO");
            TestChanM(txt, "OBJE");
            TestChanM(txt, "INDI");
            TestChanM(txt, "FAM");
//            TestChanM(txt, "HEAD");
//            TestChanM(txt, "SUBM");
//            TestChanM(txt, "SUBN");
        }

        // No date test for a given record
        private GEDCommon TestNoDateM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            Assert.AreNotEqual(0, rec.Errors.Count, tag); // TODO validate error detail
            return rec;
        }

        [Test]
        public void TestNoDate()
        {
            // no date for chan
            var txt = "0 @A1@ {0}\n1 CHAN";
            TestNoDateM(txt, "NOTE");
            TestNoDateM(txt, "REPO");
            TestNoDateM(txt, "SOUR");
            TestNoDateM(txt, "OBJE");
            TestNoDateM(txt, "INDI");
            TestNoDateM(txt, "FAM");
            //            TestNoDateM(txt, "HEAD");
        }

        private GEDCommon TestAfterM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));

            // Assert.AreEqual(0, rec.Errors.Count, tag);  TODO record specific errors
            Assert.AreEqual(0, rec.Unknowns.Count, tag);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date), tag);
            Assert.AreEqual("fumbar", rec.RIN, tag);
            return rec;
        }

        [Test]
        public void TestFollow()
        {
            // Verify that tags after CHAN are read
            var txt = "0 @A1@ {0}\n1 CHAN\n2 DATE 1 APR 2000\n1 RIN fumbar";
            TestAfterM(txt, "NOTE");
            TestAfterM(txt, "SOUR");
            TestAfterM(txt, "REPO");
            TestAfterM(txt, "OBJE");
            TestAfterM(txt, "INDI");
            TestAfterM(txt, "FAM");
            //            TestAfterM(txt, "HEAD");
        }

        private GEDCommon NoDateFollowM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            
            Assert.AreNotEqual(0, rec.Errors.Count, tag); // TODO validate error detail
            Assert.AreEqual(0, rec.Unknowns.Count, tag);
            Assert.AreEqual("fumbar", rec.RIN, tag);
            return rec;
        }

        [Test]
        public void TestChan4()
        {
            // no date value, with follow-on tag
            var txt = "0 @A1@ {0}\n1 CHAN\n2 DATE\n1 RIN fumbar";
            NoDateFollowM(txt, "OBJE");
            NoDateFollowM(txt, "REPO");
            NoDateFollowM(txt, "SOUR");
            NoDateFollowM(txt, "NOTE");
            NoDateFollowM(txt, "INDI");
            NoDateFollowM(txt, "FAM");
            //            TestChanM(txt, "HEAD");
        }

        private GEDCommon ChanExtraM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            Assert.AreNotEqual(0, rec.Errors.Count, tag); // TODO validate error detail
            Assert.AreEqual(0, rec.Unknowns.Count, tag);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count, tag);
            Assert.AreEqual("fumbar", rec.RIN, tag);
            return rec;
        }

        [Test]
        public void TestChan5()
        {
            // extra
            var txt = "0 @N1@ {0}\n1 CHAN\n2 CUSTOM foo\n1 RIN fumbar";
            ChanExtraM(txt, "REPO");
            ChanExtraM(txt, "NOTE");
            ChanExtraM(txt, "SOUR");
            ChanExtraM(txt, "OBJE");
            ChanExtraM(txt, "INDI");
            ChanExtraM(txt, "FAM");
            //            ChanExtraM(txt, "HEAD");
        }

        [Test]
        public void TestChan6() // TODO test all supported records?
        {
            // multi line extra
            var txt = "0 @N1@ SOUR\n1 CHAN\n2 CUSTOM foo\n3 _BLAH bar\n1 RIN fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual("fumbar", rec.RIN);
            Assert.AreEqual(1, chan.OtherLines.Count);
            Assert.AreEqual(2, chan.OtherLines[0].LineCount);
        }

        [Test]
        public void TestChan7() // TODO test all supported records?
        {
            // multiple CHAN
            var txt = "0 @N1@ SOUR\n1 CHAN\n2 DATE 1 MAR 2000\n1 RIN fumbar\n1 CHAN";
            var rec = ReadOne(txt);

            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreNotEqual(0, rec.Errors[0].Error); // Mutation testing
            Assert.AreNotEqual(-1, rec.Errors[0].Beg); // Mutation testing
            Assert.AreNotEqual(-1, rec.Errors[0].End); // Mutation testing
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
            Assert.AreEqual("fumbar", rec.RIN);
        }

        [Test]
        public void OtherDateFormat1()
        {
            // TODO ambiguous date - depends on locale
            var txt = "0 @N1@ SOUR\n1 CHAN\n2 DATE 03/01/2000\n1 RIN fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
            Assert.AreEqual("fumbar", rec.RIN);
        }

        [Test]
        public void OtherDateFormat2()
        {
            var txt = "0 @N1@ SOUR\n1 CHAN\n2 DATE 20000301\n1 RIN fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreNotEqual(0, rec.Errors[0].Error); // Mutation testing
            Assert.AreNotEqual(-1, rec.Errors[0].Beg); // Mutation testing
            Assert.AreNotEqual(-1, rec.Errors[0].End); // Mutation testing
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.RIN);
        }

        [Test]
        public void TimeRec() // TODO test with all supported record types?
        {
            // TODO broken: throwing TIME record away
            // TIME record treated as 'other'
            var txt = "0 @N1@ SOUR\n1 CHAN\n2 DATE 1 MAR 2000\n3 TIME 13:24\n1 RIN fumbar";
            var rec = ReadOne(txt);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
            Assert.AreEqual(1, rec.CHAN.OtherLines.Count);
            Assert.AreEqual("fumbar", rec.RIN);
        }

        #region CHAN+NOTE

        private GEDCommon ChanNoteM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date), tag);
            Assert.AreEqual(1, res2.Notes.Count, tag);
            Assert.AreEqual("N1", res2.Notes[0].Xref, tag);
            return rec;
        }

        [Test]
        public void TestChanNote()
        {
            // CHAN with NOTE
            var txt = "0 @A1@ {0}\n1 CHAN\n2 NOTE @N1@\n2 DATE 1 APR 2000";
            ChanNoteM(txt, "NOTE");
            ChanNoteM(txt, "SOUR");
            ChanNoteM(txt, "OBJE");
            ChanNoteM(txt, "REPO");
            ChanNoteM(txt, "FAM");
            ChanNoteM(txt, "INDI");
        }

        private GEDCommon MultiNoteM(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date), tag);
            Assert.AreEqual(1, res2.Notes.Count, tag);
            Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text, tag);
            return rec;
        }

        [Test]
        public void TestChanNote2()
        {
            // CHAN with multi-line note - CONT
            var txt = "0 @A1@ {0}\n1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000";
            MultiNoteM(txt, "REPO");
            MultiNoteM(txt, "OBJE");
            MultiNoteM(txt, "SOUR");
            MultiNoteM(txt, "NOTE");
            MultiNoteM(txt, "FAM");
            MultiNoteM(txt, "INDI");
        }

        public GEDCommon MultiNote2M(string teststring, string tag)
        {
            var rec = ReadOne(string.Format(teststring, tag));
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date), tag);
            Assert.AreEqual(1, res2.Notes.Count, tag);
            Assert.AreEqual("notes more detail", res2.Notes[0].Text, tag);
            Assert.AreEqual("foebar", rec.RIN);
            Assert.AreEqual("A1", rec.Ident); // TODO replicate?
            return rec;
        }

        [Test]
        public void TestChanNote3()
        {
            // CHAN with multi-line note - CONC
            var txt = "0 @A1@ {0}\n1 CHAN\n2 NOTE notes \n3 CONC more detail\n2 DATE 1 APR 2000\n1 RIN foebar";
            var rec = MultiNote2M(txt, "SOUR");
            Assert.IsNotNull(rec as SourceRecord); // TODO replicate?
            rec = MultiNote2M(txt, "NOTE");
            Assert.IsNotNull(rec as NoteRecord);
            rec = MultiNote2M(txt, "REPO");
            Assert.IsNotNull(rec as Repository);
            rec = MultiNote2M(txt, "OBJE");
            Assert.IsNotNull(rec as MediaRecord);
            rec = MultiNote2M(txt, "FAM");
            Assert.IsNotNull(rec as FamRecord);
            rec = MultiNote2M(txt, "INDI");
            Assert.IsNotNull(rec as IndiRecord);
        }
        #endregion
    }
}
