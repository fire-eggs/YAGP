using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable InconsistentNaming

// TODO warn if trailing characters (including space!) after "0 HEAD"
// TODO a HEAD.CHAR value of UNICODE w/o a matching BOM: fatal error

namespace SharpGEDParser
{
    public class GedReader
    {
        private int _lineNum;
        private List<int> _spuriousLoc;
        private char[] _buffer;
        private StreamReader _fs;
        private const int BUFSIZE = 4096; // TODO buffer size property
        private int _blockLen;
        private int _blockPos;
        private bool _sawEOF;

        // Default file encoding
        private readonly Encoding _enc = System.Text.Encoding.GetEncoding(28591);

        // 'String' constants
        private static readonly char[] HEAD0 = { '0', ' ', 'H', 'E', 'A', 'D' };
        private static readonly char[] CHAR1 = { '1', ' ', 'C', 'H', 'A', 'R', ' ' };
        private static readonly char[] ASCII = { 'A', 'S', 'C', 'I', 'I' };
        private static readonly char[] ANSEL = { 'A', 'N', 'S', 'E', 'L' };
        private static readonly char[] UTF8  = { 'U', 'T', 'F', '-', '8' };
        private static readonly char[] UNICODE = { 'U', 'N', 'I', 'C', 'O', 'D', 'E' };

        // Line termination constants
        private const char LF = '\n';
        private const char CR = '\r';

        public enum LB
        {
            ERR = -1,
            DOS,  // CRLF
            UNIX, // LF
            MAC,  // CR
        };


        public delegate bool Processor(char[] lineToProcess, int lineNumber);

        public delegate void ErrorTrack(string error, int lineNum);

        public Processor ProcessALine { get; set; }
        public ErrorTrack ErrorTracker { get; set; }

        public void ReadFile(string path)
        {
            try
            {
                // State to be preserved across file re-open
                // TODO errors in opening stages might be duplicated
                Errors = new List<string>();
                BomEncoding = "";
                Encoding = "";

                if (!StartFile(path))
                    return;
                Encoding gedEnc;
                if (FindHeadEncoding(out gedEnc))
                {
                    _fs.Dispose();
                    if (!StartFile(path, gedEnc))
                        return;
                }

                // TODO line numbers 'off' due to initial garbage?
                ProcessALine(HEAD0, 1);
                //Lines.Add("0 HEAD");
                _lineNum = 2; // "first" line of "0 HEAD" has been read
                ReadLines();
            }
            finally
            {
                _buffer = null;
                if (_fs != null)
                    _fs.Dispose();
                _fs = null;
            }
        }

        /*
         * Logic for "starting" a file.
         * This will be called twice if the file does NOT have a BOM and
         * the encoding as specified by the HEAD.CHAR record is UTF8/Unicode.
         */
        private bool StartFile(string path, Encoding enc = null)
        {
            _lineNum = 0;
            _spuriousLoc = new List<int>();
            _buffer = new char[BUFSIZE];
            _blockLen = 0;
            _blockPos = 0;
            _lineBreaks = LB.ERR;
            //Lines = new List<string>();
            _sawEOF = false;

            if (!OpenFile(path, enc))
                return false;
            if (!FindHead())
                return false;
            if (!DetermineLineEnding())
                return false;
            return true;
        }

        private bool OpenFile(string path, Encoding enc)
        {
            bool startwdefault = false;
            if (enc == null)
            {
                enc = _enc;
                startwdefault = true;
            }

            // Try to open the file
            _fs = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), enc, true, BUFSIZE);
            _blockLen = _fs.ReadBlock(_buffer, 0, BUFSIZE);
            if (startwdefault)
            {
                BomEncoding = _fs.CurrentEncoding.ToString();
                if (BomEncoding.Contains("ASCII") || BomEncoding.Contains("Latin1"))
                    BomEncoding = "None";
                if (BomEncoding.Contains("UTF8"))
                    BomEncoding = "UTF8";
                if (BomEncoding.Contains("Unicode"))
                    BomEncoding = "Unicode"; // TODO LE vs BE
            }

