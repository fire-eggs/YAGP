using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
	[TestFixture ()]
	public class RepositoryTest : GedParseTest
	{
		// TODO this is temporary until GEDCommon replaces KBRGedRec
		public new static List<GEDCommon> ReadIt(string testString)
		{
			var fr = ReadItHigher(testString);
			return fr.Data.Select(o => o as GEDCommon).ToList();
		}

		[Test ()]
		public void TestSimple1()
		{
			var txt = "0 @R1@ REPO\n1 NAME foobar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual("foobar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test ()]
		public void TestSimple2()
		{
			var txt = "0 @R1@ REPO\n1 RIN foobar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual("foobar", rec.RIN);
			Assert.AreEqual("R1", rec.Ident);
		}

	    [Test()]
	    public void TestMissingName()
	    {
	        // NAME record is required
	        var txt = "0 @R1@ REPO";
	        var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            GedRepository rec = res[0] as GedRepository;
            Assert.IsNotNull(rec);
            Assert.AreEqual("R1", rec.Ident);
            Assert.AreEqual(1, rec.Errors.Count); // TODO error details
        }

		[Test ()]
		public void TestCust1()
		{
			var txt = "0 @R1@ REPO\n1 _CUST foobar\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test ()]
		public void TestCust2()
		{
			// multi-line custom tag
			var txt = "0 @R1@ REPO\n1 _CUST foobar\n2 CONC foobar2\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(2, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test ()]
		public void TestCust3()
		{
			// custom tag at the end of the record
			var txt = "0 @R1@ REPO\n1 NAME fumbar\n1 _CUST foobar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(1, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Unknowns[0].LineCount);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual("R1", rec.Ident);
		}

		[Test ()]
		public void TestREFN()
		{
			// single REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
		}
		[Test ()]
		public void TestREFNs()
		{
			// multiple REFNs
			var txt = "0 @R1@ REPO\n1 REFN 001\n1 NAME fumbar\n1 REFN 002\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(2, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
			Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
		}

		[Test ()]
		public void TestREFNExtra()
		{
			// extra on REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
			Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
		}

		[Test ()]
		public void TestREFNExtra2()
		{
			// multi-line extra on REFN
			var txt = "0 @R1@ REPO\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
			Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
		}

		[Test ()]
		public void TestMissingId()
		{
			// empty record; missing id
			var txt = "0 REPO";
			var res = ReadItHigher(txt);
			Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
			Assert.AreEqual(1, res.Data.Count);
			Assert.AreEqual(2, (res.Data[0] as GEDCommon).Errors.Count);
		}

		[Test ()]
		public void TestMissingId2()
		{
			// missing id
			var txt = "0 REPO\n1 NAME foobar\n0 KLUDGE";
			var res = ReadItHigher(txt);
			Assert.AreEqual(0, res.Errors.Count);
			Assert.AreEqual(1, res.Data.Count);
			GedRepository rec = res.Data[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual("foobar", rec.Name);
		}

		[Test ()]
		public void TestChan()
		{
			var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 APR 2000\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			var res2 = rec.CHAN;
			Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
		}

		[Test ()]
		public void TestChan2()
		{
			// no date for chan
            var txt = "0 @R1@ REPO\n1 CHAN\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Errors.Count);
		}

		[Test ()]
		public void TestChan3()
		{
			var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 APR 2000\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			var res2 = rec.CHAN;
			Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
		}

		[Test ()]
		public void TestChan4()
		{
			// no date value
			var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE\n1 NAME fumbar\n0 KLUDGE";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
		}

		[Test ()]
		public void TestChan5()
		{
			// extra
			var txt = "0 @R1@ REPO\n1 CHAN\n2 CUSTOM foo\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			ChangeRec chan = rec.CHAN;
			Assert.AreEqual(1, chan.OtherLines.Count);
		}
		[Test ()]
		public void TestChan6()
		{
			// multi line extra
			var txt = "0 @R1@ REPO\n1 CHAN\n2 CUSTOM foo\n3 _BLAH bar\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			ChangeRec chan = rec.CHAN;
			Assert.AreEqual(1, chan.OtherLines.Count);
		}

		[Test ()]
		public void TestChan7()
		{
			// multiple CHAN
			var txt = "0 @R1@ REPO\n1 CHAN\n2 DATE 1 MAR 2000\n1 NAME fumbar\n1 CHAN";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
		}

		[Test ()]
		public void TestNote1()
		{
			// simple note
			var txt = "0 @R1@ REPO\n1 NOTE @N1@\n1 REFN 001\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("N1", rec.Notes[0].Xref);
		}
		[Test ()]
		public void TestNote2()
		{
			// simple note
			var txt = "0 @R1@ REPO\n1 NOTE blah blah blah\n1 REFN 001\n1 NAME fumbar";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual("fumbar", rec.Name);
			Assert.AreEqual(1, rec.Ids.REFNs.Count);
			Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("blah blah blah", rec.Notes[0].Text);
		}

		[Test ()]
		public void TestNote()
		{
			var indi = "0 REPO @R1@\n1 NOTE";
			var res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("", rec.Notes[0].Text);

			indi = "0 REPO @R1@\n1 NOTE notes\n2 CONT more detail";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);

			indi = "0 REPO @R1@\n1 NOTE notes\n2 CONT more detail\n1 NAME foo\n1 NOTE notes2";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(2, rec.Notes.Count);
			Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
			Assert.AreEqual("notes2", rec.Notes[1].Text);

			// trailing space must be preserved
			indi = "0 REPO @R1@\n1 NOTE notes\n2 CONC more detail \n2 CONC yet more detail";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notesmore detail yet more detail", rec.Notes[0].Text);

			indi = "0 REPO @R1@\n1 NOTE notes \n2 CONC more detail \n2 CONC yet more detail ";
			res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("notes more detail yet more detail ", rec.Notes[0].Text);
		}

		[Test ()]
		public void TestNoteOther()
		{
			// exercise other lines
			var indi = "0 REPO @R1@\n1 NOTE\n2 OTHR gibber";
			var res = ReadIt(indi);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("", rec.Notes[0].Text);
			Assert.AreEqual(1, rec.Notes[0].OtherLines.Count);
		}

		[Test ()]
		public void TestChanNote()
		{
			var txt = "0 @R1@ REPO\n1 CHAN\n2 NOTE @N1@\n2 DATE 1 APR 2000";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			var res2 = rec.CHAN;
			Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
			Assert.AreEqual(1, res2.Notes.Count);
			Assert.AreEqual("N1", res2.Notes[0].Xref);

			txt = "0 REPO @R1@\n1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000";
			res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			res2 = rec.CHAN;
			Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
			Assert.AreEqual(1, res2.Notes.Count);
			Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text);

		}

		[Test ()]
		public void TestAddr()
		{
			// Real REPO record taken from a downloaded GED
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
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

		[Test ()]
		public void TestAddrOther()
		{
			// other lines
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)\n3 OTHR gibber";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
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

		[Test ()]
		public void TestAddr2()
		{
			// ADDR _not_ the last sub-record
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR1 York Register Office\n2 ADR2 56 Bootham\n2 CITY York,\n2 POST YO30 7DA\n2 CTRY England (UK)\n1 NOTE blah blah";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
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
		[Test ()]
		public void TestAddr3()
		{
			// additional ADDR sub-tags
			var txt = "0 @R29@ REPO\n1 NAME Superintendent Registrar (York)\n1 ADDR York Register Office\n2 CONT 56 Bootham\n2 CONT York,,  YO30 7DA\n2 CONT England (UK)\n2 ADR3 York Register Office\n2 ADR2 56 Bootham\n2 STAE York,\n2 PHON YO30 7DA\n2 FAX blah\n2 EMAIL blah\n2 WWW blah\n2 CTRY England (UK)\n1 NOTE blah blah";
			var res = ReadIt(txt);
			Assert.AreEqual(1, res.Count);
			GedRepository rec = res[0] as GedRepository;
			Assert.IsNotNull(rec);
			Assert.AreEqual(0, rec.Errors.Count);
			Assert.AreEqual(0, rec.Unknowns.Count);
			Assert.AreEqual(1, rec.Notes.Count);
			Assert.AreEqual("Superintendent Registrar (York)", rec.Name);
			Assert.IsNotNull(rec.Addr);
			Assert.AreEqual("56 Bootham", rec.Addr.Adr2);
			Assert.AreEqual("York Register Office", rec.Addr.Adr3);
			Assert.AreEqual("York,", rec.Addr.Stae);
			Assert.AreEqual("YO30 7DA", rec.Addr.Phon);
			Assert.AreEqual("blah", rec.Addr.Fax);
			Assert.AreEqual("blah", rec.Addr.Email);
			Assert.AreEqual("blah", rec.Addr.WWW);
		}

	}
}

