using System;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    public class HeadParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add(GedTag.SOUR, SourProc);
            _tagSet2.Add(GedTag.DATE, DateProc);
            _tagSet2.Add(GedTag.SUBM, SubmProc);
            _tagSet2.Add(GedTag.GEDC, GedcProc);
            _tagSet2.Add(GedTag.CHAR, CSetProc);
            _tagSet2.Add(GedTag.PLAC, PlacProc);
            _tagSet2.Add(GedTag.NOTE, NoteProc);

            // throw these away
            _tagSet2.Add(GedTag.SUBN, junkProc);
            _tagSet2.Add(GedTag.DEST, junkProc);
            _tagSet2.Add(GedTag.FILE, junkProc);
            _tagSet2.Add(GedTag.LANG, junkProc);
            _tagSet2.Add(GedTag.COPR, junkProc);
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
            string val = seekSubRecord(GedTag.TIME, context); // NOTE: ignoring
        }

        private void GedcProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            string val = seekSubRecord(GedTag.VERS, context);
            self.GedVersion = val;
        }

        private void PlacProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            string val = seekSubRecord(GedTag.FORM, context);
            self.PlaceFormat = val;
        }

        private void SourProc(ParseContext2 context)
        {
            var self = (context.Parent as HeadRecord);
            self.Source = context.Remain;
            string val = seekSubRecord(GedTag.VERS, context);
            self.ProductVersion = val;
            val = seekSubRecord(GedTag.NAME, context);
            self.Product = val;
        }

        private void SubmProc(ParseContext2 context)
        {
            string xref = parseForXref(context);
            if (!string.IsNullOrEmpty(xref))
            {
                var self = (context.Parent as HeadRecord);
                self.AddSubmitter(Submitter.SubmitType.SUBM, xref);
            }
        }

        private void junkProc(ParseContext2 context)
        {
            LookAhead(context);
        }

        private static readonly LineUtil.LineData ld = new LineUtil.LineData();
        private static readonly GEDSplitter gs = new GEDSplitter(GedParser._masterTagCache);

        private string seekSubRecord(GedTag target, ParseContext2 context)
        {
            LookAhead(context); // Any sub-lines (e.g. _FREL/_MREL)?

            if (context.Endline <= context.Begline)
                return "";

            int i = context.Begline + 1;
            while (i <= context.Endline)
            {
                gs.LevelTagAndRemain(context.Lines.GetLine(i), ld);
                if (ld.Tag == target)
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
