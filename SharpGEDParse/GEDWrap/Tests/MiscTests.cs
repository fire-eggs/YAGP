﻿using NUnit.Framework;
using SharpGEDParser;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local

namespace GEDWrap.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class MiscTests : TestUtil
    {
        [Test]
        public void DuplIndi()
        {
            // Duplicated INDI
            var txt = "0 @I1@ INDI\n0 @I1@ INDI";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.DUPL_INDI, f.Issues.First().IssueId);
            Assert.AreEqual(1, f.Errors.Count);  // TODO verify IdentCollide
        }
        [Test]
        public void DuplFam()
        {
            // Duplicated INDI
            var txt = "0 @I1@ FAM\n0 @I1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.DUPL_FAM, f.Issues.First().IssueId);
            Assert.AreEqual(1, f.Errors.Count);  // TODO verify IdentCollide
        }

        [Test]
        public void BadIndiLink()
        {
            // 20180125 missing/bad xref for FAMS/FAMC will not create an IndiLink
            // Error in INDI link out
            var txt = "0 @I1@ INDI\n1 FAMC\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MissIdent, f.Errors[0].Error);

            //Assert.AreEqual(1, f.ErrorsCount);
            //Assert.AreEqual(Issue.IssueCode.MISS_XREFID, f.Issues.First().IssueId);
        }

        [Test]
        public void BadFamLink()
        {
            // Error in FAM link out
            var txt = "0 @I1@ INDI\n0 @F1@ FAM\n1 CHIL";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(1, f.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MissIdent, f.Errors[0].Error);
        }

        [Test]
        public void MultiTree()
        {
            var txt = "0 @I1@ INDI\n0 @I2@ INDI";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(0, f.ErrorsCount);
                Assert.AreEqual(0, f.Errors.Count);
                Assert.AreEqual(2, f.NumberOfTrees);
            }
        }

        [Test]
        public void MultiTree2()
        {
            // A specific code coverage case: treenum != -1
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @I3@ INDI\n0 @F1@ FAM\n1 CHIL @I1@\n1 CHIL @I3@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(2, f.NumberOfTrees);
            }
        }

        [Test]
        public void MissFamId()
        {
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @ @ FAM\n1 CHIL @I1@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(1, f.ErrorsCount);
                Assert.AreEqual(Issue.IssueCode.MISS_FAMID, f.Issues.First().IssueId);
                Assert.AreEqual(1, f.Errors.Count);
                Assert.AreEqual(UnkRec.ErrorCode.MissIdent, f.Errors[0].Error);
            }
        }

        [Test,Ignore]
        public void MissIndiId()
        {
            // code coverage - generated id
            var txt = "0 @ @ INDI\n0 @I2@ INDI\n0 @F1@ FAM\n1 CHIL @I1@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(1, f.ErrorsCount);
                Assert.AreEqual(Issue.IssueCode.CHIL_MISS, f.Issues.First().IssueId);
                Assert.AreEqual(1, f.Errors.Count);
                Assert.AreEqual(UnkRec.ErrorCode.MissIdent, f.Errors[0].Error);
                var id = f.AllIndiIds.First();
                Assert.IsTrue(id.StartsWith("A"));
            }
        }

        [Test]
        public void Props()
        {
            // Code coverage: invoke some more Forest properties
            var txt = "0 @I1@ INDI\n1 FAMC @F1@\n1 FAMS @F2@\n0 @F1@ FAM\n1 CHIL @I1@\n0 @F2@ FAM\n1 HUSB @I1@";
            Forest f = LoadGEDFromStream(txt);

            Assert.AreEqual(1, f.AllIndiIds.Count());
            Assert.AreEqual(3, f.AllRecords.Count());
            Assert.IsNull(f.Header);
            Assert.AreEqual(7, f.NumberOfLines);
        }

        [Test]
        public void Empty()
        {
            var txt = "blah";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.AllRecords.Count);
            Assert.AreEqual(1, f.Errors.Count);
            Assert.AreEqual(0, f.ErrorsCount);
        }
    }
}
