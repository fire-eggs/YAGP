using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class GedEventParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet.Add("AGE", AgeProc);
            _tagSet.Add("DATE", DateProc);
            _tagSet.Add("TYPE", TypeProc);
            _tagSet.Add("CAUS", CausProc);
            _tagSet.Add("PLAC", PlacProc);
            _tagSet.Add("AGNC", AgncProc);
            _tagSet.Add("RELI", ReliProc);
            _tagSet.Add("RESN", ResnProc);
            _tagSet.Add("CHAN", ChanProc);
            _tagSet.Add("SOUR", SourProc);
            _tagSet.Add("NOTE", NoteProc);
// TODO            _tagSet.Add("OBJE", ObjeProc);

            _tagSet.Add("HUSB", HusbProc); // Family event support
            _tagSet.Add("WIFE", WifeProc); // Family event support

            _tagSet.Add("FAMC", FAMCProc); // BIRT / CHR / ADOP support
        }

        private void AgeProc()
        {
            (_rec as KBRGedEvent).Age = Remainder();
        }
        private void DateProc()
        {
            (_rec as KBRGedEvent).Date = Remainder();
        }
        private void TypeProc()
        {
            (_rec as KBRGedEvent).Type = Remainder();
        }
        private void CausProc()
        {
            (_rec as KBRGedEvent).Cause = Remainder();
        }
        private void PlacProc()
        {
            (_rec as KBRGedEvent).Place = Remainder();
        }
        private void AgncProc()
        {
            (_rec as KBRGedEvent).Agency = Remainder();
        }
        private void ReliProc()
        {
            (_rec as KBRGedEvent).Religion = Remainder();
        }
        private void ResnProc()
        {
            (_rec as KBRGedEvent).Restriction = Remainder();
        }

        private void FAMCProc()
        {
            (_rec as KBRGedEvent).Famc = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "ADOP")
                    (_rec as KBRGedEvent).FamcAdop = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }

        private void HusbProc()
        {
            (_rec as KBRGedEvent).HusbDetail = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    (_rec as KBRGedEvent).HusbAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }
        private void WifeProc()
        {
            (_rec as KBRGedEvent).WifeDetail = Remainder();
            if (_context.Endline > _context.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(_context.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    (_rec as KBRGedEvent).WifeAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }

        private void NoteProc()
        {
            // Multiple notes allowed
            (_rec as KBRGedEvent).Notes.Add(new Tuple<int, int>(_context.Begline, _context.Endline));
        }

        private void ChanProc()
        {
            // GEDCOM spec says to take the FIRST
            if ((_rec as KBRGedEvent).Change != null)
            {
                AddError("Multiple CHAN: first one used");
                return;
            }
            (_rec as KBRGedEvent).Change = new Tuple<int, int>(_context.Begline, _context.Endline);
        }

        private void ErrorTag(string tag, int startLineDex, int maxLineDex, string err)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            rec.Error = err;
            _rec.Errors.Add(rec);
        }

        private void AddError(string reason)
        {
            ErrorTag(_context.Tag, _context.Begline, _context.Endline, reason);
        }

        private void SourProc()
        {
            SourCitProc(_rec);
        }
    }
}
