using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGEDParser
{

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
                UnknownTag(rec, tag, startLineDex, maxLineDex);
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

        protected override void BuildTagSet()
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
            _rec.Data.Add(rec);
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
            var rec = CommonXRefProcessing();
            _rec.Subm.Add(rec);
            // TODO sufficient? are there submitter details?
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

            rec.Date = KBRGedUtil.ParseFor(lines, begline + 1, endline, "DATE");
            rec.Place = KBRGedUtil.ParseFor(lines, begline + 1, endline, "PLAC");
            rec.Age = KBRGedUtil.ParseFor(lines, begline + 1, endline, "AGE");
            rec.Type = KBRGedUtil.ParseFor(lines, begline + 1, endline, "TYPE");

            rec.Change = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "CHAN");
            rec.Note = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");

            // TODO more than one source permitted!
            rec.Source = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");
            return rec;
        }

        private void EventProc()
        {
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
            var rec = CommonEventProcessing();
            _rec.Events.Add(rec);

            // TODO parse birt, adop specific
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, _context.begline, _context.endline, "FAMC") == null);
            // ADOP is a sub-tag on FAMC
            //            Debug.Assert(_tag == "BIRT" && KBRGedUtil.ParseFor(Lines, begline, endline, "ADOP") == null);
        }

        private void SexProc()
        {
            // TODO this looks wrong - code smell
            int max = _context.Line.Length;
            int sexDex = KBRGedUtil.FirstChar(_context.Line, _context.nextchar, max);
            if (sexDex > 0)
                _rec.Sex = _context.Line[sexDex];
        }

        private void NoteProc()
        {
            Debug.Assert(_rec.Note == null);
            _rec.Note = new Tuple<int, int>(_context.begline, _context.endline);
        }
        private void ChanProc()
        {
            Debug.Assert(_rec.Change == null);
            _rec.Change = new Tuple<int, int>(_context.begline, _context.endline);
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
