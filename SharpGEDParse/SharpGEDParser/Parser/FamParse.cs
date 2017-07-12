using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parsing for the FAM (family) record.

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

            _tagSet2.Add("CHIL", childProc);
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

            _tagSet2.Add("_UID", DataProc);
            _tagSet2.Add("UID",  DataProc);

            _tagSet2.Add("_PREF", junkProc);

            // Drat. These are sub-tags per-child. Child doesn't yet have more than an id string.
            //_tagSet2.Add("_FREL", UidProc); // Commonly encountered non-standard tags
            //_tagSet2.Add("_MREL", UidProc);
        }

        private void junkProc(ParseContext2 context)
        {
            // TODO throw away
        }

        private void ldsSpouseSeal(ParseContext2 context)
        {
            LDSEvent evt = LDSEventParse.Parse(context);
            var fam = (context.Parent as FamRecord);
            fam.LDSEvents.Add(evt);
        }

        private void nchiProc(ParseContext2 context)
        {
            // TODO Data loss: The Master Genealogist (TMG) treats NCHI as a text field.
            // NCHI in TMG can have CONC, CONT, and other sub-tags. Here, these sub-tags 
            // are not correctly connected to the NCHI tag, nor am I preserving the NCHI 
            // data as entered.

            var fam = (context.Parent as FamRecord);

            int childCount;
            if (!int.TryParse(context.Remain, out childCount))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.InvNCHI; //"Invalid child count";
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                fam.Errors.Add(err);
            }
            else if (fam.ChildCount != -1) // has been specified once already
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MultNCHI;
                //err.Error = "Child count specified more than once";
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                fam.Errors.Add(err);
            }
            else
            {
                fam.ChildCount = childCount;
            }
        }

        // CHIL processing pulled out for _FREL/_MREL
        private void childProc(ParseContext2 context)
        {
            var fam = (context.Parent as FamRecord);

            LookAhead(context); // Any sub-lines (e.g. _FREL/_MREL)?

            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO "Missing/unterminated identifier: " + context.Tag;
                err.Beg = context.Begline + context.Parent.BegLine;
                err.End = context.Endline + context.Parent.BegLine;
                fam.Errors.Add(err);
                return;
            }

            foreach (var child in fam.Childs)
            {
                if (child.Xref == xref)
                {
                    UnkRec err = new UnkRec();
                    err.Error = UnkRec.ErrorCode.MultCHIL; // TODO "CHIL ident used more than once (one person cannot be two children)";
                    err.Beg = context.Begline + context.Parent.BegLine;
                    err.End = context.Endline + context.Parent.BegLine;
                    fam.Errors.Add(err);
                    return;
                }
            }

            string mrel = null;
            string frel = null;
            if (context.Endline > context.Begline)
            {
                LineUtil.LineData ld = new LineUtil.LineData();
                var gs = new GEDSplitter();
                //ParseContext2 ctx = new ParseContext2();
                int i = context.Begline + 1;
                while (i <= context.Endline)
                {
                    //LineUtil.LevelTagAndRemain(ld, context.Lines.GetLine(i));
                    gs.LevelTagAndRemain(context.Lines.GetLine(i), ld);
                    switch (ld.Tag)
                    {
                        case "_MREL":
                            if (!string.IsNullOrWhiteSpace(ld.Remain)) // FTA expects 'Natural' and I canna see why not && ld.Remain != "Natural")
                                mrel = ld.Remain;
                            break;
                        case "_FREL":
                            if (!string.IsNullOrWhiteSpace(ld.Remain)) // FTA expects 'Natural' and I canna see why not && ld.Remain != "Natural")
                                frel = ld.Remain;
                            break;
                        case "_PREF":
                        case "_STAT":
                            // TODO temporarily ignore: see 2524482.ged
                            break;
                        default:
                            UnkRec unk = new UnkRec(ld.Tag, i + context.Parent.BegLine, i + context.Parent.BegLine);
                            fam.Unknowns.Add(unk);
                            break;
                    }
                    i++;
                }
                gs = null;
            }
            fam.AddChild(xref, frel, mrel);
        }

        // Common processing for SUBM, HUSB, WIFE
        private void xrefProc(ParseContext2 context)
        {
            // TODO how are sub-tags handled? E.g. _PREF on HUSB, WIFE

            var fam = (context.Parent as FamRecord);

            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent;
                //err.Error = "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                fam.Errors.Add(err);
            }
            else
            {
                switch (context.Tag)
                {
                    case "HUSB":
                        if (fam.Dads.Count != 0) // HUSB already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = UnkRec.ErrorCode.MultHUSB; //"HUSB line used more than once";
                            err.Beg = err.End = context.Begline + context.Parent.BegLine;
                            fam.Errors.Add(err);
                        }
                        fam.Dads.Add(xref);
                        break;
                    case "WIFE":
                        if (fam.Moms.Count != 0) // WIFE already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = UnkRec.ErrorCode.MultWIFE; //"WIFE line used more than once";
                            err.Beg = err.End = context.Begline + context.Parent.BegLine;
                            fam.Errors.Add(err);
                        }
                        fam.Moms.Add(xref);
                        break;
                    //case "CHIL":
                    //    foreach (var child in fam.Childs)
                    //    {
                    //        if (child == xref)
                    //        {
                    //            UnkRec err = new UnkRec();
                    //            err.Error = "CHIL ident used more than once (one person cannot be two children)";
                    //            err.Beg = err.End = context.Begline + context.Parent.BegLine;
                    //            fam.Errors.Add(err);
                    //            return;
                    //        }
                    //    }
                    //    fam.Childs.Add(xref);
                    //    break;
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
                err.Error = UnkRec.ErrorCode.MultRESN; // TODO "RESN specified more than once";
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                fam.Errors.Add(err);
            }
        }

        private void eventProc(ParseContext2 context)
        {
            // 5.5.1 : MARR allows 'Y'
            // 5.5 : most events allow 'Y'
            // Syntax changed between 5.5 and 5.5.1 for HUSB.AGE / WIFE.AGE
            var famEvent = FamilyEventParse.Parse(context, false);
            var fam = (context.Parent as FamRecord);
            fam.FamEvents.Add((FamilyEvent)famEvent);
        }

        public override void PostCheck(GEDCommon rec)
        {
            var me = rec as FamRecord;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // "Missing identifier"; // TODO assign one?
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // TODO NCHI value doesn't match # of CHIL refs?

            CheckRestriction(me, me.Restriction);

            // TODO check restriction value on events
        }

    }
}