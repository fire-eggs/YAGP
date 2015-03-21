using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ConvertToConstant.Local

namespace UnitTestProject1
{
    [TestClass]
    public class IndiTest : GedParseTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // NOTE extra trailing '0' record: testing kludge
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

            // TODO multiple aliases
            // TODO confused about GED syntax here...
        }
    }
}
