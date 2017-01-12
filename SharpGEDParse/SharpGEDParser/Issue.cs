using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SharpGEDParser
{
    public class Issue
    {
        // A situation has been discovered.
        public IssueCode IssueId { get; set; } // message string id - localization

        private List<object> evidence = new List<object>();

        public string Message()
        {
            return string.Format(messages[(int) IssueId], evidence.ToArray());
        }

        public enum IssueCode
        {
            DUPL_INDI = 0,
            MISS_FAMID,
            DUPL_FAM,
            MISS_XREFID,
            SPOUSE_CONN,
            FAMS_MISSING,
            FAMC_MISSING,
            AMB_CONN,
            SPOUSE_CONN2,
            CHIL_MISS,
            CHIL_NOTMATCH
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
            "Warn: ambiguous {0} connection for family {1}", // {0} is 'dad'/'mom' // TODO L10N problem
            "Error: family {0} has {2} link {1} to non-existing INDI", // {2} is 'HUSB'/'WIFE' // TODO L10N problem
            "Error: family {0} has CHIL link {1} to non-existing INDI",
            "Error: family {0} has CHIL link {1} with no matching FAMC",
        };

        public Issue(IssueCode code, params object[] evidence)
        {
            IssueId = code;
            foreach (object t in evidence)
                this.evidence.Add(t);
        }
    }
}
