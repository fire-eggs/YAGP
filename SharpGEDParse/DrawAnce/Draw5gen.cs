using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawAnce
{
    public class Draw5gen : DrawGen
    {
        private const int BOXH = 80;
        private const int BOXW = 250;
        private const int GEN3VM = 15; // vertical margin between boxes for Gen 3
        private const int OuterMargin = 5;
        private const int GEN2HM = 75;

        private const int TEXTSTEP = 6;
        private const int TEXTINDENT = 2;

        public override Image MakeAncTree()
        {
            throw new NotImplementedException();
        }

    }
}
