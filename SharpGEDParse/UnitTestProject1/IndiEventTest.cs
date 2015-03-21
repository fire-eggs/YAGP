using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class IndiEventTest : GedParseTest
    {
        [TestMethod]
        public void TestBirth()
        {
            string indi = "0 INDI\n1 BIRT\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("1774", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);

            string indi2 = "0 INDI\n1 BIRT\n2 TYPE suspicious\n2 DATE 1776\n2 PLAC Sands, Oldham, Lncshr, Eng";
            rec = parse(indi2);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("1776", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
            Assert.AreEqual("suspicious", rec.Events[0].Type);

            string indi3 = "0 INDI\n1 BIRT\n2 PLAC Sands, Oldham, Lncshr, Eng";
            rec = parse(indi3);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
        }

        [TestMethod]
        public void TestCHR()
        {
            string indi = "0 INDI\n1 CHR\n2 DATE 20 Nov 1774\n2 PLAC St  Mary, Oldham, Lancashire, England";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("20 Nov 1774", rec.Events[0].Date);
            Assert.AreEqual("St  Mary, Oldham, Lancashire, England", rec.Events[0].Place);
        }

        [TestMethod]
        public void TestDeath()
        {
            string indi = "0 INDI\n1 DEAT\n2 DATE 4 Mar 1878\n2 PLAC Laneside, Crompton, Lancashire, England";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("4 Mar 1878", rec.Events[0].Date);
            Assert.AreEqual("Laneside, Crompton, Lancashire, England", rec.Events[0].Place);
        }

        [TestMethod]
        public void TestBurial()
        {
            string indi = "0 INDI\n1 BURI\n2 DATE 8 Jul 1846\n2 PLAC Shaw, Lancashire, England";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("8 Jul 1846", rec.Events[0].Date);
            Assert.AreEqual("Shaw, Lancashire, England", rec.Events[0].Place);
        }

        [TestMethod]
        public void TestEventSource()
        {
            string indi2 = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR @S122@\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n2 SOUR @S124@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(3, rec.Events[0].Source.Item1);
            Assert.AreEqual(5, rec.Events[0].Source.Item2);
            // TODO - more than one source!
        }

    }
}
