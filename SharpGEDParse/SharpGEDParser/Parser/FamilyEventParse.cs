using SharpGEDParser.Model;
using System;
using System.Collections.Generic;

namespace SharpGEDParser.Parser
{
    public class FamilyEventParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"HUSB", ageProc},
            {"WIFE", ageProc},

            {"ADDR", addrProc},
            {"AGNC", remainProc},
            {"CAUS", remainProc},
            {"DATE", dateProc},
            {"PLAC", placProc},
            {"RELI", remainProc},
            {"RESN", remainProc},
            {"TYPE", remainProc},

            // Unfortunately the spec does NOT have these as subordinate to ADDR
            {"PHON", commonAddr2},
            {"WWW",  commonAddr2},
            {"EMAIL", commonAddr2},
            {"FAX", commonAddr2},

            {"NOTE", noteProc},
            {"OBJE", objeProc},
            {"SOUR", sourProc}

        };

        private static void dateProc(StructParseContext context, int linedex, char level)
        {
            // TODO full Date support
            var famE = (context.Parent as FamilyEvent);
            famE.Date = context.Remain;
        }

        private static void ageProc(StructParseContext context, int linedex, char level)
        {
            var det = EventAgeParse.AgeParser(context, linedex, level);
            if (context.Tag == "HUSB")
                (context.Parent as FamilyEvent).HusbDetail = det;
            else
                (context.Parent as FamilyEvent).WifeDetail = det;
        }

        private static void sourProc(StructParseContext context, int linedex, char level)
        {
            var cit = SourceCitParse.SourceCitParser(context, linedex, level);
            (context.Parent as SourceCitHold).Cits.Add(cit);
        }

        private static void objeProc(StructParseContext context, int linedex, char level)
        {
            var med = MediaStructParse.MediaParser(context, linedex, level);
            (context.Parent as MediaHold).Media.Add(med);
        }

        private static void placProc(StructParseContext context, int linedex, char level)
        {
            // TODO full PLACE_STRUCTURE support
            var famE = (context.Parent as FamilyEvent);
            famE.Place = context.Remain;
        }

        private static void addrProc(StructParseContext ctx, int linedex, char level)
        {
            var addr = AddrStructParse.AddrParse(ctx, linedex, level);
            (ctx.Parent as FamilyEvent).Address = addr;
        }

        private static void commonAddr2(StructParseContext ctx, int linedex, char level)
        {
            var dad = (ctx.Parent as FamilyEvent);
            dad.Address = AddrStructParse.OtherTag(ctx, ctx.Tag, dad.Address);
        }

        private static void remainProc(StructParseContext context, int linedex, char level)
        {
            var famE = (context.Parent as FamilyEvent);
            switch (context.Tag) // TODO consider using reflection and property name?
            {
                case "TYPE":
                    famE.Type = context.Remain;
                    break;
                case "AGNC":
                    famE.Agency = context.Remain;
                    break;
                case "RELI":
                    famE.Religion = context.Remain;
                    break;
                case "CAUS":
                    famE.Cause = context.Remain;
                    break;
                case "RESN":
                    famE.Restriction = context.Remain;
                    break;
            }
/*
            Type type = target.GetType();
            PropertyInfo prop = type.GetProperty("propertyName");
            prop.SetValue(target, propertyValue, null);
 */
        }

        public static FamilyEvent Parse(GedRecParse.ParseContext2 ctx)
        {
            var gedEvent = new FamilyEvent();
            StructParseContext ctx2 = new StructParseContext(ctx, gedEvent);
            gedEvent.Tag = ctx.Tag;
            gedEvent.Descriptor = ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return gedEvent;
        }
    }
}
