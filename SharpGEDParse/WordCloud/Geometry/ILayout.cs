//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//
using System.Collections.Generic;
using System.Drawing;

namespace WordCloud.Geometry
{
    public interface ILayout
    {
        void Arrange(IEnumerable<IWord> words, IGraphicEngine graphicEngine);
        IEnumerable<LayoutItem> GetWordsInArea(RectangleF area);
    }
}