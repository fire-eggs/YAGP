using SharpGEDParser.Model;
using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Parser
{
    class IndiAssoParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>
        {
            {GedTag.RELA, relaProc},
            {GedTag.NOTE, noteProc},
            {GedTag.SOUR, sourProc}
        };

        private static void relaProc(StructParseContext context, int linedex, char level)
        {
            var rec = context.Parent as AssoRec;
            rec.Relation = context.Remain;
        }

        public static AssoRec AssoParse(ParseContext2 ctx)
        {
            var asso = new AssoRec();

            string xref;
            string extra;
            parseXrefExtra(ctx.Remain, out xref, out extra);

            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.UntermIdent;
                // TODO err.Error = "Missing/unterminated identifier: " + ctx.Tag;
                err.Beg = err.End = ctx.Begline + ctx.Parent.BegLine;
                err.Tag = ctx.Tag.ToString();
                ctx.Parent.Errors.Add(err); // TODO parent level or structure level?
            }
            else
            {
                asso.Ident = xref;
            }
            //StructParseContext ctx2 = new StructParseContext(ctx, asso);
            StructParseContext ctx2 = PContextFactory.Alloc(ctx, asso);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return asso;
            // TODO validate relation specified
            // TODO validate ident existance
        }
    }
}
