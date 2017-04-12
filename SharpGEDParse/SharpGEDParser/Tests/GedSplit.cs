using NUnit.Framework;

// TODO exercise Get, GetRest with edge cases
// TODO leading tab?

namespace SharpGEDParser.Tests
{
    // Exercise the GedSplitter class

    [TestFixture]
    class GedSplit
    {
        [Test]
        public void Empty()
        {
            var txt = "";
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(1, gs.Split(txt, ' '));
            Assert.AreEqual(0, gs.Lengths[0]);
            Assert.AreEqual(0, gs.Starts[0]);
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("", res[0]);
        }

        [Test]
        public void Simple()
        {
            var txt = "0 @I1@ INDI";
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(3, gs.Split(txt, ' '));
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("0", res[0]);
            Assert.AreEqual("@I1@", res[1]);
            Assert.AreEqual("INDI", res[2]);
        }

        [Test]
        public void Partial1()
        {
            var txt = "0 @I1@";
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(2, gs.Split(txt, ' '));
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("0", res[0]);
            Assert.AreEqual("@I1@", res[1]);
        }

        [Test]
        public void Partial2()
        {
            var txt = "0";
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(1, gs.Split(txt, ' '));
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("0", res[0]);
        }

        [Test]
        public void ExtraSpaces()
        {
            var txt = "0      @I1@         INDI";
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(3, gs.Split(txt, ' '));
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("0", res[0]);
            Assert.AreEqual("@I1@", res[1]);
            Assert.AreEqual("INDI", res[2]);
        }

        [Test]
        public void Longer()
        {
            var txt = "1 Page According to his death certificate, John McGinnis was bor";
            GEDSplitter gs = new GEDSplitter(20);
            Assert.AreEqual(11, gs.Split(txt, ' '));
            var res = gs.ZeroOneMany(txt);
            Assert.AreEqual("1", res[0]);
            Assert.AreEqual("Page", res[1]);
            Assert.AreEqual("According to his death certificate, John McGinnis was bor", res[2]);
        }

        [Test]
        public void FourParts()
        {
            var txt = "0 @NI28@ NOTE Blah blah blah";
            GEDSplitter gs = new GEDSplitter(20);
            Assert.AreEqual(6, gs.Split(txt, ' '));
            Assert.AreEqual("0", gs.Get(txt, 0));
            Assert.AreEqual("@NI28@", gs.Get(txt, 1));
            Assert.AreEqual("NOTE", gs.Get(txt, 2));
            Assert.AreEqual("Blah blah blah", gs.GetRest(txt, 3));
        }
    }
}
