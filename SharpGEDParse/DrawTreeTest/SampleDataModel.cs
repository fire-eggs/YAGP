
namespace DrawTreeTest
{
    public class SampleDataModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }

        // just for testing
        public override string ToString()
        {
            return Name ?? Id;
        }
    }
}
