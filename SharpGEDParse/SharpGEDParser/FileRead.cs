using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Parser;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser
{
    /// <summary>
    /// Reads a GEDCOM file into memory.
    /// 
    /// Usage:
    /// 1. Construct a #FileRead instance
    /// 2. #ReadGed loads a GEDCOM file, parsing GEDCOM records
    /// 3. You can then access various record types using #Data
    /// 
    /// Example:
    /// (here, path is assumed to be a filename)
    /// \code{.cs}
    /// using (var reader = new FileRead())
    /// {
    /// reader.ReadGed(path);
    /// Console.WriteLine("{0} individuals", reader.AllIndividuals.Count);
    /// }
    /// \endcode
    /// </summary>
    public class FileRead : IDisposable
    {
        private int _lineNum;
        private int _emptyLineSeen; // 20170715 only one 'empty line' message per file // TODO show instance count

        // TODO: expose as public?
        private enum GedcomCharset
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

        // TODO: expose as public?
        private GedcomCharset Charset { get; set; }

        // TODO: expose as public?
        private Encoding FileEnc { get; set; }

        private string FilePath { get; set; }

        private GedParser Parser { get; set; }

        /// <summary>
        /// The number of lines loaded from the GEDCOM file.
        /// </summary>
        public int NumberLines { get { return _lineNum; } }

        /// <summary>
        /// All the GEDCOM records contained in the file.
        /// 
        /// To differentiate between kinds of GEDCOM records, a given record must be
        /// cast to determine if it is an INDI, FAM, NOTE, OBJE, SOUR, REPO, etc.
        /// 
        /// The other accessors such as #AllIndividuals, #AllFamilies might be more
        /// convenient.
        /// 
        /// Example:
        /// (reader is assumed to be an instance of #FileRead)
        /// \code{.cs}
        /// foreach (var record in reader.Data)
        /// if (record is IndiRecord)
        /// Console.Writeline("Person{0}:", ((IndiRecord)record).FullName);
        /// \endcode
        /// </summary>
        public List<GEDCommon> Data { get; set; }

        /// <summary>
        /// All 'top-level' (file level) errors encountered during the parse.
        /// </summary>
        public List<UnkRec> Errors { get; set; }

        private GedRecord _currRec;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bufferSize">Optional size of the read buffer. If not 
        /// specified, the default is 32K. If specified, the value probably 
        /// should not be less than 4K.</param>
        public FileRead(int bufferSize=0)
        {
            _bufferSize = bufferSize;
        }

        // Common method for unit testing (instream != null) or file reading (gedPath != null)
        /// <summary>
        /// Read GEDCOM data into memory.
        /// </summary>
        /// <param name="gedPath">The path to the file. If null, will attempt to read from instream instead.</param>
        /// <param name="instream">An input stream to read from instead of a file path.</param>
        public void ReadGed(string gedPath, StreamReader instream=null)
        {
            _emptyLineSeen = 0;
            FilePath = gedPath;
            Errors = new List<UnkRec>();
            GedReader _reader = new GedReader();
            _reader.BufferSize = _bufferSize;
            _reader.ProcessALine = ProcessLine;
            _reader.ErrorTracker = DoError;

            // Processor context
            Parser = new GedParser(FilePath);
            Data = new List<GEDCommon>();
            if (Errors == null)
                Errors = new List<UnkRec>();
            _currRec = new GedRecord();

            try
            {
                if (gedPath == null)
                    _reader.ReadFile(instream);
                else
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

        #region Old Character Encoding code - re-use?
#if false
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
#endif
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
                if (gedCommon.AnyErrors)
                {
                    foreach (var err in gedCommon.Errors)
                    {
                        if (err.Error == UnkRec.ErrorCode.NonStdAlias)
                            nonStdAliasSeen = true;
                        else
                            _allErrors.Add(err);
                    }
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
        /// <summary>
        /// All errors encountered during the parse.
        /// </summary>
        public List<UnkRec> AllErrors { get { return _allErrors; } }
        /// <summary>
        /// All 'unknown' records - custom, or non-standard - encountered
        /// during the parse.
        /// </summary>
        public List<UnkRec> AllUnknowns { get { return _allUnknowns; } }

        private List<IndiRecord> _indis;
        private List<FamRecord> _fams;

        // TODO this needs to go into an API class?
        /// <summary>
        /// All INDI records encountered during the parse.
        /// </summary>
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
        /// <summary>
        /// All FAM records encountered during the parse.
        /// </summary>
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
        /// <summary>
        /// All SOUR records encountered during the parse.
        /// </summary>
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

        // TODO FindById() method?
        private Dictionary<string, GEDCommon> _allRecsDict;
        private readonly int _bufferSize;

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

#if false
        // TODO this needs to go into an API class?
        public NoteRecord GetNote(string id)
        {
            GEDCommon rec;
            if (!_allRecsDict.TryGetValue(id, out rec))
                return null;
            return rec as NoteRecord;
        }
#endif

        // TODO this needs to go into an API class
        // TODO this just looks wrong
        public IndiRecord GetDad(FamRecord fam)
        {
            if (fam.Dads.Count < 1)
                return null;
            return _allRecsDict[fam.Dads[0]] as IndiRecord;
        }
        public IndiRecord GetMom(FamRecord fam)
        {
            if (fam.Moms.Count < 1)
                return null;
            return _allRecsDict[fam.Moms[0]] as IndiRecord;
        }
    }
}
