using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    // SOURCE record parsing
    public class SourceRecParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add(GedTag.REFN, RefnProc);
            _tagSet2.Add(GedTag.RIN, RinProc);
            _tagSet2.Add(GedTag.CHAN, ChanProc);

            _tagSet2.Add(GedTag.ABBR, abbrProc);
            _tagSet2.Add(GedTag.AUTH, authProc);
            _tagSet2.Add(GedTag.DATA, dataProc);
            _tagSet2.Add(GedTag.NOTE, NoteProc);
            _tagSet2.Add(GedTag.OBJE, ObjeProc);
            _tagSet2.Add(GedTag.PUBL, publProc);
            _tagSet2.Add(GedTag.REPO, repoProc);
            _tagSet2.Add(GedTag.TEXT, textProc);
            _tagSet2.Add(GedTag.TITL, titlProc);

            // NOTE: technically not required: FamilySearch recommends INDI/FAM only. Used by MyHeritage.
            _tagSet2.Add(GedTag._UID, UidProc);
            _tagSet2.Add(GedTag.UID, UidProc);
        }

        private void abbrProc(ParseContext2 context)
        {
            (context.Parent as SourceRecord).Abbreviation = context.Remain;
        }

        private void authProc(ParseContext2 context)
        {
            string val = extendedText(context);
            (context.Parent as SourceRecord).Author = val;
        }

        private void dataProc(ParseContext2 context)
        {
            var data = SourceDataParse.DataParser(context);
            (context.Parent as SourceRecord).Data = data;

            // TODO validate multiple DATA records
        }

        private void publProc(ParseContext2 context)
        {
            string val = extendedText(context);
            (context.Parent as SourceRecord).Publication = val;
        }

        private void repoProc(ParseContext2 context)
        {
            RepoCit cit = RepoCitParse.CitParser(context);
            (context.Parent as SourceRecord).Cits.Add(cit);
        }

        private void textProc(ParseContext2 context)
        {
            string val = extendedText(context);
            (context.Parent as SourceRecord).Text = val;
        }

        private void titlProc(ParseContext2 context)
        {
            string val = extendedText(context);
            (context.Parent as SourceRecord).Title = val;
        }

        public override void PostCheck(GEDCommon rec)
        {
            SourceRecord me = rec as SourceRecord;

            if (string.IsNullOrWhiteSpace(me.Ident)) // TODO common?
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO {Error = "Missing identifier"};
                err.Beg = err.End = rec.BegLine;
                me.Errors.Add(err);
            }

            // No required data except Xref id?
            // TODO Warning: no data provide
        }
    }
}
