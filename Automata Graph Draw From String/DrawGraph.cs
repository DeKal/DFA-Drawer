using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automata_Graph_Draw_From_String
{

    public partial class DFADrawer : Form
    {
        private string input;
        private int[][] trace;
        private string[] str;
        private Point[] bottomOfVertex, rightMostOfVertex, leftMostOfVertex;
        private int n;
        private const int VERTEX_AT_X = 120;
        private const int VERTEX_AT_Y = 200;
        private const int DIAMETER_OF_VERTEX = 70;
        private const int SPACE_DOUBLE_CIRCLE = 5;
        private const int GAP = 20;
        private Bitmap bitmap, original;
        private bool isInputChanged = false;
        public DFADrawer()
        {
            InitializeComponent();
            this.vScrollBar1.Maximum = graph.Height + this.ClientSize.Height;
            trackBar1.TickStyle = TickStyle.Both;

            // Set the minimum and maximum number of ticks.
            trackBar1.Minimum = 10;
            trackBar1.Maximum = 100;
            trackBar1.Value = 100;
            // Set the tick frequency to one tick every ten units.
            trackBar1.TickFrequency = 10;

            // Associate the event-handling method with the 
            // ValueChanged event.
           
        }

       

        private void DrawVertex(Graphics graphics, int x)
        {
           
              
            
            //calculate the x and y of the bottom of each vertex
            bottomOfVertex[x] = new Point(VERTEX_AT_X * x + GAP + DIAMETER_OF_VERTEX / 2, VERTEX_AT_Y + DIAMETER_OF_VERTEX);
            leftMostOfVertex[x] = new Point(VERTEX_AT_X * x + GAP, VERTEX_AT_Y + DIAMETER_OF_VERTEX / 2);
            rightMostOfVertex[x] = new Point(VERTEX_AT_X * x + GAP + DIAMETER_OF_VERTEX, VERTEX_AT_Y + DIAMETER_OF_VERTEX / 2);
            
            if (x == n) return;
            
            //Smooth the line
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //Draw a eclipse
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(VERTEX_AT_X * x + GAP, VERTEX_AT_Y, DIAMETER_OF_VERTEX
                , DIAMETER_OF_VERTEX);

            SolidBrush brush = new SolidBrush(Color.Black);
            Pen myPen = new Pen(Color.White,3);
            graphics.FillEllipse(brush, rectangle);
            graphics.DrawEllipse(myPen, rectangle);
            
            if (x==n-1)
                graphics.DrawEllipse(myPen, new Rectangle( VERTEX_AT_X*x+GAP + SPACE_DOUBLE_CIRCLE, VERTEX_AT_Y + SPACE_DOUBLE_CIRCLE, 
                    DIAMETER_OF_VERTEX - SPACE_DOUBLE_CIRCLE*2 , DIAMETER_OF_VERTEX - SPACE_DOUBLE_CIRCLE*2));
                
            
            //Write Text
            SolidBrush myBrush = new SolidBrush(Color.White);
            
            Font myFont = new Font("Times New Roman", 24);
            //Format String
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            graphics.DrawString(x.ToString(), myFont, myBrush, rectangle,stringFormat);
           
  
        }
        private void DrawDFAGraph(Graphics g)
        {
            bottomOfVertex = new Point[n + 1];
            rightMostOfVertex = new Point[n + 1];
            leftMostOfVertex = new Point[n + 1];
            for (int i = 0; i <= n; ++i)
                DrawVertex(g , i);
            currentCurve = 0;
            for (int i = 0; i < n ; ++i)
                {
                    DrawLine(g, i, trace[i][0], 0);
                    DrawLine(g, i, trace[i][1], 1);
                }
            currentCurve = 0;
            for (int i = 0; i < n ; ++i)
            {

                InsertStringToDFA(g, i, trace[i][0], 0);
                InsertStringToDFA(g, i, trace[i][1], 1);
            }
        }

        private void InsertStringToDFA(Graphics graphics, int a, int b, int p2)
        {
            
            //Smooth the line
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            SolidBrush myBrush = new SolidBrush(Color.White);

            Font myFont = new Font("Times New Roman", 17, FontStyle.Bold);

            //Format String
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            
            if (a < b)
            {
                Rectangle tmp = new Rectangle((rightMostOfVertex[a].X + leftMostOfVertex[b].X) / 2 - 15, rightMostOfVertex[a].Y - 25, 20, 20);
                graphics.FillRectangle(new SolidBrush(Color.Black) , tmp);
                graphics.DrawString(p2.ToString(), myFont, myBrush,
                    tmp , stringFormat);
            }
            else if (a > b)
            {
                
                Point tmp = new Point((bottomOfVertex[a].X + bottomOfVertex[b].X) / 2 -10, bottomOfVertex[a].Y + 60 + currentCurve++ * 20);
                graphics.FillRectangle(new SolidBrush(Color.Black) , new Rectangle(tmp, new Size(20, 20)));
                int temp = currentCurve % 5;
                switch (temp)
                {
                    case 0:
                        myBrush.Color = Color.White;
                        break;
                    case 1:
                        myBrush.Color = Color.Red;
                        break;
                    case 2:
                        myBrush.Color = Color.Orange;
                        break;
                    case 3:
                        myBrush.Color = Color.Chocolate;
                        break;
                    default:
                        myBrush.Color = Color.Green;
                        break;
                }
                
                
                graphics.DrawString(p2.ToString(), myFont, myBrush,
                   new Rectangle(tmp, new Size(20, 20)), stringFormat);
                myBrush.Color = Color.White;
            }
            else
            {
                Rectangle rect = new Rectangle(bottomOfVertex[a].X -10, bottomOfVertex[a].Y - 2 * DIAMETER_OF_VERTEX - 20,20, 20);
                graphics.DrawString(p2.ToString(), myFont, myBrush,
                   rect, stringFormat);
            }
        }
        int currentCurve;
        private void DrawLine(Graphics graphics, int a, int b, int p2)
        {
            //Smooth the line
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //Adjust Arrow Line
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5); 
            Pen p = new Pen(Color.White, 3);
            p.CustomEndCap = bigArrow;
            
            if (a < b)
            {
                graphics.DrawLine(p, rightMostOfVertex[a], leftMostOfVertex[b]);           
            }
            else if (a > b)
            {
                
                //Draw curve ....
                Point[] tmp = new Point[3];
                tmp[0] = bottomOfVertex[a];
                tmp[1] = new Point((bottomOfVertex[a].X + bottomOfVertex[b].X) / 2, bottomOfVertex[a].Y + 50 + currentCurve++*20);
                tmp[2] = bottomOfVertex[b];
                int temp = currentCurve %5;
                switch (temp)
                {
                    case 0: 
                        p.Color = Color.White;
                        break;
                    case 1:
                        p.Color = Color.Red;
                        break;
                    case 2:
                        p.Color = Color.Orange;
                        break;
                    case 3:
                        p.Color = Color.Chocolate;
                        break;
                    default:
                        p.Color = Color.Green;
                        break;
                }
                
                graphics.DrawCurve(p, tmp);
                p.Color = Color.White;
            }
            else
            {
                Rectangle rect = new Rectangle(bottomOfVertex[a].X - 35, bottomOfVertex[a].Y - 2*DIAMETER_OF_VERTEX
                    , DIAMETER_OF_VERTEX, DIAMETER_OF_VERTEX);

                // Create start and sweep angles on ellipse.
                float startAngle = 90F;
                float sweepAngle = 359F;

                // Draw arc to screen.
                graphics.DrawArc(p, rect, startAngle, sweepAngle);

            }

        }

        private int findBackVertex(int start, int value )
        {
            if (start == 0) return 0;
            else
            {
                string str1 = str[start];
                string str2 = str[start] + value;
                int count = start;
                while (str2.Length > 0)
                {
                    str2 = str2.Substring(1);
                    if (str2.CompareTo(str1) == 0)
                        break;
                    else
                        str1 = str1.Substring(0, str1.Length - 1);
                    count--;
                }
                return count;
            }
        }
        string oldInput ="";
        private void tb_input_TextChanged(object sender, EventArgs e)
        {
            
            string input = tb_input.Text.ToString();
            for (int i = 0; i < input.Length; ++i)
                if (tb_input.Text[i] - '0' < 0 || tb_input.Text[i] - '0' > 1) {
                    input = input.Remove(i, 1);
                    i--;
                }
            tb_input.Text = input;
            if (tb_input.Text.Length > 0)
                tb_input.SelectionStart = tb_input.Text.Length;
            else
                tb_input.SelectionStart = 0;

            if (oldInput.CompareTo(input) == 0) return;
            oldInput = input;
            isInputChanged = true;

            hScrollBar1.Maximum = Math.Max(tb_input.Text.Length * 120 + 100 + 200 - this.ClientSize.Width, 0);
            bitmap = new Bitmap(tb_input.Text.Length * 120 + 100 ,graph.Size.Height
                ,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bitmap);
            Redraw(g);
            graph.Invalidate();
        }
        void Redraw(Graphics g)
        {
            input = tb_input.Text.ToString();
            if (input.CompareTo("") == 0) return;
            n = input.Length + 1;
            trace = new int[n ][];
            str = new string[n + 1];
            str[0] = "";

            for (int i = 1; i < n; ++i)
                str[i] = str[i - 1] + input[i - 1];
            str[n] = str[n-1] + "0";

            for (int i = 0; i < n; ++i)
                trace[i] = new int[2];

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < 2; ++j)
                    trace[i][j] = -1;
            for (int i = 0; i < n - 1 ; ++i)
            {
                int tmp = (input[i] == '0') ? 0 : 1;
                trace[i][tmp] = i + 1;
                
                tmp = (tmp == 0) ? 1 : 0;
                trace[i][tmp] = findBackVertex(i, tmp);
            }

            trace[n - 1][1] = findBackVertex(n - 1, 1);
            trace[n - 1][0] = findBackVertex(n - 1, 0);
            
            
            DrawDFAGraph(g);
            
            original = new Bitmap(bitmap, bitmap.Size);
            
        }
        private void graph_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmap == null) return;
            using (Graphics g = e.Graphics)
            {
                
                if (trackBar1.Value != 100 && isInputChanged)
                    bitmap = ResizeImage(original, (int)Math.Ceiling(original.Width * trackBar1.Value / 100.0),
                               (int)Math.Ceiling(original.Height * trackBar1.Value / 100.0));
                
                Rectangle rect = new Rectangle(-hScrollBar1.Value, -vScrollBar1.Value, bitmap.Width, bitmap.Height);
                g.DrawImage(bitmap,rect);
                
                
            }
        }
     
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            if (e.Type == ScrollEventType.EndScroll)
            {
                isInputChanged = false;
                graph.Refresh();
                
            }
            //this.graph.Left = -this.hScrollBar1.Value;
        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                isInputChanged = false;
                //this.graph.Top = -this.vScrollBar1.Value;
                graph.Refresh();
            }
        }
        

        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void DFADrawer_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = trackBar1.Value.ToString();
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            if (original != null)
            {
                bitmap = ResizeImage(original, (int)Math.Ceiling(original.Width * trackBar1.Value / 100.0),
                        (int)Math.Ceiling(original.Height * trackBar1.Value / 100.0));

                graph.Refresh();

                //bitmap.Dispose();
            }
        }

        Point mouseDownLocation;
        private void graph_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownLocation = new System.Drawing.Point(e.X, e.Y);
        }

        private void graph_MouseUp(object sender, MouseEventArgs e)
        {
            Point mouseUpLocation = new Point(e.Y, e.Y);
            if (!mouseUpLocation.Equals(mouseDownLocation))
            {
                if (mouseDownLocation.Y - e.Y +vScrollBar1.Value <0)
                    vScrollBar1.Value = 0;
                else if (mouseDownLocation.X - e.X + vScrollBar1.Value > vScrollBar1.Maximum)
                    vScrollBar1.Value = vScrollBar1.Maximum;
                else 
                    vScrollBar1.Value += (mouseDownLocation.Y - e.Y);
                if (mouseDownLocation.X - e.X + hScrollBar1.Value < 0)
                    hScrollBar1.Value = 0;
                else if (mouseDownLocation.X - e.X + hScrollBar1.Value > hScrollBar1.Maximum)
                    hScrollBar1.Value = hScrollBar1.Maximum;
                else
                    hScrollBar1.Value += (mouseDownLocation.X - e.X);
                isInputChanged = false;
                graph.Refresh();
            }

        }
       
    }
   
}
