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
    }
}
