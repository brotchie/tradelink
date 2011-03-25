using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;
using System.ComponentModel;

namespace TradeLink.Common
{
    /// <summary>
    /// historical simulation component.
    /// plays back many tickfiles insequence over time.
    /// also processes orders and executions against same tickfiles (via embedded Broker component).
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class MultiSimImpl : HistSim
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
        int _availticks;
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
        public int TicksPresent { get { return _availticks; } }
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
        /// Create a historical simulator using default tick folder and null filter
        /// </summary>
        public MultiSimImpl() : this(Util.TLTickDir, null) { }
        /// <summary>
        /// Create historical simulator with your own tick folder
        /// </summary>
        /// <param name="TickFolder"></param>
        public MultiSimImpl(string TickFolder) : this(TickFolder, null) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="tff"></param>
        public MultiSimImpl(TickFileFilter tff) : this(Util.TLTickDir, tff) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="TickFolder">tick folder to use</param>
        /// <param name="tff">filter to determine what tick files from folder to use</param>
        public MultiSimImpl(string TickFolder, TickFileFilter tff)
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
        /// change the tickfolder histsim scans for historical data
        /// </summary>
        public string Folder { get { return _folder; } set { _folder = value; D("Restarting simulator with " + _filter.ToString()); Reset(); Initialize(); } }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="filenames">list of tick files to use</param>
        public MultiSimImpl(string[] filenames)
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
            orderok = true;
            simstart = 0;
            simend = 0;
            _inited = false;
            _tickfiles = new string[0];
            Workers.Clear();
            _nextticktime = STARTSIM;
            _broker.Reset();
            _executions = 0;
            _availticks = 0;
            _tickcount = 0;

        }

        SecurityImpl getsec(int tickfileidx) { return getsec(_tickfiles[tickfileidx]); } 
        SecurityImpl getsec(string file)
        {
            SecurityImpl s = SecurityImpl.FromTIK(file);
            return s;
        }

        const string tickext = TikConst.WILDCARD_EXT;
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
            for (int i = 0; i<_tickfiles.Length; i++)
            {
                SecurityImpl s = getsec(i);
                if ((s!=null) && s.isValid && s.HistSource.isValid)
                    Workers.Add(new simworker(s));
            }
            // setup our initial index
            idx = genidx(Workers.Count);
            cidx = new int[Workers.Count];

            D("Initialized " + (_tickfiles.Length ) + " instruments.");
            D(string.Join(Environment.NewLine.ToString(), _tickfiles));
            // read in single tick just to get first time for user
            FillCache(1);

            // get total ticks represented by files
            _availticks = 0;
            for (int i = 0; i < Workers.Count; i++)
                if (Workers[i].workersec != null)
                    _availticks += Workers[i].workersec.ApproxTicks;

            D("Approximately " + TicksPresent + " ticks to process...");
            _inited = true;
            // set first time as hint for user
            setnexttime();
        }

        long simstart = 0;
        long simend = 0;
        TimeSpan runtime
        {
            get
            {
                DateTime start = new DateTime(simstart);
                DateTime end = new DateTime(simend);
                return end.Subtract(start);
            }
        }

        public double RunTimeSec { get { return runtime.TotalSeconds; } }
        public double RunTimeSecMs { get { return runtime.TotalMilliseconds; } }
        public double RunTimeTicksPerSec { get { return _tickcount / RunTimeSec; } }
        
        /// <summary>
        /// Run simulation to specific time
        /// </summary>
        /// <param name="time">Simulation will run until this time (use HistSim.ENDSIM for last time)</param>
        public void PlayTo(long ftime)
        {
            simstart = DateTime.Now.Ticks;
            orderok = true;
            if (!_inited)
                Initialize();
            if (_inited)
            {
                SecurityPlayTo(ftime); // then do stocks
            }
            else throw new Exception("Histsim was unable to initialize");
        }

        /// <summary>
        /// stops any running simulation and closes all data files
        /// </summary>
        public void Stop()
        {
            foreach (simworker w in Workers)
            {
                if (w.IsBusy) 
                    w.CancelAsync();
                if (w.workersec.HistSource.BaseStream.CanRead)
                    w.workersec.HistSource.Close();
            }
        }

        int YIELDTIME = 1;
        private void SecurityPlayTo(long ftime)
        {
            // see if we can truely thread or not
            if (Environment.ProcessorCount > 1)
            {
                // start all the workers reading files in background
                FillCache(int.MaxValue);
                // wait a moment to allow tick reading to start
                System.Threading.Thread.Sleep(_cachepause);
                // continuously loop through next ticks, playing most
                // recent ones, until simulation end is reached.
                FlushCache(ftime);
                // when we end simulation, stop reading but don't touch buffer
                CancelWorkers();
            }
            else // if we're a single core machine, add some delays
            {
                // continuously loop through next ticks sequentially, playing most
                // recent ones, until simulation end is reached.
                FlushCacheSingleCore(ftime);
            }

            // set next tick time as hint to user
            setnexttime();
            // mark end of simulation
            simend = DateTime.Now.Ticks;
        }

        

        int _cachepause = 10;
        /// <summary>
        /// milliseconds to wait between starting I/O threads and trying to access data.
        /// is used only on multi processor machines.
        /// </summary>
        public int CacheWait { get { return _cachepause; } set { _cachepause = value; } }


        void FillCache(int readahead)
        {
            // start all the workers not running
            // have them read 'readahead' ticks in advance
            for (int i = 0; i < Workers.Count; i++)
            {
                // for some reason background worker is slow exiting, recreate
                if (Workers[i].IsBusy)
                {
                    v(Workers[i].Name+ " worker#" + i + " is busy, waiting till free...");
                    // retry
                    while (Workers[i].IsBusy)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    v(Workers[i].Name + " is no longer busy.");
                    System.Threading.Thread.Sleep(10);
                }
                Workers[i].RunWorkerAsync(readahead);
                v(Workers[i].Name+ " worker#" + i + " now is working.");

            }

        }


        long lasttime = long.MinValue;
        bool orderok = true;


        void v(string msg)
        {
            D(lasttime + " " + msg);
        }

        public bool isTickPlaybackOrdered { get { return orderok; } }

        void FlushCache(long endsim)
        {
            bool simrunning = true;
            while (simrunning)
            {
                // get next times of ticks in cache
                long[] times = nexttimes();
                // copy our master index list into a temporary for sorting
                Buffer.BlockCopy(idx,0,cidx, 0,idx.Length*4);
                // sort loaded instruments by time
                Array.Sort(times, cidx);
                int nextidx = 0;
                // get next time from all instruments we have loaded
                while ((nextidx<times.Length) && (times[nextidx] == -1))
                    nextidx++;
                // test to see if ticks left in simulation
                bool ticksleft = (nextidx<times.Length);
                bool simtimeleft = ticksleft && (times[nextidx]<=endsim);
                simrunning = ticksleft && simtimeleft;
                // if no ticks left or we exceeded simulation time, quit
                if (!simrunning)
                {
                    if (!ticksleft)
                        v("No ticks left.");
                    if (!simtimeleft)
                        v("Hit end of simulation.");

                    break;
                }
                // get next tick
                Tick k = Workers[cidx[nextidx]].NextTick();
                // time check
                orderok &= k.datetime >= lasttime;
                if (orderok != lastorderok)
                {
                    v("tick out of order: " + k.symbol + " w/" + k.datetime + " behind: " + lasttime);
                    lastorderok = orderok;
                }
                // update time
                lasttime = k.datetime;
                // notify tick
                GotTick(k);
                // count tick
                _tickcount++;
            }
            v("simulating exiting.");
        }

        bool lastorderok = true;

        void FillCacheSingleCore(int readhead)
        {
            // loop through instruments and read 'readadhead' ticks in advance
            for (int i = 0; i < Workers.Count; i++)
                Workers[i].SingleCoreFillCache(readhead);
        }

        void FlushCacheSingleCore(long endsim)
        {
            bool simrunning = true;
            while (simrunning)
            {
                // get next ticks
                FillCacheSingleCore(1);
                // get next times of ticks in cache
                long[] times = nexttimes();
                // copy our master index list into a temporary for sorting
                Buffer.BlockCopy(idx, 0, cidx, 0, idx.Length * 4);
                // sort loaded instruments by time
                Array.Sort(times, cidx);
                int nextidx = 0;
                // get next time from all instruments we have loaded
                while ((nextidx < times.Length) && (times[nextidx] == -1))
                    nextidx++;
                // test to see if ticks left in simulation
                simrunning = (nextidx < times.Length) && (times[nextidx] <= endsim);
                // if no ticks left or we exceeded simulation time, quit
                if (!simrunning) 
                    break;
                // get next tick
                Tick k = Workers[cidx[nextidx]].NextTick();
                // notify tick
                GotTick(k);
                // count tick
                _tickcount++;
            }
           
        }

        const int COMPLETED = -1;

        void setnexttime()
        {
            // get next times of ticks in cache
            long[] times = nexttimes();
            int i = 0;
            // get first one available
            while ((i<times.Length) && (times[i] == COMPLETED))
                i++;
            // set next time to first available time, or end of simulation if none available
            _nextticktime = i==times.Length ? MultiSimImpl.ENDSIM : times[i];
        }


        long[] nexttimes()
        {
            // setup a next entry for every instrument
            long[] times = new long[Workers.Count];
            // loop through instrument's next time, set flag if no more ticks left in cache
            for (int i = 0; i < times.Length; i++)
            {
                // loop until worker has ticks
                while (!Workers[i].hasTicks)
                {
                    // or the worker is done reading tick stream
                    if (!Workers[i].isWorking)
                    {
                        break;
                    }
                    // if we're not done, wait for the I/O thread to catch up
                    System.Threading.Thread.Sleep(YIELDTIME);
                }
                // we should either have ticks or be finished with this worker,
                // set the value of this worker's next time value accordingly
                times[i] = Workers[i].hasTicks ? Workers[i].NextTime() : COMPLETED;
                
            }
            return times;
        }
        void CancelWorkers() { for (int i = 0; i < Workers.Count; i++) Workers[i].CancelAsync(); } 
        int[] genidx(int length) { int[] idx = new int[length]; for (int i = 0; i < length; i++) idx[i] = i; return idx; }
        public static long ENDSIM = long.MaxValue;
        public static long STARTSIM = long.MinValue;

    }

    // reads ticks from file into queue
    internal class simworker : BackgroundWorker
    {
        Queue<Tick> Ticks = new Queue<Tick>(100000);
        public SecurityImpl workersec = null;
        volatile int readcount = 0;
        public bool isWorking = false;

        bool lastworking = false;
        public bool isWorkingChange { get { bool r = lastworking != isWorking; lastworking = isWorking; return r; } }

        public bool hasTicks { get { lock (Ticks) { return (Ticks.Count > 0); } } }
        public Tick NextTick() { lock (Ticks) { return Ticks.Dequeue(); } } 
        public long NextTime() { return Ticks.Peek().datetime; }
        public string Name { get { return workersec.Symbol + workersec.Date; } }

        public simworker(SecurityImpl sec)
        {
            workersec = sec;
            WorkerSupportsCancellation = true;
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(simworker_RunWorkerCompleted);
            // if we're multi-core prepare to start I/O thread for this security
            if (Environment.ProcessorCount > 1)
            {
                DoWork += new DoWorkEventHandler(simworker_DoWork);
                workersec.HistSource.gotTick += new TickDelegate(HistSource_gotTick2);
            }
            else
            {
                workersec.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
            }

 
        }

        void HistSource_gotTick2(Tick t)
        {
            lock (Ticks)
            {
                Ticks.Enqueue(t);
            }

        }

        void HistSource_gotTick(Tick t)
        {
            Ticks.Enqueue(t);
        }


        // here is cache filling for single core
        public void SingleCoreFillCache(int readahead)
        {
            isWorking = true;
            lastworking = true;
            readcount = 0;
            while (!CancellationPending && workersec.HistSource.NextTick()
                && (readcount++ < readahead)) ;
            isWorking = false;
        }


        // this is run when I/O thread completes on multi core
        void simworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // reset counts
            readcount = 0;
            // mark as done
            isWorking = false;
            Dispose();
        }


        // fill cache for multi-core
        void simworker_DoWork(object sender, DoWorkEventArgs e)
        {
            isWorking = true;
            lastworking = true;
            int readahead = (int)e.Argument;
            // while simulation hasn't been canceled, we still have historical ticks to read and we haven't read too many, cache a tick
            while ((!e.Cancel && workersec.NextTick()
                && (readcount++ < readahead))) ;
        }




    }
}
