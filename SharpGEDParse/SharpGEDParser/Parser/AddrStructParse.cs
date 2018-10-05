using System.Collections.Generic;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Parser
{
    // Some common/custom tags from programs: handled as OtherLines
    // RootsMagic ADDR.MAP, ADDR.MAP.LATI, ADDR.MAP.LONG
    // ADDR.CHAN - PAF, Reunion
    // ADDR.NOTE - geni.com
    // ADDR._NAME
    // ADDR.OBJE - Legacy

    public class AddrStructParse : StructParser
    {
        // TODO could reflection be used to replace these copy-pasta methods? Would it be better?

        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>
        {
            {GedTag.CONT, contProc},
            {GedTag.ADR1, adr1Proc},
            {GedTag.ADR2, adr2Proc},
            {GedTag.ADR3, adr3Proc},
            {GedTag.CITY, cityProc},
            {GedTag.STAE, staeProc},
            {GedTag.POST, postProc},
            {GedTag.CTRY, ctryProc},
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
            StructParseContext ctx2 = PContextFactory.Alloc(ctx, addr);
            //StructParseContext ctx2 = new StructParseContext(ctx, addr);
            addr.Adr += ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return addr;
        }

        public static Address AddrParse(StructParseContext ctx, int linedex, char level)
        {
            Address addr = new Address();
            var ctx2 = PContextFactory.Alloc(ctx, addr, linedex);
            ctx2.Record = ctx.Record;
            ctx2.Level = level;
            addr.Adr += ctx.Remain;
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return addr;
        }

        public static Address OtherTag(ParseContextCommon ctx, GedTag Tag, Address exist)
        {
            // These tags are not subordinate to the ADDR struct. Strictly speaking,
            // the ADDR tag is required, but allow it not to exist.
            Address addr = exist ?? new Address();
            switch (Tag)
            {
                case GedTag.PHON:
                    addr.Phon.Add(ctx.Remain);
                    break;
                case GedTag.WWW:
                    addr.WWW.Add(ctx.Remain);
                    break;
                case GedTag.EMAIL:
                    addr.Email.Add(ctx.Remain);
                    break;
                case GedTag.FAX:
                    addr.Fax.Add(ctx.Remain);
                    break;
            }
            return addr;
        }
    }
}
