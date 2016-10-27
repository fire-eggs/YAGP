using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local

// TODO RESN variations
// TODO specifying HUSB, WIFE more than once
// TODO specifying NCHI more than once
// TODO specifying RESN more than once
// TODO SLGS: note, source cite, STAT.DATE

// TODO REFN -> common testing
// TODO NOTE -> common testing
// TODO SOUR -> common testing
// TODO OBJE -> common testing

// TODO test _MREL, _FREL from 'AGES'
//0 @F17299@ FAM\n1 HUSB @I4235@\n1 WIFE @I4236@\n1 CHIL @I7431@\n2 _FREL step\n2 _MREL step\n1 CHIL @I7432@\n2 _FREL step\n2 _MREL step\n1 MARR

// TODO _STAT


namespace SharpGEDParser.Tests
{
    // Basic FAM record testing

    [TestFixture]
    class FamTest1 : GedParseTest
    {
        private FamRecord parse(string val)
        {
            return parse<FamRecord>(val);
        }

        [Test]
        public void TestFam()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 WIFE @p2@";
            var rec = parse(fam);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);
        }

        [Test]
        public void TestFam2()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 WIFE @p2@\n1 CHIL @p3@\n1 CHIL @p4@\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(2, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            Assert.AreEqual("p4", rec.Childs[1]);
            Assert.AreEqual("2", rec.RIN);
        }

        [Test]
        public void TestFam1()
        {
            // simpliest valid family record
            // TODO is this truly valid? isn't one family event required? see FAMILY_EVENT_STRUCTURE
            string fam = "0 @F1@ FAM";
            var rec = parse(fam);
            Assert.AreEqual("F1", rec.Ident);
            Assert.AreEqual(0, rec.Errors.Count);
        }

        private FamRecord TestIdentErr(string dadIdent, string momIdent, string kidIdent, int expectedErrCount)
        {
            string fam = string.Format("0 @F1@ FAM\n1 HUSB{0}\n1 WIFE{1}\n1 CHIL{2}", dadIdent, momIdent, kidIdent);
            var rec = parse(fam);
            Assert.AreEqual(expectedErrCount, rec.Errors.Count);
            return rec;
        }

        [Test]
        public void TestDadIdentErrs()
        {
            var rec = TestIdentErr("", " @p2@", " @p3@", 1);
            Assert.AreEqual(null, rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            rec = TestIdentErr(" @", " @p2@", " @p3@", 1);
            Assert.AreEqual(null, rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            rec = TestIdentErr(" @p1", " @p2@", " @p3@", 1); // TODO is this correct? unterminated ident?
            rec = TestIdentErr(" ", " @p2@", " @p3@", 1);
        }

        [Test]
        public void TestMomIdentErrs()
        {
            var rec = TestIdentErr(" @p1@", "", " @p3@", 1);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual(null, rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            rec = TestIdentErr(" @p1@", " @", " @p3@", 1);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual(null, rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            rec = TestIdentErr(" @p1@", " @p2", " @p3@", 1); // TODO is this correct? unterminated ident?
            rec = TestIdentErr(" @p1@", " ", " @p3@", 1);
        }

        [Test]
        public void TestKidIdentErrs()
        {
            var rec = TestIdentErr(" @p1@", " @p2@", "", 1);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);
            var rec2 = TestIdentErr(" @p1@", " @p2@", " @", 1);
            var rec3 = TestIdentErr(" @p1@", " @p2@", " @p3", 1); // TODO is this correct? unterminated ident?
            var rec4 = TestIdentErr(" @p1@", " @p2@", " ", 1);
        }

        [Test]
        public void TestFamNote()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 NOTE @N123@";
            var rec = parse(fam);
            Assert.AreNotEqual(null, rec.Notes);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("N123", rec.Notes[0].Xref);
        }

        [Test]
        public void TestFamNote2()
        {
            string fam = "0 @F1@ FAM\n1 NOTE This is a family record note\n2 CONT blah blah";
            var rec = parse(fam);
            Assert.AreNotEqual(null, rec.Notes);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("This is a family record note\nblah blah", rec.Notes[0].Text);
        }

        [Test]
        public void FamResn()
        {
            string fam = "0 @F1@ FAM\n1 RESN locked";
            var rec = parse(fam);
            Assert.AreEqual("locked", rec.Restriction);
        }

        [Test]
        public void FamNChild()
        {
            string fam = "0 @F1@ FAM\n1 NCHI 1";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.ChildCount);

            fam = "0 @F1@ FAM\n1 NCHI ";
            rec = parse(fam);
            Assert.AreEqual(-1, rec.ChildCount);
            Assert.AreEqual(1, rec.Errors.Count); // TODO error details

            fam = "0 @F1@ FAM\n1 NCHI A";
            rec = parse(fam);
            Assert.AreEqual(-1, rec.ChildCount);
            Assert.AreEqual(1, rec.Errors.Count); // TODO error details
        }

        [Test]
        public void TestSimpleSour()
        {
            string txt = "0 @F1@ FAM\n1 SOUR @S1@\n1 NCHI 1";
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident);
            Assert.AreEqual(1, rec.ChildCount);

            Assert.AreEqual(0, rec.Errors.Count, "No error");

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("S1", rec.Cits[0].Xref);
        }

        [Test]
        public void TestSimpleSourErr()
        {
            string txt = "0 @F1@ FAM\n1 SOUR @S1@\n2 TEXT 3\n1 NCHI 1";
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident);
            Assert.AreEqual(1, rec.ChildCount);

            Assert.AreEqual(1, rec.Errors.Count, "Error expected");

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("S1", rec.Cits[0].Xref);
        }

        [Test]
        public void TestFamSubm()
        {
            string fam = "0 @F1@ FAM\n1 SUBM @p1@\n1 WIFE @p2@";
            var rec = parse(fam);
            Assert.AreEqual("p1", rec.FamSubm[0]);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);

            fam = "0 @F1@ FAM\n1 SUBM @s1@\n1 WIFE @w2@\n1 SUBM @s2@";
            rec = parse(fam);
            Assert.AreEqual("s1", rec.FamSubm[0]);
            Assert.AreEqual("s2", rec.FamSubm[1]);
            Assert.AreEqual("w2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);
        }

        [Test]
        public void TestErrorId()
        {
            var txt = "0 @ @ FAM\n1 HUSB @h1@";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as FamRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test]
        public void TestErrorId2()
        {
            var txt = "0 @@ FAM\n1 HUSB @h1@";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as FamRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test]
        public void TestErrorId3()
        {
            // Nested '@' breaking ident parsing
            var txt = "0 @@@ FAM\n1 HUSB @h1@";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as FamRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test]
        public void TestMissingId()
        {
            // empty record; missing id
            var txt = "0 FAM";
            var res = ReadItHigher(txt);
            // TODO 'empty record' disabled intentionally: valid?
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test]
        public void TestMissingId2()
        {
            // missing id
            var txt = "0 FAM\n1 HUSB @h1@";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as FamRecord;
            Assert.IsNotNull(rec);
        }

        [Test]
        public void FamSLGS()
        {
            // Simple - no note, source citation or STAT.DATE
            var txt = "0 @F1@ FAM\n1 SLGS\n2 DATE Bogus\n2 TEMP temple\n2 PLAC the place\n2 STAT status";
            var rec = parse(txt);
            Assert.AreEqual("F1", rec.Ident);
            Assert.AreEqual(1, rec.LDSEvents.Count);
            var evt = rec.LDSEvents[0];
            Assert.AreEqual("Bogus", evt.Date);
            Assert.AreEqual("temple", evt.Temple);
            Assert.AreEqual("the place", evt.Place);
            Assert.AreEqual("status", evt.Status);
        }
    }
}
