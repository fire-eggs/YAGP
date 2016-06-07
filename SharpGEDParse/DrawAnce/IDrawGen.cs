using System.Drawing;

namespace DrawAnce
{
    public interface IDrawGen
    {
        IndiWrap[] AncData { get; set; }

        Image MakeAncTree();

        int HitIndex(Point hit);
    }
}
