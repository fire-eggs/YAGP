using System;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class GedRepoParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("NAME", nameproc);
            _tagSet2.Add("ADDR", addrproc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN",  RinProc);
            _tagSet2.Add("CHAN", ChanProc);
        }

        private void NoteProc(ParseContext2 ctx)
        {
            var note = NoteStructParse.NoteParser(ctx);
            (ctx.Parent as GedRepository).Notes.Add(note);
        }

        private void nameproc(ParseContext2 ctx)
        {
            (ctx.Parent as GedRepository).Name = ctx.Remain;
        }

        private void addrproc(ParseContext2 ctx)
        {
            var addr = AddrStructParse.AddrParse(ctx);
            (ctx.Parent as GedRepository).Addr = addr;
        }
    }
}
