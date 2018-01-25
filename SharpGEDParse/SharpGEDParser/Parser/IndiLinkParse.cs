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
            UnkRec err = null;

            IndiLink link = new IndiLink();

            // Can't get here for values other than FAMC/FAMS [unless caller changes!]
            link.Type = ctx.Tag == "FAMC" ? IndiLink.FAMC_TYPE : IndiLink.FAMS_TYPE;

            string xref;
            string extra;
            parseXrefExtra(ctx.Remain, out xref, out extra);

            if (string.IsNullOrEmpty(xref))
            {
                err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent;
                err.Beg = err.End = ctx.Begline + ctx.Parent.BegLine;
                err.Tag = ctx.Tag;
                ctx.Parent.Errors.Add(err);
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

            if (err != null)
            {
                // Fallout from GedValid: an error in the link should not create an IndiLink
                err.End = ctx.Endline + ctx.Parent.BegLine; // entire structure in error
                return null;
            }

            return link;
        }
    }
}
