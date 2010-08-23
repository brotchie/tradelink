using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;
using NxCoreAPI;
namespace ServerNxCore
{
    public class ServerNxCore 
    {
        static GenericTracker<bool> _ssym = new GenericTracker<bool>();
        static TradeLinkServer tl;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        GenericTracker<bool> _syms = new GenericTracker<bool>(2000);
        public const string LIVEFEED = "";

        string _fn = LIVEFEED;
        public bool isLive { get { return _fn == LIVEFEED; } }
        public ServerNxCore(TradeLinkServer tls, string filename,DebugDelegate debugs)
        {
            SendDebugEvent = debugs;
            d = debugs;
            _fn = filename;
            _proc = new System.Threading.Thread(proc);
            tl = tls;
            tl.newProviderName = Providers.Nanex;
            tl.newFeatureRequest += new MessageArrayDelegate(ServerNxCore_newFeatureRequest);
            tl.newRegisterStocks += new DebugDelegate(ServerNxCore_newRegisterStocks);
        }
        System.Threading.Thread _proc;


        public void Start()
        {
            try
            {
                if (!isLive && !System.IO.File.Exists(_fn))
                {
                    debug("can't start playback, file not found: " + _fn);
                    return;
                }
                _proc.Start();
            }
            catch (Exception ex)
            {
                debug("error starting server: " + ex.Message + ex.StackTrace);
                return;
            }
            debug(ServerNxCoreMain.PROGRAM + " started ok.");
        }
        bool _go = true;
        void proc()
        {
            while (_go)
            {
                try
                {
                    NxCore.ProcessTape(_fn,
                     null, 0, 0,
                     OnNxCoreCallback);
                }
                catch (Exception ex)
                {
                    debug(ex.Message + ex.StackTrace);
                }
            }
        }
        public void Stop()
        {
            QUIT = true;
            _go = false;
            try
            {
                _proc.Abort();
            }
            catch { }
        }
        static bool QUIT = false;
        static unsafe int OnNxCoreCallback(IntPtr pSys, IntPtr pMsg)
        {
            // Alias structure pointers to the pointers passed in.
            NxCoreSystem* pNxCoreSys = (NxCoreSystem*)pSys;
            NxCoreMessage* pNxCoreMsg = (NxCoreMessage*)pMsg;
            if (QUIT)
                return (int)NxCore.NxCALLBACKRETURN_STOP;
            // Do something based on the message type
            switch (pNxCoreMsg->MessageType)
            {
                // NxCore Status Message
                case NxCore.NxMSG_STATUS:
                    OnNxCoreStatus(pNxCoreSys, pNxCoreMsg);
                    break;
                // NxCore Trade Message
                case NxCore.NxMSG_TRADE:
                    OnNxCoreTrade(pNxCoreSys, pNxCoreMsg);
                    break;
                // NxCore Level1 Quote Message
                case NxCore.NxMSG_EXGQUOTE:
                    OnNxCoreExgQuote(pNxCoreSys, pNxCoreMsg);
                    break;
                // NxCore Level2 Quote Message
                case NxCore.NxMSG_MMQUOTE:
                    //OnNxCoreMMQuote(pNxCoreSys, pNxCoreMsg);
                    break;
            }
            // Continue running the tape
            return (int)NxCore.NxCALLBACKRETURN_CONTINUE;
        }
        static DebugDelegate d;
        static void D(string msg)
        {
            if (d!=null)
                d(msg);
        }
        static unsafe void OnNxCoreStatus(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
        {
            // Print the specific NxCore status message
            switch (pNxCoreSys->Status)
            {
                case NxCore.NxCORESTATUS_COMPLETE:
                    D("NxCore Complete Message.");
                    break;
                case NxCore.NxCORESTATUS_INITIALIZING:
                    D("NxCore Initialize Message.");
                    break;
                case NxCore.NxCORESTATUS_SYNCHRONIZING:
                    D("NxCore Synchronizing Message.");
                    break;
                case NxCore.NxCORESTATUS_WAITFORCOREACCESS:
                    {
                        //D("NxCore Wait For Access.");
                        break;
                    }
                case NxCore.NxCORESTATUS_RESTARTING_TAPE:
                    D("NxCore Restart Tape Message.");
                    break;
                case NxCore.NxCORESTATUS_ERROR:
                    D("NxCore Error.");
                    break;
                case NxCore.NxCORESTATUS_RUNNING:
                    break;
            }
        }
        static unsafe void OnNxCoreTrade(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
        {
            // Get the symbol for category message
            String Symbol = new String(&pNxCoreMsg->coreHeader.pnxStringSymbol->String);
            Symbol = Symbol.Remove(0, 1);
            int idx = _ssym.getindex(Symbol);
            if (idx < 0) return;
            // Assign a pointer to the Trade data
            NxCoreTrade* Trade = &pNxCoreMsg->coreData.Trade;
            // Get the price and net change
            double Price = NxCore.PriceToDouble(Trade->Price, Trade->PriceType);
            //double NetChange = NxCore.PriceToDouble(Trade->NetChange, Trade->PriceType);
            NxTime time = pNxCoreMsg->coreHeader.nxExgTimestamp;
            int tltime = time.Hour * 10000 + time.Minute * 100 + time.Second;
            NxDate date = pNxCoreMsg->coreHeader.nxSessionDate;
            int tldate = (int)date.Year * 10000 + (int)date.Month * 100 + (int)date.Day;
            string ex = excode2name(pNxCoreMsg->coreHeader.ReportingExg);
            int size = (int)Trade->Size;
            // check for index
            if (size <= 0) return;
            Tick k = new TickImpl();
            k.symbol = Symbol;
            k.date = tldate;
            k.time = tltime;
            k.trade = (decimal)Price;
            k.ex = ex;
            k.size = size;
            tl.newTick(k);
        }
        static unsafe void OnNxCoreExgQuote(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
	    {
	      // Get the symbol for category message
	      String Symbol = new String(&pNxCoreMsg->coreHeader.pnxStringSymbol->String);
          Symbol = Symbol.Remove(0, 1);
              int idx = _ssym.getindex(Symbol);
              if (idx < 0) return;
          
	      // Assign a pointer to the ExgQuote data
	      NxCoreExgQuote* Quote = &pNxCoreMsg->coreData.ExgQuote;
            NxCoreQuote cq = Quote->coreQuote;
	      // Get bid and ask price
            double bid = 0;
            double ask = 0;
            int bs = 0;
            int os = 0;
            string be = string.Empty;
            string oe = string.Empty;
            bool bbid = false;
            bool bask = false;
          if ((cq.BidPriceChange != 0) || (cq.BidSizeChange != 0))
          {
              bid = NxCore.PriceToDouble(Quote->coreQuote.BidPrice, Quote->coreQuote.PriceType);
              bs = Quote->coreQuote.BidSize;
              be = excode2name(Quote->BestBidExg);
              bbid = true;
          }
          if ((cq.AskPriceChange != 0) || (cq.AskSizeChange != 0))
          {
              ask = NxCore.PriceToDouble(Quote->coreQuote.AskPrice, Quote->coreQuote.PriceType);
              os = Quote->coreQuote.AskSize;
              oe = excode2name(Quote->BestAskExg);
              bask = true;
          }
          if (bask || bbid)
          {
              NxTime time = pNxCoreMsg->coreHeader.nxExgTimestamp;
              int tltime = time.Hour * 10000 + time.Minute * 100 + time.Second;
              NxDate date = pNxCoreMsg->coreHeader.nxSessionDate;
              int tldate = (int)date.Year * 10000 + (int)date.Month * 100 + (int)date.Day;
              Tick k = new TickImpl();
              k.symbol = Symbol;
              k.date = tldate;
              k.time = tltime;
              if (bask && bbid)
              {
                  k.bid = (decimal)bid;
                  k.bs = bs;
                  k.be = be;
                  k.ask = (decimal)ask;
                  k.os = os;
                  k.oe = oe;
              }
              else if (bbid)
              {
                  k.bid = (decimal)bid;
                  k.bs = bs;
                  k.be = be;
              }
              else
              {
                  k.ask = (decimal)ask;
                  k.os = os;
                  k.oe = oe;
              }
              tl.newTick(k);
          }
      }
        static unsafe void OnNxCoreMMQuote(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
        {
            // Get the symbol for category message
            String Symbol = new String(&pNxCoreMsg->coreHeader.pnxStringSymbol->String);
            // Assign a pointer to the MMQuote data
            NxCoreMMQuote* Quote = &pNxCoreMsg->coreData.MMQuote;
            if ((IntPtr)Quote->pnxStringMarketMaker == IntPtr.Zero) return;
            //String MarketMaker = new String(&Quote->pnxStringMarketMaker->String);
            // Get bid and ask price
            //double Bid = NxCore.PriceToDouble(Quote->coreQuote.BidPrice, Quote->coreQuote.PriceType);
            //double Ask = NxCore.PriceToDouble(Quote->coreQuote.AskPrice, Quote->coreQuote.PriceType);
            
            /*D(string.Format("MMQuote for Symbol: {0:S}, MarketMaker: {1:S}  Time: {2:d}:{3:d}:{4:d}  Bid: {5:f}  Ask: {6:f}  BidSize: {7:d}  AskSise: {8:d}  Exchg: {9:d} ",
                              Symbol, MarketMaker,
                              pNxCoreMsg->coreHeader.nxExgTimestamp.Hour, pNxCoreMsg->coreHeader.nxExgTimestamp.Minute, pNxCoreMsg->coreHeader.nxExgTimestamp.Second,
                              Bid, Ask, Quote->coreQuote.BidSize, Quote->coreQuote.AskSize,
                              pNxCoreMsg->coreHeader.ReportingExg));*/
        }
        static string excode2name(uint code)
        {
            try
            {
                if (code == 0) return string.Empty;
                return ((nxST_EXCHANGE)code).ToString();
            }
            catch { }
            return string.Empty;
        }
        static string excode2name(ushort code)
        {
            try
            {
                if (code == 0) return string.Empty;
                return ((nxST_EXCHANGE)code).ToString();
            }
            catch { }
            return string.Empty;
        }
        void ServerNxCore_newRegisterStocks(string msg)
        {
            D("got subscribe request: " + msg);
            // get new basket
            Basket b = BasketImpl.FromString(msg);
            _syms = new GenericTracker<bool>(2000);
            // ensure we have an id
            foreach (string sym in b.ToSymArray())
                _syms.addindex(sym, true);
            // save it
            _ssym = _syms;
        }
        MessageTypes[] ServerNxCore_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            return f.ToArray();
        }
    }
}
