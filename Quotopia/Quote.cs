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
        NewsService news = new NewsService();
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
            TLTypes servers = tl.TLFound();
            if (tl.Mode(servers, true))
            {
                status("Found TradeLink broker server: " + servers.ToString());
                show("Found TradeLink broker server: " + servers.ToString());
            }
            else status("Unable to find any broker instance.  Do you have one running?");
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
            qg.CaptionVisible = true;
            qg.RowHeadersVisible = false;
            qg.ColumnHeadersVisible = true;
            qg.Capture = true;
            qg.FlatMode = true;
            qg.ContextMenu = new ContextMenu();
            qg.ContextMenu.MenuItems.Add("Remove", new EventHandler(rightremove));
            qg.ContextMenu.MenuItems.Add("Chart", new EventHandler(rightchart));
            qg.ContextMenu.MenuItems.Add("Ticket", new EventHandler(rightticket));
            qg.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            qg.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;
            qg.HeaderBackColor = Quotopia.Properties.Settings.Default.colheaderbg;
            qg.HeaderForeColor = Quotopia.Properties.Settings.Default.colheaderfg;
            qg.GridLineColor = Quotopia.Properties.Settings.Default.gridcolor;
            qg.AlternatingBackColor = qg.BackColor;
            qg.ReadOnly = true;
            qg.DataSource = qt;
            qg.Parent = Markets;
            qg.Dock = DockStyle.Fill;
            qg.DoubleClick += new EventHandler(qg_DoubleClick);
            quoteTab.KeyUp +=new KeyEventHandler(qg_KeyUp);
            this.KeyUp += new KeyEventHandler(qg_KeyUp);
            qg.MouseUp += new MouseEventHandler(qg_MouseUp);
        }

        void qg_DoubleClick(object sender, EventArgs e)
        {
            rightticket(null, null);
        }

        void rightticket(object sender, EventArgs e)
        {
            string sym = GetVisibleStock(qg.CurrentRowIndex);
            Position me = tl.FastPos(sym);
            order o = new order(sym, me.Size, me.Side);
            o.neworder += new QuotopiaOrderDel(o_neworder);
            spillTick +=new TickDelegate(o.newTick);
            orderStatus+=new OrderStatusDel(o.orderStatus);
            System.Drawing.Point p = new System.Drawing.Point(MousePosition.X, MousePosition.Y);
            p.Offset(-315, 20);
            o.SetDesktopLocation(p.X, p.Y);
            o.Show();
        }

        void o_neworder(Order sendOrder)
        {
            int res = tl.SendOrder(sendOrder);
            if (res != 0)
            {
                string err = TradeLink.PrettyAnvilError(res);
                status(err);
                show(sendOrder.ToString() + "( " + err + " )");
            }
        }

        void rightremove(object sender, EventArgs e)
        {
            string sym = GetVisibleStock(qg.CurrentRowIndex);
            if (MessageBox.Show("Are you sure you want to remove "+sym+"?","Confirm remove",MessageBoxButtons.YesNo)== DialogResult.Yes)
            {
                qt.Rows.RemoveAt(qg.CurrentRowIndex);
            }
        }


        void rightchart(object sender, EventArgs e)
        {
            Point p = qg.PointToClient(MousePosition);
            DataGrid.HitTestInfo ht = qg.HitTest(p);
            if (ht.Type != DataGrid.HitTestType.Cell) return;
            if (ht.Row < 0) return;
            string sym = GetVisibleStock(qg.CurrentRowIndex);
            Chart c = new Chart();
            try
            {
                c = new Chart(bardict[sym]);
            }
            catch (Exception) { return; }
            c.Symbol = sym;
            c.Show();
        }


            
        void qg_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Clicks == 1) && (qg.CurrentRowIndex >= 0) && (qg.CurrentRowIndex < qg.VisibleRowCount))
            {
                qg.Select(qg.CurrentRowIndex);
                Markets.Select();
            }
        
        }

        string newsymbol = "";
        void qg_KeyUp(object sender, KeyEventArgs e)
        {
            string preface = "Adding symbol: ";
            if (newsymbol.Contains("$") || newsymbol.Contains("/")) 
                preface = "Adding index: ";
            if (e.KeyCode == Keys.Enter)
            {
                if (Stock.isStock(newsymbol))
                {
                    mb.Add(new Stock(newsymbol));
                    addsymbol(newsymbol);
                    newsymbol = "";
                    tl.Subscribe(mb);
                }
                else if (Index.isIdx(newsymbol))
                {
                    ib.Add(new Index(newsymbol));
                    addsymbol(newsymbol);
                    newsymbol = "";
                    tl.RegIndex(ib);
                }
                else
                {
                    status("Invalid stock or index.");
                    newsymbol = "";
                }
            }
            else if (e.KeyCode == Keys.OemQuestion)
            {
                newsymbol += "/";
                status(preface + newsymbol);
            }
            else if ((e.KeyCode == Keys.D4) && e.Shift)
            {
                newsymbol = "$";
                status(preface + newsymbol);
            }
            else if ((e.KeyCode == Keys.Escape) || (e.KeyCode == Keys.Delete))
            {
                newsymbol = "";
                status("Symbol add canceled...");
            }
            else if ((e.KeyCode == Keys.Back) && (newsymbol.Length>0))
            {
                newsymbol = newsymbol.Substring(0, newsymbol.Length - 1);
                status(preface + newsymbol);
            }
            else if ((e.KeyValue>(int)Keys.A) && (e.KeyValue<(int)Keys.Z))
            {
                newsymbol += (char)e.KeyValue;
                status("Adding symbol: " + newsymbol);
            }
        }

        Dictionary<string, BarList> bardict = new Dictionary<string, BarList>();
        void BarUpdate(Tick t) { if (bardict.ContainsKey(t.sym)) bardict[t.sym].AddTick(t); }

        void addsymbol(string sym)
        {
            // SYM,LAST,TSIZE,BID,ASK,BSIZE,ASIZE,SIZES,OHLC(YEST),CHANGE
            DataRow r = qt.Rows.Add(sym,0,0,0,0,0,0,"0x0",0,0,0,0,0);
            if (!bardict.ContainsKey(sym))
                bardict.Add(sym, new BarList(BarInterval.FiveMin, sym));
            qg.Select(qg.VisibleRowCount - 1); // selects most recently added symbol
            status("Added " + sym);
        }





        void tl_gotIndexTick(Index idx)
        {
            tl_gotTick(idx.ToTick());
            // archive index (just like ticks)
            // high/low coloring (just like ticks)
            // uptick/downtick coloring (just like ticks)
        }


        string GetVisibleStock(int row)
        {
            return ((row < 0) || (row >= qg.VisibleRowCount))
                ? "" : qg[row, 0].ToString();
        }
        int[] GetSymbolRows(string sym)
        {
            List<int> r = new List<int>();
            for (int i = 0; i < qt.Rows.Count; i++)
                if (qt.Rows[i]["Symbol"].ToString() == sym)
                    r.Add(i);
            return r.ToArray();
        }

        void RefreshRow(Tick t)
        {
            int[] rows = GetSymbolRows(t.sym);
            for (int i = 0; i<rows.Length; i++)
            {
                // last,size,bid/ask,sizes
                // fetch OHLC from TL
                // fetch position from TL
                int r = rows[i];
                if (t.isTrade)
                {
                    qt.Rows[r]["Last"] = t.trade;
                    if (t.size>0) // make sure TSize is reported
                        qt.Rows[r]["TSize"] = t.size;
                }
                else if (t.FullQuote)
                {

                    qt.Rows[r]["Bid"] = t.bid;
                    qt.Rows[r]["Ask"] = t.ask;
                    qt.Rows[r]["BSize"] = t.bs;
                    qt.Rows[r]["ASize"] = t.os;
                    qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + t.os.ToString();
                }
                else if (t.hasBid)
                {
                    qt.Rows[r]["Bid"] = t.bid;
                    qt.Rows[r]["BSize"] = t.bs;
                    int os = (int)qt.Rows[r]["ASize"];
                    qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + os.ToString();
                }
                else if (t.hasAsk)
                {
                    qt.Rows[r]["Ask"] = t.ask;
                    qt.Rows[r]["ASize"] = t.os;
                    int bs = (int)qt.Rows[r]["BSize"];
                    qt.Rows[r]["Sizes"] = bs.ToString() + "x" + t.os.ToString();
                }
                else return;
            }




        }


        void tl_gotFill(Trade t)
        {
            if ((t.symbol == null) || (t == null)) return;
            TradesView.Rows.Add(t.xdate, t.xtime, t.xsec, t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice, t.comment); // if we accept trade, add it to list
        }

        void tl_gotTick(Tick t)
        {
            BarUpdate(t);
            if (spillTick != null) spillTick(t);
            RefreshRow(t);
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

        MarketBasket mb = new MarketBasket();
        IndexBasket ib = new IndexBasket();


        private void importbasketbut_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = true;
            od.Filter = "Basket (*.mb)|*.mb|All files (*.*)|*.*";
            od.AddExtension = true;
            od.DefaultExt = ".mb";
            od.CheckFileExists = true;
            od.Title = "Select the baskets you wish to import to quototpia";
            if (od.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in od.FileNames)
                {
                    StreamReader sr = new StreamReader(f);
                    string line = sr.ReadLine();
                    sr.Close();
                    string[] r = line.Split(',');
                    int skipped = 0;
                    for (int i = 0; i < r.Length; i++)
                    {
                        bool add = true;
                        if (Stock.isStock(r[i]))
                            mb.Add(new Stock(r[i]));
                        else if (Index.isIdx(r[i]))
                            ib.Add(new Index(r[i]));
                        else { add = false; skipped++; }
                        if (add) addsymbol(r[i]);
                    }
                    status("Imported " + (r.Length - skipped) + " instruments.");
                }
            }
            tl.Subscribe(mb);
            tl.RegIndex(ib);
        }

    }




}
