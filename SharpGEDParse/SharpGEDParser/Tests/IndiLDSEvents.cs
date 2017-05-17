using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpGEDParser.Model;

// TODO empty FAMC reference

// TODO FAMC : error if not SLGC?
// TODO tag-specific behaviors?
// TODO note
// TODO source
// TODO multiple sources
// TODO multiple notes
// TODO NOTE with CONC/CONT
// TODO SOUR with CONC/CONT


namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class IndiLDSEvents : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        public IndiRecord CommonLDS(string tag)
        {
            var indi = string.Format("0 INDI\n1 {0}\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual(tag, rec.LDSEvents[0].Tag);
            Assert.AreEqual("statdate", rec.LDSEvents[0].Date); // TODO real date parsing
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual("insane", rec.LDSEvents[0].Status);
            Assert.AreEqual(2, rec.LDSEvents[0].Notes.Count); // TODO verify details
            Assert.AreEqual(1, rec.LDSEvents[0].Cits.Count); // TODO verify details
            return rec;
        }

        [Test]
        public void TestBapl()
        {
            CommonLDS("BAPL");
        }

        [Test]
        public void TestConl()
        {
            CommonLDS("CONL");
        }

        [Test]
        public void TestEndl()
        {
            CommonLDS("ENDL");
        }

        [Test]
        public void TestSlgc()
        {
            var indi = "0 INDI\n1 SLGC\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@\n2 FAMC @foo@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("SLGC", rec.LDSEvents[0].Tag);
            Assert.AreEqual("statdate", rec.LDSEvents[0].Date); // TODO real date parsing
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual("insane", rec.LDSEvents[0].Status);
            Assert.AreEqual(0, rec.LDSEvents[0].Errors.Count);
            Assert.AreEqual("foo", rec.LDSEvents[0].FamilyXref);
        }

        [Test]
        public void SlgcErrXref()
        {
            // error in FamilyXref
            var indi = "0 INDI\n1 SLGC\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@\n2 FAMC @ @";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("SLGC", rec.LDSEvents[0].Tag);
            Assert.AreEqual("statdate", rec.LDSEvents[0].Date); // TODO real date parsing
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual("insane", rec.LDSEvents[0].Status);
            Assert.AreEqual(1, rec.LDSEvents[0].Errors.Count);
            Assert.AreNotEqual(0, (int)rec.LDSEvents[0].Errors[0].Error);
            Assert.IsNullOrEmpty(rec.LDSEvents[0].FamilyXref);
        }

        [Test]
        public void TestSlgs()
        {
            CommonLDS("SLGS");
        }

    }
}
