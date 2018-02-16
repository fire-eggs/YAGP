using System.Collections.Generic;

namespace SharpGEDParser.Model
{
	/// <summary>
    /// A repository citation from a source (SOUR) record.
	/// </summary>
	/// A cross-reference to a repository (REPO) record. The repository record identifies
	/// where this source is held.
    public class RepoCit : StructCommon, NoteHold
    {
		/// <summary>
		/// Call number data for formal repositories.
		/// </summary>
        public class CallNum
        {
			/// <summary>
			/// The repository identification or reference description.
			/// </summary>
            public string Number { get; set; }
			/// <summary>
			/// The type of material in which the referenced source is stored.
			/// </summary>
            public string Media { get; set; }
        }

		/// <summary>
		/// The identifier of the REPO record.
		/// </summary>
        public string Xref { get; set; }

        private List<CallNum> _callNums;
		/// <summary>
		/// Call number information for formal repositories.
		/// </summary>
		/// Will be an empty list if none.
        public List<CallNum> CallNums { get { return _callNums ?? (_callNums = new List<CallNum>()); }}

        private List<Note> _notes;

		/// <summary>
		/// NOTEs or informal repository data for this source.
		/// </summary>
		/// Will be an empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
