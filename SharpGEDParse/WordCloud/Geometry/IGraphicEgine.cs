//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//
using System;
using System.Drawing;

namespace WordCloud.Geometry
{
    public interface IGraphicEngine : IDisposable
    {
        SizeF Measure(string text, int weight);
        void Draw(LayoutItem layoutItem);
        void DrawEmphasized(LayoutItem layoutItem);
    }
}
