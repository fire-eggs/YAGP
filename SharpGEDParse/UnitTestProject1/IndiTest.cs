using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class IndiTest : GedParseTest
    {
        // TODO a surname with space? e.g. "von neumann"? extra spaces?

        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val, "INDI");
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

        //[TestMethod]
        //public void TestFams()
        //{
        //    var indi = "0 @PERSON2@ INDI\n1 NAME /Wife/\n1 SEX F\n1 FAMS @FAMILY1@";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.FamLinks.Count);
        //    Assert.AreEqual("FAMS", rec.FamLinks[0].Tag);
        //    Assert.AreEqual("FAMILY1", rec.FamLinks[0].XRef);
        //}

        //[TestMethod]
        //public void TestFamc()
        //{
        //    var indi2 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC @FAMILY1@";
        //    var rec = parse(indi2);
        //    Assert.AreEqual(1, rec.ChildLinks.Count);
        //    Assert.AreEqual("FAMC", rec.ChildLinks[0].Tag);
        //    Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef);

        //    // xref identifier cannot start with a space.
        //    var indi1 = "0 @PERSON3@ INDI\n1 NAME /Child 1/\n1 FAMC @ FAMILY1 @";
        //    rec = parse(indi1);
        //    Assert.AreEqual(0, rec.ChildLinks.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

        //[TestMethod]
        //public void TestFamsFamc()
        //{
        //    var indi2 = "0 @PERSON3@ INDI\n1 FAMS @FAMILY2@\n1 NAME /Child 1/\n1 FAMC @FAMILY1@";
        //    var rec = parse(indi2);
        //    Assert.AreEqual(1, rec.ChildLinks.Count);
        //    Assert.AreEqual("FAMC", rec.ChildLinks[0].Tag);
        //    Assert.AreEqual("FAMILY1", rec.ChildLinks[0].XRef);
        //    Assert.AreEqual(1, rec.FamLinks.Count);
        //    Assert.AreEqual("FAMS", rec.FamLinks[0].Tag);
        //    Assert.AreEqual("FAMILY2", rec.FamLinks[0].XRef);
        //}

        //[TestMethod]
        //public void TestAlias()
        //{
        //    var indi = "0 INDI\n1 ALIA Zettie";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Alia.Count);
        //    Assert.AreEqual("Zettie", rec.Alia[0].XRef);
        //    var indi2 = "0 INDI\n1 ALIA John /Doe/  ";
        //    rec = parse(indi2);
        //    Assert.AreEqual(1, rec.Alia.Count);
        //    Assert.AreEqual("John /Doe/", rec.Alia[0].XRef);

        //    indi2 = "0 INDI\n1 ALIA John /Doe/  \n1 ALIA Jane Doe";
        //    rec = parse(indi2);
        //    Assert.AreEqual(2, rec.Alia.Count);
        //    Assert.AreEqual("John /Doe/", rec.Alia[0].XRef);
        //    Assert.AreEqual("Jane Doe", rec.Alia[1].XRef);

        //    // TODO confused about GED syntax here...
        //}

        //[TestMethod]
        //public void TestAnci()
        //{
        //    var indi = "0 INDI\n1 ANCI @SUBM1@";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Anci.Count);
        //    Assert.AreEqual("SUBM1", rec.Anci[0].XRef);

        //    indi = "0 INDI\n1 ANCI @SUBM2@\n1 ANCI @SUBM3@";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Anci.Count);
        //    Assert.AreEqual("SUBM2", rec.Anci[0].XRef);
        //    Assert.AreEqual("SUBM3", rec.Anci[1].XRef);
        //}

        //[TestMethod]
        //public void AnciXrefMissing()
        //{
        //    var indi = "0 INDI\n1 ANCI";
        //    var rec = parse(indi);
        //    Assert.AreEqual(0, rec.Anci.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ANCI ";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Anci.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ANCI xref";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Anci.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ANCI xref @@";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Anci.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

        //[TestMethod]
        //public void TestDesi()
        //{
        //    var indi = "0 INDI\n1 DESI @SUBM1@";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Desi.Count);
        //    Assert.AreEqual("SUBM1", rec.Desi[0].XRef);

        //    indi = "0 INDI\n1 DESI @SUBM2@\n1 DESI @SUBM3@";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Desi.Count);
        //    Assert.AreEqual("SUBM2", rec.Desi[0].XRef);
        //    Assert.AreEqual("SUBM3", rec.Desi[1].XRef);
        //}

        //[TestMethod]
        //public void DesiXrefMissing()
        //{
        //    var indi = "0 INDI\n1 DESI";
        //    var rec = parse(indi);
        //    Assert.AreEqual(0, rec.Desi.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 DESI ";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Desi.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 DESI xref";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Desi.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 DESI xref @@";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Desi.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

