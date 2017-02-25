using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DrawAnce
{
    public partial class SwatchPick : Form
    {
        // A selection of hard-coded palettes from ColorBrewer
        private static string[][] _swatches =
        {
            new[] {"#f0f9e8","#ccebc5","#a8ddb5","#7bccc4","#4eb3d3","#2b8cbe","#08589e"},
            new[] {"#fef0d9","#fdd49e","#fdbb84","#fc8d59","#ef6548","#d7301f","#990000"},
            new[] {"#feebe2","#fcc5c0","#fa9fb5","#f768a1","#dd3497","#ae017e","#7a0177"},
            new[] {"#ffffcc","#d9f0a3","#addd8e","#78c679","#41ab5d","#238443","#005a32"},
            new[] {"#8c510a","#d8b365","#f6e8c3","#f5f5f5","#c7eae5","#5ab4ac","#01665e"},
            new[] {"#c51b7d","#e9a3c9","#fde0ef","#f7f7f7","#e6f5d0","#a1d76a","#4d9221"},
            new[] {"#762a83","#af8dc3","#e7d4e8","#f7f7f7","#d9f0d3","#7fbf7b","#1b7837"},
            new[] {"#b2182b","#ef8a62","#fddbc7","#f7f7f7","#d1e5f0","#67a9cf","#2166ac"},
            new[] {"#d73027","#fc8d59","#fee090","#ffffbf","#e0f3f8","#91bfdb","#4575b4"},
            new[] {"#f1eef6","#d0d1e6","#a6bddb","#74a9cf","#3690c0","#0570b0","#034e7b"},
            new[] {"#ffffb2","#fed976","#feb24c","#fd8d3c","#fc4e2a","#e31a1c","#b10026"},
        };

        public static Color[] Swatch(int index)
        {
            List<Color> alist = new List<Color>();
            foreach (var hexcolor in _swatches[index])
            {
                alist.Add(ColorTranslator.FromHtml(hexcolor));
            }
            return alist.ToArray();
        }

        public SwatchPick()
        {
            InitializeComponent();
            List<object> listbox_items = new List<object>();
            for (int i=0; i < _swatches.Length; i++)
                listbox_items.Add(i);
            listBox1.Items.AddRange(listbox_items.ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private const int BOX_SIZE = 25;

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var swatch = Swatch(e.Index);
            int i = 0;
            foreach (var color in swatch)
            {
                using (Brush brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds.X + i * BOX_SIZE, e.Bounds.Top + 1, BOX_SIZE, BOX_SIZE);
                }
                i++;
            }

            e.DrawFocusRectangle();
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = BOX_SIZE + 2;
        }

        public int SwatchIndex
        {
            get { return listBox1.SelectedIndex; }
            set { listBox1.SelectedIndex = value; }
        }
    }
}
