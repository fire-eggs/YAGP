
using System.Collections.Generic;

namespace DrawTreeTest
{
    public class SampleDataModel
    {
        public SampleDataModel()
        {
            Parents = new List<string>();
            CurrentMarriage = -1;
            CurrentParents = -1;
            IsDup = false;
        }

        public bool IsDup { get; set; } // A person with this ID exists more than once in the tree

        public string Id { get; set; }

        public string SpouseId { get; set; }

        public string ParentId { get; set; }

        public string Name { get; set; }

        public string SpouseName { get; set; }

        public bool HasSpouse { get; set; } // spouse (double-node) involved

        public int CurrentMarriage { get; set; } // more than one marriage possible if not -1

        public List<string> Parents { get; private set; }

        public int CurrentParents { get; set; } // More than one parent set if not -1

        // just for testing
        public override string ToString()
        {
            if (HasSpouse)
                if (Name == null)
                    return Id + "+" + SpouseId;
                else
                    return Name + "+" + SpouseName;
            return Name ?? Id;
        }
    }
}
