using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Parser;

namespace SharpGEDParser
{
    public class FileRead : IDisposable
    {
        private int _lineNum;

        // ReSharper disable InconsistentNaming
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
        // ReSharper restore InconsistentNaming

        private GedcomCharset Charset { get; set; }

        private Encoding FileEnc { get; set; }

        private string FilePath { get; set; }

        private GedParser Parser { get; set; }

        public List<GEDCommon> Data { get; set; }

        // Top-level (file level) errors, such as blank lines
        public List<UnkRec> Errors { get; set; }

        private GedRecord _currRec;

        public void ReadGed(string gedPath)
        {
            _lineNum = 0;
            Errors = new List<UnkRec>();

            try
            {
                FilePath = gedPath;
                GetEncoding(gedPath);
                if (Charset == GedcomCharset.EmptyFile)
                {
                    UnkRec err = new UnkRec();
                    err.Error = UnkRec.ErrorCode.EmptyFile;
                    // TODO err.Error = string.Format("Empty file");
                    Errors.Add(err);
                    return;
                }
                using (StreamReader stream = new StreamReader(FilePath, FileEnc))
                {
                    ReadLines(stream);
                }
            }
            catch (Exception ex)
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.Exception;
                // TODO err.Error = string.Format("Exception: {0} line {1} | {2}", ex.Message, _lineNum, ex.StackTrace);
                Errors.Add(err);
            }
        }

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

        /// <summary>
        /// Read GED lines from a stream. This is split from file processing so unit testing can 
        /// be done using string streams.
        /// </summary>
        /// <param name="instream"></param>
        public void ReadLines(StreamReader instream)
        {
            Parser = new GedParser(FilePath ?? "");
            Data = new List<GEDCommon>();
            if (Errors == null)
                Errors = new List<UnkRec>();

            try
            {
                _currRec = new GedRecord();
                _lineNum = 1;
                string line = instream.ReadLine(); // TODO CR, LF, CR/LF, and LF/CR must be validated?
                while (line != null)
                {
                    ProcessLine(line, _lineNum);

                    line = instream.ReadLine();
                    _lineNum++;
                }
                EndOfFile();
            }
            catch (Exception ex)
            {
                if (ex.Message == "record head not zero")
                {
                    UnkRec unk = new UnkRec();
                    unk.Error = UnkRec.ErrorCode.MissHEAD;
                    Errors.Add(unk);
                    //Errors.Add(new UnkRec { Error = "File doesn't start with '0 HEAD', parse fails" });
                }
                else
                {
                    var err = new UnkRec();
                    err.Error = UnkRec.ErrorCode.Exception;
                    // err.Error = string.Format("Exception: {0} line {1} | {2}", ex.Message, _lineNum, ex.StackTrace);
                    Errors.Add(err);
                }
            }

            Parser.FinishUp();
            GatherRecords();
            GatherErrors();
            _currRec = null;
        }

        /// <summary>
        /// Deal with a single line. It either starts with '0' and is to be a new record,
        /// or is accumulated into a record.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNum"></param>
        private void ProcessLine(string line, int lineNum)
        {
            int len = line.Length;
            int dex = LineUtil.FirstChar(line, 0, len);
            if (dex < 0)
            {
                DoError( UnkRec.ErrorCode.EmptyLine, lineNum);
                return; // empty line
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
                return; // cannot proceed
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
                Data.Add(parsed);
                //if (Data.Count % 10000 == 0) // TODO force garbage collection every few records: major performance hit
                //    GC.Collect();
                _currRec = new GedRecord(lineNum, line);
            }
            else
            {
                _currRec.AddLine(line);
            }
        }

        private void EndOfFile()
        {
            // A mal-formed file might be missing the end 0 TRLR line. If so, _currRec might contain a record which has not been processed
            if (_currRec.LineCount < 1)
                return;
            var parsed = Parser.Parse(_currRec);
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
            foreach (var gedCommon in Data)
            {
                if (gedCommon.Errors != null)
                    _allErrors.AddRange(gedCommon.Errors);
                if (gedCommon.Unknowns != null)
                    _allUnknowns.AddRange(gedCommon.Unknowns);
            }
            // TODO errors/unknown in sub-records
        }

        public List<UnkRec> AllErrors { get { return _allErrors; } }
        public List<UnkRec> AllUnknowns { get { return _allUnknowns; } }

        private List<IndiRecord> _indis;
        private List<FamRecord> _fams;

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

        private void GatherRecords()
        {
            // Create a lookup dictionary based on id.
            _allRecsDict = new Dictionary<string, GEDCommon>(Data.Count);
            foreach (var gedCommon in Data)
            {
                if (!string.IsNullOrEmpty(gedCommon.Ident))
                    _allRecsDict.Add(gedCommon.Ident, gedCommon);
            }
        }

        public NoteRecord GetNote(string id)
        {
            GEDCommon rec;
            if (!_allRecsDict.TryGetValue(id, out rec))
                return null;
            return rec as NoteRecord;
        }
    }
}
