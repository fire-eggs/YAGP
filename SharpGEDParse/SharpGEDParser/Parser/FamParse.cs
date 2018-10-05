using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Parser
{
    // Parsing for the FAM (family) record.

    public class FamParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add(GedTag.CHAN, ChanProc);
            _tagSet2.Add(GedTag.NOTE, NoteProc);
            _tagSet2.Add(GedTag.OBJE, ObjeProc);
            _tagSet2.Add(GedTag.REFN, RefnProc);
            _tagSet2.Add(GedTag.RIN,  RinProc);
            _tagSet2.Add(GedTag.SOUR, SourCitProc);

            _tagSet2.Add(GedTag.CHIL, childProc);
            _tagSet2.Add(GedTag.HUSB, xrefProc);
            _tagSet2.Add(GedTag.NCHI, nchiProc);
            _tagSet2.Add(GedTag.RESN, resnProc);
            _tagSet2.Add(GedTag.SUBM, xrefProc);
            _tagSet2.Add(GedTag.WIFE, xrefProc);

            _tagSet2.Add(GedTag.SLGS, ldsSpouseSeal);

            _tagSet2.Add(GedTag.ANUL, eventProc);
            _tagSet2.Add(GedTag.CENS, eventProc);
            _tagSet2.Add(GedTag.DIV,  eventProc);
            _tagSet2.Add(GedTag.DIVF, eventProc);
            _tagSet2.Add(GedTag.ENGA, eventProc);
            _tagSet2.Add(GedTag.EVEN, eventProc);
            _tagSet2.Add(GedTag.MARB, eventProc);
            _tagSet2.Add(GedTag.MARC, eventProc);
            _tagSet2.Add(GedTag.MARR, eventProc);
            _tagSet2.Add(GedTag.MARL, eventProc);
            _tagSet2.Add(GedTag.MARS, eventProc);
            _tagSet2.Add(GedTag.RESI, eventProc);

            _tagSet2.Add(GedTag._UID, UidProc);
            _tagSet2.Add(GedTag.UID,  UidProc);

            _tagSet2.Add(GedTag._PREF, junkProc);

            // Drat. These are sub-tags per-child. Child doesn't yet have more than an id string.
            //_tagSet2.Add(GedTag._FREL, UidProc); // Commonly encountered non-standard tags
            //_tagSet2.Add(GedTag._MREL, UidProc);
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
                err.Tag = context.TagAsString;
                fam.Errors.Add(err);
                return;
            }
#if XREFTRACK
            int key = XrefTrack.Instance.StoreXref(xref);
#endif
            foreach (var child in fam.Childs)
            {
#if XREFTRACK
                if (child.Key == key)
#else
                if (child.Xref == xref)
#endif
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
                //var gs = new GEDSplitter();
                //ParseContext2 ctx = new ParseContext2();
                int i = context.Begline + 1;
                while (i <= context.Endline)
                {
                    //LineUtil.LevelTagAndRemain(ld, context.Lines.GetLine(i));
                    context.gs.LevelTagAndRemain(context.Lines.GetLine(i), ld);
                    switch (ld.Tag)
                    {
                        case GedTag._MREL:
                            if (!string.IsNullOrWhiteSpace(ld.Remain)) // FTA expects 'Natural' and I canna see why not && ld.Remain != "Natural")
                                mrel = ld.Remain;
                            break;
                        case GedTag._FREL:
                            if (!string.IsNullOrWhiteSpace(ld.Remain)) // FTA expects 'Natural' and I canna see why not && ld.Remain != "Natural")
                                frel = ld.Remain;
                            break;
                        case GedTag._PREF:
                        case GedTag._STAT:
                            // TODO temporarily ignore: see 2524482.ged
                            break;
                        default:
                            UnkRec unk = new UnkRec(context.gs.TagAsString(context.Lines.GetLine(i)), i + context.Parent.BegLine, i + context.Parent.BegLine);
                            fam.Unknowns.Add(unk);
                            break;
                    }
                    i++;
                }
                //gs = null;
            }
            fam.AddChild(xref, frel, mrel);
        }

        // Common processing for SUBM, HUSB, WIFE
        private void xrefProc(ParseContext2 context)
        {
            // TODO how are sub-tags handled? E.g. _PREF on HUSB, WIFE

            string xref = parseForXref(context);
            var fam = (context.Parent as FamRecord);

            if (!string.IsNullOrEmpty(xref))
            {
                switch (context.Tag)
                {
                    case GedTag.HUSB:
                        if (fam.Dads.Count != 0) // HUSB already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = UnkRec.ErrorCode.MultHUSB; //"HUSB line used more than once";
                            err.Beg = err.End = context.Begline + context.Parent.BegLine;
                            fam.Errors.Add(err);
                        }
                        fam.Dads.Add(xref);
                        break;
                    case GedTag.WIFE:
                        if (fam.Moms.Count != 0) // WIFE already specified
                        {
                            UnkRec err = new UnkRec();
                            err.Error = UnkRec.ErrorCode.MultWIFE; //"WIFE line used more than once";
                            err.Beg = err.End = context.Begline + context.Parent.BegLine;
                            fam.Errors.Add(err);
                        }
                        fam.Moms.Add(xref);
                        break;
                    case GedTag.SUBM:
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
                err.Tag = rec.Tag;
                me.Errors.Add(err);
            }

            // TODO NCHI value doesn't match # of CHIL refs?

            CheckRestriction(me, me.Restriction);

            // TODO check restriction value on events
        }

    }
}