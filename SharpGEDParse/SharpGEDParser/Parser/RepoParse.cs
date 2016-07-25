using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class RepoParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("NAME", nameproc);
            _tagSet2.Add("ADDR", addrproc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN",  RinProc);
            _tagSet2.Add("CHAN", ChanProc);
        }

        private void NoteProc(ParseContext2 ctx)
        {
            var note = NoteStructParse.NoteParser(ctx);
            (ctx.Parent as Repository).Notes.Add(note);
        }

        private void nameproc(ParseContext2 ctx)
        {
            (ctx.Parent as Repository).Name = ctx.Remain;
        }

        private void addrproc(ParseContext2 ctx)
        {
            var addr = AddrStructParse.AddrParse(ctx);
            (ctx.Parent as Repository).Addr = addr;
        }

        public override void PostCheck(GEDCommon rec)
        {
            Repository me = rec as Repository;

            // A NAME record is required
            if (string.IsNullOrWhiteSpace(me.Name))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing NAME";
                me.Errors.Add(err);
            }
        }

    }
}
