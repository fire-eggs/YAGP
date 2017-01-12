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
            DUPL_FAM
        };

        // TODO warn/error prefix is temporary for unit testing
        private readonly string[] messages =
        {
            "Error: Duplicate INDI ident {0}",
            "Error: Missing FAM id at/near line {0}",
            "Error: Duplicate family '{0}'"
        };

        public Issue(IssueCode code, params object[] evidence)
        {
            IssueId = code;
            foreach (object t in evidence)
                this.evidence.Add(t);
        }
    }
}
