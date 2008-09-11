using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using System.IO;

namespace Kadina
{
    public partial class kadinamain : Form
    {
        string tickfile = "";
        string boxdll = "";
        string boxname = "";
        Stock stock;
        Broker broker = new Broker();
        Box mybox;
        System.IO.StreamReader sr;
        PlayTo pt = PlayTo.TenTrade;
        public event TickDelegate KadTick;
        BarList bl;
        DataTable dt = new DataTable("ticktable");
        DataTable it = new DataTable("itable");
        DataTable ptab = new DataTable("ptable");
        DataGrid pg = new DataGrid();
        DataGrid ig = new DataGrid();
        DataGrid dg = new DataGrid();
        DataTable ot = new DataTable("otable");
        DataTable ft = new DataTable("ftable");
        DataGrid og = new DataGrid();
        DataGrid fg = new DataGrid();
        List<string> exfilter = new List<string>();
        BackgroundWorker bw = new BackgroundWorker();


        bool boxdebugs = Kadina.Properties.Settings.Default.boxdebugs;

        public kadinamain()
        {
            InitializeComponent();
            boxlist.DropDownItemClicked += new ToolStripItemClickedEventHandler(boxlist_DropDownItemClicked);
            playtobut.DropDownItemClicked += new ToolStripItemClickedEventHandler(playtobut_DropDownItemClicked);
            broker.GotOrder += new OrderDelegate(broker_GotOrder);
            broker.GotFill += new FillDelegate(broker_GotFill);
            KadTick += new TickDelegate(kadinamain_KadTick);
            InitPlayTo();
            InitTickGrid();
            InitPGrid();
            InitOFGrids();
            InitContext();
            restorerecent();
            FormClosing += new FormClosingEventHandler(kadinamain_FormClosing);
            bw.DoWork += new DoWorkEventHandler(Play);
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PlayComplete);
            status(Util.TLSIdentity());
        }



