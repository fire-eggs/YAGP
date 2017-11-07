using SharpGEDParser.Model;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

// Parsing for sub-tags under the INDI+FAMC/FAMS tags.
// 5.5.1 Standard includes INDI.FAMC.PEDI, .STAT, .NOTE; INDI.FAMS.NOTE

// TODO consider 'common' custom tags?

namespace SharpGEDParser.Parser
{
    public class IndiLinkParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>
        {
            {"PEDI", pediProc},
            {"STAT", statProc},
            {"NOTE", noteProc}
        };

        private static void pediProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as IndiLink).Pedi = context.Remain;
        }

        private static void statProc(StructParseContext context, int linedex, char level)
        {
            (context.Parent as IndiLink).Stat = context.Remain;
        }

        public static IndiLink LinkParse(ParseContext2 ctx)
        {
            IndiLink link = new IndiLink();
            link.Tag = ctx.TagS;

            string xref;
            string extra;
            parseXrefExtra(ctx.Remain, out xref, out extra);

            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.UntermIdent; // TODO "Missing/unterminated identifier: " + ctx.Tag;
                err.Beg = err.End = ctx.Begline + ctx.Parent.BegLine;
                ctx.Parent.Errors.Add(err); // TODO parent level or structure level?
            }
            else
            {
                link.Xref = xref;
            }

            if (!string.IsNullOrEmpty(extra))
                link.Extra = extra;

            StructParseContext ctx2 = new StructParseContext(ctx, link);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return link;
        }
    }
}
