using GEDWrap;
using System.Net;
using System.Text;

namespace FamilyGroup
{
    public interface IChartDraw
    {
        StringBuilder DrawTo { set; }

        Person[] Ancestors { set; }

        Union Base { set; }

        Forest Trees { set; }

        void FillStyle();

        void DrawChart();

        string Spouse1Text { set; }
        string Spouse2Text { set; }

        string FontFam { set; }
    }

    public abstract class ChartDraw
    {
        public virtual string Filler { get; set; }

        protected string HtmlText(string inT)
        {
            if (inT == null)
                inT = Filler;
            if (string.IsNullOrWhiteSpace(inT))
                return "&nbsp;";
            return WebUtility.HtmlEncode(inT);
        }
    }
}
