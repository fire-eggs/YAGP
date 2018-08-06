//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//
// Tweaked to eliminate the left/right padding that DrawString/MeasureString introduce by default.
// This results in a "tighter" cloud - possibly too tight? Currently has padding above/below which
// I've not yet figured out how to reduce.
//

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using WordCloud.Geometry;

namespace WordCloud
{
    public class GdiGraphicEngine : IGraphicEngine
    {
        private readonly Graphics _graphics;
        private const TextFormatFlags FLAGS = TextFormatFlags.NoPadding;

        private readonly int m_MinWordWeight;
        private readonly int m_MaxWordWeight;
        private Font m_LastUsedFont;

        public FontFamily FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public Color[] Palette { get; private set; }
        public float MinFontSize { get; set; }
        public float MaxFontSize { get; set; }

        public GdiGraphicEngine(Graphics graphics, Color[] palette, int minWordWeight, int maxWordWeight)
        {
            m_MinWordWeight = minWordWeight;
            m_MaxWordWeight = maxWordWeight;
            _graphics = graphics;
            Palette = palette;
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        }

        public SizeF Measure(string text, int weight)
        {
            Font font = GetFont(weight);

            Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            return _graphics.MeasureString(text, font, proposedSize, StringFormat.GenericTypographic);
        }

        public void Draw(LayoutItem layoutItem)
        {
            Font font = GetFont(layoutItem.Word.Occurrences);
            Color color = GetPresudoRandomColorFromPalette(layoutItem);
            Brush brush = new SolidBrush(color);
            _graphics.DrawString(layoutItem.Word.Text, font, brush, layoutItem.Rectangle, StringFormat.GenericTypographic);
        }

        public void DrawEmphasized(LayoutItem layoutItem)
        {
            Font font = GetFont(layoutItem.Word.Occurrences);
            Color color = GetPresudoRandomColorFromPalette(layoutItem);
            Point point = new Point((int)layoutItem.Rectangle.X, (int)layoutItem.Rectangle.Y);
            // TODO is this out-of-sync with the measure / draw code?
            TextRenderer.DrawText(_graphics, layoutItem.Word.Text, font, point, Color.LightGray, FLAGS);
            int offset = (int)(5 *font.Size / MaxFontSize)+1;
            point.Offset(-offset, -offset);
            TextRenderer.DrawText(_graphics, layoutItem.Word.Text, font, point, color, FLAGS);
        }

        private Font GetFont(int weight)
        {
            float fontSize = (float)(weight - m_MinWordWeight) / (m_MaxWordWeight - m_MinWordWeight) * (MaxFontSize - MinFontSize) + MinFontSize;
            if (m_LastUsedFont == null ||
                Math.Abs(m_LastUsedFont.Size - fontSize) > float.Epsilon)
            {
                m_LastUsedFont = new Font(FontFamily, fontSize, FontStyle);
            }
            return m_LastUsedFont;
        }

        private Color GetPresudoRandomColorFromPalette(LayoutItem layoutItem)
        {
            Color color = Palette[layoutItem.Word.Occurrences * layoutItem.Word.Text.Length % Palette.Length];
            return color;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            _graphics.Dispose();
        }
    }
}
