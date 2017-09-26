using System;
using System.Drawing;
using System.Drawing.Printing;

// TODO seriously consider reworking diagrams to be drawn left-to-right
using GEDWrap;

namespace DrawAnce
{
    public class Draw4Gen : DrawGen
    {
        private const int GEN3VM = 15; // vertical margin between boxes for Gen 3
        private const int OuterMargin = 5;
        private const int GEN2HM = 46;

        private const int NAME_V_MARGIN = 3; // change when the boxPen thickness changes
        private const int TEXTSTEP = 4;
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

        private Point[] boxSz;
        private int BOXH;

        private void CalcActualBoxWidths(Graphics gr)
        {
            boxSz = new Point[4];
            boxSz[0] = CalcBoxDims(gr, 1, 1);
            boxSz[1] = CalcBoxDims(gr, 2, 3);
            boxSz[2] = CalcBoxDims(gr, 4, 7);
            boxSz[3] = CalcBoxDims(gr, 8, 15);

            // Take the tallest content height into account. For reasons I don't get yet,
            // drawing only works well if the boxes are all the same height.
            BOXH = 80;
            foreach (var point in boxSz)
            {
                BOXH = Math.Max(BOXH, point.Y);
            }
        }

        private void DrawAncTree(Graphics gr, Rectangle bounds)
        {
            int midline = 0;
            int maxW = bounds.Right;

            using (Pen connPen = new Pen(Color.Black, 2.0f))
            using (_sf = new StringFormat())
            {
                //Pen linePen = new Pen(Color.Blue, 1.0f);

                //{
                //    int mid = bounds.Top + bounds.Height/2;
                //    gr.DrawLine(linePen, bounds.Left, mid, bounds.Right, mid);
                //}

                //                _sf.Trimming = StringTrimming.EllipsisCharacter;
                _sf.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;

                //Point boxSz = calcBoxDims(gr, 8, 15);

                // 3. draw gen 3 on right side
                int right = maxW - MoreGenW; // maxW - OuterMargin - MoreGenW;
                int left = right - boxSz[3].X;
                int top = OuterMargin + bounds.Top;
                Rectangle box3Rect = new Rectangle(left, top, boxSz[3].X, BOXH);
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

                    if (i == 11)
                    {
                        midline = top + BOXH + GEN3VM / 2; // this was the easiest way to figure out the position of the Gen0 box...
                                                           //gr.DrawLine(linePen, bounds.Left, midline, bounds.Right, midline);
                    }
                    top += BOXH + GEN3VM;
                    box3Rect.Location = new Point(left, top);
                }

                // 4. draw gen 2. each box is centered vertically against
                // the two boxes to the right in gen 3. The boxes are stepped
                // left as a separate column to allow drawing connectors.
                int gen3step = 2 * BOXH + GEN3VM;
                right = left - GEN2HM;
                left = right - boxSz[2].X;
                int gen3top = OuterMargin + bounds.Top;
                Rectangle box2Rect = new Rectangle(left, top, boxSz[2].X, BOXH);
                int gen2Togen1LineLeft = left - boxSz[1].X / 3;
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
                right = left + boxSz[1].X / 3;
                left = right - boxSz[1].X;

                int top1 = bounds.Top + OuterMargin + gen3step / 2 - BOXH / 2; // top of 1st gen2
                int high = BOXH + gen3step + GEN3VM;
                int cent1 = top1 + high / 2;
                top = cent1 - BOXH / 2;
                // TODO verify there is enough vertical room? at what time?
                Rectangle box1Rect = new Rectangle(left, top, boxSz[1].X, BOXH);
                DrawAnc(2, gr, box1Rect);
                // complete connectors, gen2 to gen1 [vertical line]
                gr.DrawLine(connPen, gen2Togen1LineLeft, top1 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top1 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn2Y = top + BOXH / 2;
                int conn2X = left - boxSz[1].X / 4;
                gr.DrawLine(connPen, left, conn2Y, conn2X, conn2Y);

                int top2 = top1 + high + gen3step + GEN3VM - BOXH;
                top = top2 + high / 2 - BOXH / 2;
                box1Rect.Location = new Point(left, top);
                DrawAnc(3, gr, box1Rect);
                // complete connectors, gen2 to gen1
                gr.DrawLine(connPen, gen2Togen1LineLeft, top2 + BOXH / 2 - 1, gen2Togen1LineLeft, top - 1);
                gr.DrawLine(connPen, gen2Togen1LineLeft, top + BOXH + 1, gen2Togen1LineLeft, top2 + high - BOXH / 2 + 1);

                // partial connector, gen1 to gen0
                int conn3X = left - boxSz[1].X / 4;
                int conn3Y = top + BOXH / 2;
                gr.DrawLine(connPen, left, conn3Y, conn3X, conn3Y);

                // 6. Draw gen 0.
                int top0 = midline - BOXH / 2; // bounds.Top + finalGen3Bottom / 2 - BOXH / 2;
                int left0 = 5; // 20161123 fixed left side: GEDCOM_Amssoms
                               //int left0 = left - (int)(0.5 * boxSz[0].X);
                               //if (left0 < bounds.Left)
                               //    left0 = bounds.Left; // TODO past bounds of printing?
                Rectangle box0Rect = new Rectangle(left0, top0, boxSz[0].X, BOXH);
                DrawAnc(1, gr, box0Rect);
                // finish connectors, gen1 to gen0
                gr.DrawLine(connPen, conn2X, top0 - 1, conn2X, conn2Y - 1);
                gr.DrawLine(connPen, conn3X, top0 + BOXH + 1, conn3X, conn3Y + 1);
            }
        }

