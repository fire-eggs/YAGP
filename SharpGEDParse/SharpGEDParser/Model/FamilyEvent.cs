
namespace SharpGEDParser.Model
{
    public class FamilyEvent : EventCommon
    {
        public AgeDetail HusbDetail { get; set; } // FAM event
        public AgeDetail WifeDetail { get; set; } // FAM event
    }

    public class IndiEvent : EventCommon
    {
        public string Famc { get; set; } // INDI event

        public string FamcAdop { get; set; } // INDI event
        
    }
}
