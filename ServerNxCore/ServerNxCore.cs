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
        
        static TLServer tl;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        const int ESTMAXSYMBOLS = 10000;
        static GenericTracker<bool> _syms = new GenericTracker<bool>(ESTMAXSYMBOLS);
        static GenericTracker<string> _realsymbol = new GenericTracker<string>(ESTMAXSYMBOLS);
        public const string LIVEFEED = "";

        string _fn = LIVEFEED;
        bool _islive = true;
        bool hasstate { get { return isLive && System.IO.File.Exists(statefilepath); } }
        string statefilepath { get { return Environment.CurrentDirectory+"\\nxstate."+Util.ToTLDate() + ".tmp"; } }
        public bool isLive { get { return _islive; } }
        public ServerNxCore(TLServer tls, string filename,DebugDelegate debugs)
        {
            _fn = filename;
            _islive = _fn == LIVEFEED;
            _syms.NewTxt += new TextIdxDelegate(_syms_NewTxt);
            SendDebugEvent = debugs;
            d = debugs;
            
            _proc = new System.Threading.Thread(proc);
            tl = tls;
            tl.newProviderName = Providers.Nanex;
            tl.newFeatureRequest += new MessageArrayDelegate(ServerNxCore_newFeatureRequest);
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            if (isLive)
            {
                DOLIVESKIPTEST = true;
                // if live and no previous state, remove old state files
                if (!hasstate)
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
                    System.IO.FileInfo[] fis = di.GetFiles("nxstate.*.tmp");
                    foreach (System.IO.FileInfo fi in fis)
                    {
                        try
                        {
                            System.IO.File.Delete(fi.FullName);
                        }
                        catch { }
                    }
                }
                else 
                    _fn = statefilepath;
            }
            
        }

        void _syms_NewTxt(string txt, int idx)
        {

        }

        Basket old = new BasketImpl();



        void tl_newRegisterSymbols(string client, string symbols)
        {
            D("got subscribe request: " + client+": "+symbols);


                // ensure new symbols are added
                Basket newb = tl.AllClientBasket;
                // ensure we have an id
                foreach (string sym in newb.ToSymArray())
                {
                    Security sec = SecurityImpl.Parse(sym);
                    char p;
                    if (sec.Type == SecurityType.STK)
                        p = 'e';
                    else if (sec.Type == SecurityType.BND)
                        p = 'b';
                    else if (sec.Type == SecurityType.FUT)
                        p = 'f';
                    else if (sec.Type == SecurityType.IDX)
                        p = 'i';
                    else if (sec.Type == SecurityType.OPT)
                        p = 'o';
                    else if (sec.Type == SecurityType.CASH)
                        p = 'c';
                    else if (sec.Type == SecurityType.FOP)
                        p = 'p';
                    else
                        p = 'e';
                    string nxsym = p + sec.Symbol;
                    _syms.addindex(nxsym, true);
                    _realsymbol.addindex(nxsym, sec.Symbol);
                }
                // ensure old symbols are removed
                if (old.Count > 0)
                {
                    Basket rem = BasketImpl.Subtract(old, newb);
                    foreach (Security s in rem)
                    {
                        string tsym = string.Empty;
                        try
                        {
                            tsym = s.Symbol.Substring(1, s.Symbol.Length - 1);
                            _syms[tsym] = false;
                        }
                        catch (Exception ex)
                        {
                            debug("error unsubscribing: " + s.Symbol + " with: " + tsym + " " + ex.Message + ex.StackTrace);
                        }
                    }
                }
                // save new as old
                old = newb;
            
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
        volatile bool _go = true;
        void proc()
        {

            while (!QUIT)
            {
                try
                {
                    if (!QUIT)
                    {
                        NxCore.ProcessTape(_fn,
                         null, 0, 0,
                         OnNxCoreCallback);

                    }
                    else
                        break;
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
        }
        static bool QUIT = false;
        static bool DOLIVESKIPTEST = false;
        static unsafe int OnNxCoreCallback(IntPtr pSys, IntPtr pMsg)
        {
            // Alias structure pointers to the pointers passed in.
            NxCoreSystem* pNxCoreSys = (NxCoreSystem*)pSys;
            NxCoreMessage* pNxCoreMsg = (NxCoreMessage*)pMsg;

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
            if (QUIT)
            {
                D("NxCore thread received exit signal.");
                return (int)NxCore.NxCALLBACKRETURN_STOP;
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
                case NxCore.NxCORESTATUS_LOADED_STATE:
                    D("Nxcore has been loaded from a saved state.");
                    break;
                case NxCore.NxCORESTATUS_SAVING_STATE:
                    D("Nxcore is now saving it's current state.");
                    break;
            }
        }
        static unsafe void OnNxCoreTrade(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
        {
            if (DOLIVESKIPTEST)
            {
                if (pNxCoreSys->nxTime.MsOfDay < (DateTime.UtcNow.TimeOfDay.TotalMilliseconds - (DateTime.Now.IsDaylightSavingTime() ? (1000 * 60 * 60 * 4) : (1000 * 60 * 60 * 5)))) 
                    return;
                DOLIVESKIPTEST = false;
                D("NxCore starting realtime data");
            }
            // Get the symbol for category message

            int idx = _syms.getindex(new string(&pNxCoreMsg->coreHeader.pnxStringSymbol->String));
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
            k.symbol = _realsymbol[idx];
            k.date = tldate;
            k.time = tltime;
            k.trade = (decimal)Price;
            k.ex = ex;
            k.size = size;
            try
            {
                tl.newTick(k);
            }
            catch (Exception e)
            {
                D("bad tick: " + k.symbol + " " + Price + " " + size + " " + ex+" "+e.Message+e.StackTrace);
            }
        }
        static unsafe void OnNxCoreExgQuote(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
	    {
            if (DOLIVESKIPTEST)
            {
                if (pNxCoreSys->nxTime.MsOfDay<(DateTime.UtcNow.TimeOfDay.TotalMilliseconds-(DateTime.Now.IsDaylightSavingTime() ? (1000*60*60*4) : (1000*60*60*5))))
                    return;
                DOLIVESKIPTEST = false;
                D("NxCore starting realtime data");
            }
	      // Get the symbol for category message

              int idx = _syms.getindex(new string(&pNxCoreMsg->coreHeader.pnxStringSymbol->String));
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
              k.symbol = _realsymbol[idx];
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
              try
              {
                  tl.newTick(k);
              }
              catch (Exception ex)
              {
                  D("bad tick: " + k.ToString()+" "+ex.Message+ex.StackTrace);
              }
          }
      }
        static unsafe void OnNxCoreMMQuote(NxCoreSystem* pNxCoreSys, NxCoreMessage* pNxCoreMsg)
        {

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
