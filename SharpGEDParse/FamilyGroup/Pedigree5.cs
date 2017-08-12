using GEDWrap;
using System;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCommaInArrayInitializer
// ReSharper disable UnusedAutoPropertyAccessor.Local

/* 5-Generation pedigree chart to HTML
 */
namespace FamilyGroup
{
    class Pedigree5 : ChartDraw, IChartDraw
    {
        public StringBuilder DrawTo { set; private get; }
        public Person[] Ancestors { set; private get; }
        public Union Base { set; private get; }
        public Forest Trees { set; private get; }

        // TODO non-conflicting style names
        // TODO format strings for color
        private static readonly string[] STYLE_STRINGS =
        {
            "table, td, th { border: 0px solid #595959; border-spacing:0; width: 100%; }",
            "/* Internet explorer looks better w/ collapse, but chrome looks worse */",
            ".ie {border-collapse:collapse;}",
            "td, th {width: 25%;padding: 3px;}",
            "th {background: #f0e6cc;}",
            ".even {background: #ebe8e0;vertical-align:bottom;}",
            ".odd {background: #f7f6f4;vertical-align:top;}",
            ".botB { border-bottom: 2px solid black;}",
            ".leftB { border-left: 2px solid black;}",
        };

        public void FillStyle()
        {
            // Style for this table. names cannot conflict with other styles.
            foreach (var s in STYLE_STRINGS)
            {
                DrawTo.AppendLine(s);
            }
        }

        private static readonly string[] TABLE_STRINGS =
        {
"<!-- Conditional formatting for Internet Explorer, see above -->",
"<!--[if !IE]>-->",
"<table>",
"<!--><![endif]-->",
"<!--[if IE]>",
"<table class=\"ie\">",
"<![endif]-->",
	"<tbody>",
		"<tr>",
			"<td></td>",
			"<td></td>",
			"<td class=\"even botB\" rowspan=\"2\">8</td>",
			"<td class=\"botB\">16</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td></td>",
			"<td class=\"leftB\">17</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td class=\"botB even\" rowspan=\"2\">4</td>",
			"<td class=\"odd leftB\" rowspan=\"2\">9</td>",
			"<td class=\"botB leftB\">18</td>",
"		</tr>",
		"<tr>",
			"<td class=\"botB even\" rowspan=\"2\">2</td>",
			"<td>19</td>",
"		</tr>",
		"<tr>",
			"<td class=\"odd\" rowspan=\"2\">5</td>",
			"<td class=\"even botB leftB\" rowspan=\"2\">10</td>",
			"<td class=\"botB\">20</td>",
"		</tr>",
		"<tr>",
			"<td class=\"leftB\">&nbsp;</td>",
			"<td class=\"leftB\">21</td>",
"		</tr>",
		"<tr>",
			"<td class=\"leftB\">&nbsp;</td>",
			"<td></td>",
			"<td class=\"odd\" rowspan=\"2\">11</td>",
			"<td class=\"botB leftB\">22</td>",
"		</tr>",
		"<tr>",
			"<td class=\"leftB\" rowspan=\"2\">1</td>",
			"<td></td>",
			"<td>23</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td class=\"even botB\" rowspan=\"2\">12</td>",
			"<td class=\"botB\">24</td>",
"		</tr>",
		"<tr>",
			"<td class=\"leftB\"></td>",
			"<td></td>",
			"<td class=\"leftB\">25</td>",
"		</tr>",
		"<tr>",
			"<td class=\"leftB botB\"></td>",
			"<td class=\"botB even\" rowspan=\"2\">6</td>",
			"<td class=\"odd leftB\" rowspan=\"2\">13</td>",
			"<td class=\"botB leftB\">26</td>",
"		</tr>",
		"<tr>",
			"<td class=\"odd\" rowspan=\"2\">3</td>",
			"<td>27</td>",
"		</tr>",
		"<tr>",
			"<td class=\"odd\" rowspan=\"2\">7</td>",
			"<td class=\"even botB leftB\" rowspan=\"2\">14</td>",
			"<td class=\"botB\">28</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td class=\"leftB\">29</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td></td>",
			"<td class=\"odd\" rowspan=\"2\">15</td>",
			"<td class=\"botB leftB\">30</td>",
"		</tr>",
		"<tr>",
			"<td></td>",
			"<td></td>",
			"<td>31</td>",
"		</tr>",
"	</tbody>",
"</table>",

        };

        public void DrawChart(bool showUrl = false)
        {
            foreach (var s in TABLE_STRINGS)
            {
                DrawTo.AppendLine(s);
            }
        }

        public string Spouse1Text { set; private get; }
        public string Spouse2Text { set; private get; }
        public string FontFam { set; private get; }
    }
}
