using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;

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
        int _nextticktime = DT2FT(ENDSIM);
        int _executions = 0;
        volatile int _tickcount;
        long _bytestoprocess = 0;
        List<SecurityImpl> Instruments = new List<SecurityImpl>();
        
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
        public DateTime NextTickTime { get { return FT2DT(_nextticktime); } }
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
            Instruments.Clear();
            _tickcache = new TickImpl[MAXTICKS];
            _timecache = new int[MAXTICKS];
            _readcount = new uint[Instruments.Count];
            _writecount = new uint[Instruments.Count];
            _endstream = new bool[Instruments.Count];
            _master = new cursor();
            _broker.Reset();
            _executions = 0;
            _bytestoprocess = 0;

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
                Instruments.Add(SecurityImpl.FromFile(file));
            }

            // setup per-instrument read and write cursors
            _readcount = new uint[Instruments.Count];
            _writecount = new uint[Instruments.Count];
            _endstream = new bool[Instruments.Count];

            D("Initialized " + (_tickfiles.Length ) + " instruments.");
            D(string.Join(Environment.NewLine.ToString(), _tickfiles));
            FillCache();
            D("Read initial ticks into cache...");

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
        }
        /// <summary>
        /// Run simulation to specific time
        /// </summary>
        /// <param name="time">Simulation will run until this time (use HistSim.ENDSIM for last time)</param>
        public void PlayTo(DateTime time)
        {
            if (!_inited)
                Initialize();
            if (_inited)
            {
                int ftime = DT2FT(time);
                SecurityPlayTo(ftime); // then do stocks
            }
            else throw new Exception("Histsim was unable to initialize");
        }

        static int DT2FT(DateTime d) { return TL2FT(d.Hour,d.Minute,d.Second); }
        static int TL2FT(int hour, int min, int sec) { return hour * 10000 + min * 100 + sec; }
        static int TL2FT(Tick t) { return t.time * 100 + t.sec; }
        static DateTime FT2DT(int ftime)
        {
            int s = ftime % 100;
            int m = ((ftime - s)/100) % 100;
            int h = ((ftime - m*100 - s)/100) % 100;
            return new DateTime(1, 1, 1, h, m, s);
        }

        const int TICKCACHEPERINSTRUMENT = 100;
        const uint MAXTICKS = 10000;
        TickImpl[] _tickcache = new TickImpl[MAXTICKS];
        int[] _timecache = new int[MAXTICKS];
        uint[] _symcache = new uint[MAXTICKS];
        cursor _master = new cursor();
        uint[] _writecount;
        uint[] _readcount;
        bool[] _endstream;
        volatile bool haveticks = true;

        private void SecurityPlayTo(int ftime)
        {
            // continue flushing cache until nothing left to flush
            while (FlushCache(ftime) && haveticks) 
                FillCache(); // repopulate cache
        }


        bool needcache(int symidx) { return (!_endstream[symidx]) && (_readcount[symidx] == _writecount[symidx]); }

        int[] needcachefill()
        {
            // we reverse count to allow us to use CopyTo later
            int nocachecount = 0;
            // store instruments w/no ticks cached
            int[] uncached = new int[Instruments.Count];
            // check every instrument to see if it has a cache
            for (int i = 0; i < Instruments.Count; i++)
                if (needcache(i)) // if it has no cached ticks
                    uncached[nocachecount++] = i; // mark it for 'filling'
            // now we know how big our result is, size it
            int[] ret = new int[nocachecount];
            // copy only the instruments that need more ticks
            Array.ConstrainedCopy(uncached,0,ret,0,nocachecount);
            // return only instruments needing more ticks
            return ret;
        }


        void FillCache()
        {
            int loc;
            // get list of instruments with no cache
            int[] list = needcachefill();
            // make sure we have something to fill
            haveticks = list.Length > 0;
            // for every one of said instruments:
            for (uint i = 0; i < list.Length; i++)
            {
                int ci = list[i];
                TickImpl k;
                try
                {
                    // get tick
                    k = Instruments[ci].NextTick();
                }
                catch (EndSecurityTicks) { _endstream[ci] = true; continue; }
                // get time
                int ftime = TL2FT(k);
                // get cache location
                lock (_master)
                    loc = (int)(_master.write % MAXTICKS);
                // cache tick
                _tickcache[loc] = k;
                // cache time
                _timecache[loc] = ftime;
                // cache symbol's index
                _symcache[loc] = (uint)ci;
                // prepare for next write
                _master.write++;
                // make note of write on per-symbol counter
                _writecount[ci]++;
            }

        }

        bool FlushCache(int endsim)
        {
            // we stop sim when simtime is exceeded
            bool continuesim = true;
            // get size of unordered, unread cache
            int start, end, size;
            bool flipped = false;
            lock (_master)
            {
                // start/end is remainder of ticks cached less size of array
                size = _master.write - _master.read;
                start = (int)(_master.read % MAXTICKS);
                end = (int)(_master.write % MAXTICKS);
                flipped = end < start;
            }
            // make a point-in-time copy of cache from start to end
            int[] times = new int[size];
            // if we've overrun buffer, we copy in two steps
            if (flipped)
            {
                int preflipsize = _timecache.Length - start;
                Array.ConstrainedCopy(_timecache, start, times, 0, preflipsize);
                Array.ConstrainedCopy(_timecache, 0, times, preflipsize, size - preflipsize);
            }
            else // otherwise we can copy in single step
                Array.ConstrainedCopy(_timecache, start, times, 0, size);
            // assign every ticktime an index value, so when we 
            // re-order the times we can still get matching tick
            int[] idx = genidx(size);
            // sort it by most recent time first
            Array.Sort(times, idx);
            // play every cached tick until end is reached
            for (uint i = 0; i < times.Length; i++)
            {
                // update current time
                _nextticktime = times[i];
                // see if we've exceeded user's desire
                continuesim = _nextticktime < endsim;
                if (!continuesim) break;
                // get original location of tick using index
                int idxval = idx[i] + start;
                // readjust the location if we have overrun situation
                int loc = idxval>end ? (int)(MAXTICKS-idxval) : idxval;
                // get the tick
                TickImpl k = _tickcache[loc];
                // count tick as read (global)
                _master.read++;
                // count tick as read (per-instrument)
                _readcount[_symcache[loc]]++;
                // invalidate cache entry
                _tickcache[loc] = null;
                _timecache[loc] = 0;
                // skip invalid ticks
                if (k == null) continue;
                // fill tick against pending orders
                _executions += SimBroker.Execute(k);
                // notify listeners
                GotTick(k);
                // count total tick
                _tickcount++;
            }
            return continuesim;
        }
        int[] genidx(int length) { int[] idx = new int[length]; for (int i = 0; i < length; i++) idx[i] = i; return idx; }
        public static DateTime ENDSIM = DateTime.MaxValue;
        public static DateTime STARTSIM = DateTime.MinValue;

    }

    class cursor
    {
        public int read;
        public int write;
    }
}
