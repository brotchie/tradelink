using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;
using TradeLink.AppKit;

namespace Kadina
{
    public partial class kadinamain : AppTracker
    {
        SecurityImpl sec = null;
        string tickfile = "";
        string responsedll = "";
        string resname = "";
        Response myres;
        PlayTo pt = PlayTo.Hour;
        int _dp = Properties.Settings.Default.DecimalPlaces;
        string _dps = "N2";

        ResponseList _rl = new ResponseList();

        DataTable dt = new DataTable("ticktable");
        DataTable it = new DataTable("itable");
        DataTable ptab = new DataTable("ptable");
        SafeBindingSource tbs = new SafeBindingSource(false);
        SafeBindingSource ibs = new SafeBindingSource(false);
        DataGridView pg = new DataGridView();
        DataGridView ig = new DataGridView();
        DataGridView dg = new DataGridView();

        DataTable ot = new DataTable("otable");
        SafeBindingSource obs = new SafeBindingSource(false);
        DataTable ft = new DataTable("ftable");
        SafeBindingSource fbs = new SafeBindingSource(false);
        DataGridView og = new DataGridView();
        DataGridView fg = new DataGridView();
        

        BackgroundWorker bw = new BackgroundWorker();
        BackgroundWorker bw2 = new BackgroundWorker();
        HistSimImpl h = new HistSimImpl();
        ChartControl c = new ChartControl();

        public kadinamain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            _dps = "N" + _dp;
            h.SimBroker.UseBidAskFills = Properties.Settings.Default.UseBidAskFills;
            InitializeComponent();
            initgrids();
            
            sizetabs();
            restorerecentfiles();
            restorerecentlibs();
            
            FormClosing += new FormClosingEventHandler(kadinamain_FormClosing);
            Resize += new EventHandler(kadinamain_Resize);
            bw.DoWork += new DoWorkEventHandler(Play);
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PlayComplete);

            bw2.DoWork += new DoWorkEventHandler(bw2_DoWork);
            bw2.RunWorkerAsync();
            debug(Util.TLSIdentity());

