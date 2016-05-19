using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class GedFamParse : GedRecParse
    {
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

        private void SourProc()
        {
            SourCitProc(_rec);
        }

        private void NoteProc()
        {
            _rec.Notes.Add(new Tuple<int, int>(_context.Begline, _context.Endline));
        }

        private void kidProc()
        {
            string ident = null;
            int res = GedLineUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                (_rec as KBRGedFam).Childs.Add(ident);
            else
            {
                ErrorRec("missing identifier");
            }
        }

        private void momProc()
        {
            string ident = null;
            int res = GedLineUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                (_rec as KBRGedFam).Mom = ident;
            else
            {
                ErrorRec("missing identifier");
            }
        }

        private void dadProc()
        {
            string ident = null;
            int res = GedLineUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);
            if (res != -1 && !string.IsNullOrEmpty(ident))
                (_rec as KBRGedFam).Dad = ident;
            else
            {
                ErrorRec("missing identifier");
            }
        }

        private void ChanProc()
        {
            // GEDCOM spec says only one change allowed; says to take the FIRST one
            if (_rec.Change == null)
                _rec.Change = new Tuple<int, int>(_context.Begline, _context.Endline);
            else
            {
                ErrorRec("More than one change record");
            }
        }

        private void DataProc()
        {
            string data = Remainder();
            var rec = new DataRec(_context.Tag, data);
            rec.Beg = _context.Begline;
            rec.End = _context.Endline;
            _rec.Data.Add(rec);
        }

        private GedParse _EventParseSingleton;

        private void FamEventProc()
        {
            // TODO push into GedEventParse

            var eRec = new KBRGedEvent(_rec.Lines, _context.Tag);
            eRec.Detail = _context.Line.Substring(_context.Nextchar).Trim();
            if (_EventParseSingleton == null)
                _EventParseSingleton = new GedEventParse();
            _EventParseSingleton.Parse(eRec, _context);

            (_rec as KBRGedFam).FamEvents.Add(eRec);
        }

    }
}
