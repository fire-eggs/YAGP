
namespace SharpGEDParser.Model
{
    public class Unknown : GEDCommon
    {
        public Unknown(GedRecord lines, string ident, string tag) : base(lines, ident)
        {
            _tag = tag;
        }
        public Unknown(GedRecord lines, string ident, char [] tag)
            : base(lines, ident)
        {
            _tag = new string(tag);
        }

        private string _tag;
        public override string Tag { get { return _tag; } }
    }
}
