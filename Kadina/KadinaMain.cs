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
    public partial class kadinamain : Form
    {
        SecurityImpl sec = null;
        string tickfile = "";
        string responsedll = "";
        string resname = "";
        Response myres;
        PlayTo pt = PlayTo.OneMin;

        ResponseList _rl = new ResponseList();

        DataTable dt = new DataTable("ticktable");
        DataTable it = new DataTable("itable");
        DataTable ptab = new DataTable("ptable");
        SafeBindingSource tbs = new SafeBindingSource();
        SafeBindingSource ibs = new SafeBindingSource();
        DataGridView pg = new DataGridView();
        DataGridView ig = new DataGridView();
        DataGridView dg = new DataGridView();

        DataTable ot = new DataTable("otable");
        SafeBindingSource obs = new SafeBindingSource();
        DataTable ft = new DataTable("ftable");
        SafeBindingSource fbs = new SafeBindingSource();
        DataGridView og = new DataGridView();
        DataGridView fg = new DataGridView();

        BackgroundWorker bw = new BackgroundWorker();
        HistSim h = new HistSim();
       

        public kadinamain()
        {

            InitializeComponent();
            initgrids();
            InitContext();
            restorerecentfiles();
            restorerecentlibs();
            FormClosing += new FormClosingEventHandler(kadinamain_FormClosing);
            bw.DoWork += new DoWorkEventHandler(Play);
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PlayComplete);
            status(Util.TLSIdentity());
        }


        void h_GotTick(Tick t)
        {
            // get time for display
            nowtime = t.time.ToString();
            
            // don't display ticks for unmatched exchanges
            string time = t.time.ToString();
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
                trade = t.trade.ToString("N2");
            }
            else if (t.isTrade)
            {
                trade = t.trade.ToString("N2");
                ts = t.size.ToString();
                ex = t.ex;
            }
            if (t.hasBid)
            {
                bs = t.bs.ToString();
                be = t.be;
                bid = t.bid.ToString("N2");
            }
            if (t.hasAsk)
            {
                ask = t.ask.ToString("N2");
                oe = t.oe;
                os = t.os.ToString();
            }
            
            // add tick to grid
            NewTRow(new string[] { nowtime,t.symbol,trade,ts,bid,ask,bs,os,ex,be,oe});
        }

        void Play(object sender, DoWorkEventArgs e)
        {
            PlayTo type = (PlayTo)e.Argument;
            if (e.Cancel) return;
            int t = (int)type;
            h.Initialize();
            int maxmin = (t > 127) && (t < 450) ? t - 127 : 0;
            long firsttime = h.NextTickTime % 1000000;
            long rem = h.NextTickTime - firsttime;
            int stop = Util.FTADD((int)firsttime, maxmin * 60);
            rem += stop;
            cleardebugs(); // clear the message box on first box run
            h.PlayTo(rem);
        }

        void cleardebugs()
        {
            if (msgbox.InvokeRequired)
                msgbox.Invoke(new VoidDelegate(cleardebugs));
            else
            {
                msgbox.Clear(); 
            }
        }



        void kadinamain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saverecentlibs();
            saverecentfiles();
            Kadina.Properties.Settings.Default.Save();
        }
        const string PLAYTO = "Play +";
        const string PLAYTOUNIT = " min";
        void InitContext()
        {
            ContextMenu = new ContextMenu();
            string[] list = Enum.GetNames(typeof(PlayTo));
            for (int i = 0; i < list.Length; i++)
                ContextMenu.MenuItems.Add(PLAYTO+list[i]+PLAYTOUNIT,new EventHandler(rightplay));
            ContextMenu.MenuItems.Add("Reset", new EventHandler(rightreset));
            msgbox.ContextMenu = ContextMenu;
            
        }


        void rightplay(object sender, EventArgs e)
        {
            if (epffiles.Count==0) { status("No data selected."); return; }
            if (myres == null) { status("No response selected."); return; }
            if (bw.IsBusy) { status("Still playing, please wait..."); return; }
            bw.RunWorkerAsync(pt);
            ContextMenu.MenuItems.Add("Cancel", new EventHandler(rightcancel));
            status("Playing...");
        }
        void rightcancel(object sender, EventArgs e) { bw.CancelAsync(); }

        void rightreset(object sender, EventArgs e)
        {
            if (h!=null) h.Reset();
            msgbox.Clear();
            dt.Clear();
            ptab.Clear();
            ot.Clear();
            ft.Clear();
            tabControl1.Refresh();
            if (it != null) { it.Clear(); it.Columns.Clear(); }
            h.Reset();
            loadboxname(resname);
            if (myres!=null) myres.Reset();
            status("Reset box " + myres.Name + " "+PrettyEPF());
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
            pg.Dock = DockStyle.Fill;
            pg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            pg.Show();

            // order tab
            ot.Columns.Add("Time");
            ot.Columns.Add("Symbol");
            ot.Columns.Add("Side");
            ot.Columns.Add("Size");
            ot.Columns.Add("Price");
            og.Parent = ordertab;
            obs.DataSource = ot;
            og.DataSource = obs;
            og.RowHeadersVisible = false;
            og.ContextMenu = ContextMenu;
            og.ReadOnly = true;
            og.AllowUserToAddRows = false;
            og.AllowUserToDeleteRows = false;
            og.ShowEditingIcon = false;
            og.Dock = DockStyle.Fill;
            og.BackgroundColor = BackColor;
            og.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            og.Show();

            // fill tab
            ft.Columns.Add("xTime");
            ft.Columns.Add("Symbol");
            ft.Columns.Add("xSide");
            ft.Columns.Add("xSize");
            ft.Columns.Add("xPrice");
            fg.Parent = filltab;
            fbs.DataSource = ft;
            fg.DataSource = fbs;
            fg.RowHeadersVisible = false;
            fg.ContextMenu = ContextMenu;
            fg.ReadOnly = true;
            fg.Dock = DockStyle.Fill;
            fg.AllowUserToAddRows = false;
            fg.AllowUserToDeleteRows = false;
            fg.ShowEditingIcon = false;
            fg.BackgroundColor = BackColor;
            fg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg.ReadOnly = true;
            dg.Dock = DockStyle.Fill;
            dg.Show();
            ig.Parent = itab;
            ibs.DataSource = it;
            ig.DataSource = ibs;
            ig.RowHeadersVisible = false;
            ig.ContextMenu = this.ContextMenu;
            ig.ReadOnly = true;
            ig.Dock = DockStyle.Fill;
            ig.AllowUserToAddRows = false;
            ig.AllowUserToDeleteRows = false;
            ig.ShowEditingIcon = false;
            ig.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ig.BackgroundColor = BackColor;
            ig.Show();

        }

        void igridinit()
        {


            if ((myres == null) || (myres.Indicators.Length == 0))
                return;
                for (int i = 0; i < myres.Indicators.Length; i++)
                    it.Columns.Add(myres.Indicators[i]);
                ig.Invalidate();
        }


        Dictionary<string, PositionImpl> poslist = new Dictionary<string, PositionImpl>();
        void broker_GotFill(Trade t)
        {
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

            ptab.Rows.Add(nowtime, mypos.Symbol,(mypos.isFlat ? "FLAT" : (mypos.isLong ? "LONG" : "SHORT")), mypos.Size, mypos.AvgPrice.ToString("N2"), cpl.ToString("C2"), cpt.ToString("N1"));
            ft.Rows.Add(t.xtime.ToString(), t.symbol,(t.side ? "BUY" : "SELL"),t.xsize, t.xprice.ToString("N2"));
        }

        void broker_GotOrder(Order o)
        {
            ot.Rows.Add(o.time, o.symbol,(o.side ? "BUY" : "SELL"), o.size, o.isStop? o.stopp : (o.isTrail ? o.trail : o.price));
        }
        string nowtime = "0";


        delegate void StringArrayDelegate(string[] vals);
        void NewTRow(string[] values)
        {
            dt.Rows.Add(values);
        }



        int time = 0;

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
                return; 
            }
            if ((myres != null) && (myres.FullName == name))
            {
                resname = name;
                myres.SendDebug += new DebugFullDelegate(myres_GotDebug);
                myres.SendCancel += new UIntDelegate(myres_CancelOrderSource);
                myres.SendOrder += new OrderDelegate(myres_SendOrder);
                myres.SendIndicators += new StringParamDelegate(myres_SendIndicators);
                myres.SendMessage += new MessageDelegate(myres_SendMessage);
                h.SimBroker.GotOrder += new OrderDelegate(myres.GotOrder);
                h.SimBroker.GotFill += new FillDelegate(myres.GotFill);
                h.GotTick += new TickDelegate(myres.GotTick);
                status(resname + " is current response.");
                updatetitle();
                igridinit();
            }
            else status("Response did not load.");

        }

        void myres_SendMessage(MessageTypes type, uint id, string data)
        {
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
                NewIRow(new object[] { parameters });
            }
        }


        void myres_SendOrder(Order o)
        {
            h.SimBroker.sendOrder(o);
        }

        void broker_GotOrderCancel(string sym, bool side, uint id)
        {
            if (myres != null)
                myres.GotOrderCancel(id);
        }

        void myres_CancelOrderSource(uint number)
        {
            h.SimBroker.CancelOrder(number);
        }

        void myres_GotDebug(Debug msg)
        {
            debug(nowtime+":"+myres.Name+" "+msg.Msg);
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
                
                return true;
            }
            else if (isTIK(f))
            {
                if (System.IO.File.Exists(f))
                    if (!isRecentTickfile(f) && Util.SecurityFromFileName(f).isValid)
                        recent.DropDownItems.Add(f);
                epffiles.Add(f);
                h = new HistSim(epffiles.ToArray());
                h.SimBroker.GotOrder += new OrderDelegate(broker_GotOrder);
                h.SimBroker.GotFill += new FillDelegate(broker_GotFill);
                h.GotTick += new TickDelegate(h_GotTick);
                h.SimBroker.GotOrderCancel += new OrderCancelDelegate(broker_GotOrderCancel);

                updatetitle();
                status("Loaded tickdata: "+PrettyEPF());
                return true;
            }

            return false;

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
            statuslab.Text = msg;
            statusStrip1.Refresh();
        }
        void debug(string msg)
        {
            if (msgbox.InvokeRequired)
            {
                try
                {
                    Invoke(new DebugDelegate(debug), new object[] { msg });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                msgbox.AppendText(msg + Environment.NewLine);
                msgbox.Refresh();
            }
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
            if (e.Error != null)
            {
                debug(e.Error.Message);
                status("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled) status("Canceled play.");
            else status("Reached next " + pt.ToString() + " at time " + KadTime);
            if (ContextMenu.MenuItems.Count > 2) // remove cancel option
                ContextMenu.MenuItems.RemoveAt(ContextMenu.MenuItems.Count - 1);
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
            if (e.ClickedItem.Text == "Browse") return;
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
            if (e.ClickedItem.Text == "Browse") return;
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

        void restorerecentfiles()
        {
            recent.DropDownItems.Add("Browse", null, new EventHandler(browserecent));
            string[] r = Kadina.Properties.Settings.Default.recentfiles.Split(',');
            for (int i = 0; i < r.Length; i++)
                if ((r[i]!="") && System.IO.File.Exists(r[i]))
                    recent.DropDownItems.Add(r[i]);
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
            reslist.DropDownItems.Add("Browse", null, new EventHandler(browselibs));
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
    }

    enum PlayTo
    {
        // ticks get 0-31
        // quotes get 32-63
        // trades get 64-127
        // minutes get 128-up
        End = Int32.MaxValue,
        Pause = Int32.MaxValue-1,
        Time = Int32.MaxValue-2,
        OneMin = 128,
        FiveMin = 131,
        FifteenMin = 141,
        HalfHour = 156,
        Hour = 186,
        TwoHour = 246,
    }
}