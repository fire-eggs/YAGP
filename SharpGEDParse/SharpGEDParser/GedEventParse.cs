using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class GedEventParse : GedRecParse
    {
        private static KBRGedEvent _rec;

        private delegate void EventTagProc();
        private Dictionary<string, EventTagProc> tagSet = new Dictionary<string, EventTagProc>();

        protected override void BuildTagSet()
        {
            tagSet.Add("AGE", AgeProc);
            tagSet.Add("DATE", DateProc);
            tagSet.Add("TYPE", TypeProc);
            tagSet.Add("CAUS", CausProc);
            tagSet.Add("PLAC", PlacProc);
            tagSet.Add("AGNC", AgncProc);
            tagSet.Add("RELI", ReliProc);
            tagSet.Add("RESN", ResnProc);
            tagSet.Add("CHAN", ChanProc);
            tagSet.Add("SOUR", SourProc);
            tagSet.Add("NOTE", NoteProc);
// TODO            tagSet.Add("OBJE", ObjeProc);

            tagSet.Add("HUSB", HusbProc); // Family event support
            tagSet.Add("WIFE", WifeProc); // Family event support

            tagSet.Add("FAMC", FAMCProc); // BIRT / CHR / ADOP support
        }

        protected override void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex)
        {
            string line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string tag = "";

            int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces

            if (tagSet.ContainsKey(tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                _context.Line = line;
                _context.Max = line.Length;
                _context.Tag = tag;
                _context.Begline = startLineDex;
                _context.Endline = maxLineDex;
                _context.Nextchar = nextChar;
                _rec = rec as KBRGedEvent;

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

        private string Remainder()
        {
            return _context.Line.Substring(_context.Nextchar).Trim();
        }

        private void AgeProc()
        {
            _rec.Age = Remainder();
        }
        private void DateProc()
        {
            _rec.Date = Remainder();
        }
        private void TypeProc()
        {
            _rec.Type = Remainder();
        }
        private void CausProc()
        {
            _rec.Cause = Remainder();
        }
        private void PlacProc()
        {
            _rec.Place = Remainder();
        }
        private void AgncProc()
        {
            _rec.Agency = Remainder();
        }
        private void ReliProc()
        {
            _rec.Religion = Remainder();
        }
        private void ResnProc()
        {
            _rec.Restriction = Remainder();
        }

        private void FAMCProc()
        {
            _rec.Famc = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = _rec.Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "ADOP")
                    _rec.FamcAdop = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }

        private void HusbProc()
        {
            _rec.HusbDetail = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = _rec.Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    _rec.HusbAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }
        private void WifeProc()
        {
            _rec.WifeDetail = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = _rec.Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    _rec.WifeAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }
        private void NoteProc()
        {
            // Multiple notes allowed
            _rec.Notes.Add(new Tuple<int, int>(_context.Begline, _context.Endline));
        }
        private void ChanProc()
        {
            // GEDCOM spec says to take the FIRST
            if (_rec.Change != null)
            {
                ErrorTag(_context.Tag, _context.Begline, _context.Endline, "Multiple CHAN: first one used");
                return;
            }
            _rec.Change = new Tuple<int, int>(_context.Begline, _context.Endline);
        }

        private void ErrorTag(string tag, int startLineDex, int maxLineDex, string err)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            rec.Error = err;
            _rec.Errors.Add(rec);
        }

        private void SourProc()
        {
            // "1 SOUR @n@"
            // TODO "1 SOUR descr"

            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);

            // TODO test missing ident
            // TODO missing ident as error
            if (res == -1)
                return;

            var rec = new SourceRec(ident);
            rec.Beg = _context.Begline;
            rec.End = _context.Endline;
            _rec.Sources.Add(rec);

            // TODO parse more stuff
        }
    }
}
