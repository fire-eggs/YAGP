using System.Collections.Generic;

namespace TimeBeam
{
    public class Marker
    {
        public string Char { get; set; }
        public int Time { get; set; }
        public bool Above { get; set; }
    }

    public enum TrackStyle
    {
        Imprecise, Precise
    }

    /// <summary>
    ///   Describes an item that can be placed on a track in the timeline.
    /// </summary>
    public interface ITimelineTrack : ITimelineTrackBase
    {
        /// <summary>
        ///   The beginning of the item.
        /// </summary>
        float Start { get; set; }

        TrackStyle StartStyle { get; set; }

        /// <summary>
        ///   The end of the item.
        /// </summary>
        float? End { get; set; }

        List<Marker> Marks { get; set; }

        bool Split { get; set; }
    }
}