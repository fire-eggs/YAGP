using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

// TODO move to SharpGedWriter, not TravisWrite

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Misc : GedWriteTest
    {

        [Test,Ignore("Echo-ing unknown tags NYI")]
        public void Repo_Rfn()
        {
            // TODO non-standard tag, apparently from FamilyTreeMaker and possibly PAF. Should be output as-entered?
            var inp = "0 @R1@ REPO\n1 RFN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test,Ignore("Echo-ing unknown tags NYI")]
        public void Repo_Caln()
        {
            // TODO non-standard tag, apparently from FamilyTreeMaker and possibly PAF. Should be output as-entered?
            var inp = "0 @R1@ REPO\n1 CALN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Repo_Refn()
        {
            var inp = "0 @R1@ REPO\n1 REFN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Repo_Refn_Type()
        {
            // TODO bug - not storing uncommon lines like 'TYPE' to be regurgitated out again
            var inp = "0 @R1@ REPO\n1 REFN blah\n2 TYPE User-defined\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Repo_Rin()
        {
            var inp = "0 @R1@ REPO\n1 RIN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Note_Refn()
        {
            var inp = "0 @N1@ NOTE\n1 REFN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }
        [Test]
        public void Note_Refn_Type()
        {
            // TODO bug - not storing uncommon lines like 'TYPE' to be regurgitated out again
            var inp = "0 @N1@ NOTE\n1 REFN blah\n2 TYPE User-defined\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

        [Test]
        public void Note_Rin()
        {
            var inp = "0 @N1@ NOTE\n1 RIN blah\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp, res);
        }

    }
}
