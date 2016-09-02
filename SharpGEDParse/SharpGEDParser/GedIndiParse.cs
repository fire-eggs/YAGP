using System;

namespace SharpGEDParser
{
    // TODO need to handle bad lines as errors (missing level, etc)

    public class GedIndiParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            // Details
            _tagSet.Add("NAME", NameProc);
            _tagSet.Add("RESN", RestrictProc);
            _tagSet.Add("SEX",  SexProc);
            _tagSet.Add("SUBM", SubmProc);
            _tagSet.Add("ASSO", AssocProc);
            _tagSet.Add("ALIA", AliasProc);
            _tagSet.Add("ANCI", AnciProc);
            _tagSet.Add("DESI", DesiProc);
            _tagSet.Add("REFN", DataProc); // TODO temporary - need type sub-record
            _tagSet.Add("SOUR", SourProc);
            _tagSet.Add("_UID", DataProc);
            _tagSet.Add("NOTE", NoteProc);
            _tagSet.Add("CHAN", ChanProc);
            _tagSet.Add("OBJE", DataProc); // TODO temporary

            _tagSet.Add("RFN", OneDataProc);
            _tagSet.Add("AFN", OneDataProc);
            _tagSet.Add("RIN", OneDataProc);

            // Non-standard tags
            _tagSet.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            _tagSet.Add("LVNG", LivingProc); // "Generations" custom

            // Events
            _tagSet.Add("DEAT", EventProc);
            _tagSet.Add("CREM", EventProc);
            _tagSet.Add("BURI", EventProc);
            _tagSet.Add("NATU", EventProc);
            _tagSet.Add("IMMI", EventProc);
            _tagSet.Add("WILL", EventProc);
            _tagSet.Add("EMIG", EventProc);
            _tagSet.Add("BAPM", EventProc);
            _tagSet.Add("BARM", EventProc);
            _tagSet.Add("BASM", EventProc);
            _tagSet.Add("BLES", EventProc);
            _tagSet.Add("CHRA", EventProc);
            _tagSet.Add("CONF", EventProc);
            _tagSet.Add("FCOM", EventProc);
            _tagSet.Add("ORDN", EventProc);
            _tagSet.Add("PROB", EventProc);
            _tagSet.Add("GRAD", EventProc);
            _tagSet.Add("RETI", EventProc);
            _tagSet.Add("EVEN", EventProc);

            _tagSet.Add("BIRT", BirtProc); // birth,adoption
            _tagSet.Add("ADOP", BirtProc); // birth,adoption
            _tagSet.Add("CHR",  BirtProc);

            // Attributes
            _tagSet.Add("CAST", AttribProc);
            _tagSet.Add("TITL", AttribProc);
            _tagSet.Add("OCCU", AttribProc);
            _tagSet.Add("FACT", AttribProc);
            _tagSet.Add("DSCR", AttribProc);
            _tagSet.Add("EDUC", AttribProc);
            _tagSet.Add("IDNO", AttribProc);
            _tagSet.Add("NATI", AttribProc);
            _tagSet.Add("NCHI", AttribProc);
            _tagSet.Add("NMR", AttribProc);
            _tagSet.Add("PROP", AttribProc);
            _tagSet.Add("RELI", AttribProc);
            _tagSet.Add("SSN", AttribProc);
            _tagSet.Add("CENS", AttribProc);
            _tagSet.Add("RESI", AttribProc);

            // LDS events
            _tagSet.Add("BAPL", LdsOrdProc);
            _tagSet.Add("CONL", LdsOrdProc);
            _tagSet.Add("ENDL", LdsOrdProc);
            _tagSet.Add("SLGC", LdsOrdProc);
            _tagSet.Add("SLGS", LdsOrdProc);

