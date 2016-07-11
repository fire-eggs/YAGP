using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class ChanStructParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"DATE", dateProc},
            {"NOTE", noteProc}
        };

        private static void noteProc(StructParseContext ctx, int linedex, char level)
        {
            var chan = ctx.Parent as ChangeRec;
            var note = NoteStructParse.NoteParser(ctx, linedex, level);
            chan.Notes.Add(note);
        }

        private static void dateProc(StructParseContext ctx, int linedex, char level)
        {
            var chan = ctx.Parent as ChangeRec;
            DateTime res;
            if (DateTime.TryParse(ctx.Remain, out res))
                chan.Date = res;
        }

        public static void ChanParse(GedRecParse.ParseContext2 ctx, ChangeRec chan)
        {
            StructParseContext ctx2 = new StructParseContext(ctx, chan);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
        }

        public static void ChanProc(GedRecParse.ParseContext2 ctx)
        {
            var Parent = (ctx.Parent as GEDCommon);
            ChangeRec chan = Parent.CHAN;
            if (chan.Date != null)
            {
                UnkRec err = new UnkRec();
                err.Error = "More than one change record";
                GedRecParse.LookAhead(ctx);
                err.Beg = ctx.Begline;
                err.End = ctx.Endline;
                Parent.Errors.Add(err);
                return;
            }

            ChanParse(ctx, chan);
            if (chan.Date == null)
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing required data for CHAN";
                // TODO missing line numbers
                Parent.Errors.Add(err);
            }
        }

    }
}
