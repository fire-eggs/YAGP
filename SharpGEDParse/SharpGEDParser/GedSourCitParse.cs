
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
        public override KBRGedRec Parse0(KBRGedRec rec, ParseContext context)
        {
            // TODO this is getting confusing; possibly single-threaded issue
            ctx = context;
            _rec = rec;

            // "1 SOUR @n@"
            // "1 SOUR text"
            // "1 SOUR text\n2 CONC text"

            string embed = null;
            string ident = null;
            int res = GedLineUtil.Ident(context.Line, context.Max, context.Nextchar, ref ident);
            if (res == -1 || string.IsNullOrWhiteSpace(ident))
            {
                embed = extendedText();
                if (string.IsNullOrEmpty(embed))
                {
                    ErrorRec("empty embedded source");
                    embed = null;
                }
                if (embed != null && embed.Contains("@"))
                {
                    ErrorRec("Invalid source reference");
                    return null;
                }
            }

            GedSourCit sRec = new GedSourCit(rec.Lines);
            sRec.Beg = context.Begline;
            sRec.End = context.Endline;
            sRec.XRef = ident;
            sRec.Embed = embed;
            Parse(sRec, ctx);
            return sRec;
        }

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
            var rec = _rec as GedSourCit;
            rec.Page = Remainder();
            if (rec.IsEmbedded)
            {
                ErrorRec("PAGE tag used with reference source");
            }
        }

        private void roleProc()
        {
            // TODO technically an error if not subordinate to an EVEN tag
            (_rec as GedSourCit).Role = Remainder();
        }

        private void evenProc()
        {
            var rec = _rec as GedSourCit;
            rec.Event = Remainder();
            if (rec.IsEmbedded)
            {
                ErrorRec("EVEN tag used with reference source");
            }
        }

        private void rinProc()
        {
            (_rec as GedSourCit).RIN = Remainder();
        }

        private void dataProc()
        {
            // The DATA line itself should have no relevance.
            // TODO create a singleton
            new SourCitDataParse().Parse(_rec, ctx);
        }

        private void textProc()
        {
            GedSourCit rec = _rec as GedSourCit;
            rec.Text = extendedText();
            if (!rec.IsEmbedded)
            {
                ErrorRec("TEXT tag used with reference source");
            }
        }
    }
}
