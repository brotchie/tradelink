using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    public class HistSim 
    {
        // working variables
        string _folder = Util.TLTickDir;
        TickFileFilter _filter = new TickFileFilter();
        Broker _broker = new Broker();
        string[] _tickfiles = new string[0];
        bool _inited = false;
        DateTime _nextticktime = DateTime.MinValue;
        DateTime _nextindextime = DateTime.MinValue;
        int _executions = 0;
        int _tickcount = 0;
        long _bytestoprocess = 0;
        List<Security> Instruments = new List<Security>();
        
        // events
        public event TickDelegate GotTick;
        public event DebugDelegate GotDebug;
        
        // user-facing interfaces
        public TickFileFilter FileFilter { get { return _filter; } set { _filter = value; D("Restarting simulator with " + _filter.ToString()); Reset(); Initialize(); } }
        /// <summary>
        /// Total ticks available for processing, based on provided filter or tick files.
        /// </summary>
        public int ApproxTotalTicks { get { return (int)Math.Floor((double)_bytestoprocess/39); } }
        /// <summary>
        /// Ticks processed in this simulation run.
        /// </summary>
        public int TickCount { get { return _tickcount; } }
        public int CountedTicks { get { return _tickcount; } }
        /// <summary>
        /// Fills executed during this simulation run.
        /// </summary>
        public int SimBrokerFillCount { get { return _executions; } }
        /// <summary>
        /// Gets next tick in the simulation
        /// </summary>
        public DateTime NextTickTime { get { return _nextticktime; } }
        /// <summary>
        /// Gets broker used in the simulation
        /// </summary>
        public Broker SimBroker { get { return _broker; }  }
        
        /// <summary>
        /// Create a historical simulator using default tick folder and null filter
        /// </summary>
        public HistSim() : this(Util.TLTickDir, null) { }
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
        }
        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="filename">Single file to use</param>
        public HistSim(string filename)
        {
            _tickfiles = new string[] { filename };
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
            cachedsymbols.Clear();
            tickcache.Clear();
            _broker.Reset();
            _executions = 0;
            _tickcount = 0;
            _bytestoprocess = 0;
            _nextticktime = ENDSIM;
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
            D("got tickfiles: "+string.Join(",",_tickfiles));

            // now we have our list, initialize instruments from files
            foreach (string file in _tickfiles)
            {
                Instruments.Add(Security.FromFile(file));
            }

            D("Initialized " + (_tickfiles.Length ) + " instruments.");
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
            D("Approximately " + ApproxTotalTicks + " ticks to process...");
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
                StockPlayTo(time); // then do stocks
            }
            else throw new Exception("Histsim was unable to initialize");
        }

        List<Tick> tickcache = new List<Tick>();
        List<string> cachedsymbols = new List<string>();

        private void StockPlayTo(DateTime time)
        {
            while (FlushTickCache(time)) // continue flushing cache until nothing left to flush
                FillCache(); // repopulate cache (ignored symbols already cached)
        }

        void FillCache()
        {  
            // if a tick is in the cache it's because it's too new (in future)
            // so we only need to fetch ticks for uncached symbols
            foreach (Security i in Instruments)
            {
                if (cachedsymbols.Contains(i.Name)) continue;

                // if it's not already cached we need the next tick/index:
                switch (i.Type)
                {
                    case SecurityType.STK:
                        Security s = (Security)i;
                        Tick next = s.NextTick;
                        if (next.isValid)
                        {
                            tickcache.Add(next); // add next tick to cache
                            _tickcount++;
                            cachedsymbols.Add(i.Name); // update index of cached symbols
                        }
                        break;
                }
            }
            updatenextticktime();
        }

        bool FlushTickCache(DateTime time)
        {
            bool didsomething = false;
            List<int> remove = new List<int>();
            for (int i = 0; i < tickcache.Count; i++)
                if (time >= Util.ToDateTime(tickcache[i].date, tickcache[i].time, tickcache[i].sec))
                {
                    if (SimBroker != null)
                        _executions += SimBroker.Execute(tickcache[i]); // use tick to execute any pending orders
                    if (GotTick != null)
                        GotTick(tickcache[i]); // send cached tick as event
                    cachedsymbols.Remove(tickcache[i].symbol); // update symbol cache
                    remove.Add(i);// remove item from cache
                    didsomething = true;
                }
            for (int i = remove.Count - 1; i >= 0; i--) // process removals
                tickcache.RemoveAt(remove[i]);
            return didsomething;
        }

        public static DateTime ENDSIM = DateTime.MaxValue;


        void updatenextticktime()
        {
            DateTime recent = ENDSIM;
            for (int i = 0; i < tickcache.Count; i++)
            {
                DateTime comparetime = Util.ToDateTime(tickcache[i].date, tickcache[i].time, tickcache[i].sec);
                recent = (comparetime<=recent) ? comparetime : recent;
            }
            _nextticktime = recent;
        }

    }
}
