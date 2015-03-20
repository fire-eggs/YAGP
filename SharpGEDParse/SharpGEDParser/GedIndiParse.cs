using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGEDParser
{
    public class ParseContext
    {
        public string Line;
        public string Tag;
        public int begline;
        public int endline;
        public int nextchar;
        public KBRGedIndi rec;
    }

    public class GedIndiParse : GedParse
    {
        public GedIndiParse()
        {
            BuildTagSet();
            // TODO does this make parsing effectively single-threaded? need one context per thread?
            _context = new ParseContext();
        }

        private static ParseContext _context;

        public void Parse(KBRGedRec rec)
        {
            // At this point we know it is an INDI record and its ident.
            // TODO any trailing data after 'INDI' keyword?

            var Lines = rec.Lines;

            int linedex = 1;
            while (Lines.GetLevel(linedex) != '1')
                linedex++;
            if (linedex > Lines.Max)
                return;

            while (true)
            {
                int startrec = linedex;
                linedex++;
                if (linedex >= Lines.Max)
                    break;
                while (Lines.GetLevel(linedex) > '1')
                    linedex++;
                ParseSubRec(rec, startrec, linedex - 1);
                if (linedex >= Lines.Max)
                    break;
            }

            // TODO all this can be refactored in common for other record types (FAM,INDI, etc), except the "parseSub" delegate invocation
        }

        public void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex)
        {
            string _line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string _tag = "";

            int nextChar = KBRGedUtil.IdentAndTag(_line, 1, ref ident, ref _tag); //HACK assuming no leading spaces
            //            Console.WriteLine("____{2}({3}):{0}-{1}", startLineDex, maxLineDex, _tag, ident);

            if (tagSet.ContainsKey(_tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                _context.Line = _line;
                _context.Tag = _tag;
                _context.begline = startLineDex;
                _context.endline = maxLineDex;
                _context.nextchar = nextChar;
                _context.rec = rec as KBRGedIndi;

                tagSet[_tag]();
            }
            else
            {
                UnknownTag(rec, _tag, startLineDex, maxLineDex);
            }
        }

        private void UnknownTag(KBRGedRec mRec, string _tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(_tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            (mRec as KBRGedIndi).Unknowns.Add(rec); // TODO general property?
        }

        private delegate void IndiTagProc();

        private Dictionary<string, IndiTagProc> tagSet = new Dictionary<string, IndiTagProc>();

        private void BuildTagSet()
        {
            tagSet.Add("NAME", NameProc);
            tagSet.Add("RESN", DataProc); // TODO sufficient?
            tagSet.Add("SEX", SexProc);
            tagSet.Add("BAPM", EventProc); // simple event
            tagSet.Add("DEAT", EventProc); // simple event
            tagSet.Add("CREM", EventProc); // simple event
            tagSet.Add("BURI", EventProc); // simple event
            tagSet.Add("CHR", EventProc); // simple event
            tagSet.Add("NATU", EventProc);
            tagSet.Add("IMMI", EventProc);
            tagSet.Add("WILL", EventProc);
            tagSet.Add("EMIG", EventProc);

            tagSet.Add("EVEN", EventProc); // TODO simple or family event?

            tagSet.Add("BIRT", BirtProc); // birth,adoption
            tagSet.Add("ADOP", BirtProc); // birth,adoption

            tagSet.Add("CAST", AttribProc);
            tagSet.Add("TITL", AttribProc);
            tagSet.Add("RESI", AttribProc);
            tagSet.Add("OCCU", AttribProc);
            tagSet.Add("FACT", AttribProc);
            tagSet.Add("DSCR", AttribProc);

            tagSet.Add("ANUL", FamEventProc);
            tagSet.Add("MARL", FamEventProc);
            tagSet.Add("DIV", FamEventProc);

            tagSet.Add("BAPL", LdsOrdProc);

            tagSet.Add("FAMC", ChildLink);
            tagSet.Add("FAMS", SpouseLink);

            tagSet.Add("SUBM", SubmProc);
            tagSet.Add("ASSO", AssocProc);
            tagSet.Add("ALIA", AliasProc);
            tagSet.Add("ANCI", AnciProc);
            tagSet.Add("DESI", DesiProc);
            tagSet.Add("RFN", DataProc); // TODO is this sufficient?
            tagSet.Add("AFN", DataProc);
            tagSet.Add("REFN", DataProc);
            tagSet.Add("SOUR", SourceProc);
            tagSet.Add("_UID", DataProc);
            tagSet.Add("NOTE", NoteProc);
            tagSet.Add("CHAN", ChanProc);
            tagSet.Add("OBJE", DataProc); // TODO temporary

            tagSet.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            tagSet.Add("LVNG", LivingProc); // "Generations" custom
        }

        private void DataProc()
        {
            string data = _context.Line.Substring(_context.nextchar);
            var rec = new DataRec(_context.Tag, data);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _context.rec.Data.Add(rec);
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
            _context.rec.Desi.Add(rec);
        }

        private void AnciProc()
        {
            var rec = CommonXRefProcessing();
            _context.rec.Anci.Add(rec);
        }

        private void AliasProc()
        {
            var rec = CommonXRefProcessing();
            _context.rec.Alia.Add(rec);
        }

        private void SubmProc()
        {
            var rec = CommonXRefProcessing();
            _context.rec.Subm.Add(rec);
            // TODO sufficient? are there submitter details?
        }

        private void LivingProc()
        {
            // Some programs explicitly indicate 'living'. 
            _context.rec.Living = true;
        }

        private void SpouseLink()
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.nextchar, ref ident);

            var rec = new FamLinkRec(ident);
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            _context.rec.FamLinks.Add(rec);

            // TODO parse NOTE
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines, 
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
            _context.rec.ChildLinks.Add(rec);

            // TODO parse PEDI, STAT, NOTE
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines,
                                             _context.begline,
                                             _context.endline, "NOTE") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines,
                                             _context.begline,
                                             _context.endline, "PEDI") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines,
                                             _context.begline,
                                             _context.endline, "STAT") == null);
        }

        private EventRec CommonEventProcessing()
        {
            var lines = _context.rec.Lines;
            int begline = _context.begline;
            int endline = _context.endline;

            var rec = new EventRec(_context.Tag);
            rec.Beg = begline;
            rec.End = endline;

            rec.Date = KBRGedUtil.ParseFor(lines, begline + 1, endline, "DATE");
            rec.Place = KBRGedUtil.ParseFor(lines, begline + 1, endline, "PLAC");
            rec.Age = KBRGedUtil.ParseFor(lines, begline + 1, endline, "AGE");

            rec.Change = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "CHAN");
            rec.Note = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");
            rec.Source = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");
            return rec;
        }

        private void EventProc()
        {
            var rec = CommonEventProcessing();
            _context.rec.Events.Add(rec);
        }

        private void FamEventProc()
        {
            // A family event: same as an event but has additional husband, wife tags
            var rec = CommonEventProcessing();
            _context.rec.FamEvents.Add(rec);

            // TODO family event specific processing
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines, _context.begline, _context.endline, "HUSB") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines, _context.begline, _context.endline, "WIFE") == null);
        }

        private void BirtProc()
        {
            var rec = CommonEventProcessing();
            _context.rec.Events.Add(rec);

            // TODO parse birt, adop specific
            Debug.Assert(KBRGedUtil.ParseFor(_context.rec.Lines, _context.begline, _context.endline, "FAMC") == null);
            // ADOP is a sub-tag on FAMC
            //            Debug.Assert(_tag == "BIRT" && KBRGedUtil.ParseFor(Lines, begline, endline, "ADOP") == null);
        }

        private void SexProc()
        {
            _context.rec.Sex = _context.Line[_context.nextchar];
        }

        private void NoteProc()
        {
            Debug.Assert(_context.rec.Note == null);
            _context.rec.Note = new Tuple<int, int>(_context.begline, _context.endline);
        }
        private void ChanProc()
        {
            Debug.Assert(_context.rec.Change == null);
            _context.rec.Change = new Tuple<int, int>(_context.begline, _context.endline);
        }

        private void LdsOrdProc()
        {
            // TODO
        }

        private void AttribProc()
        {
            // TODO
        }

        private void AssocProc()
        {
            // TODO
        }

        private void NameProc()
        {
            string line = _context.Line;
            int nextchar = _context.nextchar;

            int max = line.Length;
            int startName = KBRGedUtil.FirstChar(line, nextchar, max);

            int startSur = KBRGedUtil.AllCharsUntil(line, max, startName, '/');
            int endSur = KBRGedUtil.AllCharsUntil(line, max, startSur + 1, '/');

            var rec = new NameRec();
            rec.Beg = _context.begline;
            rec.End = _context.endline;
            rec.Names = line.Substring(startName, startSur - startName).Trim();
            if (startSur < max) // e.g. "1 NAME LIVING"
                rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1);
            _context.rec.Names.Add(rec);

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
            _context.rec.Sources.Add(rec);

            // TODO parse more stuff
        }

    }
}
