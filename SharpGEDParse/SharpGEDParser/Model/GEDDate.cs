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
            JDN = -1;
        }

        public bool Initialized { get { return JDN != -1; } }

        public override string ToString()
        {
            // TODO variants: range, BC
            // TODO other formats
            return string.Format("{0}{1}{2}{3}{4}", 
                Day <= 0 ? "" : Day.ToString(),
                Day <= 0 ? "" : "-",
                Month <= 0 ? "" : DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(Month),
                Month <= 0 ? "" : "-",
                Year <=0 ? "" : Year.ToString());
        }
    }
}
