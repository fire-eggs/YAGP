using System.Text;
using SharpGEDParser.Model;
using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    // Parse a set of lines for a Note structure
    public class NoteStructParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>()
        {
            {GedTag.CONC, concProc},
            {GedTag.CONT, contProc},
            {GedTag.SOUR, sourProc}
        };

        private static void contProc(StructParseContext context, int linedex, char level)
        {
            Note note = (context.Parent as Note);
            note.Builder.Append("\n");
            note.Builder.Append(context.gs.RemainLS(context.Lines.GetLine(linedex)));
//            note.Builder.Append(context.Remain); // NOTE: trailing spaces are preserved, may be confusing
        }

        private static void concProc(StructParseContext context, int linedex, char level)
        {
            Note note = (context.Parent as Note);
            note.Builder.Append(context.gs.RemainLS(context.Lines.GetLine(linedex)));
            //note.Builder.Append(context.Remain); // NOTE: trailing spaces are preserved, may be confusing
        }

        public static char[] trim = {'@'};
        
        public static Note NoteParser(ParseContextCommon ctx, int linedex, char level)
        {
            Note note = new Note();
            note.Builder = new StringBuilder(512);
            var ctx2 = PContextFactory.Alloc(ctx, note, linedex); // TODO no record for context!
            ctx2.Level = level;

            if (ctx as ParseContext2 != null)
            {
                ctx2.Record = (ctx as ParseContext2).Parent;
            }
            // FAM.NOTE.SOUR crapped out 'cause Record was null
            if (ctx as StructParseContext != null)
            {
                ctx2.Record = (ctx as StructParseContext).Record;
            }

            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                note.Xref = ctx.Remain.Trim(trim);
            }
            else
            {
                note.Builder.Append(ctx.gs.RemainLS(ctx.Lines.GetLine(linedex)));
                //note.Builder.Append(ctx.Remain); // NOTE: trailing spaces are preserved, may be confusing
            }

            StructParse(ctx2, tagDict);

            if (note.Builder.Length > 0)
            {
                // Store an in-line note to the database
                string text = note.Builder.ToString().Replace("@@", "@");
#if SQLITE
                note.Key = SQLite.Instance.StoreNote(text); 
#elif LITEDB
                note.Key = LiteDB.Instance.StoreNote(text);
#elif NOTESTREAM
                note.Key = NoteStream.Instance.StoreNote(text);
#else
                note.Text = text;
#endif
            }
            else
            {
                note.Text = "";
            }

            //note.Text = note.Builder.ToString().Replace("@@", "@"); // TODO faster replace;
            note.Builder = null;
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return note;
        }
    }
}
