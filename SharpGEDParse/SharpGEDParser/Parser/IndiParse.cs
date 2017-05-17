using System;
using System.Globalization;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Parser
{
    public class IndiParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("OBJE", ObjeProc);
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN", RinProc);
            _tagSet2.Add("SOUR", SourCitProc);

            _tagSet2.Add("RESN", resnProc);
            _tagSet2.Add("NAME", NameProc);
            _tagSet2.Add("SEX",  SexProc);
            _tagSet2.Add("SUBM", xrefProc);
            _tagSet2.Add("ASSO", AssocProc);
            _tagSet2.Add("ALIA", xrefProc);
            _tagSet2.Add("ANCI", xrefProc);
            _tagSet2.Add("DESI", xrefProc);

            _tagSet2.Add("_UID", DataProc);
            _tagSet2.Add("UID",  DataProc);
            _tagSet2.Add("RFN",  DataProc);
            _tagSet2.Add("AFN",  DataProc);

            // Events
            _tagSet2.Add("DEAT", EventProc);
            _tagSet2.Add("CREM", EventProc);
            _tagSet2.Add("BURI", EventProc);
            _tagSet2.Add("NATU", EventProc);
            _tagSet2.Add("IMMI", EventProc);
            _tagSet2.Add("WILL", EventProc);
            _tagSet2.Add("EMIG", EventProc);
            _tagSet2.Add("BAPM", EventProc);
            _tagSet2.Add("BARM", EventProc);
            _tagSet2.Add("BASM", EventProc);
            _tagSet2.Add("BLES", EventProc);
            _tagSet2.Add("CHRA", EventProc);
            _tagSet2.Add("CONF", EventProc);
            _tagSet2.Add("FCOM", EventProc);
            _tagSet2.Add("ORDN", EventProc);
            _tagSet2.Add("PROB", EventProc);
            _tagSet2.Add("GRAD", EventProc);
            _tagSet2.Add("RETI", EventProc);
            _tagSet2.Add("EVEN", EventProc);

            // Birth, Christening, Adoption: extra FAMC tag
            _tagSet2.Add("BIRT", EventProc); 
            _tagSet2.Add("ADOP", EventProc); 
            _tagSet2.Add("CHR",  EventProc); 

            // Attributes
            _tagSet2.Add("CAST", AttribProc);
            _tagSet2.Add("TITL", AttribProc);
            _tagSet2.Add("OCCU", AttribProc);
            _tagSet2.Add("FACT", AttribProc);
            _tagSet2.Add("DSCR", AttribProc);
            _tagSet2.Add("EDUC", AttribProc);
            _tagSet2.Add("IDNO", AttribProc);
            _tagSet2.Add("NATI", AttribProc);
            _tagSet2.Add("NCHI", AttribProc);
            _tagSet2.Add("NMR", AttribProc);
            _tagSet2.Add("PROP", AttribProc);
            _tagSet2.Add("RELI", AttribProc);
            _tagSet2.Add("SSN", AttribProc);
            _tagSet2.Add("CENS", AttribProc);
            _tagSet2.Add("RESI", AttribProc);

            // LDS events
            _tagSet2.Add("BAPL", LdsOrdProc);
            _tagSet2.Add("CONL", LdsOrdProc);
            _tagSet2.Add("ENDL", LdsOrdProc);
            _tagSet2.Add("SLGC", LdsOrdProc);
            _tagSet2.Add("SLGS", LdsOrdProc);

            // Family association
            _tagSet2.Add("FAMC", famLink);
            _tagSet2.Add("FAMS", famLink);

            // Non-standard tags
            _tagSet2.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            _tagSet2.Add("LVNG", LivingProc); // "Generations" custom
        }

        private void LivingProc(ParseContext2 context)
        {
            (context.Parent as IndiRecord).Living = true;
        }

        private void AssocProc(ParseContext2 context)
        {
            var res = IndiAssoParse.AssoParse(context);
            var own = (context.Parent as IndiRecord);
            own.Assocs.Add(res);
        }

        private void SexProc(ParseContext2 context)
        {
            var own = (context.Parent as IndiRecord);
            string rem = context.Remain.Trim();
            if (rem.Length < 1)
            {
                own.Sex = 'U';
                own.FullSex = "";
            }
            else
            {
                own.Sex = context.Remain[0];
                own.FullSex = context.Remain;
            }
        }

        private void NameProc(ParseContext2 ctx)
        {
            var rec = NameParse.Parse(ctx);
            var indi = (ctx.Parent as IndiRecord);
            indi.Names.Add(rec);
            // TODO parsing details
            // TODO parsing sub-records
        }

        private void EventProc(ParseContext2 context)
        {
            var @event = FamilyEventParse.Parse(context, true); // TODO family flag? post-check? what INDI specific details?
            var indi = (context.Parent as IndiRecord);
            indi.Events.Add(@event as IndiEvent);
        }

        private void LdsOrdProc(ParseContext2 context)
        {
            LDSEvent evt = LDSEventParse.Parse(context);
            var indi = (context.Parent as IndiRecord);
            indi.LDSEvents.Add(evt);
        }

        private void AttribProc(ParseContext2 context)
        {
            var @event = FamilyEventParse.Parse(context, true); // TODO family flag? post-check? what INDI specific details?
            var indi = (context.Parent as IndiRecord);
            indi.Attribs.Add((IndiEvent)@event);
        }

        private void resnProc(ParseContext2 context)
        {
            var indi = (context.Parent as IndiRecord);
            if (string.IsNullOrEmpty(indi.Restriction))
            {
                indi.Restriction = context.Remain.Trim();
            }
            else
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MultRESN; // "RESN specified more than once";
                err.Beg = err.End = context.Begline;
                indi.Errors.Add(err);
            }
        }

        private static void famLink(ParseContext2 context)
        {
            var res = IndiLinkParse.LinkParse(context);
            var own = (context.Parent as IndiRecord);
            own.Links.Add(res);
        }

        // Common processing for SUBM, FAMC, FAMS
        // TODO what additional error handling?
        private static void xrefProc(ParseContext2 context)
        {
            var indi = (context.Parent as IndiRecord);

            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                indi.Errors.Add(err);
            }
            else
            {
                switch (context.Tag)
                {
                    case "SUBM":
                        indi.AddSubmitter(IndiRecord.Submitter.SUBM, xref);
                        break;
                    case "ALIA":
                        indi.AliasLinks.Add(xref);
                        break;
                    case "DESI":
                        indi.AddSubmitter(IndiRecord.Submitter.DESI, xref);
                        break;
                    case "ANCI":
                        indi.AddSubmitter(IndiRecord.Submitter.ANCI, xref);
                        break;
                    default:
                        throw new NotSupportedException(); // NOTE: this will be thrown if a tag is added to tagDict but no case added here
                }
            }
        }

        public override void PostCheck(GEDCommon rec)
        {
            var me = rec as IndiRecord;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO "INDI missing identifier"; // TODO assign one?
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // TODO should this be checked in the SEX routine so line # points at the actual line???
            // Make sure sex is set
            if (!"MFU".Contains(me.Sex.ToString(CultureInfo.InvariantCulture).ToUpper()))
            {
                me.Sex = 'U'; // TODO warning

                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.InvSex; // "Non-standard SEX value corrected to U";
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            CheckRestriction(me, me.Restriction);

            // TODO check restriction value on events

        }

    }
}
