using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sub-records of INDI have been fixed to an order which is similar to that of PAF, AncQuest
using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class IndiOrder : GedWriteTest
    {
        private string[] validOrder =
        {
            "0 @I1@ INDI",
            "1 NAME Fred",
            "1 SEX M",
            "1 FAMS @F1@",
            "1 FAMC @F2@",
            "1 _UID 9876543210", // Note: not valid value
            "1 NOTE @N1@",
            "1 CHAN",
            "2 DATE 2 DEC 2017"
        };

        private string MakeInput(int start=1)
        {
            // Take the valid order set and output in a different sequence
            // Done by shifting, works only with single-line sub-records
            StringBuilder inp = new StringBuilder();
            inp.Append(validOrder[0]);
            inp.Append("\n");

            for (int i = start; i < validOrder.Length; i++)
            {
                inp.Append(validOrder[i]);
                inp.Append("\n");
            }
            for (int i = 1; i < start; i++)
            {
                inp.Append(validOrder[i]);
                inp.Append("\n");
            }
            return inp.ToString();
        }

        [Test]
        public void Baseline()
        {
            // Using original valid order, what goes in must come out
            var str = MakeInput();
            var res = ParseAndWrite(str);
            Assert.AreEqual(str, res);
        }

        [Test]
        public void Variants()
        {
            // exercise rotated variants - works only with one line sub-records
            var valid = MakeInput(); // valid order

            for (int i = 2; i < 7; i++) // NOTE: makes assumptions about input
            {
                var str = MakeInput(i);
                var res = ParseAndWrite(str);
                Assert.AreEqual(valid, res, "Shift "+i);
            }
        }

    }
}
