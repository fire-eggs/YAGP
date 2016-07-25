using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class NoteParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN", RinProc);
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("SOUR", sourCitProc);
            _tagSet2.Add("CONC", concProc);
            _tagSet2.Add("CONT", contProc);
        }

        // TODO switch to StringBuilder for parse
        private void contProc(ParseContext2 ctx)
        {
            (ctx.Parent as NoteRecord).Text += "\n";
            concProc(ctx);
        }

        // TODO switch to StringBuilder for parse
        private void concProc(ParseContext2 ctx)
        {
            (ctx.Parent as NoteRecord).Text += ctx.Remain;
        }

        // TODO don't have a 'source citation container' base class
        protected void sourCitProc(ParseContext2 ctx)
        {
            var cit = SourceCitParse.SourceCitParser(ctx);
            (ctx.Parent as NoteRecord).Cits.Add(cit);
        }

    }
}
