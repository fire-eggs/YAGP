using System.Runtime.CompilerServices;
using SharpGEDParser.Model;
using System;
using System.Collections.Generic;

// TODO any post-parse validation?

namespace SharpGEDParser.Parser
{
    public class RepoCitParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"CALN", calnProc},
            {"MEDI", mediProc},
            {"NOTE", noteProc},
        };

        private static void mediProc(StructParseContext context, int linedex, char level)
        {
            // HACK apply this to the last CALN entry
            RepoCit cit = (context.Parent as RepoCit);
            cit.CallNums[cit.CallNums.Count - 1].Media = context.Remain;
        }

        private static void calnProc(StructParseContext context, int linedex, char level)
        {
            RepoCit cit = (context.Parent as RepoCit);
            cit.CallNums.Add(new RepoCit.CallNum());
            cit.CallNums[cit.CallNums.Count-1].Number = context.Remain;
        }

        private static void noteProc(StructParseContext ctx, int linedex, char level)
        {
            var note = NoteStructParse.NoteParser(ctx, linedex, level);
            RepoCit cit = (ctx.Parent as RepoCit);
            cit.Notes.Add(note);
        }

        public static RepoCit CitParser(GedRecParse.ParseContext2 ctx)
        {
            RepoCit cit = new RepoCit();
            StructParseContext ctx2 = new StructParseContext(ctx, cit);

            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                cit.Xref = ctx.Remain.Trim(new char[] { '@' });
                if (string.IsNullOrWhiteSpace(cit.Xref) || cit.Xref.Contains("@"))
                {
                    ctx.Parent.Errors.Add(new UnkRec() { Error = "Invalid repository citation xref id" });
                }
            }
            else
            {
                // TODO missing xref is valid; need a way to store 'extra' text?
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return cit;
        }
    }
}
