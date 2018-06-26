using System.Collections.Generic;
using System.Text;
using SharpGEDParser.Parser;
#if LITEDB
using LiteDB;
using LiteDB = SharpGEDParser.Parser.LiteDB;
#endif

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a NOTE reference within a record.
    /// 
    /// The GEDCOM standards specify two variants of NOTE references: a cross-reference
    /// to a NOTE record, or an "embedded" note.
    /// 
    /// The GEDCOM 5.5 standard permits source citations for a NOTE reference.
    /// 
    /// \warning Future library releases may convert "embedded" notes to cross-reference.
    /// \warning Future library releases may move note source citations to a note record.
    /// </summary>
    public class Note : StructCommon, SourceCitHold
    {
        /// <summary>
        /// A NOTE record cross-reference.
        /// 
        /// Will be empty if this is an embedded note.
        /// </summary>
        public string Xref { get; set; }

        /// <summary>
        /// All text associated to an "embedded" note.
        /// 
        /// Will typically be empty if this is a cross-reference, barring non-standard
        /// GEDCOM files.
        /// </summary>
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
            set { Key = -1;
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

        internal StringBuilder Builder { get; set; } // Accumulate text during parse

        private List<SourceCit> _cits;

        // GEDCOM 5.5 standard allows source citations on NOTE structure
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }

    /// <summary>
    /// Represents a record which may contain NOTE references.
    /// </summary>
    public interface NoteHold
    {
        /// <summary>
        /// Any NOTE references associated with a record.
        /// </summary>
        List<Note> Notes { get; }
    }
}
