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

        private static void contProc(StructParseContext context, int linedex)
        {
            Note note = (context.Parent as Note);
            note.Text += "\n" + context.Remain;
        }

        private static void concProc(StructParseContext context, int linedex)
        {
            Note note = (context.Parent as Note);
            note.Text += context.Remain;
        }

        public static Note NoteParser(GedRecParse.ParseContext2 ctx)
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

        public static Note NoteParser(StructParseContext ctx, int linedex)
        {
            Note note = new Note();
            StructParseContext ctx2 = new StructParseContext(ctx, linedex, note);
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                note.Xref = ctx.Remain.Trim(new char[] { '@' });
            }
            else
            {
                note.Text = ctx.Remain;
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline - 1;
            return note;
        }

        // Common logic for a Note structure which is within a structure
        // e.g. 1 CHAN / 2 NOTE. The NOTE line has already been split.
        // The parameter i is the current line index into ctx.Lines; it
        // will be updated on the way out.
        public static Note NoteSubParse(GedRecParse.ParseContext2 ctx, char level, string remain, ref int i)
        {
            GedRecParse.ParseContext2 ctx2 = new GedRecParse.ParseContext2();
            ctx2.Begline = i;
            ctx2.Level = level;
            ctx2.Lines = ctx.Lines;
            ctx2.Remain = remain;
            ctx2.Parent = ctx.Parent; // TODO errors recorded at parent level, not CHAN level
            var note = NoteParser(ctx2);
            i = ctx2.Endline;
            return note;
        }
    }
}