            // Family association
            _tagSet.Add("FAMC", ChildLink);
            _tagSet.Add("FAMS", SpouseLink);
        }

        private void RestrictProc()
        {
            string data = ctx.Line.Substring(ctx.Nextchar);
            if (string.IsNullOrWhiteSpace(data))
                return;
            (_rec as KBRGedIndi).Restriction = data.Trim();
        }

        private void DataProc()
        {
            string data = ctx.Line.Substring(ctx.Nextchar);
            var rec = new DataRec(ctx.Tag, data.Trim());
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;
            _rec.Data.Add(rec);
        }

        // GEDCOM standard states only one of these allowed, take the first
        private void OneDataProc()
        {
            if ((_rec as KBRGedIndi).HasData(ctx.Tag))
            {
                ErrorRec(string.Format("Multiple {0}: used first", ctx.Tag));
                return;
            }
            DataProc();
        }

        private XRefRec CommonXRefProcessing()
        {
            string ident = null;
            int res = GedLineUtil.Ident(ctx.Line, ctx.Max, ctx.Nextchar, ref ident);
            if (res != -1)
            {
                var rec = new XRefRec(ctx.Tag, ident);
                rec.Beg = ctx.Begline;
                rec.End = ctx.Endline;
                return rec;
            }

            ErrorRec(string.Format("Missing xref for {0}", ctx.Tag));
            return null;
        }

        private void DesiProc()
        {
            var rec = CommonXRefProcessing();
            if (rec != null)
                (_rec as KBRGedIndi).Desi.Add(rec);
        }

        private void AnciProc()
        {
            // TODO one GED has 'HIGH', 'LOW' and 'MEDIUM'... "Legacy"
            var rec = CommonXRefProcessing();
            if (rec != null)
                (_rec as KBRGedIndi).Anci.Add(rec);
        }

        private void AliasProc()
        {
            // TODO some samples have slashes; others names + /surname/
            string ident = ctx.Line.Substring(ctx.Nextchar).Trim();
            var rec = new XRefRec(ctx.Tag, ident);
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;
            (_rec as KBRGedIndi).Alia.Add(rec);
        }

        private void SubmProc()
        {
            // some real GEDs not using Xref format
            var rec = CommonXRefProcessing();
            if (rec != null)
                (_rec as KBRGedIndi).Subm.Add(rec);
        }

        private void LivingProc()
        {
            // Some programs explicitly indicate 'living'. 
            (_rec as KBRGedIndi).Living = true;
        }

        private void SpouseLink()
        {
            string ident = null;
            int res = GedLineUtil.Ident(ctx.Line, ctx.Max, ctx.Nextchar, ref ident);
            if (res == -1)
            {
                ErrorRec(string.Format("Missing xref for {0}", ctx.Tag));
                return;
            }

            var rec = new FamLinkRec(ident);
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;
            (_rec as KBRGedIndi).FamLinks.Add(rec);

            // TODO more than one note permitted!
            rec.Note = GedLineUtil.ParseForMulti(_rec.Lines, ctx.Begline+1, ctx.Endline, "NOTE"); // TODO better implementation?
        }

        private void ChildLink()
        {
            string ident = null;
            int res = GedLineUtil.Ident(ctx.Line, ctx.Max, ctx.Nextchar, ref ident);
            if (res == -1)
            {
                ErrorRec(string.Format("Missing xref for {0}", ctx.Tag));
                return;
            }

            var rec = new ChildLinkRec(ident);
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;
            (_rec as KBRGedIndi).ChildLinks.Add(rec);

            var lines = _rec.Lines;
            int begline = ctx.Begline;
            int endline = ctx.Endline;

            // TODO more than one note permitted!
            rec.Note = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "NOTE"); // TODO better implementation?
            rec.Pedi = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "PEDI"); // TODO better implementation?
            rec.Stat = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "STAT"); // TODO better implementation?
        }

        private void EventProc()
        {
            // TODO a second CREM record is an error?
            // TODO a second DEAT record is an error?

            var rec = CommonEventProcessing(_rec.Lines);
            (_rec as KBRGedIndi).Events.Add(rec);
        }

        private void BirtProc()
        {
            // TODO a second birth record is an error?
            var rec = CommonEventProcessing(_rec.Lines);
            (_rec as KBRGedIndi).Events.Add(rec);
        }

        private void SexProc()
        {
            // TODO log unknown value as warn?
            int sexDex = GedLineUtil.FirstChar(ctx.Line, ctx.Nextchar, ctx.Max);
            if (sexDex > 0)
            {
                (_rec as KBRGedIndi).Sex = ctx.Line[sexDex];
                if (!"MFU".Contains((_rec as KBRGedIndi).Sex.ToString().ToUpper()))
                    (_rec as KBRGedIndi).Sex = 'U';
            }
        }

        private void ChanProc() // TODO refactor to common
        {
            // GEDCOM spec says to take the FIRST
            if (_rec.Change != null)
            {
                ErrorRec("Multiple CHAN: first one used");
                return;
            }
            _rec.Change = new Tuple<int, int>(ctx.Begline, ctx.Endline);
        }

        private void LdsOrdProc()
        {
            // TODO these all parse similarly except SLGC which requires a FAMC tag
            var lines = _rec.Lines;
            int begline = ctx.Begline;
            int endline = ctx.Endline;

            var rec = new LDSRec(ctx.Tag);
            rec.Beg = begline;
            rec.End = endline;

            rec.Temple = GedLineUtil.ParseFor(lines, begline + 1, endline, "TEMP");
            rec.Place = GedLineUtil.ParseFor(lines, begline + 1, endline, "PLAC");

            // TODO SLGC specific?
            rec.Famc = GedLineUtil.ParseFor(lines, begline + 1, endline, "FAMC");

            rec.Status = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "STAT");

            // TODO date for this record vs date for Status sub-record?
            rec.Date = GedLineUtil.ParseFor(lines, begline + 1, endline, "DATE");

            // TODO more than one note permitted!
            rec.Note = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");

            // TODO more than one source permitted!
            rec.Source = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");

            (_rec as KBRGedIndi).LDSEvents.Add(rec);
        }

        private void AttribProc()
        {
            var rec = CommonEventProcessing(_rec.Lines);
            (_rec as KBRGedIndi).Attribs.Add(rec);
        }

        private void AssocProc()
        {
            var rec = CommonXRefProcessing();
            if (rec != null)
            {
                (_rec as KBRGedIndi).Assoc.Add(rec);

                var lines = _rec.Lines;
                var begline = ctx.Begline;
                var endline = ctx.Endline;

                rec.Rela = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "RELA"); // TODO better implementation?
                // TODO more than one source permitted?
                rec.Sour = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "SOUR"); // TODO better implementation?
                // TODO more than one note permitted!
                rec.Note = GedLineUtil.ParseForMulti(lines, begline + 1, endline, "NOTE"); // TODO better implementation?
            }
        }

        private void NameProc()
        {
            string line = ctx.Line;
            int nextchar = ctx.Nextchar;

            int max = line.Length;
            int startName = GedLineUtil.FirstChar(line, nextchar, max);

            var rec = new NameRec();
            rec.Beg = ctx.Begline;
            rec.End = ctx.Endline;

            // BOULDER_CEM_02212009b.GED had a "1 NAME" with nothing else
            if (startName >= 0)
            {
                int startSur = GedLineUtil.AllCharsUntil(line, max, startName, '/');
                int endSur = GedLineUtil.AllCharsUntil(line, max, startSur + 1, '/');

                var suffix = "";
                if (endSur+1 < max)
                    suffix = line.Substring(endSur+1).Trim();

                rec.Names = line.Substring(startName, startSur - startName).Trim();
                rec.Names = string.Join(" ", rec.Names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)); // Remove extra spaces
                if (startSur < max) // e.g. "1 NAME LIVING"
                    rec.Surname = line.Substring(startSur + 1, endSur - startSur - 1);
                if (suffix.Length > 0)
                    rec.Suffix = suffix;
            }

            (_rec as KBRGedIndi).Names.Add(rec);

            // TODO parse more details
        }

        private void SourProc()
        {
            SourCitProc(_rec);
        }

    }
}
