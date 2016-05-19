
// Parse the DATA sub-record within a SOUR citation record.
// Essentially only DATE and TEXT.
namespace SharpGEDParser
{
    public class SourCitDataParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet.Add("TEXT", txtProc);
            _tagSet.Add("DATE", dateProc);
        }

        private void dateProc()
        {
            (_rec as GedSourCit).Date = Remainder();
        }

        private void txtProc()
        {
            // TODO correct handling of spaces around CONC/CONT
            // TODO error handling when non-CONC/CONT found
            // TODO generalize (see GedEventParse\FAMCProc)

            (_rec as GedSourCit).Text = Remainder();
            if (_context.Endline > _context.Begline)
            {
                for (int i = _context.Begline + 1; i <= _context.Endline; i++)
                {
                    string line = _rec.Lines.GetLine(i);
                    string ident = null;
                    string tag = null;
                    int nextChar = GedLineUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
                    if (tag == "CONC")
                        (_rec as GedSourCit).Text += line.Substring(nextChar+1).Trim();
                    if (tag == "CONT")
                        (_rec as GedSourCit).Text += "\n" + line.Substring(nextChar+1).TrimEnd();
                }
            }

        }
    }
}
