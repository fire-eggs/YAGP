using NUnit.Framework;
using SharpGEDParser.Model;

// Tests for event date parsing

// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class DateParsing : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        private GEDDate ParseForDate(string datestr)
        {
            const string common = "0 @I1@ INDI\n1 BIRT\n2 DATE {0}";
            string val = string.Format(common, datestr);
            var rec = parse(val);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.IsNotNull(rec.Events[0].GedDate);
            return rec.Events[0].GedDate;
        }

        [Test]
        public void TestValid()
        {
            const string val = "13 Apr 1964";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1964, res.Year);
            Assert.AreEqual(4, res.Month);
            Assert.AreEqual(13, res.Day);
        }

        [Test]
        public void TestValidYear()
        {
            const string val = "1964";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1964, res.Year);
            Assert.AreEqual(-1, res.Month);
            Assert.AreEqual(-1, res.Day);
        }

        [Test]
        public void TestValidMonYear()
        {
            const string val = "Apr 1964";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1964, res.Year);
            Assert.AreEqual(4, res.Month);
            Assert.AreEqual(-1, res.Day);
        }

        [Test]
        public void DayMon()
        {
            const string val = "13 Apr";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        [Test]
        public void InvMon()
        {
            const string val = "13 foo 1964";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        [Test]
        public void InvMon2()
        {
            const string val = "foo 1964";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        [Test]
        public void ValidLongMonYear()
        {
            const string val = "December 1964";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1964, res.Year);
            Assert.AreEqual(12, res.Month);
            Assert.AreEqual(-1, res.Day);
        }

        [Test]
        public void ValidDayLongMonYear()
        {
            const string val = "25 December 1964";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1964, res.Year);
            Assert.AreEqual(12, res.Month);
            Assert.AreEqual(25, res.Day);
        }

    }
}
