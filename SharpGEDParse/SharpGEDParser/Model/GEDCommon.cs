using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Low-level line range reference
    public class LineSet
    {
        // TODO 20160629 line references are relative to sub-record, not file
        public int Beg { get; set; }
        public int End { get; set; }

        public int LineCount { get { return End - Beg + 1; } }
    }

    // Attributes about an unknown tag - custom or not
    public class UnkRec : LineSet
    {
        public string Tag { get; set; }

        public string Error { get; set; }
    }

    // Properties which are common across the main top-level records (INDI, FAM, OBJE, SOUR, NOTE, REPO)
    // Based on the data from https://www.genealogieonline.nl/en/GEDCOM-tags/ some properties are less
    // frequent and their instances are allocated on an as-needed basis.
    //
    public class GEDCommon
    {
        // The record's id
        public string Ident { get; set; }

        // The first line of the record in the original GEDCOM
        public int BegLine { get; set; }

        // The last line of the record in the original GEDCOM
        public int EndLine { get; set; }

        // Any RIN
        public string RIN { get; set; }

        // Any CHAN
        private ChangeRec _chan;
        public ChangeRec CHAN
        {
            get { return _chan ?? (_chan = new ChangeRec()); }
        }

        // Unknown and custom tags encountered at this level
        private List<UnkRec> _unknowns;
        public List<UnkRec> Unknowns { get { return _unknowns ?? (_unknowns = new List<UnkRec>()); } }

        // Problems, other than 'unknown'/'custom' tags at this _or_children_ level
        private List<UnkRec> _errors;
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); } }

        // Container for other ids (REFN, UID)
        // NOTE: RIN is not here because used by > 50% of records
        private IdHold _ids;
        public IdHold Ids { get { return _ids ?? (_ids = new IdHold()); } }

        // TODO consider a REFN accessor to Ids?
        // TODO consider a UID accessor to Ids?
    }

    public class ChangeRec
    {
        public DateTime? Date { get; set; }

        // TODO somehow make this on-demand
        private NoteHold _noteHold = new NoteHold();

        public List<Note> Notes { get { return _noteHold.Notes; } }

        // All other lines (typically custom/unknown)
        private List<LineSet> _other;
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }
    }

    public class IdHold
    {
        private List<StringPlus> _refns;
        public List<StringPlus> REFNs { get { return _refns ?? (_refns = new List<StringPlus>()); } }

        public string RIN { get; set; }
    }

    public class StringPlus
    {
        public string Value { get; set; }

        private LineSet _extra;
        public LineSet Extra { get { return _extra ?? (_extra = new LineSet()); } }
    }
}
