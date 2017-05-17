using System.Collections.Generic;
using System.Text;

namespace SharpGEDParser.Model
{
    public class Note : StructCommon
    {
        public string Xref { get; set; }

        public string Text { get; set; }

        public StringBuilder Builder { get; set; } // Accumulate text during parse

        // Memory hog
        //public Note()
        //{
        //    Builder = new StringBuilder(1024);
        //}
    }

    public interface NoteHold
    {
        List<Note> Notes { get; }
    }
}
