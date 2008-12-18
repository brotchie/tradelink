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
            statfade.Interval = 5000;
            statfade.Tick += new EventHandler(statfade_Tick);
            statfade.Start();
            ticker.Resolution = 500;
            ticker.Period = 100;
            ticker.Mode = Multimedia.TimerMode.Periodic;
            ticker.Tick += new EventHandler(ticker_Tick);
            ticker.Start();
            
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotOrder += new OrderDelegate(tl_gotOrder);
            tl.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            tl.gotPosition += new PositionDelegate(tl_gotPosition);
            tl.gotAccounts += new DebugDelegate(tl_gotAccounts);
            ordergrid.ContextMenuStrip = new ContextMenuStrip();
            ordergrid.ContextMenuStrip.Items.Add("Cancel", null, new EventHandler(cancelorder));
            FormClosing += new FormClosingEventHandler(Quote_FormClosing);
            tl.RequestAccounts();

        }

        string[] accts;

        void tl_gotAccounts(string msg)
        {
            accts = msg.Split(',');
            
        }

        void tl_gotPosition(Position pos)
        {
            Position p = null;
            if (posdict.TryGetValue(pos.Symbol, out p))
                posdict[pos.Symbol] = pos;
            else
                posdict.Add(pos.Symbol, pos);
            int[] rows = new int[0];
            if (symidx.TryGetValue(pos.Symbol, out rows))
            {
                foreach (int r in rows)
                {
                    qt.Rows[r]["AvgPrice"] = pos.AvgPrice.ToString("N2");
                    qt.Rows[r]["PosSize"] = pos.Size.ToString();
                }

            }
        }

        Dictionary<string, Position> posdict = new Dictionary<string, Position>();

        void ticker_Tick(object sender, EventArgs e)
        {
            // update the screen periodically
            for (int r = 0; r<ticks.Length; r++)
            {
                Tick t = ticks[r];
                if ((t==null) || !t.isValid) continue;
                if (spillTick != null) spillTick(t);
                RefreshRow(t);
            }
        }

        void statfade_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(laststat).TotalSeconds > 10)
                statusStrip1.Visible = false;
        }

        void ToggleCol(object sender, EventArgs e)
        {
            string col = ((ToolStripItem)sender).Text;
            if (!qg.Columns.Contains(col)) return;
            qg.Columns[col].Visible = !qg.Columns[col].Visible;
            qg.Refresh();
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
            try
            {
                tl.Unsubscribe();
                tl.Disconnect();
            }
            catch (TLServerNotFound) { }
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

        Timer statfade = new Timer();
        Multimedia.Timer ticker = new Multimedia.Timer();
        DataGridView qg = new DataGridView();
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
            qt.Columns.Add("Exch");
            qt.Columns.Add("BidEx");
            qt.Columns.Add("AskEx");
            qg.AllowUserToAddRows = false;
            qg.AllowUserToDeleteRows = false;
            qg.AllowUserToOrderColumns = true;
            qg.AllowUserToResizeColumns = true;
            qg.BackgroundColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            qg.RowHeadersVisible = false;
            qg.ColumnHeadersVisible = true;
            qg.Capture = true;
            qg.ContextMenuStrip = new ContextMenuStrip();
            qg.ContextMenuStrip.Items.Add("Remove", null,new EventHandler(rightremove));
            qg.ContextMenuStrip.Items.Add("Chart", null,new EventHandler(rightchart));
            qg.ContextMenuStrip.Items.Add("Ticket", null,new EventHandler(rightticket));
            qg.ContextMenuStrip.Items.Add("Import Basket", null,new EventHandler(importbasketbut_Click));
            qg.BackgroundColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            qg.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;
            qg.DefaultCellStyle.BackColor = qg.BackgroundColor;
            qg.DefaultCellStyle.ForeColor = qg.ForeColor;
            qg.RowHeadersDefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.colheaderbg;
            qg.RowHeadersDefaultCellStyle.ForeColor = Quotopia.Properties.Settings.Default.colheaderfg;
            qg.AlternatingRowsDefaultCellStyle.BackColor = qg.BackgroundColor;
            qg.AlternatingRowsDefaultCellStyle.ForeColor = qg.ForeColor;
            qg.GridColor = Quotopia.Properties.Settings.Default.gridcolor;
            qg.MultiSelect = false;
            qg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            qg.ReadOnly = true;
            qg.DataSource = qt;
            qg.Parent = Markets;
            qg.Dock = DockStyle.Fill;
            qg.DoubleClick += new EventHandler(qg_DoubleClick);
            quoteTab.KeyUp +=new KeyEventHandler(qg_KeyUp);
            this.KeyUp += new KeyEventHandler(qg_KeyUp);
            qg.MouseUp += new MouseEventHandler(qg_MouseUp);
            SetColumnContext();
        }

        void qg_DoubleClick(object sender, EventArgs e)
        {
            rightticket(null, null);
        }

        int CurrentRow { get { return (qg.SelectedRows.Count>0 ? qg.SelectedRows[0].Index : -1); } }

        void SetColumnContext()
        {

            ToolStripMenuItem dd = new ToolStripMenuItem("Columns");
            for (int i = 0; i < qg.Columns.Count; i++)
            {
                string col = qg.Columns[i].HeaderText;
                dd.DropDownItems.Add(col,null,new EventHandler(ToggleCol));
            }
            qg.ContextMenuStrip.Items.Add(dd);
        }

        void rightticket(object sender, EventArgs e)
        {
            Security s = GetVisibleSecurity(CurrentRow);
            if (s.Type == SecurityType.IDX) return;
            string sym = s.Symbol;
            Order o = new Order(sym,0);
            o.Exchange = s.DestEx;
            o.Security = s.Type;
            o.LocalSymbol = sym;
            Ticket t = new Ticket(o);
            t.SendOrder += new OrderDelegate(t_neworder);
            spillTick +=new TickDelegate(t.newTick);
            orderStatus+=new OrderStatusDel(t.orderStatus);
            System.Drawing.Point p = new System.Drawing.Point(MousePosition.X, MousePosition.Y);
            p.Offset(-315, 20);
            t.SetDesktopLocation(p.X, p.Y);
            t.Show();
        }

        void t_neworder(Order sendOrder)
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
            string sym = GetVisibleSecurity(CurrentRow).Symbol;
            if (MessageBox.Show("Are you sure you want to remove "+sym+"?","Confirm remove",MessageBoxButtons.YesNo)== DialogResult.Yes)
            {
                //remove the row
                qt.Rows.RemoveAt(CurrentRow);
                // clear current index
                symidx.Clear();
                // reget index
                symindex();
            }
        }


        void rightchart(object sender, EventArgs e)
        {
            string sym = GetVisibleSecurity(CurrentRow).Symbol;
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
            if ((e.Clicks == 2) && (CurrentRow >= 0) && (CurrentRow < qg.Rows.Count))
            {
                rightticket(null, null);
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
                Security sec = Security.Parse(newsymbol);
                if (sec.isValid)
                {
                    mb.Add(sec);
                    addsymbol(newsymbol);
                    newsymbol = "";
                    tl.Subscribe(mb);
                }
                else
                {
                    status("Invalid Security "+newsymbol);
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
            else if (((e.KeyValue>=(int)Keys.A) && (e.KeyValue<=(int)Keys.Z)) 
                || ((e.KeyValue>=(int)Keys.D0) && (e.KeyValue<=(int)Keys.D9)) || e.Shift || (e.KeyData== Keys.Space))
            {
                string val = "";
                if (e.Shift)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.D3: val = "#";
                            break;
                    }
                }
                else
                {
                    char v = (char)e.KeyValue;
                    val += v;
                }
                newsymbol += val;
                status("Adding symbol: " + newsymbol);
            }
        }

        Dictionary<string, BarList> bardict = new Dictionary<string, BarList>();

        void addsymbol(string sym)
        {
            // SYM,LAST,TSIZE,BID,ASK,BSIZE,ASIZE,SIZES,OHLC(YEST),CHANGE
            DataRow r = qt.Rows.Add(sym, "", "", "", "", "", "", "", "", "", "", "");
            if (!bardict.ContainsKey(sym))
                bardict.Add(sym, new BarList(BarInterval.FiveMin, sym));
            status("Added " + sym);
            symindex();
            tl.RequestPositions(accts!=null ? accts[0] : "");
        }

        
        Dictionary<string, int[]> symidx = new Dictionary<string, int[]>();

        void symindex()
        {
            for (int i = 0; i < qt.Rows.Count; i++)
            {
                if (qt.Rows[i].RowState == DataRowState.Deleted) continue;
                Security sec = Security.Parse(qt.Rows[i]["Symbol"].ToString());
                string sym = sec.Symbol;
                if (!symidx.ContainsKey(sec.FullName)) // if we've not seen this symbol add it's index
                    symidx.Add(sec.FullName, new int[] { i });
                else
                {
                    // otherwise create a new array with an extra spot
                    int[] newidx = new int[symidx[sec.FullName].Length + 1];
                    // copy the old indicies in
                    symidx[sec.FullName].CopyTo(newidx, 0);
                    // save this latest index
                    newidx[symidx[sec.FullName].Length] = i;
                    // replace the old with the new
                    symidx[sec.FullName] = newidx;
                }
            }
        }

        Security GetVisibleSecurity(int row)
        {
            if ((row < 0) || (row >= qg.Rows.Count)) return new Security();
            Security s = Security.Parse(qt.Rows[row]["Symbol"].ToString());
            return s;
        }
        int[] GetSymbolRows(string sym)
        {
            int[] res = new int[] { };
            symidx.TryGetValue(sym,out res);
            if (res == null) return new int[0];
            return res;
        }

        void RefreshRow(Tick t)
        {
            if (qg.InvokeRequired)
            {
                qg.Invoke(new TickDelegate(RefreshRow), new object[] { t });

            }
            else
            {
                int[] rows = GetSymbolRows(t.Sec.FullName);
                decimal high = tl.FastHigh(t.symbol);
                decimal low = tl.FastLow(t.symbol);
                for (int i = 0; i < rows.Length; i++)
                {
                    // last,size,bid/ask,sizes
                    // fetch OHLC from TL
                    // fetch position from TL
                    int r = rows[i];
                    if (qt.Rows[r].RowState == DataRowState.Deleted) continue;
                    if ((r < 0) || (r > qt.Rows.Count - 1)) continue;
                    if (t.isTrade)
                    {
                        qt.Rows[r]["Last"] = t.trade.ToString("N2");
                        qt.Rows[r]["Exch"] = t.ex;
                        if (t.size > 0) // make sure TSize is reported
                            qt.Rows[r]["TSize"] = t.size;
                    }
                    else if (t.FullQuote)
                    {

                        qt.Rows[r]["Bid"] = t.bid.ToString("N2");
                        qt.Rows[r]["Ask"] = t.ask.ToString("N2");
                        qt.Rows[r]["BidEx"] = t.be;
                        qt.Rows[r]["AskEx"] = t.oe;
                        qt.Rows[r]["BSize"] = t.bs;
                        qt.Rows[r]["ASize"] = t.os;
                        qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + t.os.ToString();
                    }
                    else if (t.hasBid)
                    {
                        qt.Rows[r]["Bid"] = t.bid.ToString("N2");
                        qt.Rows[r]["BidEx"] = t.be;
                        qt.Rows[r]["BSize"] = t.bs;
                        string s = qt.Rows[r]["ASize"].ToString();
                        int os = (s != "") ? Convert.ToInt32(s) : 0;
                        qt.Rows[r]["Sizes"] = t.bs.ToString() + "x" + os.ToString();
                    }
                    else if (t.hasAsk)
                    {
                        qt.Rows[r]["Ask"] = t.ask.ToString("N2");
                        qt.Rows[r]["ASize"] = t.os;
                        qt.Rows[r]["AskEx"] = t.oe;
                        string s = qt.Rows[r]["BSize"].ToString();
                        int bs = (s != "") ? Convert.ToInt32(s) : 0;
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
                
                int[] rows = GetSymbolRows(t.Sec.FullName);
                Position p = null;
                if (posdict.TryGetValue(t.symbol, out p))
                {
                    p.Adjust(t);
                    posdict[t.symbol] = p;
                }
                else 
                    posdict.Add(t.symbol, new Position(t));
                int size = posdict[t.symbol].Size;
                decimal price = posdict[t.symbol].AvgPrice;
                for (int i = 0; i < rows.Length; i++)
                {
                    qt.Rows[rows[i]]["PosSize"] = size.ToString();
                    qt.Rows[rows[i]]["AvgPrice"] = price.ToString("N2");
                }
                TradesView.Rows.Add(t.xdate, t.xtime, t.xsec, t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice.ToString("N2"), t.comment, t.Account.ToString()); // if we accept trade, add it to list
            }
        }
        Tick[] ticks = new Tick[100];

        void tl_gotTick(Tick t)
        {
            int[] rows = GetSymbolRows(t.Sec.FullName);
            if ((rows==null)||(rows.Length == 0) ) return;
            ticks[rows[0]] = t;
            BarList bl = null;
            if (bardict.TryGetValue(t.symbol, out bl))
                bardict[t.symbol].newTick(t);
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
            else
            {
                statusWindow.AppendText(s + Environment.NewLine);
            }
        }

        DateTime laststat = DateTime.Now;
        public void status(string s)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { s });
            else
            {
                statuslab.Text = s;
                statusStrip1.Visible = true;
                laststat = DateTime.Now;
            }
        }

        MarketBasket mb = new MarketBasket();


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
                        Security sec = Security.Parse(r[i]);
                        if (sec.isValid)
                            mb.Add(sec);
                        else { add = false; skipped++; }
                        if (add) addsymbol(r[i]);
                    }
                    status("Imported " + (r.Length - skipped) + " instruments.");
                }
            }
            tl.Subscribe(mb);
        }

    }




}
