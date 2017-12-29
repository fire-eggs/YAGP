using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGEDParser.Model
{
    public class Child
    {
        public string Xref { get; set; }

        public string MotherRelation { get; set; }

        public string FatherRelation { get; set; }
    }

    public class FamRecord : GEDCommon, NoteHold, SourceCitHold, MediaHold
    {
		/// <summary>
        /// GEDCOM Tag value.
        /// </summary>
        public override string Tag { get { return "FAM"; } }

        private List<Note> _notes;
        private List<SourceCit> _cits;
        private List<MediaLink> _media;
        private List<LDSEvent> _ldsEvents; // TODO common?

        /// Any NOTEs associated with the record. An empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
        /// Any Source citations associated with the record. An empty list if none.
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
        /// Any Multimedia objects associated with the record. An empty list if none.
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
		///
        public List<LDSEvent> LDSEvents { get { return _ldsEvents ?? (_ldsEvents = new List<LDSEvent>()); } }

        // Identity strings for children
        private List<Child> _childs;
		/// A list of 'CHIL' cross-references to INDI records.
		///
		/// Will be an empty list if there are none. See #Child class for more details.
		/// Future: use a grammatically correct name
        public List<Child> Childs { get { return _childs ?? (_childs = new List<Child>()); } }

        // Identity strings for HUSB (multiple are possible from some programs)
        private List<string> _dads;
		/// A list of 'HUSB' cross-references to INDI records.
		///
		/// Will be an empty list if there are none. A single entry is GEDCOM standard
		/// conformant, but this is a list because multiple entries are possible from 
		/// some programs.
		/// Future: use a less sexist name
        public List<string> Dads { get { return _dads ?? (_dads = new List<string>()); } }

        // Identity strings for WIFE (multiple are possible from some programs)
        private List<string> _moms;
		/// A list of 'WIFE' cross-references to INDI records.
		///
		/// Will be an empty list if there are none. A single entry is GEDCOM standard
		/// conformant, but this is a list because multiple entries are possible from 
		/// some programs.
		/// Future: use a less sexist name
        public List<string> Moms { get { return _moms ?? (_moms = new List<string>()); } }

        private List<FamilyEvent> _famEvents; // TODO common?
		/// The events associated to a family. 
		///
		/// Will be an empty list if there are none.
		///
		/// Events include the GEDCOM tags MARR, MARB, etc.
        public List<FamilyEvent> FamEvents { get { return _famEvents ?? (_famEvents = new List<FamilyEvent>()); } }

        // Identity strings for submitters
        private List<string> _famSubm; // TODO common?
		/// The list of submitter cross-references for the person.
		///
		/// Will be an empty list if there are none.
		/// The list contains SUBM, ANCI and DESI references.
        public List<string> FamSubm { get { return _famSubm ?? (_famSubm = new List<string>()); } }

        internal FamRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            ChildCount = -1;
            GedRecParse.NonStandardRemain(remain, this);
        }

		// TODO make this disappear
        //[ExcludeFromCodeCoverage]
        //public override string ToString()
        //{
        //    return Tag;
        //}

		// TODO make this disappear
        //[ExcludeFromCodeCoverage]
        //public string Marriage
        //{
        //    get
        //    {
        //        foreach (var kbrGedEvent in FamEvents)
        //        {
        //            if (kbrGedEvent.Tag == "MARR")
        //            {
        //                return kbrGedEvent.Date + " " + kbrGedEvent.Place;
        //            }
        //        }
        //        return "";
        //    }
        //}

		/// The value of the NCHI tag from the GEDCOM.
		///
		/// Will be -1 if the NCHI tag was not specified. This field value is not
		/// guaranteed to match the number of entries in the #Childs list.
        public int ChildCount { get; set; }

		/// Any restriction notice applied to the record.
		///
		/// Will be an empty string if none.
		/// The GEDCOM standard values are "confidential", "locked" or "privacy".
        public string Restriction { get; set; }

        internal void AddChild(string xref, string frel = null, string mrel = null)
        {
            // Add a child record, given a cross reference id.
            // Assumes that the father/mother relation is 'Natural'
            Child ch = new Child();
            ch.Xref = xref;
            ch.FatherRelation = frel;
            ch.MotherRelation = mrel;
            Childs.Add(ch);
        }
    }
}
