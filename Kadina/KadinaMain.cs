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
        HistSim h = new HistSim();

       

        public kadinamain()
        {
            InitializeComponent();
            reslist.DropDownItemClicked += new ToolStripItemClickedEventHandler(boxlist_DropDownItemClicked);
            playtobut.DropDownItemClicked += new ToolStripItemClickedEventHandler(playtobut_DropDownItemClicked);
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
            debug("1. Drag and drop tick files, or select from Recent menu.");
            debug("2. Drag and drop DLL containing Responses to test, or select from Recent menu.");
            debug("3. Select a Response from Responses list.");
            debug("4. PlayTo time.");
            debug("5. Finally right click and click play to play to that time.");
        }

        void h_GotTick(Tick t)
        {
            // get time for display
            nowtime = t.time.ToString();
            
            // don't display ticks for unmatched exchanges
            if (t.isTrade && !isDesiredExchange(t.ex)) return;
            else if (t.hasBid && !isDesiredExchange(t.be)) return;
            else if (t.hasAsk && !isDesiredExchange(t.oe)) return;
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
            h.PlayTo(rem);
        }



        void kadinamain_FormClosing(object sender, FormClosingEventArgs e)
        {
            saverecent();
            Kadina.Properties.Settings.Default.Save();
        }

        void InitContext()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("Play", new EventHandler(rightplay));
            ContextMenu.MenuItems.Add("Reset", new EventHandler(rightreset));
        }


        void rightplay(object sender, EventArgs e)
        {
            if (epffiles.Count==0) { status("You must select a tickfile to play."); return; }
            if (myres == null) { status("You must drop a box dll AND select a specific box from the Boxes menu."); return; }
            if (!igridinit) InitIGrid();
            if (bw.IsBusy) { status("Play is already running..."); return; }
            bw.RunWorkerAsync(pt);
            ContextMenu.MenuItems.Add("Cancel", new EventHandler(rightcancel));
            status("Playing...");
        }
        void rightcancel(object sender, EventArgs e) { bw.CancelAsync(); }

        void rightreset(object sender, EventArgs e)
        {
            igridinit = false;
            if (h!=null) h.Reset();
            msgbox.Clear();
            dt.Clear();
            ptab.Clear();
            ot.Clear();
            ft.Clear();
            tabControl1.Refresh();
            if (it != null) { it.Clear(); it.Columns.Clear(); }
            loadfile(tickfile);
            loadfile(responsedll);
            loadboxname(resname);
            if (myres!=null) myres.Reset();
            status("Reset box " + myres.Name + " "+PrettyEPF());
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
            ptab.Columns.Add("Symbol");
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
            ot.Columns.Add("Symbol");
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
            ft.Columns.Add("Symbol");
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
                if (it.Rows.Count > hti.Row) ig.Select(hti.Row);
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
                if (it.Rows.Count > hti.Row) ig.Select(hti.Row);
            }    
        }


        void InitIGrid()
        {
            if ((myres == null) || (myres.Indicators.Length==0))
                return;
            for (int i = 0; i < myres.Indicators.Length; i++)
                it.Columns.Add(myres.Indicators[i]);

            msgbox.Clear(); // clear the message box on first box run
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
                if (it.Rows.Count>hti.Row) ig.Select(hti.Row);
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
            if (dg.InvokeRequired)
            {
                try
                {
                    Invoke(new StringArrayDelegate(NewTRow), new object[] { values });
                }
                catch (ObjectDisposedException) { }
            }
            else
                dt.Rows.Add(values);
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
                    DateTime dt = Util.TLT2DT(time); // this should throw exceptions for bad times
                }
                catch (Exception) { status("You entered an invalid time."); return; }
            }
            else time = 0;
            status("Playing to next " + e.ClickedItem.Text+" "+extra);
        }

        bool isDesiredExchange(string name)
        {
            foreach (ToolStripMenuItem m in filter.DropDown.Items)
                if (m.Checked && !name.Contains(m.Text)) return false;
            return true;
        }

        void boxlist_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            resname = e.ClickedItem.Text;
            loadboxname(resname);
        }

        void loadboxname(string name)
        {
            try
            {
                myres = ResponseLoader.FromDLL(name, responsedll);
            }
            catch (Exception ex) { debug(ex.Message); debug("Error, quitting..."); return; }
            if ((myres != null) && (myres.FullName == name))
            {
                myres.SendDebug += new DebugFullDelegate(myres_GotDebug);
                myres.SendCancel += new UIntDelegate(myres_CancelOrderSource);
                myres.SendOrder += new OrderDelegate(myres_SendOrder);
                myres.SendIndicators += new StringParamDelegate(myres_SendIndicators); 
                h.SimBroker.GotOrder += new OrderDelegate(myres.GotOrder);
                h.SimBroker.GotFill += new FillDelegate(myres.GotFill);
                h.GotTick += new TickDelegate(myres.GotTick);
                status(resname + " is current response.");
                updatetitle();
            }
            else status("Response did not load.");

        }
        const string PROGRAM = "Kadina";
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
                reslist.DropDownItems.Clear();
                List<string> l = Util.GetResponseList(responsedll);
                if (System.IO.File.Exists(f))
                    if (!isRecent(f))
                        recent.DropDownItems.Add(f);
                for (int i = 0; i < l.Count; i++)
                    reslist.DropDownItems.Add(l[i]);
                status("Found " + l.Count + " responses.  Please select one from Responses drop-down.");
                return true;
            }
            else if (isEPF(f))
            {

                if (System.IO.File.Exists(f))
                    if (!isRecent(f) && Util.SecurityFromFileName(f).isValid)
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

        bool isEPF(string path) { return path.Contains("EPF")||path.Contains("epf"); }
        bool isResponse(string path) { return path.Contains("DLL")||path.Contains("dll"); }

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
            of.Filter = "Responses and TickFiles (*.dll;*.epf)|*.dll;*.epf|AllFiles|*.*";
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