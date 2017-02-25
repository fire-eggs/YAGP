//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//
using System.Drawing;

namespace WordCloud.Geometry
{
    public class LayoutItem
    {
        public LayoutItem(RectangleF rectangle, IWord word)
        {
            Rectangle = rectangle;
            Word = word;
        }

        public RectangleF Rectangle { get; private set; }
        public IWord Word { get; private set; }

        public LayoutItem Clone()
        {
            return new LayoutItem(Rectangle, Word);
        }
    }
}
