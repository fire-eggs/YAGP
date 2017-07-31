using System;
using GEDWrap;
using System.Text;

// ReSharper disable RedundantCommaInArrayInitializer
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable InconsistentNaming

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

        private static readonly string[] STYLE_STRINGS =
        {
            ".PEDperson_name{vertical-align: bottom;border-bottom: solid 2px black;margin: 0;padding: 10px 10px 5px 10px;font-weight: bold;}",
            ".PEDdate{vertical-align: top; font-size: .85em;color: #777;margin: 0;padding: 2px 10px 10px 10px;}",
            ".PEDleftborder{border-left: solid 2px black;}",
            ".PEDtable{margin: 2em 0 2em 0; border:0; border-spacing:0px; width:100%; border-collapse:collapse;}",
        };

        public void FillStyle()
        {
            // Style for this table. names cannot conflict with other styles.
            foreach (var s in STYLE_STRINGS)
            {
                DrawTo.AppendLine(s);
            }
        }

        // first: 1-based row in TABLE_STRINGS array
        // second: 1-based person index in ancestors
        // third: true if name, false if data
        private static readonly Tuple<int, int, bool>[] TABLE_MAP =
        {
            Tuple.Create( 4, 1,true),
            Tuple.Create( 5, 2,true),
            Tuple.Create( 6, 4,true),
            Tuple.Create( 7, 8,true),
            Tuple.Create(10, 8,false),

            Tuple.Create(13, 4,false),
            Tuple.Create(14, 9,true),
            Tuple.Create(17, 9,false),
            Tuple.Create(20, 2,false),
            Tuple.Create(21, 5,true),

            Tuple.Create(22,10,true),
            Tuple.Create(25,10,false),
            Tuple.Create(28, 5,false),
            Tuple.Create(29,11,true),
            Tuple.Create(32,11,false),

            Tuple.Create(35, 1,false),
            Tuple.Create(36, 3,true),
            Tuple.Create(37, 6,true),
            Tuple.Create(38,12,true),
            Tuple.Create(41,12,false),

            Tuple.Create(44, 6,false),
            Tuple.Create(45,13,true),
            Tuple.Create(48,13,false),
            Tuple.Create(51, 3,false),
            Tuple.Create(52, 7,true),

            Tuple.Create(53,14,true),
            Tuple.Create(56,14,false),
            Tuple.Create(59, 7,false),
            Tuple.Create(60,15,true),
            Tuple.Create(63,15,false),
        };

        private static readonly string[] TABLE_STRINGS =
        {
        "<table border=\"1px\" class=\"PEDtable\">",
        "<tbody>",
        "<tr>",
        "<td class=\"PEDperson_name\" rowspan=\"8\">{0}</td>", /* person 1-name */
        "<td class=\"PEDperson_name\" rowspan=\"4\">{0}</td>", /* person 2-name */ // 5
        "<td class=\"PEDperson_name\" rowspan=\"2\">{0}</td>", /* person 4-name */
        "<td class=\"PEDperson_name\">{0}</td>",               /* person 8-name */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate PEDleftborder\">{0}</td>",                    /* person 8-data */ // 10
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate PEDleftborder\" rowspan=\"2\">{0}</td>",                    /* person 4-data */
        "<td class=\"PEDperson_name PEDleftborder\">{0}</td>", /* person 9-name */
        "</tr>", // 15
        "<tr>",
        "<td class=\"PEDdate\">{0}</td>",                    /* person 9-data */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate PEDleftborder\" rowspan=\"4\">{0}</td>",                    /* person 2-data */ // 20
        "<td class=\"PEDperson_name PEDleftborder\" rowspan=\"2\">{0}</td>",  /* person 5-name */
        "<td class=\"PEDperson_name\">{0}</td>",                             /* person 10-name */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate PEDleftborder\">{0}</td>",                    /* person 10-data */ // 25
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate\" rowspan=\"2\">{0}</td>",                    /* person 5-data */
        "<td class=\"PEDperson_name PEDleftborder\">{0}</td>",               /* person 11-name */
        "</tr>", // 30
        "<tr>",
        "<td class=\"PEDdate\">{0}</td>",                    /* person 11-data */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate\" rowspan=\"8\">{0}</td>",                    /* person 1-data */ // 35
        "<td class=\"PEDperson_name PEDleftborder\" rowspan=\"4\">{0}</td>", /* person 3-name */
        "<td class=\"PEDperson_name\" rowspan=\"2\">{0}</td>",               /* person 6-name */
        "<td class=\"PEDperson_name\">{0}</td>",                            /* person 12-name */
        "</tr>",
        "<tr>", // 40
        "<td class=\"PEDdate PEDleftborder\">{0}</td>",                    /* person 12-data */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate PEDleftborder\" rowspan=\"2\">{0}</td>",                    /* person 6-data */
        "<td class=\"PEDperson_name PEDleftborder\">{0}</td>",              /* person 13-name */ // 45
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate\">{0}</td>",                    /* person 13-data */
        "</tr>",
        "<tr>", // 50
        "<td class=\"PEDdate\" rowspan=\"4\">{0}</td>",                    /* person 3-data */
        "<td class=\"PEDperson_name PEDleftborder\" rowspan=\"2\">{0}</td>", /* person 7-name */
        "<td class=\"PEDperson_name\">{0}</td>",                            /* person 14-name */
        "</tr>",
        "<tr>", // 55
        "<td class=\"PEDdate PEDleftborder\">{0}</td>",                    /* person 14-data */
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate\" rowspan=\"2\">{0}</td>",                     /* person 7-data */
        "<td class=\"PEDperson_name PEDleftborder\">{0}</td>",              /* person 15-name */ // 60
        "</tr>",
        "<tr>",
        "<td class=\"PEDdate\">{0}</td>",                                  /* person 15-data */
        "</tr>",
        "</tbody>", // 65
        "</table>",
        };

        public void DrawChart()
        {
            int i = 1; // map entries are 1-based
            int mapdex = 0;
            foreach (var s in TABLE_STRINGS)
            {
                while (mapdex < TABLE_MAP.Length) // Find the row in the map for this table row (if it exists)
                {
                    var tup = TABLE_MAP[mapdex];
                    if (i <= tup.Item1)
                        break;
                    mapdex++;
                }

                if (mapdex >= TABLE_MAP.Length || TABLE_MAP[mapdex].Item1 != i) // didn't find row in map
                {
                    DrawTo.AppendLine(s);
                }
                else
                {
                    var tup2 = TABLE_MAP[mapdex];
                    // NOTE tup2.Item2 is 1-based, ancestors list is  0-based
                    var val = string.Format("{0}({1})", tup2.Item3 ? "Name" : "Data", tup2.Item2);
                    DrawTo.AppendFormat(s, val).AppendLine();
                }
                i++;
            }
        }

        public string Spouse1Text { set; private get; }
        public string Spouse2Text { set; private get; }
        public string FontFam { set; private get; }
    }
}
