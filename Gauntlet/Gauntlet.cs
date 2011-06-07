using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;
using TradeLink.AppKit;

namespace WinGauntlet
{
    public partial class Gauntlet : AppTracker
    {


        HistSim h;
        BackgroundWorker bw = new BackgroundWorker();
        BackgroundWorker getsymwork = new BackgroundWorker();
        static GauntArgs args = new GauntArgs();
        public const string PROGRAM = "Gauntlet";
        StreamWriter indf;

        Log _log = new Log(PROGRAM);



        public Gauntlet()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            debug(Util.TLSIdentity());
            args.GotDebug += new DebugDelegate(args_GotDebug);
            args.ParseArgs(Environment.GetCommandLineArgs());
            FormClosing += new FormClosingEventHandler(Gauntlet_FormClosing);
            debug(RunTracker.CountNewGetPrettyRuns(PROGRAM, Util.PROGRAM));
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            getsymwork.WorkerSupportsCancellation = true;
            getsymwork.DoWork += new DoWorkEventHandler(getsymwork_DoWork);
            getsymwork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(getsymwork_RunWorkerCompleted);
            getsymwork.RunWorkerAsync();
            getsymwork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(getsymwork_RunWorkerCompleted);

            if (args.isUnattended)
            {
                ordersincsv.Checked = true;
                //ShowWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, SW_MINIMIZE);
                bindresponseevents();
                queuebut_Click(null, null);
            }
            else
            {
                status("wait while tickdata is loaded...");
                UpdateResponses(Util.GetResponseList(args.DllName));
            }

        }

