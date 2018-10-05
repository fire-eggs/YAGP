using System;
using System.Collections.Generic;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

// TODO -.-.<lds>.STAT.DATE
// TODO revisit: child-level errors (INDI.SLGS.FAMC where xref is in error)

namespace SharpGEDParser.Parser
{
    public class LDSEventParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>()
        {
            {GedTag.DATE, remainProc},
            {GedTag.PLAC, remainProc},
            {GedTag.STAT, remainProc},
            {GedTag.TEMP, remainProc},
            {GedTag.FAMC, xrefproc}, // INDI-SLGC support

            {GedTag.NOTE, noteProc},
            {GedTag.SOUR, sourProc}
        };

        private static void xrefproc(StructParseContext context, int linedex, char level)
        {
            var me = (context.Parent as LDSEvent);

            // TODO copy-pasta from IndiParse
            string xref;
            string extra;
            parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.UntermIdent;// TODO "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline;
                context.Record.Errors.Add(err); // TODO is Record only for errors?
            }
            else
            {
                me.FamilyXref = xref;
            }
        }

        private static void remainProc(StructParseContext context, int linedex, char level)
        {
            var me = (context.Parent as LDSEvent);
            switch (context.Tag)
            {
                case GedTag.DATE:
                    me.Date = context.Remain;
                    break;
                case GedTag.PLAC:
                    me.Place = context.Remain;
                    break;
                case GedTag.STAT:
                    me.Status = context.Remain;
                    break;
                case GedTag.TEMP:
                    me.Temple = context.Remain;
                    break;
                default:
                    throw new NotSupportedException(); // NOTE: this will be thrown if a tag is added to tagDict but no case added here
            }
        }

        public static LDSEvent Parse(ParseContext2 ctx)
        {
            LDSEvent evt = new LDSEvent();
            evt.Tag = ctx.Tag;
            //StructParseContext ctx2 = new StructParseContext(ctx, evt);
            StructParseContext ctx2 = PContextFactory.Alloc(ctx, evt);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return evt;
        }
    }
}
