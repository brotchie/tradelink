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
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace Quotopia
{
    public delegate void OrderStatusDel(string sym, int error);
    public partial class Quote : AppTracker
    {

        public int GetDate { get { DateTime d = DateTime.Now; int i = (d.Year * 10000) + (d.Month * 100) + d.Day; return i; } }
        public int GetTime { get { DateTime d = DateTime.Now; int i = (d.Hour * 100) + (d.Minute); return i; } }
        public event TickDelegate spillTick;
        public event OrderStatusDel orderStatus;
        AsyncResponse _ar = new AsyncResponse();
        public const string PROGRAM = "Quotopia";
        Log _log = new Log(PROGRAM);
        TLTracker _tlt;
        BackgroundWorker bw = new BackgroundWorker();
        bool _constructed = false;
        public Quote()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();

            if ((Location.X == 0) && (Location.Y == 0))
            {
                Quotopia.Properties.Settings.Default.location = new Point(300, 300);
                Quotopia.Properties.Settings.Default.Save();
                Refresh();
            }

            debug(Util.TLSIdentity());
            QuoteGridSetup();
            initfeeds();
            statfade.Interval = 3000;
            statfade.Tick += new EventHandler(statfade_Tick);
            statfade.Start();
            
            ordergrid.ContextMenuStrip = new ContextMenuStrip();
            ordergrid.ContextMenuStrip.Items.Add("Cancel", null, new EventHandler(cancelorder));
            FormClosing += new FormClosingEventHandler(Quote_FormClosing);
            Resize += new EventHandler(Quote_Resize);
            _tlt_GotConnect();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            if (!bw.IsBusy)
                bw.RunWorkerAsync();
            processcommands();

            _constructed = true;
        }

        BrokerFeed _bf;
        void initfeeds()
        {
            string[] servers = Properties.Settings.Default.ServerIpAddresses.Split(',');
            _bf = new BrokerFeed(Properties.Settings.Default.PreferredQuote, Properties.Settings.Default.PreferredExec, Properties.Settings.Default.FallbackToAnyProvider, false,PROGRAM,servers,Properties.Settings.Default.ServerPort);
            _bf.SendDebugEvent+=new DebugDelegate(debug);
            _bf.Reset();

            // if our machine is multi-core we use seperate thread to process ticks
            if (Environment.ProcessorCount == 1)
                _bf.gotTick += new TickDelegate(tl_gotTick);
            else
            {
                _bf.gotTick += new TickDelegate(tl_gotTickasync);
                _ar.GotTick += new TickDelegate(tl_gotTick);
            }
            _bf.gotFill += new FillDelegate(tl_gotFill);
            _bf.gotOrder += new OrderDelegate(tl_gotOrder);
            _bf.gotOrderCancel += new LongDelegate(tl_gotOrderCancel);
            _bf.gotPosition += new PositionDelegate(tl_gotPosition);
            _bf.gotAccounts += new DebugDelegate(tl_gotAccounts);
            // monitor quote feed
            if (_bf.isFeedConnected)
            {
                int poll = (int)((double)Properties.Settings.Default.brokertimeoutsec * 1000 / 2);
                debug(poll == 0 ? "connection timeout disabled." : "using connection timeout: " + poll);
                _tlt = new TLTracker(poll, (int)Properties.Settings.Default.brokertimeoutsec, _bf.FeedClient, Providers.Unknown, true);
                _tlt.GotConnectFail += new VoidDelegate(_tlt_GotConnectFail);
                _tlt.GotConnect += new VoidDelegate(_tlt_GotConnect);
                _tlt.GotDebug += new DebugDelegate(_tlt_GotDebug);
                status("Connected: " + _bf.Feed);
            }
        }

        void processcommands()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) return;
            string f = args[1];
            if (!File.Exists(f))
            {
                status("Can't load basket file: " + Path.GetFileName(f));
                debug("file does not exist: " + f);
                return;
            }
            Basket nb = BasketImpl.FromFile(f);
            addbasket(nb);
            
            Refresh();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TradeLink.AppKit.Versions.UpgradeAlert(_bf.BrokerClient,true);
        }

        void _tlt_GotDebug(string msg)
        {
            //debug(msg);
        }

        void _tlt_GotConnect()
        {
            try
            {
                if (_tlt.tw.RecentTime != 0)
                {
                    debug(_bf.BrokerName + " " + _bf.ServerVersion + " refreshed.");
                    status(_bf.BrokerName + " connected.");
                }
                // if we have a quote provider
                if ((_bf.ProvidersAvailable.Length>0))
                {
                    // don't track tradelink
                    if (_bf.BrokerName == Providers.TradeLink)
                    {
                        _tlt.Stop();
                    }

                    // if we have a quote provid
                    if (mb.Count > 0)
                        subscribe();
                }
            }
            catch { }
        }

        void _tlt_GotConnectFail()
        {
            if (_tlt.tw.RecentTime != 0)
            {
            }
        }


        void Quote_Resize(object sender, EventArgs e)
        {
            Quotopia.Properties.Settings.Default.wsize = Size;

        }


        string[] accts;

        void tl_gotAccounts(string msg)
        {
            debug("avail accounts: " + msg);
            accts = msg.Split(',');
            
        }

        void tl_gotTickasync(Tick t)
        {
            // on multi-core machines, this will be invoked to write ticks
            // to a cache where they will be processed by a seperate thread
            // asynchronously
            _ar.newTick(t);
        }

        void tl_gotPosition(Position pos)
        {
            debug("pos: " + pos.ToString());
            pt.Adjust(pos);
            int[] rows = new int[0];
            if (symidx.TryGetValue(pos.Symbol, out rows))
            {
                foreach (int r in rows)
                {
                    qt.Rows[r]["AvgPrice"] = pos.AvgPrice.ToString(_dispdecpointformat);
                    qt.Rows[r]["PosSize"] = pos.Size.ToString();
                    qt.Rows[r]["ClosedPL"] = pos.ClosedPL.ToString(_dispdecpointformat);
                }

            }
        }

        Dictionary<string, Position> posdict = new Dictionary<string, Position>();


        void statfade_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(laststat).TotalSeconds > 5)
                statusStrip1.Visible = false;
        }

        void ToggleCol(object sender, EventArgs e)
        {
            string col = ((ToolStripItem)sender).Text;
            if (!qg.Columns.Contains(col)) return;
            qg.Columns[col].Visible = !qg.Columns[col].Visible;
            qg.Refresh();
        }

        void tl_gotOrderCancel(long number)
        {
            debug("cancelack: " + number);
            if (ordergrid.InvokeRequired)
                ordergrid.Invoke(new LongDelegate(tl_gotOrderCancel), new object[] { number });
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
                long oid = (long)ordergrid["oid", ordergrid.SelectedRows[i].Index].Value;
                _bf.CancelOrder(oid);
                debug("cancel: " + oid.ToString());
            }
        }

        void tl_gotOrder(Order o)
        {
            debug("orderack: " + o.ToString());
            if (orderidx(o.id)==-1) // if we don't have this order, add it
                ordergrid.Rows.Add(new object[] { o.id, o.symbol, (o.side ? "BUY" : "SELL"), o.UnsignedSize, (o.price == 0 ? "Market" : o.price.ToString(_dispdecpointformat)), (o.stopp == 0 ? "" : o.stopp.ToString(_dispdecpointformat)), o.Account });
        }

        int orderidx(long orderid)
        {
            for (int i = 0; i < ordergrid.Rows.Count; i++) // see if's an existing existing order
                if ((long)ordergrid["oid", i].Value == orderid)
                    return i;
            return -1;
        }

        void Quote_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();

            try
            {
                _log.Stop();
                _ar.Stop();
                _tlt.Stop();
                _bf.Unsubscribe();
                _bf.Disconnect();
            }
            catch { }
        }

        Timer statfade = new Timer();
        Multimedia.Timer ticker = new Multimedia.Timer();
        DataGridView qg = new DataGridView();
        DataTable qt = new DataTable();
        const string SYMBOL = "Symbol";
        const string LAST = "Last";
        const string TSIZE = "TSize";
        const string AVGPRICE = "AvgPrice";
        const string POSSIZE = "PosSize";
        const string CLOSEDPL = "ClosedPL";
        const string DATE = "Date";
        const string TIME = "Time";
        void QuoteGridSetup()
        {
            qt.Columns.Add(SYMBOL);
            qt.Columns.Add(LAST);
            qt.Columns.Add(TSIZE);
            qt.Columns.Add("Bid");
            qt.Columns.Add("Ask");
            qt.Columns.Add("BSize");
            qt.Columns.Add("ASize");
            qt.Columns.Add("Sizes");
            qt.Columns.Add(AVGPRICE);
            qt.Columns.Add(POSSIZE);
            qt.Columns.Add(CLOSEDPL);
            qt.Columns.Add("High");
            qt.Columns.Add("Low");
            qt.Columns.Add("Exch");
            qt.Columns.Add("BidEx");
            qt.Columns.Add("AskEx");
            qt.Columns.Add(DATE);
            qt.Columns.Add(TIME);
            //bid-ask spread
            qt.Columns.Add("BASpreadBPS");
            qt.Columns.Add("BASpread");
            qg.AllowUserToAddRows = false;
            qg.AllowUserToDeleteRows = false;
            qg.AllowUserToOrderColumns = true;
            qg.AllowUserToResizeColumns = true;
            qg.BackgroundColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            qg.RowHeadersVisible = false;
            qg.ColumnHeadersVisible = true;
            qg.Capture = true;
            qg.ContextMenuStrip = new ContextMenuStrip();
            qg.ContextMenuStrip.Items.Add("Add Symbol", null, new EventHandler(rightadd));
            qg.ContextMenuStrip.Items.Add("Remove", null, new EventHandler(rightremove));
            qg.ContextMenuStrip.Items.Add("Chart", null, new EventHandler(rightchart));
            qg.ContextMenuStrip.Items.Add("Ticket", null, new EventHandler(rightticket));
            qg.ContextMenuStrip.Items.Add("Import Basket", null,new EventHandler(importbasketbut_Click));
            qg.ContextMenuStrip.Items.Add("Export Basket", null, new EventHandler(exportbasket));
            qg.ContextMenuStrip.Items.Add("Report Bug", null, new EventHandler(report));
            qg.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
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
            qg.ColumnHeadersHeight = qg.ColumnHeadersDefaultCellStyle.Font.Height*2;
            qg.DoubleClick += new EventHandler(qg_DoubleClick);
            quoteTab.KeyUp +=new KeyEventHandler(qg_KeyUp);
            this.KeyUp += new KeyEventHandler(qg_KeyUp);
            qg.MouseUp += new MouseEventHandler(qg_MouseUp);
            SetColumnContext();
        }

        void report(object o, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty,string.Empty,_dmsg.ToString(),null,null,false);
            _dmsg = new System.Text.StringBuilder();
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
            if ((s.FullName == string.Empty) || (sym == string.Empty))
            {
                return;
            }
            Order o = new OrderImpl(s.FullName,0);
            o.ex = s.DestEx;
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
            if (accountname.Text != "") 
                sendOrder.Account = accountname.Text;
            if (exchdest.Text != "")
                sendOrder.Exchange = exchdest.Text;
            int res = _bf.SendOrder(sendOrder);
            if (res != 0)
            {
                string err = Util.PrettyError(_bf.BrokerName, res);
                status(err);
                debug(sendOrder.ToString() + "( " + err + " )");
            }
            else
                debug("order: "+sendOrder.ToString());
        }

        void rightadd(object sender, EventArgs e)
        {
            string syms = TextPrompt.Prompt("Symbols to add", "Enter symbols seperated by commas: ");
            addbasket(BasketImpl.FromString(syms));
        }

        void rightremove(object sender, EventArgs e)
        {
            Security sec = GetVisibleSecurity(CurrentRow);
            if (!sec.isValid)
                return;
            string sym = sec.Symbol;
            if (MessageBox.Show("Are you sure you want to remove "+sym+"?","Confirm remove",MessageBoxButtons.YesNo)== DialogResult.Yes)
            {
                // remove symbol from basket
                mb.Remove(sym);
                //remove the row
                qt.Rows.RemoveAt(CurrentRow);
                // accept changes
                qt.AcceptChanges();
                // clear current index
                symidx.Clear();
                // reget index
                symindex();
            }
        }


        void rightchart(object sender, EventArgs e)
        {
            Security sec = GetVisibleSecurity(CurrentRow);
            if (!sec.isValid)
                return;
            string sym = sec.Symbol;
            BarList bl;
            if (!bardict.TryGetValue(sec.FullName,out bl))
            {
                debug("Unable to locate chart for: "+sec.FullName);
                return;
            }
            Chart c = new Chart(bl);
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

        bool subscribe()
        {
            try
            {
                _bf.Subscribe(mb);
                return true;
            }
            catch (TLServerNotFound) { debug("no broker or feed server running."); }
            return false;
        }

        string newsymbol = "";
        void qg_KeyUp(object sender, KeyEventArgs e)
        {
            // only pay attention when market tab is selected
            if (quoteTab.SelectedTab.Name != "Markets") return;
            string preface = "Adding symbol: ";
            if (e.KeyCode == Keys.Enter)
            {
                Security sec = SecurityImpl.Parse(newsymbol);
                if (sec.isValid)
                {
                    if (addsymbol(newsymbol))
                    {
                        newsymbol = "";
                        subscribe();
                    }
                }
                else
                {
                    status("Invalid Security "+newsymbol);
                    newsymbol = "";
                }
            }
            else if (e.Shift && (e.KeyCode == Keys.OemPipe))
            {
                newsymbol += "|";
                status(preface + newsymbol);
            }
            else if (e.Shift && (e.KeyCode== Keys.OemQuestion))
            {
                newsymbol += "?";
                status(preface + newsymbol);
            }
            else if ((e.KeyCode == Keys.OemPipe))
            {
                newsymbol += "\\";
                status(preface + newsymbol);
            }
            else if (e.KeyCode == Keys.OemQuestion)
            {
                newsymbol += "/";
                status(preface + newsymbol);
            }
            else if (e.KeyCode == Keys.OemPeriod)
            {
                newsymbol += ".";
                status(preface + newsymbol);
            }
            else if ((e.KeyCode == Keys.OemMinus) && e.Shift)
            {
                newsymbol += "_";
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
            else if ((e.KeyCode == Keys.Back) && (newsymbol.Length > 0))
            {
                newsymbol = newsymbol.Substring(0, newsymbol.Length - 1);
                status(preface + newsymbol);
            }
            else if (((e.KeyValue >= (int)Keys.A) && (e.KeyValue <= (int)Keys.Z))
                || ((e.KeyValue >= (int)Keys.D0) && (e.KeyValue <= (int)Keys.D9)) || e.Shift || (e.KeyData == Keys.Space))
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
            else if (e.KeyData == Keys.OemPeriod)
            {
                newsymbol += ".";
                status("Adding symbol: " + newsymbol);
            }
            else if (e.KeyData == Keys.Space)
            {
                newsymbol += " ";
                status("Adding symbol: " + newsymbol);
            }
            else if (e.KeyData == Keys.OemMinus)
            {
                newsymbol += "-";
                status("Adding symbol: " + newsymbol);
            }
        }

        Dictionary<string, BarList> bardict = new Dictionary<string, BarList>();

        bool addsymbol(string sym) { return addsymbol(sym, true); }
        bool addsymbol(string sym, bool dosubscribe)
        {
            Security sec = SecurityImpl.Parse(sym);
            try
            {
                // SYM,LAST,TSIZE,BID,ASK,BSIZE,ASIZE,SIZES,OHLC(YEST),CHANGE
                DataRow r = qt.Rows.Add(sym, "", "", "", "", "", "", "", "", "", "", "");
                qt.Rows[qt.Rows.Count - 1][AVGPRICE] = pt[sym].AvgPrice;
                qt.Rows[qt.Rows.Count - 1][POSSIZE] = pt[sym].Size;
                qt.Rows[qt.Rows.Count - 1][CLOSEDPL] = pt[sym].ClosedPL;
            }
            catch { }

            if (!bardict.ContainsKey(sec.FullName))
                bardict.Add(sec.FullName, new BarListImpl(sym));
            status("Added " + sym);
            if (!hassym(sym))
                mb.Add(sym);
            symindex();
            if (dosubscribe)
                return subscribe();
            return true;
        }

        bool hassym(string sym)
        {
            foreach (Security s in mb)
                if ((s.FullName == sym) || (s.Symbol == sym))
                    return true;
            return false;
        }

        

        
        Dictionary<string, int[]> symidx = new Dictionary<string, int[]>();

        void symindex()
        {
            for (int i = 0; i < qt.Rows.Count; i++)
            {
                if (qt.Rows[i].RowState == DataRowState.Deleted) continue;
                Security sec = SecurityImpl.Parse(qt.Rows[i]["Symbol"].ToString());
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
            if ((row < 0) || (row >= qg.Rows.Count)) return new SecurityImpl();
            Security s = SecurityImpl.Parse(qt.Rows[row]["Symbol"].ToString());
            return s;
        }
        int[] GetSymbolRows(string sym)
        {
            int[] res = new int[0];
            if (symidx.TryGetValue(sym, out res))
                return res;
            List<int> rows = new List<int>();
            for (int i = 0; i < qt.Rows.Count; i++)
            {
                if (qt.Rows[i].RowState == DataRowState.Deleted) 
                    continue;
                Security sec = SecurityImpl.Parse(qt.Rows[i]["Symbol"].ToString());
                Security csec = SecurityImpl.Parse(sym);
                if (sec.Symbol == csec.Symbol)
                    rows.Add(i);
            }
            
            return rows.ToArray();
        }
        HighTracker _ht = new HighTracker();
        LowTracker _lt = new LowTracker();
        
        void RefreshRow(Tick t)
        {

            if (qg.InvokeRequired)
            {
                qg.Invoke(new TickDelegate(RefreshRow), new object[] { t });

            }
            else
            {
                _ht.newTick(t);
                _lt.newTick(t);
                int[] rows = GetSymbolRows(t.symbol);
                for (int i = 0; i < rows.Length; i++)
                {
                    // last,size,bid/ask,sizes
                    // fetch OHLC from TL
                    // fetch position from TL
                    int r = rows[i];
                    if (qt.Rows[r].RowState == DataRowState.Deleted) continue;
                    if ((r < 0) || (r >= qt.Rows.Count)) continue;
                    //get number of shares to display per unit size
                    int numSharesPerContract = (int)_sharepercontract.Value;
                    if (t.isTrade)
                    {
                        qt.Rows[r]["Last"] = t.trade.ToString(_dispdecpointformat);
                        qt.Rows[r]["Exch"] = t.ex;
                        if (t.size > 0) // make sure TSize is reported
                            qt.Rows[r]["TSize"] = t.size;
                    }
                    if (t.isFullQuote)
                    {

                        qt.Rows[r]["Bid"] = t.bid.ToString(_dispdecpointformat);
                        qt.Rows[r]["Ask"] = t.ask.ToString(_dispdecpointformat);
                        qt.Rows[r]["BidEx"] = t.be;
                        qt.Rows[r]["AskEx"] = t.oe;
                        int bs = t.bs / numSharesPerContract;
                        int os = t.os / numSharesPerContract;
                        qt.Rows[r]["BSize"] = bs;
                        qt.Rows[r]["ASize"] = os;
                        qt.Rows[r]["Sizes"] = bs.ToString() + "x" + os.ToString();
                    }
                    else if (t.hasBid)
                    {
                        qt.Rows[r]["Bid"] = t.bid.ToString(_dispdecpointformat);
                        qt.Rows[r]["BidEx"] = t.be;
                        int bs = t.bs / numSharesPerContract;
                        qt.Rows[r]["BSize"] = bs;
                        string s = qt.Rows[r]["ASize"].ToString();
                        int os = (s != "") ? Convert.ToInt32(s) : 0;
                        qt.Rows[r]["Sizes"] = bs.ToString() + "x" + os.ToString();
                    }
                    else if (t.hasAsk)
                    {
                        qt.Rows[r]["Ask"] = t.ask.ToString(_dispdecpointformat);
                        int os = t.os / numSharesPerContract;
                        qt.Rows[r]["ASize"] = os;
                        qt.Rows[r]["AskEx"] = t.oe;
                        string s = qt.Rows[r]["BSize"].ToString();
                        int bs = (s != "") ? Convert.ToInt32(s) : 0;
                        qt.Rows[r]["Sizes"] = bs.ToString() + "x" + os.ToString();
                    }
                    try
                    {
                        qt.Rows[r]["High"] = _ht[t.symbol].ToString(_dispdecpointformat);
                        qt.Rows[r]["Low"] = _lt[t.symbol].ToString(_dispdecpointformat);
                    }
                    catch { }
                    if (_dispdecpoints.Value > 2)
                    {
                        try
                        {
                            //most recent bid
                            decimal bid = 0;
                            decimal ask = 0;
                            if (decimal.TryParse(qt.Rows[r]["Bid"].ToString(), out bid)
                                && decimal.TryParse(qt.Rows[r]["Ask"].ToString(), out ask))
                            {
                                decimal baSpread = ask - bid;
                                decimal baSpreadRel = 10000 * baSpread / bid;
                                qt.Rows[r]["BASpread"] = baSpread.ToString(_dispdecpointformat);
                                qt.Rows[r]["BASpreadBPS"] = baSpreadRel.ToString(_dispdecpointformat);
                            }
                        }
                        catch { }
                    }
                    try
                    {
                        qt.Rows[r][DATE] = t.date;
                        qt.Rows[r][TIME] = t.time;
                    }
                    catch { }
                }
            }
        }


        void tl_gotFill(Trade t)
        {
            debug("fill: " + t.ToString());
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
                pt.Adjust(t);
                
                UpdateSymbolTrade(GetSymbolRows(t.Sec.FullName),t);
                UpdateSymbolTrade(GetSymbolRows(t.Sec.Symbol),t);

                TradesView.Rows.Add(t.xdate, t.xtime, t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice.ToString(_dispdecpointformat), t.comment, t.Account.ToString()); // if we accept trade, add it to list
            }
        }

        void UpdateSymbolTrade(int[] rows,Trade t)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                qt.Rows[rows[i]]["PosSize"] = pt[t.symbol].Size.ToString();
                qt.Rows[rows[i]]["AvgPrice"] = pt[t.symbol].AvgPrice.ToString(_dispdecpointformat);
                qt.Rows[rows[i]]["ClosedPL"] = pt[t.symbol].ClosedPL.ToString(_dispdecpointformat);
            }
        }

        PositionTracker pt = new PositionTracker();
        bool usebidonfx = Properties.Settings.Default.UseBidNotAskOnFxCharts;
        TickTracker _kt = new TickTracker(100);
        void tl_gotTick(Tick t)
        {

            try
            {
                _tlt.newTick(t);
                _kt.newTick(t);
                if (spillTick != null)
                    spillTick(t);
                RefreshRow(t);
                BarList bl = null;
                if (bardict.TryGetValue(t.symbol, out bl))
                {
                    if (SecurityImpl.Parse(t.symbol).Type == SecurityType.CASH)
                    {
                        Tick k = _kt[t.symbol];
                        decimal p = usebidonfx ? k.bid : k.ask;
                        int s = usebidonfx ? k.bs : k.os;
                        bardict[t.symbol].newPoint(t.symbol,p, k.time, k.date, s);
                    }
                    else
                        bardict[t.symbol].newTick(t);
                }
            }
            catch (System.Threading.ThreadInterruptedException) { }
        }




        ~Quote() { QuotopiaClose(); }
        void QuotopiaClose()
        {
            _bf.Disconnect(); // unregister all stocks and this client
        }

        System.Text.StringBuilder _dmsg = new System.Text.StringBuilder();
        public void debug(string s)
        {
            _dmsg.AppendLine(s);
            debugControl1.GotDebug(s);
            _log.GotDebug(s);
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

        Basket mb = new BasketImpl();

        void exportbasket(object send, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.AddExtension = true;
            sd.DefaultExt = ".txt";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                BasketImpl.ToFile(mb, sd.FileName);
            }
        }

        private void importbasketbut_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = true;
            od.Filter = "Basket (*.txt)|*.txt|All files (*.*)|*.*";
            od.AddExtension = true;
            od.DefaultExt = ".txt";
            od.CheckFileExists = true;
            od.Title = "Select the baskets you wish to import to quototpia";
            if (od.ShowDialog() == DialogResult.OK)
            {
                int count = 0;
                foreach (string f in od.FileNames)
                {
                    Basket nb = BasketImpl.FromFile(f);
                    addbasket(nb);
                    count += nb.Count;
                }
                status("Imported " + count + " instruments.");
            }
            else return;
            
            Invalidate(true);
        }



        void addbasket(Basket b)
        {
            foreach (Security s in b)
                addsymbol(s.FullName,false);
            subscribe();
        }

        private void saveSettingsbut_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void resetsetbut_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
        }

        private void restoredefaultsbut_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            
        }


        private void _sharepercontract_ValueChanged(object sender, EventArgs e)
        {

        }

        string _dispdecpointformat = "N"+((int)Properties.Settings.Default.displaydecpoints).ToString();
        private void _dispdecpoints_ValueChanged(object sender, EventArgs e)
        {
            if (_constructed)
                _dispdecpointformat = "N" + ((int)_dispdecpoints.Value).ToString();
        }

        private void _brokertimeout_ValueChanged(object sender, EventArgs e)
        {
            _tlt.AlertThreshold = (int)_brokertimeout.Value;
        }


    }




}
