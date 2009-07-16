using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using System.Windows.Forms;
using IESignal;
namespace ServerEsignal
{
    public class EsignalServer : TLServer_WM
    {
        Timer tt = new Timer();
        Hooks esig = new Hooks();
        bool _valid = false;

        Basket _mb = new BasketImpl();
        public event DebugFullDelegate GotDebug;

        public bool isValid { get { return _valid; } }
        public EsignalServer() :base()
        {
            // use a timer to queue up COM-events
            tt.Interval = 1000;
            tt.Enabled = true;
            tt.Tick += new EventHandler(tt_Tick);
            // set provider
            newProviderName = Providers.eSignal;
            // handle subscription requests
            newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            // handle feature requests
            newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            // handle esignal quotes
            esig.OnQuoteChanged += new _IHooksEvents_OnQuoteChangedEventHandler(esig_OnQuoteChanged);

        }

        public void Start(string user, string password, string data1, int data2)
        {
            if ((user==null) || (user==string.Empty)) return;
            esig.SetApplication(user);
            _valid = esig.IsEntitled != 0;
        }

        string _tmpregister = string.Empty;

        void tt_Tick(object sender, EventArgs e)
        {
            if (!_valid) return;

            try
            {
                if (qc > qr)
                {
                    // get requested symbols
                    string[] syms = _tmpregister.Split(',');
                    // go through each one
                    foreach (string sym in syms)
                    {
                        // if we don't have subscription already
                        if (!contains(sym))
                        {
                            // add it to list
                            _mb.Add(sym);
                            // request subscription
                            esig.RequestSymbol(sym, 1);
                        }
                    }
                    qr = qc;
                }
            }
            catch (Exception ex)
            {
                if (GotDebug != null)
                    GotDebug(DebugImpl.Create(ex.Message + ex.StackTrace, DebugLevel.Debug));
            }
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            // features supported by connecotr
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.TICKNOTIFY);
            return f.ToArray();
        }

        void esig_OnQuoteChanged(string sSymbol)
        {
            try
            {
                // get tick info
                BasicQuote q = esig.get_GetBasicQuote(sSymbol);
                // get our struct
                Tick k = new TickImpl(sSymbol);
                // convert it
                k.ask = (decimal)q.dAsk;
                k.bid = (decimal)q.dBid;
                k.trade = (decimal)q.dLast;
                k.bs = q.lBidSize;
                k.os = q.lAskSize;
                k.size = q.lLastSize;
                // send it
                newTick(k);
            }
            catch (Exception ex)
            {
                if (GotDebug != null)
                    GotDebug(DebugImpl.Create(ex.Message + ex.StackTrace, DebugLevel.Debug));

            }
        }

        public void Stop()
        {
            if (!_valid) return;
            try
            {
                // release symbols
                foreach (Security sec in _mb)
                    esig.ReleaseSymbol(sec.Symbol);
            }
            catch (Exception ex)
            {
                if (GotDebug != null)
                    GotDebug(DebugImpl.Create(ex.Message + ex.StackTrace, DebugLevel.Debug));

            }
            // garbage collect esignal object
            esig = null;
        }


        int qc, qr;

        void tl_newRegisterStocks(string msg)
        {
            _tmpregister = msg;
            qc++;
        }

        // see if we already subscribed to this guy
        bool contains(string sym)
        {
            foreach (Security sec in _mb)
                if (sec.Symbol == sym) return true;
            return false;
        }

    }
}
