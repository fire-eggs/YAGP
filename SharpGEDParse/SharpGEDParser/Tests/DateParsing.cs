using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// Tests for event date parsing

// TODO every month name combination - drive via DateTime?
// TODO julian date calculation verify

// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
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
            const string val = "25 Jul";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        [Test]
        public void InvMon()
        {
            const string val = "25 foo 1964";
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

        [Test]
        public void ValidNonStdMonth()
        {
            const string val = "25 Jul. 1972";
            GEDDate res = ParseForDate(val);

            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(7, res.Month);
            Assert.AreEqual(25, res.Day);
        }

        [Test]
        public void ValidAllShortMonths()
        {
            DateTime dt = new DateTime(1972,1,25);

            for (int i = 1; i <= 12; i++)
            {
                string str1 = dt.ToString("dd MMM yyyy");

                GEDDate res = ParseForDate(str1);

                Assert.AreEqual(GEDDate.Types.Exact, res.Type);
                Assert.AreEqual(1972, res.Year);
                Assert.AreEqual(i, res.Month);
                Assert.AreEqual(25, res.Day);

                dt = dt.AddMonths(1);
            }
        }
        [Test]
        public void ValidAllNonStdShortMonths()
        {
            DateTime dt = new DateTime(1972, 1, 25);

            for (int i = 1; i <= 12; i++)
            {
                string str1 = dt.ToString("dd MMM. yyyy");

                GEDDate res = ParseForDate(str1);

                Assert.AreEqual(GEDDate.Types.Exact, res.Type);
                Assert.AreEqual(1972, res.Year);
                Assert.AreEqual(i, res.Month);
                Assert.AreEqual(25, res.Day);

                dt = dt.AddMonths(1);
            }
        }
        [Test]
        public void ValidAllLongMonths()
        {
            DateTime dt = new DateTime(1972, 1, 25);

            for (int i = 1; i <= 12; i++)
            {
                string str1 = dt.ToString("dd MMMM yyyy");

                GEDDate res = ParseForDate(str1);

                Assert.AreEqual(GEDDate.Types.Exact, res.Type);
                Assert.AreEqual(1972, res.Year);
                Assert.AreEqual(i, res.Month);
                Assert.AreEqual(25, res.Day);

                dt = dt.AddMonths(1);
            }
        }

        [Test]
        public void BasicCalendar()
        {
            // specifying greg cal should have no impact
            string val = "@#DGREGORIAN@ 17 May 1972";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(5, res.Month);
            Assert.AreEqual(17, res.Day);
        }

        [Test]
        public void BasicCalendar2()
        {
            // specifying greg cal should have no impact
            string val = "@#DGREGORIAN 17 May 1972";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(5, res.Month);
            Assert.AreEqual(17, res.Day);
        }

        [Test]
        public void InvalidCalendar()
        {
            string val = "@#GREGORIAN@ 17 May 1972";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }
        [Test]
        public void InvalidChar()
        {
            string val = "#@DGREGORIAN@ 17 May 1972";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        private void TestEra(string era, bool shouldbebc)
        {
            string val = "17 May 1972";
            GEDDate res = ParseForDate(val + era);
            Assert.AreEqual(GEDDate.Types.Exact, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(5, res.Month);
            Assert.AreEqual(17, res.Day);
            Assert.AreEqual(shouldbebc, res.IsBC, era);
        }

        [Test]
        public void ValidStdEra()
        {
            TestEra("B.C.",true);
            TestEra(" B.C.", true);
        }

        [Test]
        public void NonStdBC()
        {
            string[] variants = {"BC", "BCE", "B.C.E."};
            foreach (var variant in variants)
            {
                TestEra(variant, true);
                TestEra(" "+variant, true);
            }
        }
        [Test]
        public void NonStdAD()
        {
            string[] variants = { "AD", "CE", "C.E.", "A.D." };
            foreach (var variant in variants)
            {
                TestEra(variant, false);
                TestEra(" " + variant, false);
            }
        }

        private void TestPrefix(string pref, GEDDate.Types target, bool? isStandard)
        {
            // TODO isStandard: verify flagged as (not)standard

            string val = "17 May 1972";
            GEDDate res = ParseForDate(pref + val);
            Assert.AreEqual(target, res.Type, pref);

            if (!isStandard.HasValue)
                return; // Couldn't parse: values not expected

            Assert.AreEqual(1972, res.Year, pref);
            Assert.AreEqual(5, res.Month, pref);
            Assert.AreEqual(17, res.Day, pref);
            Assert.IsFalse(res.IsBC, pref);
        }

        [Test]
        public void AftKeywords()
        {
            // standard keywords where result is 'after'
            string[] pref2 = { "aft", "from" };
            foreach (var s in pref2)
            {
                TestPrefix(s, GEDDate.Types.After, true);
                TestPrefix(s+" ", GEDDate.Types.After, true);
            }

            // non-standard keywords where result is 'after'
            string[] pref4 = { "aft.", "after" };
            foreach (var s in pref4)
            {
                TestPrefix(s, GEDDate.Types.After, false);
                TestPrefix(s + " ", GEDDate.Types.After, false);
            }
        }

        [Test]
        public void BefKeywords()
        {
            // standard keywords where result is 'before'
            string[] pref = { "bef", "to" };
            foreach (var s in pref)
            {
                TestPrefix(s, GEDDate.Types.Before, true);
                TestPrefix(s+" ", GEDDate.Types.Before, true);
            }

            // non-standard keywords where result is 'before'
            string[] pref3 = { "bef.", "before" };
            foreach (var s in pref3)
            {
                TestPrefix(s, GEDDate.Types.Before, false);
                TestPrefix(s + " ", GEDDate.Types.Before, false);
            }
        }

        [Test]
        public void EstKeywords()
        {
            string[] pref = {"abt", "cal", "est", "int"};
            foreach (var s in pref)
            {
                TestPrefix(s, GEDDate.Types.Estimated, true);
                TestPrefix(s + " ", GEDDate.Types.Estimated, true);
            }
            string[] pref2 = { "about", "circa", "maybe", "int.", "abt.", "cal.", "est." };
            foreach (var s in pref2)
            {
                TestPrefix(s, GEDDate.Types.Estimated, false);
                TestPrefix(s + " ", GEDDate.Types.Estimated, false);
            }
        }

        [Test]
        public void BadKeywords()
        {
            // incorrectly used keywords (incorrect by themselves)
            string[] pref = {"bet", "bet.", "between"};
            foreach (var s in pref)
            {
                TestPrefix(s, GEDDate.Types.Unknown, null);
                TestPrefix(s+" ", GEDDate.Types.Unknown, null);
            }

            // invalid keywords
            string[] pref2 = {"garbage", "foo", "aboot", "sometimes"};
            foreach (var s in pref2)
            {
                TestPrefix(s, GEDDate.Types.Unknown, null);
                TestPrefix(s + " ", GEDDate.Types.Unknown, null);
            }
        }

        [Test]
        public void SecondKeyword()
        {
            string val = "FROM 17 May 1972 TO 25 May 1972";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(5, res.Month);
            Assert.AreEqual(17, res.Day);
            Assert.IsFalse(res.IsBC);

            val = "BET 17 May 1972 AND 25 May 1972";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(5, res.Month);
            Assert.AreEqual(17, res.Day);
            Assert.IsFalse(res.IsBC);

            val = "BET 17 May 1972 BLAH 25 May 1972";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);

            val = "FROM 17 May 1972 BLAH 25 May 1972";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);

            val = "BET 17 May 1972 TO 25 May 1972";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);

            val = "FROM 17 May 1972 AND 25 May 1972";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);

        }

        [Test]
        public void SecondKeyword2()
        {
            var val = "BET 1972 AND 1974";
            var res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1972, res.Year);
            Assert.AreEqual(-1, res.Day);
            Assert.AreEqual(-1, res.Month);
            Assert.IsFalse(res.IsBC);
        }

        [Test]
        public void InvalidSecondKeyword()
        {
            string val = "FROM 17 May 1972 TO ";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);

            val = "BET 17 May 1972 AND";
            res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Unknown, res.Type);
        }

        [Test]
        public void RealWorld()
        {
            // found during real GEDCOM testing
            string val = "1910 to 1920";
            GEDDate res = ParseForDate(val);
            Assert.AreEqual(GEDDate.Types.Range, res.Type);
            Assert.AreEqual(1910, res.Year);

            // TODO verify range values
        }

        [Test]
        public void TestIntParseFail()
        {
            // Code coverage: failure to parse integer
            string val = "Apr 2147483648";
            var res = ParseForDate(val);
            Assert.AreEqual(-1, res.Year);
        }

    }
}
