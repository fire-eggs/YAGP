using System;
using System.Drawing;
using System.Drawing.Printing;

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
            using (_boxPen = new Pen(Color.Chocolate, 2.0f))
            using (_nameFont = new Font("Arial", 12))
            using (_textBrush = new SolidBrush(Color.Black))
            {

                // A. Calculate the actual box widths. When drawing, this requires creating a temp. bitmap
                // and graphics context to measure against.
                using (Bitmap tmpBmp = new Bitmap(500, 500))
                {
                    using (Graphics gr = Graphics.FromImage(tmpBmp))
                    {
                        CalcActualBoxWidths(gr);
                        SizeF moreSize = gr.MeasureString(MORE_GEN, _nameFont);
                        MoreGenW = (int)moreSize.Width;
                    }
                }

                // 16 boxes high, but the boxes are short - name only
                int maxH = 16 * BOXH + 15 * GEN4VM + 2 * OuterMargin;
                int maxW = boxSz[0].X / 2 + boxSz[1].X / 2 + (2 * boxSz[2].X / 3) + boxSz[3].X + boxSz[4].X + MoreGenW;

                Bitmap bmp = new Bitmap(maxW, maxH);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(Color.Cornsilk);
                    DrawAncTree(gr, new Rectangle(0, 0, maxW, maxH));
                }
                return bmp;
            }
        }

        private void DrawAncTree(Graphics gr, Rectangle bounds)
        {
            int maxW = bounds.Right;

            //            _boxPen = new Pen(Color.Chocolate, 2.0f);
            //            _nameFont = new Font("Arial", 12);
            //            _textBrush = new SolidBrush(Color.Black);
            using (Pen connPen = new Pen(Color.Black, 2.0f))
            {
                SizeF moreSize = gr.MeasureString(MORE_GEN, _nameFont);
                MoreGenW = (int)moreSize.Width;

                gr.Clear(Color.Cornsilk);

                // Draw gen 4 on right side
                int right = maxW - OuterMargin - MoreGenW;
                int left = right - boxSz[4].X;
                int top = OuterMargin + bounds.Top;
                Rectangle box4Rect = new Rectangle(left, top, boxSz[4].X, boxSz[4].Y);
                for (int i = 16; i <= 31; i++)
                {
                    DrawAnce(i, gr, box4Rect);

                    // draw connector
                    gr.DrawLine(connPen, left - 1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i % 2 == 0)
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 - 1, left - HCONNLEN, top + BOXH / 2 + BOXH + GEN4VM + 1);

                    // Does this individual have ancestors? If so, draw a marker
                    if (AncData[i] != null && AncData[i].Ahnen > 1)
                        gr.DrawString(MORE_GEN, _nameFont, _textBrush, box4Rect.Right + 2, box4Rect.Top + BOXH / 2 - 10); // TODO how to calc. location?

                    top += BOXH + GEN4VM;
                    box4Rect.Location = new Point(left, top);
                }

                // draw gen 3. each box is centered vertically against
                // the two boxes to the right in gen 4. The boxes are stepped
                // left as a separate column to allow drawing connectors.
                int gen4step = 2 * BOXH + GEN4VM;
                right = left - GEN3HM;
                left = right - boxSz[3].X;
                int gen4top = OuterMargin + bounds.Top;
                Rectangle box3Rect = new Rectangle(left, top, boxSz[3].X, boxSz[3].Y);
                for (int i = 8; i <= 15; i++)
                {
                    top = gen4top + gen4step / 2 - BOXH / 2;
                    box3Rect.Location = new Point(left, top);

                    // draw connector
                    gr.DrawLine(connPen, left, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    gr.DrawLine(connPen, right, top + BOXH / 2, right + HCONNLEN, top + BOXH / 2);
                    if (i % 2 == 0)
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 - 1, left - HCONNLEN, top + BOXH + GEN4VM);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 + 1, left - HCONNLEN, top - GEN4VM);

                    DrawAnce(i, gr, box3Rect);
                    gen4top += gen4step + GEN4VM;
                }

                // draw gen 2. the boxes are inset between the two boxes of gen 3.
                right = left + boxSz[2].X / 3;
                left = right - boxSz[2].X;
                top = bounds.Top + OuterMargin + gen4step / 2 - BOXH / 2 + BOXH + GEN4VM;
                int gen2step = 4 * BOXH + 4 * GEN4VM;
                Rectangle box2Rect = new Rectangle(left, top, boxSz[2].X, boxSz[2].Y);
                for (int i = 4; i <= 7; i++)
                {
                    DrawAnce(i, gr, box2Rect);
                    gr.DrawLine(connPen, left - 1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i % 2 == 0)
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 - 1, left - HCONNLEN, top + 2 * BOXH + 2 * GEN4VM);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 + 1, left - HCONNLEN, top - BOXH - 2 * GEN4VM);

                    top += gen2step;
                    box2Rect.Location = new Point(left, top);
                }

                // draw gen 1.
                left = left - boxSz[1].X / 2;
                top = bounds.Top + OuterMargin + gen4step / 2 - BOXH / 2 + 3 * BOXH + 3 * GEN4VM;
                Rectangle box1Rect = new Rectangle(left, top, boxSz[1].X, boxSz[1].Y);
                for (int i = 2; i <= 3; i++)
                {
                    DrawAnce(i, gr, box1Rect);

                    // TODO not sure why these magic '3' values are required
                    // draw connector
                    gr.DrawLine(connPen, left - 1, top + BOXH / 2, left - HCONNLEN, top + BOXH / 2);
                    if (i == 2)
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 - 1, left - HCONNLEN, top + 4 * BOXH + 3 * GEN4VM + 3);
                    else
                        gr.DrawLine(connPen, left - HCONNLEN, top + BOXH / 2 + 1, left - HCONNLEN, top - 3 * BOXH - 4 * GEN4VM - 3);

                    top += 8 * BOXH + 8 * GEN4VM;
                    box1Rect.Location = new Point(left, top);
                }

                // draw gen 0.
                left = left - boxSz[0].X / 5;
                top = bounds.Top + OuterMargin + 8 * BOXH + 7 * GEN4VM - BOXH / 2;
                DrawAnce(1, gr, new Rectangle(left, top, boxSz[0].X, boxSz[0].Y));
            }
        }

        public override PrintDocument PrintAncTree()
        {
            if (AncData == null || AncData[1] == null)
                return null; // no person selected

            // TODO how/when is this disposed? 
            // TODO can this be pre-created somehow?

            PrintDocument pdoc = new PrintDocument();
            pdoc.DocumentName = AncData[1].Name;
            pdoc.BeginPrint += BeginPrint;
            pdoc.PrintPage += PrintPage;
            pdoc.QueryPageSettings += QueryPageSettings;
            pdoc.EndPrint += EndPrint;
            return pdoc;
        }

        void BeginPrint(object sender, PrintEventArgs e)
        {
        }

        private void EndPrint(object sender, PrintEventArgs e)
        {
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {
            using (_boxPen = new Pen(Color.Chocolate, 2.0f))
            using (_nameFont = new Font("Arial", 12))
            using (_textBrush = new SolidBrush(Color.Black))
            {
                CalcActualBoxWidths(e.Graphics);
                DrawAncTree(e.Graphics, e.MarginBounds);
            }

            using (Pen dashed_pen = new Pen(Color.Red, 3))
            {
                dashed_pen.DashPattern = new float[] { 10, 10 };
                e.Graphics.DrawRectangle(dashed_pen, e.MarginBounds);
            }

            e.HasMorePages = false; //(Lines < LINES_TO_PRINT);
        }

        private void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
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

        private Point[] boxSz;

        private void CalcActualBoxWidths(Graphics gr)
        {
            boxSz = new Point[5];
            boxSz[0] = CalcBoxDims(gr, 1, 1);
            boxSz[1] = CalcBoxDims(gr, 2, 3);
            boxSz[2] = CalcBoxDims(gr, 4, 7);
            boxSz[3] = CalcBoxDims(gr, 8, 15);
            boxSz[4] = CalcBoxDims(gr, 16, 31);
        }

        private Point CalcBoxDims(Graphics gr, int ancL, int ancH)
        {
            int penW = (int)Math.Ceiling(_boxPen.Width); // take the box pen into account, otherwise draws over right text margin
            int maxW = 0;
            int maxH = 0;
            for (int i = ancL; i <= ancH; i++)
            {
                var nameSize = gr.MeasureString(AncData[i].Name, _nameFont);
                maxW = Math.Max(maxW, (int)Math.Ceiling(nameSize.Width) + penW); // round up: text margin too small
                maxW = Math.Max(maxW, 100); // prevent collapsed boxes
                maxH = Math.Max(BOXH, (int)nameSize.Height);
            }
            return new Point(maxW, maxH);
        }

    }
}
