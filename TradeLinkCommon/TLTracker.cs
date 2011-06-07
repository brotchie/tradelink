using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// keep track of whether data is arriving for a given client/provider
    /// </summary>
    public class TLTracker
    {
        /// <summary>
        /// default milliseconds between polls (check interval)
        /// </summary>
        public const int DEFAULTPOLLMS = 1000;
        /// <summary>
        /// default seconds for timeout of a given symbol
        /// </summary>
        public const int DEFAULTTIMEOUTSEC = 60;
        TickWatcher _tw;
        TLClient _tl = null;
        Providers _broker = Providers.Unknown;
        bool _connectany = false;
        /// <summary>
        /// called when connection is made or refreshed
        /// </summary>
        public event VoidDelegate GotConnect;
        /// <summary>
        /// called when no ticks have been received since timeout
        /// </summary>
        public event VoidDelegate GotConnectFail;
        /// <summary>
        /// contains information useful for debugging
        /// </summary>
        public event DebugDelegate GotDebug;
        /// <summary>
        /// create a tracker for selected provider on client
        /// </summary>
        /// <param name="client"></param>
        public TLTracker(TLClient client) : this(DEFAULTPOLLMS,DEFAULTTIMEOUTSEC,client, Providers.Unknown, true) { }
        /// <summary>
        /// create a tracker for a preferred provider on client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="PreferredBroker"></param>
        public TLTracker(TLClient client, Providers PreferredBroker) : this(DEFAULTPOLLMS,DEFAULTTIMEOUTSEC,client, PreferredBroker, false) { }
        /// <summary>
        /// create a tracker for preferred provider with custom tick timeout and poll frequency settings
        /// </summary>
        /// <param name="pollintervalms"></param>
        /// <param name="timeoutintervalsec"></param>
        /// <param name="client"></param>
        /// <param name="PreferredBroker"></param>
        /// <param name="connectany"></param>
        public TLTracker(int pollintervalms, int timeoutintervalsec, TLClient client, Providers PreferredBroker, bool connectany)
        {
            _tl = client;
            _tw = new TickWatcher(pollintervalms);
            _tw.AlertThreshold = timeoutintervalsec;
            _connected = (client.ProvidersAvailable.Length > 0) && (client.ProviderSelected >= 0);
            if (!_connected && (GotConnectFail!= null))
                GotConnectFail();
            if (_connected && (GotConnect != null))
                GotConnect();
            // handle situations when no ticks arrive
            _tw.GotMassAlert += new Int32Delegate(_tw_GotMassAlert);
            _broker = PreferredBroker;
            _connectany = connectany;
        }

        public TickWatcher tw { get { return _tw; } set { _tw = value; } }
        /// <summary>
        /// # of seconds a symbol (or all symbols for MassAlert) has to stop ticking before alerts are sent
        /// </summary>
        public int AlertThreshold { get { return _tw.AlertThreshold; } set { _tw.AlertThreshold = value; } }
        /// <summary>
        /// # of MILLIseconds to wait between MassAlert tests
        /// </summary>
        public int PollInterval { get { return _tw.BackgroundPollInterval; } set { _tw.BackgroundPollInterval = value; } }

        int provindex()
        {
            for (int i = 0; i < _tl.ProvidersAvailable.Length; i++)
                if (_tl.ProvidersAvailable[i] == _broker)
                    return i;
            if (_connectany && (_tl.ProvidersAvailable.Length > 0))
                return 0;
            return -1;
        }
        /// <summary>
        /// provider present connected?
        /// </summary>
        public bool isConnected { get { return _connected; } }
        bool _connected = false;
        int _lastalert = 0;
        void _tw_GotMassAlert(int number)
        {
            _lastalert = number;
            // first disconnect
            if (_connected)
            {
                if (GotDebug != null)
                    GotDebug("No ticks from broker since: " + _lastalert +". Will attempt refresh.");
                if (GotConnectFail != null)
                    GotConnectFail();
            }
            _connected = false;
            if (_tl == null) return;
            // attempt to reconnect
            Reconnect();

        }
        bool _wait4firsttickreconnect = false;
        /// <summary>
        /// wait for first tick before reconnecting
        /// </summary>
        public bool ReconnectOnlyAfterFirstTick { get { return _wait4firsttickreconnect; } set { _wait4firsttickreconnect = value; } }
        /// <summary>
        /// reconnect/refresh now
        /// </summary>
        public void Reconnect()
        {

            bool success = _tl.Mode(provindex(), false);
            // change in connect status (eg fail->success)
            if (success)
            {
                if (GotDebug != null)
                    GotDebug("Broker " + _tl.ProvidersAvailable[_tl.ProviderSelected].ToString() + " connection refresh at " + _lastalert);
                if (GotConnect != null)
                    GotConnect();
            }
            _connected = success;
        }

        /// <summary>
        /// start tracker
        /// </summary>
        public void Start()
        {
        }
        /// <summary>
        /// stop tracker
        /// </summary>
        public void Stop()
        {
            try
            {
                _tw.Stop();
            }
            catch { }
        }

        /// <summary>
        /// call this function from GotTick
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            _tw.newTick(k);
        }
    }

}
