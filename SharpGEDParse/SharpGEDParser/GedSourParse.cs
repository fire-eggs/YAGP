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
            // TODO many of these are wrong for a SOUR *record*

            _tagSet.Add("AUTH", authProc);
            _tagSet.Add("TITL", titlProc);
            _tagSet.Add("PAGE", pageProc);
            _tagSet.Add("DATA", ignoreProc);
            _tagSet.Add("QUAY", quayProc);
            _tagSet.Add("DATE", dateProc);
            _tagSet.Add("TEXT", textProc);
            _tagSet.Add("NOTE", noteProc);
            _tagSet.Add("CHAN", chngProc);
            _tagSet.Add("_RIN", rinProc);
        }

        private void rinProc()
        {
            (_rec as GedSource).RIN = Remainder();
        }

        private void chngProc()
        {
            // GEDCOM spec says only one change allowed; says to take the FIRST one
            if (_rec.Change == null)
                _rec.Change = new Tuple<int, int>(_context.Begline, _context.Endline);
            else
            {
                _rec.Errors.Add(ErrorRec("More than one change record"));
            }
        }

        private void noteProc()
        {
            throw new NotImplementedException();
        }

        private void textProc()
        {
            (_rec as GedSource).Text = Remainder();
            // TODO CONC/CONT
        }

        private void dateProc()
        {
            (_rec as GedSource).Date = Remainder();
        }

        private void quayProc()
        {
            (_rec as GedSource).Quay = Remainder();
        }

        private void ignoreProc()
        {
            // TODO do nothing?
        }

        private void pageProc()
        {
            (_rec as GedSource).Page = Remainder();
        }

        private void titlProc()
        {
            throw new NotImplementedException();
        }

        private void authProc()
        {
            throw new NotImplementedException();
        }
    }
}
