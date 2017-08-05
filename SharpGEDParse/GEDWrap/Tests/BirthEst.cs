using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

// TODO how to validate the estimated value established?
// TODO mother/father distinction only important if estimated value is different

namespace GEDWrap.Tests
{
    class BirthEst : TestUtil
    {
        private Forest Load(string txt)
        {
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);
            DateEstimator.Estimate(f);
            return f;
        }

        [Test]
        public void NoEst()
        {
            // one person, valid birth date, no estimation necessary
            var txt = "0 @I1@ INDI\n1 BIRT\n2 DATE 1 APR 2011";
            Forest f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
        }

        [Test]
        public void CantEst()
        {
            // one person, no birth date, no estimation possible
            var txt = "0 @I1@ INDI\n1 BIRT";
            Forest f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
        }

        [Test]
        public void Empty()
        {
            var txt = "";
            Forest f = Load(txt);
        }

        [Test]
        public void CantParse()
        {
            // one person, unparsable birth date, no estimation possible
            var txt = "0 @I1@ INDI\n1 BIRT\n2 DATE gibber";
            Forest f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
        }

        private string ComposePair(string sex1, string sex2, 
                                   bool ischild2, 
                                   string date1, string date2)
        {
            var indi1 = "0 @I1@ INDI\n1 SEX {0}\n1 BIRT\n{1}1 FAMS @F1@\n";
            var indi2 = "0 @I2@ INDI\n1 SEX {0}\n1 BIRT\n{1}{2}\n";
            var fam = "0 @F1@ FAM\n{0}\n{1}";
            var txt1 = string.Format(indi1, sex1,
                                     date1 == "" ? "" : "2 DATE " + date1 + "\n");
            var txt2 = string.Format(indi2, sex2,
                                     date2 == "" ? "" : "2 DATE "+ date2 + "\n",
                                     ischild2 ? "1 FAMC @F1@" : "1 FAMS @F1@");
            var txt3 = string.Format(fam, 
                sex1 == "M" ? "1 HUSB @I1@" : "1 WIFE @I1@",
                ischild2 ? "1 CHIL @I2@" : "1 WIFE @I2@");
            return txt1 + txt2 + txt3;
        }

        [Test]
        public void HWNoEst()
        {
            // Husband+wife
            var txt = ComposePair("M", "F", false, "1 APR 2011", "1 APR 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
        }

        [Test]
        public void MCNoEst()
        {
            // Mother+child
            var txt = ComposePair("F", "F", true, "1 APR 2011", "1 APR 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
        }

        [Test]
        public void FCNoEst()
        {
            // Father+child
            var txt = ComposePair("M", "F", true, "1 APR 2011", "1 APR 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
        }

        [Test]
        public void HWCantEst()
        {
            // Husband+wife
            var txt = ComposePair("M", "F", false, "gibber", "gibber");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNull(b);
        }

        [Test]
        public void HWCantEst2()
        {
            // Husband+wife
            var txt = ComposePair("M", "F", false, "", "gibber");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNull(b);
        }

        [Test]
        public void HWEstWife()
        {
            // Husband+wife
            var txt = ComposePair("M", "F", false, "1 Apr 2011", "gibber");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        [Test]
        public void HWEstHusb()
        {
            // Husband+wife
            var txt = ComposePair("M", "F", false, "gibber", "1 Apr 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        [Test]
        public void FCCantEst()
        {
            // Father+child
            var txt = ComposePair("M", "F", true, "", "");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNull(b);
        }

        [Test]
        public void FCEstDad()
        {
            // Father+child
            var txt = ComposePair("M", "F", true, "gibber", "1 Apr 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        [Test]
        public void FCEstChild()
        {
            // Father+child
            var txt = ComposePair("M", "F", true, "1 Apr 2011", "gibber");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        [Test]
        public void MCCantEst()
        {
            // Mother+child
            var txt = ComposePair("F", "F", true, "", "");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNull(b);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNull(b);
        }

        [Test]
        public void MCEstMom()
        {
            // Mother+child
            var txt = ComposePair("F", "F", true, "gibber", "1 Apr 2011");
            var f = Load(txt);

            GEDDate b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        [Test]
        public void MCEstChild()
        {
            // Mother+child
            var txt = ComposePair("F", "F", true, "1 Apr 2011", "gibber");
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
        }

        private string ComposeTwoGen(string gSex, bool gdated, bool pdated, bool cdated)
        {
            // Build a 2-generation tree. The parent/child pair is fixed
            // regarding sex, spouse; the gparent sex can be specified.

            var txt = ComposePair("F", "M", true,
                                    pdated ? "1 APR 2011" : "",
                                    cdated ? "1 APR 2011" : "");
            var indi3 = string.Format("0 @I3@ INDI\n1 SEX {0}\n1 FAMS @F2@\n1 BIRT\n{1}",
                gSex, gdated ? "2 DATE 1 APR 2011\n" : "");
            var fam2 = string.Format("\n0 @F2@ FAM\n1 {0} @I3@\n1 CHIL @I1@",
                gSex == "M" ? "HUSB" : "WIFE");
            var txt2 = indi3 + txt.Replace("@I1@ INDI","@I1@ INDI\n1 FAMC @F2@") + fam2;
            return txt2;
        }

        [Test]
        public void TwoGenEstG()
        {
            // Two generations, estimate from grandparent
            var txt = ComposeTwoGen("M", true, false, false);
            var f = Load(txt);

            GEDDate b = f.PersonById("I3").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);

        }
        [Test]
        public void TwoGenEstP()
        {
            // Two generations, estimate from parent
            var txt = ComposeTwoGen("M", false, true, false);
            var f = Load(txt);

            GEDDate b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
            b = f.PersonById("I3").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);

        }
        [Test]
        public void TwoGenEstC()
        {
            // Two generations, estimate from parent
            var txt = ComposeTwoGen("F", false, false, true);
            var f = Load(txt);

            GEDDate b = f.PersonById("I2").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreNotEqual(GEDDate.Types.Estimated, b.Type);
            Assert.AreEqual(2455653, b.JDN);
            b = f.PersonById("I1").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);
            b = f.PersonById("I3").BirthDate;
            Assert.IsNotNull(b);
            Assert.AreEqual(GEDDate.Types.Estimated, b.Type);

        }
    }
}
