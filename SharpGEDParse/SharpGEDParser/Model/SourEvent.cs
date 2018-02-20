
namespace SharpGEDParser.Model
{
    /// <summary>
    /// Container for event data in a source (SOUR) record.
    /// </summary>
    ///
    /// Example for a record of births and deaths:
    /// \code
    /// 0 @S1@ SOUR
    /// 1 DATA
    /// 2 EVEN BIRT, DEAT
    /// 3 DATE FROM 1793 TO 1857
    /// 3 PLAC Random County, KY, USA
    /// \endcode
    /// The Text value will be "BIRT, DEAT"; Date value "FROM 1793 to 1857";
    /// and the Place value "Random County, KY, USA".
    public class SourEvent
    {
        // Event under SOURCE record + DATA

        // custom: tracked as other lines in SourceData class

        /// <summary>
        /// Any DATE text as entered in the GEDCOM.
        /// </summary>
        public string Date { get; set; } // TODO keep text for now; future parse/validate

        /// <summary>
        /// Any PLAC text as entered in the GEDCOM.
        /// </summary>
        public string Place { get; set; } // TODO any further parsing/validation?

        /// <summary>
        /// Any event information as recorded in the source.
        /// </summary>
        public string Text { get; set; }
    }
}