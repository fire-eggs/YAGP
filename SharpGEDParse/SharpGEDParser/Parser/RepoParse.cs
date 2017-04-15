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

            // Unfortunately the spec does NOT have these as subordinate to ADDR
            _tagSet2.Add("PHON", commonAddr2);
            _tagSet2.Add("WWW", commonAddr2);
            _tagSet2.Add("EMAIL", commonAddr2);
            _tagSet2.Add("FAX", commonAddr2);
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

        private void commonAddr2(ParseContext2 ctx)
        {
            var dad = (ctx.Parent as Repository);
            dad.Addr = AddrStructParse.OtherTag(ctx, ctx.Tag, dad.Addr);
        }

        public override void PostCheck(GEDCommon rec)
        {
            Repository me = rec as Repository;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec {Error = "REPO missing identifier"};
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // A NAME record is required
            if (string.IsNullOrWhiteSpace(me.Name))
            {
                UnkRec err = new UnkRec {Error = "REPO missing NAME"};
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }
        }

    }
}
