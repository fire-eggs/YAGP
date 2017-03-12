using NUnit.Framework;

// TODO consider a family builder interface?

namespace GEDWrap.Tests
{
    [TestFixture]
    class PedigreeTests : TestUtil
    {
        private Pedigrees GetPedigree(string ident, bool checkErrors=false)
        {
            // I4 is child of I2
            // I2 is biologic child of I1
            // I2 is adopted child of I3
            // I5 is unrelated

            var txt = "0 @I1@ INDI\n1 FAMS @F2@\n" +
                      "0 @I2@ INDI\n1 ADOP\n2 FAMC @F3@\n1 FAMC @F2@\n1 FAMS @F1@\n" +
                      "0 @I3@ INDI\n1 FAMS @F3@\n" +
                      "0 @I4@ INDI\n1 FAMC @F1@\n" +
                      "0 @I5@ INDI\n" +
                      "0 @F1@ FAM\n1 HUSB @I2@\n1 CHIL @I4@\n" +
                      "0 @F2@ FAM\n1 HUSB @I1@\n1 CHIL @I2@\n" +
                      "0 @F3@ FAM\n1 HUSB @I3@\n1 CHIL @I2@\n";

            Forest f = LoadGEDFromStream(txt);
            Assert.IsNotNull(f, ident);
            if (checkErrors)
            {
                Assert.AreEqual(0, f.ErrorsCount); 
                Assert.AreEqual(0, f.Errors.Count);
            }

            Person p = f.PersonById(ident);
            Assert.IsNotNull(p, ident);
            
            Pedigrees pd = new Pedigrees(p,false);
            Assert.IsNotNull(pd, ident);
            return pd;
        }

        [Test]
        public void AdoptedChildCheck()
        {
            // TODO "0 FAM + 1 CHIL" check not handling adoption            
            var pd = GetPedigree("I1", checkErrors: true);
        }

        [Test]
        public void Normal()
        {
            var pd = GetPedigree("I1");
            Assert.AreEqual(1, pd.PedigreeCount);
            pd = GetPedigree("I5");
            Assert.AreEqual(1, pd.PedigreeCount);
        }

        [Test]
        public void Adopted()
        {
            var pd = GetPedigree("I2");
            Assert.AreEqual(2, pd.PedigreeCount);
        }

        [Test]
        public void ParentAdopted()
        {
            var pd = GetPedigree("I4");
            Assert.AreEqual(2, pd.PedigreeCount);
        }
    }
}
