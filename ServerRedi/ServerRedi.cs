using System;
using System.Collections.Generic;
using System.Text;
using VBRediClasses;
using TradeLink.API;
using TradeLink.Common;
using System.Threading;

namespace ServerRedi
{
    public class ServerRedi : TLServer_WM
    {
        public const string PROGRAM = "RediServer";
        VBCacheClass _cc;
        VBOrderClass _oc;
        bool _conn = false;
        public bool isConnected { get { return _conn; } }
        const int MAXRECORD = 5000;
        RingBuffer<bool> _newsyms = new RingBuffer<bool>(5);
        RingBuffer<Order> _neworders = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<uint> _newcancel = new RingBuffer<uint>(MAXRECORD);
        string _newsymlist = string.Empty;
        Thread _bw;
        int _SLEEP = 50;
        bool _bwgo = true;

        public ServerRedi() : this(50) { }
        public ServerRedi(int sleepvalue)
        {
            _bw = new Thread(new ParameterizedThreadStart(doqueues));
            newProviderName = Providers.REDI;
            newRegisterStocks += new DebugDelegate(ServerRedi_newRegisterStocks);
        }

        /*
        bool isMsgTableOpen = false;
         *                             if (isMsgTableOpen)
                            {
                                _cc.VBRevokeObject(ref se);
                            }
                            _cc.VBSubmit(ref isMsgTableOpen, ref se);
         */


        void doqueues(object obj)
        {
            while (_bwgo)
            {
                bool newsym = false;
                while (!_newsyms.isEmpty)
                {
                    _newsyms.Read();
                    newsym = true;
                }
                if (newsym)
                {
                    // get symbols
                    Basket b = BasketImpl.FromString(_newsymlist);
                    object err = null;
                    foreach (Security s in b)
                    {
                        try
                        {
                            string se= string.Empty;

                            object vret = _cc.VBRediCache.Submit("L1", true, ref err);
                            checkerror(ref err,"submit");
                            _cc.VBRediCache.AddWatch(0, s.Symbol, string.Empty, ref err);
                            checkerror(ref err,"addwatch");
                        }
                        catch (Exception ex)
                        {
                            debug(s.Symbol + " error subscribing: " + ex.Message + ex.StackTrace);
                        }
                    }
                    debug("registered: " + _newsymlist);
                }

                while (!_neworders.isEmpty)
                {
                    Order o = _neworders.Read();
                }

                while (!_newcancel.isEmpty)
                {
                    uint id = _newcancel.Read();
                }

                if (_newcancel.isEmpty && _neworders.isEmpty && _newsyms.isEmpty)
                    Thread.Sleep(_SLEEP);
            }

            
        }

        // after you are done watching a symbol (eg not subscribed)
        // you should close message table for that symbol

        void ServerRedi_newRegisterStocks(string msg)
        {
            debug("Subscribe request: " + msg);
            if (!isConnected)
            {
                debug("not connected.");
                return;
            }
            // save list of symbols to subscribe
            _newsymlist = msg;
            // notify other thread to subscribe to them
            _newsyms.Write(true);

            
            
        }

        void checkerror(ref object err, string context)
        {
            if (err != null)
                debug(context+" error: "+err.ToString());
            err = null;
        }

        // respond to redi events
        void VBRediCache_CacheEvent(int action, int row)
        {
            int err = 0;
            object cv = new object();
            decimal dv = 0;
            int iv = 0;
            debug("action: " + action.ToString());
            switch (action)
            {
                case 1: //CN_SUBMIT
                    {

                        debug("row: "+row.ToString());
                        for (int i = 0; i < row; i++)
                        {
                            Tick k = new TickImpl();
                            _cc.VBGetCell(i, "SYMBOL", ref cv, ref err);
                            debug("getcellerr: "+err.ToString());
                            k.symbol = cv.ToString();
                            _cc.VBGetCell(i, "LastPrice", ref cv, ref err);
                            if (decimal.TryParse(cv.ToString(), out dv))
                                k.trade = dv;
                            _cc.VBGetCell(i, "BidPrice", ref cv, ref err);
                            if (decimal.TryParse(cv.ToString(), out dv))
                                k.bid = dv;
                            _cc.VBGetCell(i, "AskPrice", ref cv, ref err);
                            if (decimal.TryParse(cv.ToString(), out dv))
                                k.ask = dv;
                            _cc.VBGetCell(i, "LastSize", ref cv, ref err);
                            if (int.TryParse(cv.ToString(), out iv))
                                k.size = iv;
                            _cc.VBGetCell(i, "BidSize", ref cv, ref err);
                            if (int.TryParse(cv.ToString(), out iv))
                                k.bs = iv;
                            _cc.VBGetCell(i, "AskSize", ref cv, ref err);
                            if (int.TryParse(cv.ToString(), out iv))
                                k.os = iv;
                            newTick(k);
                        }
                    }
                    break;
                    /*
                case 4 : //CN_INSERT
                    break;
                case 5: // CN_UPDATE
                    break;
                case 7: // CN_REMOVING
                    break;
                case 8 : // CN_REMOVED
                    break;
                     */
            }

        }

        public event DebugDelegate SendDebug;
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public bool Start(string user,string pw)
        {
            try
            {
                debug(Util.TLSIdentity());
                debug("Attempting to start " + PROGRAM);
                _cc = new VBCacheClass();
                _oc = new VBOrderClass();
                _cc.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(VBRediCache_CacheEvent);
                _cc.VBRediCache.UserID = user;
                _cc.VBRediCache.Password = pw;
                _bw.Start();
            }
            catch (Exception ex)
            {
                debug("error starting "+PROGRAM);
                debug(ex.Message + ex.StackTrace);
                debug("Did you forget to login to Redi?");
                _conn = false;
                return false;
            }
            debug("Started successfully.");
            debug("User: " + _cc.VBRediCache.UserID);
            _conn = true;
            return true;
        }

        public override void Stop()
        {
            try
            {
                _bwgo = false;
                string b = string.Empty;
                _cc.VBRevokeObject(ref b);
            }
            catch { }
            base.Stop();
        }


    }
}
