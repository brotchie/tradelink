using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLib
{
    /// <summary>
    /// Used to pass changes to barlists
    /// </summary>
    public delegate void BarListUpdated(BarList newbl);

    /// <summary>
    /// A generic charting form that plots BarList objects
    /// </summary>
    public partial class Chart : Form
    {
        public event StockDelegate FetchStock;
        BarList bl = null;
        public BarList Bars { get { return bl; } set { bl = value; } }
        string sym = "";
        public string Symbol { get { return sym; } set { sym = value; Text = Title; } }
        Graphics g = null;
        string mlabel = "";
        decimal highesth = 0;
        decimal lowestl = 10000000000000000000;
        int barc = 0;
        Rectangle r;
        const int border = 40;
        decimal pixperbar = 0;
        decimal  pixperdollar = 0;
        string newstock = "";
        List<TextLabel> points = new List<TextLabel>();
        public Chart() : this(null,false) { }
        public Chart(BarList b) : this(b, false) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="allowtype">if set to <c>true</c> [allowtype] will allow typing/changing of new symbols on the chart window.</param>
        public Chart(BarList b,bool allowtype)
        {
            InitializeComponent();
            bl = b;
            Symbol = b.Symbol;
            Paint += new PaintEventHandler(Chart_Paint);
            MouseWheel +=new MouseEventHandler(Chart_MouseUp);
            if (allowtype) this.KeyUp += new KeyEventHandler(Chart_KeyUp);
        }

        /// <summary>
        /// Provide this chart with a new barlist, and refresh the display.
        /// </summary>
        /// <param name="barlist">The barlist.</param>
        public void NewBarList(BarList barlist)
        {
            bl = barlist;
            Symbol = bl.Symbol;
            Text = Title;
            Refresh();
        }

        void Chart_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if ((e.KeyValue >= (int)Keys.A) && (e.KeyValue <= (int)Keys.Z)) newstock += e.KeyCode.ToString();
                if ((newstock.Length > 0) && (e.KeyValue == (int)Keys.Back)) newstock = newstock.Substring(0, newstock.Length - 1);
                this.Text = Title + " " + newstock;
            }
            catch (Exception) { }
            if (e.KeyValue == (int)Keys.Enter)
            {
                string stock = newstock.ToString();
                newstock = "";
                if (FetchStock != null) FetchStock(new Stock(stock));
                Refresh();
            }

            
        }
        // as we add bars and barindex gets higher, the xcoordinate moves right and gets higher
        int getX(int bar) { return (int)(bar * pixperbar) + (border/3); } 
        // as price goes up and pricemagnitude goes higher, the y coordinate moves up and goes lower
        int getY(decimal price) { return (int)(border+((highesth-price) * pixperdollar)); }

        /// <summary>
        /// Gets the title of this chart.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get { if (bl==null) return ""; return Symbol + " " + Enum.GetName(typeof(BarInterval), bl.Int).ToString(); } }
        void Chart_Paint(object sender, PaintEventArgs e)
        {
            if (bl != null)
            {
                highesth = BarMath.HH(bl);
                lowestl = BarMath.LL(bl);
                barc = bl.Count;
            }
            if ((bl == null)||(bl.Count==0)) return;
            Text = Title;
            r = ClientRectangle;
            pixperbar = (((decimal)r.Width - (decimal)border - ((decimal)border/3)) / (decimal)barc);
            pixperdollar = (((decimal)r.Height - (decimal)border * 2) / (highesth - lowestl));
            Pen p = new Pen(Color.Black);
            g = e.Graphics;
            Form f = (Form)sender;
            g.Clear(f.BackColor);

            Color fgcol = (f.BackColor == Color.Black) ? Color.White : Color.Black;

            // x-axis
            g.DrawLine(new Pen(fgcol),(int)(border/3), r.Height-border, r.Width - border, r.Height - border);
            // y-axis
            g.DrawLine(new Pen(fgcol), r.Width - border, r.Y + border, r.Width-border, r.Height - border);

            const int minxlabelwidth = 15;

            int lastlabelcoord = -500;

            for (int i = 0; i < barc; i++)
            {
                Color bcolor = (bl.Get(i).Close > bl.Get(i).Open) ? Color.Green : Color.Red;
                p = new Pen(bcolor);
                // draw high/low bar
                g.DrawLine(p, getX(i), getY(bl.Get(i).Low), getX(i), getY(bl.Get(i).High));
                // draw open bar
                g.DrawLine(p, getX(i), getY(bl.Get(i).Open), getX(i) - (int)(pixperbar / 3), getY(bl.Get(i).Open));
                // draw close bar
                g.DrawLine(p, getX(i), getY(bl.Get(i).Close), getX(i) + (int)(pixperbar / 3), getY(bl.Get(i).Close));
                // draw time labels (time @30min and date@noon)

                if (bl.Int != BarInterval.Day)
                {
                    if ((i % 6) == 0) g.DrawString(bl.Get(i).Bartime.ToString(), f.Font, new SolidBrush(fgcol), getX(i), r.Height - (f.Font.GetHeight() * 3));
                    if (bl.Get(i).Bartime == 1200) g.DrawString(bl.Get(i).Bardate.ToString(), f.Font, new SolidBrush(fgcol), getX(i), r.Height - (float)(f.Font.GetHeight() * 1.5));
                }
                else
                {
                    int[] date = BarMath.Date(bl.Get(i).Bardate);
                    int[] lastbardate = date;
                    if ((i-1)>0) lastbardate = BarMath.Date(bl.Get(i-1).Bardate);
                    if ((getX(lastlabelcoord) + minxlabelwidth) <= getX(i))
                    {
                        lastlabelcoord = i;
                        g.DrawString(date[2].ToString(), f.Font, new SolidBrush(fgcol), getX(i), r.Height - (f.Font.GetHeight() * 3));
                    }
                    if ((i == 0) || (lastbardate[1] != date[1]))
                    {
                        string ds = date[1].ToString();
                        if ((i==0)||(lastbardate[0]!=date[0])) ds += '/'+date[0].ToString();
                        g.DrawString(ds, f.Font, new SolidBrush(fgcol), getX(i), r.Height - (float)(f.Font.GetHeight() * 1.5));
                    }
                }
            }

            // DRAW YLABELS
            // max number of even intervaled ylabels possible on yaxis
            int numlabels = (int)((r.Height - border * 2) / (f.Font.GetHeight()*1.5));
            // nearest price units giving "pretty" even intervaled ylabels
            decimal priceunits = NearestPrettyPriceUnits(highesth - lowestl, numlabels);
            // starting price point from low end of range, including lowest low in barlist
            decimal lowstart = lowestl - ((lowestl * 100) % (priceunits * 100)) / 100;
            // original "non-pretty" price units calc
            //decimal priceunits = (highesth-lowestl)/numlabels;
            Pen priceline = new Pen(Color.BlueViolet);
            priceline.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            for (decimal i = 0;i<=numlabels; i++) 
            {
                decimal price = lowstart+(i*priceunits);
                g.DrawString(price.ToString("C"),f.Font,new SolidBrush(fgcol), r.Width-border,getY(price)-f.Font.GetHeight());
                g.DrawLine(priceline, border/3, getY(price), r.Width - border, getY(price));
            }
            DrawLabels();
        }

        private decimal NearestPrettyPriceUnits(decimal pricerange,int maxlabels)
        {
            decimal[] prettyunits = new decimal[] { .01m, .02m, .04m, .05m, .1m, .2m, .25m, .4m, .5m, 1,2,5,10 };
            for (int i = prettyunits.Length-1; i>=0; i--)
            {
                int numprettylabels = (int)(pricerange / prettyunits[i]);
                if (numprettylabels < maxlabels) continue;
                else return (i == (prettyunits.Length - 1)) ? prettyunits[prettyunits.Length - 1] : prettyunits[i + 1];
            }
            return prettyunits[0];
        }

        private void Chart_Resize(object sender, EventArgs e)
        {
            Refresh();
        }
        private void DrawLabels()
        {
            if (points == null) return;
            Graphics gd = CreateGraphics();
            Font font = new Font(FontFamily.GenericSerif, 14, FontStyle.Bold);
            for (int i = 0; i<points.Count; i++) gd.DrawString(points[i].Label, font, Brushes.Purple, points[i].X, points[i].Y);
        }

        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(new TextLabel(mlabel, e.X, e.Y));
            }
            else if (e.Button == MouseButtons.Middle) 
            {
                points.Clear();
            }
            Refresh();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mlabel = "2";
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mlabel = "1";
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mlabel = "0";
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        { //sell
            mlabel = "S";
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {// buy
            mlabel = "B";
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mlabel = "";
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            points.Clear();
            Refresh(); 

        }

        private void Chart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + Symbol + ".png";
            ScreenCapture sc = new ScreenCapture();
            sc.CaptureWindowToFile(Handle, s, System.Drawing.Imaging.ImageFormat.Png);

        }

        public void Chart_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Delta == 0) return;
            BarInterval old = bl.Int;
            BarInterval [] v = (BarInterval [])Enum.GetValues(typeof(BarInterval));
            int biord = 0;
            for (int i = 0;i<v.Length;i++) if (old==v[i]) biord = i;

            if (e.Delta > 0)
            {
                bl.Int = (biord + 1 < v.Length) ? v[biord + 1] : v[0];
            }
            else
            {
                bl.Int = (biord - 1 < 0) ? v[v.Length - 1] : v[biord - 1];
            }
            if ((bl.Int != old) && bl.Has(1)) NewBarList(this.bl);
        }
    }
    public class TextLabel
    {
        public TextLabel(string label, int xCoord, int yCoord) { tlabel = label; x = xCoord; y = yCoord; }
        string tlabel = "";
        int x = 0;
        int y = 0;
        public string Label { get { return tlabel; } }
        public int X { get { return x; } }
        public int Y { get { return y; } }
    }
}