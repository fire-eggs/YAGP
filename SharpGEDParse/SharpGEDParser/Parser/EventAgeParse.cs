using SharpGEDParser.Model;
using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    class EventAgeParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>()
        {
            {GedTag.AGE, ageProc}
        };

        private static void ageProc(StructParseContext context, int linedex, char level)
        {
            var det = context.Parent as AgeDetail;
            det.Age = context.Remain;
        }

        public static AgeDetail AgeParser(StructParseContext ctx, int linedex, char level)
        {
            AgeDetail det = new AgeDetail();
            var ctx2 = PContextFactory.Alloc(ctx, det, linedex);
            ctx2.Level = level;
            ctx2.Record = ctx.Record;
            if (!string.IsNullOrWhiteSpace(ctx.Remain))
            {
                det.Detail = ctx.Remain;
            }
            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return det;
        }
    }
}
