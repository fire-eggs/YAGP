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

        private List<UnkRec> _errors;
        /// <summary>
        /// Problems encountered at this level.
        /// 
        /// These are typically 'syntactic' errors, i.e. the parser was unable to process
        /// these lines.
        /// 
        /// <b>TBD</b> at this level or also from children?
        /// </summary>
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); } }
    }
}
