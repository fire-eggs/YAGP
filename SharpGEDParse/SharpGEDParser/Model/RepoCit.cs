using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser.Model
{
    // Repository Citation from SOURCE record
    public class RepoCit
    {
        public string Xref { get; set; }

        public string CallNum { get; set; }

        public string Media { get; set; }

        private NoteHold _noteHold = new NoteHold();

        public List<Note> Notes { get { return _noteHold.Notes; } }

        // TODO unknowns
    }
}
