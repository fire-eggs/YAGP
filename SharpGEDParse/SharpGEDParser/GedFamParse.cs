using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class GedFamParse : GedRecParse
    {
        private static KBRGedFam _rec;

        private delegate void FamTagProc(int begLine, int endLine, int nextChar);

        private readonly Dictionary<string, FamTagProc> _tagSet = new Dictionary<string, FamTagProc>();

        protected override void BuildTagSet()
        {
            _tagSet.Add("HUSB", dadProc);
            _tagSet.Add("WIFE", momProc);
            _tagSet.Add("CHIL", kidProc);

            _tagSet.Add("NOTE", NoteProc);
            _tagSet.Add("SOUR", SourProc);
            _tagSet.Add("CHAN", ChanProc);

            //_tagSet.Add("_UID", DataProc); // TODO not seen?
            _tagSet.Add("_STAT", DataProc); // From 'AGES' program

            _tagSet.Add("EVEN", FamEventProc);
            _tagSet.Add("ANUL", FamEventProc);
            _tagSet.Add("MARL", FamEventProc);
            _tagSet.Add("MARS", FamEventProc);
            _tagSet.Add("DIV", FamEventProc);
            _tagSet.Add("DIVF", FamEventProc);
            _tagSet.Add("ENGA", FamEventProc);
            _tagSet.Add("MARB", FamEventProc);
            _tagSet.Add("MARC", FamEventProc);
            _tagSet.Add("MARR", FamEventProc);

            // TODO LDS Spouse sealing? SLGS
        }

        protected override void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex)
        {
            string line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string tag = "";

            int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
            if (_tagSet.ContainsKey(tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                _context.Line = line;
                _context.Max = line.Length;
                _context.Tag = tag;
                _context.Begline = startLineDex;
                _context.Endline = maxLineDex;
                _context.Nextchar = nextChar;
                _rec = rec as KBRGedFam;

                _tagSet[tag](startLineDex, maxLineDex, nextChar);
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
            (mRec as KBRGedFam).Unknowns.Add(rec); // TODO general property?
        }

        private void SourProc(int begline, int endline, int nextchar)
        {
            SourceProc(_rec);
        }

        private void NoteProc(int begline, int endline, int nextchar)
        {
            _rec.Notes.Add(new Tuple<int, int>(begline, endline));
        }

        private void kidProc(int begline, int endline, int nextchar)
        {
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                _rec.Childs.Add(ident);
            else
            {
                _rec.Errors.Add(ErrorRec("missing identifier"));
            }
        }

        private void momProc(int begline, int endline, int nextchar)
        {
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                _rec.Mom = ident;
            else
            {
                _rec.Errors.Add(ErrorRec("missing identifier"));
            }
        }

        private void dadProc(int begline, int endline, int nextchar)
        {
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                _rec.Dad = ident;
            else
            {
                _rec.Errors.Add(ErrorRec("missing identifier"));
            }
        }

        private void ChanProc(int begline, int endline, int nextchar)
        {
            // GEDCOM spec says only one change allowed; says to take the FIRST one
            if (_rec.Change == null)
                _rec.Change = new Tuple<int, int>(begline, endline);
            else
            {
                _rec.Errors.Add(ErrorRec("More than one change record"));
            }
        }

        private void DataProc(int begline, int endline, int nextchar)
        {
            string data = _context.Line.Substring(nextchar);
            var rec = new DataRec(_context.Tag, data);
            rec.Beg = begline;
            rec.End = endline;
            _rec.Data.Add(rec);
        }

        private GedParse _EventParseSingleton;

        private void FamEventProc(int begline, int endline, int nextchar)
        {
            var eRec = new KBRGedEvent(_rec.Lines, _context.Tag);
            eRec.Detail = _context.Line.Substring(_context.Nextchar).Trim();
            if (_EventParseSingleton == null)
                _EventParseSingleton = new GedEventParse();
            _EventParseSingleton.Parse(eRec, _context);

            _rec.FamEvents.Add(eRec);
        }

    }
}
