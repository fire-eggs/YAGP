using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents the header record from the GEDCOM.
    /// </summary>
    /// \todo useful information
    public class HeadRecord : GEDCommon, NoteHold
    {
        /// <summary>
        /// GEDCOM Tag value.
        /// </summary>
        public override string Tag { get { return "HEAD"; } }

        private List<Note> _notes;
		/// <summary>
		/// Any NOTEs associated with the header.
		/// </summary>
		/// Will be an empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        // Identity strings for submitters
        private List<Submitter> _submit; // TODO common?

		/// <summary>
		/// Submitter record cross-references.
		/// </summary>
		/// Will be an empty list if none.
        public List<Submitter> Submitters { get { return _submit ?? (_submit = new List<Submitter>()); } }

		/// <summary>
		/// The application used to create the GEDCOM.
		/// </summary>
        public string Source { get; set; }
		/// <summary>
		/// An identification string for the application used to create the GEDCOM.
		/// </summary>
        public string Product { get; set; }
		/// <summary>
		/// Version information about the creating application.
		/// </summary>
        public string ProductVersion { get; set; }

		/// <summary>
		/// The claimed version of the GEDCOM standard used.
		/// </summary>
		/// Only versions 5.5 and 5.5-1 are supported.
		/// Many applications claim the GEDCOM file is V5.5 yet use V5.5-1 features. 
        public string GedVersion { get; set; }

		/// <summary>
		/// The identified date of the GEDCOM file.
		/// </summary>
		/// todo how used to limit age calcs?
        public DateTime GedDate { get; set; }

		/// <summary>
		/// The identified character encoding of the GEDCOM file.
		/// </summary>
		/// todo details - ANSEL, IBM-PC and how handled
        public string CharSet { get; set; }
		
		/// <summary>
		/// The place hierarchy.
		/// </summary>
        public string PlaceFormat { get; set; }

        internal HeadRecord(GedRecord lines) : base(lines, null)
        {
            Source = Product = ProductVersion = GedVersion = CharSet = "";
            Ident = "HEAD";
        }

        internal void AddSubmitter(Submitter.SubmitType submType, string ident)
        {
            Submitters.Add(new Submitter { SubmitterType = submType, Xref = ident });
            // TODO at later time must validate the specified xref exists
        }

    }
}
