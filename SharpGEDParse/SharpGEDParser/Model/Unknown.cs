
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
}
