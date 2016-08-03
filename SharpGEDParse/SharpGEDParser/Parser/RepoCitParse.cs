using SharpGEDParser.Model;
using System.Collections.Generic;

// TODO any post-parse validation?

namespace SharpGEDParser.Parser
{
    public class RepoCitParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"CALN", calnProc},
            {"MEDI", mediProc},
            {"NOTE", noteProc},
        };

        private static RepoCit.CallNum GetCallNum(RepoCit dad)
        {
            if (dad.CallNums.Count == 0)
            {
                dad.CallNums.Add(new RepoCit.CallNum());
            }
            return dad.CallNums[dad.CallNums.Count - 1];
        }

        private static void mediProc(StructParseContext context, int linedex, char level)
        {
            // HACK apply this to the last CALN entry - error if more than one?
            RepoCit cit = (context.Parent as RepoCit);
            var callN = GetCallNum(cit);
            callN.Media = context.Remain;
        }

        private static void calnProc(StructParseContext context, int linedex, char level)
        {
            RepoCit cit = (context.Parent as RepoCit);
            cit.CallNums.Add(new RepoCit.CallNum());
            cit.CallNums[cit.CallNums.Count-1].Number = context.Remain;
        }

        public static RepoCit CitParser(GedRecParse.ParseContext2 ctx)
        {
            RepoCit cit = new RepoCit();
            StructParseContext ctx2 = new StructParseContext(ctx, cit);

            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                cit.Xref = ctx.Remain.Trim(new char[] { '@' });
                if (string.IsNullOrWhiteSpace(cit.Xref) || cit.Xref.Contains("@"))
                {
                    ctx.Parent.Errors.Add(new UnkRec() { Error = "Invalid repository citation xref id" });
                }
            }
            else
            {
                // TODO missing xref is valid; need a way to store 'extra' text?
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return cit;
        }
    }
}
