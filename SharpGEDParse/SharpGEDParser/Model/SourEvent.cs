
namespace SharpGEDParser.Model
{
    public class SourEvent
    {
        // Event under SOURCE record + DATA

        // custom: tracked as other lines in SourceData class

        public string Date { get; set; } // TODO keep text for now; future parse/validate

        public string Place { get; set; } // TODO any further parsing/validation?

        public string Text { get; set; }
    }
}
