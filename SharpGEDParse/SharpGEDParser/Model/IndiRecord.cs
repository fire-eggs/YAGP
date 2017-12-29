using System.Collections.Generic;

namespace SharpGEDParser.Model
{
	/// Represents a record for an individual (GEDCOM INDI).
	///
    public class IndiRecord : GEDCommon, NoteHold, SourceCitHold, MediaHold
    {
		/// <summary>
        /// GEDCOM Tag value.
        /// </summary>
        public override string Tag { get { return "INDI"; } }

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

        // Identity strings for submitters [SUBM/ANCI/DESI]
        private List<Submitter> _submit; // TODO common?
		/// The list of submitter cross-references for the person.
		///
		/// Will be an empty list if there are none.
		/// The list contains SUBM, ANCI and DESI references.
        public List<Submitter> Submitters { get { return _submit ?? (_submit = new List<Submitter>()); } }

        private List<IndiEvent> _events;
		/// The events associated to an individual. 
		///
		/// Will be an empty list if there are none.
		///
		/// Events include the GEDCOM tags BIRT, DEAT, CHR, BURI, etc.
        public List<IndiEvent> Events { get { return _events ?? (_events = new List<IndiEvent>()); } }

        private List<IndiEvent> _attribs;
		/// The individual's attributes. 
		///
		/// Will be an empty list if there are none.
		///
		/// Attributes include the GEDCOM tags NATI, OCCU, FACT, etc.
        public List<IndiEvent> Attribs { get { return _attribs ?? (_attribs = new List<IndiEvent>()); } }

        // Family xref links
        // TODO xref-only accessors for children, spouses
        private List<IndiLink> _famLinks;
		/// The list of family links for the person.
		///
		/// Will be an empty list if there are none.
		/// The list contains both 'child-of' and 'spouse-in' links.
        public List<IndiLink> Links { get { return _famLinks ?? (_famLinks = new List<IndiLink>()); } }

        // xref strings for aliases [pointer to record which may be the same person]
        // TODO at later point must validate the referenced record exists
        private List<string> _aliases;
		/// A list of cross-reference strings for aliases.
		///
		/// Will be an empty list if none.
		///
		/// An alias is a pointer to another record which may be the same person
		/// as this individual.
        public List<string> AliasLinks { get { return _aliases ?? (_aliases = new List<string>()); } }

        private List<AssoRec> _assoc;
		/// A list of associations for the person.
		///
		/// Will be an empty list if none.
		///
        public List<AssoRec> Assocs { get { return _assoc ?? (_assoc = new List<AssoRec>()); } }

	    /// Any restriction notice applied to the record.
	    ///
	    /// Will be an empty string if none.
	    /// The GEDCOM standard values are "confidential", "locked" or "privacy".
	    public string Restriction { get; set; }

	    /// Is the individual alive?
		///
		/// Often used for restricted records, when the person's birth/death 
		/// information is not available.
        public bool Living { get; set; }

		/// The GEDCOM-standard indication of the person's sex.
		///
		/// Values are limited to 'M','F', and 'U'.
        public char Sex { get; set; }
		
		/// The value imported from the GEDCOM file representing the person's sex.
		///
		/// May be any string.
        public string FullSex { get; set; } // full details of sex

        private List<NameRec> _names;
		/// The collection of an individual's names.
		///
		/// Will return an empty list if none.
        public List<NameRec> Names { get { return _names ?? (_names = new List<NameRec>()); } }

        internal IndiRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            Sex = 'U'; // TODO is this the best thing to do?

            GedRecParse.NonStandardRemain(remain, this);
        }

		// TODO move elsewhere
        public class Submitter
        {
            public static int SUBM = 0;
            public static int DESI = 1;
            public static int ANCI = 2;
            public int SubmitterType;
            public string Xref;
        }

        internal void AddSubmitter(int submType, string ident)
        {
            Submitters.Add(new Submitter {SubmitterType = submType, Xref = ident});
            // TODO at later time must validate the specified xref exists
        }

		/// Does the person have any name records?
        public bool HasName
        {
            get { return Names.Count > 0; }
        }

		// TODO variations in part ordering (e.g. surname suffix, given)
		/// The individual's full name, using the first (preferred) name record.
		///
		/// Will be an empty string if the person has no name data.
		///
		/// Example:
		/// The GEDCOM record of "1 NAME John Doe /Jones/ Jr." will be returned
		/// by this property as "John Doe Jones Jr.".
		///
        public string FullName
        {
            get
            {
                if (!HasName)
                    return "";
                var name1 = Names[0];
                string name = name1.Names + " " + name1.Surname + " " + name1.Suffix;
                return name.Trim();
            }
        }
    }
}
