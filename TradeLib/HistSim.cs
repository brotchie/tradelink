using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    public class HistSim : TradeLinkServer
    {
        int _simtime = 0;
        string _folder;
        TickFileFilter _filter = new TickFileFilter();
        bool _ticksloaded = false;
        Broker _broker = new Broker();
        string[] _tickfiles;
        public Broker SimBroker { get { return _broker; } set { _broker = value; } }
        public HistSim() : this(Util.TLTickDir, null) { }
        public HistSim(string TickFolder, TickFileFilter tff)
        {
            _folder = TickFolder;
            if (tff != null)
                _filter = tff;
        }

        public void Load()
        {
            string[] files = Directory.GetFiles(_folder);
            _tickfiles = _filter.Allows(files);

        }

        public void GoSrv() { }
        public void newTick(Tick t) { }
        public void newFill(Trade trade) { }
    }
}
