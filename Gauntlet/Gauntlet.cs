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


        HistSim h;
        BackgroundWorker bw = new BackgroundWorker();
        static GauntArgs args = new GauntArgs();
        public const string PROGRAM = "Gauntlet";
        StreamWriter indf;
        StreamWriter log;
        bool background = false;



        public Gauntlet()
        {
            InitializeComponent();
            debug(Util.TLSIdentity());
            args.GotDebug += new DebugDelegate(args_GotDebug);
            args.ParseArgs(Environment.GetCommandLineArgs());
            FormClosing += new FormClosingEventHandler(Gauntlet_FormClosing);

            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);

            if (args.isUnattended)
            {
                background = true;
                ordersincsv.Checked = true;
                //ShowWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, SW_MINIMIZE);
                bindresponseevents();
                queuebut_Click(null, null);
            }
            else
            {
                tickFileFilterControl1.SetSymbols(args.Folder);
                UpdateResponses(Util.GetResponseList(args.DllName));
                debug(Util.TLSIdentity());
            }
            
        }

        void args_GotDebug(string msg)
        {
            debug(msg);
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
                status("No valid response was selected, quitting.");
                return;
            }
            // make sure tick folder is valid
            if (args.Folder=="")
            {
                string msg = "No tick folder option is configured.";
                status(msg);
                if (!background) MessageBox.Show(msg);
                return;
            }
            // prepare other arguments for the run
            if (!background)
            {
                args.Orders = ordersincsv.Checked;
                args.Indicators = _indicatcsv.Checked;
                args.Debugs = _debugs.Checked;
                args.Filter = tickFileFilterControl1.GetFilter();
            }
            args.Name = args.ResponseName+uniquen;
            args.Started = DateTime.Now;

            // enable progress reporting
            ProgressBar1.Enabled = true;
            // disable more than one simulation at once
            queuebut.Enabled = false;

            // start the run in the background
            bw.RunWorkerAsync(args);

        }
        string uniquen { get { return DateTime.Now.ToString(".yyyMMdd.HHmm"); } }

        string LogFile(string logtype) { return OUTFOLD+args.Response.FullName+(_unique.Checked ? uniquen:"")+ "."+logtype+".csv"; }

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
            GauntArgs gargs = (GauntArgs)e.Result;
            if (!e.Cancelled)
            {

                if (gargs.Trades)
                {
                    List<Trade> list = h.SimBroker.GetTradeList();
                    debug("writing "+list.Count+" trades...");
                    Util.ClosedPLToText(list, ',', LogFile("Trades"));
                }
                if (gargs.Orders)
                {
                    List<Order> olist = h.SimBroker.GetOrderList();
                    debug("writing "+olist.Count+" orders...");
                    StreamWriter sw = new StreamWriter(LogFile("Orders"), false);
                    string[] cols = Enum.GetNames(typeof(OrderField));
                    sw.WriteLine(string.Join(",", cols));
                    for (int i = 0; i < olist.Count; i++)
                        sw.WriteLine(OrderImpl.Serialize(olist[i]));
                    sw.Close();
                }
                debug("Done.  Ticks: " + gargs.TicksProcessed + " Speed:" + gargs.TicksSecond.ToString("N0") + " t/s  Fills: " + gargs.Executions.ToString());
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
            Invalidate(true);
        }

        int count = 0;
        uint lastp = 0;
        void h_GotTick(Tick t)
        {
            if (args.Response == null) return;
            count++;
            try
            {
                args.Response.GotTick(t);
            }
            catch (Exception ex) { debug("response threw exception: " + ex.Message); }
            if (background) return;
            uint percent = (uint)((double)count*100 / h.TicksPresent);
            if ((percent!=lastp) && (percent % 5 == 0))
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
            if (args.Debugs)
                debug(msg);
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
                tickFileFilterControl1.SetSymbols(args.Folder);
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
        string OUTFOLD = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
        void debug(string message) { status(message + Environment.NewLine); }
        void status(string message)
        {
            if (log == null)
            {
                log = new StreamWriter(OUTFOLD+"Debugs"+uniquen+".txt", true);
                log.AutoFlush = true;
            }
            log.Write(DateTime.Now.ToShortTimeString()+": "+message);
            if (background) return;
            if (messages.InvokeRequired)
            {
                try
                {
                    Invoke(new ShowCallBack(status), new object[] { message });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                try
                {
                    messages.AppendText(message);
                    if (message.Contains(Environment.NewLine)) lastmessage.Text = message.Substring(0,message.Length-2);
                    else lastmessage.Text = lastmessage.Text + message;
                    lastmessage.Invalidate();
                    messages.Invalidate(true);
                }
                catch (ObjectDisposedException) { }
            }
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
            catch (Exception ex) { status("Response failed to load, quitting... (" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message.ToString() : "") + ")"); }
            if (!args.Response.isValid) { status("Response did not load or loaded in a shutdown state. "+args.Response.Name+ " "+args.Response.FullName); return; }
            args.ResponseName = args.Response.FullName;
            _boundonce = false;
            bindresponseevents();
        }

        bool _boundonce = false;
        bool bindresponseevents()
        {
            if ((args.Response== null) || !args.Response.isValid)
                return false;
            if (_boundonce) return true;
            args.Response.SendIndicators += new StringParamDelegate(Response_SendIndicators);
            args.Response.SendDebug += new DebugFullDelegate(Response_GotDebug);
            args.Response.SendCancel += new UIntDelegate(Response_CancelOrderSource);
            args.Response.SendOrder += new OrderDelegate(Response_SendOrder);
            _boundonce = true;
            return true;
        }

        void Response_SendIndicators(string param)
        {
            if (!args.Indicators) return;
            // prepare indicator output
            if (indf == null)
            {
                indf = new StreamWriter(LogFile("Indicators"), false);
                indf.WriteLine(string.Join(",", args.Response.Indicators));
                indf.AutoFlush = true;
            }
            indf.WriteLine(param);
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
            if (!args.Debugs) return;
            debug(msg.Msg);
        }

        void Response_IndicatorUpdate(object[] parameters)
        {

        }



        private void _stopbut_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
        }

        class GauntArgs
        {
            public GauntArgs() 
            {
                // if using default filter then make it inclusive
                _filter.DefaultDeny = false;
            }
            // command line arguments
            const int DLL = 1;
            const int RESPONSE = 2;
            const int FLAGS = 3;
            const int TICKFOLDER = 4;
            const int FILEFILTERFILE = 5;
            public string Name;
            public int TicksProcessed = 0;
            public int Executions = 0;
            public long PlayTo = HistSim.ENDSIM;
            public DateTime Started = DateTime.MaxValue;
            public DateTime Stopped = DateTime.MaxValue;
            public double Seconds { get { return Stopped.Subtract(Started).TotalSeconds; } }
            public double TicksSecond { get { return Seconds == 0 ? 0 : ((double)TicksProcessed / Seconds); } }

            bool _debugs = false;
            bool _indicators = false;
            bool _trades = true;
            bool _orders = false;
            public bool Orders { get { return _orders; } set { _orders = value; } }
            public bool Trades { get { return _trades; } set { _trades = value; } }
            public bool Indicators { get { return _indicators; } set { _indicators = value; } }
            public bool Debugs { get { return _debugs; } set { _debugs = value; } }
            string _dllname = !File.Exists(WinGauntlet.Properties.Settings.Default.boxdll) ? "Responses.dll" : WinGauntlet.Properties.Settings.Default.boxdll;
            string _resp = "";
            Response _response;
            string _folder = (Properties.Settings.Default.tickfolder == null) ? Util.TLTickDir : Properties.Settings.Default.tickfolder;
            TickFileFilter _filter = new TickFileFilter();
            string _filterloc = "";
            void D(string msg) { if (GotDebug != null) GotDebug(msg); }
            public event DebugDelegate GotDebug;
            public string DllName { get { return _dllname; } set { _dllname = value; } }
            public string ResponseName { get { return _resp; } set { _resp = value; } }
            public Response Response { get { return _response; } set { _response = value; } }
            public string Folder { get { return _folder; } set { _folder = value; } }
            public TickFileFilter Filter { get { return _filter; } set { _filter = value; } }
            public string FilterLocation { get { return _filterloc; } set { _filterloc = value; } }
            public bool isUnattended { get { return (_response != null) && Directory.Exists(_folder); } }
            public override string ToString()
            {
                string[] r = new string[] { DllName, ResponseName, Folder, Flags,FilterLocation };
                return string.Join("|", r);
            }
            public void ParseArgs(string[] args)
            {
                int l = args.Length-1;
                if (l == FILEFILTERFILE)
                {
                    SetFilter(args[FILEFILTERFILE]);
                    SetFolder(args[TICKFOLDER]);
                    SetDll(args[DLL]);
                    SetResponse(args[RESPONSE]);
                    SetFlags(args[FLAGS]);
                }
                else if (l == TICKFOLDER)
                {
                    SetFolder(args[TICKFOLDER]);
                    SetDll(args[DLL]);
                    SetResponse(args[RESPONSE]);
                    SetFlags(args[FLAGS]);
                }
                else if (l == FLAGS)
                {
                    SetDll(args[DLL]);
                    SetResponse(args[RESPONSE]);
                    SetFlags(args[FLAGS]);
                }
                else if (l == RESPONSE)
                {
                    SetDll(args[DLL]);
                    SetResponse(args[RESPONSE]);
                }
                else if (l == DLL)
                {
                    SetDll(args[DLL]);

                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("GAUNTLET USAGE: ");
                    Console.WriteLine("gauntlet.exe [Response.dll] [ResponseName] [FLAGS] [TickPath] [TickFileFilterPath]");
                    Console.WriteLine("Flags: control output produced.  (O)rders (T)rades (I)ndicators (D)ebugs");
                    Console.WriteLine(@"eg: gauntlet 'c:\users\administrator\my documents\MyStrategies.dll' ");
                    Console.WriteLine("\t\tMyStrategies.MyStrategy OTIF 'c:\\tradelink\\tickdata\\' ");
                    Console.WriteLine("\t\t'c:\\users\\administrator\\my documents\\filefilter.txt'");
                    Console.WriteLine("");
                }
                D("dll|resp|fold|flags|filt: " + this.ToString());

                
            }
            string Flags { get { return (Orders ? "O" : "") + (Debugs ? "D" : "") + (Indicators ? "I" : "") + (Trades ? "T" : ""); } }
            void SetFlags(string flags)
            {
                _orders = flags.Contains("O");
                _debugs = flags.Contains("F");
                _trades = flags.Contains("T");
                _indicators = flags.Contains("I");
                D("set flags: "+Flags);
            }
            bool SetFolder(string folder)
            {
                if (!Directory.Exists(folder))
                {
                    D("no folder exists: " + folder);
                    return false;
                }
                _folder = folder;
                D("tickfolder: " + folder);
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
                bool r = _response.isValid;
                if (r)
                    D("loaded response: " + name);
                else
                    D("invalid response: " + name + " " + _response.FullName);
                return r;
            }

            bool SetDll(string file)
            {
                if (File.Exists(file))
                {
                    _dllname = file;
                    D("found dll: " + file);
                    return true;
                }
                else
                    D("no dll found: " + file);
                return false;

            }

            bool SetFilter(string file)
            {
                if (File.Exists(file))
                {
                    _filterloc = file;
                    _filter = TickFileFilter.FromFile(file);
                    D("found filter: " + file);
                    return true;
                }
                else D("no filter found: " + file);
                return false;
            }
        }

        private void _twithelp_Click(object sender, EventArgs e)
        {
            TwitPopup.Twit();
        }


    }






}
