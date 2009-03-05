using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;

    namespace WinGauntlet
{
    public partial class Gauntlet : Form
    {
        Response myres;
        HistSim h;
        BackgroundWorker bw = new BackgroundWorker();

        public Gauntlet()
        {
            InitializeComponent();
            FindStocks((WinGauntlet.Properties.Settings.Default.tickfolder == null) ? @"c:\program files\tradelink\tickdata\" : WinGauntlet.Properties.Settings.Default.tickfolder);
            string fn = (WinGauntlet.Properties.Settings.Default.boxdll== null) ? "box.dll" : WinGauntlet.Properties.Settings.Default.boxdll;
            if (File.Exists(fn))
                UpdateResponses(Util.GetResponseList(fn));
            ProgressBar1.Enabled = false;
            FormClosing+=new FormClosingEventHandler(Gauntlet_FormClosing);
            debug(Util.TLSIdentity());
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
        }



        // when run button is clicked, setup the simulation
        private void queuebut_Click(object sender, EventArgs e)
        {
            // make sure we only have one background thread
            if (bw.IsBusy)
            {
                debug("simulation already in progress.");
                return;
            }
            // make sure tick folder is valid
            string dir = WinGauntlet.Properties.Settings.Default.tickfolder;
            if (!Directory.Exists(dir))
            {
                string msg = "No tick folder option is configured.";
                MessageBox.Show(msg, "Tick folder missing");
                show(msg);
                return;
            }
            // make sure response is valid
            if ((myres == null) || !myres.isValid)
            {
                show("No valid response was selected, quitting.");
                return;
            }
            // prepare date filter
            List<TickFileFilter.TLDateFilter> datefilter = new List<TickFileFilter.TLDateFilter>();
            if (usedates.Checked)
            {
                for (int j = 0; j < yearlist.SelectedIndices.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(yearlist.Items[yearlist.SelectedIndices[j]]) * 10000, DateMatchType.Year));
                for (int j = 0; j < monthlist.SelectedItems.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(monthlist.Items[monthlist.SelectedIndices[j]]) * 100, DateMatchType.Month));
                for (int j = 0; j < daylist.SelectedItems.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(daylist.Items[daylist.SelectedIndices[j]]), DateMatchType.Day));
            }
            // prepare symbol filter
            List<string> symfilter = new List<string>();
            if (usestocks.Checked)
                for (int j = 0; j < stocklist.SelectedItems.Count; j++)
                    symfilter.Add(stocklist.Items[stocklist.SelectedIndices[j]].ToString());

            // build consolidated filter
            TickFileFilter tff = new TickFileFilter(symfilter, datefilter);
            // prepare other arguments for the run
            SimWorker args = new SimWorker();
            args.Name = DateTime.Now.ToString("yyyMMdd.HHmm");
            args.filter = tff;
            args.TickFolder = dir;
            args.Started = DateTime.Now;

            // enable progress reporting
            ProgressBar1.Enabled = true;
            // disable more than one simulation at once
            queuebut.Enabled = false;
            // enable canceling
            _stopbut.Enabled = true;

            // prepare indicator output if requested
            if (indicatorscsv.Checked)
            {
                if (indf == null)
                {
                    string unique = csvnamesunique.Checked ? "." + args.Name : "";
                    indf = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gauntlet.Indicators" + unique + ".csv", false);
                    indf.WriteLine(string.Join(",", myres.Indicators));
                    indf.AutoFlush = true;
                }
            }

