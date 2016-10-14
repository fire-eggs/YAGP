
namespace SharpGEDParser.Model
{
    public class FamilyEvent : EventCommon
    {
        public AgeDetail HusbDetail { get; set; } // FAM event
        public AgeDetail WifeDetail { get; set; } // FAM event

        public string Age { get; set; } // INDI event, attribute

        public string Famc { get; set; } // INDI event

        public string FamcAdop { get; set; } // INDI event
    }
}
