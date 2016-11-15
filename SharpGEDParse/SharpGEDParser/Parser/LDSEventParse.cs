using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

// TODO sourProc to common
// TODO -.-.<lds>.STAT.DATE

namespace SharpGEDParser.Parser
{
    public class LDSEventParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"DATE", remainProc},
            {"PLAC", remainProc},
            {"STAT", remainProc},
            {"TEMP", remainProc},
            {"FAMC", xrefproc}, // INDI-SLGC support

            {"NOTE", noteProc},
            {"SOUR", sourProc}
        };

        private static void xrefproc(StructParseContext context, int linedex, char level)
        {
            var me = (context.Parent as LDSEvent);

            // TODO copy-pasta from IndiParse
            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline;
                // me.Errors.Add(err); // TODO no place to store errors: move to post-processing?
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
                case "DATE":
                    me.Date = context.Remain;
                    break;
                case "PLAC":
                    me.Place = context.Remain;
                    break;
                case "STAT":
                    me.Status = context.Remain;
                    break;
                case "TEMP":
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
            StructParseContext ctx2 = new StructParseContext(ctx, evt);
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return evt;
        }
    }
}
