using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Model
{
	/// <summary>
	/// Event details which are in common to both INDI and FAM events.
	/// </summary>
    public class EventCommon : StructCommon, NoteHold, SourceCitHold, MediaHold
    {
		/// <summary>
		/// The event tag.
		/// </summary>
		/// Example
		/// 1 BAPM
		/// the tag is GedTag.BAPM
        public GedTag Tag { get; set; }

		/// <summary>
		/// Any descriptor or other details associated with the event.
		/// </summary>
		/// Example 
		/// 1 EVEN description of the event
		/// the descriptor is all text following the tag.
        public string Descriptor { get; set; }

		/// <summary>
		/// The definition for a IDNO or FACT tag.
		/// </summary>
		/// Example
		/// 1 IDNO 123-45-6789
		/// 2 TYPE U.S. Social Security Number
        public string Type { get; set; } // detail, classification
		
		/// <summary>
		/// Responsible agency associated with the event.
		/// </summary>
        public string Agency { get; set; }
		
		/// <summary>
		/// Event cause associated with the event.
		/// </summary>
        public string Cause { get; set; }
		
		/// <summary>
		/// Religious affiliation associated with the event.
		/// </summary>
        public string Religion { get; set; }
		
		/// <summary>
		/// A restriction notice for the event.
		/// </summary>
        public string Restriction { get; set; }

		/// <summary>
		/// Address information for the event.
		/// </summary>
        public Address Address { get; set; }

		/// <summary>
		/// </summary>
        public string Place { get; set; } // TODO temporary - need full PLACE_STRUCTURE support

		/// <summary>
		/// The entered date value for the event.
		/// </summary>
        public string Date { get; set; }
		/// <summary>
		/// The interpreted date information for the event.
		/// </summary>
		/// \todo available if forest not used?
        public GEDDate GedDate { get; set; }

        private List<Note> _notes;
		/// <summary>
		/// Notes associated with the event.
		/// </summary>
		/// Will be an empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
		/// <summary>
		/// Source citations associated with the event.
		/// </summary>
		/// Will be an empty list if none.
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }

        private List<MediaLink> _media;
		/// <summary>
		/// Media associated with the event.
		/// </summary>
		/// Will be an empty list if none.
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }

		/// <summary>
		/// The age details for INDI events.
		/// </summary>
        public string Age { get; set; } // TODO move to IndiEvent?
    }
}
