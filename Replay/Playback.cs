using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TradeLib;
using System.ComponentModel;


namespace TLReplay
{

    public class StockPlayback : BackgroundWorker
    {
        Broker broker = new Broker();



        public StockPlayback()
        {
            WorkerSupportsCancellation = true;

        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (CancellationPending) { e.Cancel = true; return; }
            StockPlayBackArgs sp = (StockPlayBackArgs)e.Argument;
            string Exchange = sp.Exchange;
            StreamReader cf = sp.Source;
            int lines = 0;
            int factorsize = 100;
            TickLink link = new TickLink();
            TradeLink_WM tl = sp.tl;
            tl.gotSrvFillRequest += new SrvFillRequest(tl_gotSrvFillRequest);

            // we need to put something here or in day playback to differentiate reading of indexes

            while (!cf.EndOfStream) // we have a tick
            { // process the ticks
                if (CancellationPending) { e.Cancel = true; return; }
                string line = "";
                try
                {
                    line = cf.ReadLine();
                }
                catch (IOException) { }

                eSigTick tick = new eSigTick();
                tick.factor = factorsize;
                bool good = tick.Load(line);
                tick.sym = sp.Symbol;

                lines++;

                if ((Exchange != "") && // and our tick passes our filter
                    (tick.isQuote && (!tick.be.Contains(Exchange) || !tick.oe.Contains(Exchange))) ||
                    (tick.isTrade && !tick.ex.Contains(Exchange))) continue;

                if (good && tick.hasTick) link.Tick((Tick)tick);



                // sincelast should represent the # of seconds between current and previous tick
                // (# of sec in the historical data... in the application there might be milliseconds
                // or less between current and most previous tick)

                int sincelast = 0;
                if (link.Valid)
                {
                    sincelast = link.A.time - link.B.time;
                    sincelast = (int)(sincelast / 100) + (sincelast % 100) + (link.A.sec - link.B.sec);
                }
                sincelast = (sincelast < 0) ? 0 : sincelast;

                if (sp.DelayMult != 0)
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                    System.Threading.Thread.CurrentThread.Join((int)((decimal)sincelast * 1000* sp.DelayMult));
                }



                // technically you can get filled on other exchanges even if you don't watch their 
                // prints, but for transparency and the simulation behaving as expected, we'll pretend
                // they don't exist and only execute nyse ticks.
                // otherwise this line could get moved up (to before where Exchange filter is applied)
                int fills = broker.Execute(tick);
                List<Trade> list = broker.GetTradeList();
                if (fills > 0)
                    for (int i = list.Count - fills; i < list.Count; i++)
                        tl.newFill(list[i]); // send a message for every order filled
                tl.newTick(tick);



            }
        }
            

