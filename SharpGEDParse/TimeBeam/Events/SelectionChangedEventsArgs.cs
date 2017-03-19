using System;

namespace TimeBeam.Events
{
    /// <summary>
    ///   Event arguments for an event that notifies about a change in the selection
    ///   of tracks.
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   The tracks that were selected in the operation.
        /// </summary>
        public ITimelineTrack Selected { get; private set; }

        /// <summary>
        ///   The track elements that were deselected in the operation.
        /// </summary>
        public ITimelineTrack Deselected { get; private set; }

        /// <summary>
        ///   Construct a new SelectionChangedEventArgs instance.
        /// </summary>
        /// <param name="selected">The track elements that were deselected in the operation.</param>
        /// <param name="deselected">The tracks that were selected in the operation.</param>
        public SelectionChangedEventArgs(ITimelineTrack selected, ITimelineTrack deselected)
        {
            Selected = selected;
            Deselected = deselected;
        }

        /// <summary>
        ///   An empty instance of the <see cref="SelectionChangedEventArgs"/> class.
        /// </summary>
        public new static SelectionChangedEventArgs Empty
        {
            get { return new SelectionChangedEventArgs(null, null); }
        }
    }
}