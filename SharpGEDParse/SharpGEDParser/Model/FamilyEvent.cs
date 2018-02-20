
namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents an event for a family.
    /// </summary>
    /// 
    /// Stores details specific to family events.
    /// Examples include MARR, DIV, ENGA, RESI, ...
    public class FamilyEvent : EventCommon
    {
        /// <summary>
        /// The age of the Husband at the time of the event.
        /// </summary>
        /// Will be null if not specified in the GEDCOM.
        public AgeDetail HusbDetail { get; set; } // FAM event

        /// <summary>
        /// The age of the Wife at the time of the event.
        /// </summary>
        /// Will be null if not specified in the GEDCOM.
        public AgeDetail WifeDetail { get; set; } // FAM event
    }

    /// <summary>
    /// Represents an event for an individual.
    /// </summary>
    /// 
    /// Includes BIRT,CHR,DEAT,BURI,CREM,ADOP, ...
    public class IndiEvent : EventCommon
    {
        /// <summary>
        /// Cross-reference to the family for this event.
        /// </summary>
        /// 
        /// Applies to BIRT, CHR and ADOP events.
        /// See FamcAdop for an example.
        public string Famc { get; set; } // INDI event

        /// <summary>
        /// Adopted by which parent?
        /// </summary>
        /// 
        /// Will be null if not specified in the GEDCOM.
        /// 
        /// Example:
        /// \code
        /// 0 @I1@ INDI
        /// 1 ADOP
        /// 2 FAMC @F1@
        /// 3 ADOP HUSB
        /// \endcode
        /// The Famc property will be "F1" and the FamcAdop property "HUSB".
        public string FamcAdop { get; set; } // INDI event
    }
}