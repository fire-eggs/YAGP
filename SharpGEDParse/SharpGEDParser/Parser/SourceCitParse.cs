using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parse a set of lines for a Source Citation structure
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

        private static void textProc(StructParseContext context, int linedex)
        {
            throw new NotImplementedException();
        }

        private static void objeProc(StructParseContext context, int linedex)
        {
            throw new NotImplementedException();
        }

        private static void quayProc(StructParseContext context, int linedex)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Quay = context.Remain;
        }

        private static void pageProc(StructParseContext context, int linedex)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Page = context.Remain;
        }

        private static void eventProc(StructParseContext context, int linedex)
        {
            throw new NotImplementedException();
        }

        private static void dataProc(StructParseContext context, int linedex)
        {
            throw new NotImplementedException();
        }

        private static void noteProc(StructParseContext ctx, int linedex)
        {
            SourceCit cit = (ctx.Parent as SourceCit);
            var note = NoteStructParse.NoteParser(ctx, linedex);
            cit.Notes.Add(note);
        }

        private static void contProc(StructParseContext context, int linedex)
        {
            SourceCit cit = (context.Parent as SourceCit);
            cit.Desc += "\n" + context.Remain;
        }

        private static void concProc(StructParseContext context, int linedex)
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
            }
            else
            {
                cit.Desc = ctx.Remain;
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return cit;

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
                        cit.Desc += remain;
                        break;
                    case "CONT":
                        cit.Desc += "\n" + remain;
                        break;
                    case "DATA":
                        // Ignore!
                        break;
                    case "OBJE":
                        break;
                    case "NOTE":
                        var note = NoteStructParse.NoteSubParse(ctx, level, remain, ref i);
                        cit.Notes.Add(note);
                        break;
                    case "QUAY":
                        break;
                    case "EVEN":
                        break;
                    case "PAGE":
                        break;
                    case "DATE":
                        DateTime res;
                        if (DateTime.TryParse(remain, out res))
                            cit.Date = res;
                        break;
                    case "TEXT":
                        // TODO multiple TEXT entries
                        cit.Text = ExtendedTextParse(ctx, level, remain, ref i);
                        break;
                    default:
                        LineSet extra = new LineSet();
                        ctx.Begline = i;
                        GedRecParse.LookAhead(ctx);
                        extra.Beg = ctx.Begline;
                        extra.End = ctx.Endline;
                        cit.OtherLines.Add(extra);
                        i = ctx.Endline;
                        break;
                }
            }
            ctx.Endline = i - 1;

            return cit;
        }

        private static string ExtendedTextParse(GedRecParse.ParseContext2 ctx, char level, string remain, ref int i)
        {
            return null;
        }
    }
}
