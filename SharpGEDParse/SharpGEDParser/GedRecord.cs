using System.Collections.Generic;

// Set of lines read from a GED file which encompass a 'record'
// Starts with '0'
using System.Diagnostics.CodeAnalysis;
using SharpGEDParser.Parser;

namespace SharpGEDParser
{
    public class GedRecord
    {
        public int LineCount { get { return _max; } }

        private readonly List<char []> _lines;
        private readonly int _firstLine;
        private int _max;

        public int Beg { get { return _firstLine; } }
        public int End { get { return _firstLine + _max -1; } }

        public GedRecord()
        {
            _lines = new List<char []>();
            _max = 0;
            _firstLine = 1;
        }

        public GedRecord(int firstLine, char [] line) : this()
        {
            AddLine(line);
            _firstLine = firstLine;
        }

        public void AddLine(char [] line)
        {
            _lines.Add(line);
            _max++;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            int count = _lines.Count;
            if (count < 1)
                return "";
            return string.Format("Record:({0},{1}):'{2}'", _firstLine, _lines.Count, _lines[0]);
        }

        public char [] FirstLine()
        {
            return _lines[0];
        }

        public char [] GetLine(int linedex)
        {
            return _lines[linedex];
        }

        public int Max { get { return _max; } }

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
            var line = _lines[linedex];
            int dex = LineUtil.FirstChar(line, 0, line.Length);
            // Can't happen? empty lines stripped earlier...
            //if (dex < 0)
            //    return ' '; // empty line
            sublinedex = dex;
            return line[dex];

            // TODO can level values exceed 9 ???
        }
    }
}
