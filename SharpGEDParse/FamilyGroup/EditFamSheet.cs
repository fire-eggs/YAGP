using System;
using System.Security.Permissions;
using System.Windows.Forms;
using GEDWrap;
using System.Collections.Generic;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCommaInArrayInitializer
// ReSharper disable UnusedAutoPropertyAccessor.Local

// NOTE: styles probably not in external file because of special formatting for sb.AppendFormat

namespace FamilyGroup
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    public class EditFamSheet : ChartDraw, IChartDraw
    {
        public StringBuilder DrawTo { set; private get; }

        public Person[] Ancestors { set; private get; }

        private Union _family;
        private List<Child> _childs;

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

        /*
         * 1. style string
         * 2. color details (look up in map)
         * 3. font details (look up in map)
         * 4. font-size delta
         */
        private static readonly Tuple<string, bool, bool, int>[] STYLE_STRINGS2 =
        {
            Tuple.Create(".tt{<color><font>font-weight:bold;}",true,true,0),
            Tuple.Create(".tg{width:100%;border-collapse:collapse;border-spacing:0;<color>}",true,false,0),
            Tuple.Create(".tg td{<font>padding:4px 3px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;<color>}",true,true,-1),
            Tuple.Create(".tg th{<font>font-weight:normal;padding:5px 4px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;<color>}",true,true,0),
            Tuple.Create(".tNest td{<font>padding:1px 1px;border-width:0px;overflow:hidden;word-break:normal;<color>}",true,true,-1),
            Tuple.Create(".tg .tg-9hbo{font-weight:bold;vertical-align:top}",false,false,0),
            Tuple.Create(".tg .tg-9hboN{font-weight:bold;vertical-align:top;width:5%;}",false,false,0),
            Tuple.Create(".tg .tg-9hboPH{font-weight:bold;vertical-align:top;width:15%;}",false,false,0), /* person header */
            Tuple.Create(".tg .tg-dzk6{<color>text-align:center;vertical-align:top}",true,false,0),
            Tuple.Create(".tg .tg-b7b8{<color>vertical-align:top}",true,false,0),
            Tuple.Create(".tg .tg-b7b8PD{<color>vertical-align:top;width:30%;}",true,false,0), /* person data */
            Tuple.Create(".tg .tg-yw4l{vertical-align:top}",false,false,0),
            Tuple.Create(".tg .tg-baqh{text-align:center;vertical-align:top}",false,false,0),
            Tuple.Create(".tNest {width:100%;border-collapse:collapse;}",false,false,0),
            Tuple.Create(".tNest .tNt{border-style:none none dashed none; border-width:1px;<color>}",true,false,0),
            Tuple.Create(".tNest .tN-b7b8{<color>vertical-align:top}",true,false,0),
            Tuple.Create(".tNest .tNt-b7b8{border-style:none none dashed none; border-width:1px;<color>vertical-align:top}",true,false,0),
            Tuple.Create("a:link{<color>}",true,false,0),
            Tuple.Create("a:active{<color>}",true,false,0),
            Tuple.Create("a:visited{<color>}",true,false,0),
            Tuple.Create("a:hover{<color>}",true,false,0),
        };

        private static readonly Tuple<int, string>[] COLOR_STRINGS_GREY =
        {
            Tuple.Create(0, "color:#333;background-color:#fff;"),
            Tuple.Create(1, "border-color:#ccc;"),
            Tuple.Create(2, "border-color:#ccc;color:#333;"),
            Tuple.Create(3, "border-color:#ccc;color:#333;background-color:#aaa;"),
            Tuple.Create(4, "border-color:#ccc;color:#333;"),
            Tuple.Create(8, "background-color:#f0f0f0;"),
            Tuple.Create(9, "background-color:#f0f0f0;"),
            Tuple.Create(10, "background-color:#f0f0f0;"),
            Tuple.Create(14, "border-color:#ccc;"),
            Tuple.Create(15, "background-color:#f0f0f0;"),
            Tuple.Create(16, "border-color:#ccc;background-color:#f0f0f0;"),
            Tuple.Create(17, "color:#333;"),
            Tuple.Create(18, "color:#333;"),
            Tuple.Create(19, "color:#333;"),
            Tuple.Create(20, "color:#333;"),
        };

        private static readonly Tuple<int, string>[] COLOR_STRINGS_BLUE =
        {
            Tuple.Create(0, "color:#039;background-color:#D2E4FC;"), // tt
            Tuple.Create(1, "border-color:#aabcfe;"), // tg
            Tuple.Create(2, "border-color:#aabcfe;color:#669;background-color:#e8edff;"), // tg td
            Tuple.Create(3, "border-color:#aabcfe;color:#039;background-color:#b9c9fe;"), // tg th
            Tuple.Create(4, "border-color:#ccc;color:#333;"), // tnest td
            Tuple.Create(8, "background-color:#D2E4FC;"), // .tg .tg-dzk6
            Tuple.Create(9, "background-color:#D2E4FC;"), // .tg .tg-b7b8
            Tuple.Create(10, "background-color:#D2E4FC;"),
            Tuple.Create(14, "border-color:#aabcfe;"),
            Tuple.Create(15, "background-color:#D2E4FC;"),
            Tuple.Create(16, "border-color:#aabcfe;background-color:#D2E4FC;"),
            Tuple.Create(17, "color:#039;"),
            Tuple.Create(18, "color:#039;"),
            Tuple.Create(19, "color:#039;"),
            Tuple.Create(20, "color:#039;"),
        };

        private static readonly Tuple<int, string>[] COLOR_STRINGS_GREEN =
        {
            Tuple.Create(0, "color:#493F3F;background-color:#C2FFD6;"), // tt
            Tuple.Create(1, "border-color:#bbb;"), // tg
            Tuple.Create(2, "border-color:#bbb;color:#594F4F;background-color:#E0FFEB;"), // tg td
            Tuple.Create(3, "border-color:#bbb;color:#493F3F;background-color:#9DE0AD;"), // tg th
            Tuple.Create(4, "border-color:#bbb;color:#333;"), // tnest td
            Tuple.Create(8, "background-color:#C2FFD6;"), // .tg .tg-dzk6
            Tuple.Create(9, "background-color:#C2FFD6;"), // .tg .tg-b7b8
            Tuple.Create(10, "background-color:#C2FFD6;"),
            Tuple.Create(14, "border-color:#594F4F;"),
            Tuple.Create(15, "background-color:#C2FFD6;"),
            Tuple.Create(16, "border-color:#493F3F;background-color:#C2FFD6;"),
            Tuple.Create(17, "color:#493F3F;"),
            Tuple.Create(18, "color:#493F3F;"),
            Tuple.Create(19, "color:#493F3F;"),
            Tuple.Create(20, "color:#493F3F;"),
        };

        private string getColorString(int dex)
        {
            foreach (var tuple in _colorScheme)
            {
                if (tuple.Item1 == dex)
                    return tuple.Item2;
            }
            throw new Exception("blech");
        }

        private string getFontString(int sizeDelta)
        {
            string fSize = FontSize;
            if (sizeDelta != 0)
                fSize = (int.Parse(FontSize) + sizeDelta).ToString();
            return string.Format("font-family:{0};font-size:{1}px;", FontFam, fSize);
        }

        public void FillStyle()
        {
            int i = 0;
            foreach (var tuple in STYLE_STRINGS2)
            {
                var outStr = tuple.Item1;
                if (tuple.Item2) // color
                {
                    var cStr = getColorString(i);
                    outStr = outStr.Replace("<color>", cStr);
                }

                if (tuple.Item3) // font
                {
                    var fStr = getFontString(tuple.Item4);
                    outStr = outStr.Replace("<font>", fStr);
                }
                DrawTo.AppendLine(outStr);
                i++;
            }
        }

        public void DrawChart(bool showUrl = false)
        {
            StringBuilder sb = DrawTo;

            // TODO needs a style
            // TODO need to use HTMLText
            sb.AppendFormat("<h3>Family Group Report - {0} : {1} + {2}", _family.Id,
                _family.Husband == null ? " " : _family.Husband.Name,
                _family.Wife == null ? " " : _family.Wife.Name).AppendLine();
            sb.AppendLine("</h3>");

            Person who = _family.Husband ?? _family.Wife;
            fillPerson(showUrl, Spouse1Text, who, true);

            sb.AppendLine("&nbsp;");

            who = _family.Spouse(who);
            fillPerson(showUrl, Spouse2Text, who);

            sb.AppendLine("&nbsp;");

            fillChildren();

            // TODO need Done/Cancel buttons. When those are clicked, set EditElem to null.
            // TODO switching views / family should be an implicit Cancel. Consider disabling controls until edit is complete?
            EditElem = null;

        }

        public string Spouse1Text { set; private get; }
        public string Spouse2Text { set; private get; }
        public string FontFam { set; private get; }
        public string Theme
        {
            get { return ""; }
            set
            {
                switch (value)
                {
                    case "grey":
                        _colorScheme = COLOR_STRINGS_GREY;
                        break;
                    case "blue":
                        _colorScheme = COLOR_STRINGS_BLUE;
                        break;
                    case "green":
                        _colorScheme = COLOR_STRINGS_GREEN;
                        break;
                    default:
                        throw new Exception("blech");
                }
            }
        }

        // User has clicked on something to edit it
        public string EditElem { get; set; }

        private Tuple<int, string>[] _colorScheme;

        private void fillPerson(bool showUrl, string caption, Person who, bool inclMarr = false)
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

                string occu;
                string flag = inclMarr ? "P" : "S";
                if (showUrl)
                {
                    occu = "<a href='' onclick='window.external.doEdit(\"" + flag + "-OCCU\")'>Occupation</a>";
                }
                else
                {
                    occu = "Occupation";
                }
                string occuId = flag + "-OCCU";

                string occuView;
                if (EditElem == occuId)
                {
                    occuView = "<input type=\"text\" value=\"" + HtmlText(who.GetWhat("OCCU")) + "\" />";
                }
                else
                {
                    occuView = HtmlText(who.GetWhat("OCCU"));
                }

                sb.AppendFormat("<td class=\"tg-b7b8\";>{2}</td><td class=\"tg-b7b8\";>{0}</td><th class=\"tg-9hboPH\";>{3}</th><td id=\"{4}\" class=\"tg-b7b8\";>{1}</td>", HtmlText(who.Name), occuView, HtmlText(who.Id), occu, occuId).AppendLine();
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

        public void doEdit(string field)
        {
            MessageBox.Show(field, "link");
        }
    }
}

/*

<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta charset="utf-8" />
  <title></title>
</head>
<body>
  <image src="data:image/gif;base64,{0}" /> <!-- Replace here -->
</body>
</html>
 
var html = GetStringResource(); // However you do it.
var imageData = GetImageResource(); // Ditto
var encoded = GetBase64FromBinary(imageData);
var finalHtml = string.Format(html, encoded);

*/