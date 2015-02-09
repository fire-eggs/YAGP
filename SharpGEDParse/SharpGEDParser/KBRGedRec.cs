using System;

namespace SharpGEDParser
{
    public class KBRGedRec
    {
        protected GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            if (lines.LineCount < 1)
                throw new Exception("Empty GedRecord!");
            Lines = lines;
        }

        public override string ToString()
        {
            return string.Format("KBRGedRec:{0}:{1}:{2}", Tag, Ident, Lines);
        }

        public virtual void Parse()
        {
            // TODO make abstract?
            // TODO parse sub-record data: OVERRIDE ME
        }

        public virtual void Validate()
        {
            // TODO make abstract?
            // TODO check lines and add errors to an error set OVERRIDE ME
        }

        public string Ident { get; set; }
        public string Tag { get; set; }
    }
}