            // grid errors
            dg.DataError += new DataGridViewDataErrorEventHandler(dg_DataError);
            ig.DataError += new DataGridViewDataErrorEventHandler(ig_DataError);
            fg.DataError += new DataGridViewDataErrorEventHandler(fg_DataError);
            og.DataError += new DataGridViewDataErrorEventHandler(og_DataError);
            pg.DataError += new DataGridViewDataErrorEventHandler(pg_DataError);
        }

        void bw2_DoWork(object sender, DoWorkEventArgs e)
        {
            Versions.UpgradeAlert();
        }

        void kadinamain_Resize(object sender, EventArgs e)
        {
            sizetabs();
        }

        void sizetabs()
        {
            _tabs.Size = new Size(Width, Height - (statusStrip1.Height + (int)(statusStrip2.Height * 2.5)));
            ordertab.Width = _tabs.Width;
            ordertab.Height = _tabs.Height;
            itab.Width = _tabs.Width;
            itab.Height = _tabs.Height;
            filltab.Height = _tabs.Height;
            filltab.Width = _tabs.Width;
            postab.Width = _tabs.Width;
            postab.Height = _tabs.Height;
            ticktab.Width = _tabs.Width;
            ticktab.Height = _tabs.Height;
            msgtab.Width = _tabs.Width;
            msgtab.Height = _tabs.Height;
            ig.Width = itab.Width - SystemInformation.VerticalScrollBarWidth;
            ig.Height = itab.Height;
            og.Width = ordertab.Width - SystemInformation.VerticalScrollBarWidth;
            og.Height = ordertab.Height;
            fg.Width = filltab.Width - SystemInformation.VerticalScrollBarWidth;
            fg.Height = filltab.Height;
            pg.Height = postab.Height;
            pg.Width = postab.Width - SystemInformation.VerticalScrollBarWidth;
            dg.Height = ticktab.Height;
            dg.Width = ticktab.Width - SystemInformation.VerticalScrollBarWidth;

            ig.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            ig.ScrollBars = ScrollBars.Both;

            Invalidate(true);
        }


        int _time = 0;
        int _date = 0;

        void h_GotTick(Tick t)
        {
            _date = t.date;
            _time = t.time;
            // get time for display
            nowtime = t.time.ToString();
            
            // don't display ticks for unmatched exchanges
            string time = nowtime;
            string trade = "";
            string bid = "";
            string ask = "";
            string ts = "";
            string bs = "";
            string os = "";
            string be = "";
            string oe = "";
            string ex = "";
            if (t.isIndex)
            {
                trade = t.trade.ToString(_dps);
            }
            else if (t.isTrade)
            {
                trade = t.trade.ToString(_dps);
                ts = t.size.ToString();
                ex = t.ex;
            }
            if (t.hasBid)
            {
                bs = t.bs.ToString();
                be = t.be;
                bid = t.bid.ToString(_dps);
            }
            if (t.hasAsk)
            {
                ask = t.ask.ToString(_dps);
                oe = t.oe;
                os = t.os.ToString();
            }
            
            // add tick to grid
            NewTRow(new string[] { nowtime,t.symbol,trade,ts,bid,ask,bs,os,ex,be,oe});
            // send to response
            if (myres != null)
                myres.GotTick(t);
            // send to chart
            if (c != null)
            {
                if (_chartlast)
                {
                    if (t.isTrade)
                        c.newPoint(t.trade, t.time, t.date, t.size);
                }
                else if (_chartbid)
                {
                    if (t.hasBid)
                        c.newPoint(t.bid, t.time, t.date, t.BidSize);
                }
                else if (t.hasAsk)
                {
                    c.newPoint(t.ask, t.time, t.date, t.AskSize);
                }
            }
        }

        bool _chartlast = Properties.Settings.Default.ChartLast;
        bool _chartbid = Properties.Settings.Default.ChartNoLastUseBid;

        void Play(object sender, DoWorkEventArgs e)
        {
            PlayTo type = (PlayTo)e.Argument;
            if (e.Cancel) return;
            int time = (int)(h.NextTickTime % 100000);
            long date = (h.NextTickTime / 100000)*100000;
            int t = (int)type;
            long val = 0;
            switch (type)
            {
                case PlayTo.End : 
                    val = HistSimImpl.ENDSIM; 
                    break;
                case PlayTo.FiveMin : 
                case PlayTo.OneMin:
                case PlayTo.TenMin:
                case PlayTo.HalfHour :
                    val = date + Util.FTADD(time, (t / 10)*60);
                    break;
                case PlayTo.Hour:
                    val = date + Util.FTADD(time,(t/1000)*3660);
                    break;
                case PlayTo.OneSec:
                case PlayTo.ThirtySec:
                    val = date+ Util.FTADD(time, t); 
                    break;
            }
            cleardebugs(); // clear the message box on first box run
            h.PlayTo(val);
        }

        void cleardebugs()
        {
            debugControl1.Clear();
        }



        void kadinamain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saverecentlibs();
            saverecentfiles();
            Kadina.Properties.Settings.Default.Save();
        }
        const string PLAYTO = "Play +";
        

        bool hasprereq() { return hasprereq(true); }
        bool hasprereq(bool stat)
        {
            if (bw.IsBusy) { if (stat) status("Still playing, please wait..."); return false; }
            if (myres == null) { if (stat) status("Add response."); return false; }
            if (epffiles.Count == 0) { if (stat) status("Add study data."); return false; }
            if (stat)
                status("Click on desired play duration to begin.");
            return true;
        }


        void playto(PlayTo pt)
        {
            if (!hasprereq(true))
                return;
            bw.RunWorkerAsync(pt);
            status("Playing next " + pt.ToString().Replace(PLAYTO, string.Empty) + "...");
        }

        void reset(bool keepparams)
        {
            // clear all GUIs
            _msg = new StringBuilder(10000);
            debugControl1.Clear();
            dt.Clear();
            ptab.Clear();
            _tradelist.Clear();
            ot.Clear();
            ft.Clear();
            _tabs.Refresh();
            c.Reset();
            _tr.Clear();
            if (it != null) { it.Clear(); it.Columns.Clear(); ig.Invalidate(); }
            // see if we are keeping study or not
            if (keepparams)
            {
                if (h != null) 
                    h.Reset();
                loadboxname(resname);
                if (myres != null)
                {
                    try
                    {
                        myres.Reset();
                    }
                    catch (Exception ex)
                    {
                        debug("An error occured inside your response Reset method: ");
                        debug(ex.Message + ex.StackTrace);
                    }
                }
                status("Reset box " + myres.Name + " " + PrettyEPF());
            }
            else
            {
                myres = null;
                h = new HistSimImpl();
                epffiles.Clear();
                resname = string.Empty;
                updatetitle();
            }
        }


        void NewIRow(object[] values)
        {
            it.Rows.Add(values);
        }
        void initgrids()
        {
            // position tab
            ptab.Columns.Add("Time");
            ptab.Columns.Add("Symbol");
            ptab.Columns.Add("Side");
            ptab.Columns.Add("Size");
            ptab.Columns.Add("AvgPrice");
            ptab.Columns.Add("Profit");
            ptab.Columns.Add("Points");
            pg.Parent = postab;
            pg.DataSource = ptab;
            pg.RowHeadersVisible = false;
            pg.ContextMenu = ContextMenu;
            pg.ReadOnly = true;
            pg.AllowUserToAddRows = false;
            pg.AllowUserToDeleteRows = false;
            pg.ShowEditingIcon = false;
            pg.BackgroundColor = BackColor;
            pg.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            pg.RowHeadersDefaultCellStyle.BackColor = BackColor;
            pg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            pg.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
            pg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            pg.Show();

            // order tab
            ot.Columns.Add("Time");
            ot.Columns.Add("Symbol");
            ot.Columns.Add("Side");
            ot.Columns.Add("Size");
            ot.Columns.Add("Type");
            ot.Columns.Add("Price");
            ot.Columns.Add("Id");
            og.Parent = ordertab;
            obs.DataSource = ot;
            og.DataSource = obs;
            og.RowHeadersVisible = false;
            og.ContextMenu = ContextMenu;
            og.ReadOnly = true;
            og.AllowUserToAddRows = false;
            og.AllowUserToDeleteRows = false;
            og.ShowEditingIcon = false;
            og.BackgroundColor = BackColor;
            og.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            og.RowHeadersDefaultCellStyle.BackColor = BackColor;
            og.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
            og.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;


            og.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            og.Show();

            // fill tab
            ft.Columns.Add("xTime");
            ft.Columns.Add("Symbol");
            ft.Columns.Add("xSide");
            ft.Columns.Add("xSize");
            ft.Columns.Add("xPrice");
            ft.Columns.Add("Id");
            fg.Parent = filltab;
            fbs.DataSource = ft;
            fg.DataSource = fbs;
            fg.RowHeadersVisible = false;
            fg.ContextMenu = ContextMenu;
            fg.ReadOnly = true;
            fg.AllowUserToAddRows = false;
            fg.AllowUserToDeleteRows = false;
            fg.ShowEditingIcon = false;
            fg.BackgroundColor = BackColor;
            fg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            fg.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
            fg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            fg.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            fg.RowHeadersDefaultCellStyle.BackColor = BackColor;

            fg.Show();

            // indicator tab
            igridinit();

            // tick tab
            dt.Columns.Add("Time", "".GetType());
            dt.Columns.Add("Sym");
            dt.Columns.Add("Trade");
            dt.Columns.Add("TSize");
            dt.Columns.Add("Bid");
            dt.Columns.Add("Ask");
            dt.Columns.Add("BSize");
            dt.Columns.Add("ASize");
            dt.Columns.Add("TExch");
            dt.Columns.Add("BidExch");
            dt.Columns.Add("AskExch");
            dg.ContextMenu = this.ContextMenu;
            tbs.DataSource = dt;
            dg.DataSource = tbs;
            dg.AllowUserToAddRows = false;
            dg.AllowUserToDeleteRows = false;
            dg.ShowEditingIcon = false;
            dg.Parent = ticktab;
            dg.RowHeadersVisible = false;
            dg.BackgroundColor = BackColor;
            dg.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
            dg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dg.RowHeadersDefaultCellStyle.BackColor = BackColor;

            dg.ReadOnly = true;
            dg.Show();

            // indicators
            ig.Parent = itab;
            //itab.HorizontalScroll.Enabled = true;
            //itab.HorizontalScroll.Visible = true;
            ibs.DataSource = it;
            ig.ScrollBars = ScrollBars.Both;
            ig.DataSource = ibs;
            ig.RowHeadersVisible = false;
            ig.ContextMenu = this.ContextMenu;
            ig.ReadOnly = true;
            ig.Width = itab.Width;
            ig.Height = itab.Height;
            ig.AllowUserToAddRows = false;
            ig.AllowUserToDeleteRows = false;
            ig.ShowEditingIcon = false;
            ig.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ig.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ig.RowHeadersDefaultCellStyle.BackColor = BackColor;
            ig.ColumnHeadersDefaultCellStyle.BackColor = BackColor;
            ig.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;


            ig.BackgroundColor = BackColor;
            ig.Show();

            // chart
            c.Parent = charttab;
            c.Dock = DockStyle.Fill;

            // trade results
            _tr.Parent = _results;
            _tr.Dock = DockStyle.Fill;

        }


        TradeResults _tr = new TradeResults();

        void igridinit()
        {

            // don't process invalid responses
            if ((myres == null) || (myres.Indicators.Length == 0))
                return;
            // clear existing indicators
            it.Clear();
            it.Columns.Clear();
            // load new ones
            for (int i = 0; i < myres.Indicators.Length; i++)
                it.Columns.Add(myres.Indicators[i]);
            // refresh screen
                ig.Invalidate();
        }


        Dictionary<string, PositionImpl> poslist = new Dictionary<string, PositionImpl>();
        List<Trade> _tradelist = new List<Trade>();
        void broker_GotFill(Trade t)
        {
            if (myres != null)
                myres.GotFill(t);
            _tradelist.Add(t);
            PositionImpl mypos = new PositionImpl(t);
            decimal cpl = 0;
            decimal cpt = 0;
            if (!poslist.TryGetValue(t.symbol, out mypos))
            {
                mypos = new PositionImpl(t);
                poslist.Add(t.symbol, mypos);
            }
            else
            {
                cpt = Calc.ClosePT(mypos, t);
                cpl = mypos.Adjust(t);
                poslist[t.symbol] = mypos;
            }

            ptab.Rows.Add(nowtime, mypos.Symbol, (mypos.isFlat ? "FLAT" : (mypos.isLong ? "LONG" : "SHORT")), mypos.Size, mypos.AvgPrice.ToString(_dps), cpl.ToString("C2"), cpt.ToString(_dps));
            ft.Rows.Add(t.xtime.ToString(), t.symbol, (t.side ? "BUY" : "SELL"), t.xsize, t.xprice.ToString(_dps), t.id);
        }

        void broker_GotOrder(Order o)
        {
            if (myres != null)
                myres.GotOrder(o);
            ot.Rows.Add(o.time, o.symbol,(o.side ? "BUY" : "SELL"), o.size, (o.isMarket ? "Mkt" : (o.isLimit ? "Lmt" : "Stp")),o.isStop? o.stopp : (o.isTrail ? o.trail : o.price),o.id);
        }
        string nowtime = "0";


        delegate void StringArrayDelegate(string[] vals);
        void NewTRow(string[] values)
        {
            dt.Rows.Add(values);
        }



        void loadboxname(string name)
        {
            try
            {
                myres = ResponseLoader.FromDLL(name, responsedll);
            }
            catch (Exception ex) 
            { 
                debug(ex.Message+ex.StackTrace); 
                status("Error loading response");
                myres = null;
                return; 
            }
            if ((myres != null) && (myres.FullName == name))
            {
                resname = name;
                myres.SendTicketEvent += new TicketDelegate(myres_SendTicketEvent);
                myres.SendDebugEvent += new DebugFullDelegate(myres_GotDebug);
                myres.SendCancelEvent += new LongDelegate(myres_CancelOrderSource);
                myres.SendOrderEvent += new OrderDelegate(myres_SendOrder);
                myres.SendIndicatorsEvent += new StringParamDelegate(myres_SendIndicators);
                myres.SendMessageEvent += new MessageDelegate(myres_SendMessage);
                myres.SendBasketEvent += new BasketDelegate(myres_SendBasket);
                myres.SendChartLabelEvent += new ChartLabelDelegate(myres_SendChartLabel);
                status(resname + " is current response.");
                updatetitle();
                igridinit();
                myres.ID = 0;
                try
                {
                    myres.Reset();
                }
                catch (Exception ex)
                {
                    debug("An error occured inside your response Reset method: ");
                    debug(ex.Message + ex.StackTrace);
                }
            }
            else status("Response did not load.");
            hasprereq();

        }
        bool _sendticketwarn = false;
        void myres_SendTicketEvent(string space, string user, string password, string summary, string description, Priority pri, TicketStatus stat)
        {
            if (_sendticketwarn) return;
            debug("Sendticket not supported in kadina.");
            _sendticketwarn = true;
        }

        bool _sendbaskwarn = false;
        void myres_SendBasket(Basket b, int id)
        {
            if (_sendbaskwarn) return;
            debug("Sendbasket not supported in kadina.");
            debug("To specify trading symbols, add data to study.");
            _sendbaskwarn = true;
        }

        void myres_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color col)
        {
            c.DrawChartLabel(price, bar, label,col);
        }
        bool _sendmesswarn = false;
        void myres_SendMessage(MessageTypes type, long source, long dest, long id, string data, ref string response)
        {
            if (_sendmesswarn) return;
            _sendmesswarn = true;
            debug("SendMessage and custom messages not supported in kadina.");
        }
        public const string PROGRAM = "Kadina";
        void updatetitle() { Text = PROGRAM + " - Study: " + resname + " " + PrettyEPF(); Invalidate(); }

        void myres_SendIndicators(string param)
        {
            if (myres == null) return;
            if (myres.Indicators.Length == 0)
                debug("No indicators defined on response: " + myres.Name);
            else
            {
                string[] parameters = param.Split(',');
                NewIRow(parameters);
            }
        }


        void myres_SendOrder(Order o)
        {
            if (o.time == 0)
            {
                o.date = _date;
                o.time = _time;
            }
            h.SimBroker.SendOrderStatus(o);
        }

        void broker_GotOrderCancel(string sym, bool side, long id)
        {
            if (myres != null)
                myres.GotOrderCancel(id);
        }

        void myres_CancelOrderSource(long number)
        {
            h.SimBroker.CancelOrder(number);
        }

        StringBuilder _msg = new StringBuilder(100000000);
        void myres_GotDebug(Debug msg)
        {
            _msg.AppendFormat("{0}: {1}{2}",nowtime,msg.Msg,Environment.NewLine);
        }



        private void kadinamain_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            string []s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string f = s[0];
            loadfile(f);
        }


        List<string> epffiles = new List<string>();
        string PrettyEPF()
        {
            string[] list = new string[epffiles.Count];
            for (int i = 0; i < epffiles.Count; i++)
                list[i] = Path.GetFileNameWithoutExtension(epffiles[i]);
            return list.Length > 0 ? "[" + string.Join(",", list) + "]" : "[?]";
        }
         private bool loadfile(string path)
         {
            string f = path;
            bool success = false;
            if (isResponse(f))
            {
                responsedll = f;
                List<string> l = Util.GetResponseList(responsedll);
                if (System.IO.File.Exists(f) && (l.Count>0))
                    if (!isRecentResLib(f))
                        reslist.DropDownItems.Add(f);
                status("Found " + l.Count + " responses.  ");
                _rl = new ResponseList(l);
                _rl.ResponseSelected+=new DebugDelegate(loadboxname);
                if (_rl.ShowDialog() != DialogResult.OK)
                    status("no response was selected.");

                success = true;
            }
            else if (isTIK(f))
            {
                if (System.IO.File.Exists(f))
                    if (!isRecentTickfile(f) && SecurityImpl.SecurityFromFileName(f).isValid)
                        recent.DropDownItems.Add(f);
                epffiles.Add(f);
                h = new HistSimImpl(epffiles.ToArray());
                h.SimBroker.GotOrder += new OrderDelegate(broker_GotOrder);
                h.SimBroker.GotFill += new FillDelegate(broker_GotFill);
                h.GotTick += new TickDelegate(h_GotTick);
                h.SimBroker.UseBidAskFills = Properties.Settings.Default.UseBidAskFills;
                h.SimBroker.GotOrderCancel += new OrderCancelDelegate(broker_GotOrderCancel);
                try
                {
                    h.Initialize();

                    updatetitle();
                    status("Loaded tickdata: " + PrettyEPF());
                    success = true;
                }
                catch (IOException ex)
                {
                    if (ex.Message.Contains("used by another process"))
                    {
                        status("Try again, file in use: " + f);
                    }
                }
            }
            hasprereq();

            return success;

        }

        
        bool isRecentTickfile(string path)
        {
            for (int i = 0; i < recent.DropDownItems.Count; i++)
                if (recent.DropDownItems[i].Text.Equals(path))
                    return true;
            return false;
        }

        bool isRecentResLib(string path)
        {
            for (int i = 0; i < reslist.DropDownItems.Count; i++)
                if (reslist.DropDownItems[i].Text.Equals(path))
                    return true;
            return false;
        }

        void status(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { msg });
            else
            {
                _stat.Text = msg;
                _stat.Invalidate();
            }
        }

       
        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        private void kadinamain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        bool isTIK(string path) { return System.Text.RegularExpressions.Regex.IsMatch(path, TikConst.EXT, System.Text.RegularExpressions.RegexOptions.IgnoreCase); }
        bool isResponse(string path) { return System.Text.RegularExpressions.Regex.IsMatch(path, "DLL", System.Text.RegularExpressions.RegexOptions.IgnoreCase); }

        void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] r = _msg.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string l in r)
                debug(l);
            ig.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            SafeBindingSource.refreshgrid(dg, tbs);
            SafeBindingSource.refreshgrid(ig, ibs);
            SafeBindingSource.refreshgrid(og, obs);
            SafeBindingSource.refreshgrid(fg, fbs);
            c.redraw();
            _tr.Clear();
            _tr.NewResultTrades(resname +"."+PrettyEPF(), _tradelist);
            _tr.Refresh();
            if (e.Error != null)
            {
                debug(e.Error.Message+e.Error.StackTrace);
                status("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled) status("Canceled play.");
            else status("Reached next " + pt.ToString() + " at time " + KadTime);

        }

        string KadTime
        {
            get
            {
                return nowtime!="" ? nowtime : "(none)";
            }
        }

        private void recent_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == BROWSEMENU) return;
            if (e.ClickedItem.Text == CLEARRECENTDATA) return;
            if (System.IO.File.Exists(e.ClickedItem.Text))
                loadfile(e.ClickedItem.Text);
            else
            {
                status(e.ClickedItem.Text + " not found, removing from recent items.");
                recent.DropDown.Items.Remove(e.ClickedItem);
            }
        }

        private void libs_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == BROWSEMENU) return;
            if (System.IO.File.Exists(e.ClickedItem.Text))
                loadfile(e.ClickedItem.Text);
            else
            {
                status(e.ClickedItem.Text + " not found, removing from recent items.");
                reslist.DropDown.Items.Remove(e.ClickedItem);
            }
        }

        void saverecentfiles()
        {
            string s = "";
            for (int i = 0; i < recent.DropDownItems.Count; i++)
                if (recent.DropDownItems[i].Text!="")
                    s += recent.DropDownItems[i].Text + ",";
            Kadina.Properties.Settings.Default.recentfiles = s;
        }

        const string BROWSEMENU = "Browse";
        const string CLEARRECENTDATA = "Clear";
        void restorerecentfiles()
        {
            recent.DropDownItems.Add(BROWSEMENU, null, new EventHandler(browserecent));
            recent.DropDownItems.Add(CLEARRECENTDATA, null, new EventHandler(clearrecentdata));
            string[] r = Kadina.Properties.Settings.Default.recentfiles.Split(',');
            for (int i = 0; i < r.Length; i++)
                if ((r[i]!="") && System.IO.File.Exists(r[i]))
                    if (isTIK(r[i]))
                        recent.DropDownItems.Add(r[i]);
        }

        void clearrecentdata(object o, EventArgs e)
        {
            recent.DropDownItems.Clear();
            Properties.Settings.Default.recentfiles = string.Empty;
            Properties.Settings.Default.Save();
            restorerecentfiles();
        }

        void saverecentlibs()
        {
            string s = "";
            for (int i = 0; i < reslist.DropDownItems.Count; i++)
                if (reslist.DropDownItems[i].Text != "")
                    s += reslist.DropDownItems[i].Text + ",";
            Kadina.Properties.Settings.Default.recentresponselibs= s;
        }

        void restorerecentlibs()
        {
            reslist.DropDownItems.Add(BROWSEMENU, null, new EventHandler(browselibs));
            string[] r = Kadina.Properties.Settings.Default.recentresponselibs.Split(',');
            for (int i = 0; i < r.Length; i++)
                if ((r[i] != "") && System.IO.File.Exists(r[i]))
                    reslist.DropDownItems.Add(r[i]);
        }

        void browserecent(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "TickFiles|"+TikConst.WILDCARD_EXT+"|AllFiles|*.*";
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in of.FileNames)
                    loadfile(f);
            }
        }

        void browselibs(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Responses Libraries|*.dll|AllFiles|*.*";
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in of.FileNames)
                    loadfile(f);
            }
        }

        void pg_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void og_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void fg_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void ig_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void dg_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void onemin_Click(object sender, EventArgs e)
        {
            playto(PlayTo.OneMin);
        }

        private void fivemin_Click(object sender, EventArgs e)
        {
            playto(PlayTo.FiveMin);
        }

        private void tenmin_Click(object sender, EventArgs e)
        {
            playto(PlayTo.TenMin);
        }

        private void thirtymin_Click(object sender, EventArgs e)
        {
            playto(PlayTo.HalfHour);
        }

        private void onehour_Click(object sender, EventArgs e)
        {
            playto(PlayTo.Hour);
        }

        private void ptend_Click(object sender, EventArgs e)
        {
            playto(PlayTo.End);
        }

        private void onesec_Click(object sender, EventArgs e)
        {
            playto(PlayTo.OneSec);
        }

        public static void success(string u, string p)
        {
            Properties.Settings.Default.user = u;
            Properties.Settings.Default.pw = p;
            Properties.Settings.Default.Save();
        }
    }

    enum PlayTo
    {
        LastPlayTo,
        OneSec = 1,
        ThirtySec = 30,
        OneMin = 10,
        FiveMin = 50,
        TenMin = 100,
        HalfHour = 300,
        Hour = 1000,
        End,

    }
}