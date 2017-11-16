
namespace SharpGEDParser.Model
{
    public class Unknown : GEDCommon
    {
        public Unknown(GedRecord lines, string ident, string tag) : base(lines, ident)
        {
            _tag = tag;
        }

        private string _tag;
        public override string Tag { get { return _tag; } }
    }

    public class DontCare : GEDCommon
    {
        public DontCare(GedRecord lines, string tag) : base(lines, "")
        {
            _tag = tag;
        }

        private string _tag;
        public override string Tag { get { return _tag; } }
    }
}