        void kadinamain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saverecent();
            Kadina.Properties.Settings.Default.boxdebugs = boxdebugs;
            Kadina.Properties.Settings.Default.Save();
        }

        void InitContext()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("Play", new EventHandler(rightplay));
            ContextMenu.MenuItems.Add("Reset", new EventHandler(rightreset));
            ContextMenu.MenuItems.Add("Debugs", new EventHandler(rightdebugs));
        }
        void rightdebugs(object sender, EventArgs e)
        {
            boxdebugs = !boxdebugs;
            if (boxdebugs) status("Box debugging enabled.");
            else status("Box debugging disabled");
            if (mybox != null)
                mybox.Debug = boxdebugs;
        }

        void rightplay(object sender, EventArgs e)
        {
            if (sr == null) { status("You must select a tickfile to play."); return; }
            if (!igridinit) InitIGrid();
            bw.RunWorkerAsync(pt);
            ContextMenu.MenuItems.Add("Cancel", new EventHandler(rightcancel));
            status("Playing...");
        }
        void rightcancel(object sender, EventArgs e) { bw.CancelAsync(); }

        void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error!=null)
            {
                debug(e.Error.Message);
                status("Terminated because of an Exception.  See messages.");
            }
            else if (e.Cancelled) status("Canceled play.");
            else status("Reached next " + pt.ToString() + " at time "+KadTime);
            if (ContextMenu.MenuItems.Count > 3) // remove cancel option
                ContextMenu.MenuItems.RemoveAt(ContextMenu.MenuItems.Count - 1); 
        }

        string KadTime
        {
            get
            {
                if (dt.Rows.Count > 0) return dt.Rows[dt.Rows.Count - 1]["Time"].ToString();
                return "(none)";
            }
        }

        void rightreset(object sender, EventArgs e)
        {
            igridinit = false;
            if (sr!=null) sr.Close();
            msgbox.Clear();
            dt.Clear();
            broker.Reset();
            ptab.Clear();
            ot.Clear();
            ft.Clear();
            tabControl1.Refresh();
            if (it != null) { it.Clear(); it.Columns.Clear(); }
            loadfile(tickfile);
            loadfile(boxdll);
            loadboxname(boxname);
            if (mybox!=null) mybox.Reset();
            status("Reset box " + mybox.Name + " for stock " + stock.Symbol);
        }

        bool igridinit = false;

        void NewIRow(object[] values)
        {
            if (ig.InvokeRequired)
            {
                try
                {
                    Invoke(new ObjectArrayDelegate(NewIRow), values);
                }
                catch (ObjectDisposedException) { return; }
            }
            else
                it.Rows.Add(values);
        }

        void InitPGrid()
        {
            ptab.Columns.Add("Time");
            ptab.Columns.Add("Side");
            ptab.Columns.Add("Size");
            ptab.Columns.Add("AvgPrice");
            ptab.Columns.Add("Profit");
            ptab.Columns.Add("Points");
            pg.Parent = postab;
            pg.DataSource = ptab;
            pg.RowHeadersVisible = false;
            pg.CaptionVisible = false;
            pg.ContextMenu = ContextMenu;
            pg.MouseUp += new MouseEventHandler(pg_MouseUp);
            pg.FlatMode = true;
            pg.HeaderBackColor = pg.BackColor;
            pg.HeaderForeColor = pg.ForeColor;
            pg.ReadOnly = true;
            pg.Dock = DockStyle.Fill;
            pg.Show();
        }

        void InitOFGrids()
        {
            ot.Columns.Add("Time");
            ot.Columns.Add("Side");
            ot.Columns.Add("Size");
            ot.Columns.Add("Price");
            og.Parent = ordertab;
            og.DataSource = ot;
            og.RowHeadersVisible = false;
            og.CaptionVisible = false;
            og.ContextMenu = ContextMenu;
            og.MouseUp +=new MouseEventHandler(og_MouseUp);
            og.HeaderBackColor = og.BackColor;
            og.HeaderForeColor = og.ForeColor;
            og.ReadOnly = true;
            og.Dock = DockStyle.Fill;
            og.Show();
            ft.Columns.Add("xTime");
            ft.Columns.Add("xSide");
            ft.Columns.Add("xSize");
            ft.Columns.Add("xPrice");
            fg.Parent = filltab;
            fg.DataSource = ft;
            fg.RowHeadersVisible = false;
            fg.CaptionVisible = false;
            fg.ContextMenu = ContextMenu;
            fg.MouseUp += new MouseEventHandler(fg_MouseUp);
            fg.HeaderBackColor = fg.BackColor;
            fg.HeaderForeColor = fg.ForeColor;
            fg.ReadOnly = true;
            fg.Dock = DockStyle.Fill;
            fg.Show();
        }

        void fg_MouseUp(object sender, MouseEventArgs e)
        {
            System.Drawing.Point pt = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = fg.HitTest(pt);
            if (hti.Type == DataGrid.HitTestType.Cell)
            {
                fg.CurrentCell = new System.Windows.Forms.DataGridCell(hti.Row, hti.Column);
                fg.Select(hti.Row);
                if (mybox.hasIndicators && (it.Rows.Count > hti.Row)) ig.Select(hti.Row);
            }    
        }

        void og_MouseUp(object sender, MouseEventArgs e)
        {
            System.Drawing.Point pt = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = og.HitTest(pt);
            if (hti.Type == DataGrid.HitTestType.Cell)
            {
                og.CurrentCell = new System.Windows.Forms.DataGridCell(hti.Row, hti.Column);
                og.Select(hti.Row);
                if (mybox.hasIndicators && (it.Rows.Count > hti.Row)) ig.Select(hti.Row);
            }    
        }


        void InitIGrid()
        {
            if ((mybox == null) || !mybox.hasIndicators) 
                return;
            for (int i = 0; i < mybox.IndicatorNames.Length; i++)
                it.Columns.Add(mybox.IndicatorNames[i]);

            ig.Parent = itab;
            ig.DataSource = it;
            ig.RowHeadersVisible = false;
            ig.CaptionVisible = false;
            ig.ContextMenu = this.ContextMenu;
            ig.HeaderBackColor = ig.BackColor;
            ig.HeaderForeColor = ig.ForeColor;
            ig.MouseUp += new MouseEventHandler(ig_MouseUp);
            ig.FlatMode = true;
            ig.ReadOnly = true;
            ig.Dock = DockStyle.Fill;
            ig.Show();
            igridinit = true;
        }
        void InitTickGrid()
        {
            dt.Columns.Add("Time", "".GetType());
            dt.Columns.Add("Trade", new Decimal().GetType());
            dt.Columns.Add("TSize", new Int32().GetType());
            dt.Columns.Add("Bid", new Decimal().GetType());
            dt.Columns.Add("Ask", new Decimal().GetType());
            dt.Columns.Add("BSize", new Int32().GetType());
            dt.Columns.Add("ASize", new Int32().GetType());
            dt.Columns.Add("Flags", "".GetType());
            dg.TableStyles.Clear();
            dg.TableStyles.Add(new DataGridTableStyle());
            dg.TableStyles[0].GridColumnStyles.Clear();
            dg.TableStyles[0].GridColumnStyles.Add(new DataGridCell());
            dg.ContextMenu = this.ContextMenu;
            dg.DataSource = dt;
            dg.Parent = ticktab;
            dg.RowHeadersVisible = false;
            dg.CaptionVisible = false;
            dg.HeaderBackColor = dg.BackColor;
            dg.HeaderForeColor = dg.ForeColor;
            dg.MouseUp += new MouseEventHandler(dg_MouseUp);
            dg.FlatMode = true;
            dg.ReadOnly = true;
            dg.Dock = DockStyle.Fill;
            dg.Show();
        }

        void dg_MouseUp(object sender, MouseEventArgs e)
        {
            System.Drawing.Point pt = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = dg.HitTest(pt);
            if (hti.Type == DataGrid.HitTestType.Cell)
            {
                dg.CurrentCell = new System.Windows.Forms.DataGridCell(hti.Row, hti.Column);
                dg.Select(hti.Row);
                if (mybox.hasIndicators && (it.Rows.Count>hti.Row)) ig.Select(hti.Row);
            }            
        }

        void ig_MouseUp(object sender, MouseEventArgs e)
        {
            System.Drawing.Point pt = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = ig.HitTest(pt);
            if (hti.Type == DataGrid.HitTestType.Cell)
            {
                ig.CurrentCell = new System.Windows.Forms.DataGridCell(hti.Row, hti.Column);
                ig.Select(hti.Row);
                if (dg.VisibleRowCount < hti.Row)
                    dg.Select(hti.Row);
            }
        }


        void pg_MouseUp(object sender, MouseEventArgs e)
        {
            System.Drawing.Point point = new Point(e.X, e.Y);
            DataGrid.HitTestInfo hti = pg.HitTest(point);
            if (hti.Type == DataGrid.HitTestType.Cell)
                pg.Select(hti.Row);
        }



        void InitPlayTo()
        {
            string [] list = Enum.GetNames(typeof(PlayTo));
            playtobut.DropDownItems.Clear();
            for (int i = 0; i < list.Length; i++)
                playtobut.DropDownItems.Add(list[i]);
        }

        void broker_GotFill(Trade t)
        {
            ft.Rows.Add(t.xtime.ToString() + "." + t.xsec.ToString(), (t.Side ? "BUY" : "SELL"),t.xsize, t.xprice);
        }

        void broker_GotOrder(Order o)
        {
            ot.Rows.Add(o.time, (o.side ? "BUY" : "SELL"), o.size, o.price);
        }
        string nowtime = "0";

        void kadinamain_KadTick(Tick t)
        {
            if ((t.sym == "") || (t.sym!=stock.Symbol)) return;
            // get stats prior to execution
            decimal cpl = broker.GetClosedPL(t.sym);
            decimal cpt = broker.GetClosedPT(t.sym);
            int x = broker.Execute(t);
            if (x != 0) // mark tick/row if an execution happened
                xrows.Add(dt.Rows.Count - 1);

            nowtime = t.time.ToString() + ":" + t.sec.ToString();

            if (bl == null) 
                bl = new BarList(BarInterval.FiveMin, t.sym);
            bl.AddTick(t);
            Order o = new Order();
            Position mypos = broker.GetOpenPosition(t.sym);
            if (mybox != null)
            {
                o = mybox.Trade(t, bl, mypos, BoxInfo.FromBarList(bl));
                mybox_IndicatorUpdate(mybox.Indicators);
            }

            if (o.isValid) // mark tick/row if an order happened
                orows.Add(dt.Rows.Count - 1);
            broker.sendOrder(o);
            string flags = "";
            flags += o.isValid ? "O" : "";
            flags += x > 0 ? "X" : "";
            
            // tick grid
            NewTRow(new object[] { nowtime, t.trade, t.size, t.bid, t.ask, t.bs, t.os, flags });
            // position grid
            if (x != 0)
            {
                // get difference between stats pre and stats post
                cpl = broker.GetClosedPL(t.sym) - cpl;
                cpt = broker.GetClosedPT(t.sym) - cpt;
                ptab.Rows.Add(nowtime, (mypos.Flat ? "FLAT" : (mypos.Side ? "LONG" : "SHORT")), mypos.Size, mypos.AvgPrice, cpl.ToString("C2"),cpt.ToString("N1"));
            }
        }

        void NewTRow(object[] values)
        {
            if (dg.InvokeRequired)
                Invoke(new ObjectArrayDelegate(NewTRow), new object[] { values });
            else
                dt.Rows.Add(values);
        }


        List<int> xrows = new List<int>();
        List<int> orows = new List<int>();

        void Play(object sender, DoWorkEventArgs e)
        {
            PlayTo type = (PlayTo)e.Argument;
            if (e.Cancel) return;
            // set our counter limits (0 = not used)
            int t = (int)type;
            int maxtick = (t>=0) && (t<32)? t: 0;
            int maxquote = (t > 31) && (t < 64) ? t - 31 : 0;
            int maxtrade = (t > 63) && (t < 128) ? t - 63 : 0;
            int maxmin = (t > 127) && (t < 450) ? t - 127 : 0;
            int quote = 0, tick = 0, trade = 0;
            DateTime start = DateTime.Now;
            bool needtoset = true;
            int itime = 0;
            while (!sr.EndOfStream)
            {
                if (e.Cancel) break;
                Tick tk = eSigTick.FromStream(stock.Symbol,sr);
                if (tk.isTrade && !isDesiredExchange(tk.ex)) continue;
                else if (tk.hasBid && !isDesiredExchange(tk.be)) continue;
                else if (tk.hasAsk && !isDesiredExchange(tk.oe)) continue;
                if (needtoset) { start = Util.ToDateTime(tk.time, tk.sec); needtoset = false; }
                if ((mybox!=null) && ((itime == 0) || (itime != tk.time)))
                {
                    // load all the indicies for this time
                    List<Index> itix = FetchIdx(tk.time);
                    itime = tk.time;
                    // send them to the box (before we send the tix)
                    for (int id = 0; id < itix.Count; id++)
                        mybox.NewIndex(itix[id]);
                }
                if (KadTick!=null) KadTick(tk);
                tick++;
                if (tk.isTrade) trade++;
                if (tk.isQuote) quote++;
                if ((type == PlayTo.Time) && (time != 0) && (tk.time == time)) break;
                TimeSpan since = new TimeSpan(Util.ToDateTime(tk.time, tk.sec).Ticks - start.Ticks);
                if ((maxmin!=0) && ((int)since.TotalMinutes>=maxmin)) break;
                if ((maxtick != 0) && (tick >= maxtick)) break;
                if ((maxtrade != 0) && (trade >= maxtrade)) break;
                if ((maxquote != 0) && (quote >= maxquote)) break;
            }
        }

        int time = 0;
        void playtobut_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            pt = (PlayTo)Enum.Parse(typeof(PlayTo), e.ClickedItem.Text);
            string extra = "";
            if (pt == PlayTo.Time)
            {
                extra = Microsoft.VisualBasic.Interaction.InputBox("When should Kadina stop reading ticks?  Enter a time in TLTime.  930 = 9:30AM, 1459 = 2:59PM", "PlayTo Time", "0", 0, 0);
                try
                {
                    time = Convert.ToInt32(extra);
                    DateTime dt = Util.ToDateTime(time, 0); // this should throw exceptions for bad times
                }
                catch (Exception) { status("You entered an invalid time."); return; }
            }
            else time = 0;
            status("Playing to next " + e.ClickedItem.Text+" "+extra);
        }

        bool isDesiredExchange(string name)
        {
            foreach (ToolStripMenuItem m in filter.DropDown.Items)
                if (m.Checked && name.Contains(m.Text)) return true;
            return false;
        }

        void boxlist_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            boxname = e.ClickedItem.Text;
            loadboxname(boxname);
        }

        void loadboxname(string name)
        {
            try
            {
                mybox = Box.FromDLL(name, boxdll);
            }
            catch (Exception ex) { debug(ex.Message); debug("Error, quitting..."); return; }
            if ((mybox != null) && (mybox.FullName == name))
            {
                mybox.Debug = boxdebugs;
                mybox.GotDebug += new DebugFullDelegate(mybox_GotDebug);
                mybox.CancelOrderSource += new UIntDelegate(mybox_CancelOrderSource);
                broker.GotOrder+=new OrderDelegate(mybox.gotOrderSink);
                broker.GotOrderCancel += new Broker.OrderCancelDelegate(broker_GotOrderCancel);
                status(boxname + " is current box.");
            }
            else status("Box did not load.");

        }

        void broker_GotOrderCancel(string sym, bool side, uint id)
        {
            if (mybox != null)
                mybox.gotCancelSink(id);
        }

        void mybox_CancelOrderSource(uint number)
        {
            broker.CancelOrder(number);
        }

        void mybox_GotDebug(Debug msg)
        {
            if (msg.Level == DebugLevel.Debug)
                debug(msg.Msg);
            else if (msg.Level == DebugLevel.Status)
                status(msg.Msg);
        }

        void mybox_IndicatorUpdate(object[] parameters)
        {
            if ((mybox == null) || !mybox.hasIndicators) return;
            NewIRow(new object[] { parameters });
        }


        private void kadinamain_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            string []s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string f = s[0];
            loadfile(f);
        }



         private bool loadfile(string path)
         {
             string f = path;
            if (isBOX(f))
            {
                boxdll = f;
                boxlist.DropDownItems.Clear();
                List<string> l = Util.GetBoxList(boxdll);
                if (System.IO.File.Exists(f))
                    if (!isRecent(f))
                        recent.DropDownItems.Add(f);
                for (int i = 0; i < l.Count; i++)
                    boxlist.DropDownItems.Add(l[i]);
                status("Found " + l.Count + " boxes in DLL.");
                return true;
            }
            else if (isEPF(f))
            {
                tickfile = f;
                sr = new System.IO.StreamReader(f);
                stock = eSigTick.InitEpf(sr);
                if (System.IO.File.Exists(f))
                    if (!isRecent(f))
                        recent.DropDownItems.Add(f);
                status("Loaded "+stock.Symbol+" for "+Util.ToDateTime(stock.Date));
                LoadIndexFiles(stock.Date);
                return true;
            }

            return false;

        }
        bool isRecent(string path)
        {
            for (int i = 0; i < recent.DropDownItems.Count; i++)
                if (recent.DropDownItems[i].Text.Equals(path))
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
                Invoke(new DebugDelegate(debug), new object[] { msg });
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

        bool isEPF(string path) { return path.Contains("EPF")||path.Contains("epf"); }
        bool isBOX(string path) { return path.Contains("DLL")||path.Contains("dll"); }

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

        void saverecent()
        {
            string s = "";
            for (int i = 0; i < recent.DropDownItems.Count; i++)
                if (recent.DropDownItems[i].Text!="")
                    s += recent.DropDownItems[i].Text + ",";
            Kadina.Properties.Settings.Default.recentfiles = s;
        }

        void restorerecent()
        {
            recent.DropDownItems.Add("Browse", null, new EventHandler(browserecent));
            string[] r = Kadina.Properties.Settings.Default.recentfiles.Split(',');
            for (int i = 0; i < r.Length; i++)
                if ((r[i]!="") && System.IO.File.Exists(r[i]))
                    recent.DropDownItems.Add(r[i]);
        }

        void browserecent(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Box DLL (*.dll)|*.dll|TickFile (*.EPF)|*.EPF";
            of.Multiselect = true;
            if (of.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in of.FileNames)
                    loadfile(f);
            }
        }

        private void filter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)e.ClickedItem;
            if (m.Checked)
                exfilter.Add(m.Text);
            else exfilter.Remove(m.Text);
        }

        void LoadIndexFiles(int date)
        {
            idxstream.Clear();
            Dictionary<string, StreamReader> idxfile = new Dictionary<string, StreamReader>();
            DirectoryInfo di = new DirectoryInfo(Util.TLTickDir);
            FileInfo[] files = di.GetFiles("*" + date + "*.idx");
            for (int i = 0; i < files.Length; i++)
                if (!idxfile.ContainsKey(files[i].FullName))
                    idxfile.Add(files[i].FullName, new StreamReader(files[i].FullName));
                else idxfile[files[i].FullName] = new StreamReader(files[i].FullName);
            debug("Preparing " + idxfile.Count + " indicies: ");
            string sym = "";
            foreach (string stock in idxfile.Keys)
            {
                if (!idxstream.ContainsKey(stock))
                    idxstream.Add(stock, new List<Index>());
                while (!idxfile[stock].EndOfStream)
                { // read every index into memory
                    string line = idxfile[stock].ReadLine();
                    Index fi = Index.Deserialize(line);
                    idxstream[stock].Add(fi);
                    if (!sym.Contains(fi.Name)) sym += fi.Name;
                }
            }
            debug(sym);
        }


        Dictionary<string, List<Index>> idxstream = new Dictionary<string, List<Index>>();

        List<Index> FetchIdx(int time)
        {
            List<Index> res = new List<Index>();
            foreach (string file in idxstream.Keys)
            {
                for (int i = 0; i < idxstream[file].Count; i++)
                    if (idxstream[file][i].Time == time)
                        res.Add(idxstream[file][i]);
            }
            return res;
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
        Tick = 0,
        TenTick = 9,
        Quote = 32,
        TenQuote =41,
        Trade = 64,
        TenTrade = 73,
        OneMin = 128,
        FiveMin = 131,
        FifteenMin = 141,
        HalfHour = 156,
        Hour = 186,
        TwoHour = 246,
    }
}