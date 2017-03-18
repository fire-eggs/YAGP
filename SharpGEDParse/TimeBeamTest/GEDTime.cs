using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeBeamTest
{
    public partial class GEDTime : UserControl
    {
        public Color BackgroundColor { get; set; }

        public GEDTime()
        {
            InitializeComponent();

            _tracks = new List<ITrack>();

            BackgroundColor = Color.PowderBlue;
            TrackLabelWide = 200;
            TrackHigh = 20;
            TrackBorderSize = 0;
            TrackSpace = 5;
            DecadeLabelHigh = 16;

            DoubleBuffered = true;

            // TODO is this necessary? useful?
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.Opaque |
                      ControlStyles.OptimizedDoubleBuffer |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.Selectable |
                      ControlStyles.UserPaint, true);

            // TODO adjust font for scale
            float emHeightForLabel = EmHeightForLabel("WM_g^~", TrackHigh); // TODO KBR TrackHeight changes on RenderingScale change?
            _labelFont = new Font(DefaultFont.FontFamily, emHeightForLabel - 2);

            // The last year showable
            int year = DateTime.Now.Year;
            year += 10 - year%10; // round up to next decade
            year += 4; // TODO tweak for decade label size?
            FarRightYear = year;
        }

        private Font _labelFont;

        private float EmHeightForLabel(string label, float maxHeight)
        {
            float size = DefaultFont.Size;
            Font currentFont = new Font(DefaultFont.FontFamily, size);
            Graphics graphics = Graphics.FromHwnd(Handle);
            SizeF measured = graphics.MeasureString(label, currentFont);
            while (measured.Height < maxHeight)
            {
                size += 1;
                currentFont = new Font(DefaultFont.FontFamily, size);
                measured = graphics.MeasureString(label, currentFont);
            }
            return size - 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(BackgroundColor);
            DrawGrid(g);
            DrawTracks(g);
            DrawLabels(g);

            ScrollbarH.Refresh();
            ScrollbarV.Refresh();
        }

        private void DrawGrid(Graphics g)
        {
            Rectangle trackAreaBounds = GetTrackAreaBounds();

            g.DrawRectangle(new Pen(Color.Red), trackAreaBounds );

            Pen gridPen = new Pen(Color.Black);
            Pen decPen = new Pen(Color.Salmon);

            int year = FarRightYear;
            // TODO adjust final year value for scroll amount

            int gridWide = (int) (10 * _renderingScale.X);

            int x = 1;
            int maxX = trackAreaBounds.Width;
            for (; x < maxX; x += gridWide)
            {
                if (year%10 == 0)
                {
                    var strS = g.MeasureString(year.ToString(), _labelFont);
                    float left = trackAreaBounds.Right - x - strS.Width/2;
                    float top = trackAreaBounds.Y - _labelFont.GetHeight();
                    using (var textBrush = new SolidBrush(Color.DodgerBlue))
                    {
                        g.DrawString(year.ToString(), _labelFont, textBrush, left, top);
                    }
                }

                Pen penToUse = (year%10 == 0) ? decPen : gridPen;

                g.DrawLine(penToUse, trackAreaBounds.Right - x, 
                                     trackAreaBounds.Y, 
                                     trackAreaBounds.Right - x, 
                                     trackAreaBounds.Height);
                year -= 2;

            }
        }

        private int YearDelta(int year)
        {
            int delta = (int) ((FarRightYear - year)*5*_renderingScale.X);
            return delta;
        }

        private void DrawTracks(Graphics g)
        {
            Rectangle trackAreaBounds = GetTrackAreaBounds();
            float y = _renderingScale.Y;

            foreach (var track in _tracks)
            {
                int left = YearDelta(track.Start);
                int right = DateTime.Now.Year;
                if (track.End.HasValue)
                    right = track.End.Value;
                right = YearDelta(right);

                using (Brush b = new SolidBrush(Color.Purple))
                    g.FillRectangle(b, 
                        trackAreaBounds.Right - left,
                        trackAreaBounds.Y+ (int)y,
                        left-right,
                        TrackHigh * _renderingScale.Y);

                y += (TrackSpace + TrackHigh)*_renderingScale.Y;
            }
        }

        private void DrawLabels(Graphics g)
        {
            using (Brush b = new SolidBrush(Color.Peru))
            {
                float y = (4.0f + DecadeLabelHigh) * _renderingScale.Y;
                foreach (var track in _tracks)
                {
                    g.DrawString(track.Name, _labelFont, b, 0, y);

                    y += (TrackSpace + TrackHigh) * _renderingScale.Y;
                }
            }

        }

        // TODO update as track labels / font changes
        private int TrackLabelWide { get; set; }

        private int TrackSpace { get; set; }

        private int TrackHigh { get; set; }

        private int TrackBorderSize { get; set; }

        // TODO update as font changes?
        private int DecadeLabelHigh { get; set; }

        private int FarRightYear { get; set; }

        private Rectangle GetTrackAreaBounds()
        {
            Rectangle trackArea = new Rectangle();

            // Start after the track labels
            trackArea.X = TrackLabelWide;
            // Start at the top (later, we'll deduct the playhead and time label height)
            trackArea.Y = (int)(DecadeLabelHigh * _renderingScale.Y);
            // Deduct scrollbar width.
            trackArea.Width = Width - TrackLabelWide - ScrollbarV.Width;
            // Deduct scrollbar height.
            trackArea.Height = Height - ScrollbarH.Height;

            return trackArea;
        }

        private List<ITrack> _tracks;

        /// <summary>
        ///   The scale at which to render the timeline.
        ///   This enables us to "zoom" the timeline in and out.
        /// </summary>
        private PointF _renderingScale = new PointF(1, 1);

        public PointF RenderingScale
        {
            get { return _renderingScale; }
            set
            {
                _renderingScale = value;
                RecalculateScrollbarBounds();
                Invalidate();
            }
        }

        private void RecalculateScrollbarBounds()
        {
            if (_tracks.Count == 0)
            {
                ScrollbarH.Maximum = 0;
                ScrollbarV.Maximum = 0;
                //ScrollbarH.Max = ScrollbarV.Max = 0;
            }
            else
            {
                ScrollbarV.Maximum = (int)((_tracks.Count * (TrackHigh + TrackSpace)) * _renderingScale.Y);
                ScrollbarH.Maximum = 100;
                // TODO (int)(_tracks.Max(t => t.TrackElements.Any() ? t.TrackElements.Max(te => te.End) : 0) * _renderingScale.X);
            }
            ScrollbarV.Refresh();
            ScrollbarH.Refresh();
        }

        private void ReactToChange()
        {
            RecalculateScrollbarBounds();
            Invalidate();
        }

        public void ClearAllTracks()
        {
            _tracks.Clear();
            ReactToChange();
        }

        public void AddTrack(ITrack track)
        {
            _tracks.Add(track);
            ReactToChange();
        }
    }
}
