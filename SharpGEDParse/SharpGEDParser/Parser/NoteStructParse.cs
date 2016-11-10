using SharpGEDParser.Model;
using System.Collections.Generic;

namespace SharpGEDParser.Parser
{
    // Parse a set of lines for a Note structure
    public class NoteStructParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"CONC", concProc},
            {"CONT", contProc}
        };

        private static void contProc(StructParseContext context, int linedex, char level)
        {
            Note note = (context.Parent as Note);
            note.Text += "\n" + context.Remain;
        }

        private static void concProc(StructParseContext context, int linedex, char level)
        {
            Note note = (context.Parent as Note);
            note.Text += context.Remain;
        }

        public static Note NoteParser(ParseContext2 ctx)
        {
            Note note = new Note();
            StructParseContext ctx2 = new StructParseContext(ctx, note);
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                note.Xref = ctx.Remain.Trim(new char[] { '@' });
            }
            else
            {
                note.Text = ctx.Remain;
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return note;
        }

        public static Note NoteParser(StructParseContext ctx, int linedex, char level)
        {
            Note note = new Note();
            StructParseContext ctx2 = new StructParseContext(ctx, linedex, note);
            ctx2.Level = level;
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                note.Xref = ctx.Remain.Trim(new char[] { '@' });
            }
            else
            {
                note.Text = ctx.Remain;
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return note;
        }
    }
}
