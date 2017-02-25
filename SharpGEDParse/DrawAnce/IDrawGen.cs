using GEDWrap;
using System.Drawing;
using System.Drawing.Printing;

namespace DrawAnce
{
    public interface IDrawGen
    {
        Person[] AncData { get; set; }

        Image MakeAncTree();

        int HitIndex(Point hit);

        PrintDocument PrintAncTree();

        int Palette { set; }
    }

    public abstract class DrawGen : IDrawGen
    {
        protected const string MORE_GEN = "►";
        protected int MoreGenW;

        protected Rectangle[] _hitRect;

        public int Palette { protected get; set; }

        public Person[] AncData { get; set; }

        public abstract Image MakeAncTree();

        public abstract PrintDocument PrintAncTree();

        protected void Init()
        {
            _hitRect = new Rectangle[32];
            for (int i = 0; i < 32; i++)
                _hitRect[i] = new Rectangle();
        }

        /// <summary>
        /// Determine which rectangle a Point intersects.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns>The index within the rectangle array; -1 if no intersection.</returns>
        public int HitIndex(Point hit)
        {
            if (_hitRect == null)
                return -1;
            for (int i = 0; i < _hitRect.Length; i++)
                if (_hitRect[i].Contains(hit))
                    return i;
            return -1;
        }

    }
}
