using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TradeLib;
using System.Reflection;
using System.ComponentModel;

namespace WinGauntlet
{

    public class BackTest : BackgroundWorker
    {
        public event DebugFullDelegate BTStatus;
        private string include = "";
        private string exclude = "";
        private string PATH = null;
        public Broker mybroker;
        public string name;
        private TextReader cf;
        int factorsize = 100;
        private int line = 0;
        private int delay = 0;
        public string exfilter = "";
        public string symbol;
        public bool debug = false;
        private bool _idx = true;
        public int time;
        public string aname = "c:\\program files\\tradelink\\tradelinksuite\\box.dll";
        public eSigTick tick = new eSigTick();
        public int bint = 5;
        public bool UseIndex { get { return _idx; } set { _idx = value; } }
        public string Path { get { return PATH; } set { PATH = value; } }
        public void Debug(bool deb) { this.debug = deb; }
        public void ExchFilter(string exfilter) { this.exfilter = exfilter.Trim();}
        public void TickDelay(int delay) { this.delay = delay; }
        public void Name(string name) { this.name = name; }
        public int Interval { get { return bint; } set { bint = value; } }
        public string Include { get { return include; } set { include = value; } }
        public string Exclude { get { return exclude; } set { exclude = value; } }

        public BackTest() 
        {
            this.name = "GauntletRun";
            this.mybroker = new Broker();
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = false;
            this.DoWork += new DoWorkEventHandler(BackTest_DoWork);
        }

        void BackTest_DoWork(object sender, DoWorkEventArgs e)
        {
            BackTestArgs arg = (BackTestArgs)e.Argument;
            Test(arg.Files,arg.Box);
        }
        public void TickFile(string filename) 
        {
            this.cf = new StreamReader(PATH+filename);
            this.line = 0;
            string symline = this.cf.ReadLine();
            string dateline = this.cf.ReadLine();
            Regex se = new Regex("=[a-z-A-Z]+");
            Regex dse = new Regex("=[0-9V]+");
            Regex dee = new Regex("-[0-9V]+");
            MatchCollection r = se.Matches(symline, 0);
            string t = r[0].Value;
            this.symbol = t.Substring(1, t.Length - 1);
            Regex sm = new Regex("[a-z]{1,3}",RegexOptions.IgnoreCase);
            Match m = sm.Match(filename);
            this.symbol = this.symbol.ToUpper();
            if (!this.symbol.Equals(m.Value.ToUpper())) this.show(filename + " has symbol name " + m.Value + " but contains ticks for " + symbol+Environment.NewLine);
        }

        int approxticks(List<FileInfo> fi)
        {
            int ticks = 0;
            for (int i = 0; i < fi.Count; i++)
                ticks += (int)(fi[i].Length / 39);
            return ticks;
        }

        

        public int Test(List<FileInfo> tf,Response mybox) 
        {
            show("Starting run "+name+" containing "+ tf.Count + " symbols."+Environment.NewLine);
            int totfills = 0;
            int totalticks = approxticks(tf);

            for (int i = 1; i <= tf.Count; i++) // loop through the list
            {
                FileInfo f = tf[i-1];
                Match m = Regex.Match(f.Name, "([0-9]+)", RegexOptions.IgnoreCase);
                int date = Convert.ToInt32(m.Result("$1"));
                TickFile(f.Name); // set current file
                if (f.Length == 0) continue; // ignore if tick file is empty
                show(Environment.NewLine);
                show("Symbol " + this.symbol + " (" + i + " of " + tf.Count + ") ");

                // reset per-symbol statistics
                if (mybox!=null) mybox.Reset();
                int fills = 0;
                tick = new eSigTick(); // reset our tick
                

                while (this.getTick() && tick.hasTick)
                { // process the ticks
                    line++;
                    if ((line % 5000) == 0) show(".");

                    if ((this.exfilter != "") &&
                        ((!tick.isTrade && (!tick.be.Contains(exfilter) || !tick.oe.Contains(exfilter))) ||
                         (tick.isTrade && tick.ex.Contains(exfilter)))) continue;

                    // execute any pending orders on this tick
                    if (mybroker.GetOrderList().Count>0) fills += mybroker.Execute(tick); 
                    // trade box on this tick, if he generates any orders then send them
                    if (mybox != null)
                    {
                        mybox.GotTick(tick);
                        // quit early if box shuts itself off and no pending orders
                        if (!mybox.isValid && (mybroker.GetOrderList().Count == 0)) break;
                    }


                    if (this.delay != 0)
                    {
                        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                    }
                    this.ReportProgress((int)((100*line) / totalticks));
                }
                show(fills+" executions.");
                totfills += fills;
                this.cf.Close();
            }
            show(Environment.NewLine);
            show(name+" complete: "+tf.Count+" symbols, "+ totfills + " executions."+Environment.NewLine);
            return totfills;
        }

        public bool getTick()
        {
            string line = "";
            try
            {
                line = this.cf.ReadLine();
            }
            catch (Exception ex) { show(ex.Message+Environment.NewLine); }
            tick = new eSigTick();
            tick.factor = factorsize;
            bool good = false;
            try
            {
                good = tick.Load(line);
            }
            catch (AlreadySmallSizeException)
            {
                factorsize = 1; // fix it for this file
                tick.factor = factorsize;
                good = tick.Load(line);
            }
            tick.symbol = symbol;
            return good;
        }





        public void show(string debug)
        {
            if (BTStatus != null)
                BTStatus(TradeLib.Debug.Create(debug, DebugLevel.Status));
        }

    }

    public class BackTestArgs
    {
        public Response Box = new InvalidResponse();
        public List<FileInfo> Files = new List<FileInfo>();
        public BackTestArgs(List<FileInfo> files, Response box) { Box = box; Files = files; }
    }


}
