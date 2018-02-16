using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Model
{
    // Low-level line range reference [20180106: line #s now relative to file]
    /// <summary>
    /// A line range container. 
    /// </summary>
    /// Line numbers are relative to the file.
    public class LineSet
    {
        /// <summary>
        /// Beginning line number of the set.
        /// </summary>
        public int Beg { get; set; }
        /// <summary>
        /// Ending line number of the set.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Number of lines in the set.
        /// </summary>
        public int LineCount { get { return End - Beg + 1; } }
    }

    /// <summary>
    /// An unknown tag or error.
    /// </summary>
    /// If this represents an unknown tag, the ErrorCode will be zero.
    /// 
    /// If this represents an error, the ErrorCode will have a non-zero value.
    /// Some errors may not have a line range, being a global or breaking problem.
    public class UnkRec : LineSet
    {
        /// <summary>
        /// Possible error codes.
        /// </summary>
        public enum ErrorCode
        {
            Exception = 1,
            UntermIdent,
            MissIdent,
            MultChan, // More than one change record: first one preserved
            InvXref, // 5
            MissName,
            ChanDate, // Missing/invalid date for CHAN
            InvNCHI,
            MultNCHI,
            MultId,  // 10
            MissFile,
            MissForm,
            RefSourText,
            EmbSourEven,
            MultCHIL, // 15
            MissHEAD,
            EmbSourPage,
            MultHUSB,
            MultWIFE,
            MultRESN, // 20
            InvRestrict,
            InvExtra, // Non-standard extra text
            InvSex,
            EmptyLine,
            LineTooLong, // 25
            InvLevel,
            EmptyFile, // input file is empty
            MissTag, // missing tag
            IdentCollide, // more than one record with same ident
            UnsuppLB, // 30 // Unsupported line breaks (CR)
            MissCharSet, // No character set specified in header
            BOMMismatch, // BOM doesn't match specified charset
            ForceASCII, // unsupported or other problem charset; forced to ASCII
            NonStdAlias, // Non-standard ALIA tag
            CustTagsSeen, // 35 // one or more valid custom tags seen
            EmptyName, // NAME line with no value
            SlashInName, // Surname containing slash(es)
            UntermSurname, // Surname is not terminated with slash
        }

        internal UnkRec()
        {
            Beg = End = -1;
        }

        internal UnkRec(string tag, int beg, int end)
        {
            Tag = tag;
            Beg = beg;
            End = end;
        }

        /// <summary>
        /// The tag associated with the record.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Any error code associated with the record.
        /// </summary>
        public ErrorCode Error { get; set; }
    }

    // Based on the data from https://www.genealogieonline.nl/en/GEDCOM-tags/ some properties are less
    // frequent and their instances are allocated on an as-needed basis.

    /// <summary>
    /// Properties which are common across the main top-level GEDCOM records.
    ///  
    /// I.e. INDI, FAM, OBJE, SOUR, NOTE, REPO records.
    /// </summary>
    public abstract class GEDCommon
    {
        /// The record's GEDCOM tag.
        public abstract string Tag { get; }

        /// The record's id
        public string Ident { get; set; }

        /// The first line of the record in the original GEDCOM
        public int BegLine { get; set; }

        /// The last line of the record in the original GEDCOM
        public int EndLine { get; set; }

        /// Any RIN
        public string RIN { get; set; }

        private ChangeRec _chan;
        /// Any change record (CHAN).
        public ChangeRec CHAN
        {
            get { return _chan ?? (_chan = new ChangeRec()); }
        }

        private List<UnkRec> _unknowns;
        /// Unknown and custom tags encountered at this level
        public List<UnkRec> Unknowns { get { return _unknowns ?? (_unknowns = new List<UnkRec>()); } }

        private List<UnkRec> _errors;
        /// Problems, other than 'unknown'/'custom' tags at this _or_children_ level
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); } }

        /// Are there any errors associated with this record? Returns true if yes.
        public bool AnyErrors { get { return _errors != null && _errors.Count > 0; } }

        // The IdHold implementation proved to be memory expensive when used with files
        // containing large numbers of UIDs, AFNs. NOTE: GK uses a <tag,value> sort of
        // scheme for these and other tags, consider revisiting.

        internal byte[] _uid;

        /// An UID (universal identifier) associated with the record.
        public string UID
        {
            get
            {
                if (_uid == null) return "";
                return System.Text.Encoding.ASCII.GetString(_uid);
            }
        }
        /// An AFN (ancestral file number) associated with the record.
        public StringPlus AFN { get; set; }
        /// An RFN (something file number) associated with the record.
        public StringPlus RFN { get; set; }

        private List<StringPlus> _refns;
        /// Any user reference numbers associated with the record. Will be null if none.
        public List<StringPlus> REFNs { get { return _refns ?? (_refns = new List<StringPlus>()); } }
        /// Returns true if there are any REFNs associated to the record.
        public bool AnyREFNs { get { return _refns != null; } }

        // TODO revisit this, esp. not using StringPlus if not required for UID/AFN/RFN/REFN
        //// Container for other ids (REFN, UID, AFN, RFN)
        //// NOTE: RIN is not here because used by > 50% of records
        //private IdHold _ids;
        //public IdHold Ids { get { return _ids ?? (_ids = new IdHold()); } }

        internal GEDCommon(GedRecord lines, string ident)
        {
            Ident = ident;
            if (lines == null) // DrawAnce creating INDI on-the-fly
                return;
            BegLine = lines.Beg;
            EndLine = lines.End;
        }
    }

    /// <summary>
    /// Represents a 'Change record' - a GEDCOM CHAN tag.
    /// </summary>
    /// NOTE: any TIME data is preserved in OtherLines.
    public class ChangeRec : StructCommon, NoteHold
    {
        /// <summary>
        /// The date for the record change.
        /// </summary>
        /// This is a 'standard' calendar date, not a GEDCOM date.
        public DateTime? Date { get; set; }

        private List<Note> _notes;

        /// <summary>
        /// Any notes associated with the record change.
        /// </summary>
        /// Will be an empty list if there are none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }

    public class StringPlus
    {
        public string Value { get; set; }

        private LineSet _extra;
        public LineSet Extra { get { return _extra ?? (_extra = new LineSet()); } }
    }
}
