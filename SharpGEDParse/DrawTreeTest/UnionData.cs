using System;
using System.Collections.Generic;
using GEDWrap;

namespace DrawTreeTest
{
    // A union-driven dataset
    public class UnionData
    {
        public UnionData()
        {
            CurrentParents = -1;
            CurrentMarriage = -1;
            Link = -1;
            Parents = new List<string>();
        }

        public UnionData ShallowCopy()
        {
            return (UnionData) MemberwiseClone();
        }

        // This node's id
        public int Id { get; set; }

        // The parent node id.
        public int ParentId { get; set; }

        // The UNION id for this node. null if a person.
        public string UnionId { get; set; }

        // The INDI id - the 'primary' person for this node.
        // Drives alternate marriages / adoption
        public string PersonId { get; set; }

        public Person Who { get; set; }
        public Person Spouse { get; set; }

        // The INDI id for the spouse, iff a union
        public string SpouseId { get; set; }

        // In the case of a duplicate, children of this node
        // have been removed and a link should be drawn to
        // the other node.
        public int Link { get; set; }

        public bool IsUnion { get; set; }

        public int CurrentMarriage { get; set; }

        public List<string> Parents { get; private set; }

        public int CurrentParents { get; set; } // More than one parent set if not -1

        public override string ToString()
        {
            if (!IsUnion)
                return PersonId + Environment.NewLine + Who.Name;
            string line1 = string.Format("{0}:{1}+{2}", UnionId, PersonId, SpouseId);
            string line2 = Who != null ? Who.Name : "";
            string line3 = Spouse != null ? Spouse.Name : "";
            return line1 + Environment.NewLine + line2 + Environment.NewLine + line3;
        }
    }
}
