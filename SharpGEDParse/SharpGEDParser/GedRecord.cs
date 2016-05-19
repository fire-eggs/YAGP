using System.Collections.Generic;

// Set of lines read from a GED file which encompass a 'record'
// Starts with '0'
namespace SharpGEDParser
{
    public class GedRecord
    {
        public int LineCount { get { return _lines.Count; } }

        private List<string> _lines;
        private int _firstLine;

        public int Beg { get { return _firstLine; } }
        public int End { get { return _firstLine + LineCount; } }

        public GedRecord()
        {
            _lines = new List<string>();
        }

        public GedRecord(int firstLine, string line) : this()
        {
            _lines.Add(line);
            _firstLine = firstLine;
        }

        public void AddLine(string line)
        {
            _lines.Add(line);
        }

        public override string ToString()
        {
            int count = _lines.Count;
            if (count < 1)
                return "";
            return string.Format("Record:({0},{1}):'{2}'", _firstLine, _lines.Count, _lines[0]);
        }

        public string FirstLine()
        {
            return _lines[0];
        }

        public string GetLine(int linedex)
        {
            return _lines[linedex];
        }
        public int Max { get { return _lines.Count; } }

        /// <summary>
        /// Determine the level value for a line.
        /// </summary>
        /// <param name="linedex">Index in the record for the line in question</param>
        /// <param name="sublinedex">The index WITHIN the line for the first character. non-zero if leading spaces.</param>
        /// <returns>Space on error; otherwise the level as a character</returns>
        public char GetLevel(int linedex, out int sublinedex)
        {
            sublinedex = -1;
            if (linedex >= Max)
                return ' ';
            string line = _lines[linedex];
            int dex = GedLineUtil.FirstChar(line);
            if (dex < 0)
                return ' '; // empty line
            sublinedex = dex;
            return line[dex];

            // TODO can level values exceed 9 ???
        }
    }
}
