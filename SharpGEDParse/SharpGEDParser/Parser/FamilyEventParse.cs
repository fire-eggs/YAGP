using SharpGEDParser.Model;
using System.Collections.Generic;

// ReSharper disable PossibleNullReferenceException

namespace SharpGEDParser.Parser
{
    public class FamilyEventParse : StructParser
    {
        //private static StringCache2 _placeCache = new StringCache2();

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
            {"SOUR", sourProc},

            {"FAMC", famcProc}, // ADOP, BIRT, CHR
            {"ADOP", adopProc}, // ADOP
            {"AGE",  ageProc},   // INDI attributes

            {"CONT", dscrProc},
            {"CONC", dscrProc}
        };

        private static void dscrProc(StructParseContext ctx, int linedex, char level)
        {
            // handling of CONC/CONT tags for DSCR
            // NOTE: I experimented with using extendedText instead. Doing so requires adding
            // a complication [incrementing ctx.Begline when necessary] for the 1% scenario.
            // E.g. in IndiParse.AttribProc, the call to extendedText() would have to be followed
            // by something like ctx2.Begline = ctx.Endline, where ctx.Endline has been adjusted by extendedText.

            var own = ctx.Parent as EventCommon;
            // 20180106 Allow conc/cont for any tag

            string extra = ctx.Remain.TrimStart();
            if (ctx.Tag == "CONC")
                own.Descriptor += extra;
            if (ctx.Tag == "CONT")
                own.Descriptor += "\n" + extra;
        }

        private static void adopProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as IndiEvent).FamcAdop = context.Remain;
        }

        private static void famcProc(StructParseContext context, int linedex, char level)
        {
            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrWhiteSpace(xref))
                (context.Parent as IndiEvent).Famc = context.Remain; // TODO what file hit this codepath?
            else
                (context.Parent as IndiEvent).Famc = xref;
        }

        private static void dateProc(StructParseContext context, int linedex, char level)
        {
            // TODO full Date support
            var famE = (context.Parent as EventCommon);
            famE.Date = context.Remain;
            famE.GedDate = EventDateParse.DateParser(context.Remain);
        }

        private static void ageProc(StructParseContext context, int linedex, char level)
        {
            if (context.Tag == "AGE")
            {
                (context.Parent as EventCommon).Age = context.Remain;
                //evt.Age = context.Remain;
            }
            else
            {
                var evt = context.Parent as FamilyEvent;
                var det = EventAgeParse.AgeParser(context, linedex, level);
                switch (context.Tag)
                {
                    case "HUSB":
                    default: // TODO when might this happen?
                        evt.HusbDetail = det;
                        break;
                    case "WIFE":
                        evt.WifeDetail = det;
                        break;
                }
            }
        }

        private static void objeProc(StructParseContext context, int linedex, char level)
        {
            var med = MediaStructParse.MediaParser(context, linedex, level);
            (context.Parent as MediaHold).Media.Add(med);
        }

        private static void placProc(StructParseContext context, int linedex, char level)
        {
            // TODO full PLACE_STRUCTURE support
            var famE = (context.Parent as EventCommon);
            //famE.Place = _placeCache.GetFromCache(context.Remain1);
            famE.Place = context.Remain;
        }

        private static void addrProc(StructParseContext ctx, int linedex, char level)
        {
            var addr = AddrStructParse.AddrParse(ctx, linedex, level);
            (ctx.Parent as EventCommon).Address = addr;
        }

        private static void commonAddr2(StructParseContext ctx, int linedex, char level)
        {
            var dad = (ctx.Parent as EventCommon);
            dad.Address = AddrStructParse.OtherTag(ctx, ctx.Tag, dad.Address);
        }

        private static void remainProc(StructParseContext context, int linedex, char level)
        {
            var famE = (context.Parent as EventCommon);
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

        public static EventCommon Parse(ParseContext2 ctx, bool indi)
        {
            EventCommon gedEvent = indi ? new IndiEvent() as EventCommon : new FamilyEvent() as EventCommon;
            StructParseContext ctx2 = new StructParseContext(ctx, gedEvent);
            gedEvent.Tag = ctx.Tag;
            gedEvent.Descriptor = ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return gedEvent;
        }
    }
}
