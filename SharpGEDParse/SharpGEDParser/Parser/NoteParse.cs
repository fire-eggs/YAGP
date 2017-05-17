using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class NoteParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN",  RinProc);
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("SOUR", SourCitProc);
            _tagSet2.Add("CONC", concProc);
            _tagSet2.Add("CONT", contProc);
        }

        private void contProc(ParseContext2 ctx)
        {
            //(ctx.Parent as NoteRecord).Text += "\n";
            //concProc(ctx);
            var dad = ctx.Parent as NoteRecord;
            dad.Builder.Append("\n");
            dad.Builder.Append(ctx.Remain);
        }

        private void concProc(ParseContext2 ctx)
        {
            //(ctx.Parent as NoteRecord).Text += ctx.Remain;
            var dad = ctx.Parent as NoteRecord;
            dad.Builder.Append(ctx.Remain);
        }

        public override void PostCheck(GEDCommon rec)
        {
            var me = rec as NoteRecord;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO {Error = "Missing identifier"};
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            me.Text = me.Builder.ToString();
            me.Builder = null;
        }

    }
}
