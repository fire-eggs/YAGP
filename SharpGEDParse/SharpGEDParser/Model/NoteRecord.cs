// Top-level note record "0 @R1@ NOTE"

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using SharpGEDParser.Parser;

namespace SharpGEDParser.Model
{
    public class NoteRecord : GEDCommon, SourceCitHold
    {
        public override string Tag { get { return "NOTE"; } }

        // Submitter text during parse
        public StringBuilder Builder { get; set; }

        public string Text
        {
#if SQLITE
            get { return Key == -1 ? _text : SQLite.Instance.GetNote(Key); }
            set { Key = -1;
                _text = value;
            }
#elif LITEDB
            get { return Key == null ? _text : Parser.LiteDB.Instance.GetNote(Key); }
            set { Key = null;
                _text = value;
            }
#elif NOTESTREAM
            get { return Key == -1 ? _text : NoteStream.Instance.GetNote(Key); }
            set
            {
                Key = -1;
                _text = value;
            }

#else
            get { return _text; }
            set { _text = value; }
#endif
        }

        private string _text = null;

#if LITEDB
        public BsonValue Key { get; set; }
#else
        public int Key { get; set; }
#endif

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
