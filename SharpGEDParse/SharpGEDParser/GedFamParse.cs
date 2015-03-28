using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            _tagSet.Add("_UID", DataProc);

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
            (mRec as KBRGedIndi).Unknowns.Add(rec); // TODO general property?
        }

        private void SourProc(int begline, int endline, int nextchar)
        {
            // "1 SOUR @n@"
            // TODO "1 SOUR descr"

            string ident = "";
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);

            var rec = new SourceRec(ident);
            rec.Beg = begline;
            rec.End = endline;
            _rec.Sources.Add(rec);

            // TODO parse more stuff
            //            Console.WriteLine(rec);
        }

        private void NoteProc(int begline, int endline, int nextchar)
        {
            _rec.Note = new Tuple<int, int>(begline, endline);
        }

        private void kidProc(int begline, int endline, int nextchar)
        {
            // TODO test missing identifier
            // TODO missing ident is error
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1)
                _rec.Childs.Add(ident);
        }

        private void momProc(int begline, int endline, int nextchar)
        {
            // TODO test missing identifier
            // TODO missing ident is error
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1)
                _rec.Mom = ident;
        }

        private void dadProc(int begline, int endline, int nextchar)
        {
            // TODO test missing identifier
            // TODO missing ident is error
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, nextchar, ref ident);
            if (res != -1)
                _rec.Dad = ident;
        }

        private void ChanProc(int begline, int endline, int nextchar)
        {
            _rec.Change = new Tuple<int, int>(begline, endline);
        }
        private void DataProc(int begline, int endline, int nextchar)
        {
            string data = _context.Line.Substring(nextchar);
            var rec = new DataRec(_context.Tag, data);
            rec.Beg = begline;
            rec.End = endline;
            _rec.Data.Add(rec);
        }

        private void FamEventProc(int begline, int endline, int nextchar)
        {
            // A family event: same as an event but has additional husband, wife tags
            var rec = CommonEventProcessing(_rec.Lines);
            _rec.FamEvents.Add(rec);

            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, begline, endline, "HUSB") == null);
            Debug.Assert(KBRGedUtil.ParseFor(_rec.Lines, begline, endline, "WIFE") == null);
        }

    }
}
