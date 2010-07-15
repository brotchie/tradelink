using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using System.Windows.Forms;
using IESignal;
using System.ComponentModel;

namespace ServerEsignal
{
    public class EsignalServer : TLServer_WM
    {
        BackgroundWorker bw = new BackgroundWorker();
        Hooks esig;
        bool _valid = false;

        Basket _mb = new BasketImpl();
        public event DebugFullDelegate GotDebug;
        bool _go = true;

        public bool isValid { get { return _valid; } }
        public EsignalServer() :base()
        {
            // use a background thread to queue up COM-events
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            // set provider
            newProviderName = Providers.eSignal;
            // handle subscription requests
            newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            // handle feature requests
            newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            // handle unknown messages
            newUnknownRequest += new UnknownMessageDelegate(EsignalServer_newUnknownRequest);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!_valid) return;
            while (_go)
            {

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
                        if (ReleaseDeadSymbols)
                        {
                            // clone requested basket
                            Basket newbasket = new BasketImpl(syms);
                            // clone existing basket as deadbasket
                            Basket deadbasket = new BasketImpl(_mb);
                            // existing - new = deadsymbols
                            deadbasket.Remove(newbasket);
                            // release dead symbols
                            string symsreleased = string.Empty;
                            foreach (Security dead in deadbasket)
                            {
                                try
                                {
                                    esig.ReleaseSymbol(dead.Symbol);
                                    symsreleased += dead.Symbol + " ";
                                }
                                catch { }
                            }
                            if (symsreleased!=string.Empty)
                                verb("released unused symbols: " + symsreleased);

                        }
                        qr = qc;
                    }

                    while (_barrequests.hasItems)
                    {
                        BarRequest br = new BarRequest();
                        try
                        {
                            br = _barrequests.Read();
                            BarInterval bi = (BarInterval)br.Interval;
                            string interval = string.Empty;
                            int barsback = DefaultBarsBack;
                            if (bi == BarInterval.CustomTicks)
                                interval = br.CustomInterval + "T";
                            else if (bi == BarInterval.CustomTime)
                                interval = br.CustomInterval + "S";
                            else if (bi == BarInterval.CustomVol)
                                interval = br.CustomInterval + "V";
                            else
                            {
                                interval = (br.Interval / 60).ToString();
                                barsback = BarImpl.BarsBackFromDate(bi, br.StartDate, br.EndDate);
                            }

                            int hnd = esig.get_RequestHistory(br.Symbol, interval, (bi == BarInterval.Day) ? barType.btDAYS : barType.btBARS, barsback, -1, -1);
                            verb("requested bar data for " + br.Symbol + " on: " + br.Interval.ToString() + " " + br.CustomInterval.ToString() + " reqhandle: " + hnd);
                            // cache request
                            if (!_barhandle2barrequest.ContainsKey(hnd))
                                _barhandle2barrequest.Add(hnd, br);
                            else
                                verb("already had bar request: " + hnd + " " + _barhandle2barrequest[hnd].ToString());
                            if (esig.get_IsHistoryReady(hnd) != 0)
                                processhistory(hnd, br);
                        }
                        catch (Exception ex)
                        {
                            debug("error on historical bar request: " + br.ToString());
                            debug(ex.Message + ex.StackTrace);
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (GotDebug != null)
                        GotDebug(DebugImpl.Create(ex.Message + ex.StackTrace, DebugLevel.Debug));
                }
                if (e.Cancel || !_go)
                    break;
                System.Threading.Thread.Sleep(WaitBetweenEvents);
            }
        }

        public int DefaultBarsBack = 100;
        public bool VerboseDebugging = false;
        public int WaitBetweenEvents = 50;
        public bool ReleaseDeadSymbols = false;

        void verb(string msg)
        {
            if (!VerboseDebugging) return;
            debug(msg);
        }

        Dictionary<int, BarRequest> _barhandle2barrequest = new Dictionary<int, BarRequest>();
        RingBuffer<BarRequest> _barrequests = new RingBuffer<BarRequest>(500);

