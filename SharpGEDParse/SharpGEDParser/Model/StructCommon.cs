using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class StructCommon
    {
        // All other lines (typically custom/unknown)
        private List<LineSet> _other;
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }

        // Problems, other than 'unknown'/'custom' tags at this _or_children_ level
        private List<UnkRec> _errors;
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); } }
    }
}
