using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // TODO what common/custom tags from programs?

    public class AddrStructParse : StructParser
    {
        // TODO could reflection be used to replace these copy-pasta methods? Would it be better?

        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"CONT", contProc},
            {"ADR1", adr1Proc},
            {"ADR2", adr2Proc},
            {"ADR3", adr3Proc},
            {"CITY", cityProc},
            {"STAE", staeProc},
            {"POST", postProc},
            {"CTRY", ctryProc},
            {"PHON", phonProc},
            {"FAX", faxProc},
            {"EMAIL", emailProc},
            {"WWW", wwwProc}
        };

        private static void wwwProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).WWW = context.Remain;
        }

        private static void emailProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Email = context.Remain;
        }

        private static void faxProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Fax = context.Remain;
        }

        private static void phonProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Phon = context.Remain;
        }

        private static void ctryProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Ctry = context.Remain;
        }

        private static void postProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Post = context.Remain;
        }

        private static void staeProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Stae = context.Remain;
        }

        private static void cityProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).City = context.Remain;
        }

        private static void adr1Proc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Adr1 = context.Remain;
        }

        private static void adr2Proc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Adr2 = context.Remain;
        }

        private static void adr3Proc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Adr3 = context.Remain;
        }

        private static void contProc(StructParseContext context, int linedex)
        {
            (context.Parent as Address).Adr += "\n" + context.Remain;
        }

        public static Address AddrParse(GedRecParse.ParseContext2 ctx)
        {
            Address addr = new Address();
            StructParseContext ctx2 = new StructParseContext(ctx, addr);
            addr.Adr += ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return addr;
        }
    }
}
