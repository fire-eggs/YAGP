using System;
using System.Globalization;
using SharpGEDParser.Model;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Parser
{
    public class IndiParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add(GedTag.CHAN, ChanProc);
            _tagSet2.Add(GedTag.NOTE, NoteProc);
            _tagSet2.Add(GedTag.OBJE, ObjeProc);
            _tagSet2.Add(GedTag.REFN, RefnProc);
            _tagSet2.Add(GedTag.RIN, RinProc);
            _tagSet2.Add(GedTag.SOUR, SourCitProc);

            _tagSet2.Add(GedTag.RESN, resnProc);
            _tagSet2.Add(GedTag.NAME, NameProc);
            _tagSet2.Add(GedTag.SEX,  SexProc);
            _tagSet2.Add(GedTag.SUBM, xrefProc);
            _tagSet2.Add(GedTag.ASSO, AssocProc);
            _tagSet2.Add(GedTag.ALIA, aliasProc);
            _tagSet2.Add(GedTag.ANCI, xrefProc);
            _tagSet2.Add(GedTag.DESI, xrefProc);

            _tagSet2.Add(GedTag._UID, UidProc);
            _tagSet2.Add(GedTag.UID, UidProc);
            _tagSet2.Add(GedTag.RFN,  RfnProc);
            _tagSet2.Add(GedTag.AFN,  AfnProc);

            // Events
            _tagSet2.Add(GedTag.DEAT, EventProc);
            _tagSet2.Add(GedTag.CREM, EventProc);
            _tagSet2.Add(GedTag.BURI, EventProc);
            _tagSet2.Add(GedTag.NATU, EventProc);
            _tagSet2.Add(GedTag.IMMI, EventProc);
            _tagSet2.Add(GedTag.WILL, EventProc);
            _tagSet2.Add(GedTag.EMIG, EventProc);
            _tagSet2.Add(GedTag.BAPM, EventProc);
            _tagSet2.Add(GedTag.BARM, EventProc);
            _tagSet2.Add(GedTag.BASM, EventProc);
            _tagSet2.Add(GedTag.BLES, EventProc);
            _tagSet2.Add(GedTag.CHRA, EventProc);
            _tagSet2.Add(GedTag.CONF, EventProc);
            _tagSet2.Add(GedTag.FCOM, EventProc);
            _tagSet2.Add(GedTag.ORDN, EventProc);
            _tagSet2.Add(GedTag.PROB, EventProc);
            _tagSet2.Add(GedTag.GRAD, EventProc);
            _tagSet2.Add(GedTag.RETI, EventProc);
            _tagSet2.Add(GedTag.CENS, EventProc); // 20170518 Actually an event, not an attribute!!!
            _tagSet2.Add(GedTag.EVEN, EventProc);

            // Birth, Christening, Adoption: extra FAMC tag
            _tagSet2.Add(GedTag.BIRT, EventProc); 
            _tagSet2.Add(GedTag.ADOP, EventProc); 
            _tagSet2.Add(GedTag.CHR,  EventProc); 

            // Attributes
            _tagSet2.Add(GedTag.CAST, AttribProc);
            _tagSet2.Add(GedTag.TITL, AttribProc);
            _tagSet2.Add(GedTag.OCCU, AttribProc);
            _tagSet2.Add(GedTag.FACT, AttribProc);
            _tagSet2.Add(GedTag.DSCR, AttribProc);
            _tagSet2.Add(GedTag.EDUC, AttribProc);
            _tagSet2.Add(GedTag.IDNO, AttribProc);
            _tagSet2.Add(GedTag.NATI, AttribProc);
            _tagSet2.Add(GedTag.NCHI, AttribProc);
            _tagSet2.Add(GedTag.NMR, AttribProc);
            _tagSet2.Add(GedTag.PROP, AttribProc);
            _tagSet2.Add(GedTag.RELI, AttribProc);
            _tagSet2.Add(GedTag.SSN, AttribProc);
            _tagSet2.Add(GedTag.RESI, AttribProc);

            // LDS events
            _tagSet2.Add(GedTag.BAPL, LdsOrdProc);
            _tagSet2.Add(GedTag.CONL, LdsOrdProc);
            _tagSet2.Add(GedTag.ENDL, LdsOrdProc);
            _tagSet2.Add(GedTag.SLGC, LdsOrdProc);
            _tagSet2.Add(GedTag.SLGS, LdsOrdProc);

            // Family association
            _tagSet2.Add(GedTag.FAMC, famLink);
            _tagSet2.Add(GedTag.FAMS, famLink);

            // Non-standard tags
            _tagSet2.Add(GedTag.LVG, LivingProc); // "Family Tree Maker for Windows" custom
            _tagSet2.Add(GedTag.LVNG, LivingProc); // "Generations" custom
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

        //private static StringCache2 _sexCache = new StringCache2();

        private void SexProc(ParseContext2 context)
        {
            var own = (context.Parent as IndiRecord);
            var rem0 = context.Remain1;
            own.FullSex = null;

            // Hopefully the 99% case: single 'M' or 'F' w/ no trailing space
            if (rem0.Length == 1)
            {
                own.Sex = rem0[0];
                if (own.Sex != 'M' && own.Sex != 'F')
                    own.FullSex = context.Remain;
                return;
            }

            string rem = context.Remain.Trim();
            if (string.IsNullOrEmpty(rem))
            {
                own.Sex = 'U';
            }
            else
            {
                own.Sex = rem[0];
                if (rem != "M" && rem != "F" )
                    own.FullSex = rem;
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
            var @event = FamilyEventParse.Parse(context, true);
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
            var @event = FamilyEventParse.Parse(context, true);
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
            if (res == null)
                return; // Fallout from GedValid: on error don't record the link
            var own = (context.Parent as IndiRecord);
            own.Links.Add(res);
        }

        // Some GEDCOM have been seen to use slashes around non-standard ALIA text
        private static readonly char[] aliasTrimChars = {' ', '\t', '/'};

        // ALIAS (ALIA) specific processing. A common but non-standard use of ALIA
        // is to not specify a cross-reference.
        private static void aliasProc(ParseContext2 context)
        {
            var indi = (context.Parent as IndiRecord);

            string xref = parseForXref(context, UnkRec.ErrorCode.NonStdAlias);
            if (string.IsNullOrEmpty(xref))
            {
                IndiEvent nick = new IndiEvent();
                nick.Tag = GedTag.ALIA;
                nick.Descriptor = context.Remain.Trim(aliasTrimChars);
                indi.Attribs.Add(nick);
            }
            else
            {
                indi.AliasLinks.Add(xref);
            }
        }

        // Common processing for SUBM, ANCI, DESI
        // TODO what additional error handling?
        private static void xrefProc(ParseContext2 context)
        {
            var indi = (context.Parent as IndiRecord);

            string xref = parseForXref(context);
            if (!string.IsNullOrEmpty(xref))
            {
                Submitter.SubmitType res;
                if (Submitter.SubmitType.TryParse(context.Tag.ToString(), out res)) // TODO can this be improved?
                    indi.AddSubmitter(res, xref);
                else
                    throw new NotSupportedException(); // NOTE: this will be thrown if a tag is added to tagDict but not added to enum
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
                err.Tag = rec.Tag;
                me.Errors.Add(err);
            }

            // TODO should this be checked in the SEX routine so line # points at the actual line???
            // Make sure sex is set
            if (me.Sex != '\0')
            {
                if (!"MFUmfu".Contains(me.Sex.ToString(CultureInfo.InvariantCulture)))
                {
                    me.Sex = 'U';

                    UnkRec err = new UnkRec();
                    err.Error = UnkRec.ErrorCode.InvSex; // "Non-standard SEX value corrected to U";
                    err.Beg = err.End = me.BegLine;
                    me.Errors.Add(err);  // TODO as warning
                }
            }
            CheckRestriction(me, me.Restriction);

            // TODO check restriction value on events

        }

    }
}
