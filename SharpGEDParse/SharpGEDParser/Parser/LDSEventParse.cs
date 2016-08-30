using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

// TODO sourProc to common
// TODO -.-.<lds>.STAT.DATE

namespace SharpGEDParser.Parser
{
    public class LDSEventParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"DATE", remainProc},
            {"PLAC", remainProc},
            {"STAT", remainProc},
            {"TEMP", remainProc},

            {"NOTE", noteProc},
            {"SOUR", sourProc}
        };

        private static void remainProc(StructParseContext context, int linedex, char level)
        {
            var me = (context.Parent as LDSEvent);
            switch (context.Tag)
            {
                case "DATE":
                    me.Date = context.Remain;
                    break;
                case "PLAC":
                    me.Place = context.Remain;
                    break;
                case "STAT":
                    me.Status = context.Remain;
                    break;
                case "TEMP":
                    me.Temple = context.Remain;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void sourProc(StructParseContext context, int linedex, char level)
        {
            var cit = SourceCitParse.SourceCitParser(context, linedex, level);
            (context.Parent as SourceCitHold).Cits.Add(cit);
        }

        //public static LDSEvent Parse(StructParseContext ctx, int linedex, char level)
        //{
        //    LDSEvent evt = new LDSEvent();
        //    evt.Tag = ctx.Tag;
        //    StructParseContext ctx2 = new StructParseContext(ctx, linedex, evt);
        //    ctx2.Level = level;
        //    StructParse(ctx2, tagDict);
        //    ctx.Endline = ctx2.Endline;
        //    return evt;
        //}

        public static LDSEvent Parse(GedRecParse.ParseContext2 ctx)
        {
            LDSEvent evt = new LDSEvent();
            evt.Tag = ctx.Tag;
            StructParseContext ctx2 = new StructParseContext(ctx, evt);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return evt;
        }
    }
}
