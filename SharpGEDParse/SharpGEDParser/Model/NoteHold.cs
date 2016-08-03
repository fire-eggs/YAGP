using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Note : StructCommon
    {
        public string Xref { get; set; }

        public string Text { get; set; }
    }

    public interface NoteHold
    {
        List<Note> Notes { get; }
    }
}
