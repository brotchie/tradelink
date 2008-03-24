using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TradeLib;

namespace Quotopia
{
    public delegate void OrderStatusDel(string sym, int error);
    public partial class Quote : Form
    {

        public int GetDate { get { DateTime d = DateTime.Now; int i = (d.Year * 10000) + (d.Month * 100) + d.Day; return i; } }
        public int GetTime { get { DateTime d = DateTime.Now; int i = (d.Hour * 100) + (d.Minute); return i; } }
        public event TickDelegate spillTick;
        public event OrderStatusDel orderStatus;
        const string version = "2.0";
        const string build = "$Rev: 998 $";
        string Ver { get { return version + "." + Util.CleanVer(build); } }


        public Quote()
        {
            InitializeComponent();
            Size = Quotopia.Properties.Settings.Default.wsize;
            show("Quotopia" + Ver + " (Tradelink" + tl.Ver + ")");

            // figure out the name of this window
            string name = "Quotopia";
            if (TradeLink_WM.Found(name))
            {
                int inst = -1;
                do
                {
                    inst++;
                } while (TradeLink_WM.Found(name + "." + inst.ToString()));
                name += "." + inst.ToString();
            }
            this.Text = name;
            tl.MeH = this.Handle; // gives our window handle to tradelink
            tl.Me = this.Text; // gives tradelink our window name

            QuoteGridSetup();
            FetchTLServer();
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotIndexTick += new IndexDelegate(tl_gotIndexTick);
            news.NewsEventSubscribers += new NewsDelegate(news_NewsEventSubscribers);
        }

        void FetchTLServer()
        {
            try
            {
                TLTypes mode = (TLTypes)Properties.Settings.Default.tlmode;

                // we need to add a method to TradeLib.TradeLink_WM that will
                // take a TLTypes and set the appropriate client mode
                // ie:
                // bool tl.MyMode(TlTypes changeto, bool throwonmissing, bool showdialogue)
             
            }
            catch (TLServerNotFound)
            {
             
            }


        }


        DataGrid qg = new DataGrid();
        DataTable qt = new DataTable();

        void QuoteGridSetup()
        {
            qt.Columns.Add("Symbol", "".GetType());
            qt.Columns.Add("Last", new Decimal().GetType());
            qt.Columns.Add("TSize", new Int32().GetType());
            qt.Columns.Add("Bid", new Decimal().GetType());
            qt.Columns.Add("Ask", new Decimal().GetType());
            qt.Columns.Add("BSize", new Int32().GetType());
            qt.Columns.Add("ASize", new Int32().GetType());
            qt.Columns.Add("Sizes", "".GetType());
            qt.Columns.Add("Open", new Decimal().GetType());
            qt.Columns.Add("High", new Decimal().GetType());
            qt.Columns.Add("Low", new Decimal().GetType());
            qt.Columns.Add("YestClose", new Decimal().GetType());
            qt.Columns.Add("Change", new Decimal().GetType());
            qg.CaptionVisible = false;
            qg.RowHeadersVisible = false;
            qg.ColumnHeadersVisible = true;
            qg.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            qg.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;
            qg.HeaderBackColor = Quotopia.Properties.Settings.Default.colheaderbg;
            qg.HeaderForeColor = Quotopia.Properties.Settings.Default.colheaderfg;
            qg.GridLineColor = Quotopia.Properties.Settings.Default.gridcolor;
            qg.ReadOnly = true;
            qg.DataSource = qt;
            qg.Parent = quoteTab;
            qg.Dock = DockStyle.Fill;
            qg.KeyUp += new KeyEventHandler(qg_KeyUp);
            qg.MouseUp += new MouseEventHandler(qg_MouseUp);
        }
            
