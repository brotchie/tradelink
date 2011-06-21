using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;
using TradeLink.Common;
using System.ComponentModel;

namespace TradeLink.Common
{
    /// <summary>
    /// historical simulation component.
    /// plays back many tickfiles insequence over time.
    /// different than multi-sim, multiple symbols in the same day are not guaranteed to be played back in sequence.
    /// also processes orders and executions against same tickfiles (via embedded Broker component).
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class SingleSimImpl : HistSim
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
        List<long> dates = new List<long>();
        List<SecurityImpl> secs = new List<SecurityImpl>();

        
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
        /// Gets broker used in the simulation
        /// </summary>
        public Broker SimBroker { get { return _broker; }  }
        
        /// <summary>
        /// Create a historical simulator using default tick folder and null filter
        /// </summary>
        public SingleSimImpl() : this(Util.TLTickDir, null) { }
        /// <summary>
        /// Create historical simulator with your own tick folder
        /// </summary>
        /// <param name="TickFolder"></param>
        public SingleSimImpl(string TickFolder) : this(TickFolder, null) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="tff"></param>
        public SingleSimImpl(TickFileFilter tff) : this(Util.TLTickDir, tff) { }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="TickFolder">tick folder to use</param>
        /// <param name="tff">filter to determine what tick files from folder to use</param>
        public SingleSimImpl(string TickFolder, TickFileFilter tff)
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
        public SingleSimImpl(string[] filenames)
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
            dates.Clear();
            secs.Clear();
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
            List<long> d = new List<long>(_tickfiles.Length);
            List<SecurityImpl> ss = new List<SecurityImpl>(_tickfiles.Length);
            // now we have our list, initialize instruments from files
            for (int i = 0; i<_tickfiles.Length; i++)
            {
                SecurityImpl s = getsec(i);
                if ((s != null) && s.isValid && s.HistSource.isValid)
                {
                    s.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
                    ss.Add(s);
                    d.Add(s.Date);
                }
            }
            // setup our initial index
            long[] didx = d.ToArray();
            SecurityImpl[] sidx = ss.ToArray();
            Array.Sort(didx, sidx);
            // save index and objects in order
            secs.Clear();
            dates.Clear();
            secs.AddRange(sidx);
            dates.AddRange(didx);
            doneidx = _tickfiles.Length - 1;


            D("Initialized " + (_tickfiles.Length ) + " instruments.");
            D(string.Join(Environment.NewLine.ToString(), _tickfiles));
            // check for event
            if (GotTick != null)
                hasevent = true;
            else
                D("No GotTick event defined!");
            // read in single tick just to get first time for user
            isnexttick();

            // get total ticks represented by files
            _availticks = 0;
            for (int i = 0; i < secs.Count; i++)
                if (secs[i]!= null)
                    _availticks += secs[i].ApproxTicks;

