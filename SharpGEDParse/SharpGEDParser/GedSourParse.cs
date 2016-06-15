using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

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
            _tagSet.Add("_RIN", rinProc);
            _tagSet.Add("OBJE", ignoreProc);
            _tagSet.Add("ABBR", abbrProc);
            // TODO source repository citation
        }

        private void rinProc()
        {
            (_rec as GedSource).RIN = Remainder();
        }

        private void abbrProc()
        {
            (_rec as GedSource).Abbreviation = Remainder(); // TODO validate
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
            (_rec as GedSource).Text = Remainder();
            // TODO CONC/CONT
        }

        private void ignoreProc()
        {
            // TODO flesh out DATA, MULTIMEDIA handling
        }

        private void titlProc()
        {
            (_rec as GedSource).Title = Remainder();
            // TODO CONC/CONT
        }

        private void authProc()
        {
            (_rec as GedSource).Author = Remainder();
            // TODO CONC/CONT
        }

        private void refnProc()
        {
            (_rec as GedSource).UserReferences.Add(Remainder());
            // TODO 'TYPE' subtag
        }

    }
}
