using System;

namespace SharpGEDParser
{
    public class GedSourParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet.Add("AUTH", authProc);
            _tagSet.Add("TITL", titlProc);
            _tagSet.Add("DATA", ignoreProc); // TODO is this the same as source-citation parsing?
            _tagSet.Add("REFN", refnProc);
            _tagSet.Add("TEXT", textProc);
            _tagSet.Add("NOTE", NoteProc);
            _tagSet.Add("CHAN", ChanProc);
            _tagSet.Add("RIN", rinProc);
            _tagSet.Add("OBJE", ignoreProc);
            _tagSet.Add("ABBR", abbrProc);
            _tagSet.Add("PUBL", publProc);
            // TODO source repository citation
        }

        private void rinProc()
        {
            // TODO push down to common record parsing?
            var rec = _rec as GedSource;
            if (rec.RIN != null)
            {
                ErrorRec("Multiple RIN: used first");
                return;
            }
            rec.RIN = Remainder();
        }

        private void abbrProc()
        {
            (_rec as GedSource).Abbreviation = Remainder(); // TODO validate
        }
        private void publProc()
        {
            (_rec as GedSource).Publication = extendedText(); // TODO validate
        }

        private void ChanProc() // TODO refactor to common - see GedIndiParse
        {
            // GEDCOM spec says only one change allowed; says to take the FIRST one
            if (_rec.Change == null)
                _rec.Change = new Tuple<int, int>(_context.Begline, _context.Endline);
            else
            {
                ErrorRec("More than one change record");
            }
        }

        private void textProc()
        {
            (_rec as GedSource).Text = extendedText();
        }

        private void ignoreProc()
        {
            // TODO flesh out DATA, MULTIMEDIA handling
        }

        private void titlProc()
        {
            (_rec as GedSource).Title = extendedText();
        }

        private void authProc()
        {
            (_rec as GedSource).Author = extendedText();
        }

        private void refnProc()
        {
            (_rec as GedSource).UserReferences.Add(Remainder());
            // TODO 'TYPE' subtag
        }

    }
}
