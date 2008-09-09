using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    public class HistSim 
    {
        // working variables
        string _folder;
        TickFileFilter _filter = new TickFileFilter();
        Broker _broker = new Broker();
        string[] _indexfiles;
        string[] _tickfiles;
        bool _inited = false;
        DateTime _nextticktime = DateTime.MinValue;
        DateTime _nextindextime = DateTime.MinValue;
        int _executions = 0;
        int _tickcount = 0;
        int _indexcount = 0;
        long _bytestoprocess = 0;
        List<Security> Instruments = new List<Security>();
        
        // events
        public event TickDelegate GotTick;
        public event DebugDelegate GotDebug;
        public event IndexDelegate GotIndex;
        
        // user-facing interfaces
        public TickFileFilter FileFilter { get { return _filter; } set { _filter = value; D("Restarting simulator with " + _filter.ToString()); Reset(); Initialize(); } }
        public int ApproxTotalTicks { get { return (int)Math.Floor((double)_bytestoprocess/39); } }
        public int TickCount { get { return _tickcount; } }
        public int IndexCount { get { return _indexcount; } }
        public int CountedTicks { get { return _tickcount + _indexcount; } }
        public int SimBrokerFillCount { get { return _executions; } }
        public DateTime NextIndexTime { get { return _nextindextime; } }
        public DateTime NextTickTime { get { return _nextticktime; } }
        public Broker SimBroker { get { return _broker; } set { _broker = value; } }
        public HistSim() : this(Util.TLTickDir, null) { }
        public HistSim(TickFileFilter tff) : this(Util.TLTickDir, tff) { }
        public HistSim(string TickFolder, TickFileFilter tff)
        {
            _folder = TickFolder;
            if (tff != null)
                _filter = tff;
        }
        private void D(string message)
        {
            if (GotDebug!=null) GotDebug(message);
        }

        public void Reset()
        {

            _inited = false;
            _indexfiles = null;
            _tickfiles = null;
            Instruments.Clear();
            cachedsymbols.Clear();
            indexcache.Clear();
            tickcache.Clear();
            _broker.Reset();
            _executions = 0;
            _tickcount = 0;
            _indexcount = 0;
            _bytestoprocess = 0;
            _nextindextime = ENDSIM;
            _nextticktime = ENDSIM;
        }

        const string tickext = "*.EPF";
        const string idxext = "*.IDX";

        public void Initialize()
        {
            if (_inited) return; // only init once
            // get our listings of historical files (idx and epf)
            string[] files = Directory.GetFiles(_folder,tickext);
            _tickfiles = _filter.Allows(files);
            D("got tickfiles: "+string.Join(",",_tickfiles));
            files = Directory.GetFiles(_folder,idxext);
            _indexfiles = _filter.Allows(files);
            D("got indexfiles: "+string.Join(",",_indexfiles));
            // now we have our list, initialize instruments from files
            foreach (string file in _tickfiles)
            {
                Instruments.Add(Stock.FromFile(file));
            }
            foreach (string file in _indexfiles)
                Instruments.Add(Index.FromFile(file));
            D("Initialized " + (_tickfiles.Length + _indexfiles.Length) + " instruments.");
            FillCache();
            D("Read initial ticks into cache...");

            // get total bytes represented by files
            
            DirectoryInfo di = new DirectoryInfo(_folder);
            FileInfo[] fi = di.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo thisfi in fi)
            {
                foreach (string file in _tickfiles)
                    if (thisfi.FullName==file)
                        _bytestoprocess += thisfi.Length;
                foreach (string file in _indexfiles)
                    if (thisfi.FullName == file)
                        _bytestoprocess += thisfi.Length;
            }
            D("Approximately " + ApproxTotalTicks + " ticks to process...");
            _inited = true;
        }

        public void PlayTo(DateTime time)
        {
            if (!_inited)
                Initialize();
            if (_inited)
            {
                IndexPlayTo(time); // do indicies first
                StockPlayTo(time); // then do stocks
            }
            else throw new Exception("Histsim was unable to initialize");
        }

        List<Tick> tickcache = new List<Tick>();
        List<Index> indexcache = new List<Index>();
        List<string> cachedsymbols = new List<string>();

        private void StockPlayTo(DateTime time)
        {
            while (FlushTickCache(time)) // continue flushing cache until nothing left to flush
                FillCache(); // repopulate cache (ignored symbols already cached)
        }

        private void IndexPlayTo(DateTime time)
        {
            while (FlushIndexCache(time))// continue flushing cache until nothing left to flush
                FillCache(); // repopulate cache (ignore symbols already cached)
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
                        Stock s = (Stock)i;
                        Tick next = s.NextTick;
                        if (next.isValid)
                        {
                            tickcache.Add(next); // add next tick to cache
                            _tickcount++;
                            cachedsymbols.Add(i.Name); // update index of cached symbols
                        }
                        break;
                    case SecurityType.IDX:
                        Index x = (Index)i;
                        Index nexti = x.NextTick;
                        if (nexti.isValid)
                        {
                            indexcache.Add(nexti); // add next to index cache
                            _indexcount++;
                            cachedsymbols.Add(i.Name);
                        }
                        break;
                }
            }
            updatenextindextime();
            updatenextticktime();
        }

        bool FlushIndexCache(DateTime time)
        {
            bool didsomething = false;
            for (int i = 0; i < indexcache.Count; i++)
                if (time >= Util.ToDateTime(indexcache[i].Date, indexcache[i].Time, 0)) // indicies don't have seconds
                {
                    if (GotIndex != null)
                        GotIndex(indexcache[i]); // send cached item as event
                    cachedsymbols.Remove(indexcache[i].Name); // update symbol cache
                    indexcache.RemoveAt(i);// remove item from cache
                    didsomething = true;
                }
            return didsomething;
        }

        bool FlushTickCache(DateTime time)
        {
            bool didsomething = false;
            for (int i = 0; i < tickcache.Count; i++)
                if (time >= Util.ToDateTime(tickcache[i].date, tickcache[i].time, tickcache[i].sec))
                {
                    if (SimBroker != null)
                        _executions += SimBroker.Execute(tickcache[i]); // use tick to execute any pending orders
                    if (GotTick != null)
                        GotTick(tickcache[i]); // send cached tick as event
                    cachedsymbols.Remove(tickcache[i].sym); // update symbol cache
                    tickcache.RemoveAt(i); // remove item from cache
                    didsomething = true;
                }
            return didsomething;
        }

        public static DateTime ENDSIM = DateTime.MaxValue;

        void updatenextindextime()
        {
            DateTime recent = ENDSIM;
            for (int i = 0; i < indexcache.Count; i++)
            {
                DateTime comparetime = Util.ToDateTime(indexcache[i].Date, indexcache[i].Time, 0);
                recent = (recent <= comparetime) ? comparetime : recent;
            }
            _nextindextime = recent;
        }

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
