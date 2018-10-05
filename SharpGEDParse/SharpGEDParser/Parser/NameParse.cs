using System;
using System.Collections.Generic;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

// TODO unit-testing of sub-records

namespace SharpGEDParser.Parser
{
    public class NameParse : StructParser
    {
        //private static StringCache2 _surnameCache = new StringCache2();

        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>
        {
            { GedTag.SPFX, subProc},
            { GedTag.NPFX, subProc},
            { GedTag.NSFX, subProc},
            { GedTag._AKA, subProc},
            { GedTag.NICK, subProc},
            { GedTag.SURN, surnProc},
            { GedTag.GIVN, givnProc},
            { GedTag.NOTE, noteProc},
            { GedTag.SOUR, sourProc},
        };

        private static void givnProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO punting: grab&store w/o analysis
            var rec = (ctx.Parent as NameRec);
            if (ctx.Remain != rec.Names) // only store if different
                rec.Parts.Add(new Tuple<GedTag, string>(ctx.Tag, ctx.Remain));
        }
        private static void surnProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO punting: grab&store w/o analysis
            var rec = (ctx.Parent as NameRec);
            if (ctx.Remain != rec.Surname) // only store if different
                rec.Parts.Add(new Tuple<GedTag, string>(ctx.Tag, ctx.Remain));
        }

        private static void subProc(StructParseContext ctx, int linedex, char level)
        {
            // TODO punting: grab&store w/o analysis
            var rec = (ctx.Parent as NameRec);
            rec.Parts.Add(new Tuple<GedTag, string>(ctx.Tag, ctx.Remain));
        }

        private static bool parseName(NameRec rec, char[] line, int linenum, List<UnkRec> errors)
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
                //suffix = _nameCache.GetFromCache(tmp, 0, newlen).Trim(); // TODO trim
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
                //rec.Names = _nameCache.GetFromCache(tmp, a, newlen).Trim(); // TODO trim
                rec.Names = new string(tmp, a, newlen).Trim();
            }

            // Observed bug from ege.ged: empty surname was not parsed properly
            int surnameLen = endSur - startSur - 2; // Will be zero if empty, e.g. "1 NAME Liz //"
            if (startSur < max && surnameLen > 0) // e.g. "1 NAME LIVING"
            {
                //rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1).Trim();
                //rec.Surname = _surnameCache.GetFromCache(line, startSur + 1, endSur - startSur - 1).Trim();// TODO trim
                rec.Surname = new string(line, startSur + 1, endSur - startSur - 1).Trim();
                if (rec.Surname.Contains("/"))
                {
                    UnkRec err = new UnkRec();
                    err.Beg = err.End = linenum;
                    err.Error = UnkRec.ErrorCode.SlashInName;
                    errors.Add(err);
                }
                if (endSur == max && startSur < max)
                {
                    UnkRec err = new UnkRec();
                    err.Beg = err.End = linenum;
                    err.Error = UnkRec.ErrorCode.UntermSurname;
                    errors.Add(err);
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
            if (!parseName(name, ctx.Remain1, ctx.Begline, ctx.Parent.Errors))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.EmptyName;
                err.Beg = err.End = ctx.Begline;
                ctx.Parent.Errors.Add(err);
            }
            //StructParseContext ctx2 = new StructParseContext(ctx, name);
            var ctx2 = PContextFactory.Alloc(ctx, name);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return name;
        }
    }
}
