
using System.Collections.Generic;

namespace DrawTreeTest
{
    public class SampleDataModel
    {
        public SampleDataModel()
        {
            Parents = new List<string>();
            CurrentMarriage = -1;
        }

        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        public int CurrentMarriage { get; set; }

        public List<string> Parents { get; private set; }

        // just for testing
        public override string ToString()
        {
            return Name ?? Id;
        }
    }
}