        void tl_gotSrvFillRequest(Order order)
        {
            broker.sendOrder(order);
        }
    }

    public class StockPlayBackArgs
    {
        string sym = "";
        decimal delay = 1;
        StreamReader cf = null;
        int approxticks = 0;
        string fn = "";
        string filter = "";
        public string Exchange { get { return filter; } set { filter = value; } }
        public string FileName { get { return fn; } set { fn = value; } }
        public StreamReader Source { get { return cf; } }
        public string Symbol { get { return sym; } set { sym = value; } }
        public int Ticks { get { return approxticks; } set { approxticks = value; } }
        public decimal DelayMult { get { return delay; } set { delay = value; } }
        public TradeLink_WM tl = null;
        public StockPlayBackArgs(StreamReader EPFfile,TradeLink_WM tlink) 
        {
            tl = tlink;
            cf = EPFfile; 
            string symline = cf.ReadLine();
            string dateline = cf.ReadLine();
            Regex se = new Regex("=[a-z-A-Z]+");
            Regex dse = new Regex("=[0-9V]+");
            Regex dee = new Regex("-[0-9V]+");
            MatchCollection r = se.Matches(symline, 0);
            string t = r[0].Value;
            Symbol = t.Substring(1, t.Length - 1);
            Regex sm = new Regex("[a-z]{1,3}", RegexOptions.IgnoreCase);
            Match m = sm.Match(FileName);
            Symbol = Symbol.ToUpper();
        }

    }

    public class IndexArgs
    {
        public TradeLink_WM tl = null;
        public StreamReader cf = null;
        string index = "";
        public string Index { get { return index; } }
        public decimal DelayMult = 0;
        public IndexArgs(string name, StreamReader indexfile, TradeLink_WM link)
        {
            cf = indexfile;
            index = name;
            tl = link;
        }
    }

    public class IndexPlayback : BackgroundWorker
    {
        public IndexPlayback()
        {
            WorkerSupportsCancellation = true;
        }


        protected override void OnDoWork(DoWorkEventArgs e)
        {
            IndexArgs ia = (IndexArgs)e.Argument;
            string name = ia.Index;
            int lasttime = 0;
            while (!ia.cf.EndOfStream)
            {
                string line = "";
                try
                {
                    line = ia.cf.ReadLine();
                }
                catch (Exception) { return; }

                Index i = Index.Deserialize(line);

                // pause here if necessary
                int sincelast = 0;
                if (lasttime == 0) lasttime = i.Time;
                else sincelast = i.Time - lasttime;
                sincelast = (int)(sincelast / 100) + (sincelast % 100);
                sincelast = (sincelast < 0) ? 0 : sincelast;

                // delay appropriate interval before next tick
                // works same as stock playback, except we don't have second resolution for index ticks
                if (ia.DelayMult != 0)
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                    System.Threading.Thread.CurrentThread.Join((int)((decimal)sincelast * 60000 * ia.DelayMult));
                }


                ia.tl.newIndexTick(i);

            }
            
        }
    }

    public class DayPlayback : BackgroundWorker
    {

        string exfilter = "";
        int delay = 1;
        public void ExchFilter(string ExchangeFilter) { exfilter = ExchangeFilter.Trim(); }
        public void DelayMult(int delay) { this.delay = delay; }
        TradeLink_WM tl = null;
        public TradeLink_WM TLInst { get { return tl; } set { tl = value; } }

        public DayPlayback(string tickdatapath) : this (tickdatapath,"","(SP#F)|([.]IDX)") { }
        public DayPlayback(string tickdatapath,string include) : this(tickdatapath,include,"(SP#F)|([.]IDX)") { }

        public DayPlayback(string tickdatapath,string include,string exclude)
        {
            WorkerSupportsCancellation = true;
            DirectoryInfo di = new DirectoryInfo(tickdatapath);
            FileInfo[] files = di.GetFiles("*.epf");
            Regex ir = new Regex(include, RegexOptions.IgnoreCase);
            Regex er = new Regex(exclude, RegexOptions.IgnoreCase);

            for (int i = 0; i < files.Length; i++)
                if ((ir.IsMatch(files[i].Name)) && (!er.IsMatch(files[i].Name)))
                {
                    int fileticks = (int)(files[i].Length / 39);
                    totalticks += fileticks;
                    tf.Add(files[i].FullName, fileticks);
                }

            files = di.GetFiles("*.idx"); // now get the indicies
            for (int i = 0; i < files.Length; i++)
                if (ir.IsMatch(files[i].Name)) 
                    ti.Add(files[i].FullName);
        }
        Dictionary<string, int> tf = new Dictionary<string, int>();
        List<string> ti = new List<string>();
        public int totalticks = 0;

        protected override void  OnDoWork(DoWorkEventArgs e)
        {
            string [] clients = tl.SrvGetClients();
            if ((tl == null) || (clients.Length==0)) return;
            foreach (string filename in tf.Keys)// loop through the list
            {
                if (CancellationPending) { e.Cancel = true; return; }
                StockPlayBackArgs args = new StockPlayBackArgs(new StreamReader(filename),tl);
                args.FileName = filename;
                args.Ticks = tf[filename];
                args.Exchange = exfilter;
                args.DelayMult = delay;
                StockPlayback splay = new StockPlayback();
                splay.RunWorkerCompleted += new RunWorkerCompletedEventHandler(splay_RunWorkerCompleted);
                splay.RunWorkerAsync(args);
            }

            for (int i = 0; i < ti.Count; i++)
            {
                if (CancellationPending) { e.Cancel = true; return; }
                string fn = ti[i];
                Regex fr = new Regex(@"^.*[\\]([a-z]+)[0-9]+[.]IDX$",RegexOptions.IgnoreCase);
                string name = "/"+fr.Replace(fn, "$1");
                IndexArgs args = new IndexArgs(name, new StreamReader(fn), tl);
                args.DelayMult = delay;
                IndexPlayback iplay = new IndexPlayback();
                iplay.RunWorkerCompleted += new RunWorkerCompletedEventHandler(iplay_RunWorkerCompleted);
                iplay.RunWorkerAsync(args);

            }
        }

        void iplay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }


        void splay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }

}
