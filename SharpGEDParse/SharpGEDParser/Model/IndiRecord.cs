using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class IndiRecord : GEDCommon, NoteHold, SourceCitHold, MediaHold
    {
        public override string Tag { get { return "INDI"; } }

        private List<Note> _notes;
        private List<SourceCit> _cits;
        private List<MediaLink> _media;
        private List<LDSEvent> _ldsEvents; // TODO common?

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
        public List<LDSEvent> LDSEvents { get { return _ldsEvents ?? (_ldsEvents = new List<LDSEvent>()); } }

        // Identity strings for submitters
        private List<Submitter> _submit; // TODO common?
        public List<Submitter> Submitters { get { return _submit ?? (_submit = new List<Submitter>()); } }

        private List<IndiEvent> _events;
        public List<IndiEvent> Events { get { return _events ?? (_events = new List<IndiEvent>()); } }

        private List<IndiEvent> _attribs;
        public List<IndiEvent> Attribs { get { return _attribs ?? (_attribs = new List<IndiEvent>()); } }

        // Family xref links
        // TODO xref-only accessors for children, spouses
        private List<IndiLink> _famLinks;
        public List<IndiLink> Links { get { return _famLinks ?? (_famLinks = new List<IndiLink>()); } }

        // xref strings for aliases [pointer to record which may be the same person]
        // TODO at later point must validate the referenced record exists
        private List<string> _aliases;
        public List<string> AliasLinks { get { return _aliases ?? (_aliases = new List<string>()); } }

        private List<AssoRec> _assoc;
        public List<AssoRec> Assocs { get { return _assoc ?? (_assoc = new List<AssoRec>()); } }

        private string _restriction;
        public string Restriction
        {
            get { return _restriction; }
            set { _restriction = value; }
        }

        public bool Living { get; set; }

        public char Sex { get; set; }
        public string FullSex { get; set; } // full details of sex

        private List<NameRec> _names;
        public List<NameRec> Names { get { return _names ?? (_names = new List<NameRec>()); } }

        public IndiRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            Sex = 'U'; // TODO is this the best thing to do?

            GedRecParse.NonStandardRemain(remain, this);
        }

        public class Submitter
        {
            public static int SUBM = 0;
            public static int DESI = 1;
            public static int ANCI = 2;
            public int SubmitterType;
            public string Xref;
        }

        public void AddSubmitter(int submType, string ident)
        {
            Submitters.Add(new Submitter {SubmitterType = submType, Xref = ident});
            // TODO at later time must validate the specified xref exists
        }
    }
}
