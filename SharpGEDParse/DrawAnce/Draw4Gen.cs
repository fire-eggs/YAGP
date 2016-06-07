using System;
using System.Drawing;

namespace DrawAnce
{
    public class Draw4Gen : DrawGen
    {
        private const int BOXH = 80;
        private const int GEN3VM = 15; // vertical margin between boxes for Gen 3
        private const int OuterMargin = 5;
        private const int GEN2HM = 75;

        private const int TEXTSTEP = 6;
        private const int TEXTINDENT = 2;

        private Pen _boxPen;
        private Font _nameFont;
        private Font _textFont;
        private Brush _textBrush;
        private StringFormat _sf;

        public Draw4Gen()
        {
            Init();
        }

        public override Image MakeAncTree()
        {
            Point boxSz0, boxSz1, boxSz2, boxSz3;

            // 1. calc h/w. Assume 3 gen back. therefore, 8 boxes high plus margin. Width is calculated based on contents.
            int maxH = 8 * BOXH + 7 * GEN3VM + 2 * OuterMargin;
            int maxW; // = 2 * OuterMargin + GEN2HM + 2 * BOXW + (int)(BOXW * 1.5);

            _boxPen = new Pen(Color.Chocolate, 2.0f);
            Pen connPen = new Pen(Color.Black, 2.0f);
            _nameFont = new Font("Arial", 12);
            _textFont = new Font("Arial", 9);
            _textBrush = new SolidBrush(Color.Black);

            // A. Calculate the actual box widths. This requires creating a temp. bitmap
            // and graphics context to measure against.
            using (Bitmap tmpBmp = new Bitmap(500, 500))
            {
                using (Graphics gr = Graphics.FromImage(tmpBmp))
                {
                    boxSz0 = CalcBoxDims(gr, 1, 1);
                    boxSz1 = CalcBoxDims(gr, 2, 3);
                    boxSz2 = CalcBoxDims(gr, 4, 7);
                    boxSz3 = CalcBoxDims(gr, 8, 15);
                }
            }

            maxW = MoreGenW + 2 * OuterMargin + GEN2HM + boxSz3.X + boxSz2.X + 2 * boxSz1.X / 3 + 3 * boxSz0.X / 4;

            // 2. make bitmap to draw on.
            Bitmap bmp = new Bitmap(maxW, maxH);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.Cornsilk);

                _sf = new StringFormat();
                //                _sf.Trimming = StringTrimming.EllipsisCharacter;
                _sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;

                //Point boxSz = calcBoxDims(gr, 8, 15);

                // 3. draw gen 3 on right side
                int right = maxW - OuterMargin - MoreGenW;
                int left = right - boxSz3.X;
                int top = OuterMargin;
                Rectangle box3Rect = new Rectangle(left, top, boxSz3.X, BOXH);
                int vLineTop = 0;
                for (int i = 8; i <= 15; i++)
                {
                    DrawAnc(i, gr, box3Rect);
                    // partial connector, gen3 to gen2
                    gr.DrawLine(connPen, left, top + BOXH / 2, left - GEN2HM / 2, top + BOXH / 2);

                    // draw vertical connector line between pairs of boxes
                    if (vLineTop != 0)
                        gr.DrawLine(connPen, left - GEN2HM / 2, vLineTop - 1, left - GEN2HM / 2, top + BOXH / 2 + 1);
                    vLineTop = i % 2 == 0 ? top + BOXH / 2 : 0;

                    // Does this individual have ancestors? If so, draw a marker
                    if (AncData[i] != null && AncData[i].Ahnen > 1)
                        gr.DrawString(MORE_GEN, _nameFont, _textBrush, box3Rect.Right + 2, box3Rect.Top + BOXH / 2 - 10); // TODO how to calc. location?

                    top += BOXH + GEN3VM;
                    box3Rect.Location = new Point(left, top);
                }

                // 4. draw gen 2. each box is centered vertically against
                // the two boxes to the right in gen 3. The boxes are stepped
                // left as a separate column to allow drawing connectors.
                int gen3step = 2 * BOXH + GEN3VM;
                right = left - GEN2HM;
                left = right - boxSz2.X;
                int gen3top = OuterMargin;
                Rectangle box2Rect = new Rectangle(left, top, boxSz2.X, BOXH);
                int gen2Togen1LineLeft = left - boxSz1.X / 3;
                for (int i = 4; i <= 7; i++)
                {
                    top = gen3top + gen3step / 2 - BOXH / 2;
                    box2Rect.Location = new Point(left, top);

                    // complete the connector, gen3 to gen2
                    gr.DrawLine(connPen, right, top + BOXH / 2, right + GEN2HM / 2, top + BOXH / 2);

                    // partial connector, gen2 to gen1 [horz. line]
                    gr.DrawLine(connPen, left, top + BOXH / 2, gen2Togen1LineLeft, top + BOXH / 2);

                    DrawAnc(i, gr, box2Rect);
                    gen3top += gen3step + GEN3VM;
                }

