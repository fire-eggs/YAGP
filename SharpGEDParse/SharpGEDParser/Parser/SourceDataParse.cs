using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class SourceDataParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"EVEN", evenProc},
            {"PLAC", placProc},
            {"NOTE", noteProc},
            {"AGNC", agncProc},
            {"DATE", dateProc}
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

        public static SourceData DataParser(GedRecParse.ParseContext2 ctx)
        {
            SourceData data = new SourceData();
            StructParseContext ctx2 = new StructParseContext(ctx, data);

            // TODO any text after DATA tag - keep but warn

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return data;
        }
    }
}
