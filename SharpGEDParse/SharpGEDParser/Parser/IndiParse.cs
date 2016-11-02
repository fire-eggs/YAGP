using System;
using SharpGEDParser.Model;

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

            _tagSet2.Add("_UID", UidProc);
            _tagSet2.Add("UID", UidProc);
            _tagSet2.Add("RFN", rfnProc);
            _tagSet2.Add("AFN", afnProc);

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
            _tagSet2.Add("FAMC", xrefProc);
            _tagSet2.Add("FAMS", xrefProc);

            // Non-standard tags
            _tagSet2.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            _tagSet2.Add("LVNG", LivingProc); // "Generations" custom
        }

        private void LivingProc(ParseContext2 context)
        {
            (context.Parent as IndiRecord).Living = true;
        }

        private void afnProc(ParseContext2 context)
        {
            DataProc(context, false);
        }

        private void rfnProc(ParseContext2 context)
        {
            DataProc(context, false);
        }

        private void UidProc(ParseContext2 context)
        {
            DataProc(context, false);
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
            var rec = new NameRec();
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;

            string line = ctx.Remain;
            int max = line.Length;

            // BOULDER_CEM_02212009b.GED had a "1 NAME" with nothing else
            int startName = GedLineUtil.FirstChar(line, 0, max);
            if (startName >= 0)
            {
                int startSur = GedLineUtil.AllCharsUntil(line, max, startName, '/');
                int endSur = GedLineUtil.AllCharsUntil(line, max, startSur + 1, '/');

                var suffix = "";
                if (endSur + 1 < max)
                    suffix = line.Substring(endSur + 1).Trim();

                rec.Names = line.Substring(startName, startSur - startName).Trim();
                rec.Names = string.Join(" ", rec.Names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)); // Remove extra spaces
                if (startSur < max) // e.g. "1 NAME LIVING"
                    rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1);
                if (suffix.Length > 0)
                    rec.Suffix = suffix;
            }

            //rec.Names = context.Remain.Trim();
            (ctx.Parent as IndiRecord).Names.Add(rec);
            // TODO parsing details
            // TODO parsing sub-records
        }

        private void EventProc(ParseContext2 context)
        {
            var @event = FamilyEventParse.Parse(context); // TODO family flag? post-check? what INDI specific details?
            var indi = (context.Parent as IndiRecord);
            indi.Events.Add(@event);
        }

        private void LdsOrdProc(ParseContext2 context)
        {
            LDSEvent evt = LDSEventParse.Parse(context);
            var indi = (context.Parent as IndiRecord);
            indi.LDSEvents.Add(evt);
        }

        private void AttribProc(ParseContext2 context)
        {
            var @event = FamilyEventParse.Parse(context); // TODO family flag? post-check? what INDI specific details?
            var indi = (context.Parent as IndiRecord);
            indi.Attribs.Add(@event);
        }

        private void resnProc(ParseContext2 context)
        {
            var fam = (context.Parent as IndiRecord);
            fam.Restriction = context.Remain.Trim(); // TODO post-evaluate for correctness
        }

        // Common processing for SUBM, FAMC, FAMS
        // TODO what additional error handling?
        private void xrefProc(ParseContext2 context)
        {
            var indi = (context.Parent as IndiRecord);
            if (indi == null)
                return; // TODO throw exception?

            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing/unterminated identifier: " + context.Tag;
                err.Beg = err.End = context.Begline;
                indi.Errors.Add(err);
            }
            else
            {
                switch (context.Tag)
                {
                    case "FAMS":
                        indi.FamLinks.Add(xref);
                        break;
                    case "FAMC":
                        indi.ChildLinks.Add(xref);
                        break;
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
                err.Error = "Missing identifier"; // TODO assign one?
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // Make sure sex is set
            if (!"MFU".Contains(me.Sex.ToString().ToUpper()))
                me.Sex = 'U'; // TODO warning

        }

    }
}
