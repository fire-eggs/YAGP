using GEDWrap;
using System;
using System.Text;

namespace FamilyGroup
{
    public class Pedigree : IChartDraw
    {
        // TODO name color
        // TODO name size
        // TODO data color
        // TODO data size
        // TODO data fields
        // TODO connected pedigree

        public StringBuilder DrawTo { set; private get; }
        public Person[] Ancestors { set; private get; }
        public Union Base { set; private get; }
        public Forest Trees { set; private get; }


        public void FillStyle()
        {
            // Style for this table. names cannot conflict with other styles.
        }

        public void DrawChart()
        {
            throw new NotImplementedException();
        }

        public string Spouse1Text { set; private get; }
        public string Spouse2Text { set; private get; }
        public string FontFam { set; private get; }
    }
}
