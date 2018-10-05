using SharpGEDParser.Model;
using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

// Parsing for sub-tags under the INDI+FAMC/FAMS tags.
// 5.5.1 Standard includes INDI.FAMC.PEDI, .STAT, .NOTE; INDI.FAMS.NOTE

// TODO consider 'common' custom tags?

namespace SharpGEDParser.Parser
{
    public class IndiLinkParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>
        {
            {GedTag.PEDI, pediProc},
            {GedTag.STAT, statProc},
            {GedTag.NOTE, noteProc}
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
            link.Type = ctx.Tag == GedTag.FAMC ? IndiLink.FAMC_TYPE : IndiLink.FAMS_TYPE;

            string xref;
            string extra;
            parseXrefExtra(ctx.Remain, out xref, out extra);

            if (string.IsNullOrEmpty(xref))
            {
                err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent;
                err.Beg = err.End = ctx.Begline + ctx.Parent.BegLine;
                err.Tag = ctx.TagAsString;
                ctx.Parent.Errors.Add(err);
            }
            else
            {
                link.Xref = xref;
            }

            if (!string.IsNullOrEmpty(extra))
                link.Extra = extra;

            //StructParseContext ctx2 = new StructParseContext(ctx, link);
            StructParseContext ctx2 = PContextFactory.Alloc(ctx, link);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);

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