            D("Approximately " + TicksPresent + " ticks to process...");
            _inited = true;
        }

        Tick next;
        bool hasnext = false;
        bool hasevent = false;
        int doneidx = 0;
        bool lasttick = false;

        bool isnexttick()
        {
            if (cidx > doneidx)
            {
                lasttick = true;
                HistSource_gotTick(null);
                return false;
            }
            if (!secs[cidx].NextTick())
                cidx++;
            return true;
        }
        
        void HistSource_gotTick(Tick t)
        {
            // process next tick if present
            if (hasnext)
            {
                // execute any pending orders
                SimBroker.Execute(next);
                // send existing tick
                if (hasevent)
                    GotTick(next);
                // update last time
                lasttime = next.datetime;
                orderok &= lasttick || (t.datetime >= next.datetime) || (cidx!=next.symidx);

            }
            if (lasttick)
            {
                hasnext = false;
                return;
            }
            // update next tick
            next = t;
            next.symidx = cidx;
            hasnext = true;
            _nextticktime = t.datetime;
            _tickcount++;
        }

        int cidx = 0;

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
            go = true;
            if (!_inited)
                Initialize();
            if (_inited)
            {
                // process
                while (go && (NextTickTime < ftime) && isnexttick())
                    ;
            }
            else throw new Exception("Histsim was unable to initialize");
            // mark end of simulation
            simend = DateTime.Now.Ticks;
        }

        /// <summary>
        /// stops any running simulation and closes all data files
        /// </summary>
        public void Stop()
        {
            go = false;
        }

        bool go = true;

        

       


        long lasttime = long.MinValue;
        bool orderok = true;


        void v(string msg)
        {
            D(lasttime + " " + msg);
        }

        public bool isTickPlaybackOrdered { get { return orderok; } }

        

        bool lastorderok = true;

        

        const int COMPLETED = -1;

        
        public static long ENDSIM = long.MaxValue;
        public static long STARTSIM = long.MinValue;



        public static bool PlayHistoricalTicks(Response r, string symbol, int currentdate, int daysback, int expecteddays, DebugDelegate deb)
        {
            GenericTracker<string> symbols = new GenericTracker<string>();
            symbols.addindex(symbol, symbol);
            // calculate stop date
            DateTime stop = Util.ToDateTime(currentdate, 0);
            // calculate start date
            DateTime start = stop.Subtract(new TimeSpan(daysback, 0, 0, 0));
            // return results
            return PlayHistoricalTicks(r, symbols, start, stop, expecteddays, deb);
        }

        public static bool PlayHistoricalTicks(Response r, GenericTracker<string> symbols, int currentdate, int daysback, int expecteddays, DebugDelegate deb)
        {
            // calculate stop date
            DateTime stop = Util.ToDateTime(currentdate, 0);
            // calculate start date
            DateTime start = stop.Subtract(new TimeSpan(daysback, 0, 0, 0));
            // return results
            return PlayHistoricalTicks(r, symbols, start, stop, expecteddays, deb);
        }
        public static bool PlayHistoricalTicks(Response r, GenericTracker<string> symbols, DateTime start, DateTime endexclusive, int expecteddays, DebugDelegate deb)
        {
            bool skipexpected = expecteddays == 0;
            // prepare to track actual days for each symbol
            GenericTracker<int> actualdays = new GenericTracker<int>();
            foreach (string sym in symbols)
                actualdays.addindex(sym, 0);
            // prepare to track all tickfiles
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
            // get all required tickfiles for each symbol on each date
            DateTime now = new DateTime(start.Ticks);
            int tfc = 0;
            while (now < endexclusive)
            {
                // get the tick files
                List<string> allfiles = TikUtil.GetFilesFromDate(Util.TLTickDir, Util.ToTLDate(now));
                // go through them all and see if we find expected number
                foreach (string fn in allfiles)
                {
                    // get security
                    SecurityImpl sec = SecurityImpl.FromTIK(fn);
                    // see if we want this symbol
                    int idx = symbols.getindex(sec.HistSource.RealSymbol);
                    if (idx < 0)
                        idx = symbols.getindex(sec.Symbol);
                    sec.HistSource.Close();
                    // skip if we don't
                    if (idx < 0)
                        continue;
                    string sym = symbols.getlabel(idx);
                    // if we have it, count actual day
                    actualdays[idx]++;
                    // save file and symbol
                    if (!files.ContainsKey(sym))
                        files.Add(sym, new List<string>());
                    files[sym].Add(fn);
                    // count files
                    tfc++;
                }
                // add one day
                now = now.AddDays(1);
            }
            // notify
            if (deb != null)
            {
                deb("found " + tfc + " tick files matching dates: " + Util.ToTLDate(start) + "->" + Util.ToTLDate(endexclusive) + " for: " + string.Join(",", symbols.ToArray()));
            }
            // playback when actual meets expected
            bool allok = true;
            foreach (string sym in symbols)
            {
                if (skipexpected || (actualdays[sym] >= expecteddays))
                {
                    // get tick files
                    string[] tf = files[sym].ToArray();
                    // notify
                    if (deb != null)
                    {
                        deb(sym + " playing back " + tf.Length + " tick files.");
                    }
                    // playback
                    HistSim h = new SingleSimImpl(tf);
                    h.GotTick += new TickDelegate(r.GotTick);
                    h.PlayTo(MultiSimImpl.ENDSIM);
                    h.Stop();
                    // notify
                    if (deb != null)
                    {
                        deb(sym + " completed playback. ");
                    }
                }
                else
                    allok = false;
            }


            return allok;

        }

    }

    
}
