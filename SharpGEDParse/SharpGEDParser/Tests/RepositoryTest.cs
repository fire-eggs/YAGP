using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
	[TestFixture]
	public class RepositoryTest : GedParseTest
	{
		[Test]
		public void TestSimple1()
		{
			var txt = "0 @R1@ REPO\n1 NAME foobar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
            Assert.AreEqual("REPO", rec.Tag);
			Assert.AreEqual("foobar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test]
		public void TestSimple2()
		{
			var txt = "0 @R1@ REPO\n1 NAME fumbar\n1 RIN foobar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
            Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("foobar", rec.RIN);
			Assert.AreEqual("R1", rec.Ident);
		}

	    [Test]
	    public void TestMissingName()
	    {
	        // NAME record is required
	        var txt = "0 @R1@ REPO\n";
	        var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("R1", rec.Ident);
            Assert.AreEqual(1, rec.Errors.Count); // TODO error details
        }

	    [Test]
	    public void ExtraText()
	    {
            var txt = "0 @R1@ REPO supercalifrag\n1 RIN foobar\n1 NAME fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("R1", rec.Ident);
        }

		[Test]
		public void TestCust1()
		{
			var txt = "0 @R1@ REPO\n1 _CUST foobar\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test]
		public void TestCust2()
		{
			// multi-line custom tag
			var txt = "0 @R1@ REPO\n1 _CUST foobar\n2 CONC foobar2\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(2, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test]
		public void TestCust3()
		{
			// custom tag at the end of the record
			var txt = "0 @R1@ REPO\n1 NAME fumbar\n1 _CUST foobar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test]
		public void TestREFN()
		{
			// single REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
		}
		[Test]
		public void TestREFNs()
		{
			// multiple REFNs
			var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar\n1 REFN 002";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(2, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
			Assert.AreEqual("002", rec.REFNs[1].Value);
		}

		[Test]
		public void TestREFNExtra()
		{
			// extra on REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
			Assert.AreEqual(1, rec.REFNs[0].Extra.LineCount);
		}

		[Test]
		public void TestREFNExtra2()
		{
			// multi-line extra on REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
			Assert.AreEqual(2, rec.REFNs[0].Extra.LineCount);
		}

		[Test]
		public void TestMissingId()
		{
			// empty record; missing id
			var txt = "0 REPO\n";
			var res = ReadItHigher(txt);
            // TODO 'empty record' no longer occurring. Valid?
			Assert.AreEqual(0, res.Errors.Count);
			Assert.AreEqual(1, res.Data.Count);
			Assert.AreEqual(2, (res.Data[0] as GEDCommon).Errors.Count); // TODO validate error details
        }

		[Test]
		public void TestMissingId2()
		{
			// missing id
			var txt = "0 REPO\n1 NAME foobar";
			var res = ReadItHigher(txt);
			Assert.AreEqual(0, res.Errors.Count);
			Assert.AreEqual(1, res.Data.Count);
			Repository rec = res.Data[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual("foobar", rec.Name);
		}

		[Test]
		public void TestNote1()
		{
			// simple note
			var txt = "0 @R1@ REPO\n1 NOTE @N1@\n1 REFN 001\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("N1", rec.Notes[0].Xref);
		}
		[Test]
		public void TestNote2()
		{
			// simple note
			var txt = "0 @R1@ REPO\n1 NOTE blah blah blah\n1 REFN 001\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.REFNs.Count);
			Assert.AreEqual("001", rec.REFNs[0].Value);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("blah blah blah", rec.Notes[0].Text);
		}

		[Test]
		public void TestNote()
		{
            var indi = "0 @R1@ REPO\n1 NAME fumbar\n1 NOTE";
			var res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("", rec.Notes[0].Text);

            indi = "0 @R1@ REPO\n1 NAME fumbar\n1 NOTE notes\n2 CONT more detail";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as Repository;
			Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);

            indi = "0 @R1@ REPO\n1 NAME fumbar\n1 NOTE notes\n2 CONT more detail\n1 NAME foo\n1 NOTE notes2";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as Repository;
			Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(2, rec.Notes.Count);
			Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
			Assert.AreEqual("notes2", rec.Notes[1].Text);

			// trailing space must be preserved
            indi = "0 @R1@ REPO\n1 NAME fumbar\n1 NOTE notes\n2 CONC more detail \n2 CONC yet more detail";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notesmore detail yet more detail", rec.Notes[0].Text);

            indi = "0 @R1@ REPO\n1 NAME fumbar\n1 NOTE notes \n2 CONC more detail \n2 CONC yet more detail ";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notes more detail yet more detail ", rec.Notes[0].Text);
		}

		[Test]
		public void TestNoteOther()
		{
			// exercise other lines
            var indi = "0 @R1@ REPO\n1 NOTE\n2 OTHR gibber";
			var res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("", rec.Notes[0].Text);
			Assert.AreEqual(1, rec.Notes[0].OtherLines.Count);
		}

		[Test]
		public void TestAddr()
		{
			// Real REPO record taken from a downloaded GED
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("Superintendent Registrar (York)", rec.Name);
			Assert.IsNotNull(rec.Addr);
			Assert.AreEqual("York Register Office\n56 Bootham\nYork,,  YO30 7DA\nEngland (UK)", rec.Addr.Adr);
			Assert.AreEqual("York Register Office", rec.Addr.Adr1);
			Assert.AreEqual("York,", rec.Addr.City);
			Assert.AreEqual("YO30 7DA", rec.Addr.Post);
		}

		[Test]
		public void TestAddrOther()
		{
			// other lines
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)\n3 OTHR gibber";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("Superintendent Registrar (York)", rec.Name);
			Assert.IsNotNull(rec.Addr);
			Assert.AreEqual("York Register Office\n56 Bootham\nYork,,  YO30 7DA\nEngland (UK)", rec.Addr.Adr);
			Assert.AreEqual("York Register Office", rec.Addr.Adr1);
			Assert.AreEqual("York,", rec.Addr.City);
			Assert.AreEqual("YO30 7DA", rec.Addr.Post);
			Assert.AreEqual(1, rec.Addr.OtherLines.Count);
		}

		[Test]
		public void TestAddr2()
		{
			// ADDR _not_ the last sub-record
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)\n1 NOTE blah blah";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("Superintendent Registrar (York)", rec.Name);
			Assert.IsNotNull(rec.Addr);
			Assert.AreEqual("York Register Office\n56 Bootham\nYork,,  YO30 7DA\nEngland (UK)", rec.Addr.Adr);
			Assert.AreEqual("York Register Office", rec.Addr.Adr1);
			Assert.AreEqual("York,", rec.Addr.City);
			Assert.AreEqual("YO30 7DA", rec.Addr.Post);
		}

        // TODO use of PHON/EMAIL/WWW/FAX without the required ADDR tag
        // TODO up to 3 instances of PHON/EMAIL/FAX/WWW allowed

		[Test]
		public void TestAddr3()
		{
			// additional ADDR sub-tags
            // NOTE: PHON/EMAIL/WWW/FAX tags are NOT subordinate to the ADDR tag!
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR3 York Register Office\n2 ADR2 56 Bootham\n2 STAE York,\n1 PHON YO30 7DA\n1 FAX blah\n1 EMAIL blah\n1 WWW blah\n2 CTRY England (UK)\n1 NOTE blah blah";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			Repository rec = res[0] as Repository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count); // trailing CTRY tag 
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("Superintendent Registrar (York)", rec.Name);
			Assert.IsNotNull(rec.Addr);
			Assert.AreEqual("56 Bootham", rec.Addr.Adr2);
			Assert.AreEqual("York Register Office", rec.Addr.Adr3);
			Assert.AreEqual("York,", rec.Addr.Stae);
			Assert.AreEqual("YO30 7DA", rec.Addr.Phon[0]);
			Assert.AreEqual("blah", rec.Addr.Fax[0]);
			Assert.AreEqual("blah", rec.Addr.Email[0]);
			Assert.AreEqual("blah", rec.Addr.WWW[0]);
		}

        [Test]
        public void Repo_Rfn()
        {
            // TODO REPO.RFN is non-standard: possibly from Ancestry.Com
            var txt = "0 @R1@ REPO\n1 RFN blah\n";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);
            
            // REPO.RFN is a non-standard tag
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].Beg);
        }

        [Test]
        public void Repo_Caln()
        {
            // TODO not a standard tag. Apparently exists from FamilyTreeMaker? See Gene.Genie
            var txt = "0 @R1@ REPO\n1 CALN blah\n";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            Repository rec = res[0] as Repository;
            Assert.IsNotNull(rec);

            // REPO.CALN is a common but non-standard tag
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].Beg);
        }
    }
}