//        [TestMethod]
//        public void TestSubm()
//        {
//            var indi = "0 INDI\n1 SUBM @SUBM1@";
//            var rec = parse(indi);
//            Assert.AreEqual(1, rec.Subm.Count);
//            Assert.AreEqual("SUBM1", rec.Subm[0].XRef);

//            indi = "0 INDI\n1 SUBM @SUBM2@\n1 SUBM @SUBM3@";
//            rec = parse(indi);
//            Assert.AreEqual(2, rec.Subm.Count);
//            Assert.AreEqual("SUBM2", rec.Subm[0].XRef);
//            Assert.AreEqual("SUBM3", rec.Subm[1].XRef);

//            // TODO some real geds not using xref format
//            indi = "0 INDI\n1 SUBM Jane Doe";
//            rec = parse(indi);
//            Assert.AreEqual(0, rec.Subm.Count);
//            Assert.AreEqual(1, rec.Errors.Count);
////            Assert.AreEqual("Jane Doe", rec.Subm[0].XRef, "Some GEDs not using xref format");
//        }

        //[TestMethod]
        //public void SubmXrefMissing()
        //{
        //    var indi = "0 INDI\n1 SUBM";
        //    var rec = parse(indi);
        //    Assert.AreEqual(0, rec.Subm.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 SUBM ";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Subm.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 SUBM xref";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Subm.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 SUBM @@";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Subm.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

        //[TestMethod]
        //public void TestChange()
        //{
        //    var indi = "0 INDI\n1 CHAN";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Change.Item1);
        //    Assert.AreEqual(1, rec.Change.Item2);

        //    indi = "0 INDI\n1 CHAN notes\n2 DATE blah";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Change.Item1);
        //    Assert.AreEqual(2, rec.Change.Item2);

        //    // Only 1 change record allowed
        //    // Gedcom spec says take the FIRST one
        //    indi = "0 INDI\n1 CHAN notes\n2 DATE blah\n1 CHAN notes2";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Change.Item1);
        //    Assert.AreEqual(2, rec.Change.Item2);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    // TODO test actual details
        //}

        //[TestMethod]
        //public void TestRFN()
        //{
        //    var indi = "0 INDI\n1 RFN";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RFN", rec.Data[0].Tag);
        //    Assert.AreEqual("", rec.Data[0].Data);

        //    indi = "0 INDI\n1 RFN 2547";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);

        //    // GEDCOM spec says to take the first
        //    indi = "0 INDI\n1 RFN 2547\n1 RFN gibber";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

        //[TestMethod]
        //public void TestREFN()
        //{
        //    var indi = "0 INDI\n1 REFN";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("REFN", rec.Data[0].Tag);
        //    Assert.AreEqual("", rec.Data[0].Data);

        //    indi = "0 INDI\n1 REFN 2547";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("REFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);

        //    // GEDCOM spec allows multiples
        //    indi = "0 INDI\n1 REFN 2547\n1 REFN gibber";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Data.Count);
        //    Assert.AreEqual("REFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);
        //    Assert.AreEqual("REFN", rec.Data[1].Tag);
        //    Assert.AreEqual("gibber", rec.Data[1].Data);
        //}

        //[TestMethod]
        //public void TestAFN()
        //{
        //    var indi = "0 INDI\n1 AFN";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("AFN", rec.Data[0].Tag);
        //    Assert.AreEqual("", rec.Data[0].Data);

        //    indi = "0 INDI\n1 AFN 2547";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("AFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);

        //    // GEDCOM spec says to take the first
        //    indi = "0 INDI\n1 AFN 2547\n1 AFN gibber";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("AFN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}

        //[TestMethod]
        //public void TestUID()
        //{
        //    // NOTE not a standard tag, dunno if multiples allowed. Code currently supports.

        //    var indi = "0 INDI\n1 _UID";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("_UID", rec.Data[0].Tag);
        //    Assert.AreEqual("", rec.Data[0].Data);

        //    // UID value from a real ged file
        //    indi = "0 INDI\n1 _UID BA8E953A325A1342B5F691F88A6A6E018F33";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("_UID", rec.Data[0].Tag);
        //    Assert.AreEqual("BA8E953A325A1342B5F691F88A6A6E018F33", rec.Data[0].Data);

        //    indi = "0 INDI\n1 _UID 1\n1 _UID 2";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Data.Count);
        //    Assert.AreEqual("_UID", rec.Data[0].Tag);
        //    Assert.AreEqual("1", rec.Data[0].Data);
        //    Assert.AreEqual("_UID", rec.Data[1].Tag);
        //    Assert.AreEqual("2", rec.Data[1].Data);

        //}

        //[TestMethod]
        //public void TestRIN()
        //{
        //    var indi = "0 INDI\n1 RIN\n1 NAME foo";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RIN", rec.Data[0].Tag);
        //    Assert.AreEqual("", rec.Data[0].Data);

        //    indi = "0 INDI\n1 RIN 2547";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RIN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);

        //    // GEDCOM spec says to take the first
        //    indi = "0 INDI\n1 RIN 2547\n1 RIN gibber";
        //    rec = parse(indi);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RIN", rec.Data[0].Tag);
        //    Assert.AreEqual("2547", rec.Data[0].Data);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //    // TODO test specific error?
        //}

        //[TestMethod]
        //public void TestAsso()
        //{
        //    var indi = "0 INDI\n1 ASSO @foo@";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Assoc.Count);

        //    indi = "0 INDI\n1 ASSO @foo@\n1 ASSO @bar@";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Assoc.Count);

        //    // TODO test for details - RELA, SOUR, NOTE
        //}

        //[TestMethod]
        //public void AssoXrefMissing()
        //{
        //    var indi = "0 INDI\n1 ASSO";
        //    var rec = parse(indi);
        //    Assert.AreEqual(0, rec.Assoc.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ASSO ";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Assoc.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ASSO xref";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Assoc.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);

        //    indi = "0 INDI\n1 ASSO @@";
        //    rec = parse(indi);
        //    Assert.AreEqual(0, rec.Assoc.Count);
        //    Assert.AreEqual(1, rec.Errors.Count);
        //}


        //[TestMethod]
        //public void TestSource()
        //{
        //    // Source citation and attributes
        //    var indi = "0 INDI\n1 SOUR @S331@\n2 QUAY 3";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Sources.Count);
        //    Assert.AreEqual(1, rec.Sources[0].Beg);
        //    Assert.AreEqual(2, rec.Sources[0].End);
        //    Assert.AreEqual("S331", rec.Sources[0].XRef);
        //    Assert.AreEqual("3", rec.Sources[0].Quay);

        //    indi = "0 INDI\n1 SOUR @S331@\n2 QUAY 3\n1 SOUR @S148@";
        //    rec = parse(indi);
        //    Assert.AreEqual(2, rec.Sources.Count);
        //    Assert.AreEqual("S331", rec.Sources[0].XRef);
        //    Assert.AreEqual("3", rec.Sources[0].Quay);
        //    Assert.AreEqual(1, rec.Sources[0].Beg);
        //    Assert.AreEqual(2, rec.Sources[0].End);
        //    Assert.AreEqual("S148", rec.Sources[1].XRef);
        //    Assert.AreEqual(3, rec.Sources[1].Beg);
        //    Assert.AreEqual(3, rec.Sources[1].End);
        //}

        //[TestMethod]
        //public void TestSource2()
        //{
        //    // extracted from a real GED file
        //    var indi = "0 INDI\n1 SOUR @S333@\n2 QUAY 3\n1 SOUR @S66@\n2 _RIN 6217\n2 PAGE Missouri/Richmond/Ray/ED #150/Page 4A\n2 DATA\n3 TEXT Trigg Street\n4 CONT 86 91 Claypole, Aaron, head, 59\n4 CONT Myrtle C., daughter-in-law, 17, M\n3 DATE 7 JAN 1920\n2 QUAY 3";
        //    var rec = parse(indi);
        //    Assert.AreEqual(2, rec.Sources.Count);
        //    Assert.AreEqual("S333", rec.Sources[0].XRef);
        //    Assert.AreEqual(1, rec.Sources[0].Beg);
        //    Assert.AreEqual(2, rec.Sources[0].End);
        //    Assert.AreEqual("S66", rec.Sources[1].XRef);
        //    Assert.AreEqual(3, rec.Sources[1].Beg);
        //    Assert.AreEqual(11, rec.Sources[1].End);

        //    Assert.AreEqual("3", rec.Sources[0].Quay);

        //    Assert.AreEqual("3", rec.Sources[1].Quay);
        //    Assert.AreEqual("6217", rec.Sources[1].RIN);
        //    Assert.AreEqual("Missouri/Richmond/Ray/ED #150/Page 4A", rec.Sources[1].Page);
        //    Assert.AreEqual("7 JAN 1920", rec.Sources[1].Date);

        //    Assert.AreEqual("Trigg Street\n86 91 Claypole, Aaron, head, 59\nMyrtle C., daughter-in-law, 17, M", rec.Sources[1].Text);
        //}

        //[TestMethod]
        //public void TestSourceNote()
        //{
        //    // Source citation and attributes
        //    var indi = "0 INDI\n1 SOUR @S331@\n2 QUAY 3\n2 NOTE a note on the source";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Sources.Count);
        //    Assert.AreEqual(1, rec.Sources[0].Beg);
        //    Assert.AreEqual(3, rec.Sources[0].End);
        //    Assert.AreEqual("S331", rec.Sources[0].XRef);
        //    Assert.AreEqual("3", rec.Sources[0].Quay);
        //    Assert.AreEqual(1, rec.Sources[0].Notes.Count);
        //}

        //[TestMethod]
        //public void TestSourceRole()
        //{
        //    // Source citation and attributes
        //    var indi = "0 INDI\n1 SOUR @S331@\n2 QUAY 3\n2 ROLE out the barrel";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Sources.Count);
        //    Assert.AreEqual(1, rec.Sources[0].Beg);
        //    Assert.AreEqual(3, rec.Sources[0].End);
        //    Assert.AreEqual("S331", rec.Sources[0].XRef);
        //    Assert.AreEqual("3", rec.Sources[0].Quay);
        //    Assert.AreEqual("out the barrel", rec.Sources[0].Role);
        //}

        //[TestMethod]
        //public void TestComplexSource()
        //{
        //    // 
        //    var indi = "0 INDI\n1 SOUR @SOURCE1@\n2 PAGE 55\n2 EVEN Event type cited in source\n3 ROLE Role in cited event\n2 DATA\n3 DATE 1 JAN 1900\n3 TEXT Here is some text from the source specific to this source \n4 CONC citation.\n4 CONT Here is more text but on a new line.\n2 QUAY 0\n2 OBJE\n3 TITL Multimedia link about this source\n3 FORM jpeg\n3 NOTE @N26@\n3 FILE ImgFile.JPG\n2 NOTE @N7@";
        //    var rec = parse(indi);
        //    Assert.AreEqual(1, rec.Sources.Count, "source missing");
        //    Assert.AreEqual(1, rec.Sources[0].Notes.Count, "wrong notes");
        //    Assert.AreNotEqual("",rec.Sources[0].Event);
        //    Assert.AreNotEqual("", rec.Sources[0].Role);
        //    Assert.AreNotEqual("", rec.Sources[0].Quay);
        //    Assert.AreNotEqual("", rec.Sources[0].Date);
        //    Assert.AreNotEqual("", rec.Sources[0].Text);
        //    // TODO validate details
        //}

        //[TestMethod]
        //public void EmptyName()
        //{
        //    // Downloaded file BOULDER_CEM_09212009b.GED had a line "1 NAME"
        //    var txt = "0 INDI\n1 NAME\n1 RIN blah";
        //    var rec = parse(txt);
        //    Assert.AreEqual(1, rec.Data.Count);
        //    Assert.AreEqual("RIN", rec.Data[0].Tag);
        //    Assert.AreEqual("blah", rec.Data[0].Data);

        //    Assert.AreEqual(1, rec.Names.Count);
        //    Assert.IsNull(rec.Names[0].Surname);
        //}

    }
}
