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

            // NOTE: technically not required: FamilySearch recommends INDI/FAM only. Used by MyHeritage.
            _tagSet2.Add("_UID", UidProc);
            _tagSet2.Add("UID", UidProc);
        }

        private void contProc(ParseContext2 ctx)
        {
            //(ctx.Parent as NoteRecord).Text += "\n";
            //concProc(ctx);
            var dad = ctx.Parent as NoteRecord;
            dad.Builder.Append("\n");
            dad.Builder.Append(ctx.gs.RemainLS(ctx.Lines.GetLine(ctx.Begline)));
            //dad.Builder.Append(ctx.Remain);
        }

        private void concProc(ParseContext2 ctx)
        {
            //(ctx.Parent as NoteRecord).Text += ctx.Remain;
            var dad = ctx.Parent as NoteRecord;
            dad.Builder.Append(ctx.gs.RemainLS(ctx.Lines.GetLine(ctx.Begline)));
            //dad.Builder.Append(ctx.Remain);
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

            if (me.Builder.Length > 0)
            {
                // Store an in-line note to the database
                string text = me.Builder.ToString().Replace("@@", "@");
#if SQLITE
                me.Key = SQLite.Instance.StoreNote(text); 
#elif LITEDB
                me.Key = LiteDB.Instance.StoreNote(text);
#elif NOTESTREAM
                me.Key = NoteStream.Instance.StoreNote(text);
#else
                me.Text = text;
#endif
            }
            else
            {
                me.Text = "";
            }

            //me.Text = me.Builder.ToString().Replace("@@", "@"); // TODO faster replace
            me.Builder = null;
        }

    }
}
