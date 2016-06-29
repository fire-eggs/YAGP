using System;
using SharpGEDParser.Model;

namespace SharpGEDParser
{
    public class GedEventParse : GedRecParse
    {
        public override KBRGedRec Parse0(KBRGedRec rec, ParseContext context)
        {
            KBRGedEvent eRec = new KBRGedEvent(rec.Lines, context.Tag);
            if (context.Tag == "DSCR") // TODO conc/cont support can't lose trailing spaces
                eRec.Detail = context.Line.Substring(context.Nextchar).TrimStart();
            else
                eRec.Detail = context.Line.Substring(context.Nextchar).Trim();
            Parse(eRec, context);
            return eRec;
        }

        protected override void BuildTagSet()
        {
            _tagSet.Add("AGE", AgeProc);
            _tagSet.Add("DATE", DateProc);
            _tagSet.Add("TYPE", TypeProc);
            _tagSet.Add("CAUS", CausProc);
            _tagSet.Add("PLAC", PlacProc);
            _tagSet.Add("AGNC", AgncProc);
            _tagSet.Add("RELI", ReliProc);
            _tagSet.Add("RESN", ResnProc);
            _tagSet.Add("SOUR", SourProc);
            _tagSet.Add("NOTE", NoteProc);
// TODO            _tagSet.Add("OBJE", ObjeProc);

            _tagSet.Add("HUSB", HusbProc); // Family event support
            _tagSet.Add("WIFE", WifeProc); // Family event support

            _tagSet.Add("FAMC", FAMCProc); // BIRT / CHR / ADOP support

            _tagSet.Add("CONC", dscrProc);
            _tagSet.Add("CONT", dscrProc);

            _tagSet.Add("ADDR", addrProc);
        }

        private void AgeProc()
        {
            (_rec as KBRGedEvent).Age = Remainder();
        }
        private void DateProc()
        {
            (_rec as KBRGedEvent).Date = Remainder();
        }
        private void TypeProc()
        {
            (_rec as KBRGedEvent).Type = Remainder();
        }
        private void CausProc()
        {
            (_rec as KBRGedEvent).Cause = Remainder();
        }
        private void PlacProc()
        {
            (_rec as KBRGedEvent).Place = Remainder();
            CheckForExtra();
        }
        private void AgncProc()
        {
            (_rec as KBRGedEvent).Agency = Remainder();
        }
        private void ReliProc()
        {
            (_rec as KBRGedEvent).Religion = Remainder();
        }
        private void ResnProc()
        {
            (_rec as KBRGedEvent).Restriction = Remainder();
        }

        private void addrProc()
        {
            (_rec as KBRGedEvent).Address = Remainder(); // TODO this is a punt
        }

        private void FAMCProc()
        {
            (_rec as KBRGedEvent).Famc = Remainder();
            if (ctx.Endline > ctx.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(ctx.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = GedLineUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "ADOP")
                    (_rec as KBRGedEvent).FamcAdop = line.Substring(nextChar).Trim();
                else
                {
                    ErrorRec(string.Format("Unknown FAMC subordinate tag {0}", tag));
                }
            }
        }

        private void HusbProc()
        {
            (_rec as KBRGedEvent).HusbDetail = Remainder();
            if (ctx.Endline > ctx.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(ctx.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = GedLineUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    (_rec as KBRGedEvent).HusbAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }
        private void WifeProc()
        {
            (_rec as KBRGedEvent).WifeDetail = Remainder();
            if (ctx.Endline > ctx.Begline)
            {
                string line = (_rec as KBRGedEvent).Lines.GetLine(ctx.Begline + 1);
                string ident = null;
                string tag = null;
                int nextChar = GedLineUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                if (tag == "AGE")
                    (_rec as KBRGedEvent).WifeAge = line.Substring(nextChar).Trim();
                // TODO anything else is unknown/error
            }
        }

        private void SourProc()
        {
            SourCitProc(_rec);
        }

        private void dscrProc()
        {
            // TODO not quite the same as other CONC/CONT, can't use extendedText?

            // handling of CONC/CONT tags for DSCR
            if (_rec.Tag != "DSCR")
            {
                ErrorRec(string.Format("Invalid CONC/CONT for {0}", _rec.Tag));
                return;
            }

            string extra = ctx.Line.Substring(ctx.Nextchar).TrimStart();
            if (ctx.Tag == "CONC")
                (_rec as KBRGedEvent).Detail += extra;
            if (ctx.Tag == "CONT")
                (_rec as KBRGedEvent).Detail += "\n" + extra;
        }

        /// <summary>
        /// Check for additional, unsupported sub-records. E.g. "2 PLAC foo\n3 ADOP bar"
        /// </summary>
        private void CheckForExtra()
        {
            if (ctx.Endline <= ctx.Begline) 
                return;
            var rec = new UnkRec();
            rec.Tag = ctx.Tag;
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;
            rec.Error = "Extra line(s) found and ignored";
            _rec.Errors.Add(rec);
        }
    }
}
