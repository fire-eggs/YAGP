using System;
using System.Collections.Generic;
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

        public void ReadLines(StreamReader _stream)
        {
            Parser = new KBRGedParser(FilePath ?? "");
            Data = new List<KBRGedRec>();

            _currRec = new GedRecord();
            _lineNum = 1;
            string line = _stream.ReadLine();
            while (line != null)
            {
                ProcessRecord(line, _lineNum);

                line = _stream.ReadLine();
                _lineNum++;
            }
        }

        private void ReadLines()
        {
            using (_stream = new StreamReader(FilePath, FileEnc))
            {
                ReadLines(_stream);
            }
        }

        private GedRecord _currRec;

        private void ProcessRecord(string line, int lineNum)
        {
            int dex = KBRGedUtil.FirstChar(line);
            if (dex < 0)
                return; // empty line

            if (line[dex] == '0')
            {
                if (_currRec.LineCount > 1) // TODO be smarter?
                {
                    // start of a new record
//                    Console.WriteLine(_currRec);

                    // TODO records should go into a 'to parse' list and asynchronously turned into head/indi/fam/etc
                    var parsed = Parser.Parse(_currRec);
//                    Console.WriteLine(">>>" + parsed);

                    Data.Add(parsed);
                }
                _currRec = new GedRecord(lineNum, line);
            }
            else
            {
                _currRec.AddLine(line);
            }
        }

        // TODO needs to be intelligent - header/people/families/relations/etc
        public List<KBRGedRec> Data { get; set; }
    }
}