        void getsymwork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
                status("completed loading tick data.");
            else
            {
                status("error loading data");
                debug(e.Error.Message + e.Error.StackTrace);
            }
            queuebut.Enabled = true;
            queuebut.Invalidate();
        }

        void getsymwork_DoWork(object sender, DoWorkEventArgs e)
        {
            queuebut.Enabled = false;
            queuebut.Invalidate();
            tickFileFilterControl1.SetSymbols(args.Folder);
        }

        void args_GotDebug(string msg)
        {
            Console.WriteLine(msg);
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
                if (!args.isUnattended) MessageBox.Show(msg);
                return;
            }
            // prepare other arguments for the run
            if (!args.isUnattended)
            {
                args.Orders = ordersincsv.Checked;
                args.Indicators = _indicatcsv.Checked;
                args.Debugs = _debugs.Checked;
                args.Filter = tickFileFilterControl1.GetFilter();
            }
            // clear results
            clearresults();
            // set names and times
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
        Broker SimBroker = new Broker();

        // runs the simulation in background
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            FillCount = 0;
            // get simulation arguments
            GauntArgs ga = (GauntArgs)e.Argument;
            // notify user
            debug("Run started: " + ga.Name);
            status("Started: " + ga.ResponseName);
            // prepare simulator
            bool portreal = _portfoliosim.Checked;
            if (_portfoliosim.Checked)
            {
                debug("Using portfolio simulation. (realistic)");
                h = new MultiSimImpl(ga.Folder, ga.Filter);
            }
            else
            {
                debug("Using sequential symbol simulation. (faster)");
                h = new SingleSimImpl(ga.Folder, ga.Filter);
            }
            h.GotDebug += new DebugDelegate(h_GotDebug);
            SimBroker.UseBidAskFills = _usebidask.Checked;
            h.GotTick += new TickDelegate(h_GotTick);
            SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            SimBroker.GotOrder+=new OrderDelegate(args.Response.GotOrder);
            SimBroker.GotOrderCancel += new OrderCancelDelegate(SimBroker_GotOrderCancel);
            // start simulation
            h.PlayTo(ga.PlayTo);
            // end simulation
            ga.Stopped = DateTime.Now;
            ga.TicksProcessed = h.TicksProcessed;
            ga.Executions = FillCount;
            // save result
            e.Result = ga;
        }
        int FillCount = 0;
        void SimBroker_GotFill(Trade t)
        {
            FillCount++;
            args.Response.GotFill(t);
        }

        void SimBroker_GotOrderCancel(string sym, bool side, long id)
        {
            args.Response.GotOrderCancel(id);
        }

        void clearresults()
        {
            FillCount = 0;
            tradeResults1.Clear();
            SimBroker.Reset();
        }

        // runs after simulation is complete
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            debug(_sb.ToString());
            _sb = new System.Text.StringBuilder(10000000);
            GauntArgs gargs = (GauntArgs)e.Result;
            if (!e.Cancelled)
            {
                List<Trade> list = SimBroker.GetTradeList();
                tradeResults1.NewResultTrades(LogFile("Trades"),list);
                if (gargs.Trades)
                {
                    debug("writing "+list.Count+" trades...");
                    Util.ClosedPLToText(list, ',', LogFile("Trades"));
                }
                if (gargs.Orders)
                {
                    List<Order> olist = SimBroker.GetOrderList();
                    debug("writing "+olist.Count+" orders...");
                    StreamWriter sw = new StreamWriter(LogFile("Orders"), false);
                    string[] cols = Enum.GetNames(typeof(OrderField));
                    sw.WriteLine(string.Join(",", cols));
                    for (int i = 0; i < olist.Count; i++)
                        sw.WriteLine(OrderImpl.Serialize(olist[i]));
                    sw.Close();
                }
                string msg = "Done.  Ticks: " + gargs.TicksProcessed + " Speed:" + gargs.TicksSecond.ToString("N0") + " t/s  Fills: " + gargs.Executions.ToString();
                debug(msg);
                status(msg);
                if (CapitalRequestConfim.ConfirmSubmitCapitalRequest(tradeResults1.CurrentResults, _capitalprompt.Checked, debug))
                    status("sent capital connection request.");
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
            if (args.isUnattended)
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
        long lastp = 0;
        string nowtime = "0";
        void h_GotTick(Tick t)
        {
            
            nowtime = t.time.ToString();
            // execute open orders
            SimBroker.Execute(t);
            if (args.Response == null) return;
            if (t.depth > _depth) return;
            count++;
            try
            {
                args.Response.GotTick(t);
            }
            catch (Exception ex) { debug("response threw exception: " + ex.Message); }
            if (args.isUnattended) return;
            long percent = (long)((double)count*100 / h.TicksPresent);
            if ((percent!=lastp) && (percent % 5 == 0))
            {
                updatepercent(percent);
                lastp = percent;
            }

        }

        void updatepercent(long per)
        {
            if (InvokeRequired)
                Invoke(new LongDelegate(updatepercent), new object[] { per });
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
            string folder = Util.TLTickDir;
            if (Directory.Exists(args.Folder))
                folder = args.Folder;
            fd.SelectedPath = folder;
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

        string OUTFOLD = Util.ProgramData(PROGRAM)+ "\\";
        void debug(string message) 
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(debug), new object[] { message });
            else
            {
                messages.AppendText(message+Environment.NewLine);
                messages.Invalidate(true);
            }
            _log.GotDebug(message);
        }

        void status(string message)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DebugDelegate(status), new object[] { message });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                try
                {
                    lastmessage.Text = message;
                    lastmessage.Invalidate();
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
            try
            {
                bw.CancelAsync();
                getsymwork.CancelAsync();
                if (indf != null)
                    indf.Close();
                h.Stop();
            }
            catch { }
            if (saveonexit.Checked && !args.isUnattended)
            {
                Properties.Settings.Default.tickfolder = args.Folder;
                Properties.Settings.Default.boxdll = args.DllName;
                WinGauntlet.Properties.Settings.Default.Save();
            }
            _log.Stop();
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
            string rname = string.Empty;
            try
            {
                rname = (string)reslist.SelectedItem;
                args.Response = ResponseLoader.FromDLL(rname, args.DllName);
            }
            catch (Exception ex) { status("Response failed to load, quitting... (" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message.ToString() : "") + ")"); }
            if ((args==null) || (args.Response==null) || !args.Response.isValid) { status("Response did not load or loaded in a shutdown state. "+rname); return; }
            args.ResponseName = args.Response.FullName;
            _boundonce = false;
            bindresponseevents();
            args.Response.Reset();
        }

        bool _boundonce = false;
        bool bindresponseevents()
        {
            if ((args.Response== null) || !args.Response.isValid)
                return false;
            if (_boundonce) return true;
            args.Response.ID = 0;
            args.Response.SendTicketEvent += new TicketDelegate(Response_SendTicketEvent);
            args.Response.SendMessageEvent += new MessageDelegate(Response_SendMessage);
            args.Response.SendIndicatorsEvent += new ResponseStringDel(Response_SendIndicators);
            args.Response.SendDebugEvent += new DebugFullDelegate(Response_GotDebug);
            args.Response.SendCancelEvent += new LongSourceDelegate(Response_CancelOrderSource);
            args.Response.SendOrderEvent += new OrderSourceDelegate(Response_SendOrder);
            args.Response.SendBasketEvent += new BasketDelegate(Response_SendBasket);
            args.Response.SendChartLabelEvent += new ChartLabelDelegate(Response_SendChartLabel);
            _boundonce = true;
            return true;
        }
        bool _sendticketwarn = false;
        void Response_SendTicketEvent(string space, string user, string password, string summary, string description, Priority pri, TicketStatus stat)
        {
            if (_sendticketwarn) return;
            debug("Sendticket not supported in gauntlet.");
            _sendticketwarn = true;
        }

        bool _sendbaskwarn = false;
        void Response_SendBasket(Basket b, int id)
        {
            if (_sendbaskwarn) return;
            debug("Sendbasket not supported in gauntlet.");
            debug("To specify trading symbols, select symbols via GUI.");
            _sendbaskwarn = true;
        }

        void Response_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color c)
        {
            
        }
        int _depth = 0;
        void Response_SendMessage(MessageTypes type, long source, long dest, long id, string data, ref string response)
        {
            switch (type)
            {
                case MessageTypes.DOMREQUEST:
                    
                    int d = 0;
                    string[] r = MessageTracker.ParseRequest(data);
                    if (r.Length > 1)
                        if (int.TryParse(MessageTracker.RequestParam(data,1), out d))
                            _depth = d;
                    break;
            }
        }

        void Response_SendIndicators(int id, string param)
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


        void Response_SendOrder(Order o, int id)
        {
            if (h!=null)
                SimBroker.SendOrderStatus(o);
        }

        void mybroker_GotOrderCancel(string sym, bool side, long id)
        {
            if (args.Response != null)
                args.Response.GotOrderCancel(id);
        }

        void Response_CancelOrderSource(long number, int id)
        {
            if (h!=null)
                SimBroker.CancelOrder(number);
            
        }

        System.Text.StringBuilder _sb = new System.Text.StringBuilder(10000000);

        void Response_GotDebug(Debug msg)
        {
           // _sb.AppendLine(msg.Msg);
            _sb.AppendFormat("{0}: {1}{2}", nowtime, msg.Msg, Environment.NewLine);
            
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
            public long PlayTo = MultiSimImpl.ENDSIM;
            public DateTime Started = DateTime.MaxValue;
            public DateTime Stopped = DateTime.MaxValue;
            public double Seconds { get { return Stopped.Subtract(Started).TotalSeconds; } }
            public double TicksSecond { get { return Seconds == 0 ? 0 : ((double)TicksProcessed / Seconds); } }

            bool _debugs = true;
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
            public bool hasPrereq { get { return (_response != null) && Directory.Exists(_folder); } }
            bool _background = false;
            public bool isUnattended { get { return _background; } }
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
                if (hasPrereq)
                    _background = true;
                
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
                try
                {
                    _response = ResponseLoader.FromDLL(_resp, DllName);
                }
                catch (Exception ex)
                {
                    D(ex.Message + ex.StackTrace);
                }
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
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _log.Content, null, null, false);
        }

        private void _viewresults_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Util.ProgramData(PROGRAM));
        }


        private void _docapcon_Click(object sender, EventArgs e)
        {
            CapitalRequestConfim.ConfirmSubmitCapitalRequest(tradeResults1.CurrentResults, false, debug);
        }


    }






}
