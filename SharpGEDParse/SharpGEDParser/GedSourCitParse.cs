using System;

namespace SharpGEDParser
{
    // Parse Source Citation (as opposed to Source Records)
    public class GedSourCitParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet.Add("PAGE", pageProc);
            _tagSet.Add("EVEN", evenProc);
            _tagSet.Add("ROLE", roleProc);
            _tagSet.Add("DATA", dataProc);
            _tagSet.Add("OBJE", ignoreProc);
            _tagSet.Add("NOTE", NoteProc);
            _tagSet.Add("QUAY", quayProc);

            _tagSet.Add("_RIN", rinProc); // Non-standard
        }

        private void quayProc()
        {
            (_rec as GedSourCit).Quay = Remainder();
        }

        private void ignoreProc() // TODO
        {
            //throw new NotImplementedException();
        }

        private void pageProc()
        {
            (_rec as GedSourCit).Page = Remainder();
        }

        private void roleProc()
        {
            (_rec as GedSourCit).Role = Remainder();
        }

        private void evenProc()
        {
            (_rec as GedSourCit).Event = Remainder();
        }
        private void rinProc()
        {
            (_rec as GedSourCit).RIN = Remainder();
        }

        private void dataProc()
        {
            // The DATA line itself should have no relevance.
            // TODO create a singleton
            new SourCitDataParse().Parse(_rec, _context);
        }
    }
}
