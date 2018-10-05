using System.Collections.Generic;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    public class SourceDataParse : StructParser
    {
        private static readonly Dictionary<GedTag, TagProc> tagDict = new Dictionary<GedTag, TagProc>()
        {
            {GedTag.EVEN, evenProc},
            {GedTag.PLAC, placProc},
            {GedTag.NOTE, noteProc},
            {GedTag.AGNC, agncProc},
            {GedTag.DATE, dateProc}
        };

        private static SourEvent GetEvent(SourceData dad)
        {
            if (dad.Events.Count == 0)
            {
                dad.Events.Add(new SourEvent());
                // TODO this should result in an error? missing EVEN for DATE/PLAC
            }
            return dad.Events[dad.Events.Count - 1];
        }

        private static void dateProc(StructParseContext context, int linedex, char level)
        {
            var dad = (context.Parent as SourceData);
            SourEvent even = GetEvent(dad);
            even.Date = context.Remain;
        }

        private static void agncProc(StructParseContext context, int linedex, char level)
        {
            SourceData data = (context.Parent as SourceData);
            data.Agency = context.Remain;
        }

        private static void placProc(StructParseContext context, int linedex, char level)
        {
            var dad = (context.Parent as SourceData);
            SourEvent even = GetEvent(dad);
            even.Place = context.Remain;
        }

        private static void evenProc(StructParseContext context, int linedex, char level)
        {
            SourceData data = (context.Parent as SourceData);
            SourEvent thing = new SourEvent();
            thing.Text = context.Remain;
            data.Events.Add(thing);
        }

        public static SourceData DataParser(ParseContext2 ctx)
        {
            SourceData data = new SourceData();
            //StructParseContext ctx2 = new StructParseContext(ctx, data);
            var ctx2 = PContextFactory.Alloc(ctx, data);

            if (!string.IsNullOrWhiteSpace(ctx.Remain))
            {
                addNote(data, ctx.Remain);
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            PContextFactory.Free(ctx2);
            return data;
        }
    }
}
