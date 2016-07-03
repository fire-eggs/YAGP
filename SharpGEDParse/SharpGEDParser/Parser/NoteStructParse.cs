using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parse a set of lines for a Note structure
    public class NoteStructParse
    {
        public static Note NoteParser(GedRecParse.ParseContext2 ctx)
        {
            Note note = new Note();
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                note.Xref = ctx.Remain.Trim(new char[] { '@' });
            }
            else
            {
                note.Text = ctx.Remain;
            }

            int i = ctx.Begline + 1;

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
                    case "CONC":
                        note.Text += remain;
                        break;
                    case "CONT":
                        note.Text += "\n" + remain;
                        break;
                    default:
                        LineSet extra = new LineSet();
                        ctx.Begline = i;
                        GedRecParse.LookAhead(ctx);
                        extra.Beg = ctx.Begline;
                        extra.End = ctx.Endline;
                        note.OtherLines.Add(extra);
                        i = ctx.Endline;
                        break;
                }
            }
            ctx.Endline = i - 1;
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