        public override Image MakeAncTree()
        {
            using (_boxPen = new Pen(Color.Chocolate, 2.0f))
            using (_nameFont = new Font("Arial", 12))
            using (_textFont = new Font("Arial", 9))
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

                // 1. calc h/w. Assume 3 gen back. therefore, 8 boxes high plus margin. Width is calculated based on contents.
                int maxH = 8 * BOXH + 7 * GEN3VM + 2 * OuterMargin;
                int maxW = MoreGenW + 2 * OuterMargin + GEN2HM + boxSz[3].X + boxSz[2].X + 2 * boxSz[1].X / 3 + 3 * boxSz[0].X / 4;

                Bitmap bmp = new Bitmap(maxW, maxH);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(Color.Cornsilk);
                    DrawAncTree(gr, new Rectangle(0,0,maxW,maxH));
                }
                return bmp;
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

            if (AncData[i] == null || string.IsNullOrWhiteSpace(AncData[i].Name))
                return;

            RectangleF textRect = boxRect;
            var nameLoc = new PointF(left, top + NAME_V_MARGIN);
            textRect.Location = nameLoc;
            var nameSize = gr.MeasureString(AncData[i].Name, _nameFont);
            gr.DrawString(AncData[i].Name, _nameFont, _textBrush, nameLoc);
            var textLoc = new PointF(left + 2, nameLoc.Y + TEXTSTEP + (float)Math.Ceiling(nameSize.Height));
            textRect.Location = textLoc;
            gr.DrawString(GetText(AncData[i]), _textFont, _textBrush, textLoc);
        }

        private string GetShowString(Person anc, string tag, string prefix)
        {
            var even = anc.GetEvent(tag);
            if (even == null)
                return "";

            string val = even.Descriptor + " " + even.Place;
            if (string.IsNullOrWhiteSpace(val))
                return "";

            return prefix + val.Trim() + "\r\n";
        }

        private string GetText(Person anc)
        {
            if (anc.Indi == null)
                return "";
            string val1 = GetShowString(anc, "BIRT", "B: ");
            string val2 = GetShowString(anc, "DEAT", "D: ");
            string val3 = "";
            if (anc.Marriage != null)
            {
                val3 = string.Format("M: {0} {1}\r\n", anc.Marriage.Date, anc.Marriage.Place);
            }
            string val4 = GetShowString(anc, "CHR", "C: ");
            string val5 = GetShowString(anc, "OCCU", "O: ");
            return val1 + val4 + val3 + val2 + val5;
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
            int penW = (int) Math.Ceiling(_boxPen.Width); // take the box pen into account, otherwise draws over right text margin
            int maxW = 0;
            int maxH = 0;
            for (int i = ancL; i <= ancH; i++)
            {
                int h2;
                int h1 = h2 = 25;
                if (AncData[i] != null && !string.IsNullOrEmpty(AncData[i].Name))
                {
                    SizeF nameSize = gr.MeasureString(AncData[i].Name, _nameFont);
                    maxW = Math.Max(maxW, (int) Math.Ceiling(nameSize.Width) + penW); // round up: text margin too small
                    var textSize = gr.MeasureString(GetText(AncData[i]), _textFont);
                    maxW = Math.Max(maxW, (int) textSize.Width + TEXTINDENT);
                    h1 = (int)Math.Ceiling(nameSize.Height);
                    h2 = (int)Math.Ceiling(textSize.Height);
                }
                maxW = Math.Max(maxW, 150); // prevent collapsed boxes
                int totH = h1 + h2 + TEXTSTEP + 2 * NAME_V_MARGIN;
                maxH = Math.Max(maxH, totH);
            }
            return new Point(maxW, maxH);
        }

        void BeginPrint(object sender, PrintEventArgs e)
        {
        }

        void PrintPage(object sender, PrintPageEventArgs e)
        {
            //using (Pen dashed_pen = new Pen(Color.Red, 5))
            //{
            //    dashed_pen.DashPattern = new float[] { 10, 10 };
            //    e.Graphics.DrawRectangle(dashed_pen, e.MarginBounds);
            //}

            using (_boxPen = new Pen(Color.Chocolate, 2.0f))
            using (_nameFont = new Font("Arial", 12))
            using (_textFont = new Font("Arial", 9))
            using (_textBrush = new SolidBrush(Color.Black))
            {
                CalcActualBoxWidths(e.Graphics);
                DrawAncTree(e.Graphics, e.MarginBounds);
            }

            e.HasMorePages = false; //(Lines < LINES_TO_PRINT);
        }

        private void QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
        }

        private void EndPrint(object sender, PrintEventArgs e)
        {
        }

    }
}