                // TODO refactor drawing gen 1: duplicated code [at least one copy-pasta error has occurred]

                // 5. draw gen 1. The boxes are inset between the two boxes of
                // gen 2.
                right = left + boxSz1.X / 3;
                left = right - boxSz1.X;

                int top1 = OuterMargin + gen3step / 2 - BOXH / 2; // top of 1st gen2
                int high = BOXH + gen3step + GEN3VM;
                int cent1 = top1 + high / 2;
                top = cent1 - BOXH / 2;
                // TODO verify there is enough vertical room? at what time?
                Rectangle box1Rect = new Rectangle(left, top, boxSz1.X, BOXH);
                DrawAnc(2, gr, box1Rect);
                // complete connectors, gen2 to gen1 [vertical line]
                gr.DrawLine(connPen, gen2Togen1LineLeft, top1 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top1 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn2Y = top + BOXH / 2;
                int conn2X = left - boxSz1.X / 4;
                gr.DrawLine(connPen, left, conn2Y, conn2X, conn2Y);

                int top2 = top1 + high + gen3step + GEN3VM - BOXH;
                top = top2 + high / 2 - BOXH / 2;
                box1Rect.Location = new Point(left, top);
                DrawAnc(3, gr, box1Rect);
                // complete connectors, gen2 to gen1
                gr.DrawLine(connPen, gen2Togen1LineLeft, top2 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top2 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn3X = left - boxSz1.X / 4;
                int conn3Y = top + BOXH / 2;
                gr.DrawLine(connPen, left, conn3Y, conn3X, conn3Y);

                // 6. Draw gen 0.
                int top0 = maxH / 2 - BOXH / 2;
                int left0 = left - (int)(0.75 * boxSz0.X);
                Rectangle box0Rect = new Rectangle(left0, top0, boxSz0.X, BOXH);
                DrawAnc(1, gr, box0Rect);
                // finish connectors, gen1 to gen0
                gr.DrawLine(connPen, conn2X, top0 - 1, conn2X, conn2Y - 1);
                gr.DrawLine(connPen, conn3X, top0 + BOXH + 1, conn3X, conn3Y + 1);

            }
            return bmp;
        }

        /// <summary>
        /// Common code drawing an ancestor box. Draws the box, name, text.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="gr"></param>
        /// <param name="boxRect"></param>
        private void DrawAnc(int i, Graphics gr, Rectangle boxRect)
        {
            _hitRect[i] = boxRect;

            gr.DrawRectangle(_boxPen, boxRect);
            int left = boxRect.Left;
            int top = boxRect.Top;

            RectangleF textRect = boxRect;
            var nameLoc = new PointF(left, top + 3);
            textRect.Location = nameLoc;
            var nameSize = gr.MeasureString(AncData[i].Name, _nameFont);
            gr.DrawString(AncData[i].Name, _nameFont, _textBrush, nameLoc);
            var textLoc = new PointF(left + 2, top + 6 + nameSize.Height);
            textRect.Location = textLoc;
            gr.DrawString(AncData[i].Text, _textFont, _textBrush, textLoc);
        }

        /// <summary>
        /// Calculate how big the boxes must be to fit the text of a generation.
        /// I.e. for a set of ancestors, determine how wide the name/text would
        /// draw and return the widest, highest.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="ancL"></param>
        /// <param name="ancH"></param>
        /// <returns></returns>
        private Point CalcBoxDims(Graphics gr, int ancL, int ancH)
        {
            int maxW = 0;
            int maxH = 0;
            for (int i = ancL; i <= ancH; i++)
            {
                var nameSize = gr.MeasureString(AncData[i].Name, _nameFont);
                maxW = Math.Max(maxW, (int)nameSize.Width);
                var textSize = gr.MeasureString(AncData[i].Text, _textFont);
                maxW = Math.Max(maxW, (int)textSize.Width + TEXTINDENT);
                maxW = Math.Max(maxW, 150); // prevent collapsed boxes
                int totH = (int)(nameSize.Height + textSize.Height + TEXTSTEP);
                maxH = Math.Max(maxH, totH);
            }
            return new Point(maxW, maxH);
        }
    }
}
