using System;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parsing for the FAM (family) record.

    // TODO copy-pasta: subm, chil, husb, wife processing

    public class FamParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("OBJE", ObjeProc);
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN",  RinProc);
            _tagSet2.Add("SOUR", SourCitProc);

            _tagSet2.Add("CHIL", xrefProc);
            _tagSet2.Add("HUSB", xrefProc);
            _tagSet2.Add("NCHI", nchiProc);
            _tagSet2.Add("RESN", resnProc);
            _tagSet2.Add("SUBM", xrefProc);
            _tagSet2.Add("WIFE", xrefProc);

            _tagSet2.Add("SLGS", ldsSpouseSeal);

            _tagSet2.Add("ANUL", eventProc);
            _tagSet2.Add("CENS", eventProc);
            _tagSet2.Add("DIV",  eventProc);
            _tagSet2.Add("DIVF", eventProc);
            _tagSet2.Add("ENGA", eventProc);
            _tagSet2.Add("EVEN", eventProc);
            _tagSet2.Add("MARB", eventProc);
            _tagSet2.Add("MARC", eventProc);
            _tagSet2.Add("MARR", eventProc);
            _tagSet2.Add("MARL", eventProc);
            _tagSet2.Add("MARS", eventProc);
            _tagSet2.Add("RESI", eventProc);
        }

        private void ldsSpouseSeal(ParseContext2 context)
        {
            throw new NotImplementedException();
        }

        private void nchiProc(ParseContext2 context)
        {
            var fam = (context.Parent as FamRecord);

            int childCount;
            if (!int.TryParse(context.Remain, out childCount))
            {
                UnkRec err = new UnkRec();
                err.Error = "Invalid child count";
                err.Beg = err.End = context.Begline;
                fam.Errors.Add(err);
            }
            else
            {
                fam.ChildCount = childCount;
            }
        }

        // Common processing for SUBM, HUSB, WIFE, CHIL
        // TODO what additional error handling?
        private void xrefProc(ParseContext2 context)
        {
            var fam = (context.Parent as FamRecord);

            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline;
                fam.Errors.Add(err);
            }
            else
            {
                switch (context.Tag)
                {
                    case "HUSB":
                        fam.Dad = xref; // TODO check for HUSB already specified
                        break;
                    case "WIFE":
                        fam.Mom = xref; // TODO check for WIFE already specified
                        break;
                    case "CHIL":
                        fam.Childs.Add(xref);
                        break;
                    case "SUBM":
                        fam.FamSubm.Add(xref);
                        break;
                }
            }
        }

        private void resnProc(ParseContext2 context)
        {
            var fam = (context.Parent as FamRecord);
            fam.Restriction = context.Remain; // TODO post-evaluate for correctness
        }

        private void eventProc(ParseContext2 context)
        {
            // 5.5.1 : MARR allows 'Y'
            // 5.5 : most events allow 'Y'
            // Syntax changed between 5.5 and 5.5.1 for HUSB.AGE / WIFE.AGE
            var famEvent = FamilyEventParse.Parse(context);
            var fam = (context.Parent as FamRecord);
            fam.FamEvents.Add(famEvent);
        }

        public override void PostCheck(GEDCommon rec)
        {
            var me = rec as FamRecord;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing identifier"; // TODO assign one?
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }
        }

    }
}