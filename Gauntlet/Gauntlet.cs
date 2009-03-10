using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;
using System.Runtime.InteropServices;

namespace WinGauntlet
{
    public partial class Gauntlet : Form
    {
        // minimization stuff
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr HWND, int CMDSHOW);
        const int SW_MINIMIZE = 6;

        HistSim h;
        BackgroundWorker bw = new BackgroundWorker();
        static GauntArgs args = new GauntArgs();
        const string PROGRAM = "Gauntlet";
        StreamWriter indf;
        StreamWriter log;
        bool background = false;



        public Gauntlet()
        {
            InitializeComponent();
            args.ParseArgs(Environment.GetCommandLineArgs());
            FormClosing += new FormClosingEventHandler(Gauntlet_FormClosing);
            debug(Util.TLSIdentity());
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);

            if (args.isUnattended)
            {
                background = true;
                //ShowWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, SW_MINIMIZE);
                bindresponseevents();
                queuebut_Click(null, null);
            }
            else
            {
                FindStocks(args.Folder);
                UpdateResponses(Util.GetResponseList(args.DllName));

            }
            
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
            // make sure response is valid
            if ((args.Response == null) || !args.Response.isValid)
            {
                show("No valid response was selected, quitting.");
                return;
            }
            // make sure tick folder is valid
            if (!args.isUnattended)
            {
                string msg = "No tick folder option is configured.";
                show(msg);
                if (!background) MessageBox.Show(msg);
                return;
            }
            // prepare other arguments for the run
            args.Name = PROGRAM+DateTime.Now.ToString("yyyMMdd.HHmm");
            args.Started = DateTime.Now;

            // enable progress reporting
            ProgressBar1.Enabled = true;
            // disable more than one simulation at once
            queuebut.Enabled = false;
            // enable canceling
            _stopbut.Enabled = true;

            // prepare indicator output
            if (indf == null)
            {
                indf = new StreamWriter(OutPrefix("Indicators") + Unique(_unique.Checked,args.Name)+ ".csv", false);
                indf.WriteLine(string.Join(",", args.Response.Indicators));
                indf.AutoFlush = true;
            }

