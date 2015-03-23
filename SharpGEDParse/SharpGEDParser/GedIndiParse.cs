using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGEDParser
{
    // TODO need to handle bad lines as errors (missing level, etc)

    public class GedIndiParse : GedRecParse
    {
        private static KBRGedIndi _rec;

        protected override void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex)
        {
            string line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string tag = "";

            int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
            //            Console.WriteLine("____{2}({3}):{0}-{1}", startLineDex, maxLineDex, _tag, ident);

            if (tagSet.ContainsKey(tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                _context.Line = line;
                _context.Tag = tag;
                _context.begline = startLineDex;
                _context.endline = maxLineDex;
                _context.nextchar = nextChar;
                _rec = rec as KBRGedIndi;

                tagSet[tag]();
            }
            else
            {
                UnknownTag(tag, startLineDex, maxLineDex);
            }
        }

        private void UnknownTag(string tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            _rec.Unknowns.Add(rec);
        }

        private void ErrorTag(string err)
        {
            ErrorTag(_context.Tag, _context.begline, _context.endline, err);
        }

        private void ErrorTag(string tag, int startLineDex, int maxLineDex, string err)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            rec.Error = err;
            _rec.Errors.Add(rec);
        }

        private delegate void IndiTagProc();

        private Dictionary<string, IndiTagProc> tagSet = new Dictionary<string, IndiTagProc>();

        protected override void BuildTagSet()
        {
            // Details
            tagSet.Add("NAME", NameProc);
            tagSet.Add("RESN", RestrictProc);
            tagSet.Add("SEX",  SexProc);
            tagSet.Add("SUBM", SubmProc);
            tagSet.Add("ASSO", AssocProc);
            tagSet.Add("ALIA", AliasProc);
            tagSet.Add("ANCI", AnciProc);
            tagSet.Add("DESI", DesiProc);
            tagSet.Add("REFN", DataProc);
            tagSet.Add("SOUR", SourceProc);
            tagSet.Add("_UID", DataProc);
            tagSet.Add("NOTE", NoteProc);
            tagSet.Add("CHAN", ChanProc);
            tagSet.Add("OBJE", DataProc); // TODO temporary

            tagSet.Add("RFN", OneDataProc);
            tagSet.Add("AFN", OneDataProc);
            tagSet.Add("RIN", OneDataProc);

            // Non-standard tags
            tagSet.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            tagSet.Add("LVNG", LivingProc); // "Generations" custom

            // Events
            tagSet.Add("DEAT", EventProc);
            tagSet.Add("CREM", EventProc);
            tagSet.Add("BURI", EventProc);
            tagSet.Add("NATU", EventProc);
            tagSet.Add("IMMI", EventProc);
            tagSet.Add("WILL", EventProc);
            tagSet.Add("EMIG", EventProc);
            tagSet.Add("BAPM", EventProc);
            tagSet.Add("BARM", EventProc);
            tagSet.Add("BASM", EventProc);
            tagSet.Add("BLES", EventProc);
            tagSet.Add("CHRA", EventProc);
            tagSet.Add("CONF", EventProc);
            tagSet.Add("FCOM", EventProc);
            tagSet.Add("ORDN", EventProc);
            tagSet.Add("PROB", EventProc);
            tagSet.Add("GRAD", EventProc);
            tagSet.Add("RETI", EventProc);

            tagSet.Add("BIRT", BirtProc); // birth,adoption
            tagSet.Add("ADOP", BirtProc); // birth,adoption
            tagSet.Add("CHR",  BirtProc);

            // Attributes
            tagSet.Add("CAST", AttribProc);
            tagSet.Add("TITL", AttribProc);
            tagSet.Add("OCCU", AttribProc);
            tagSet.Add("FACT", AttribProc);
            tagSet.Add("DSCR", AttribProc);
            tagSet.Add("EDUC", AttribProc);
            tagSet.Add("IDNO", AttribProc);
            tagSet.Add("NATI", AttribProc);
            tagSet.Add("NCHI", AttribProc);
            tagSet.Add("NMR", AttribProc);
            tagSet.Add("PROP", AttribProc);
            tagSet.Add("RELI", AttribProc);
            tagSet.Add("SSN", AttribProc);

            // Family Events
            tagSet.Add("ANUL", FamEventProc);
            tagSet.Add("MARL", FamEventProc);
            tagSet.Add("MARS", FamEventProc);
            tagSet.Add("DIV", FamEventProc);
            tagSet.Add("DIVF", FamEventProc);
            tagSet.Add("ENGA", FamEventProc);
            tagSet.Add("MARB", FamEventProc);
            tagSet.Add("MARC", FamEventProc);

            // LDS events
            tagSet.Add("BAPL", LdsOrdProc);
            tagSet.Add("CONL", LdsOrdProc);
            tagSet.Add("ENDL", LdsOrdProc);
            tagSet.Add("SLGC", LdsOrdProc);
            tagSet.Add("SLGS", LdsOrdProc);

            // Family association
            tagSet.Add("FAMC", ChildLink);
            tagSet.Add("FAMS", SpouseLink);

            // Ambiguous tags
            tagSet.Add("EVEN", EventProc); // TODO simple or family event?
            tagSet.Add("CENS", EventProc); // TODO simple or family event?
            tagSet.Add("RESI", AttribProc);// TODO attribute or family event?

        }

        private void RestrictProc()
        {
            string data = _context.Line.Substring(_context.nextchar);
            if (string.IsNullOrWhiteSpace(data))
                return;
            _rec.Restriction = data.Trim();
        }

        private void DataProc()
        {
            string data = _context.Line.Substring(_context.nextchar);
            var rec = new DataRec(_context.Tag, data.Trim());
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _rec.Data.Add(rec);
        }

        // GEDCOM standard states only one of these allowed, take the first
        private void OneDataProc()
        {
            if (_rec.HasData(_context.Tag))
            {
                ErrorTag(string.Format("Multiple {0}: used first", _context.Tag));
                return;
            }
            DataProc();
        }

        private XRefRec CommonXRefProcessing()
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.nextchar, ref ident);
            var rec = new XRefRec(_context.Tag, ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            return rec;
        }

        private void DesiProc()
        {
            var rec = CommonXRefProcessing();
            _rec.Desi.Add(rec);
        }

        private void AnciProc()
        {
            // TODO one GED has 'HIGH', 'LOW' and 'MEDIUM'...
            var rec = CommonXRefProcessing();
            _rec.Anci.Add(rec);
        }

        private void AliasProc()
        {
            // TODO some samples have slashes; others names + /surname/
            string ident = _context.Line.Substring(_context.nextchar).Trim();
            var rec = new XRefRec(_context.Tag, ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _rec.Alia.Add(rec);
        }

        private void SubmProc()
        {
            // TODO some real GEDs not using Xref format
            // TODO log such as an error
            var rec = CommonXRefProcessing();
            _rec.Subm.Add(rec);
        }

        private void LivingProc()
        {
            // Some programs explicitly indicate 'living'. 
            _rec.Living = true;
        }

        private void SpouseLink()
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.nextchar, ref ident);

            var rec = new FamLinkRec(ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _rec.FamLinks.Add(rec);

            // TODO parse NOTE
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, 
                                             _context.begline, 
                                             _context.endline, "NOTE") == null);
        }

        private void ChildLink()
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.nextchar, ref ident);

            var rec = new ChildLinkRec(ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _rec.ChildLinks.Add(rec);

            // TODO parse PEDI, STAT, NOTE
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "NOTE") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "PEDI") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "STAT") == null);
        }

        private EventRec CommonEventProcessing()
        {
            var lines = _rec.Lines;
            int begline = _context.begline;
            int endline = _context.endline;

            var rec = new EventRec(_context.Tag);
            rec.Beg = begline;
            rec.End = endline;
            rec.Detail = _context.Line.Substring(_context.nextchar).Trim();

            rec.Date = KBRGedUtil.ParseFor(lines, begline + 1, endline, "DATE");
            rec.Place = KBRGedUtil.ParseFor(lines, begline + 1, endline, "PLAC");
            rec.Age = KBRGedUtil.ParseFor(lines, begline + 1, endline, "AGE");
            rec.Type = KBRGedUtil.ParseFor(lines, begline + 1, endline, "TYPE");

            rec.Change = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "CHAN");

            // TODO more than one note permitted!
            rec.Note = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");

            // TODO more than one source permitted!
            rec.Source = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");
            return rec;
        }

        private void EventProc()
        {
            // TODO a second CREM record is an error?
            // TODO a second DEAT record is an error?

            var rec = CommonEventProcessing();
            _rec.Events.Add(rec);
        }

        private void FamEventProc()
        {
            // A family event: same as an event but has additional husband, wife tags
            var rec = CommonEventProcessing();
            _rec.FamEvents.Add(rec);

            // TODO family event specific processing
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, _context.begline, _context.endline, "HUSB") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, _context.begline, _context.endline, "WIFE") == null);
        }

        private void BirtProc()
        {
            // TODO a second birth record is an error?

            var rec = CommonEventProcessing();
            _rec.Events.Add(rec);

            // TODO parse birt, adop specific
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, _context.begline, _context.endline, "FAMC") == null);
            // ADOP is a sub-tag on FAMC
            //            Debug.Assert(_tag == "BIRT" && KBRGedUtil.ParseFor(Lines, begline, endline, "ADOP") == null);
        }

        private void SexProc()
        {
            // TODO log unknown value as error
            // TODO this looks wrong - code smell
            int max = _context.Line.Length;
            int sexDex = KBRGedUtil.FirstChar(_context.Line, _context.nextchar, max);
            if (sexDex > 0)
                _rec.Sex = _context.Line[sexDex];
        }

        private void NoteProc()
        {
            // Multiple notes allowed
            _rec.Notes.Add(new Tuple<int, int>(_context.begline, _context.endline));
        }

        private void ChanProc()
        {
            // GEDCOM spec says to take the FIRST
            if (_rec.Change != null)
            {
                ErrorTag(_context.Tag, _context.begline, _context.endline, "Multiple CHAN: first one used");
                return;
            }
            _rec.Change = new Tuple<int, int>(_context.begline, _context.endline);
        }

        private void LdsOrdProc()
        {
            // TODO these all parse similarly except SLGC which requires a FAMC tag
            var lines = _rec.Lines;
            int begline = _context.begline;
            int endline = _context.endline;

            var rec = new LDSRec(_context.Tag);
            rec.Beg = begline;
            rec.End = endline;

            rec.Temple = KBRGedUtil.ParseFor(lines, begline + 1, endline, "TEMP");
            rec.Place = KBRGedUtil.ParseFor(lines, begline + 1, endline, "PLAC");

            // TODO SLGC specific?
            rec.Famc = KBRGedUtil.ParseFor(lines, begline + 1, endline, "FAMC");

            rec.Status = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "STAT");

            // TODO date for this record vs date for Status sub-record?
            rec.Date = KBRGedUtil.ParseFor(lines, begline + 1, endline, "DATE");

            // TODO more than one note permitted!
            rec.Note = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");

            // TODO more than one source permitted!
            rec.Source = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");

            _rec.LDSEvents.Add(rec);
        }

        private void AttribProc()
        {
            var rec = CommonEventProcessing();
            _rec.Attribs.Add(rec);
        }

        private void AssocProc()
        {
            var rec = CommonXRefProcessing();
            _rec.Assoc.Add(rec);

            // TODO parse RELA
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "RELA") == null);
            // TODO parse SOUR
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "SOUR") == null);
            // TODO parse NOTE
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines,
                                             _context.begline,
                                             _context.endline, "NOTE") == null);
        }

        private void NameProc()
        {
            string line = _context.Line;
            int nextchar = _context.nextchar;

            int max = line.Length;
            int startName = KBRGedUtil.FirstChar(line, nextchar, max);

            int startSur = KBRGedUtil.AllCharsUntil(line, max, startName, '/');
            int endSur = KBRGedUtil.AllCharsUntil(line, max, startSur + 1, '/');

            var suffix = "";
            if (endSur+1 < max)
                suffix = line.Substring(endSur+1).Trim();

            var rec = new NameRec();
            rec.Beg = _context.begline;
            rec.End = _context.endline;

            // TODO clean up extra spaces
            rec.Names = line.Substring(startName, startSur - startName).Trim();
            if (startSur < max) // e.g. "1 NAME LIVING"
                rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1);
            if (suffix.Length > 0)
                rec.Suffix = suffix;

            _rec.Names.Add(rec);

            // TODO parse more details
        }

        private void SourceProc()
        {
            // "1 SOUR @n@"
            // TODO "1 SOUR descr"

            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.nextchar, ref ident);

            var rec = new SourceRec(ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _rec.Sources.Add(rec);

            // TODO parse more stuff
        }

    }
}
