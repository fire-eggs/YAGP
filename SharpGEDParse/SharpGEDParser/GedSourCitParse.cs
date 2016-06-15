
namespace SharpGEDParser
{
    // Parse Source Citation (as opposed to Source Records)
    //
    // This is a SOUR structure off a parent. The following GEDCOM standard objects can have source citations:
    // PERSONAL_NAME_PIECES
    // LDS_SPOUSE_SEALING
    // LDS_INDIVIDUAL_ORDINANCE
    // EVENT_DETAIL
    // ASSOCIATION_STRUCTURE
    // MULTIMEDIA_RECORD
    // INDIVIDUAL_RECORD
    // FAM_RECORD
    // There are two kinds of source citations:
    // i. "Pointer to source record" n SOUR @<xref>@
    // ii. "Not using  source records" n SOUR <description>
    // Parsing here is common for both types.
    //
    // TODO DATA, PAGE, EVEN tags are technically wrong for "Not using source records"
    // TODO TEXT tag is technically wrong for "Pointer to source record"

    public class GedSourCitParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet.Add("PAGE", pageProc);  // "pointer to source record" specific
            _tagSet.Add("EVEN", evenProc);  // "pointer to source record" specific
            _tagSet.Add("ROLE", roleProc);
            _tagSet.Add("DATA", dataProc);  // "pointer to source record" specific
            _tagSet.Add("OBJE", ignoreProc);
            _tagSet.Add("NOTE", NoteProc);
            _tagSet.Add("QUAY", quayProc);
            _tagSet.Add("TEXT", textProc); // "not using source records" specific

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
            // TODO technically an error if not subordinate to an EVEN tag
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

        private void textProc()
        {
            
        }
    }
}
