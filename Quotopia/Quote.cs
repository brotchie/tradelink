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
    public delegate void QuotopiaTickDel(Tick tick);
    public delegate void OrderStatusDel(string sym, int error);
    public partial class Quote : Form
    {

        public int GetDate { get { DateTime d = DateTime.Now; int i = (d.Year * 10000) + (d.Month * 100) + d.Day; return i; } }
        public int GetTime { get { DateTime d = DateTime.Now; int i = (d.Hour*100)+(d.Minute); return i; } }
        public event QuotopiaTickDel spillTick;
        public event OrderStatusDel orderStatus;
        const string version = "1.0";
        const string build = "$Rev: 998 $";
        string Ver { get { return version + "." + Util.CleanVer(build); } }
        public Quote()
        {
            InitializeComponent();
            Size = Quotopia.Properties.Settings.Default.wsize;
            show(Text + Ver + " (Tradelink" + tl.Ver + ")");
            
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotIndexTick += new IndexDelegate(tl_gotIndexTick);
            MarketsView.MouseWheel += new MouseEventHandler(MarketsView_MouseWheel);
            MarketsView.CellFormatting += new DataGridViewCellFormattingEventHandler(MarketsView_CellFormatting);
            news.NewsEventSubscribers += new NewsDelegate(news_NewsEventSubscribers);

            UpdateBoxList();
            trendpic["uptick"] = Quotopia.Properties.Resources.uptick;
            trendpic["downtick"] = Quotopia.Properties.Resources.downtick;
            show("BrokerServer: "+tl.BrokerName);
            
        }

        DataGridViewCellStyle upstyle = new DataGridViewCellStyle();
        DataGridViewCellStyle downstyle = new DataGridViewCellStyle();
        DataGridViewCellStyle highstyle = new DataGridViewCellStyle();
        DataGridViewCellStyle lowstyle = new DataGridViewCellStyle();
        DataGridViewCellStyle firststyle = new DataGridViewCellStyle();

        void MarketsView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            DataGridView dgv = (sender as DataGridView);
            if (e.RowIndex<0) return;
            string sym = "";

            // populate initial values
            try
            {
                sym = idx[e.RowIndex];
            }
            catch (ArgumentOutOfRangeException) { return; }
            if (sym == "") return;
            string col = dgv.Columns[e.ColumnIndex].Name;

            decimal high = tl.FastHigh(sym);
            decimal low = tl.FastLow(sym);
            decimal last =0,bid = 0, ask= 0;
            try
            {
                last = tlinklist[sym].A.trade;
                bid = (tlinklist[sym].A.bid != 0) ? tlinklist[sym].A.bid : tlinklist[sym].B.bid;
                ask = tlinklist[sym].A.ask != 0 ? tlinklist[sym].A.ask : tlinklist[sym].B.ask;
            }
            catch (Exception) { }

            // coloring

            // default row color, and last print color
            if (col.Equals("LastPrintCol"))
            {
                bool ptick = false;
                bool mtick = false;
                try
                {
                    ptick = tlinklist[sym].PlusTick;
                    mtick = tlinklist[sym].MinusTick;
                }
                catch (KeyNotFoundException) { }
                // uptick/downtick coloring
                if (ptick)
                    dgv.Rows[e.RowIndex].DefaultCellStyle = upstyle;
                else if (mtick)
                    dgv.Rows[e.RowIndex].DefaultCellStyle = downstyle;

                if ((last * high != 0) && (last >= high))
                    e.CellStyle = highstyle;
                else if ((last * low != 0) && (last <= low))
                    e.CellStyle = lowstyle;
                else
                {
                    e.CellStyle = dgv.Rows[e.RowIndex].DefaultCellStyle;
                    e.CellStyle.Format = "N2";
                }
            } // bid coloring
            else if (col.Equals("BidCol"))
            {
                if ((bid * high != 0) && (bid >= high))
                    e.CellStyle = highstyle;
                else if ((bid * low != 0) && (bid <= low))
                    e.CellStyle = lowstyle;
                else
                {
                    e.CellStyle = dgv.Rows[e.RowIndex].DefaultCellStyle;
                    e.CellStyle.Format = "N2";
                }
            } // ask coloring
            else if (col.Equals("AskCol"))
            {
                if ((ask * high != 0) && (ask >= high))
                    e.CellStyle = highstyle;
                else if ((ask * low != 0) && (ask <= low))
                    e.CellStyle = lowstyle;
                else
                {
                    e.CellStyle = dgv.Rows[e.RowIndex].DefaultCellStyle;
                    e.CellStyle.Format = "N2";
                }
            }
            else if (col.Equals("xTrendCol"))
            {
                bool ptick = false;
                bool mtick = false;
                try
                {
                    ptick = tlinklist[sym].PlusTick;
                    mtick = tlinklist[sym].MinusTick;
                }
                catch (KeyNotFoundException) { }
                if (ptick) e.Value = trendpic["uptick"];
                else if (mtick) e.Value = trendpic["downtick"];
                e.FormattingApplied = true;
            }
            else if (col.Equals("TrendCol"))
            {
                TickLink t = null;
                tlinklist.TryGetValue(sym, out t);
                if (t!=null) e.Value = t.ToString();
            }
            else if (col.Equals("NetChangeCol"))
            {
                try
                {
                    e.Value = (decimal)dgv["LastPrintCol", e.RowIndex].Value - (decimal)MarketsView["YestCloseCol", e.RowIndex].Value;
                }
                catch (NullReferenceException) { }
            }
            else if (col.Equals("LastSizeCol"))
                e.CellStyle.Format = "N0";
            else if (col.Equals("MinSizeCol"))
                e.CellStyle.Format = "N0";
        }



        Dictionary<string, Icon> trendpic = new Dictionary<string, Icon>();

        void MarketsView_MouseWheel(object sender, MouseEventArgs e)
        {
            int r = MarketsView.CurrentCell.RowIndex;
            int c = MarketsView.Columns["MinSizeCol"].Index;
            int size = Convert.ToInt32(MarketsView[c, r].Value);
            int minsize = Convert.ToInt32(Properties.Settings.Default.MinimumSize);
            if (e.Delta > 0) size += minsize;
            else if (e.Delta < 0) size -= minsize;
            if (size>0) MarketsView[c, r].Value = size.ToString();
        }

        bool ArchiveIdx(Index i)
        {
            if (!saveticks.Checked) return false;
            if (!filelist.ContainsKey(i.Name))
            {
                try
                {
                    string fn = Quotopia.Properties.Settings.Default.ticfolderpath + "//" + i.Name + GetDate.ToString() + ".IDX";
                    filelist.Add(i.Name, new StreamWriter(fn, true));
                }
                catch (Exception) { return false; }
            }
            try
            {
                filelist[i.Name].WriteLine(TradeLib.Index.Serialize(i));
            }
            catch (Exception ex) { show(ex.Message); return false; }
            filelist[i.Name].Flush();
            return true;
        }


        void tl_gotIndexTick(Index idx)
        {
            ArchiveIdx(idx);
            Tick itick = new Tick(idx.Name);
            itick.trade = idx.Value;
            itick.time = idx.Time;
            itick.date = idx.Date;
            itick.size = 1; // forces tick to be recognized as trade
            if (!barlist.ContainsKey(idx.Name)) barlist.Add(idx.Name, new BarList(BarInterval.FiveMin, idx.Name));
            barlist[idx.Name].AddTick(itick);
            foreach (Box box in boxlist.Values) if (box!=null) box.NewIndex(idx);
            if (!symidx.ContainsKey(idx.Name)) return; // stock has since been removed from screen

            for (int r = 0; r<symidx[idx.Name].Count; r++)
            {
                int i = symidx[idx.Name][r];
                decimal lastval = idx.Value;

                try
                {
                    lastval = Convert.ToDecimal(MarketsView["LastPrintCol", i].Value);
                }
                catch (NullReferenceException) { }
                MarketsView["LastPrintCol", i].Value = idx.Value;
                MarketsView["DayOpenCol", i].Value = idx.Open;
                MarketsView["DayHighCol", i].Value = idx.High;
                MarketsView["DayLowCol", i].Value = idx.Low;
                MarketsView["YestCloseCol", i].Value = idx.Close;
                MarketsView["TimeCol", i].Value = idx.Time;

                MarketsView["NetChangeCol", i].Value = idx.Value - idx.Close;
            }
        }


        void news_NewsEventSubscribers(News news)
        {
            showc(news.Msg,Color.Blue);
        }


        Dictionary<string,Box> boxlist = new Dictionary<string,Box>();
        Dictionary<string, BarList> barlist = new Dictionary<string, BarList>();
        Dictionary<string, TickLink> tlinklist = new Dictionary<string, TickLink>();
        Dictionary<string, StreamWriter> filelist = new Dictionary<string, StreamWriter>();
        NewsService news = new NewsService();

        int[] getSymbolRows(string sym)
        {
            List<int> rows = new List<int>();
            try
            {
                for (int i = 0; i < MarketsView.Rows.Count; i++)
                    if (MarketsView["SymbolCol", i].Value != null) if (MarketsView["SymbolCol", i].Value.ToString().Equals(sym, StringComparison.CurrentCultureIgnoreCase)) rows.Add(i);
            }
            catch (Exception) { }
            return rows.ToArray();
        }

        void tl_gotFill(Trade t)
        {
            if ((t.symbol == null) || (t==null)) return;
            TradesView.Rows.Add(t.xdate,t.xtime,t.xsec,t.symbol,(t.side ? "BUY" : "SELL"),t.xsize,t.xprice,t.comment); // if we accept trade, add it to list
        }

        bool ArchiveTick(Tick t)
        {
            if (!saveticks.Checked) return false;
            try
            {
                filelist[t.sym].WriteLine(eSigTick.ToEPF(t));
            }
            catch (IOException ex) { showc(GetDate + ":" + GetTime + " " + ex.Message, Color.Red); return false; }
            catch (KeyNotFoundException)
            {
                if ((Quotopia.Properties.Settings.Default.ticfolderpath == "") || (Quotopia.Properties.Settings.Default.ticfolderpath == null)) Quotopia.Properties.Settings.Default.ticfolderpath = Quotopia.Properties.Resources.tickfolderpath;
                string fn = Quotopia.Properties.Settings.Default.ticfolderpath + "//" + t.sym + t.date + ".EPF";
                try
                {
                    filelist.Add(t.sym, new StreamWriter(fn, true));
                    filelist[t.sym].Write(eSigTick.EPFheader(t.sym, t.date));
                    filelist[t.sym].WriteLine(eSigTick.ToEPF(t));
                }
                catch (IOException) { return false; }
            }
            catch (Exception ex) { showc(GetDate + ":" + GetTime + " " + ex.Message, Color.Red); return false; }
            if ((t.time % 10) == 0) 
                foreach (string sym in filelist.Keys) filelist[sym].Flush(); // flush buffer to disk periodically
            return true;
        }

        bool didheartbeat = false;

        Dictionary<string, string> bslist = new Dictionary<string, string>();
        Dictionary<string, string> oslist = new Dictionary<string,string>();


        void tl_gotTick(Tick t)
        {
            if ((t.sym == null) || (t.sym=="")) return;
            List<int> rows = new List<int>();
            try
            {
                rows = symidx[t.sym];
            }
            catch (KeyNotFoundException) { return; }

            // spill tick to any open order tickets (if any exist)
            if (spillTick != null) spillTick(t);

            // save tick as link
            try
            {
                tlinklist[t.sym].Tick(t);
            }
            catch (KeyNotFoundException) { tlinklist.Add(t.sym, new TickLink()); tlinklist[t.sym].Tick(t); }

            if (tlinklist[t.sym].A.hasTick) 
                ArchiveTick(tlinklist[t.sym].A);

            // save bar
            try
            {
                barlist[t.sym].AddTick(t); // add tick to our bar for this stock
            }
            catch (KeyNotFoundException)
            {
                barlist.Add(t.sym, new BarList(BarInterval.FiveMin, t.sym));
                barlist[t.sym].AddTick(t);
            }

            // trade any boxes provided
            if (boxenablebox.Checked)
            {
                if (boxlist.ContainsKey(t.sym) && (boxlist[t.sym] != null))
                {
                    BoxInfo bi = new BoxInfo();
                    bi.High = tl.FastHigh(t.sym);
                    bi.Low = tl.FastLow(t.sym);
                    bi.Open = (decimal)MarketsView["DayOpenCol", symidx[t.sym][0]].Value;
                    bi.YestClose = (decimal)MarketsView["YestCloseCol", symidx[t.sym][0]].Value;
                    try
                    {
                        tl.SendOrder(boxlist[t.sym].Trade(tlinklist[t.sym].A, barlist[t.sym], tl.FastPos(t.sym),bi));
                    }
                    catch (Exception ex) { showc(GetDate + ":" + GetTime + " " + ex.Message, Color.Red); }
                }

            }

            // update all the user-facing stuff for every subscription to this tick
            for (int r = 0; r<rows.Count; r++)
            {
                int i = rows[r];
                
                string symbolcol = t.sym;

                if (t.isTrade)
                {
                    MarketsView.Rows[i].Cells["LastPrintCol"].Value = t.trade;
                    MarketsView.Rows[i].Cells["LastSizeCol"].Value = t.size;
                }
                else
                {
                    if (t.hasBid)
                    {
                        MarketsView.Rows[i].Cells["BidCol"].Value = t.bid;
                        MarketsView.Rows[i].Cells["BidSizeCol"].Value = t.bs;
                        try { bslist[symbolcol] = t.bs.ToString(); }
                        catch (KeyNotFoundException) { bslist.Add(symbolcol, t.bs.ToString()); }
                    }
                    if (t.hasAsk)
                    {
                        MarketsView.Rows[i].Cells["OfferCol"].Value = t.ask;
                        MarketsView.Rows[i].Cells["OfferSizeCol"].Value = t.os;
                        try { oslist[symbolcol] = t.os.ToString(); }
                        catch (KeyNotFoundException) { oslist.Add(symbolcol, t.os.ToString()); }
                    }
                    try
                    {
                        MarketsView.Rows[i].Cells["QuoteSizeCol"].Value = bslist[symbolcol] + "x" + oslist[symbolcol];
                    }
                    catch (KeyNotFoundException) { }
                }

                decimal high = tl.FastHigh(t.sym);
                decimal low = tl.FastLow(t.sym);

                MarketsView.Rows[i].Cells["DayHighCol"].Value = high;
                MarketsView.Rows[i].Cells["DayLowCol"].Value = low;

                // next two are only called once per run
                if (MarketsView["DayOpenCol", i].Value==null)
                    MarketsView["DayOpenCol", i].Value = tl.DayOpen(t.sym);
                if (MarketsView["YestCloseCol",i].Value==null)
                    MarketsView["YestCloseCol", i].Value = tl.YestClose(t.sym);


                // only display if turned on
                int ssize = tl.FastPos(t.sym).Size;

                MarketsView["PosSizeCol", i].Value = ssize;
                MarketsView["AvgPriceCol", i].Value = tl.FastPos(t.sym).AvgPrice;
                MarketsView["TimeCol", i].Value = t.time + "." + t.sec;


                if ((t.time % 4) == 0)
                {
                    if (!didheartbeat)
                    {
                        tl.HeartBeat(); // send heartbeat every 4minutes
                        didheartbeat = true;
                    }
                }
                else didheartbeat = false;

                    
            }
        }
        TradeLink_Client_WM tl = new TradeLink_Client_WM("quotopiac",true);
        ~Quote() { QuotopiaClose(); }
        void QuotopiaClose()
        {
            if (saveexitbut.Checked)
                saveSettingsbut_Click(null, new EventArgs());
            tl.Disconnect(); // unregister all stocks and this client
            foreach (string sym in filelist.Keys)
            {
                filelist[sym].Close(); // close all open files
            }
        }

        
        delegate void SetTextCallback(string text);
        delegate void SetTextCallbackColor(string text, Color color);
        public void showc(string s,Color c)
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

        bool IsBox(Type t) { return (t.BaseType.IsSubclassOf(typeof(Box))) || t.BaseType.Equals(typeof(Box)); }
        private void LoadDllButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.DefaultExt = ".dll";
            f.InitialDirectory = "c:\\program files\\tradelink\\tradelink\\";
            f.FileName = "box.dll";
            f.Filter = "TradeLink Boxes|*.dll|All Files|*.*";
            f.CheckFileExists = true;
            f.AddExtension = true;
            f.ShowDialog();
            Quotopia.Properties.Settings.Default.boxdllpath = f.FileName;
            UpdateBoxList();
        }
        void UpdateBoxList()
        {
            boxdropdown.DropDownItems.Clear();
            if (!boxdropdown.Enabled) return;
            if ((Quotopia.Properties.Settings.Default.boxdllpath == null) || (Quotopia.Properties.Settings.Default.boxdllpath == "")) Quotopia.Properties.Settings.Default.boxdllpath = Quotopia.Properties.Resources.boxdllpath;
            Assembly a;
            Type[] t;
            try
            {
                a = Assembly.LoadFrom(Properties.Settings.Default.boxdllpath);
                show("Loading boxes from: " +Quotopia.Properties.Settings.Default.boxdllpath);
            }
            catch (Exception ex) { show(ex.Message); return; }
            try
            {
                t = a.GetTypes();
            }
            catch (Exception ex) { show(ex.Message); return; }
            for (int i = 0; i < t.GetLength(0); i++) 
                if (IsBox(t[i])) 
                { 
                    boxdropdown.DropDownItems.Add(t[i].FullName); 
                    boxdropdown.DropDownItems[boxdropdown.DropDownItems.Count - 1].Name = t[i].FullName; 
                }
            if (boxlist.Count > 0)
            {
                boxdropdown.DropDownItems.Add("activateallbox");
                boxdropdown.DropDownItems[boxdropdown.DropDownItems.Count - 1].Name = "Activate ALL boxes";
                boxdropdown.DropDownItems.Add("shutdownallbox");
                boxdropdown.DropDownItems[boxdropdown.DropDownItems.Count - 1].Name = "Shutdown ALL boxes";
                boxdropdown.DropDownItems.Add("removeallbox");
                boxdropdown.DropDownItems[boxdropdown.DropDownItems.Count - 1].Name = "Remove ALL boxes";
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!MarketsView.IsCurrentRowDirty) return; // if no changes then we're done
            if (MarketsView.CurrentRow.Cells["SymbolCol"].Value == null) return;
            string sym = MarketsView.CurrentRow.Cells["SymbolCol"].Value.ToString().ToUpper();
            sym = sym.Replace("=N", "");

            if (!barlist.ContainsKey(sym)) barlist.Add(sym, new BarList(BarInterval.FiveMin, sym));

            MarketsView.CurrentRow.Cells["SymbolCol"].Value = sym;
            if (Stock.isStock(sym))
            {
                MarketsView.CurrentRow.Cells["MinSizeCol"].Value = Convert.ToInt32(Quotopia.Properties.Settings.Default.MinimumSize);
                if (!tlinklist.ContainsKey(sym)) tlinklist.Add(sym, new TickLink());
            }
            if (Index.isIdx(sym) || Stock.isStock(sym)) ReSubscribe();
        }

        Dictionary<string, List<int>> symidx = new Dictionary<string, List<int>>();
        List<string> idx = new List<string>();

        void ReSubscribe() { ReSubscribe(true); }
        void ReSubscribe(bool dosubscribe)
        {
            symidx.Clear();
            idx.Clear();

            for (int i = 0; i < MarketsView.Rows.Count; i++)
                if ((MarketsView["SymbolCol", i].Value != null) && (MarketsView["SymbolCol", i].Value.ToString() != ""))
                {
                    string s = MarketsView["SymbolCol", i].Value.ToString();
                    idx.Add(s);
                    if (!symidx.ContainsKey(s)) symidx.Add(s, new List<int>());
                    symidx[s].Add(i);
                }
                else idx.Add("");

            if (!dosubscribe) return; // only re-index requested, so we're done

            MarketBasket mb = new MarketBasket();
            IndexBasket ib = new IndexBasket();

            foreach (string sym in symidx.Keys)
            {
                if (Index.isIdx(sym)) ib.Add(new Index(sym));
                else if (Stock.isStock(sym)) mb.Add(new Stock(sym));
            }
            try
            {
                tl.Disconnect();
                tl.Register();
                if (mb.hasStock) tl.Subscribe(mb);
                if (ib.hasIndex) tl.RegIndex(ib);
            }
            catch (TLServerNotFound) { showc(GetDate+":"+GetTime+" TradeLink Server Not Found",Color.Red); }
        }


        private void bgcolbut_Click(object sender, EventArgs e)
        {
            ColorDialog bg = new ColorDialog();
            bg.Color = MarketsView.BackgroundColor;
            bg.ShowDialog();
            Quotopia.Properties.Settings.Default.marketbgcolor = bg.Color;
            for (int i = 0; i < MarketsView.Rows.Count; i++) MarketsView.Rows[i].DefaultCellStyle.BackColor = bg.Color;
            MarketsView.RowTemplate.DefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            MarketsView.DefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            MarketsView.BackgroundColor = Quotopia.Properties.Settings.Default.marketbgcolor;
        }

        private void fontBut_Click(object sender, EventArgs e)
        {
            FontDialog f = new FontDialog();
            f.ShowColor = true;
            f.Font = MarketsView.RowTemplate.DefaultCellStyle.Font;
            f.Color = MarketsView.RowTemplate.DefaultCellStyle.ForeColor;
            f.ShowDialog();
            Quotopia.Properties.Settings.Default.marketfontcolor = f.Color;
            Quotopia.Properties.Settings.Default.marketfont = f.Font;
            MarketsView.RowTemplate.DefaultCellStyle.Font = Quotopia.Properties.Settings.Default.marketfont;
            MarketsView.RowTemplate.DefaultCellStyle.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;
            ResetFontsAndColors();
        }

        private void SimStateChanged(TLTypes mode)
        {
            SimStateChanged(mode, true);
        }

        private void SimStateChanged(TLTypes mode, bool HandleExceptions)
        {
            switch (mode)
            {
                case TLTypes.LIVEBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {
                            tl.GoLive();
                        }
                        catch (TLServerNotFound) 
                        { 
                            MessageBox.Show("No Live Anvil instance was found.  Make sure Anvil is started live, and TradeLink is loaded.", "TradeLink server not found"); 
                            liveanvilbut.Checked = false;
                            return; 
                        }
                    }
                    else tl.GoLive();
                    saveticks.Checked = true;
                    saveticks.Enabled = true;
                    liveanvilbut.Checked = true;
                    archivefolderbut.Enabled = true;
                    break;
                case TLTypes.SIMBROKER:
                    if (HandleExceptions)
                    {
                        try
                        {

                            tl.GoSim();
                        }
                        catch (TLServerNotFound) 
                        { 
                            MessageBox.Show("No simulation Anvil instance found.  Make sure Anvil is started in simulation, and TradeLink extension is loaded.","TradeLink server not found");
                            simAnvilbut.Checked = false;
                            return; 
                        }
                    }
                    else tl.GoSim();
                    saveticks.Checked = true;
                    saveticks.Enabled = true;
                    simAnvilbut.Checked = true;
                    archivefolderbut.Enabled = true;
                    break;
            }
            Quotopia.Properties.Settings.Default.tlmode = (int)mode;
            show(GetDate.ToString() + ":" + GetTime.ToString() + ": " + "MODE CHANGED TO " + Enum.GetName(typeof(TLTypes), mode));
            tl.Register();
            if (MarketsView.Rows.Count == 0) return;
            ReSubscribe();

        }

        private void saveSettingsbut_Click(object sender, EventArgs e)
        {
            // serialize column properites
            try
            {
                Quotopia.Properties.Settings.Default.colhide.Clear();
                Quotopia.Properties.Settings.Default.stocks.Clear();

            }
            catch (NullReferenceException) { Quotopia.Properties.Settings.Default.stocks = new System.Collections.Specialized.StringCollection(); }

            List<int> rsize = new List<int>();

            if (savesymbollist.Checked)
            {
                for (int i = 0; i < MarketsView.Rows.Count; i++)
                {
                    if (!MarketsView.Rows[i].IsNewRow) rsize.Add(MarketsView.Rows[i].Height);
                    if (MarketsView["SymbolCol", i].Value != null)
                        Quotopia.Properties.Settings.Default.stocks.Add(MarketsView["SymbolCol", i].Value.ToString());
                    else Quotopia.Properties.Settings.Default.stocks.Add("");
                }
            }

            Dictionary<string, int> csize = new Dictionary<string, int>();
            Dictionary<string, int> corder = new Dictionary<string, int>();
            for (int i = 0; i < MarketsView.Columns.Count; i++)
            {
                string n = MarketsView.Columns[i].Name;
                corder[n] = MarketsView.Columns[i].DisplayIndex;
                csize[n] = MarketsView.Columns[i].Width;
                // save visable columns
                if (MarketsView.Columns[i].Visible)
                    Quotopia.Properties.Settings.Default.colhide.Add(n);
            }


            FileStream s = new FileStream("QuotopiaCo.dat",FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, corder);
            s.Flush();
            s.Close();
            s = new FileStream("QuotopiaCs.dat",FileMode.Create);
            b = new BinaryFormatter();
            b.Serialize(s, csize);
            s.Flush();
            s.Close();
            s = new FileStream("QuotopiaRs.dat", FileMode.Create);
            b = new BinaryFormatter();
            b.Serialize(s, rsize);
            s.Flush();
            s.Close();

            Quotopia.Properties.Settings.Default.Save();
        }



        private void MarketsView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex == (MarketsView.RowCount - 1)) || (e.RowIndex < 0)) return; // ignore last blank column
            if (MarketsView.Rows[e.RowIndex].Cells["SymbolCol"].Value == null) return; // ignore symboless rows
            string sym = MarketsView.Rows[e.RowIndex].Cells["SymbolCol"].Value.ToString();
            if (Index.isIdx(sym)) return; // don't trade indicies
            int size = Convert.ToInt32(Quotopia.Properties.Settings.Default.MinimumSize);
            if (MarketsView.Rows[e.RowIndex].Cells["MinSizeCol"].Value != null) size = Convert.ToInt32(MarketsView.Rows[e.RowIndex].Cells["MinSizeCol"].Value.ToString());

            if (e.ColumnIndex == MarketsView.Columns["SymbolCol"].Index)
            {
                int osize = 0;
                bool side = true;
                try
                {
                    osize = tl.FastPos(sym).Flat ? size : tl.FastPos(sym).Size;
                    side = tl.FastPos(sym).Flat ? true : !tl.FastPos(sym).Side;
                }
                catch (KeyNotFoundException) { }
                order o = new order(sym, osize, side);
                o.neworder += new QuotopiaOrderDel(o_neworder);
                spillTick += new QuotopiaTickDel(o.newTick);
                orderStatus += new OrderStatusDel(o.orderStatus);
                if ((spillTick != null) && (tlinklist.ContainsKey(sym))) spillTick(tlinklist[sym].A);
                Point p = new Point(MousePosition.X,MousePosition.Y);
                p.Offset(-315, -25);
                o.SetDesktopLocation(p.X, p.Y);
                o.Show();
            }

            if (e.ColumnIndex == MarketsView.Columns["BidCol"].Index)
            {
                // send a sell order
                Order o = new Order(sym, -1 * size);
                o.time = GetTime;
                o.date = GetDate;
                int r = tl.SendOrder(o);
                if (r != 0)
                {
                    //setBg(e.RowIndex,"SymbolCol",Color.Yellow);
                    showc(o.ToString()+" [ANVILERROR: (" + r + ") " + Util.PrettyError(tl.BrokerName,r) + "] ",Color.Red);
                }
                //else setBg(e.RowIndex,"SymbolCol",MarketsView.RowTemplate.DefaultCellStyle.BackColor);
            }
            else if (e.ColumnIndex == MarketsView.Columns["OfferCol"].Index)
            {
                // send a buy order
                Order o = new Order(sym, size);
                o.time = GetTime;
                o.date = GetDate;
                int r = tl.SendOrder(o);
                if (r != 0)
                {
                    //setBg(e.RowIndex, "SymbolCol", Color.Yellow);
                    showc(o.ToString() + " [ANVILERROR: (" + r + ") " + Util.PrettyError(tl.BrokerName,r) + "] ", Color.Red);
                }
                //else setBg(e.RowIndex, "SymbolCol", MarketsView.RowTemplate.DefaultCellStyle.BackColor);
            }
        }

        void o_neworder(Order sendOrder)
        {
            sendOrder.time = GetTime;
            sendOrder.date = GetDate;
            int r = tl.SendOrder(sendOrder);
            if (r != 0) showc(sendOrder.ToString() + " [ANVILERROR: (" + r + ") " + Util.PrettyError(tl.BrokerName,r) + "] ", Color.Red);
            if (orderStatus != null) orderStatus(sendOrder.symbol, r);
        }

        private void columnsdropdown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MarketsView.Columns[e.ClickedItem.Name].Visible = !MarketsView.Columns[e.ClickedItem.Name].Visible;
        }

        private void rightclickrow_Opening(object sender, CancelEventArgs e)
        {
            rightclickrow.ShowCheckMargin = false;
            rightclickrow.ShowImageMargin = false;

            if (boxdropdown.DropDownItems.Count == 0) boxdropdown.Enabled = false;
            else boxdropdown.Enabled = true;
            if ((MarketsView.CurrentRow == null) || (MarketsView.CurrentRow.Cells["SymbolCol"].Value == null))
            {
                rightclickrow.Items["showchart"].Enabled = false;
                flatthisfull.Enabled = false;
                flatthishalf.Enabled = false;
                flatthisqtr.Enabled = false;
                boxdropdown.Enabled = false;
            }
            else
            {
                rightclickrow.Items["showchart"].Enabled = true;
                boxdropdown.Enabled = true;
                try
                {
                    if (tl.FastPos(MarketsView.CurrentRow.Cells["SymbolCol"].Value.ToString()).Flat)
                    {
                        flatthisfull.Enabled = false;
                        flatthishalf.Enabled = false;
                        flatthisqtr.Enabled = false;
                    }
                    else
                    {
                        flatthisfull.Enabled = true;
                        flatthishalf.Enabled = true;
                        flatthisqtr.Enabled = true;
                    }
                }
                catch (KeyNotFoundException) { }
            }
            columnsdropdown.DropDown.Items.Clear();
            for (int i = 0; i < MarketsView.Columns.Count; i++)
            {
                if ((MarketsView.Columns[i].Name == "Blank") || (MarketsView.Columns[i].Name == "SymbolCol")) continue;
                columnsdropdown.DropDown.Items.Add(MarketsView.Columns[i].HeaderText);
                columnsdropdown.DropDown.Items[columnsdropdown.DropDown.Items.Count-1].Name = MarketsView.Columns[i].Name;
                if (!MarketsView.Columns[i].Visible) columnsdropdown.DropDown.Items[columnsdropdown.DropDown.Items.Count - 1].ForeColor = Color.Gray;
            }

        }

        private void rightclickrow_Click(object sender, EventArgs e)
        {
                if (rightclickrow.Items["showchart"].Selected)
                {
                    string sym = MarketsView.CurrentRow.Cells["SymbolCol"].Value.ToString();
                    if (barlist.ContainsKey(sym))
                    {
                        Chart c = new Chart(barlist[sym]);
                        c.BackColor = Quotopia.Properties.Settings.Default.chartcol;
                        c.Symbol = sym;
                        c.Show();
                    }
                    rightclickrow.Close();
                }
        }

        private void flatmenuright_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name=="flatallpositions") 
            {
                
                // flat each position
                foreach (string sym in symidx.Keys)
                {
                    if (!Index.isIdx(sym)) tl.SendOrder(new Order(sym, tl.FastPos(sym).Size * -1));
                }
                return; // we're done
            }
            if (MarketsView.CurrentRow.Cells["SymbolCol"].Value == null) return;
            string symbol = MarketsView.CurrentRow.Cells["SymbolCol"].Value.ToString();
            int size = tl.PosSize(symbol);
            if (e.ClickedItem.Name == "flatthishalf") size = Position.Norm2Min((int)size/2);
            else if (e.ClickedItem.Name == "flatthisqtr") size = Position.Norm2Min((int)size/2);
            if (size != 0) tl.SendOrder(new Order(symbol, -1*size));
            return;
        }

        private void MarketsView_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
            MarketsView.CurrentCell.Value = e.Data.GetData("{n2}");
        }

        private void MarketsView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == (int)Keys.Back)
                {
                    e.Handled = true;
                    for (int i = 0; i < MarketsView.Columns.Count; i++)
                        MarketsView[i, MarketsView.CurrentRow.Index].Value = "";
                    ReSubscribe();
                }

                if (e.KeyValue== (int)Keys.Delete) 
                { 
                    e.Handled = true; 
                    MarketsView.Rows.RemoveAt(MarketsView.CurrentRow.Index);
                    ReSubscribe(); // reindex symbolist
                }
                if (e.KeyValue==(int)Keys.Insert) 
                {
                    e.Handled = true;
                    if (MarketsView.RowTemplate.Index != -1) throw new Exception("Row Template was unshared!");
                    MarketsView.Rows.Insert(MarketsView.CurrentCell.RowIndex,1);
                    ReSubscribe(false);
                }
                if (((e.KeyValue == (int)Keys.Enter) && (MarketsView.CurrentRow.Index == MarketsView.NewRowIndex))) 
                { 
                    e.Handled = true;
                    if (MarketsView.RowTemplate.Index != -1) throw new Exception("row template unshared");
                    MarketsView.Rows.Add();
                    ReSubscribe(false); // reindex symbollist
                }
                if (e.Control && (e.KeyValue == (int)Keys.Up))
                {
                    e.Handled = true;
                    MarketsView["MinSizeCol", MarketsView.CurrentRow.Index].Value = Convert.ToInt32(MarketsView["MinSizeCol", MarketsView.CurrentRow.Index].Value) + 100;
                }

                if (e.Control && (e.KeyValue == (int)Keys.Down))
                {
                    e.Handled = true;
                    int size = Convert.ToInt32(MarketsView["MinSizeCol", MarketsView.CurrentRow.Index].Value) - 100;
                    if (size<100) size = 100;
                    MarketsView["MinSizeCol", MarketsView.CurrentRow.Index].Value = size;
                }

            }// && (MarketsView.Rows[MarketsView.CurrentRow.Index-1].Cells["SymbolCol"].Value!=null)
            catch (InvalidOperationException) { } // happens when uncomitted rows are deleted
        }

        private void boxdropdown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string ci = e.ClickedItem.Name;
            if (ci == "shutdownallbox")
            {
                foreach (string sym in boxlist.Keys) boxlist[sym].Shutdown(boxlist[sym].Name + "is SHUTDOWN.");
                return;
            }
            else if (ci == "activateallbox")
            {
                foreach (string sym in boxlist.Keys) boxlist[sym].Shutdown(boxlist[sym].Name + "is Activated.");
                return;
            }
            else if (ci == "removeallbox")
            {
                boxlist.Clear();
                return;
            }
            List<int> rows = new List<int>();
            for (int i = 0; i < MarketsView.SelectedCells.Count; i++)
            {
                int r = MarketsView.SelectedCells[i].RowIndex;
                if (rows.Contains(r)) continue;
                else rows.Add(r);

                string sym = null;
                try
                {

                    sym = MarketsView.Rows[r].Cells["SymbolCol"].Value.ToString();
                }
                catch (NullReferenceException) { return; }
                if (sym == null) return;
                Assembly a;
                Type t;
                object[] args;
                try
                {
                    a = Assembly.LoadFrom(Quotopia.Properties.Settings.Default.boxdllpath);
                    show("Loading boxes from: " +Quotopia.Properties.Settings.Default.boxdllpath);
                }
                catch (Exception ex) { show(ex.Message); return; }
                try
                {
                    t = a.GetType(e.ClickedItem.Name, true, true);
                }
                catch (Exception ex) { show(ex.Message); return; }
                if (!boxlist.ContainsKey(sym)) boxlist.Add(sym,null);
                args = new object[] {this.news};
                try
                {
                    boxlist[sym] = (Box)Activator.CreateInstance(t, args);
                    if (Util.isEarlyClose(GetDate)) boxlist[sym].DayEnd = Util.GetEarlyClose(GetDate);
                    boxlist[sym].FullName = e.ClickedItem.Name;
                    boxlist[sym].MaxSize = (int)maxsize.Value;
                    boxlist[sym].Debug = boxdebug.Checked;
                    show("Loaded "+ sym + " box: " + boxlist[sym].Name);
                }
                catch (Exception ex) { show(ex.Message); }
            }
                
       
        }

        private void gridcolorbut_Click(object sender, EventArgs e)
        {
            ColorDialog gc = new ColorDialog();
            gc.Color = MarketsView.GridColor;
            gc.ShowDialog();
            Quotopia.Properties.Settings.Default.gridcolor = gc.Color;
            MarketsView.GridColor = Quotopia.Properties.Settings.Default.gridcolor;
        }

        private void restoredefaultsbut_Click(object sender, EventArgs e)
        {
           Quotopia.Properties.Settings.Default.Reset();
        }

        private void resetsetbut_Click(object sender, EventArgs e)
        {
           Quotopia.Properties.Settings.Default.Reload();
           Quote_Load(null, null);
        }

        private void liveanvilbut_CheckedChanged(object sender, EventArgs e)
        {
            SimStateChanged(TLTypes.LIVEBROKER);
        }

        private void simAnvilbut_CheckedChanged(object sender, EventArgs e)
        {
            SimStateChanged(TLTypes.SIMBROKER);
        }

        private void archivefolderbut_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.SelectedPath = Quotopia.Properties.Settings.Default.ticfolderpath;
            f.ShowDialog();
            Quotopia.Properties.Settings.Default.ticfolderpath = f.SelectedPath;
        }


        private void MarketsView_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (MarketsView.CurrentCell.ColumnIndex == MarketsView.Columns["MinSizeCol"].Index)
                {
                    int r = MarketsView.CurrentCell.RowIndex;
                    int c = MarketsView.Columns["MinSizeCol"].Index;
                    int size = Convert.ToInt32(MarketsView["MinSizeCol", MarketsView.CurrentRow.Index].Value);
                    int minsize = Convert.ToInt32(Quotopia.Properties.Settings.Default.MinimumSize);
                    if ((e.Clicks == 1) && (e.Button == MouseButtons.Middle)) size = minsize;
                    MarketsView[c, r].Value = size.ToString();
                }
            }
            catch (NullReferenceException) { int i = 0; i++; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //uptickcolor
            ColorDialog uc = new ColorDialog();
            uc.Color = upcolor;
            uc.ShowDialog();
           Quotopia.Properties.Settings.Default.uptickcolor = uc.Color;
           upcolor = uc.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //downtickcolor
            ColorDialog dc = new ColorDialog();
            dc.Color = downcolor;
            dc.ShowDialog();
            Quotopia.Properties.Settings.Default.downtickcolor = dc.Color;
            downcolor = dc.Color;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //dayhighcolor
            ColorDialog hc = new ColorDialog();
            hc.Color =Quotopia.Properties.Settings.Default.highcolor;
            hc.ShowDialog();
            Quotopia.Properties.Settings.Default.highcolor = hc.Color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //daylowcolor
            ColorDialog lc = new ColorDialog();
            lc.Color =Quotopia.Properties.Settings.Default.lowcolor;
            lc.ShowDialog();
            Quotopia.Properties.Settings.Default.lowcolor = lc.Color;

        }

        private void boxdropdown_DropDownOpening(object sender, EventArgs e)
        {
            for (int i = 0; i < boxdropdown.DropDownItems.Count; i++)
                foreach (string sym in boxlist.Keys)
                {
                    if (boxlist[sym].FullName == boxdropdown.DropDownItems[i].Name) boxdropdown.DropDownItems[i].ForeColor = Color.Gray;
                    else boxdropdown.DropDownItems[i].ForeColor = Color.Black;
                }
        }

        private void Quote_Resize(object sender, EventArgs e)
        {
            BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            Quotopia.Properties.Settings.Default.wsize = Size;
        }

        Color upcolor;
        Color downcolor;


        private void ResetFontsAndColors()
        {
            // marketview background
            MarketsView.BackgroundColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            MarketsView.GridColor = Quotopia.Properties.Settings.Default.gridcolor;

            // column header
            MarketsView.ColumnHeadersDefaultCellStyle.ForeColor = Quotopia.Properties.Settings.Default.colheaderfg;
            MarketsView.ColumnHeadersDefaultCellStyle.Font = Quotopia.Properties.Settings.Default.headerfont;
            MarketsView.ColumnHeadersDefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.colheaderbg;

            // new rows
            MarketsView.RowTemplate.DefaultCellStyle.Font = Quotopia.Properties.Settings.Default.marketfont;
            MarketsView.RowTemplate.DefaultCellStyle.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;
            MarketsView.RowTemplate.DefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;

            // default rows (when quotopia opens)
            MarketsView.DefaultCellStyle.Font = Quotopia.Properties.Settings.Default.marketfont;
            MarketsView.DefaultCellStyle.BackColor = Quotopia.Properties.Settings.Default.marketbgcolor;
            MarketsView.DefaultCellStyle.ForeColor = Quotopia.Properties.Settings.Default.marketfontcolor;

            Color highcolor;
            Color lowcolor;
            upcolor = Quotopia.Properties.Settings.Default.uptickcolor;
            downcolor = Quotopia.Properties.Settings.Default.downtickcolor;
            Font f = Quotopia.Properties.Settings.Default.marketfont;
            highcolor = Quotopia.Properties.Settings.Default.highcolor;
            lowcolor = Quotopia.Properties.Settings.Default.lowcolor;
            Color fg = Quotopia.Properties.Settings.Default.marketfontcolor;
            Color bg = Quotopia.Properties.Settings.Default.marketbgcolor;
            bool tfg = tcolfgbut.Checked;
            bool hlfg = hlcolfgbut.Checked;

            // styles
            upstyle.ForeColor = tfg ? upcolor : fg;
            upstyle.BackColor = tfg ? bg : upcolor;
            upstyle.Font = f;
            upstyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            downstyle.ForeColor = tfg ? downcolor : fg;
            downstyle.BackColor = tfg ? bg : downcolor;
            downstyle.Font = f;
            downstyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            highstyle.ForeColor = hlfg ? highcolor : fg;
            highstyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            highstyle.Format = "N2";
            highstyle.BackColor = hlfg ? bg : highcolor;
            highstyle.Font = f;
            lowstyle.ForeColor = hlfg ? lowcolor : fg;
            lowstyle.BackColor = hlfg ? bg : lowcolor;
            lowstyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            lowstyle.Format = "N2";
            lowstyle.Font = f;
        }

        private void Quote_Load(object sender, EventArgs e)
        {
            ResetFontsAndColors();
            tcolfgbut.Checked = Quotopia.Properties.Settings.Default.tickcolorfg;
            MarketsView.Columns["xTrendCol"].DefaultCellStyle.NullValue = null;
            System.Collections.Specialized.StringCollection viz = Quotopia.Properties.Settings.Default.colhide;

            Dictionary<string, int> csize = new Dictionary<string, int>();
            Dictionary<string, int> corder = new Dictionary<string, int>();
            List<int> rsize = new List<int>();
            try
            {
                FileStream s = new FileStream("QuotopiaCo.dat",FileMode.Open);
                BinaryFormatter b = new BinaryFormatter();
                corder = (Dictionary<string,int>)b.Deserialize(s);
                s = new FileStream("QuotopiaCs.dat",FileMode.Open);
                b = new BinaryFormatter();
                csize = (Dictionary<string,int>)b.Deserialize(s);
                s = new FileStream("QuotopiaRs.dat", FileMode.Open);
                b = new BinaryFormatter();
                rsize = (List<int>)b.Deserialize(s);
            }
            catch (FileNotFoundException) { }

            try
            {
                int savedstocks = Quotopia.Properties.Settings.Default.stocks.Count;
                if (savedstocks > 0)
                {
                    MarketsView.Rows.Add(savedstocks);
                    for (int i = 0; i < savedstocks; i++)
                        MarketsView["SymbolCol", i].Value = 
                            Quotopia.Properties.Settings.Default.stocks[i];
                }
                for (int i = 0; i < MarketsView.Columns.Count; i++)
                    MarketsView.Columns[i].Visible = false; // turn off all columns
                for (int i = 0; i < viz.Count; i++) 
                    MarketsView.Columns[viz[i]].Visible = true; // turn back on requested ones
                foreach (string col in corder.Keys)
                {
                    MarketsView.Columns[col].DisplayIndex = corder[col];
                    MarketsView.Columns[col].Width = csize[col];
                }
            }
            catch (NullReferenceException) { }// happens when colums are removed in new quotopia versions 

            for (int i = 0; i < MarketsView.Rows.Count; i++)
            {
                if ((i>=0) && (i<rsize.Count))
                    MarketsView.Rows[i].Height = rsize[i];
            }
            MarketsView.Refresh();
            ReSubscribe();
            


        }

        private void clearDebugButton_Click_1(object sender, EventArgs e)
        {
            statusWindow.Clear();
        }

        private void unloadallboxbut_Click(object sender, EventArgs e)
        {
            string bl = "";
            foreach (string sym in boxlist.Keys) bl += sym + "("+boxlist[sym].Name+") ";
            boxlist.Clear();
            if (bl!="") show("Unloaded boxes: " + bl);
            
        }

        private void MarketsView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                MarketsView["xTrendCol", e.Row.Index].Value = Quotopia.Properties.Resources.blank;
            }
            catch (Exception) { }

        }

        private void Quote_FormClosing(object sender, FormClosingEventArgs e)
        {
            QuotopiaClose();
        }

        private void colheadcolbut_Click(object sender, EventArgs e)
        {
            ColorDialog c = new ColorDialog();
            c.Color = MarketsView.ColumnHeadersDefaultCellStyle.BackColor;
            c.ShowDialog();
            Quotopia.Properties.Settings.Default.colheaderbg = c.Color;
            MarketsView.ColumnHeadersDefaultCellStyle.BackColor = c.Color;
        }

        private void colheadfgcolbut_Click(object sender, EventArgs e)
        {
            FontDialog f = new FontDialog();
            f.Color = MarketsView.ColumnHeadersDefaultCellStyle.ForeColor;
            f.Font = MarketsView.ColumnHeadersDefaultCellStyle.Font;
            f.ShowColor = true;
            f.ShowDialog();
            Quotopia.Properties.Settings.Default.colheaderfg = f.Color;
            Quotopia.Properties.Settings.Default.headerfont = f.Font;
            MarketsView.ColumnHeadersDefaultCellStyle.Font = f.Font;
            MarketsView.ColumnHeadersDefaultCellStyle.ForeColor = f.Color;
        }

        private void aboutbut_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Quotopia v" + Ver +Environment.NewLine+"TradeLink v"+tl.Ver+Environment.NewLine+"http://franta.com/trade/support.html","About Quotopia");
        }

        private void boxenablebox_CheckedChanged(object sender, EventArgs e)
        {
            if (boxenablebox.Checked) show(GetDate + ":" + GetTime + " BOXES ENABLED");
        }

        private void disableboxbut_CheckedChanged(object sender, EventArgs e)
        {
            if (disableboxbut.Checked) show(GetDate + ":" + GetTime + " BOXES DISABLED");
        }

        private void chartcolbut_Click(object sender, EventArgs e)
        {
            ColorDialog c = new ColorDialog();
            c.Color = Quotopia.Properties.Settings.Default.chartcol;
            c.ShowDialog();
            Quotopia.Properties.Settings.Default.chartcol = c.Color;
        }

        private void tcolfgbut_CheckedChanged(object sender, EventArgs e)
        {
            Quotopia.Properties.Settings.Default.tickcolorfg = tcolbgbut.Checked;
            ResetFontsAndColors();
        }

        private void hlcolbgbut_CheckedChanged(object sender, EventArgs e)
        {
            Quotopia.Properties.Settings.Default.highcolorbg = hlcolbgbut.Checked;
            ResetFontsAndColors();
        }

        private void MarketsView_RowUnshared(object sender, DataGridViewRowEventArgs e)
        {
            showc("row unshared:" + e.Row, Color.Purple);
        }

        




    }



}
