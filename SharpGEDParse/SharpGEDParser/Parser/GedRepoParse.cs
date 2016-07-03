using System;
using System.Data.Common;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class GedRepoParse : GedRecParse
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
            (ctx.Parent as GedRepository).Notes.Add(note);
        }

        //private Note SuperNoteProc(ParseContext2 ctx)
        //{
        //    Note note = new Note();
        //    if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
        //    {
        //        note.Xref = ctx.Remain.Trim(new char[] {'@'});
        //    }
        //    else
        //    {
        //        note.Text = ctx.Remain;
        //    }

        //    int i = ctx.Begline + 1;

        //    char level = ' ';
        //    string ident = null;
        //    string tag = null;
        //    string remain = null;
        //    for (; i < ctx.Lines.Max; i++)
        //    {
        //        GedLineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i), ref level, ref ident, ref tag, ref remain);
        //        if (level <= ctx.Level)
        //            break; // end of sub-record
        //        switch (tag)
        //        {
        //            case "CONC":
        //                note.Text += remain;
        //                break;
        //            case "CONT":
        //                note.Text += "\n" + remain;
        //                break;
        //            default:
        //                LineSet extra = new LineSet();
        //                ctx.Begline = i;
        //                LookAhead(ctx);
        //                extra.Beg = ctx.Begline;
        //                extra.End = ctx.Endline;
        //                note.OtherLines.Add(extra);
        //                i = ctx.Endline;
        //                break;
        //        }
        //    }
        //    ctx.Endline = i - 1;
        //    return note;
        //}

        private void nameproc(ParseContext2 ctx)
        {
            (ctx.Parent as GedRepository).Name = ctx.Remain;
        }

        private void addrproc(ParseContext2 ctx)
        {
            var addr = AddrStructParse.AddrParse(ctx);
            (ctx.Parent as GedRepository).Addr = addr;
        }

        private void RinProc(ParseContext2 ctx) // TODO push to common/GEDCommon
        {
            ctx.Parent.RIN = ctx.Remain;
        }

        private void ChanProc(ParseContext2 ctx) // TODO push to common/GEDCommon
        {
            ChangeRec chan = ctx.Parent.CHAN;
            if (chan.Date != null)
            {
                UnkRec err = new UnkRec();
                err.Error = "More than one change record";
                LookAhead(ctx);
                err.Beg = ctx.Begline;
                err.End = ctx.Endline;
                ctx.Parent.Errors.Add(err);
                return;
            }

            int i = ctx.Begline + 1;
            if (i > ctx.Lines.Max)
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing required data for CHAN";
                LookAhead(ctx);
                err.Beg = ctx.Begline;
                err.End = ctx.Endline;
                ctx.Parent.Errors.Add(err);
                return;
            }

            char level = ' ';
            string ident = null;
            string tag = null;
            string remain = null;
            for (; i < ctx.Lines.Max; i++)
            {
                GedLineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i), ref level, ref ident, ref tag, ref remain);
                if (level <= ctx.Level)
                    break; // end of sub-record
                switch (tag)
                {
                    case "DATE":
                        DateTime res;
                        if (DateTime.TryParse(remain, out res))
                            chan.Date = res;
                        break;
                    case "NOTE":
                        var note = NoteStructParse.NoteSubParse(ctx, level, remain, ref i);
                        chan.Notes.Add(note);
                        break;
                    default:
                        LineSet extra = new LineSet();
                        ctx.Begline = i;
                        LookAhead(ctx);
                        extra.Beg = ctx.Begline;
                        extra.End = ctx.Endline;
                        chan.OtherLines.Add(extra);
                        i = ctx.Endline;
                        break;
                }
            }
            ctx.Endline = i-1;

            if (chan.Date == null)
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing required data for CHAN"; 
                // TODO missing line numbers
                ctx.Parent.Errors.Add(err);
            }
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
    }
}
