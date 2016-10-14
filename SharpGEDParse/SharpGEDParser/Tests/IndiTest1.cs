using NUnit.Framework;
using SharpGEDParser.Model;

// TODO a surname with space? e.g. "von neumann"? extra spaces?

// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiTest1 : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        [Test]
        public void TestMultiFam()
        {
            var indi2 = "0 @PERSON3@ INDI\n1 FAMS @FAMILY2@\n1 NAME /Child 1/\n1 FAMC @FAMILY1@\n1 FAMS @FAMILY3@\n1 FAMC @FAMILY4@";
            var rec = parse(indi2);
            Assert.AreEqual(2, rec.ChildLinks.Count);
            Assert.AreEqual("FAMILY1", rec.ChildLinks[0]);
            Assert.AreEqual("FAMILY4", rec.ChildLinks[1]);
            Assert.AreEqual(2, rec.FamLinks.Count);
            Assert.AreEqual("FAMILY2", rec.FamLinks[0]);
            Assert.AreEqual("FAMILY3", rec.FamLinks[1]);
        }

        [Test]
        public void TestNoName()
        {
            var indi1 = "0 INDI\n1 SEX";
            var rec = parse(indi1);
            Assert.AreEqual(0, rec.Names.Count);
        }

        [Test]
        public void TestSexF()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX F";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Feminine";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Female";

            var rec = parse(indiU1);
            Assert.AreEqual('F', rec.Sex);
            Assert.AreEqual("F", rec.FullSex);

            rec = parse(indiU2);
            Assert.AreEqual('F', rec.Sex);
            Assert.AreEqual("Feminine", rec.FullSex);

            rec = parse(indiU3);
            Assert.AreEqual('F', rec.Sex);
            Assert.AreEqual("Female", rec.FullSex);
        }

        [Test]
        public void TestSexM()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX M";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Masculine";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Male";

            var rec = parse(indiU1);
            Assert.AreEqual('M', rec.Sex);

            rec = parse(indiU2);
            Assert.AreEqual('M', rec.Sex);

            rec = parse(indiU3);
            Assert.AreEqual('M', rec.Sex);
        }

        [Test]
        public void TestSexU()
        {
            var indi1 = "0 INDI\n1 SEX";
            var indiU1 = "0 INDI\n1 NAME kludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX U";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Gibber";

            var rec = parse(indi1);
            Assert.AreEqual('U', rec.Sex);
            rec = parse(indiU1);
            Assert.AreEqual('U', rec.Sex);
            rec = parse(indiU2);
            Assert.AreEqual('U', rec.Sex);
            rec = parse(indiU3);
            Assert.AreEqual('U', rec.Sex, "Failed to translate to U");
        }

        [Test]
        public void TestMethod1()
        {
            var simpleInd = "0 @I1@ INDI\n1 NAME One /Note/\n2 SURN Note\n2 GIVN One\n1 NOTE First line of a note.\n2 @IDENT@ CONT Second line of a note.\n2 CONT Third line of a note.";
            var rec = parse(simpleInd);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Names.Count);
        }


        [Test]
        public void FamcXrefMissing()
        {
            var indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.ChildLinks.Count);

            indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC blah";
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.ChildLinks.Count);

            indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC blah @@";
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.ChildLinks.Count);
        }

        [Test]
        public void FamsXrefMissing()
        {
            var indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS";
            var rec = parse(indi);
            Assert.AreEqual(0, rec.FamLinks.Count);
            Assert.AreEqual(1, rec.Errors.Count);

            indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS blah";
            rec = parse(indi);
            Assert.AreEqual(0, rec.FamLinks.Count);
            Assert.AreEqual(1, rec.Errors.Count);

            indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS @@";
            rec = parse(indi);
            Assert.AreEqual(0, rec.FamLinks.Count);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [Test]
        public void TestRestriction()
        {
            var indi = "0 INDI\n1 RESN";
            var rec = parse(indi);
            Assert.AreEqual("", rec.Restriction);

            indi = "0 INDI\n1 RESN locked";
            rec = parse(indi);
            Assert.AreEqual("locked", rec.Restriction);

            indi = "0 INDI\n1 RESN gibber";
            rec = parse(indi);
            Assert.AreEqual("gibber", rec.Restriction);

            indi = "0 INDI\n1 RESN       privacy     ";
            rec = parse(indi);
            Assert.AreEqual("privacy", rec.Restriction);
        }

        [Test]
        public void TestNote()
        {
            var indi = "0 INDI\n1 NOTE @N123@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("N123", rec.Notes[0].Xref);

            indi = "0 INDI\n1 NOTE notes\n2 CONT more detail";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);

            indi = "0 INDI\n1 NOTE notes\n2 CONT more detail\n1 NAME foo\n1 NOTE notes2";
            rec = parse(indi);
            Assert.AreEqual(2, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
            Assert.AreEqual("notes2", rec.Notes[1].Text);

            // trailing space must be preserved
            indi = "0 INDI\n1 NOTE notes\n2 CONC more detail \n2 CONC yet more detail";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notesmore detail yet more detail", rec.Notes[0].Text);

            indi = "0 INDI\n1 NOTE notes \n2 CONC more detail \n2 CONC yet more detail ";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes more detail yet more detail ", rec.Notes[0].Text);
        }

        [Test]
        public void TestId()
        {
            var indi1 = "0   @1@   INDI  extra\n1 SEX";
            var indi2 = "0 @@ INDI\n1 SEX";
            var indi3 = "0   @VERYLONGPERSONID@   INDI  extra\n1 SEX";
            var rec = parse(indi1);
            Assert.AreEqual("1", rec.Ident);
            rec = parse(indi2);
            Assert.AreEqual("", rec.Ident);
            rec = parse(indi3);
            Assert.AreEqual("VERYLONGPERSONID", rec.Ident);
        }

        [Test]
        public void TestLiving()
        {
            var indi = "0 INDI\n1 CHAN";
            var rec = parse(indi);
            Assert.IsFalse(rec.Living);

            indi = "0 INDI\n1 LVG Gibber";
            rec = parse(indi);
            Assert.IsTrue(rec.Living);
            indi = "0 INDI\n1 LVG";
            rec = parse(indi);
            Assert.IsTrue(rec.Living);
            indi = "0 INDI\n1 LVNG Gibber";
            rec = parse(indi);
            Assert.IsTrue(rec.Living);
            indi = "0 INDI\n1 LVNG";
            rec = parse(indi);
            Assert.IsTrue(rec.Living);
        }

    }
}
