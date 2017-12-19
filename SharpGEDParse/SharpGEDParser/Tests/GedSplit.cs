using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

// TODO exercise Get, GetRest with edge cases
// TODO leading tab?

namespace SharpGEDParser.Tests
{
    // Exercise the GedSplitter class

    [ExcludeFromCodeCoverage]
    [TestFixture]
    class GedSplit
    {
        // TODO how to get an empty character array?
        //[Test]
        //public void Empty()
        //{
        //    char[] txt = new char[] {};
        //    GEDSplitter gs = new GEDSplitter(10);
        //    Assert.AreEqual(1, gs.Split(txt, ' '));
        //    Assert.AreEqual(0, gs.Lengths[0]);
        //    Assert.AreEqual(0, gs.Starts[0]);
        //    Assert.AreEqual("", gs.Level(txt));
        //}

        [Test]
        public void Partial1()
        {
            var txt = "0 @I1@".ToCharArray();
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(2, gs.Split(txt, ' '));
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("I1", gs.Ident(txt));
        }

        [Test]
        public void Partial2()
        {
            var txt = "0".ToCharArray();
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(1, gs.Split(txt, ' '));
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual(null, gs.Tag(txt));
            Assert.AreEqual(null, gs.Remain(txt));
        }

        [Test]
        public void ExtraSpaces()
        {
            var txt = "0      @I1@         INDI".ToCharArray();
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(3, gs.Split(txt, ' '));
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("I1", gs.Ident(txt));
            Assert.AreEqual("INDI", gs.Tag(txt));
        }

        [Test]
        public void Longer()
        {
            var txt = "1 Page According to his death certificate, John McGinnis was bor".ToCharArray();
            GEDSplitter gs = new GEDSplitter(20);
            Assert.AreEqual(11, gs.Split(txt, ' '));
            Assert.AreEqual('1', gs.Level(txt));
            Assert.AreEqual("Page", gs.Tag(txt));
            Assert.AreEqual("According to his death certificate, John McGinnis was bor", gs.Remain(txt));
        }

        //[Test]
        //public void FourParts()
        //{
        //    var txt = "0 @NI28@ NOTE Blah blah blah";
        //    GEDSplitter gs = new GEDSplitter(20);
        //    Assert.AreEqual(6, gs.Split(txt, ' '));
        //    Assert.AreEqual("0", gs.Get(txt, 0));
        //    Assert.AreEqual("@NI28@", gs.Get(txt, 1));
        //    Assert.AreEqual("NOTE", gs.Get(txt, 2));
        //    Assert.AreEqual("Blah blah blah", gs.GetRest(txt, 3));
        //}

        [Test]
        public void EmptyId()
        {
            var txt = "0 @ @ FAM".ToCharArray();
            GEDSplitter gs = new GEDSplitter();
            gs.Split(txt, ' ');
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual(null, gs.Ident(txt));
            Assert.AreEqual("FAM", gs.Tag(txt));
        }

        [Test]
        public void BadId()
        {
            var txt = "0 @I1  FAM".ToCharArray();
            GEDSplitter gs = new GEDSplitter();
            gs.Split(txt, ' ');
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("FAM", gs.Tag(txt));
            Assert.AreEqual("I1", gs.Ident(txt));
        }

        [Test]
        public void BadId2()
        {
            // See V208252.ged
            var txt = "0 @ 11@  FAM".ToCharArray();
            GEDSplitter gs = new GEDSplitter();
            gs.Split(txt, ' ');
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("FAM", gs.Tag(txt));
            Assert.AreEqual("11", gs.Ident(txt));
        }

        [Test]
        public void MissTag()
        {
            var txt = "0 @Z1@ ".ToCharArray();
            GEDSplitter gs = new GEDSplitter();
            gs.Split(txt, ' ');
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("Z1", gs.Ident(txt));
            Assert.AreEqual("", gs.Tag(txt));
        }

        [Test]
        public void LongerDefault()
        {
            var txt = "1 Page According to his death certificate, John McGinnis was born in 1776 a real life nephew".ToCharArray();
            GEDSplitter gs = new GEDSplitter();
            gs.Split(txt, ' ');
            Assert.AreEqual('1', gs.Level(txt));
            Assert.AreEqual("Page", gs.Tag(txt));
        }

        [Test, Ignore("Can't split on tabs")]
        public void TabSplit()
        {
            var txt = "0\t@I1@\tINDI".ToCharArray();
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(3, gs.Split(txt, ' '));
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("I1", gs.Ident(txt));
            Assert.AreEqual("INDI", gs.Tag(txt));
        }

        [Test,Ignore("Can't split on tabs")]
        public void LeadTab()
        {
            var txt = "\t0 @I1@ INDI".ToCharArray();
            GEDSplitter gs = new GEDSplitter(10);
            Assert.AreEqual(3, gs.Split(txt, ' '));
            Assert.AreEqual('0', gs.Level(txt));
            Assert.AreEqual("I1", gs.Ident(txt));
            Assert.AreEqual("INDI", gs.Tag(txt));
        }
    }
}
