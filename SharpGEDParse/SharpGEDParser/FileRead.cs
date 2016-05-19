using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SharpGEDParser
{
    public class FileRead
    {
        private StreamReader _stream;
        private int _lineNum;

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
            UnSupported
        };

        private GedcomCharset Charset { get; set; }

        private Encoding FileEnc { get; set; }

        private string FilePath { get; set; }

        private KBRGedParser Parser { get; set; }

        // TODO needs to be intelligent - header/people/families/relations/etc
        public List<KBRGedRec> Data { get; set; }

        private GedRecord _currRec;

        public void ReadGed(string gedPath)
        {
            FilePath = gedPath;
            GetEncoding(gedPath);
            ReadLines();
        }

        private void GetEncoding(string gedPath)
        {
            // TODO exception handling (e.g. file might not exist)

            FileEnc = Encoding.Default;
            Charset = GedcomCharset.Unknown;

            using (FileStream fileStream = File.OpenRead(gedPath))
            {
                byte[] bom = new byte[4];

                fileStream.Read(bom, 0, 4);

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
            Parser = new KBRGedParser(FilePath ?? "");
            Data = new List<KBRGedRec>();

            _currRec = new GedRecord();
            _lineNum = 1;
            string line = instream.ReadLine(); // TODO CR, LF, CR/LF, and LF/CR must be validated?
            while (line != null)
            {
                ProcessLine(line, _lineNum);

                line = instream.ReadLine();
                _lineNum++;
            }

            // TODO a mal-formed file might be missing the end 0 TRLR line. If so, _currRec contains a record which as not been processed?
        }

        private void ReadLines()
        {
            // TODO what happens if file doesn't exist?
            using (_stream = new StreamReader(FilePath, FileEnc))
            {
                ReadLines(_stream);
            }
        }

        /// <summary>
        /// Deal with a single line. It either starts with '0' and is to be a new record,
        /// or is accumulated into a record.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNum"></param>
        private void ProcessLine(string line, int lineNum)
        {
            int dex = GedLineUtil.FirstChar(line);
            if (dex < 0)
            {
                DoError("Empty line", lineNum);
                return; // empty line
            }
            if (line.Length > 255) // TODO anything special for UTF-16?
            {
                DoError("Line too long", lineNum);
                // proceed anyway
            }

            // NOTE: do NOT warn about leading spaces "GEDCOM readers should ignore it when it occurs".

            if (line[dex] == '0')
            {
                if (_currRec.LineCount > 1) // TODO should warn about a mal-formed record [single '0' line]
                {
                    // start of a new record. deal with the previous record first

                    // TODO records should go into a 'to parse' list and asynchronously turned into head/indi/fam/etc
                    var parsed = Parser.Parse(_currRec);
                    Data.Add(parsed);
                }
                _currRec = new GedRecord(lineNum, line);
            }
            else
            {
                _currRec.AddLine(line);
            }
        }

        private void DoError(string msg, int lineNum)
        {
            var rec = new KBRGedUnk(null,"","");
            var err = new UnkRec("");
            err.Error = msg;
            err.Beg = lineNum;
            err.End = lineNum;
            Data.Add(rec);
        }
    }
}
