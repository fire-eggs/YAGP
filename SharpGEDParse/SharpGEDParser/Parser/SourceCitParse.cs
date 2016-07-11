using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parse a set of lines for a Source Citation structure
    // Source Citations may be embedded or reference.
 
    public class SourceCitParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"CONC", concProc}, // embedded citation
            {"CONT", contProc}, // embedded citation
            {"DATA", dataProc}, // reference citation
            {"NOTE", noteProc},
            {"EVEN", eventProc}, // reference citation
            {"PAGE", pageProc}, // reference citation
            {"QUAY", quayProc}, // reference citation
            {"OBJE", objeProc},
            {"TEXT", textProc}  // embedded citation
        };

        private static void textProc(StructParseContext context, int linedex, char level)
        {
            string val = extendedText(context);
            SourceCit cit = (context.Parent as SourceCit);
            cit.Text.Add(val);
        }

        private static void objeProc(StructParseContext context, int linedex, char level)
        {
            throw new NotImplementedException();
        }

        private static void quayProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Quay = context.Remain;
        }

        private static void pageProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Page = context.Remain;
        }

        private static void eventProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Event = context.Remain;
        }

        private static void dataProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Data = true;
        }

        private static void noteProc(StructParseContext ctx, int linedex, char level)
        {
            SourceCit cit = (ctx.Parent as SourceCit);
            var note = NoteStructParse.NoteParser(ctx, linedex, level);
            cit.Notes.Add(note);
        }

        private static void contProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Desc += "\n" + context.Remain;
        }

        private static void concProc(StructParseContext context, int linedex, char level)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Desc += context.Remain;
        }

        public static SourceCit SourceCitParser(GedRecParse.ParseContext2 ctx)
        {
            SourceCit cit = new SourceCit();
            StructParseContext ctx2 = new StructParseContext(ctx, cit);

            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                cit.Xref = ctx.Remain.Trim(new char[] { '@' });
                if (string.IsNullOrWhiteSpace(cit.Xref) || cit.Xref.Contains("@"))
                {
                    ctx.Parent.Errors.Add(new UnkRec() {Error="Invalid source citation xref id"});
                }
            }
            else
            {
                cit.Desc = ctx.Remain;
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;

            if (!cit.Data && cit.Xref != null && cit.AnyText)
            {
                ctx.Parent.Errors.Add(new UnkRec() { Error = "TEXT tag used for reference source citation" });
            }
            if (cit.Xref == null && cit.Event != null)
            {
                ctx.Parent.Errors.Add(new UnkRec() { Error = "EVEN tag used for embedded source citation" });
            }
            if (cit.Xref == null && cit.Page != null)
            {
                ctx.Parent.Errors.Add(new UnkRec() { Error = "PAGE tag used for embedded source citation" });
            }
            return cit;
        }
    }
}
