using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class StructCommon
    {
        // All other lines (typically custom/unknown)
        private List<LineSet> _other;
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }
    }
}
