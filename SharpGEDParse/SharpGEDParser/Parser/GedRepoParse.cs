using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class GedRepoParse : GedRecParse
    {
        protected new GedRepository _rec; // TODO push to common/GEDCommon

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
            // TODO
        }

        private void nameproc(ParseContext2 ctx)
        {
            (ctx.Parent as GedRepository).Name = ctx.Remain;
        }

        private void addrproc(ParseContext2 ctx)
        {
            ParseAddress(_rec.Addr);
        }

        private void RinProc(ParseContext2 ctx) // TODO push to common/GEDCommon
        {
            ctx.Parent.RIN = ctx.Remain;
        }

        private void ChanProc(ParseContext2 ctx) // TODO push to common/GEDCommon
        {
            if (ctx.Parent.CHAN.Date != null)
            {
                ErrorRec("More than one change record");
            }
            // TODO parse CHAN sub-fields... is the context set up properly?
        }

        private void RefnProc(ParseContext2 ctx) // TODO push to common/GEDCommon
        {
            ctx.Parent.Ids.REFNs.Add(ParseREFN(ctx));
        }

        private StringPlus ParseREFN(ParseContext2 ctx)
        {
            var sp = new StringPlus();
            sp.Value = ctx.Remain;
            LookAhead(ctx);
            sp.Extra.Beg = ctx.Begline + 1;
            sp.Extra.End = ctx.Endline;
            return sp;
        }

        private void ParseAddress(Address _addr)
        {
            // TODO is our context set up properly?
        }
    }
}
