﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

// Example records pulled from real-world GED files

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class RealWorld : GedWriteTest
    {
        // INDI expected order is:
        // ID, NAME, SEX, EVENTS, FAMS, FAMC, NOTE, SOUR, OBJE, CHAN

        private string[] record1 = // From Blades.ged
        {
        "0 @I4@ INDI", // 0
        "1 NAME Frances /Unknown/",
        "2 GIVN Frances",
        "2 SURN Unknown",
        "1 SEX F",
        "1 _UID 5D1461CFCB53DF439EA8F56D8146A6D4CA50", // 5
        "1 CHAN",
        "2 DATE 13 APR 2008",
        "1 SOUR @S2@",
        "2 PAGE n/a",
        "2 _TMPLT", // 10
        "3 FIELD",
        "4 NAME Page",
        "4 VALUE n/a",
        "1 BIRT",
        "2 DATE ABT 1660", // 15
        "2 _SDATE 1 JUL 1660",
        "2 PLAC prob Talbot County, Maryland",
        "1 DEAT",
        "2 DATE 1698",
        "2 _SDATE 1 JUL 1698", // 20
        "1 FAMS @F2@",
        };

        [Test]
        public void BladesReal()
        {
            int[] exp = {0,1,2,3,4,14,15,16,17,18,19,20,21,5,8,9,10,11,12,13,6,7};
            Assert.AreEqual(exp.Count(),record1.Count());
            var fr = ReadItHigher(MakeInput(record1));
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            var ideal = MakeInput(record1, exp);
            Assert.AreEqual(ideal,res); // TODO write name parts
        }

        private string[] record2 = // From Blades.ged
        {
        "0 @I4@ INDI", // 0
        "1 NAME Frances /Unknown/",
        "1 SEX F",
        "1 _UID 5D1461CFCB53DF439EA8F56D8146A6D4CA50",
        "1 CHAN",
        "2 DATE 13 APR 2008", // 5
        "1 SOUR @S2@",
        "2 PAGE n/a",
        "1 BIRT",
        "2 DATE ABT 1660", 
        "1 DEAT", // 10
        "2 DATE 1698", 
        "1 FAMS @F2@", // 12
        };

        // INDI expected order is:
        // ID, NAME, SEX, EVENTS, FAMS, FAMC, _UID, NOTE, SOUR, OBJE, CHAN

        [Test]
        public void BladesSimple()
        {
            // No EVENT.PLAC, other lines
            int[] exp = {0, 1, 2, 8, 9, 10, 11, 12, 3, 6, 7, 4, 5};
            Assert.AreEqual(exp.Count(), record2.Count());
            var fr = ReadItHigher(MakeInput(record2));
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            var ideal = MakeInput(record2, exp);
            Assert.AreEqual(ideal, res);
        }

    }
}