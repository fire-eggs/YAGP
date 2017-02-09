// Top-level note record "0 @R1@ NOTE"

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpGEDParser.Model
{
    public class NoteRecord : GEDCommon, SourceCitHold
    {
        public override string Tag { get { return "NOTE"; } }

        // Submitter text during parse
        public StringBuilder Builder { get; set; }

        // Submitter text
        public string Text { get; set; }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); }}

        public NoteRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            Builder = new StringBuilder(remain, 1024);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return Tag;
        }

    }
}
