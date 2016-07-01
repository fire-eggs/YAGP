using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [TestClass]
    public class RepositoryTest : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [TestMethod]
        public void TestSimple1()
        {
            var txt = "0 @R1@ REPO\n1 NAME foobar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.Name);
            Assert.AreEqual("R1", rec.Ident);
        }

        [TestMethod]
        public void TestSimple2()
        {
            var txt = "0 @R1@ REPO\n1 RIN foobar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("R1", rec.Ident);
        }

        [TestMethod]
        public void TestCust1()
        {
            var txt = "0 @R1@ REPO\n1 _CUST foobar\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Custom.Count);
            Assert.AreEqual(1, rec.Custom[0].LineCount);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual("R1", rec.Ident);
        }

        [TestMethod]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @R1@ REPO\n1 _CUST foobar\n2 CONC foobar2\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Custom.Count);
            Assert.AreEqual(2, rec.Custom[0].LineCount);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual("R1", rec.Ident);
        }

        [TestMethod]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
        }
        [TestMethod]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar\n1 REFN 002\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
        }

        [TestMethod]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
        }

        [TestMethod]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
        }

        [TestMethod]
        public void TestMissingId()
        {
            // empty record; missing id
            var txt = "0 REPO\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [TestMethod]
        public void TestMissingId2()
        {
            // missing id
            var txt = "0 REPO\n1 NAME foobar\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            GedRepository rec = res.Data[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.Name);
        }

        [TestMethod]
        public void TestChan()
        {
            var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 APR 2000\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
        }

        [TestMethod]
        public void TestChan2()
        {
            // no date for chan
            var txt = "0 @R1@ REPO\n1 CHAN\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [TestMethod]
        public void TestChan3()
        {
            var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 APR 2000\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
        }

        [TestMethod]
        public void TestChan4()
        {
            // no date value
            var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
        }

        [TestMethod]
        public void TestChan5()
        {
            // extra
            var txt = "0 @R1@ REPO\n1 CHAN\n2 CUSTOM foo\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
        }
        [TestMethod]
        public void TestChan6()
        {
            // multi line extra
            var txt = "0 @R1@ REPO\n1 CHAN\n2 CUSTOM foo\n3 _BLAH bar\n1 NAME fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
        }

        [TestMethod]
        public void TestChan7()
        {
            // multiple CHAN
            var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 MAR 2000\n1 NAME fumbar\n1 CHAN\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Name);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
        }
    }
}
