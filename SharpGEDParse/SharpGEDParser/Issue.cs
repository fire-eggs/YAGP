using System.Collections.Generic;


// TODO this should be in GedWrap instead
// TODO the use of an enum for error ids instead of external lookup may result in release problems

// ReSharper disable InconsistentNaming

namespace SharpGEDParser
{
    /// <summary>
    /// Represents a problem or situation discovered when processing a GEDCOM
    /// file. These are 'semantic' errors: problems involving record relationships.
    /// 
    /// 'Syntax' errors involving parsing issues are tracked via #UnkRec
    /// </summary>
    public class Issue
    {
        /// <summary>
        /// The error id for the situation.
        /// </summary>
        public IssueCode IssueId { get; set; } // message string id - localization

        private readonly List<object> _evidence = new List<object>();

        /// <summary>
        /// Provides the human-readable description of the situation.
        /// </summary>
        public string Message()
        {
            return string.Format(messages[(int) IssueId], _evidence.ToArray());
        }

        /// <summary>
        /// Possible error ids.
        /// </summary>
        public enum IssueCode
        {
            /// A duplicated INDI ident value encountered.
            DUPL_INDI = 0,
            /// A FAM record is missing its ident value.
            MISS_FAMID,
            /// A duplicated FAM ident value encountered.
            DUPL_FAM,
            /// An INDI record with a FAMC/FAMS line with no xref id.
            MISS_XREFID,
            /// 
            SPOUSE_CONN,
            /// An INDI record with a FAMS line and the referenced FAM record doesn't exist.
            FAMS_MISSING,
            /// An INDI record with a FAMC line and the referenced FAM record doesn't exist.
            FAMC_MISSING,
            /// 
            AMB_CONN,
            /// A FAM record has a HUSB/WIFE reference to a missing INDI record.
            SPOUSE_CONN_MISS,
            /// 
            CHIL_MISS,
            /// 
            CHIL_NOTMATCH,
            /// 
            SPOUSE_CONN_UNM,
            /// 
            FAMC_UNM,
            /// 
            FAMS_UNM,
            /// 
            UNKLINK
        };

        // TODO warn/error prefix is temporary for unit testing
        private readonly string[] messages =
        {
            "Error: Duplicate INDI ident {0}",
            "Error: Missing FAM id at/near line {0}",
            "Error: Duplicate family {0}",
            "Error: Empty link {1} xref id for INDI {0}",
            "Error: Could not identify spouse connection from FAM {0} to INDI {1}",
            "Error: INDI {0} has FAMS link {1} to non-existing family",
            "Error: INDI {0} has FAMC link {1} to non-existing family",
            "Warn: ambiguous {0} connection for family {1}", // {0} is 'dad'/'mom'/'unknown' // TODO L10N problem
            "Error: family {0} has {2} link {1} to non-existing INDI", // {2} is 'HUSB'/'WIFE' // TODO L10N problem
            "Error: family {0} has CHIL link {1} to non-existing INDI",
            "Error: family {0} has CHIL link {1} with no matching FAMC",
            "Error: family {0} has {2} link {1} with no matching FAMS", // {2} is 'HUSB'/'WIFE' // TODO L10N problem
            "Error: INDI {0} with FAMC link to {1} and no matching CHIL",
            "Error: INDI {0} with FAMS link to {1} and no matching HUSB/WIFE",
            "Error: INDI {0} with non-standard link '{1}'",
        };

        public Issue(IssueCode code, params object[] evidence)
        {
            IssueId = code;
            foreach (object t in evidence)
                this._evidence.Add(t);
        }
    }
}
