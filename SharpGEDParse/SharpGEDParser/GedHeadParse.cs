using System.Diagnostics;

namespace SharpGEDParser
{
    public class GedHeadParse : GedParse
    {
        public void Parse(KBRGedRec rec)
        {
//            Debug.Assert(false);
        }

        public void Parse(KBRGedRec rec, GedRecParse.ParseContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
