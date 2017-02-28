using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private long jdn;
        private long range;

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public Types Type { get; set; }

        public GEDDate(Types _type = Types.Unknown)
        {
            Type = _type;
            Year = Month = Day = -1;
        }

        public GEDDate(int year, int month, int day)
        {
            Type = Types.Exact;
            Year = year;
            Month = month;
            Day = day;
        }
    }
}
