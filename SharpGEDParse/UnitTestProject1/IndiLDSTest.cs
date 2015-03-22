using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class IndiLDSTest : GedParseTest
    {
        private KBRGedIndi parse(string val)
        {
            return parse<KBRGedIndi>(val, "INDI");
        }

        [TestMethod]
        public void TestBapl()
        {
            var indi = "0 INDI\n1 BAPL\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("BAPL", rec.LDSEvents[0].Tag);
            Assert.AreEqual("unk", rec.LDSEvents[0].Date);
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual(6, rec.LDSEvents[0].Status.Item1);
            Assert.AreEqual(7, rec.LDSEvents[0].Status.Item2);
        }

        [TestMethod]
        public void TestConl()
        {
            var indi = "0 INDI\n1 CONL\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("CONL", rec.LDSEvents[0].Tag);
            Assert.AreEqual("unk", rec.LDSEvents[0].Date);
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual(6, rec.LDSEvents[0].Status.Item1);
            Assert.AreEqual(7, rec.LDSEvents[0].Status.Item2);
        }
        [TestMethod]
        public void TestEndl()
        {
            var indi = "0 INDI\n1 ENDL\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("ENDL", rec.LDSEvents[0].Tag);
            Assert.AreEqual("unk", rec.LDSEvents[0].Date);
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual(6, rec.LDSEvents[0].Status.Item1);
            Assert.AreEqual(7, rec.LDSEvents[0].Status.Item2);
        }
        [TestMethod]
        public void TestSlgc()
        {
            var indi = "0 INDI\n1 SLGC\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@\n2 FAMC @foo@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("SLGC", rec.LDSEvents[0].Tag);
            Assert.AreEqual("unk", rec.LDSEvents[0].Date);
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual(6, rec.LDSEvents[0].Status.Item1);
            Assert.AreEqual(7, rec.LDSEvents[0].Status.Item2);
            Assert.AreEqual("@foo@", rec.LDSEvents[0].Famc);
        }
        [TestMethod]
        public void TestSlgs()
        {
            var indi = "0 INDI\n1 SLGS\n2 DATE unk\n2 TEMP salt lake\n2 NOTE note1\n2 PLAC salty\n2 STAT insane\n3 DATE statdate\n2 NOTE note2\n2 SOUR @s1@";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.LDSEvents.Count);
            Assert.AreEqual("SLGS", rec.LDSEvents[0].Tag);
            Assert.AreEqual("unk", rec.LDSEvents[0].Date);
            Assert.AreEqual("salt lake", rec.LDSEvents[0].Temple);
            Assert.AreEqual("salty", rec.LDSEvents[0].Place);
            Assert.AreEqual(6, rec.LDSEvents[0].Status.Item1);
            Assert.AreEqual(7, rec.LDSEvents[0].Status.Item2);
        }

        // TODO FAMC : error if not SLGC?
        // TODO tag-specific behaviors?
        // TODO note
        // TODO source
        // TODO multiple sources
        // TODO multiple notes
    }
}
