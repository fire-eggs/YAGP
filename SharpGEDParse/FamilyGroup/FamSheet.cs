using GEDWrap;
using System.Collections.Generic;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCommaInArrayInitializer

// NOTE: styles probably not in external file because of special formatting for sb.AppendFormat

namespace FamilyGroup
{
    public class FamSheet : ChartDraw, IChartDraw
    {
        public StringBuilder DrawTo { set; private get; }

        public Person[] Ancestors { set; private get; }

        private Union _family;
        private List<Child> _childs;

        public override string Filler
        {
            get { return base.Filler; }
            set
            {
                base.Filler = value;
                // TODO is this still necessary?
                foreach (var child in _childs)
                {
                    child.Filler = value;
                }
            }
        }
            
        public Union Base
        {
            set
            {
                _family = value;
                _childs = new List<Child>();
                int i = 1;
                foreach (var person in _family.Childs)
                {
                    Child aCh = new Child(person, i, Filler);
                    _childs.Add(aCh);
                    i++;
                }
            } 
        }

        public Forest Trees { set; private get; }

        // NOTE: double-brackets required for escape when using AppendFormat
        private static readonly string[] STYLE_STRINGS =
        {
        ".tt{{color:#333; background-color:#fff;font-family:{0};font-size:14px;font-weight:bold;}}",
        ".tg{{width:100%;border-collapse:collapse;border-spacing:0;border-color:#ccc;}}",
        ".tg td{{font-family:{0};font-size:13px;padding:4px 3px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;}}",
        ".tg th{{font-family:{0};font-size:14px;font-weight:normal;padding:5px 4px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;background-color:#aaa;}}",
        ".tNest td{{font-family:{0};font-size:13px;padding:1px 1px;border-width:0px;overflow:hidden;word-break:normal;border-color:#ccc;color:#333;}}",
        ".tg .tg-9hbo{font-weight:bold;vertical-align:top}",
        ".tg .tg-9hboN{font-weight:bold;vertical-align:top;width:5%;}",
        ".tg .tg-9hboPH{font-weight:bold;vertical-align:top;width:15%;} /* person header */",
        ".tg .tg-dzk6{background-color:#f0f0f0;text-align:center;vertical-align:top}",
        ".tg .tg-b7b8{background-color:#f0f0f0;vertical-align:top}",
        ".tg .tg-b7b8PD{background-color:#f0f0f0;vertical-align:top;width:30%;} /* person data */",
        ".tg .tg-yw4l{vertical-align:top}",
        ".tg .tg-baqh{text-align:center;vertical-align:top}",
        ".tNest {width:100%;border-collapse:collapse;}",
        ".tNest .tNt{border-style:none none dashed none; border-width:1px; border-color:#ccc;}",
        ".tNest .tN-b7b8{background-color:#f0f0f0;vertical-align:top}",
        ".tNest .tNt-b7b8{border-style:none none dashed none; border-width:1px; border-color:#ccc;background-color:#f0f0f0;vertical-align:top}",
        };

        public void FillStyle()
        {
            int i = 0;
            foreach (var s in STYLE_STRINGS)
            {
                if (i < 5)
                {
                    DrawTo.AppendFormat(s, FontFam).AppendLine();
                }
                else
                {
                    DrawTo.AppendLine(s);
                }
                i++;
            }
        }

        public void DrawChart()
        {
            StringBuilder sb = DrawTo;

            sb.AppendFormat("Family Group Report - {0} : {1} + {2}", _family.Id,
                _family.Husband == null ? " " : _family.Husband.Name,
                _family.Wife == null ? " " : _family.Wife.Name).AppendLine();
            sb.AppendLine("</h3>");

            Person who = _family.Husband ?? _family.Wife;
            fillPerson(Spouse1Text, who, true);

            sb.AppendLine("&nbsp;");

            who = _family.Spouse(who);
            fillPerson(Spouse2Text, who);

            sb.AppendLine("&nbsp;");

            fillChildren();
        }

        public string Spouse1Text { set; private get; }
        public string Spouse2Text { set; private get; }
        public string FontFam { set; private get; }

