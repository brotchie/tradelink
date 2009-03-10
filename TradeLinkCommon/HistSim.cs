using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;
using System.ComponentModel;

namespace TradeLink.Common
{
    public class HistSim 
    {
        // working variables
        string _folder = Util.TLTickDir;
        TickFileFilter _filter = new TickFileFilter();
        Broker _broker = new Broker();
        string[] _tickfiles = new string[0];
        bool _inited = false;
        long _nextticktime = ENDSIM;
        int _executions = 0;
        volatile int _tickcount;
        long _bytestoprocess = 0;
        List<simworker> Workers = new List<simworker>();
        int[] idx;
        int[] cidx;
        
        // events
        public event TickDelegate GotTick;
        public event DebugDelegate GotDebug;
        
        // user-facing interfaces
        public TickFileFilter FileFilter { get { return _filter; } set { _filter = value; D("Restarting simulator with " + _filter.ToString()); Reset(); Initialize(); } }
        /// <summary>
        /// Total ticks available for processing, based on provided filter or tick files.
        /// </summary>
        public int TicksPresent { get { return (int)Math.Floor((double)_bytestoprocess/39); } }
        /// <summary>
        /// Ticks processed in this simulation run.
        /// </summary>
        public int TicksProcessed { get { return _tickcount; } }
        /// <summary>
        /// Fills executed during this simulation run.
        /// </summary>
        public int FillCount { get { return _executions; } }
        /// <summary>
        /// Gets next tick in the simulation
        /// </summary>
        public long NextTickTime { get { return _nextticktime; } }
        /// <summary>
        /// Gets broker used in the simulation
        /// </summary>
        public Broker SimBroker { get { return _broker; }  }
        
        /// <summary>
        /// Create a historical simulator using default tick folder and null filter
        /// </summary>
        public HistSim() : this(Util.TLTickDir, null) { }
        /// <summary>
        /// Create historical simulator with your own tick folder
        /// </summary>
        /// <param name="TickFolder"></param>
        public HistSim(string TickFolder) : this(TickFolder, null) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="tff"></param>
        public HistSim(TickFileFilter tff) : this(Util.TLTickDir, tff) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="TickFolder">tick folder to use</param>
        /// <param name="tff">filter to determine what tick files from folder to use</param>
        public HistSim(string TickFolder, TickFileFilter tff)
        {
            _folder = TickFolder;
            if (tff != null)
                _filter = tff;
            else
            {
                _filter.DefaultDeny = false;
            }
        }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="filenames">list of tick files to use</param>
        public HistSim(string[] filenames)
        {
            _tickfiles = filenames;
        }
        private void D(string message)
        {
            if (GotDebug!=null) GotDebug(message);
        }
        /// <summary>
        /// Reset the simulation
        /// </summary>
        public void Reset()
        {
            _inited = false;
            _tickfiles = new string[0];
            Workers.Clear();
            _nextticktime = STARTSIM;
            _broker.Reset();
            _executions = 0;
            _bytestoprocess = 0;
            _tickcount = 0;

        }

        const string tickext = "*.EPF";
        /// <summary>
        /// Reinitialize the cache
        /// </summary>
        public void Initialize()
        {
            if (_inited) return; // only init once
            if (_tickfiles.Length == 0)
            {
                // get our listings of historical files (idx and epf)
                string[] files = Directory.GetFiles(_folder, tickext);
                _tickfiles = _filter.Allows(files);
            }

            // now we have our list, initialize instruments from files
            foreach (string file in _tickfiles)
            {
                SecurityImpl s = SecurityImpl.FromFile(file);
                if (s!=null)
                    Workers.Add(new simworker(s));
            }
            // setup our initial index
            idx = genidx(Workers.Count);
            cidx = new int[Workers.Count];

            D("Initialized " + (_tickfiles.Length ) + " instruments.");
            D(string.Join(Environment.NewLine.ToString(), _tickfiles));
            // read in single tick just to get first time for user
            FillCache(1);

            // get total bytes represented by files
            DirectoryInfo di = new DirectoryInfo(_folder);
            FileInfo[] fi = di.GetFiles("*.epf", SearchOption.AllDirectories);
            foreach (FileInfo thisfi in fi)
            {
                foreach (string file in _tickfiles)
                    if (thisfi.FullName==file)
                        _bytestoprocess += thisfi.Length;
            }
            D("Approximately " + TicksPresent + " ticks to process...");
            _inited = true;
            // set first time as hint for user
            setnexttime();
        }
        /// <summary>
        /// Run simulation to specific time
        /// </summary>
        /// <param name="time">Simulation will run until this time (use HistSim.ENDSIM for last time)</param>
        public void PlayTo(long ftime)
        {
            if (!_inited)
                Initialize();
            if (_inited)
            {
                SecurityPlayTo(ftime); // then do stocks
            }
            else throw new Exception("Histsim was unable to initialize");
        }

