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

        public Quote()
        {
            InitializeComponent();
            if ((Location.X == 0) && (Location.Y == 0))
            {
                Quotopia.Properties.Settings.Default.location = new Point(300, 300);
                Quotopia.Properties.Settings.Default.Save();
                Refresh();
            }
            
            Size = Quotopia.Properties.Settings.Default.wsize;
            show(Util.TLSIdentity());
            QuoteGridSetup();
            FetchTLServer();
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotIndexTick += new IndexDelegate(tl_gotIndexTick);
            tl.gotOrder += new OrderDelegate(tl_gotOrder);
            tl.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            ordergrid.ContextMenuStrip = new ContextMenuStrip();
            ordergrid.ContextMenuStrip.Items.Add("Cancel", null, new EventHandler(cancelorder));
            FormClosing += new FormClosingEventHandler(Quote_FormClosing);
        }

        void tl_gotOrderCancel(uint number)
        {
            if (ordergrid.InvokeRequired)
                ordergrid.Invoke(new UIntDelegate(tl_gotOrderCancel), new object[] { number });
            else
            {
                int oidx = orderidx(number); // get row number of this order in the grid
                if ((oidx > -1) && (oidx < ordergrid.Rows.Count)) // if row number is valid
                    ordergrid.Rows.RemoveAt(oidx); // remove the canceled order
            }
        }

        void cancelorder(object sender, EventArgs e)
        {
            for (int i = 0; i < ordergrid.SelectedRows.Count; i++)
            {
                uint oid = (uint)ordergrid["oid", ordergrid.SelectedRows[i].Index].Value;
                tl.CancelOrder((long)oid);
                show("Sending cancel for " + oid.ToString());
            }
        }

        void tl_gotOrder(Order o)
        {
            if (orderidx(o.id)==-1) // if we don't have this order, add it
                ordergrid.Rows.Add(new object[] { o.id, o.symbol, (o.Side ? "BUY" : "SELL"),o.UnSignedSize, (o.price == 0 ? "Market" : o.price.ToString()), (o.stopp==0 ? "" : o.stopp.ToString()), o.Account });
        }

        int orderidx(uint orderid)
        {
            for (int i = 0; i < ordergrid.Rows.Count; i++) // see if's an existing existing order
                if ((uint)ordergrid["oid", i].Value == orderid)
                    return i;
            return -1;
        }

        void Quote_FormClosing(object sender, FormClosingEventArgs e)
        {
            tl.Disconnect();
        }

        void FetchTLServer()
        {
            TLTypes servers = tl.TLFound();
            if (tl.Mode(servers, true))
            {
                string name = tl.BrokerName.ToString();
                status("Found TradeLink broker server: " + name);
                show("Found TradeLink broker server: " + name);
            }
            else status("Unable to find any broker instance.  Do you have one running?");
        }


        DataGrid qg = new DataGrid();
        DataTable qt = new DataTable();

        void QuoteGridSetup()
        {
            qt.Columns.Add("Symbol");
            qt.Columns.Add("Last");
            qt.Columns.Add("TSize");
            qt.Columns.Add("Bid");
            qt.Columns.Add("Ask");
            qt.Columns.Add("BSize");
            qt.Columns.Add("ASize");
            qt.Columns.Add("Sizes");
            qt.Columns.Add("AvgPrice");
            qt.Columns.Add("PosSize");
            qt.Columns.Add("High");
            qt.Columns.Add("Low");
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
            if (Index.isIdx(sym)) return;
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
                string err = Util.PrettyError(tl.BrokerName,res);
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
            if ((e.Clicks == 2) && (qg.CurrentRowIndex >= 0) && (qg.CurrentRowIndex < qg.VisibleRowCount))
            {
                rightticket(null, null);
            }
            else if ((e.Clicks == 1) && (qg.CurrentRowIndex >= 0) && (qg.CurrentRowIndex < qg.VisibleRowCount))
            {
                qg.Select(qg.CurrentRowIndex);
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
            else if ((e.KeyValue>=(int)Keys.A) && (e.KeyValue<=(int)Keys.Z))
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
            DataRow r = qt.Rows.Add(sym, "", "", "", "", "", "", "", "", "", "", "");
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
            if (qg.InvokeRequired)
                qg.Invoke(new TickDelegate(RefreshRow), new object[] { t });
            else
            {
                int[] rows = GetSymbolRows(t.sym);
                decimal high = tl.DayHigh(t.sym);
                decimal low = tl.DayLow(t.sym);
                for (int i = 0; i < rows.Length; i++)
                {
                    // last,size,bid/ask,sizes
                    // fetch OHLC from TL
                    // fetch position from TL
                    int r = rows[i];
                    if (t.isTrade)
                    {
                        qt.Rows[r]["Last"] = t.trade.ToString("N2");
                        if (t.size > 0) // make sure TSize is reported
                            qt.Rows[r]["TSize"] = t.size;
                    }
                    else if (t.FullQuote)
                    {

                        qt.Rows[r]["Bid"] = t.bid.ToString("N2");
                        qt.Rows[r]["Ask"] = t.ask.ToString("N2");
                        qt.Rows[r]["BSize"] = t.bs;
                        qt.Rows[r]["ASize"] = t.os;
                        qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + t.os.ToString();
                    }
                    else if (t.hasBid)
                    {
                        qt.Rows[r]["Bid"] = t.bid.ToString("N2");
                        qt.Rows[r]["BSize"] = t.bs;
                        string s = qt.Rows[r]["ASize"].ToString();
                        int os = (s != "") ? Convert.ToInt32(s) : 0;
                        qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + os.ToString();
                    }
                    else if (t.hasAsk)
                    {
                        qt.Rows[r]["Ask"] = t.ask.ToString("N2");
                        qt.Rows[r]["ASize"] = t.os;
                        string s = qt.Rows[r]["BSize"].ToString();
                        int bs = (s!="") ? Convert.ToInt32(s) : 0;
                        qt.Rows[r]["Sizes"] = bs.ToString() + "x" + t.os.ToString();
                    }
                    qt.Rows[r]["High"] = high.ToString("N2");
                    qt.Rows[r]["Low"] = low.ToString("N2");


                }
            }
        }


        void tl_gotFill(Trade t)
        {
            if (InvokeRequired)
                Invoke(new FillDelegate(tl_gotFill), new object[] { t });
            else
            {
                if (!t.isValid) return;
                int oidx = orderidx(t.id); // get order id for this order
                if (oidx != -1)
                {
                    int osign = (t.side ? 1 : -1);
                    int signedtsize = t.xsize * osign;
                    int signedosize = (int)ordergrid["osize", oidx].Value;
                    if (signedosize == signedtsize) // if sizes are same whole order was filled, remove
                        ordergrid.Rows.RemoveAt(oidx);
                    else // otherwise remove portion that was filled and leave rest on order
                        ordergrid["osize", oidx].Value = Math.Abs(signedosize - signedtsize) * osign;
                }
                int[] rows = GetSymbolRows(t.symbol);
                int size = tl.PosSize(t.symbol);
                decimal price = tl.AvgPrice(t.symbol);
                for (int i = 0; i < rows.Length; i++)
                {
                    qt.Rows[rows[i]]["PosSize"] = size.ToString();
                    qt.Rows[rows[i]]["AvgPrice"] = price.ToString("N2");
                }
                TradesView.Rows.Add(t.xdate, t.xtime, t.xsec, t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice.ToString("N2"), t.comment, t.Account.ToString()); // if we accept trade, add it to list
            }
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

        TradeLink_Client_WM tl = new TradeLink_Client_WM("quotopia.client",true);
        ~Quote() { QuotopiaClose(); }
        void QuotopiaClose()
        {
            tl.Disconnect(); // unregister all stocks and this client
        }


        void news_NewsEventSubscribers(Debug news)
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
