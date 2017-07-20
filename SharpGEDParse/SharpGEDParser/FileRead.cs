using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Parser;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser
{
    public class FileRead : IDisposable
    {
        private int _lineNum;
        private int _emptyLineSeen; // 20170715 only one 'empty line' message per file // TODO show instance count

        public enum GedcomCharset
        {
            Unknown,
            Ansel,
            Ansi,
            Ascii,
            UTF8,
            UTF16BE,
            UTF16LE,
            UTF32BE,
            UTF32LE,
            UnSupported,
            EmptyFile
        };

        private GedcomCharset Charset { get; set; }

        private Encoding FileEnc { get; set; }

        private string FilePath { get; set; }

        private GedParser Parser { get; set; }

        public int NumberLines { get { return _lineNum; } }

        public List<GEDCommon> Data { get; set; }

        // Top-level (file level) errors, such as blank lines
        public List<UnkRec> Errors { get; set; }

        private GedRecord _currRec;

        public FileRead(int bufferSize=0)
        {
            _bufferSize = bufferSize;
        }

        public void ReadGed(string gedPath)
        {
            _emptyLineSeen = 0;
            FilePath = gedPath;
            Errors = new List<UnkRec>();
            GedReader _reader = new GedReader();
            _reader.BufferSize = _bufferSize;
            _reader.ProcessALine = ProcessLine;
            _reader.ErrorTracker = DoError;

            // Processor context
            Parser = new GedParser(FilePath ?? "");
            Data = new List<GEDCommon>();
            if (Errors == null)
                Errors = new List<UnkRec>();
            _currRec = new GedRecord();

            try
            {
                _reader.ReadFile(gedPath);
                EndOfFile();
            }
            catch (Exception)
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.Exception;
                err.Beg = _lineNum;
                // TODO err.Error = string.Format("Exception: {0} line {1} | {2}", ex.Message, _lineNum, ex.StackTrace);
                Errors.Add(err);
            }

            Parser.FinishUp();
            GatherRecords();
            GatherErrors();

            _lineNum = _reader._lineNum;
            _currRec = null;
            _reader = null;
        }

        public void ReadGed(StreamReader instream)
        {
            // read from stream for unit testing. TODO refactor common code
            Errors = new List<UnkRec>();
            GedReader _reader = new GedReader();
            _reader.BufferSize = _bufferSize;
            _reader.ProcessALine = ProcessLine;
            _reader.ErrorTracker = DoError;

            // Processor context
            Parser = new GedParser("");
            Data = new List<GEDCommon>();
            if (Errors == null)
                Errors = new List<UnkRec>();
            _currRec = new GedRecord();

            try
            {
                _reader.ReadFile(instream);
                EndOfFile();
            }
            catch (Exception)
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.Exception;
                err.Beg = _lineNum;
                // TODO err.Error = string.Format("Exception: {0} line {1} | {2}", ex.Message, _lineNum, ex.StackTrace);
                Errors.Add(err);
            }

            Parser.FinishUp();
            GatherRecords();
            GatherErrors();
            _currRec = null;
        }

        #region Old Character Encoding code - re-use?
        private void GetEncoding(string gedPath)
        {
            FileEnc = Encoding.Default;
            Charset = GedcomCharset.Unknown;

            using (FileStream fileStream = File.OpenRead(gedPath))
            {
                byte[] bom = new byte[10]; // large enough for "<BOM>0 HEAD"

                int count = fileStream.Read(bom, 0, 10);
                if (count < 10)
                {
                    // empty file, quit
                    Charset = GedcomCharset.EmptyFile;
                    return;
                }

                // look for BOMs, if found we will ignore the CHAR tag
                // don't use .net look for bom as we also want to detect
                // unicode where there isn't a BOM, as far as the parser
                // is concerned the data is utf16le if we detect this way
                // as the conversion is already done

                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.UTF8;
                }
                else if (bom[0] == 0xFE && bom[1] == 0xFF)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.BigEndianUnicode;
                }
                else if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.UTF32;
                }
                else if (bom[0] == 0xFF && bom[1] == 0xFE)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.Unicode;
                }
                else if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.UTF32;
                }
                else if (bom[0] == 0x00 && bom[2] == 0x00)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.BigEndianUnicode;
                }
                else if (bom[1] == 0x00 && bom[3] == 0x00)
                {
                    Charset = GedcomCharset.UTF16LE;
                    FileEnc = Encoding.Unicode;
                }
            }
        }
        #endregion

        /// <summary>
        /// Deal with a single line. It either starts with '0' and is to be a new record,
        /// or is accumulated into a record.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNum"></param>
        //private void ProcessLine(string line, int lineNum)
        private bool ProcessLine(char [] line, int lineNum)
        {
            int len = line.Length;
            int dex = LineUtil.FirstChar(line, 0, len);
            if (dex < 0)
            {
                if (_emptyLineSeen == 0)
                    DoError( UnkRec.ErrorCode.EmptyLine, lineNum);
                _emptyLineSeen++;
                return true; // empty line
            }
            if (len > 255) // TODO anything special for UTF-16?
            {
                DoError( UnkRec.ErrorCode.LineTooLong, lineNum);
                // proceed anyway
            }

            char level = line[dex];
            if (level < '0' || level > '9')
            {
                DoError( UnkRec.ErrorCode.InvLevel, lineNum);
                return false; // cannot proceed
            }

            // NOTE: do NOT warn about leading spaces "GEDCOM readers should ignore it when it occurs".

            if (level == '0' && _currRec.LineCount > 0)
            {
                // TODO this may not be a reasonable error: 58000.GED has a large number of single line NOTE records
                //if (_currRec.LineCount == 1)
                //    Errors.Add(new UnkRec { Error = "Empty (single line) top-level record", Beg = lineNum});

                // start of a new record. deal with the previous record first

                // TODO records should go into a 'to parse' list and asynchronously turned into head/indi/fam/etc
                var parsed = Parser.Parse(_currRec);
                if (parsed == null)
                    return false;

                Data.Add(parsed);
                //if (Data.Count % 10000 == 0) // TODO force garbage collection every few records: major performance hit
                //    GC.Collect();
                _currRec = new GedRecord(lineNum, line);
            }
            else
            {
                _currRec.AddLine(line);
            }

            return true;
        }

        private void EndOfFile()
        {
            // A mal-formed file might be missing the end 0 TRLR line. If so, _currRec might contain a record which has not been processed
            if (_currRec.LineCount < 1)
                return;
            var parsed = Parser.Parse(_currRec);
            if (parsed == null)
                return;
            Data.Add(parsed);
        }

        private void DoError(UnkRec.ErrorCode msg, int lineNum)
        {
            var err = new UnkRec();
            err.Error = msg;
            err.Beg = lineNum;
            err.End = lineNum;
            Errors.Add(err);
        }

        public void Dispose()
        {
            // TODO any explicit disposal required?
            Parser = null;
            Data = null;
            Errors = null;
            _currRec = null;
        }

        private List<UnkRec> _allErrors;
        private List<UnkRec> _allUnknowns;

        private void GatherErrors()
        {
            _allUnknowns = new List<UnkRec>();
            _allErrors = new List<UnkRec>();
            if (Errors != null)
                _allErrors.AddRange(Errors);

            bool customTagSeen = false;
            bool nonStdAliasSeen = false;
            foreach (var gedCommon in Data)
            {
                foreach (var err in gedCommon.Errors)
                {
                    if (err.Error == UnkRec.ErrorCode.NonStdAlias)
                        nonStdAliasSeen = true;
                    else
                        _allErrors.Add(err);
                }
                //if (gedCommon.Errors != null)
                //    _allErrors.AddRange(gedCommon.Errors);
                foreach (var unknown in gedCommon.Unknowns)
                {
                    if (!string.IsNullOrEmpty(unknown.Tag) && unknown.Tag.StartsWith("_"))
                        customTagSeen = true;
                    else
                        _allUnknowns.Add(unknown);
                }
                //if (gedCommon.Unknowns != null)
                //    _allUnknowns.AddRange(gedCommon.Unknowns);
            }

            // TODO errors/unknown in sub-records - gather via gedCommon

            if (customTagSeen)
                _allErrors.Add(new UnkRec() {Error = UnkRec.ErrorCode.CustTagsSeen});
            if (nonStdAliasSeen)
                _allErrors.Add(new UnkRec() { Error = UnkRec.ErrorCode.NonStdAlias });
        }

        // TODO this needs to go into an API class?
        public List<UnkRec> AllErrors { get { return _allErrors; } }
        public List<UnkRec> AllUnknowns { get { return _allUnknowns; } }

        private List<IndiRecord> _indis;
        private List<FamRecord> _fams;

        // TODO this needs to go into an API class?
        public List<IndiRecord> AllIndividuals
        {
            get
            {
                if (_indis == null)
                {
                    _indis = new List<IndiRecord>();
                    foreach (var gedCommon in Data)
                    {
                        if (gedCommon is IndiRecord)
                            _indis.Add(gedCommon as IndiRecord);
                    }
                }
                return _indis;
            }
        }

        // TODO this needs to go into an API class?
        public List<FamRecord> AllFamilies
        {
            get
            {
                if (_fams == null)
                {
                    _fams = new List<FamRecord>();
                    foreach (var gedCommon in Data)
                    {
                        if (gedCommon is FamRecord)
                            _fams.Add(gedCommon as FamRecord);
                    }
                }
                return _fams;
            }
        }

        // TODO this needs to go into an API class?
        public List<SourceRecord> AllSources
        {
            get
            {
                List<SourceRecord> srcs = new List<SourceRecord>();
                foreach (var gedCommon in Data)
                {
                    if (gedCommon is SourceRecord)
                        srcs.Add(gedCommon as SourceRecord);
                }
                return srcs;
            }
        }

        private Dictionary<string, GEDCommon> _allRecsDict;
        private int _bufferSize;

        // TODO use for looking up INDI,FAM,etc
        private void GatherRecords()
        {
            // Create a lookup dictionary based on id.
            _allRecsDict = new Dictionary<string, GEDCommon>(Data.Count);
            foreach (var gedCommon in Data)
            {
                if (!string.IsNullOrEmpty(gedCommon.Ident))
                    try
                    {
                        _allRecsDict.Add(gedCommon.Ident, gedCommon);
                    }
                    catch (ArgumentException)
                    {
                        DoError(UnkRec.ErrorCode.IdentCollide, gedCommon.BegLine);
                    }
            }
        }

        // TODO this needs to go into an API class?
        public NoteRecord GetNote(string id)
        {
            GEDCommon rec;
            if (!_allRecsDict.TryGetValue(id, out rec))
                return null;
            return rec as NoteRecord;
        }
    }
}