        private void SecurityPlayTo(long ftime)
        {
            // start all the workers reading files in background
            FillCache(int.MaxValue);
            // wait a moment to allow tick reading to start
            System.Threading.Thread.Sleep(5);
            // continuously loop through next ticks, playing most
            // recent ones, until simulation end is reached.
            FlushCache(ftime);
            // when we end simulation, stop reading but don't touch buffer
            CancelWorkers();
            // set next tick time as hint to user
            setnexttime();
        }


        void FillCache(int readahead)
        {
            // start all the workers not running
            // have them read 'readahead' ticks in advance
            for (int i = 0; i < Workers.Count; i++)
                if (!Workers[i].IsBusy)
                    Workers[i].RunWorkerAsync(readahead);

        }


        void FlushCache(long endsim)
        {
            bool simrunning = true;
            while (simrunning)
            {
                long[] times = nexttimes();
                Buffer.BlockCopy(idx,0,cidx, 0,idx.Length*4);
                Array.Sort(times, cidx);
                int nextidx = 0;
                while ((nextidx<times.Length) && (times[nextidx] == -1))
                    nextidx++;
                simrunning = (nextidx<times.Length) && (times[nextidx]<=endsim);
                if (!simrunning) break;
                Tick k = Workers[cidx[nextidx]].NextTick();
                _executions += SimBroker.Execute(k);
                GotTick(k);
                _tickcount++;
            }
        }
        void setnexttime()
        {
            long[] times = nexttimes();
            int i = 0;
            while ((i<times.Length) && (times[i] == -1))
                i++;
            _nextticktime = i==times.Length ? HistSim.ENDSIM : times[i];
        }
        long[] nexttimes()
        {
            long[] times = new long[Workers.Count];
            for (int i = 0; i < times.Length; i++)
                times[i] = Workers[i].hasUnread ? Workers[i].NextTime() : -1;
            return times;
        }
        void CancelWorkers() { for (int i = 0; i < Workers.Count; i++) Workers[i].CancelAsync(); } 
        int[] genidx(int length) { int[] idx = new int[length]; for (int i = 0; i < length; i++) idx[i] = i; return idx; }
        public static long ENDSIM = long.MaxValue;
        public static long STARTSIM = long.MinValue;

    }

    // reads ticks from file into queue
    public class simworker : BackgroundWorker
    {
        Queue<Tick> Ticks = new Queue<Tick>(100000);
        SecurityImpl workersec = null;
        volatile int readcount = 0;
        public bool hasUnread { get { lock (Ticks) { return Ticks.Count>0; } } }
        public Tick NextTick() { lock (Ticks) { return Ticks.Dequeue(); } } 
        public long NextTime() { return Ticks.Peek().datetime; } 
        public simworker(SecurityImpl sec)
        {
            workersec = sec;
            DoWork += new DoWorkEventHandler(simworker_DoWork);
            WorkerSupportsCancellation = true;
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(simworker_RunWorkerCompleted);
        }

        void simworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // reset counts
            readcount = 0;
        }

        void simworker_DoWork(object sender, DoWorkEventArgs e)
        {
            int readahead = (int)e.Argument;
            while (!e.Cancel && workersec.hasHistorical && (readcount++ < readahead))
                lock (Ticks)
                {
                    Ticks.Enqueue(workersec.NextTick());
                }
           
        }




    }
}
