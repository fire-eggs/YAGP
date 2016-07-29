using System;
using System.Text;
using SharpGEDParser.Model;
using System.Collections.Generic;

// Common support / logic when parsing GED 'structures'. E.g. CHAN, NOTE-structure, Source-Citation, ADDR, etc

namespace SharpGEDParser.Parser
{
    public class StructParser
    {
        public class StructParseContext
        {
            public GedRecord Lines;
            public StructCommon Parent;
            public int Begline; // index of first line for this 'record'
            public int Endline; // index of last line FOUND for this 'record'
            public char Level;
            public string Remain;

            public StructParseContext(GedRecParse.ParseContext2 ctx, StructCommon parent)
            {
                Parent = parent;
                Lines = ctx.Lines;
                Begline = ctx.Begline;
                Endline = ctx.Endline;
                Level = ctx.Level;
                Remain = ctx.Remain;
            }
            public StructParseContext(StructParseContext ctx, int linedex, StructCommon parent)
            {
                Parent = parent;
                Lines = ctx.Lines;
                Begline = linedex;
                Endline = linedex;
                Level = ctx.Level;
                Remain = ctx.Remain;
            }
        }

        protected delegate void TagProc(StructParseContext context, int linedex, char level);

        protected static void StructParse(StructParseContext ctx, Dictionary<string, TagProc> tagSet)
        {
            int i = ctx.Begline + 1;

            for (; i < ctx.Lines.Max; i++)
            {
                LineUtil.LineData ld = LineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i));
                if (ld.Level <= ctx.Level)
                    break; // end of sub-record
                ctx.Remain = ld.Remain;
                if (tagSet.ContainsKey(ld.Tag))
                {
                    ctx.Begline = i;
                    tagSet[ld.Tag](ctx, i, ld.Level);
                }
                else
                {
                    LineSet extra = new LineSet();
                    ctx.Begline = i;
                    LookAhead(ctx);
                    extra.Beg = ctx.Begline;
                    extra.End = ctx.Endline;
                    ctx.Parent.OtherLines.Add(extra);
                }
                i = Math.Max(ctx.Endline,i);
            }
            ctx.Endline = i - 1;           
        }

        protected static void LookAhead(StructParseContext ctx) // TODO copy-pasta from GedRecParse
        {
            if (ctx.Begline == ctx.Lines.LineCount)
            {
                ctx.Endline = ctx.Begline;
                return; // Nothing to do: already at last line
            }
            int linedex = ctx.Begline;
            int sublinedex;
            while (ctx.Lines.GetLevel(linedex + 1, out sublinedex) > ctx.Level &&
                   linedex + 1 <= ctx.Lines.LineCount)
                linedex++;
            ctx.Endline = linedex;
        }

        // Handle a sub-tag with possible CONC / CONT sub-sub-tags.
        protected static string extendedText(StructParseContext ctx)
        {
            StringBuilder txt = new StringBuilder(ctx.Remain.TrimStart());

            int i = ctx.Begline + 1;
            for (; i < ctx.Lines.Max; i++)
            {
                LineUtil.LineData ld = LineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i));
                if (ld.Level <= ctx.Level)
                    break; // end of sub-record
                if (ld.Tag == "CONC")
                {
                    txt.Append(ld.Remain); // must keep trailing space
                }
                else if (ld.Tag == "CONT")
                {
                    txt.Append("\n"); // NOTE: not appendline, which is \r\n
                    txt.Append(ld.Remain); // must keep trailing space
                }
                else
                    break; // non-CONC, non-CONT: stop!
            }
            ctx.Endline = i - 1;
            return txt.ToString();
        }

    }
}
