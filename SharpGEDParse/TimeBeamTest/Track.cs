using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBeamTest
{
    public enum TimeStyle
    {
        Solid, Dashed, Faded
    }

    public interface ITrack
    {
        int Start { get; set; }
        int? End { get; set; }

        TimeStyle BegStyle { get; set; }

        TimeStyle EndStyle { get; set; }

        string Name { get; set; }

        string ToolTip { get; set; }
    }

    public abstract class Track : ITrack
    {
        public int Start { get; set; }
        public int? End { get; set; }
        public TimeStyle BegStyle { get; set; }
        public TimeStyle EndStyle { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
    }
}
