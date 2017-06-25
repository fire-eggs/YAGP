using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser.Model
{
    public class HeadRecord : GEDCommon, NoteHold
    {
        public override string Tag { get { return "HEAD"; } }

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        // Identity strings for submitters
        private List<IndiRecord.Submitter> _submit; // TODO common?
        public List<IndiRecord.Submitter> Submitters { get { return _submit ?? (_submit = new List<IndiRecord.Submitter>()); } }

        public string Source { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }

        public string GedVersion { get; set; }

        public DateTime GedDate { get; set; }

        public string CharSet { get; set; }

        public HeadRecord(GedRecord lines) : base(lines, null)
        {
            
        }

        public void AddSubmitter(int submType, string ident)
        {
            Submitters.Add(new IndiRecord.Submitter { SubmitterType = submType, Xref = ident });
            // TODO at later time must validate the specified xref exists
        }

    }
}
