using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

// TODO unit-testing of sub-records

namespace SharpGEDParser.Parser
{
    public class NameParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>
        {
            { "SPFX", subProc},
            { "NPFX", subProc},
            { "NSFX", subProc},
            { "_AKA", subProc},
            { "NICK", subProc},
            { "SURN", subProc},
            { "GIVN", subProc},
            { "NOTE", junkProc},
            { "SOUR", junkProc},
            { "DATA", junkProc},
            { "TEXT", junkProc}
        };

        private static void junkProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO temp ignore NAME.SOUR.DATA.TEXT - see 2524482.ged
            // TODO temp ignore NAME.NOTE
        }

        private static void subProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO punting: grab&store w/o analysis
            var rec = (ctx.Parent as NameRec);
            rec.Parts.Add(new Tuple<string, string>(ctx.Tag, ctx.Remain));
        }

        private static bool parseName(NameRec rec, char [] line, int linenum)
        {
            int max = line.Length;

            // BOULDER_CEM_02212009b.GED had a "1 NAME" with nothing else
            int startName = LineUtil.FirstChar(line, 0, max);
            if (startName < 0)
                return false;

            // Deal with slashes in the surname
            int startSur = LineUtil.AllCharsUntil(line, max, startName, '/');
            int endSur = LineUtil.ReverseSearch(line, max, startSur + 1, '/');

            var suffix = "";
            if (endSur + 1 < max)
            {
                //suffix = string.Join(" ", line.Substring(endSur + 1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)); // Remove extra spaces
                int newlen = 0;
                var tmp = LineUtil.RemoveExtraSpaces(line, endSur + 1, max, ref newlen);
                suffix = new string(tmp, 0, newlen).Trim();
            }

            {
                int a = 0;
                int b = max;
                if (startSur < max)
                {
                    a = startName;
                    b = startSur - startName;
                }
                int newlen = 0;
                var tmp = LineUtil.RemoveExtraSpaces(line, a, b, ref newlen);
                rec.Names = new string(tmp, a, newlen).Trim();
            }

            if (startSur < max) // e.g. "1 NAME LIVING"
            {
                //rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1).Trim();
                rec.Surname = new string(line, startSur + 1, endSur - startSur - 1).Trim();
                if (rec.Surname.Contains("/"))
                {
                    UnkRec err = new UnkRec();
                    err.Beg = err.End = linenum;
                    err.Error = UnkRec.ErrorCode.SlashInName;
                    rec.Errors.Add(err);
                }
                if (endSur == max && startSur < max)
                {
                    UnkRec err = new UnkRec();
                    err.Beg = err.End = linenum;
                    err.Error = UnkRec.ErrorCode.UntermSurname;
                    rec.Errors.Add(err);
                }
            }
            if (suffix.Length > 0)
                rec.Suffix = suffix;
            return true;
        }

        public static NameRec Parse(ParseContext2 ctx)
        {
            // TODO can this be shortcut when context length is only 1 line?
            var name = new NameRec();
            if (!parseName(name, ctx.Remain1, ctx.Begline))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.EmptyName;
                err.Beg = err.End = ctx.Begline;
                name.Errors.Add(err);
            }
            StructParseContext ctx2 = new StructParseContext(ctx, name);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return name;
        }
    }
}
