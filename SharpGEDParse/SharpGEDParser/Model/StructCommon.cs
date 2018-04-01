using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Common data from GEDCOM "substructures". 
    /// </summary>
    public class StructCommon
    {
        private List<LineSet> _other;
        /// <summary>
        /// Any other lines encountered in the record.
        /// 
        /// These are typically custom tag lines, as emitted by various genealogy programs.
        /// It may also include lines which fail to be parsed for some reason.
        /// </summary>
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }
    }
}
