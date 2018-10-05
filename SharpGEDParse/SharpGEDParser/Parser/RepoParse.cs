using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    public class RepoParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add(GedTag.NAME, nameproc);
            _tagSet2.Add(GedTag.ADDR, addrproc);
            _tagSet2.Add(GedTag.NOTE, NoteProc);
            _tagSet2.Add(GedTag.REFN, RefnProc);
            _tagSet2.Add(GedTag.RIN,  RinProc);
            _tagSet2.Add(GedTag.CHAN, ChanProc);

            // Unfortunately the spec does NOT have these as subordinate to ADDR
            _tagSet2.Add(GedTag.PHON, commonAddr2);
            _tagSet2.Add(GedTag.WWW, commonAddr2);
            _tagSet2.Add(GedTag.EMAIL, commonAddr2);
            _tagSet2.Add(GedTag.FAX, commonAddr2);

            // NOTE: technically not required: FamilySearch recommends INDI/FAM only. Used by MyHeritage.
            _tagSet2.Add(GedTag._UID, UidProc);
            _tagSet2.Add(GedTag.UID, UidProc);
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
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO {Error = "REPO missing identifier"};
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // A NAME record is required
            if (string.IsNullOrWhiteSpace(me.Name))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissName; // TODO {Error = "REPO missing identifier"};
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }
        }

    }
}
