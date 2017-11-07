using System;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    public class HeadParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("SOUR", SourProc);
            _tagSet2.Add("DATE", DateProc);
            _tagSet2.Add("SUBM", SubmProc);
            _tagSet2.Add("GEDC", GedcProc);
            _tagSet2.Add("CHAR", CSetProc);
            _tagSet2.Add("PLAC", PlacProc);
            _tagSet2.Add("NOTE", NoteProc);

            // All other tags will be treated as unknowns
        }

        private void CSetProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            self.CharSet = context.Remain.Trim();
        }

        private void DateProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            DateTime outDate;
            if (DateTime.TryParse(context.Remain.Trim(), out outDate))
                self.GedDate = outDate;
            else 
                self.GedDate = DateTime.MinValue; // TODO attempt to derive from other information in postcheck
        }

        private void GedcProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            string val = seekSubRecord("VERS", context);
            self.GedVersion = val;
        }

        private void PlacProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            // TODO what to do here
            //throw new NotImplementedException();
        }

        private void SourProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            self.Source = context.Remain;
            string val = seekSubRecord("VERS", context);
            self.ProductVersion = val;
            val = seekSubRecord("NAME", context);
            self.Product = val;
        }

        private void SubmProc(ParseContext2 context)
        {
            string xref = parseForXref(context);
            if (!string.IsNullOrEmpty(xref))
            {
                var self = (context.Parent as HeadRecord);
                self.AddSubmitter(IndiRecord.Submitter.SUBM, xref);
            }
        }

        private static readonly LineUtil.LineData ld = new LineUtil.LineData();
        private static readonly GEDSplitter gs = new GEDSplitter();

        private string seekSubRecord(string target, ParseContext2 context)
        {
            LookAhead(context); // Any sub-lines (e.g. _FREL/_MREL)?

            if (context.Endline <= context.Begline)
                return "";

            int i = context.Begline + 1;
            while (i <= context.Endline)
            {
                gs.LevelTagAndRemain(context.Lines.GetLine(i), ld);
                if (ld.TagS == target)
                    return ld.Remain;
                i++;
            }
            return "";
        }

        public override void PostCheck(GEDCommon rec)
        {
            //var me = rec as IndiRecord;
        }

    }
}
