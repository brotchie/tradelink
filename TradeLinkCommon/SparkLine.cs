using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// represent barlists as an efficiently sized graphic called a sparkline.
    /// see wikipedia for definition of a sparkline
    /// </summary>
    public class SparkLine
    {
        int h = 0;
        int w = 0;
        decimal high = 0;
        decimal low = 0;
        public SparkLine(int H, int W) { h = H; w = W; }

        public Image DrawBarListBlack(BarListImpl bl) { return DrawBarList(bl, BarInterval.FiveMin, Color.Black, Color.Black); }
        public Image DrawBarListBlack(BarListImpl bl, BarInterval bi) { return DrawBarList(bl, bi, Color.Black,Color.Black); }
        public Image DrawBarList(BarListImpl bl) { return DrawBarList(bl, BarInterval.FiveMin, Color.Green, Color.Red); }
        public Image DrawBarList(BarListImpl bl, BarInterval bi) { return DrawBarList(bl, bi, Color.Green, Color.Red); }
        public Image DrawBarList(BarListImpl bl, BarInterval bi,Color up, Color down)
        {
            bl.DefaultInterval = bi;
            Bitmap sl = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(sl);
            high = Calc.HH(bl);
            low = Calc.LL(bl);
            decimal range = high - low;
            int pixperdollar = range != 0 ? (int)(h / range) : 0;
            int pixperbar = bl.Count != 0 ? (int)(w / (decimal)bl.Count) : 0;
            for (int i = 0; i< bl.Count; i++)
            {
                Bar b = bl[i,bi];
                Pen p = new Pen(b.Close>=b.Open ? up : down);
                g.DrawLine(p, i * pixperbar, h-(int)((b.Low - low) * pixperdollar), i * pixperbar, h-(int)((b.High - low) * pixperdollar));
            }
            return sl;

        }
        public Image DrawLevel(decimal level) { return DrawLevel(level, Color.Black); }
        public Image DrawLevel(decimal level, Color color)
        {
            Bitmap sl = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(sl);
            g.Clear(Color.LightSeaGreen);
            decimal range = high - low;
            int pixperdollar = range != 0 ? (int)(h / range) : 0;
            int y = h-(int)((level-low)*pixperdollar);
            g.DrawLine(new Pen(color), 0, y, w, y);
            sl.MakeTransparent(Color.LightSeaGreen);
            return sl;
        }
    }





    /* OVERLAY TWO IMAGES
     * 
     * protected void Page_Load(object sender, EventArgs e)
{

string s = Server.MapPath("original.jpg");
string s2 = Server.MapPath("ikon.gif");

System.Drawing.Image original= Bitmap.FromFile(s);
Graphics gra = Graphics.FromImage(original);
Bitmap logo = new Bitmap(s2);
gra.DrawImage(logo, new Point(70, 70));

Response.ContentType = "image/JPEG";
big.Save(Response.OutputStream, ImageFormat.Jpeg);

}
     */


}
