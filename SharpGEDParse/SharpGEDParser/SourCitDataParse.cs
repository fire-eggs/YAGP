
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
            (_rec as GedSourCit).Text = extendedText();
        }
    }
}