        long EsignalServer_newUnknownRequest(MessageTypes t, string msg)
        {
            switch (t)
            {
                case MessageTypes.BARREQUEST:
                    {
                        verb("got barrequest: " + msg);
                        try
                        {
                            BarRequest br = BarImpl.ParseBarRequest(msg);
                            _barrequests.Write(br);

                        }
                        catch (Exception ex)
                        {
                            debug("error parsing bar request: " + msg);
                            debug(ex.Message + ex.StackTrace);
                        }
                        return 0;
                    }
            }
            return (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
        }

        public void Start(string user, string password, string data1, int data2)
        {
            try
            {
                debug("attempting to start connection");
                // connect to esignal
                esig = new Hooks();
                // handle historical bars
                esig.OnBarsReceived += new _IHooksEvents_OnBarsReceivedEventHandler(esig_OnBarsReceived);
                // handle esignal quotes
                esig.OnQuoteChanged += new _IHooksEvents_OnQuoteChangedEventHandler(esig_OnQuoteChanged);
            }
            catch (Exception ex) 
            {
                const string url = @"http://code.google.com/p/tradelink/wiki/EsignalConfig";
                System.Diagnostics.Process.Start(url);
                debug("Exception loading esignal: " + ex.Message + ex.StackTrace); _valid = false;
                debug("For more info see: " + url);
                _valid = false;
                return; 
            }
            if ((user==null) || (user==string.Empty)) return;
            esig.SetApplication(user);
            _valid = esig.IsEntitled != 0;
            if (_valid)
            {
                debug("success");
                _go = true;
                // start background processing
                if (!bw.IsBusy)
                    bw.RunWorkerAsync();
            }
            else
                debug("failed.");
        }
        public bool ReleaseBarHistoryAfteRequest = true;
        void processhistory(int lHandle,BarRequest br)
        {
            int numbars = esig.get_GetNumBars(lHandle);
            if (numbars == 0)
            {
                verb("no bars available for reqhandle: " + lHandle);
                return;
            }
            numbars *= -1;
            for (int i = numbars; i<=0; i++)
            {
                try
                {
                    BarData bd = esig.get_GetBar(lHandle, i);
                    if (VerboseDebugging)
                        verb(br.Symbol + " " + bd.dtTime.ToString() + " " + bd.dOpen + " " + bd.dHigh + " " + bd.dLow + " " + bd.dClose + " " + bd.dVolume);
                    Bar b = new BarImpl((decimal)bd.dOpen, (decimal)bd.dHigh, (decimal)bd.dLow, (decimal)bd.dClose, (long)bd.dVolume, Util.ToTLDate(bd.dtTime), Util.ToTLTime(bd.dtTime), br.Symbol, br.Interval);
                    string msg = BarImpl.Serialize(b);
                    TLSend(msg, MessageTypes.BARRESPONSE, br.Client);
                }
                catch (Exception ex)
                {
                    verb("error obtaining bar: " + i + " from: " + lHandle);
                    verb(ex.Message + ex.StackTrace);
                }
            }
            if (ReleaseBarHistoryAfteRequest)
            {
                try
                {
                    esig.ReleaseHistory(lHandle);
                    _barhandle2barrequest.Remove(lHandle);
                }
                catch { }
            }

        }

        void esig_OnBarsReceived(int lHandle)
        {
            BarRequest br;
            if (!_barhandle2barrequest.TryGetValue(lHandle, out br))
            {
                verb("Unknown barrequest handle: " + lHandle);
                return;
            }
            processhistory(lHandle,br);
            
        }

        void debug(string msg)
        {
            if (GotDebug != null)
                GotDebug(DebugImpl.Create(msg));
        }

        string _tmpregister = string.Empty;



        MessageTypes[] tl_newFeatureRequest()
        {
            // features supported by connecotr
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.BARRESPONSE);
            f.Add(MessageTypes.BARREQUEST);
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
        /// <summary>
        /// stop the server
        /// </summary>
        public void Stop()
        {
            // request thread be stopped
            _go = false;
            if (!_valid) return;
            
            try
            {
                // stop thread
                bw.CancelAsync();
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
            verb("got new symbol list: " + msg);
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
