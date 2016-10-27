﻿using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class IndiRecord : GEDCommon, NoteHold, SourceCitHold, MediaHold
    {
        public static string Tag = "INDI";

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

        private List<FamilyEvent> _events; // TODO is an INDI specific event necessary?
        public List<FamilyEvent> Events { get { return _events ?? (_events = new List<FamilyEvent>()); } }

        private List<FamilyEvent> _attribs; // TODO is an INDI specific event necessary?
        public List<FamilyEvent> Attribs { get { return _attribs ?? (_attribs = new List<FamilyEvent>()); } }

        // xref strings for children
        private List<string> _childs;
        public List<string> ChildLinks { get { return _childs ?? (_childs = new List<string>()); } }

        // xref strings for spouses
        private List<string> _spouses;
        public List<string> FamLinks { get { return _spouses ?? (_spouses = new List<string>()); } }

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

        public IndiRecord(GedRecord lines, string ident) : base(lines, ident)
        {          
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