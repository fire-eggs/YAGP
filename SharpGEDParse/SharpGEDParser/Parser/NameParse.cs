using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

// TODO unit-testing of sub-records

namespace SharpGEDParser.Parser
{
    public class NameParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            { "NPFX", subProc},
            { "NSFX", subProc},
            { "_AKA", subProc},
            { "NICK", subProc},
            { "SURN", subProc},
            { "GIVN", subProc},
            { "SOUR", junkProc},
            { "DATA", junkProc},
            { "TEXT", junkProc}
        };

        private static void junkProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO temp ignore SOUR.DATA.TEXT - see 2524482.ged
        }

        private static void subProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO punting: grab&store w/o analysis
            var rec = (ctx.Parent as NameRec);
            rec.Parts.Add(new Tuple<string, string>(ctx.Tag, ctx.Remain));
        }

        private static void parseName(NameRec rec, string line)
        {
            int max = line.Length;

            // BOULDER_CEM_02212009b.GED had a "1 NAME" with nothing else
            int startName = LineUtil.FirstChar(line, 0, max);
            if (startName >= 0)
            {
                int startSur = LineUtil.AllCharsUntil(line, max, startName, '/');
                int endSur = LineUtil.AllCharsUntil(line, max, startSur + 1, '/');

                var suffix = "";
                if (endSur + 1 < max)
                    suffix = line.Substring(endSur + 1).Trim();

                rec.Names = line.Substring(startName, startSur - startName).Trim();
                rec.Names = string.Join(" ", rec.Names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)); // Remove extra spaces
                if (startSur < max) // e.g. "1 NAME LIVING"
                    rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1);
                if (suffix.Length > 0)
                    rec.Suffix = suffix;
            }
            
        }

        public static NameRec Parse(ParseContext2 ctx)
        {
            // TODO can this be shortcut when context length is only 1 line?
            var name = new NameRec();
            parseName(name, ctx.Remain);
            StructParseContext ctx2 = new StructParseContext(ctx, name);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return name;
        }
    }
}
