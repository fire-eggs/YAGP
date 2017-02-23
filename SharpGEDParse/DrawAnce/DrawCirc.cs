using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Draw pedigree as a doughnut chart
using GEDWrap;

namespace DrawAnce
{
    public class DrawCirc : DrawGen
    {
        private const int RADIUS_STEP = 75;
        private const int OUTER_MARGIN = 10;

        public DrawCirc()
        {
            Init();
        }

        private static Color[] genColors = new Color[]
        {
            Color.Coral,
            Color.CadetBlue,
            Color.DarkGray,
            Color.Khaki,
            Color./*CadetBlue,*/LawnGreen,
            Color./*DarkGray,*/Khaki,
            Color./*Khaki,*/HotPink,
            Color./*CadetBlue,*/Ivory,
            Color.Black, // text
            Color.Moccasin, // background
            Color.Black, // lines
            Color.PaleGreen, // lines
        };

        private void DrawAncCirc(Graphics gr, Rectangle bounds)
        {
            // 16-31
            //  8-15
            //  4-7
            //  2-3
            //  1

            for (int gen = 4; gen >= 0; gen--)
            {
                int dataOffset = 1 << gen;
                int segmentCount = 1 << gen;

                int left = OUTER_MARGIN + (4 - gen)*RADIUS_STEP;
                int top = left;
                int wide = (gen+1)*RADIUS_STEP*2;
                int high = wide;
                Rectangle rect = new Rectangle(left, top, wide, high);

                float fDegAngle = 360.0f/segmentCount;
                float fDegStart = 0.0f;
                using (Pen pen = new Pen(Color.Black))
                using (Brush brush = new SolidBrush(genColors[gen]))
                    if (gen == 0)
                    {
                        // TODO draw circle
                        gr.FillEllipse(brush, rect);
                        gr.DrawEllipse(pen, rect);
                        drawText(gr, 1, 0, 0, RADIUS_STEP);
                    }
                    else
                    {
                        for (int i = 0; i < segmentCount; i++)
                        {
                            if (AncData[dataOffset + i] != null)
                            {
                                gr.FillPie(brush, rect, fDegStart, fDegAngle);
                                gr.DrawPie(pen, rect, fDegStart, fDegAngle);
                                drawText(gr, dataOffset + i, fDegStart, fDegAngle, gen*RADIUS_STEP);
                            }
                            fDegStart += fDegAngle;
                        }
                    }
            }
        }

        private Font _nameFont;
        private Brush _textBrush;

        private void drawText(Graphics gr, int ancestor, float startAngle, float sweepAngle, int radius)
        {
            if (AncData[ancestor] == null)
                return;

            Person p = AncData[ancestor];
            int center = OUTER_MARGIN + 5 * RADIUS_STEP;
            SizeF tSize = gr.MeasureString(p.Given, _nameFont);

            if (ancestor == 1)
            {
                // Special case the center/circle
                gr.TranslateTransform(center, center);
                gr.DrawString(p.Given, _nameFont, _textBrush,
                    new PointF(-tSize.Width / 2, -tSize.Height));
                tSize = gr.MeasureString(p.Surname, _nameFont);
                gr.DrawString(p.Surname, _nameFont, _textBrush,
                    new PointF(-tSize.Width / 2, 0));

                gr.ResetTransform();
                return;
            }

            radius += RADIUS_STEP/2;
            float angle = startAngle + sweepAngle/2;
            float radius1 = radius + tSize.Height / 2;

            float dy = (float) Math.Sin(Math.PI*angle/180.0)*radius1;
            float dx = (float) Math.Cos(Math.PI*angle/180.0)*radius1;

            gr.TranslateTransform(center + dx, center + dy);
            gr.RotateTransform(90+angle);
            gr.DrawString(p.Given, _nameFont, _textBrush, 
                new PointF(-tSize.Width/2,-tSize.Height/2));

            tSize = gr.MeasureString(p.Surname, _nameFont);
            gr.DrawString(p.Surname, _nameFont, _textBrush,
                new PointF(-tSize.Width / 2, +tSize.Height/2));

            gr.ResetTransform();
        }

        public override Image MakeAncTree()
        {
            if (AncData == null || AncData[1] == null)
                return null; // no person selected

            int maxh = (RADIUS_STEP*5 + OUTER_MARGIN)*2;
            int maxw = maxh;

            using (_nameFont = new Font("Arial", 12))
            using (_textBrush = new SolidBrush(Color.Black))
            {
                Bitmap bmp = new Bitmap(maxw, maxh);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.SmoothingMode = SmoothingMode.AntiAlias;
                    gr.Clear(Color.Cornsilk);
                    DrawAncCirc(gr, new Rectangle(0, 0, maxw, maxh));
                }
                return bmp;
            }
        }

        public override PrintDocument PrintAncTree()
        {
            throw new NotImplementedException();
        }
    }
}
