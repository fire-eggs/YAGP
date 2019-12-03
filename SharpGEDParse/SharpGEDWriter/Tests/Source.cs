using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Source : GedWriteTest
    {
        [Test]
        public void SourRepo()
        {
            // SOUR.REPO was missing '@'s
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO @R1@\n1 RIN rin-chan";
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void SourRepo2()
        {
            // SOUR.REPO with no xref
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO\n2 CALN blah\n1 RIN rin-chan";
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n");
        }

        private void ShortTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a single-line blah";
            var txt = string.Format(format, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag + "Sh");
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n", tag + "Sh");
        }

        private void ShortConcTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a single\n2 CONC -line blah";
            var expform = "0 @S1@ SOUR\n1 {0} this is a single-line blah\n";
            var txt = string.Format(format, tag);
            var exp = string.Format(expform, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag+"ShCo");
            var res = Write(fr);
            Assert.AreEqual(res, exp, tag + "ShCo");
        }

        private void ShortContTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a multiple\n2 CONT -line blah";
            var txt = string.Format(format, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag + "ShCt");
            var res = Write(fr);
            Assert.AreEqual(txt + "\n", res, tag + "ShCt");
        }

        [Test]
        public void Auth()
        {
            ShortTest("AUTH");
            ShortConcTest("AUTH");
            ShortContTest("AUTH");
        }

        [Test]
        public void Titl()
        {
            ShortTest("TITL");
            ShortConcTest("TITL");
            ShortContTest("TITL");
        }

        [Test]
        public void Publ()
        {
            ShortTest("PUBL");
            ShortConcTest("PUBL");
            ShortContTest("PUBL");
        }

        [Test]
        public void Text()
        {
            ShortTest("TEXT");
            ShortConcTest("TEXT");
            ShortContTest("TEXT");
        }

        [Test]
        public void SourCitRec()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitRec2()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is multi-\n4 CONT line text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitRec3()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short conc\n4 CONC atenated text";
            var res = ParseAndWrite(inp);
            var exp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short concatenated text\n";
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void SourCit()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is short text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCit2()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is multi-\n2 CONT line text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCit3()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is short conc\n2 CONC atenated text";
            var res = ParseAndWrite(inp);
            var exp = "0 @I1@ INDI\n1 SOUR This is short concatenated text\n";
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void SourCitQuay()
        {
            // I observed that Quay was done wrong [src record variant]
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 QUAY quack";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitQuay2()
        {
            // I observed that Quay was done wrong [no src record variant]
            var inp = "0 @I1@ INDI\n1 SOUR\n2 QUAY quack";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitEven()
        {
            // TODO observed issue: this is in error (PAGE tag w/an embedded cit) but is written "as-is"
            // TODO need to establish how to output when invalid input [do not want to propagate errors]
            // var inp = "0 @I1@ INDI\n1 SOUR\n2 PAGE 3\n2 ROLE bugger";

            // TODO observed issue: this is not flagged as an error, but is not valid
            //var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 PAGE 3\n2 ROLE bugger";

            // TODO the difference is custom tags are written as-is but errors/other would have to be something else?
        }

        [Test]
        public void SourCitText()
        {
            // TEXT tag w/o DATA
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 TEXT This is short text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitText2()
        {
            // TEXT tag w/o DATA
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 TEXT This is multi\n3 CONT -line text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourRecData()
        {
            var inp = "0 @S1@ SOUR\n1 DATA\n2 EVEN ice age\n3 DATE FROM 10000 BC\n2 EVEN black plague\n3 PLAC Europe";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        private void doLongSourTag(string tag)
        {
            var inp = string.Format("0 @S1@ SOUR\n1 {0} " +
                      "MR PERCY JAMES HATCHER was born at Brent Knoll, a village over the othe\n" +
                      "2 CONC r side of the hill from Rooksbridge, last but one in a family of ten br\n" +
                      "2 CONC others and sisters. On leaving school Percy helped for a couple of year\n" +
                      "2 CONC s in the pork butchery business of his late father. However, being very f\n" +
                      "2 CONC ond of horses, he left the district to go to Cromhall, a village in Glo\n" +
                      "2 CONC ucestershire, and for a period of three years looked after the hunters b\n" +
                      "2 CONC elonging to a family there. His father had by that time retired from th\n" +
                      "2 CONC e butchery and, the brother who had been running it<b> </b>having been c\n" +
                      "2 CONC alled up to join the Army, Percy returned home to manage the business. B\n" +
                      "2 CONC esides doing the buying, the slaughtering and the selling in the shop, h\n" +
                      "2 CONC e drove a horse-and-trap to deliver to customers in the surrounding cou\n" +
                      "2 CONC ntry­side. It was not long, though.before he himself was of<b> </b>age t\n" +
                      "2 CONC o<b> </b>join the Army, and in October 1916 Was enrolled in the Royal B\n" +
                      "2 CONC erkshire Regiment.\n", tag);
            var fr = ReadItHigher(inp);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);

            var exp = string.Format(
"0 @S1@ SOUR\n1 {0} MR PERCY JAMES HATCHER was born at Brent Knoll, a village over the other side of the hill from Rooksbridge, last but one in a family of ten brothers and sisters. On leaving school Percy helped for a couple of years in the pork butchery bu\n" +
"2 CONC siness of his late father. However, being very fond of horses, he left the district to go to Cromhall, a village in Gloucestershire, and for a period of three years looked after the hunters belonging to a family there. His father had by that tim\n" +
"2 CONC e retired from the butchery and, the brother who had been running it<b> </b>having been called up to join the Army, Percy returned home to manage the business. Besides doing the buying, the slaughtering and the selling in the shop, he drove a ho\n" +
"2 CONC rse-and-trap to deliver to customers in the surrounding country­side. It was not long, though.before he himself was of<b> </b>age to<b> </b>join the Army, and in October 1916 Was enrolled in the Royal Berkshire Regiment.\n",
tag);
            Assert.AreEqual(exp, res, tag);
        }

        [Test]
        public void LongSour()
        {
            doLongSourTag("AUTH");
            doLongSourTag("TITL");
            doLongSourTag("PUBL");
            doLongSourTag("TEXT");
        }

        [Test]
        public void LongSourCitDesc()
        {
            var inp = string.Format("0 @I1@ INDI\n1 SOUR " +
                      "MR PERCY JAMES HATCHER was born at Brent Knoll, a village over the othe\n" +
                      "2 CONC r side of the hill from Rooksbridge, last but one in a family of ten br\n" +
                      "2 CONC others and sisters. On leaving school Percy helped for a couple of year\n" +
                      "2 CONC s in the pork butchery business of his late father. However, being very f\n" +
                      "2 CONC ond of horses, he left the district to go to Cromhall, a village in Glo\n" +
                      "2 CONC ucestershire, and for a period of three years looked after the hunters b\n" +
                      "2 CONC elonging to a family there. His father had by that time retired from th\n" +
                      "2 CONC e butchery and, the brother who had been running it<b> </b>having been c\n" +
                      "2 CONC alled up to join the Army, Percy returned home to manage the business. B\n" +
                      "2 CONC esides doing the buying, the slaughtering and the selling in the shop, h\n" +
                      "2 CONC e drove a horse-and-trap to deliver to customers in the surrounding cou\n" +
                      "2 CONC ntry­side. It was not long, though.before he himself was of<b> </b>age t\n" +
                      "2 CONC o<b> </b>join the Army, and in October 1916 Was enrolled in the Royal B\n" +
                      "2 CONC erkshire Regiment.\n");
            var fr = ReadItHigher(inp);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);

            var exp = "0 @I1@ INDI\n1 SOUR MR PERCY JAMES HATCHER was born at Brent Knoll, a village over the other side of the hill from Rooksbridge, last but one in a family of ten brothers and sisters. On leaving school Percy helped for a couple of years in the pork butchery bu\n" +
                      "2 CONC siness of his late father. However, being very fond of horses, he left the district to go to Cromhall, a village in Gloucestershire, and for a period of three years looked after the hunters belonging to a family there. His father had by that tim\n"+
                      "2 CONC e retired from the butchery and, the brother who had been running it<b> </b>having been called up to join the Army, Percy returned home to manage the business. Besides doing the buying, the slaughtering and the selling in the shop, he drove a ho\n"+
                      "2 CONC rse-and-trap to deliver to customers in the surrounding country­side. It was not long, though.before he himself was of<b> </b>age to<b> </b>join the Army, and in October 1916 Was enrolled in the Royal Berkshire Regiment.\n";

            Assert.AreEqual(exp, res);
        }

        [Test]
        public void NameSource()
        {
            // INDI.NAME.SOUR now handled
            var inp = "0 @I1@ INDI\n1 NAME Frances\n2 GIVN Frances\n2 SOUR @S1@";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test,Ignore("Echo-ing back unknown tags is NYI")]
        public void Sour_Rfn()
        {
            var inp = "0 @S1@ SOUR\n1 RFN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }
        [Test,Ignore("Echo-ing back unknown tags is NYI")]
        public void Sour_Caln()
        {
            var inp = "0 @S1@ SOUR\n1 CALN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Sour_Refn()
        {
            var inp = "0 @S1@ SOUR\n1 REFN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }
        [Test]
        public void Sour_Refn_Type()
        {
            // TODO "extra" / uncommon lines like 'TYPE' have not been stored so currently can't be written out
            var inp = "0 @S1@ SOUR\n1 REFN blah\n2 TYPE User-defined\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Sour_Chan()
        {
            var inp = "0 @S1@ SOUR\n1 CHAN\n2 DATE 1 APR 2000\n2 NOTE Change note\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }
    }
}
