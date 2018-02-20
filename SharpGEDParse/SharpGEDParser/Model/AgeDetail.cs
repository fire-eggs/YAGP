namespace SharpGEDParser.Model
{
    /// <summary>
    /// The age of a spouse at the time of a family event.
    /// </summary>
    public class AgeDetail : StructCommon
    {
        /// <summary>
        /// Any additional information, see Age for an example.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// The age as entered in the GEDCOM.
        /// </summary>
        /// Example:
        /// \code
        /// 1 MARR Y
        /// 2 WIFE pregnant
        /// 3 AGE unknown
        /// 2 DATE 1 Apr 1880
        /// \endcode
        /// The Detail property will be 'pregnant', and Age will be 'unknown'.
        public string Age { get; set; }
    }
}