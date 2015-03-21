using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [TestClass]
    public class IndiTest : GedParseTest
    {
        private KBRGedIndi parse(string val)
        {
            return parse<KBRGedIndi>(val, "INDI");
        }

        [TestMethod]
        public void TestMethod1()
        {
            var simpleInd = "0 @I1@ INDI\n1 NAME One /Note/\n2 SURN Note\n2 GIVN One\n1 NOTE First line of a note.\n2 @IDENT@ CONT Second line of a note.\n2 CONT Third line of a note.";
            var rec = parse(simpleInd);
            Assert.AreEqual('U', rec.Sex);
            Assert.AreEqual(1, rec.Names.Count);
        }

        [TestMethod]
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

        [TestMethod]
        public void TestSexM()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX M\n0TestKludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Masculine\n0TestKludge";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Male\n0TestKludge";

            var rec = parse(indiU1);
            Assert.AreEqual('M', rec.Sex);

            rec = parse(indiU2);
            Assert.AreEqual('M', rec.Sex);

            rec = parse(indiU3);
            Assert.AreEqual('M', rec.Sex);
        }

        [TestMethod]
        public void TestSexF()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX F\n0TestKludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Feminine\n0TestKludge";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Female\n0TestKludge";

            var rec = parse(indiU1);
            Assert.AreEqual('F', rec.Sex);

            rec = parse(indiU2);
            Assert.AreEqual('F', rec.Sex);

            rec = parse(indiU3);
            Assert.AreEqual('F', rec.Sex);
        }

        [TestMethod]
        public void TestNoName()
        {
            var indi1 = "0 INDI\n1 SEX";
            var rec = parse(indi1);
            Assert.AreEqual(0, rec.Names.Count);
        }

        [TestMethod]
        public void TestBasicName()
        {
            var indi1 = "0 INDI\n1 NAME  kludge  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual(null, rec.Names[0].Surname);
        }

        [TestMethod]
        public void TestBasicSurname()
        {
            var indi1 = "0 INDI\n1 NAME kludge /clan/";
            var indi2 = "0 INDI\n1 NAME /clan2/";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("", rec.Names[0].Names);
            Assert.AreEqual("clan2", rec.Names[0].Surname);
        }

        [TestMethod]
        public void TestBasicSurname2()
        {
            // real person from a GED, extra spaces
            var indi1 = "0 INDI\n1    NAME     Marjorie    Lee     /Smith/   \n2 GIVN Marjorie Lee\n2 SURN Smith";

            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("Marjorie Lee", rec.Names[0].Names, "Failure to remove extra spaces"); // TODO failure to strip extra spaces
            Assert.AreEqual("Smith", rec.Names[0].Surname);
        }

        [TestMethod]
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

        [TestMethod]
        public void TestMultiName()
        {
            var indi = "0 INDI\n1 NAME John /Smith/\n1 NAME Eric /Jones/";
            var indi2 = "0 INDI\n1 NAME John /Smith/\n1 SEX M\n2 NOTE blah blah\n1 NAME Eric /Jones/";
            var rec = parse(indi);
            Assert.AreEqual(2, rec.Names.Count);
            Assert.AreEqual("Smith", rec.Names[0].Surname);
            Assert.AreEqual("Jones", rec.Names[1].Surname);
            rec = parse(indi2);
            Assert.AreEqual(2, rec.Names.Count);
            Assert.AreEqual("Smith", rec.Names[0].Surname);
            Assert.AreEqual("Jones", rec.Names[1].Surname);
        }

        [TestMethod]
        public void TestSuffix()
        {
            var indi = "0 INDI\n1 NAME Given Name /Smith/ jr ";
            var indi2 = "0 INDI\n1 NAME Given Name /Smith/esq";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("jr", rec.Names[0].Suffix);
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("esq", rec.Names[0].Suffix);
        }

        [TestMethod]
        public void TestFams()
        {
            var indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS @FAMILY1@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.FamLinks.Count);
            Assert.AreEqual("FAMS", rec.FamLinks[0].Tag);
            Assert.AreEqual("FAMILY1", rec.FamLinks[0].XRef);
        }

        [TestMethod]
        public void TestFamc()
        {
            var indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC @FAMILY1@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.ChildLinks.Count);
            Assert.AreEqual("FAMC", rec.ChildLinks[0].Tag);
            Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef);

            // TODO not sure what is going on. is the space an identity terminator?
            var indi1 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC @ FAMILY1 @";
            rec = parse(indi1);
            Assert.AreEqual(1, rec.ChildLinks.Count);
            Assert.AreEqual("FAMC", rec.ChildLinks[0].Tag);
            Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef, "Is space a terminator?");
        }

        [TestMethod]
        public void TestFamsFamc()
        {
            var indi2 = "0 @PERSON3@ INDI\n1 FAMS @FAMILY2@\n1 NAME /Child 1/\n1 FAMC @FAMILY1@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.ChildLinks.Count);
            Assert.AreEqual("FAMC", rec.ChildLinks[0].Tag);
            Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef);
            Assert.AreEqual(1, rec.FamLinks.Count);
            Assert.AreEqual("FAMS", rec.FamLinks[0].Tag);
            Assert.AreEqual("FAMILY2", rec.FamLinks[0].XRef);
        }

        [TestMethod]
        public void TestMultiFam()
        {
            var indi2 = "0 @PERSON3@ INDI\n1 FAMS @FAMILY2@\n1 NAME /Child 1/\n1 FAMC @FAMILY1@\n1 FAMS @FAMILY3@\n1 FAMC @FAMILY4@";
            var rec = parse(indi2);
            Assert.AreEqual(2, rec.ChildLinks.Count);
            Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef);
            Assert.AreEqual("FAMILY4", rec.ChildLinks[1].XRef);
            Assert.AreEqual(2, rec.FamLinks.Count);
            Assert.AreEqual("FAMILY2", rec.FamLinks[0].XRef);
            Assert.AreEqual("FAMILY3", rec.FamLinks[1].XRef);
        }

        [TestMethod]
        public void TestAlias()
        {
            var indi = "0 INDI\n1 ALIA Zettie";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Alia.Count);
            Assert.AreEqual("Zettie", rec.Alia[0].XRef);
            var indi2 = "0 INDI\n1 ALIA John /Doe/  ";
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Alia.Count);
            Assert.AreEqual("John /Doe/", rec.Alia[0].XRef);

            indi2 = "0 INDI\n1 ALIA John /Doe/  \n1 ALIA Jane Doe";
            rec = parse(indi2);
            Assert.AreEqual(2, rec.Alia.Count);
            Assert.AreEqual("John /Doe/", rec.Alia[0].XRef);
            Assert.AreEqual("Jane Doe", rec.Alia[1].XRef);

            // TODO multiple aliases
            // TODO confused about GED syntax here...
        }

        [TestMethod]
        public void TestAnci()
        {
            var indi = "0 INDI\n1 ANCI @SUBM1@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Anci.Count);
            Assert.AreEqual("SUBM1", rec.Anci[0].XRef);
        }
        [TestMethod]
        public void TestDesi()
        {
            var indi = "0 INDI\n1 DESI @SUBM1@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Desi.Count);
            Assert.AreEqual("SUBM1", rec.Desi[0].XRef);
        }
        [TestMethod]
        public void TestSubm()
        {
            var indi = "0 INDI\n1 SUBM @SUBM1@";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Subm.Count);
            Assert.AreEqual("SUBM1", rec.Subm[0].XRef);
        }

        [TestMethod]
        public void TestRestriction()
        {
            var indi = "0 INDI\n1 RESN";
            var rec = parse(indi);
            Assert.AreEqual(null, rec.Restriction);
            
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

        [TestMethod]
        public void TestNote()
        {
            // TODO multiple notes
            //Assert.AreEqual(1, rec.Notes.Count);

            var indi = "0 INDI\n1 NOTE";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Note.Item1);
            Assert.AreEqual(1, rec.Note.Item2);

            // TODO conc/cont

            indi = "0 INDI\n1 NOTE notes\n2 CONT more detail";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Note.Item1);
            Assert.AreEqual(2, rec.Note.Item2);
        }

        [TestMethod]
        public void TestChange()
        {
            // TODO multiple changes possible?

            var indi = "0 INDI\n1 CHAN";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(1, rec.Change.Item2);

            indi = "0 INDI\n1 CHAN notes\n2 DATE blah";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(2, rec.Change.Item2);

            // TODO test actual details
        }

        [TestMethod]
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

        [TestMethod]
        public void TestRFN()
        {
            var indi = "0 INDI\n1 RFN";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("RFN", rec.Data[0].Tag);
            Assert.AreEqual("", rec.Data[0].Data);

            indi = "0 INDI\n1 RFN 2547";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("RFN", rec.Data[0].Tag);
            Assert.AreEqual("2547", rec.Data[0].Data);
        }

        [TestMethod]
        public void TestREFN()
        {
            var indi = "0 INDI\n1 REFN";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("REFN", rec.Data[0].Tag);
            Assert.AreEqual("", rec.Data[0].Data);

            indi = "0 INDI\n1 REFN 2547";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("REFN", rec.Data[0].Tag);
            Assert.AreEqual("2547", rec.Data[0].Data);
        }
        [TestMethod]
        public void TestAFN()
        {
            var indi = "0 INDI\n1 AFN";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("AFN", rec.Data[0].Tag);
            Assert.AreEqual("", rec.Data[0].Data);

            indi = "0 INDI\n1 AFN 2547";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("AFN", rec.Data[0].Tag);
            Assert.AreEqual("2547", rec.Data[0].Data);
        }
        [TestMethod]
        public void TestUID()
        {
            var indi = "0 INDI\n1 _UID";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("_UID", rec.Data[0].Tag);
            Assert.AreEqual("", rec.Data[0].Data);

            // UID value from a real ged file
            indi = "0 INDI\n1 _UID BA8E953A325A1342B5F691F88A6A6E018F33";
            rec = parse(indi);
            Assert.AreEqual(1, rec.Data.Count);
            Assert.AreEqual("_UID", rec.Data[0].Tag);
            Assert.AreEqual("BA8E953A325A1342B5F691F88A6A6E018F33", rec.Data[0].Data);
        }
    }
}