        private void fillPerson(string caption, Person who, bool inclMarr = false)
        {
            StringBuilder sb = DrawTo;

            // Name-full Occupation
            // Born Place
            // Opt: Christened Place
            // inclMarr: Married Place
            // inclMarr: Divorced Place
            // Died Place
            // Opt: Buried Place
            // Parent Parent
            // Other spouses?
            sb.AppendLine("<table class=\"tg\">");
            sb.AppendFormat("<caption class=\"tt\">{0}</caption>", caption);
            if (who == null)
                sb.AppendLine("<tr><td>None</td></tr>");
            else
            {
                sb.AppendLine("<tr>");
                sb.AppendFormat("<td class=\"tg-b7b8\";>{2}</td><td class=\"tg-b7b8\";>{0}</td><th class=\"tg-9hboPH\";>Occupation</th><td class=\"tg-b7b8\";>{1}</td>", HtmlText(who.Name), HtmlText(who.GetWhat("OCCU")), HtmlText(who.Id)).AppendLine();
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr>");
                sb.AppendFormat("<th class=\"tg-9hboPH\";>Born</th><td class=\"tg-b7b8PD\";>{0}</td><th class=\"tg-9hboPH\";>Place</th><td class=\"tg-b7b8PD\";>{1}</td>", HtmlText(who.GetDate("BIRT")), HtmlText(who.GetPlace("BIRT"))).AppendLine();
                sb.AppendLine("</tr>");
                if (inclMarr)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendFormat("<th class=\"tg-9hboPH\";>Married</th><td class=\"tg-b7b8PD\";>{0}</td><th class=\"tg-9hboPH\";>Place</th><td class=\"tg-b7b8PD\";>{1}</td>", HtmlText(_family.GetDate("MARR")), HtmlText(_family.GetPlace("MARR"))).AppendLine();
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("<tr>");
                sb.AppendFormat("<th class=\"tg-9hboPH\";>Died</th><td class=\"tg-b7b8PD\";>{0}</td><th class=\"tg-9hboPH\";>Place</th><td class=\"tg-b7b8PD\";>{1}</td>", HtmlText(who.GetDate("DEAT")), HtmlText(who.GetPlace("DEAT"))).AppendLine();
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr>");
                sb.AppendFormat("<th class=\"tg-9hboPH\";>Parent</th><td class=\"tg-b7b8PD\";>{0}</td><th class=\"tg-9hboPH\";>Parent</th><td class=\"tg-b7b8PD\";>{1}</td>", HtmlText(who.GetParent(true)), HtmlText(who.GetParent(false))).AppendLine();
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
        }

        private static readonly string[] childHeader =
        {
        "<tr>",
        "<th class=\"tg-9hboN\">No</th>",
        "<th class=\"tg-9hboN\">Id</th>",
        "<th class=\"tg-9hbo\">Name</th>",
        "<th class=\"tg-9hboN\">Sex</th>",
        "<th class=\"tg-9hbo\">BDate/DDate</th>",
        "<th class=\"tg-9hbo\">BPlace/DPlace</th>",
        "<th class=\"tg-9hbo\">Spouse / Details</th>",
        "</tr>",
        };

        private void fillChildren()
        {
            var sb = DrawTo;
            sb.AppendLine("<table class=\"tg\">");
            sb.AppendLine("<caption class=\"tt\">Children</caption>");

            if (_childs.Count < 1)
            {
                sb.AppendLine("<tr><td>None</td></tr>");
            }
            else
            {
                foreach (var s in childHeader)
                {
                    sb.AppendLine(s);
                }
                int i = 1;
                foreach (var child in _childs)
                {
                    fillChild(i, child);
                    i++;
                }
            }

            sb.AppendLine("</table>");
        }

        // TODO different style for alternate rows!
        private static readonly string[] CHILD_ROW =
        {
        "<tr>",
        "<td class=\"tg-dzk6\">{0}</td>",
        "<td class=\"tg-b7b8\">{0}</td>",
        "<td class=\"tg-b7b8\">{0}</td>",
        "<td class=\"tg-dzk6\">{0}</td>",
        "<td class=\"tg-b7b8\">",
        "<table class=\"tNest\">",
        "<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
        "<tr><td class=\"tN-b7b8\">{0}</td></tr>",
        "</table>",
        "</td>",
        "<td class=\"tg-b7b8\"> <!-- bplace/dplace -->",
        "<table class=\"tNest\">",
        "<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
        "<tr><td class=\"tN-b7b8\">{0}</td></tr>",
        "</table>",
        "</td>",
        "<td class=\"tg-b7b8\"> <!-- Marriage details -->",
        "<table class=\"tNest\">",
        "<tr><td class=\"tNt-b7b8\">{0}</td></tr>",
        "<tr><td class=\"tN-b7b8\">{0};{1}</td></tr>",
        "</table>",
        "</td>",
        "</tr>",
        };

        private void fillChild(int num, Child child)
        {
            StringBuilder sb = DrawTo;
            int dex = 0;
            foreach (var s in CHILD_ROW)
            {
                switch (dex)
                {
                    case 1:
                        sb.AppendFormat(s, num);
                        break;
                    case 2:
                        sb.AppendFormat(s, HtmlText(child.Id));
                        break;
                    case 3:
                        sb.AppendFormat(s, HtmlText(child.Name));
                        break;
                    case 4:
                        sb.AppendFormat(s, HtmlText(child.Sex));
                        break;
                    case 7:
                        sb.AppendFormat(s, HtmlText(child.BDate));
                        break;
                    case 8:
                        sb.AppendFormat(s, HtmlText(child.DDate));
                        break;
                    case 13:
                        sb.AppendFormat(s, HtmlText(child.BPlace));
                        break;
                    case 14:
                        sb.AppendFormat(s, HtmlText(child.DPlace));
                        break;
                    case 19:
                        sb.AppendFormat(s, HtmlText(child.MSpouse));
                        break;
                    case 20:
                        // TODO when both are empty: "-;-" not good?
                        sb.AppendFormat(s, HtmlText(child.MDate), HtmlText(child.MPlace));
                        break;
                    default:
                        sb.AppendLine(s);
                        break;
                }
                dex++;
            }
        }

    }
}
