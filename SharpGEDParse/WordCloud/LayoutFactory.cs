//
// https://www.codeproject.com/Articles/224231/Word-Cloud-Tag-Cloud-Generator-Control-for-NET-Win
//
using System;
using System.Drawing;
using WordCloud.Geometry;

namespace WordCloud
{
    public enum LayoutType
    {
        Typewriter,
        Spiral
    }

    public static class LayoutFactory
    {
        public static ILayout CrateLayout(LayoutType layoutType, SizeF size)
        {
            switch (layoutType)
            {
                case LayoutType.Typewriter:
                    return new TypewriterLayout(size);

                case LayoutType.Spiral:
                    return new SpiralLayout(size);
            
                default:
                    throw new ArgumentException(string.Format("No constructor specified to create a layout instance for {0}.", layoutType), "layoutType");
            }
        }
    }
}
