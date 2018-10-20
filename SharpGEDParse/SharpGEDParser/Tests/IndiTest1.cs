using NUnit.Framework;
using SharpGEDParser.Model;

// TODO a surname with space? e.g. "von neumann"? extra spaces?
// TODO saving of extra, see IndiLinkParse

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
            Assert.AreEqual(4, rec.Links.Count);
            Assert.AreEqual("FAMILY2", rec.Links[0].Xref);
            Assert.AreEqual(IndiLink.FAMS_TYPE, rec.Links[0].Type);
            Assert.AreEqual("FAMILY1", rec.Links[1].Xref);
            Assert.AreEqual(IndiLink.FAMC_TYPE, rec.Links[1].Type);
            Assert.AreEqual("FAMILY3", rec.Links[2].Xref);
            Assert.AreEqual(IndiLink.FAMS_TYPE, rec.Links[2].Type);
            Assert.AreEqual("FAMILY4", rec.Links[3].Xref);
            Assert.AreEqual(IndiLink.FAMC_TYPE, rec.Links[3].Type);
        }

        [Test]
        public void TestNoName()
        {
            var indi1 = "0 INDI\n1 SEX";
            var rec = parse(indi1);
            Assert.AreEqual("INDI", rec.Tag);
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
            Assert.IsNull(rec.FullSex); // default value not stored

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
            // 20180125 invalid FAMC/FAMS xref won't create IndiLink
            var indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(3, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMC_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);

            indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC blah";
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(3, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMC_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);
            //Assert.AreEqual("blah", rec.Links[0].Extra);

            indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC @@";
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(3, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMC_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);
            //Assert.IsEmpty(rec.Links[0].Extra);

            // Make sure the sub-struct is in the error & doesn't botch other tags
            indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC blah\n2 STAT blah\n1 SEX U";
            rec = parse(indi2);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(3, rec.Errors[0].Beg);
            Assert.AreEqual(4, rec.Errors[0].End);
            Assert.AreEqual(0, rec.Links.Count);
        }

        [Test]
        public void FamsXrefMissing()
        {
            // 20180125 missing xref for FAMS/FAMC will not create a link
            var indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(4, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMS_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);
            //Assert.IsEmpty(rec.Links[0].Extra);

            indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS blah";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(4, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMS_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);
            //Assert.AreEqual("blah", rec.Links[0].Extra);

            indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS @@";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(4, rec.Errors[0].Beg);
            Assert.AreEqual(0, rec.Links.Count);
            //Assert.AreEqual(IndiLink.FAMS_TYPE, rec.Links[0].Type);
            //Assert.IsEmpty(rec.Links[0].Xref);
            //Assert.IsEmpty(rec.Links[0].Extra);

            // Make sure the sub-struct is in the error & doesn't botch other tags
            indi = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMS blah\n2 STAT blah\n1 SEX U";
            rec = parse(indi);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(3, rec.Errors[0].Beg);
            Assert.AreEqual(4, rec.Errors[0].End);
            Assert.AreEqual(0, rec.Links.Count);

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

            indi = "0 INDI\n1 RESN       privacy     ";
            rec = parse(indi);
            Assert.AreEqual("privacy", rec.Restriction);

            indi = "0 INDI\n1 RESN       confidential     ";
            rec = parse(indi);
            Assert.AreEqual("confidential", rec.Restriction);
        }

        [Test]
        public void IndiResnErr()
        {
            var indi = "0 @I1@ INDI\n1 RESN gibber";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count); // TODO validate details
            Assert.AreEqual("gibber", rec.Restriction);
        }

        [Test]
        public void IndiResnMulti()
        {
            var indi = "0 @I1@ INDI\n1 RESN locked\n1 RESN gibber";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count); // TODO validate details
            Assert.AreEqual("locked", rec.Restriction);
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
            Assert.IsNull(rec.Ident);
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

        [Test]
        public void Alias()
        {
            var indi = "0 @I1@ INDI\n1 ALIA @I2@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.AliasLinks.Count);
            Assert.AreEqual("I2", rec.AliasLinks[0]);
        }

        [Test]
        public void NonStdAlias()
        {
            var indi = "0 @I1@ INDI\n1 ALIA nickname";
            var rec = parse(indi);
            Assert.AreEqual(0, rec.AliasLinks.Count);
            Assert.AreEqual(1, rec.Attribs.Count);
            var attrib = rec.Attribs[0];
            Assert.AreEqual("ALIA", attrib.Tag);
            Assert.AreEqual("nickname", attrib.Descriptor);

            Assert.AreEqual(1, rec.Errors.Count);
        }

        [Test]
        public void ObjeXref()
        {
            var indi = "0 @I1@ INDI\n1 OBJE @o1@\n1 SEX U";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Media.Count);
            Assert.AreEqual("o1", rec.Media[0].Xref);
            Assert.IsNull(rec.Media[0].Title);
        }

        [Test]
        public void ObjeXref2()
        {
            // TODO this should be an error: invalid xref id
            var indi = "0 @I1@ INDI\n1 OBJE gibber\n1 SEX U";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Media.Count);
            Assert.IsNull(rec.Media[0].Xref);
            Assert.IsNull(rec.Media[0].Title);
        }

        [Test]
        public void ObjeXref3()
        {
            // TODO this should be an error: invalid xref id
            var indi = "0 @I1@ INDI\n1 OBJE @gibber\n1 SEX U";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Media.Count);
            Assert.AreEqual("gibber", rec.Media[0].Xref);
            Assert.IsNull(rec.Media[0].Title);
        }

        [Test]
        public void ObjeEmbed()
        {
            // Mutation testing: verify structure sub-parsing correctly
            var indi = "0 @I1@ INDI\n1 OBJE gibber\n2 FILE refn\n1 SEX U";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Media.Count);
            // TODO what happened w/ the extra? Assert.AreEqual("gibber", rec.Media[0].Xref);
            Assert.IsNull(rec.Media[0].Title);
            Assert.AreEqual(1, rec.Media[0].Files.Count);
            Assert.AreEqual("refn", rec.Media[0].Files[0].FileRefn);
        }

        [Test]
        public void ObjeMultFile()
        {
            // Mutation testing: verify structure sub-parsing correctly
            var indi = "0 @I1@ INDI\n1 OBJE gibber\n2 FILE refn\n2 FILE refn2\n2 FILE refn3\n1 SEX U";
            var rec = parse(indi);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Media.Count);
            // TODO what happened w/ the extra? Assert.AreEqual("gibber", rec.Media[0].Xref);
            Assert.IsNull(rec.Media[0].Title);
            Assert.AreEqual(3, rec.Media[0].Files.Count);
            Assert.AreEqual("refn", rec.Media[0].Files[0].FileRefn);
            Assert.AreEqual("refn2", rec.Media[0].Files[1].FileRefn);
            Assert.AreEqual("refn3", rec.Media[0].Files[2].FileRefn);
        }

    }
}