            // start the run in the background
            bw.RunWorkerAsync(args);

        }

        string Unique(bool unique, string name) { return unique ? "." + name : ""; }

        string OutPrefix(string type) { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + PROGRAM+"."+type; }

        // runs the simulation in background
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // get simulation arguments
            GauntArgs ga = (GauntArgs)e.Argument;
            // notify user
            debug("Run started: " + ga.Name);
            // prepare simulator
            h = new HistSim(ga.Folder,ga.Filter);
            h.GotDebug += new DebugDelegate(h_GotDebug);
            h.GotTick += new TickDelegate(h_GotTick);
            // start simulation
            h.PlayTo(ga.PlayTo);
            // end simulation
            ga.Stopped = DateTime.Now;
            ga.TicksProcessed = h.TicksProcessed;
            ga.Executions = h.FillCount;
            // save result
            e.Result = ga;
        }

        // runs after simulation is complete
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (!e.Cancelled)
            {
                GauntArgs args = (GauntArgs)e.Result;
                string unique = _unique.Checked ? "." + args.Name : "";
                debug("writing trades...");
                Util.ClosedPLToText(h.SimBroker.GetTradeList(), ',', OutPrefix("Trades") + unique + ".csv");
                if (ordersincsv.Checked)
                {
                    debug("writing orders...");
                    StreamWriter sw = new StreamWriter(OutPrefix("Orders") + unique + ".csv", false);
                    for (int i = 0; i < h.SimBroker.GetOrderList().Count; i++)
                        sw.WriteLine(OrderImpl.Serialize(h.SimBroker.GetOrderList()[i]));
                    sw.Close();
                }
                debug("Completed. Ticks: " + args.TicksProcessed + " Speed:" + args.TicksSecond.ToString("N0") + " t/s  Fills: " + args.Executions.ToString());
            }
            else debug("Canceled.");
            // close indicators
            if (indf != null)
            {
                indf.Close();
                indf = null;
            }

            // reset simulation
            h.Reset();
            count = 0;
            lastp = 0;
            if (background)
            {
                Close();
                return;
            }
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
            if (args.Response == null) return;
            count++;
            args.Response.GotTick(t);
            if (background) return;
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

        void PrepareFilter()
        {
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
            //save it
            args.Filter = tff;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // tick folder
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Select the folder containing tick files";
            fd.SelectedPath = args.Folder;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                args.Folder = fd.SelectedPath;
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
                args.DllName = of.FileName;
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
            if (log == null)
            {
                log = new StreamWriter(OutPrefix("Debugs") + Unique(_unique.Checked, args.Name) + ".txt", true);
                log.AutoFlush = true;
            }
            else
                log.WriteLine(message);
            if (background) return;
            if (messages.InvokeRequired)
                Invoke(new ShowCallBack(show), new object[] { message });
            else
            {
                messages.AppendText(message);
                if (message.Contains(Environment.NewLine)) lastmessage.Text = message.Replace(Environment.NewLine, "");
                else lastmessage.Text = lastmessage.Text + message;
            }
        }

        

        void bt_BTStatus(Debug debug)
        {
            show(debug.Msg);
        }



        private void savesettings_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Save();
        }

        private void Gauntlet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saveonexit.Checked && !background)
            {
                Properties.Settings.Default.tickfolder = args.Folder;
                Properties.Settings.Default.boxdll = args.DllName;
                WinGauntlet.Properties.Settings.Default.Save();
            }
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
                args.Response = ResponseLoader.FromDLL((string)reslist.SelectedItem, WinGauntlet.Properties.Settings.Default.boxdll);
            }
            catch (Exception ex) { show("Response failed to load, quitting... (" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message.ToString() : "") + ")"); }
            if (!args.Response.isValid) { show("Response did not load or loaded in a shutdown state. "+args.Response.Name+ " "+args.Response.FullName); return; }
            bindresponseevents();
        }

        bool _boundonce = false;
        bool bindresponseevents()
        {
            if ((args.Response== null) || !args.Response.isValid)
                return false;
            if (_boundonce) return true;
            args.Response.SendIndicators += new ObjectArrayDelegate(Response_IndicatorUpdate);
            args.Response.SendDebug += new DebugFullDelegate(Response_GotDebug);
            args.Response.SendCancel += new UIntDelegate(Response_CancelOrderSource);
            args.Response.SendOrder += new OrderDelegate(Response_SendOrder);
            _boundonce = true;
            return true;
        }


        void Response_SendOrder(Order o)
        {
            if (h!=null)
                h.SimBroker.sendOrder(o);
        }

        void mybroker_GotOrderCancel(string sym, bool side, uint id)
        {
            if (args.Response != null)
                args.Response.GotOrderCancel(id);
        }

        void Response_CancelOrderSource(uint number)
        {
            if (h!=null)
                h.SimBroker.CancelOrder(number);
            
        }

        void Response_GotDebug(Debug msg)
        {
            if (!showdebug.Checked && !background) return;
            debug(msg.Msg);
        }

        void Response_IndicatorUpdate(object[] parameters)
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

        class GauntArgs
        {
            const int DLL = 1;
            const int RESPONSE = 2;
            const int TICKFOLDER = 3;
            const int FILEFILTERFILE = 4;
            public string Name;
            public int TicksProcessed = 0;
            public int Executions = 0;
            public long PlayTo = HistSim.ENDSIM;
            public DateTime Started = DateTime.MaxValue;
            public DateTime Stopped = DateTime.MaxValue;
            public double Seconds { get { return Stopped.Subtract(Started).TotalSeconds; } }
            public double TicksSecond { get { return Seconds == 0 ? 0 : ((double)TicksProcessed / Seconds); } }

            string _dllname = "";
            string _resp = Properties.Settings.Default.boxdll == null ? "Responses.dll" : Properties.Settings.Default.boxdll;
            Response _response;
            string _folder = (Properties.Settings.Default.tickfolder == null) ? Util.TLTickDir : Properties.Settings.Default.tickfolder;
            TickFileFilter _filter = new TickFileFilter();
            string _filterloc = "";
            public string DllName { get { return _dllname; } set { _dllname = value; } }
            public string ResponseName { get { return _resp; } set { _resp = value; } }
            public Response Response { get { return _response; } set { _response = value; } }
            public string Folder { get { return _folder; } set { _folder = value; } }
            public TickFileFilter Filter { get { return _filter; } set { _filter = value; } }
            public string FilterLocation { get { return _filterloc; } set { _filterloc = value; } }
            public bool isUnattended { get { return (_response != null) && Directory.Exists(_folder); } }

            public void ParseArgs(string[] args)
            {
                int l = args.Length;
                if (l> FILEFILTERFILE)
                {
                    SetFilter(args[FILEFILTERFILE]);
                    SetFolder(args[TICKFOLDER]);
                    SetDll(args[DLL]);
                    SetResponse(args[DLL]);
                }
                else if (l < FILEFILTERFILE)
                {
                    SetFolder(args[TICKFOLDER]);
                    SetDll(args[DLL]);
                    SetResponse(args[DLL]);

                }
                else if (l < TICKFOLDER)
                {
                    SetDll(args[DLL]);
                    SetResponse(args[DLL]);
                } 
                else if (l<RESPONSE)
                {
                    SetDll(args[DLL]);

                }

                
            }

            bool SetFolder(string folder)
            {
                if (!Directory.Exists(folder))
                    return false;
                _folder = folder;
                return true;
            }
            bool SetResponse(string name)
            {
                // dll must exist, otherwise quit
                if (DllName== "") return false;
                // response must exist in dll, otherwise quit
                if (!Util.GetResponseList(DllName).Contains(name))
                    return false;
                _resp = name;
                _response = ResponseLoader.FromDLL(_resp, DllName);
                return true;
            }

            bool SetDll(string file)
            {
                if (File.Exists(file))
                {
                    _dllname = file;
                    return true;
                }
                return false;

            }

            bool SetFilter(string file)
            {
                if (File.Exists(file))
                {
                    _filterloc = file;
                    _filter = TickFileFilter.FromFile(file);
                    return true;
                }
                return false;
            }
        }


    }






}
