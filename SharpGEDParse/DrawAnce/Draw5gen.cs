﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawAnce
{
    public class Draw5gen : DrawGen
    {
        private const int BOXH = 30;
        private const int BOXW = 250;
        private const int GEN4VM = 8; // vertical margin between boxes for Gen 3
        private const int OuterMargin = 5;
        private const int GEN3HM = 20;

        private const int HCONNLEN = GEN3HM/2;

        private Pen _boxPen;
        private Font _nameFont;
        private Brush _textBrush;

        public Draw5gen()
        {
            Init();
        }

        public override Image MakeAncTree()
        {
            Point boxSz0, boxSz1, boxSz2, boxSz3, boxSz4;

            // 16 boxes high, but the boxes are short - name only
            int maxH = 16*BOXH + 15*GEN4VM + 2*OuterMargin;
            int maxW = 4*BOXW;

            _boxPen = new Pen(Color.Chocolate, 2.0f);
            Pen connPen = new Pen(Color.Black, 2.0f);
            _nameFont = new Font("Arial", 12);
            _textBrush = new SolidBrush(Color.Black);

            // TODO calculate actual box widths
            boxSz0 = new Point(BOXW, BOXH);
            boxSz1 = new Point(BOXW, BOXH);
            boxSz2 = new Point(BOXW, BOXH);
            boxSz3 = new Point(BOXW, BOXH);
            boxSz4 = new Point(BOXW, BOXH);

            Bitmap bmp = new Bitmap(maxW, maxH);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.Cornsilk);
                
                // Draw gen 4 on right side
                int right = maxW - OuterMargin - MoreGenW;
                int left = right - boxSz4.X;
                int top = OuterMargin;
                Rectangle box4Rect = new Rectangle(left, top, boxSz4.X, boxSz4.Y);
                for (int i = 16; i <= 31; i++)
                {
                    DrawAnce(i, gr, box4Rect);

                    // draw connector
                    gr.DrawLine(connPen, left - 1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i%2 == 0)
                        gr.DrawLine(connPen, left-HCONNLEN, top+BOXH/2-1, left-HCONNLEN, top + BOXH / 2 + BOXH + GEN4VM + 1);

                    // Does this individual have ancestors? If so, draw a marker
                    if (AncData[i] != null && AncData[i].Ahnen > 1)
                        gr.DrawString(MORE_GEN, _nameFont, _textBrush, box4Rect.Right + 2, box4Rect.Top + BOXH / 2 - 10); // TODO how to calc. location?

                    top += BOXH + GEN4VM;
                    box4Rect.Location = new Point(left, top);
                }

                // draw gen 3. each box is centered vertically against
                // the two boxes to the right in gen 4. The boxes are stepped
                // left as a separate column to allow drawing connectors.
                int gen4step = 2*BOXH + GEN4VM;
                right = left - GEN3HM;
                left = right - boxSz3.X;
                int gen4top = OuterMargin;
                Rectangle box3Rect = new Rectangle(left, top, boxSz3.X, boxSz3.Y);
                for (int i = 8; i <= 15; i++)
                {
                    top = gen4top + gen4step/2 - BOXH/2;
                    box3Rect.Location = new Point(left, top);

                    // draw connector
                    gr.DrawLine(connPen, left, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    gr.DrawLine(connPen, right, top + BOXH / 2, right + HCONNLEN, top + BOXH / 2);
                    if (i % 2 == 0)
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2-1, left - HCONNLEN, top + BOXH + GEN4VM);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2+1, left - HCONNLEN, top - GEN4VM);

                    DrawAnce(i, gr, box3Rect);
                    gen4top += gen4step + GEN4VM;
                }

                // draw gen 2. the boxes are inset between the two boxes of gen 3.
                right = left + boxSz2.X/3;
                left = right - boxSz2.X;
                top = OuterMargin + gen4step / 2 - BOXH / 2 + BOXH + GEN4VM;
                int gen2step = 4*BOXH + 4*GEN4VM;
                Rectangle box2Rect = new Rectangle(left, top, boxSz2.X, boxSz2.Y);
                for (int i = 4; i <= 7; i++)
                {
                    DrawAnce(i, gr, box2Rect);
                    gr.DrawLine(connPen, left-1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i%2 == 0)
                        gr.DrawLine(connPen, left-HCONNLEN, top+BOXH/2-1, left-HCONNLEN, top+2*BOXH+2*GEN4VM);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 + 1, left - HCONNLEN, top - BOXH - 2*GEN4VM);

                    top += gen2step;
                    box2Rect.Location = new Point(left, top);
                }

                // draw gen 1.
                left = left - boxSz1.X / 2;
                top = OuterMargin + gen4step / 2 - BOXH / 2 + 3 * BOXH + 3 * GEN4VM;
                Rectangle box1Rect = new Rectangle(left, top, boxSz1.X, boxSz1.Y);
                for (int i = 2; i <= 3; i++)
                {
                    DrawAnce(i, gr, box1Rect);

                    // TODO not sure why these magic '3' values are required
                    // draw connector
                    gr.DrawLine(connPen, left-1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i==2)
                        gr.DrawLine(connPen, left-HCONNLEN, top+BOXH/2-1, left - HCONNLEN, top+4*BOXH+3*GEN4VM+3);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 + 1, left - HCONNLEN, top - 3 * BOXH - 4 * GEN4VM-3);

                    top += 8 * BOXH + 8 * GEN4VM;
                    box1Rect.Location = new Point(left, top);
                }

                // draw gen 0.
                left = left - boxSz0.X/5;
                top = OuterMargin + 8*BOXH + 7*GEN4VM - BOXH/2;
                DrawAnce(1, gr, new Rectangle(left, top, boxSz0.X, boxSz0.Y));
            }
            return bmp;
        }

        private void DrawAnce(int i, Graphics gr, Rectangle boxRect)
        {
            _hitRect[i] = boxRect;
            gr.DrawRectangle(_boxPen, boxRect);
            int left = boxRect.Left;
            int top = boxRect.Top;

            RectangleF textRect = boxRect;
            var nameLoc = new PointF(left, top + 3);
            textRect.Location = nameLoc;
            gr.DrawString(AncData[i].Name, _nameFont, _textBrush, nameLoc);
        }
    }
}
