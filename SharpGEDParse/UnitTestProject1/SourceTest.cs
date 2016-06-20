using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    // Source Record parse testing
    // TODO TYPE sub-tag testing on REFN

    // TODO 'testsubtag' and 'testsubtag2' invocations are copy-pasta
    // TODO real OBJE testing

    [TestClass]
    public class SourceTest : GedParseTest
    {
        private GedSource parse(string val)
        {
            return parse<GedSource>(val, "SOUR");
        }

        [TestMethod]
        public void TestBasic()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred";
            var rec = parse(txt);
            Assert.AreEqual("S1", rec.XRef);
            Assert.AreEqual("Fred", rec.Author);
        }

        [TestMethod]
        public void TestRefn()
        {
            var txt = "0 @S1@ SOUR\n1 REFN 123";
            var rec = parse(txt);
            Assert.AreEqual(1, rec.UserReferences.Count);
            Assert.AreEqual("123", rec.UserReferences[0]);
            txt = "0 @S1@ SOUR\n1 REFN 123\n1 REFN 456";
            rec = parse(txt);
            Assert.AreEqual(2, rec.UserReferences.Count);
        }

        [TestMethod]
        public void TestTitle()
        {
            var rec = TestSubTag("TITL");
            Assert.AreEqual("Fred", rec.Title);
        }

        private GedSource TestSubTag(string tag)
        {
            var txt = string.Format("0 @S1@ SOUR\n1 {0} Fred", tag);
            var rec = parse(txt);
            Assert.AreEqual("S1", rec.XRef);
            return rec;
        }
        private GedSource TestSubTag2(string tag)
        {
            var txt = string.Format("0 @S1@ SOUR\n1 {0} Fred \n2 CONC Flintstone\n2 CONT yabba dabba doo", tag);
            var rec = parse(txt);
            Assert.AreEqual("S1", rec.XRef);
            return rec;
        }

        [TestMethod]
        public void TestPubl()
        {
            var rec = TestSubTag("PUBL");
            Assert.AreEqual("Fred", rec.Publication);
        }

        [TestMethod]
        public void TestAbbr()
        {
            var rec = TestSubTag("ABBR");
            Assert.AreEqual("Fred", rec.Abbreviation);
        }

        [TestMethod]
        public void TestAuth()
        {
            var rec = TestSubTag("AUTH");
            Assert.AreEqual("Fred", rec.Author);
        }

        [TestMethod]
        public void TestAuth2()
        {
            var rec = TestSubTag2("AUTH");
            Assert.AreEqual("Fred Flintstone\nyabba dabba doo", rec.Author);
        }

        [TestMethod]
        public void TestText()
        {
            var rec = TestSubTag("TEXT");
            Assert.AreEqual("Fred", rec.Text);
        }

        [TestMethod]
        public void TestText2()
        {
            var rec = TestSubTag2("TEXT");
            Assert.AreEqual("Fred Flintstone\nyabba dabba doo", rec.Text);
        }

        [TestMethod]
        public void TestTitle2()
        {
            var rec = TestSubTag2("TITL");
            Assert.AreEqual("Fred Flintstone\nyabba dabba doo", rec.Title);
        }

        [TestMethod]
        public void TestPubl2()
        {
            var rec = TestSubTag2("PUBL");
            Assert.AreEqual("Fred Flintstone\nyabba dabba doo", rec.Publication);
        }

        [TestMethod]
        public void TestRIN()
        {
            var txt = "0 @S1@ SOUR\n1 RIN 2547\n1 RIN gibber";
            var rec = parse(txt);
            Assert.AreEqual("2547", rec.RIN);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [TestMethod]
        public void TestOBJE()
        {
            var txt = "0 @S1@ SOUR\n1 OBJE";
            var rec = parse(txt);
            // TODO real guts/validate
        }

        [TestMethod]
        public void TestChange()
        {
            var indi = "0 @F1@ SOUR\n1 CHAN";
            var rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(1, rec.Change.Item2);

            indi = "0 @F1@ SOUR\n1 CHAN notes\n2 DATE blah";
            rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(2, rec.Change.Item2);

            // Only 1 change record allowed
            // Gedcom spec says take the FIRST one
            indi = "0 @F1@ SOUR\n1 CHAN notes\n2 DATE blah\n1 CHAN notes2";
            rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(2, rec.Change.Item2);
            Assert.AreEqual(1, rec.Errors.Count);

            // TODO test actual details
        }

        [TestMethod]
        public void TestData()
        {
            // taken from allged
            var txt = "0 @S1@ SOUR\n1 DATA\n2 EVEN BIRT, CHR\n3 DATE FROM 1 JAN 1980 TO 1 FEB 1982\n3 PLAC Place\n2 EVEN DEAT\n3 DATE FROM 1 JAN 1980 TO 1 FEB 1982\n3 PLAC Another place\n2 AGNC Resposible agency\n2 NOTE A note about whatever\n3 CONT Note continued here. The word TE\n3 CONC ST should not be broken!";
            var rec = parse(txt);
            Assert.AreNotEqual(0, rec.Data.Count, "DATA tag NYI");
        }

        [TestMethod]
        public void TestReal()
        {
            // A real source record taken from a GED file... Note the missing trailing space in the TEXT tag!
            var txt = "0 @S7@ SOUR\n1 ABBR Truelove-Newton, Alice\n1 TITL Ms Alice [Truelove] Newton, her info abt Lourena Vaughan\n1 TEXT Estelle Truelove's daughter--born in vacinity of New Hill/Aplex, Wake\n2 CONC Co., North Carloina";
            var rec = parse(txt);
            Assert.AreEqual("S7", rec.XRef);
            Assert.IsNotNull(rec.Abbreviation);
            Assert.IsNotNull(rec.Title);
            Assert.IsNotNull(rec.Text);
        }

        [TestMethod]
        public void TestReal2()
        {
            // Another real source record
            var txt = "0 @S4@ SOUR\n1 ABBR Heiratsurkunde Lensahn Nr.7 09.05.1908 Gustav Christian Hahn\n1 DATA\n2 EVEN MARR\n3 DATE 9 MAY 1908\n3 PLAC Lensahn\n1 TITL Heiratsurkunde Lensahn Nr.7/1908 09.05.1908";
            var rec = parse(txt);
            Assert.AreEqual("S4", rec.XRef);
            Assert.IsNotNull(rec.Abbreviation);
            Assert.IsNotNull(rec.Title);
            Assert.AreNotEqual(0, rec.Data.Count, "DATA tag");
        }

        [TestMethod]
        public void TestReal3()
        {
            var txt = "0 @S12@ SOUR\n1 ABBR Geburtsurkunde Weinheim-Altstadt 1843 Seite 333 Anna Margaretha Haas\n1 DATA\n2 AGNC Amtsgericht Weinheim Bergstraße\n1 TITL Geburtsurkunde Weinheim-Altstadt 1843 Seite 333 Nr. 80\n1 REPO @R2@";
            var rec = parse(txt);
            Assert.AreEqual("S12", rec.XRef);
            Assert.IsNotNull(rec.Abbreviation);
            Assert.IsNotNull(rec.Title);
            Assert.AreNotEqual(0, rec.Citations.Count, "REPO tag");
            Assert.AreNotEqual(0, rec.Data.Count, "DATA tag");
        }

        [TestMethod]
        public void TestReal4()
        {
            // note the weird tag WHEREINSOURCE - appears to be from GenoPro
            var txt = "0 @source67329@ SOUR\n1 TITL @S0066966@\n1 WHEREINSOURCE 55\n1 DATA \n2 TEXT 3 JAN 2004";
            var rec = parse(txt);
            Assert.AreEqual("source67329", rec.XRef);
            Assert.AreEqual("@S0066966@", rec.Title);
            Assert.AreNotEqual(0, rec.Unknowns.Count);
        }
    }
}
