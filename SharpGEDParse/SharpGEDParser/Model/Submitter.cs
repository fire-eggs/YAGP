
namespace SharpGEDParser.Model
{
    public class Submitter
    {
        public enum SubmitType
        {
            SUBM,
            DESI,
            ANCI
        }
        public SubmitType SubmitterType;
        public string Xref;
    }
}