            // start the run in the background
            bw.RunWorkerAsync(args);

        }

        // runs the simulation in background
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // get simulation arguments
            SimWorker sw = (SimWorker)e.Argument;
            // notify user
            debug("Run started: " + sw.Name);
            // prepare simulator
            h = new HistSim(sw.TickFolder,sw.filter);
            h.GotDebug += new DebugDelegate(h_GotDebug);
            h.GotTick += new TickDelegate(h_GotTick);
            // start simulation
            h.PlayTo(sw.PlayTo);
            // end simulation
            sw.Stopped = DateTime.Now;
            sw.TicksProcessed = h.TicksProcessed;
            sw.Executions = h.FillCount;
            // save result
            e.Result = sw;
        }

        // runs after simulation is complete
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (!e.Cancelled)
            {
                SimWorker args = (SimWorker)e.Result;
                string unique = csvnamesunique.Checked ? "." + args.Name : "";
                if (tradesincsv.Checked)
                {
                    debug("writing trades...");
                    Util.ClosedPLToText(h.SimBroker.GetTradeList(), ',', Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gauntlet.Trades" + unique + ".csv");
                }
                if (ordersincsv.Checked)
                {
                    debug("writing orders...");
                    StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gauntlet.Orders" + unique + ".csv", false);
                    for (int i = 0; i < h.SimBroker.GetOrderList().Count; i++)
                        sw.WriteLine(OrderImpl.Serialize(h.SimBroker.GetOrderList()[i]));
                    sw.Close();
                }
                debug("Completed. Ticks: " + args.TicksProcessed + " Speed:" + args.TicksSecond.ToString("N0") + " t/s  Fills: " + args.Executions.ToString());
            }
            else debug("Canceled.");
            // close indicators
            if (indf != null)
                indf.Close();

            // reset simulation
            h.Reset();
            count = 0;
            lastp = 0;
            // enable new runs
            ProgressBar1.Enabled = false;
            ProgressBar1.Value = 0;
            queuebut.Enabled = true;
            _stopbut.Enabled = false;
            Invalidate(true);
        }

        int count = 0;
        uint lastp = 0;
        void h_GotTick(Tick t)
        {
            if (myres == null) return;
            count++;
            myres.GotTick(t);
            uint percent = (uint)((double)count*100 / h.TicksPresent);
            if (percent != lastp)
            {
                updatepercent(percent);
                lastp = percent;
            }

        }

        void updatepercent(uint per)
        {
            if (InvokeRequired)
                Invoke(new UIntDelegate(updatepercent), new object[] { per });
            else
            {
                try
                {
                    ProgressBar1.Value = (int)per;
                    ProgressBar1.Invalidate();
                }
                catch (Exception) { }
            }
        }


        void h_GotDebug(string msg)
        {
            debug(msg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // tick folder
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Select the folder containing tick files";
            fd.SelectedPath = (WinGauntlet.Properties.Settings.Default.tickfolder == null) ? @"c:\program files\tradelink\tickdata\" : WinGauntlet.Properties.Settings.Default.tickfolder;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                WinGauntlet.Properties.Settings.Default.tickfolder = fd.SelectedPath;
                FindStocks(fd.SelectedPath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // response library selection
            OpenFileDialog of = new OpenFileDialog();
            of.CheckPathExists = true;
            of.CheckFileExists = true;
            of.DefaultExt = ".dll";
            of.Filter = "Response DLL|*.dll";
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                WinGauntlet.Properties.Settings.Default.boxdll = of.FileName;
                UpdateResponses(Util.GetResponseList(of.FileName));
            }
        }

        void UpdateResponses(List<string> responses)
        {
            reslist.Items.Clear();
            for (int i = 0; i < responses.Count; i++)
                reslist.Items.Add(responses[i]);
        }

        private void messages_DoubleClick(object sender, EventArgs e)
        {
            messages.Clear();
        }

        delegate void ShowCallBack(string msg);
        void debug(string message) { show(message + Environment.NewLine); }
        void show(string message)
        {
            if (messages.InvokeRequired)
                Invoke(new ShowCallBack(show), new object[] { message });
            else
            {
                messages.AppendText(message);
                if (message.Contains(Environment.NewLine)) lastmessage.Text = message.Replace(Environment.NewLine,"");
                else lastmessage.Text = lastmessage.Text + message;
            }
        }

        

        void bt_BTStatus(Debug debug)
        {
            show(debug.Msg);
        }



        void bt_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = e.ProgressPercentage > 100 ? 100 : e.ProgressPercentage;
        }


        private void savesettings_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Save();
        }

        private void Gauntlet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saveonexit.Checked) WinGauntlet.Properties.Settings.Default.Save();
        }

        private void usestocks_CheckedChanged(object sender, EventArgs e)
        {
            stocklist.Enabled = !stocklist.Enabled;
        }

        private void usedates_CheckedChanged(object sender, EventArgs e)
        {
            yearlist.Enabled = !yearlist.Enabled;
            monthlist.Enabled = !monthlist.Enabled;
            daylist.Enabled = !daylist.Enabled;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Reload();
        }

        private void boxlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                myres = ResponseLoader.FromDLL((string)reslist.SelectedItem, WinGauntlet.Properties.Settings.Default.boxdll);
            }
            catch (Exception ex) { show("Response failed to load, quitting... (" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message.ToString() : "") + ")"); }
            if (!myres.isValid) { show("Response did not load or loaded in a shutdown state. "+myres.Name+ " "+myres.FullName); return; }
            myres.SendIndicators += new ObjectArrayDelegate(myres_IndicatorUpdate);
            myres.SendDebug += new DebugFullDelegate(myres_GotDebug);
            myres.SendCancel+= new UIntDelegate(myres_CancelOrderSource);
            myres.SendOrder += new OrderDelegate(myres_SendOrder);
        }

        void myres_SendOrder(Order o)
        {
            if (h!=null)
                h.SimBroker.sendOrder(o);
        }

        void mybroker_GotOrderCancel(string sym, bool side, uint id)
        {
            if (myres != null)
                myres.GotOrderCancel(id);
        }

        void myres_CancelOrderSource(uint number)
        {
            if (h!=null)
                h.SimBroker.CancelOrder(number);
            
        }

        void myres_GotDebug(Debug msg)
        {
            if (!showdebug.Checked) return;
            debug(msg.Msg);
        }
        StreamWriter indf = null;
        void myres_IndicatorUpdate(object[] parameters)
        {
            if (indf == null) return;
            string[] ivals = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) ivals[i] = parameters[i].ToString();
            indf.WriteLine(string.Join(",", ivals));
        }

        void FindStocks(string path)
        {
            // build list of available stocks and dates available
            stocklist.Items.Clear();
            yearlist.Items.Clear();
            daylist.Items.Clear();
            monthlist.Items.Clear();
            DirectoryInfo di;
            FileInfo[] fi;

            try
            {
                di = new DirectoryInfo(path);
                fi = di.GetFiles("*.epf");
            }
            catch (Exception ex) { show("exception loading stocks: " + ex.ToString()); return; }

            int[] years = new int[200];
            int[] days = new int[31];
            int[] months = new int[12];
            int yc = 0;
            int dc = 0;
            int mc = 0;

            for (int i = 0; i < fi.Length; i++)
            {
                SecurityImpl s = StockFromFileName(fi[i].Name);
                if (!s.isValid) continue;
                DateTime d = Util.ToDateTime(s.Date,0);
                if (!stocklist.Items.Contains(s.Symbol))
                    stocklist.Items.Add(s.Symbol);
                if (!contains(d.Year, years))
                    years[yc++] = d.Year;
                if (!contains(d.Month, months))
                    months[mc++] = d.Month;
                if (!contains(d.Day, days))
                    days[dc++] = d.Day;
            }
            Array.Sort(years);
            Array.Sort(days);
            Array.Sort(months);
            for (int i = 0; i < years.Length; i++)
                if (years[i] == 0) continue;
                else yearlist.Items.Add(years[i]);
            for (int i = 0; i < months.Length; i++)
                if (months[i] == 0) continue;
                else monthlist.Items.Add(months[i]);
            for (int i = 0; i < days.Length; i++)
                if (days[i] == 0) continue;
                else daylist.Items.Add(days[i]);
        }

        bool contains(int number, int[] array) { for (int i = 0; i < array.Length; i++) if (array[i] == number) return true; return false; }


        SecurityImpl StockFromFileName(string filename)
        {
            try
            {
                string ds = System.Text.RegularExpressions.Regex.Match(filename, "([0-9]{8})[.]", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Result("$1");
                string sym = filename.Replace(ds, "").Replace(".EPF", "");
                SecurityImpl s = new SecurityImpl(sym);
                s.Date = Convert.ToInt32(ds);
                return s;
            }
            catch (Exception) { }
            return new SecurityImpl();

        }

        private void _stopbut_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
        }


    }

    public class SimWorker
    {
        public string Name;
        public int TicksProcessed = 0;
        public int Executions = 0;
        public TickFileFilter filter = new TickFileFilter();
        public string TickFolder = Properties.Settings.Default.tickfolder;
        public long PlayTo = HistSim.ENDSIM;
        public DateTime Started = DateTime.MaxValue;
        public DateTime Stopped = DateTime.MaxValue;
        public double Seconds { get { return Stopped.Subtract(Started).TotalSeconds; } }
        public double TicksSecond { get { return Seconds == 0 ? 0 : ((double)TicksProcessed / Seconds); } }
    }



}