            if (_blockLen < 20)
            {
                Errors.Add("Empty File");
                return false;
            }
            return true;
        }

        private bool FindHead()
        {
            // Are the characters "0 HEAD" within the first buffer? [scan past
            // any initial garbage, occurs with certain gen. programs]

            // NOTE this will do the 'wrong' thing if the initial garbage includes the text '0 HEAD'
            bool result = Locate(HEAD0);
            if (!result)
                Errors.Add("Cannot find initial '0 HEAD'");
            return result;

            // TODO error for leading garbage?
        }

        /// <summary>
        /// Find the first occurance of the search 'string'.
        /// </summary>
        /// <param name="needle">'string' to search for</param>
        /// <param name="update">true if _blockPos is to be modified to point at the next character after the 'string'</param>
        /// <returns></returns>
        private bool Locate(char[] needle, bool update = true)
        {
            for (int i = _blockPos; i < _blockLen; i++)
            {
                if (IsMatch(i, needle))
                {
                    if (update)
                        _blockPos = i + needle.Length;
                    return true;
                }
            }
            return false;
        }

        private bool IsMatch(int dex, char[] needle)
        {
            if (needle.Length > BUFSIZE - dex)
                return false; // Too close to end of buffer
            for (int i = 0; i < needle.Length; i++)
                if (_buffer[dex + i] != needle[i])
                    return false;
            return true;
        }

        private bool DetermineLineEnding()
        {
            // This assumes that _blockPos has been set to point after the initial '0 HEAD'.
            int dex = _blockPos;
            while (dex < _blockLen)
            {
                if (_buffer[dex] == CR)
                {
                    if (dex + 1 >= _blockLen)
                        return false; // TODO file too short?
                    if (_buffer[dex + 1] != LF)
                    {
                        _lineBreaks = LB.MAC;
                        Errors.Add("Carriage return linebreaks not supported.");
                        return false;
                    }
                    _lineBreaks = (_buffer[dex + 1] == LF) ? LB.DOS : LB.MAC;
                }
                if (_buffer[dex] == LF)
                {
                    _lineBreaks = (_buffer[dex - 1] == CR) ? LB.DOS : LB.UNIX;
                    _blockPos = dex+1;
                    return true;
                }
                dex++;
            }

            Errors.Add("Supported linebreaks don't exist!"); // CR broken file
            return false;
        }

        private bool FindHeadEncoding(out Encoding gedEnc)
        {
            gedEnc = null;

            // TODO a cleaner implementation
            // Locating '1 CHAR' in the header moves the block position.
            // This _must_ happen so the subsequent character set matching
            // works. If a file re-open isn't necessary, the block position
            // is now scrod: restore it in that case.
            int saveBlockPos = _blockPos;

            // The GEDCOM header includes a character encoding
            bool result = Locate(CHAR1);
            if (!result)
            {
                _blockPos = saveBlockPos;
                Errors.Add("No character set specified.");
                return false;
            }

            if (Locate(ANSEL, false))
            {
                Encoding = "ANSEL";
                Errors.Add("ANSEL not supported. Using ASCII.");
                gedEnc = _enc;
                if (BomEncoding != "None")
                {
                    Errors.Add("BOM doesn't match specified character set");
                    return true; // only necessary if BOM exists
                }
            }
            else if (Locate(ASCII, false))
            {
                Encoding = "ASCII";
                gedEnc = _enc;
                if (BomEncoding != "None")
                {
                    Errors.Add("BOM doesn't match specified character set");
                    return true; // only necessary if BOM exists
                }
            }
            else if (Locate(UTF8, false))
            {
                Encoding = "UTF8";
                gedEnc = System.Text.Encoding.UTF8;
                if (BomEncoding != "UTF8")
                {
                    Errors.Add("BOM doesn't match specified character set");
                    return true; // only necessary if BOM is not UTF8
                }
            }
            else if (Locate(UNICODE, false))
            {
                Encoding = "UNICODE";
                if (BomEncoding != "Unicode")
                {
                    Errors.Add("Marked as UNICODE but missing BOM: handled as ASCII");
                    return false;
                }
                gedEnc = System.Text.Encoding.Unicode;
                var macintosh = false;
                if (macintosh)
                    gedEnc = System.Text.Encoding.BigEndianUnicode;
                // TODO error if BOM doesn't match
                // TODO Unicode vs BigEndianUnicode
                return true; // TODO only necessary if BOM is not Unicode
            }
            else
            {
                Errors.Add("Non-standard character set specified. Using ASCII.");
                Encoding = "ASCII";
                gedEnc = _enc;
                if (BomEncoding != "None")
                {
                    Errors.Add("BOM doesn't match specified character set");
                    return true; // only necessary if BOM exists
                }
            }

            _blockPos = saveBlockPos;
            return false;
        }

        public int LineCount { get { return _lineNum; } }
        public string Encoding { get; set; }
        public List<int> Spurious { get { return _spuriousLoc; } }
        public string BomEncoding { get; set; }
        public List<string> Errors { get; set; }

        private LB _lineBreaks;
        public string LineBreaks { get { return _lineBreaks.ToString(); } }

        private void ReadLines()
        {
            char [] line = ReadLine();
            while (line != null)
            {
                if (!ProcessALine(line, _lineNum))
                    break;
                _lineNum++;
                line = ReadLine();
            }
        }

        private char [] ReadLine()
        {
            // Read a line from the input buffer. A line is determined by the found
            // linebreak. Strip (and count) spurious linebreaks. If the buffer is
            // emptied, refill.
            if (_sawEOF)
                return null;

            while (true)
            {
                int dex = _blockPos;
                while (dex < _blockLen)
                {
                    if (_buffer[dex] == LF)
                    {
                        // A linefeed is spurious if no CR and DOS
                        bool iscrlf = dex > 0 && _buffer[dex - 1] == CR;
                        if (iscrlf || _lineBreaks == LB.UNIX)
                            return FetchLine(_blockPos, dex+1);
                    }
                    dex ++;
                }

                // we ran out of buffer before we hit end-of-line
                int partlen = _blockLen - _blockPos;
                for (int i = 0; i < partlen; i++)
                    _buffer[i] = _buffer[i + _blockPos];
                int readcount = _fs.ReadBlock(_buffer, partlen, BUFSIZE - partlen);
                _blockLen = readcount + partlen;
                _blockPos = 0;

                if (readcount == 0) // EOF
                {
                    _sawEOF = true;
                    if (partlen > 0)
                        return FetchLine(0, partlen);
                    return null;
                }
            }

            return null; 
        }

        private char[] FetchLine(int start, int end)
        {
            _blockPos = end; // point past end of line first
            if (_buffer[end-1] == '\n')
                end -= _lineBreaks == LB.DOS ? 2 : 1; // ignore terminators

            char [] line = new char[end-start];
            int dex = 0;
            for (int i = start; i < end; i++)
            {
                char val = _buffer[i];
                switch (val)
                {
                    case CR:
                    case LF:
                        // don't copy a spurious terminator
                        _spuriousLoc.Add(_lineNum); // track a spurious terminator
                        break;
                    default:
                        line[dex++] = val;
                        break;
                }
            }
            return line; // NOTE length does NOT take into account spurious chars removed
        }

        //public List<string> Lines { get; set; }

        //private bool ProcessLine(char[] line, int linenum)
        //{
        //    var val = new string(line).Trim('\0'); // trim to deal w/removed terminators
        //    Lines.Add(val);
        //    if (val == "0 TRLR")
        //        return false;
        //    return true;
        //}

        // TODO: error? - trailing stuff after "0 TRLR"
    }
}

// TODO test w/ actual unicode-16 chars
// TODO unicode-16 BOM variants in testing
// TODO buffer boundary exactly on LF (no partial)

// TODO see Choquet.GED: HEAD.CHAR says 'ANSI' but BOM says UTF8. Should I use UTF8???

/*
 * Limitations:
 * 1) won't support CR line terminator
 * 2) Line numbers will be 'wrong' when there is garbage preceding
 *    the "0 HEAD" line (i.e. "0 HEAD" is always line 1)
 * 3) IBMPC, MSDOS, ANSI, MACINTOSH or similar CHARACTER_SET values
 *    will be treated as Latin1, not their code-page equivalents.
 * 4) ANSEL???
*/