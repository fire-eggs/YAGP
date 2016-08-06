using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // SOURCE record parsing
    public class SourceRecParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN", RinProc);
            _tagSet2.Add("CHAN", ChanProc);

            _tagSet2.Add("ABBR", abbrProc);
            _tagSet2.Add("AUTH", authProc);
            _tagSet2.Add("DATA", dataProc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("OBJE", ObjeProc);
            _tagSet2.Add("PUBL", publProc);
            _tagSet2.Add("REPO", repoProc);
            _tagSet2.Add("TEXT", textProc);
            _tagSet2.Add("TITL", titlProc);
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

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing identifier"; // TODO assign one?
                err.Tag = SourceRecord.Tag;
                me.Errors.Add(err);
            }

            // No required data except Xref id?
            // TODO Warning: no data provide
        }
    }
}
