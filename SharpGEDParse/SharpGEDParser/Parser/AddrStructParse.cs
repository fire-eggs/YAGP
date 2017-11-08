using System.Collections.Generic;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Parser
{
    // TODO what common/custom tags from programs?
    // RootsMagic ADDR.MAP, ADDR.MAP.LATI, ADDR.MAP.LONG

    public class AddrStructParse : StructParser
    {
        // TODO could reflection be used to replace these copy-pasta methods? Would it be better?

        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>
        {
            {"CONT", contProc},
            {"ADR1", adr1Proc},
            {"ADR2", adr2Proc},
            {"ADR3", adr3Proc},
            {"CITY", cityProc},
            {"STAE", staeProc},
            {"POST", postProc},
            {"CTRY", ctryProc},
        };

        private static void ctryProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Ctry = context.Remain;
        }

        private static void postProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Post = context.Remain;
        }

        private static void staeProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Stae = context.Remain;
        }

        private static void cityProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).City = context.Remain;
        }

        private static void adr1Proc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Adr1 = context.Remain;
        }

        private static void adr2Proc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Adr2 = context.Remain;
        }

        private static void adr3Proc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Adr3 = context.Remain;
        }

        private static void contProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as Address).Adr += "\n" + context.Remain;
        }

        public static Address AddrParse(ParseContext2 ctx)
        {
            Address addr = new Address();
            StructParseContext ctx2 = new StructParseContext(ctx, addr);
            addr.Adr += ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return addr;
        }

        public static Address AddrParse(StructParseContext ctx, int linedex, char level)
        {
            Address addr = new Address();
            StructParseContext ctx2 = new StructParseContext(ctx, addr, linedex);
            ctx2.Level = level;
            addr.Adr += ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return addr;
        }

        public static Address OtherTag(ParseContextCommon ctx, string Tag, Address exist)
        {
            // These tags are not subordinate to the ADDR struct. Strictly speaking,
            // the ADDR tag is required, but allow it not to exist.
            Address addr = exist ?? new Address();
            switch (Tag)
            {
                case "PHON":
                    addr.Phon.Add(ctx.Remain);
                    break;
                case "WWW":
                    addr.WWW.Add(ctx.Remain);
                    break;
                case "EMAIL":
                    addr.Email.Add(ctx.Remain);
                    break;
                case "FAX":
                    addr.Fax.Add(ctx.Remain);
                    break;
                default:
                    // TODO can't really get here
                    // TODO punting here, need to perform LookAhead and that requires a StructParseContext
                    addr.OtherLines.Add(new LineSet {Beg=ctx.Begline});
                    break;
            }
            return addr;
        }
    }
}
