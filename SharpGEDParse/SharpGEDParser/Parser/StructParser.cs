using System;
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
            public string Remain;
            public char Level;
            public int Begline; // index of first line for this 'record'
            public int Endline; // index of last line FOUND for this 'record'

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

        protected delegate void TagProc(StructParseContext context, int linedex);

        protected static void StructParse(StructParseContext ctx, Dictionary<string, TagProc> tagSet)
        {
            int i = ctx.Begline + 1;

            char level = ' ';
            string ident = null;
            string tag = null;
            for (; i < ctx.Lines.Max; i++)
            {
                GedLineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i), ref level, ref ident, ref tag, ref ctx.Remain);
                if (level <= ctx.Level)
                    break; // end of sub-record
                if (tagSet.ContainsKey(tag))
                {
                    tagSet[tag](ctx, i);
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

    }
}