        void qg_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Clicks==1) && (qg.CurrentRowIndex>=0) && (qg.CurrentRowIndex<qg.VisibleRowCount))
                qg.Select(qg.CurrentRowIndex);
        
        }

        string newsymbol = "";
        void qg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Stock.isStock(newsymbol) || Index.isIdx(newsymbol))
                {
                    addsymbol(newsymbol);
                    newsymbol = "";
                }
                else
                {
                    status("Invalid stock or index.");
                    newsymbol = "";
                }
            }
            else if ((e.KeyCode == Keys.Escape) || (e.KeyCode == Keys.Delete))
            {
                newsymbol = "";
                status("Symbol add canceled...");
            }
            else if (e.KeyCode == Keys.Back)
            {
                newsymbol = newsymbol.Substring(0, newsymbol.Length - 2);
                status("Adding symbol: " + newsymbol);
            }
            else
            {
                newsymbol += (char)e.KeyValue;
                status("Adding symbol: " + newsymbol);
            }
            e.Handled = true;
            // have to resubscribe here
        }
        void addsymbol(string sym)
        {
            DataRow r = qt.Rows.Add(sym);
            qg.Select(qg.VisibleRowCount - 1); // selects most recently added symbol
        }





        void tl_gotIndexTick(Index idx)
        {
            // archive index (just like ticks)
            // high/low coloring (just like ticks)
            // uptick/downtick coloring (just like ticks)
        }


        NewsService news = new NewsService();


        int[] GetSymbolRows(string sym)
        {
            List<int> r = new List<int>();
            for (int i = 0; i < qt.Rows.Count; i++)
                if (qt.Rows[i]["Symbol"].ToString() == sym)
                    r.Add(i);
            return r.ToArray();
        }

        void RefreshRow(int r, Tick t)
        {

        }

        void tl_gotFill(Trade t)
        {
            if ((t.symbol == null) || (t == null)) return;
            TradesView.Rows.Add(t.xdate, t.xtime, t.xsec, t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice, t.comment); // if we accept trade, add it to list
        }

        void tl_gotTick(Tick t)
        {
            // bars
            // boxinfo
            // trades boxes
            // spilltick to order tickets
            // archive
            // watch ticks
            // color high/low
            // color uptick/downtick
            // write trends
            // write consolidated order-sizes
        }

        TradeLink_WM tl = new TradeLink_WM();
        ~Quote() { QuotopiaClose(); }
        void QuotopiaClose()
        {
            tl.Disconnect(); // unregister all stocks and this client
        }


        void ReSubscribe() { ReSubscribe(true); }
        void ReSubscribe(bool dosubscribe)
        {
            // have to resubscribe with TLServer 
            // when new stocks are entered/imported or deleted

            // don't have to resubscribe if stocks are moved or reordered
        }


        protected override void WndProc(ref Message m)
        {
            tl.GotWM_Copy(ref m);
            base.WndProc(ref m);
        }

        void news_NewsEventSubscribers(News news)
        {
            showc(news.Msg, Color.Blue);
        }
        delegate void SetTextCallback(string text);
        delegate void SetTextCallbackColor(string text, Color color);
        public void showc(string s, Color c)
        {
            statusWindow.SelectionColor = c;
            statusWindow.SelectedText = s + Environment.NewLine;
            statusWindow.SelectionColor = statusWindow.ForeColor;

        }
        public void show(string s)
        {
            if (this.statusWindow.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(show);
                this.Invoke(d, new object[] { s });
            }
            else statusWindow.AppendText(s + Environment.NewLine);
        }
        public void status(string s)
        {
            statuslab.Text = s;
        }

        private void importbasketbut_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = true;
            od.Filter = "MarketBasket (*.mb)|*.mb|All files (*.*)|*.*";
            od.AddExtension = true;
            od.DefaultExt = ".mb";
            od.CheckFileExists = true;
            od.Title = "Select the baskets you wish to import to quototpia";
            if (od.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in od.FileNames)
                {
                    StreamReader sr = new StreamReader(f);

                }
            }
        }

    }




}
