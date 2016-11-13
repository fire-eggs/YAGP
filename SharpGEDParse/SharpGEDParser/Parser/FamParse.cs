using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parsing for the FAM (family) record.

    // TODO validate that HUSB, WIFE, SUBM, CHIL idents actually exist [final post-process stage]

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

            _tagSet2.Add("_UID", UidProc);
            _tagSet2.Add("UID", UidProc);
        }

        private void ldsSpouseSeal(ParseContext2 context)
        {
            LDSEvent evt = LDSEventParse.Parse(context);
            var fam = (context.Parent as FamRecord);
            fam.LDSEvents.Add(evt);
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
            else if (fam.ChildCount != -1) // has been specified once already
            {
                UnkRec err = new UnkRec();
                err.Error = "Child count specified more than once";
                err.Beg = err.End = context.Begline;
                fam.Errors.Add(err);
            }
            else
            {
                fam.ChildCount = childCount;
            }
        }

        // Common processing for SUBM, HUSB, WIFE, CHIL
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
                        if (fam.Dad != null) // HUSB already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = "HUSB line used more than once";
                            err.Beg = err.End = context.Begline;
                            fam.Errors.Add(err);
                        }
                        else
                            fam.Dad = xref;
                        break;
                    case "WIFE":
                        if (fam.Mom != null) // HUSB already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = "WIFE line used more than once";
                            err.Beg = err.End = context.Begline;
                            fam.Errors.Add(err);
                        }
                        else
                            fam.Mom = xref;
                        break;
                    case "CHIL":
                        foreach (var child in fam.Childs)
                        {
                            if (child == xref)
                            {
                                UnkRec err = new UnkRec();
                                err.Error = "CHIL ident used more than once (one person cannot be two children)";
                                err.Beg = err.End = context.Begline;
                                fam.Errors.Add(err);
                                return;
                            }
                        }
                        fam.Childs.Add(xref);
                        break;
                    case "SUBM":
                        fam.FamSubm.Add(xref); // TODO check if xref specified more than once
                        break;
                }
            }
        }

        private void resnProc(ParseContext2 context)
        {
            var fam = (context.Parent as FamRecord);
            if (string.IsNullOrEmpty(fam.Restriction))
            {
                fam.Restriction = context.Remain.Trim();
            }
            else
            {
                UnkRec err = new UnkRec();
                err.Error = "RESN specified more than once";
                err.Beg = err.End = context.Begline;
                fam.Errors.Add(err);
            }
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

        private void UidProc(ParseContext2 context)
        {
            DataProc(context, false);
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

            // TODO NCHI value doesn't match # of CHIL refs?

            CheckRestriction(me, me.Restriction);

            // TODO check restriction value on events
        }

    }
}