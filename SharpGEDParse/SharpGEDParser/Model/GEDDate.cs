using System;
using System.Globalization;

namespace SharpGEDParser.Model
{
    public class GEDDate
    {
        public enum Types
        {
            Unknown=0,
            Exact,
            Estimated,
            Before,
            After,
            Range
        }

        public long JDN { get; set; } // This date's Julian Day Number
        public long JDR { get; set; } // This date's Julian Day Range [meaning defined by Type]

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public bool IsBC { get; set; }

        public Types Type { get; set; }

        public GEDDate(Types _type = Types.Unknown)
        {
            Type = _type;
            Year = Month = Day = -1;
        }

        public override string ToString()
        {
            // TODO variants: range, BC
            // TODO other formats
            return string.Format("{0}-{1}-{2}", 
                Day <= 0 ? "" : Day.ToString(),
                Month <= 0 ? "" : DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Month), 
                Year);
        }
    }
}
