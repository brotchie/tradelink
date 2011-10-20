using System;
using System.Collections.Generic;
using com.omnesys.omne.om;
using com.omnesys.rapi;
using TradeLink.API;
using TradeLink.Common;
using System.Text;

namespace ServerRithmic
{
    public class ServerRithmic : RCallbacks
    {
        TLServer tl;
        DebugDelegate debs;

        REngine oEngine;
        REngineParams oParams = new REngineParams();
        admcallback oAdmCallbacks;

        public ServerRithmic(TLServer tls, DebugDelegate dd)
        {
            debs = dd;
            tl = tls;
            
            // set defaults
            PRI_bLoggedIntoMd = false;
            PRI_bGotPriceIncrInfo = false;
            PRI_oAccount = null;
            PRI_bGotAccounts = false;
            PRI_bLoggedIntoTs = false;
            PRI_bOrderComplete = false;
        }

        public string AdmPt = "dd_admin_sslc";
        public string MarketDataPt = "login_agent_sim_uatc";
        public string TsConnectPt = "login_agent_uatc";

        void setupparams()
        {
            oAdmCallbacks = new admcallback(this);
            oParams.AdmCnnctPt = AdmPt;
            
            oParams.AppName = "TradeLinkRithmicConnector";
            oParams.AppVersion = "1.0.0.0";
            oParams.AdmCallbacks = oAdmCallbacks;
            oParams.CertFile = "RithmicCertificate.pk12";
            oParams.DmnSrvrAddr = "rituz01000.01.rithmic.com:65000";
            oParams.DomainName = "rithmic_uat_01_dmz_domain";
            oParams.LicSrvrAddr = "rituz01000.01.rithmic.com:56000";
            oParams.LocBrokAddr = "rituz01000.01.rithmic.com:64100";
            oParams.LoggerAddr = "rituz01000.01.rithmic.com:45454";
        }

        bool _connected = false;

        public bool Start(string user,string pw)
        {
            if (_connected)
                return true;
            try
            {
                // rithmic stuff
                setupparams();
                oEngine = new REngine(oParams);
                oEngine.login(this,
                    user,
                    pw,
                    MarketDataPt,
                    TsConnectPt,
                    string.Empty,
                    string.Empty);

                // tradelink stuff
                tl.newProviderName = Providers.Rithmic;
                tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
                tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
                tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
                tl.newPosList += new PositionArrayDelegate(tl_newPosList);
                tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
                tl.newSendOrderRequest += new OrderDelegateStatus(tl_newSendOrderRequest);
                tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
                tl.SendDebugEvent+=new DebugDelegate(debug);

                _connected = LoggedIntoMd;

            }
            catch (Exception ex)
            {
                debug("error starting: " + ex.Message + ex.StackTrace);
            }

            return _connected;
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            return (long)MessageTypes.UNKNOWN_MESSAGE;
        }

        long tl_newSendOrderRequest(Order o)
        {
            MarketOrderParams oOrderParams = new MarketOrderParams();
            oOrderParams.Account = Account;
            oOrderParams.BuySellType = o.side ? Constants.BUY_SELL_TYPE_BUY : Constants.BUY_SELL_TYPE_SELL;
            oOrderParams.Context = null;
            oOrderParams.Duration = Constants.ORDER_DURATION_DAY;
            oOrderParams.EntryType = Constants.ORDER_ENTRY_TYPE_AUTO;
            oOrderParams.Exchange = o.ex;
            oOrderParams.Qty = o.size;
            oOrderParams.Symbol = o.symbol;
            oOrderParams.Tag = o.id.ToString();
            oOrderParams.UserMsg = o.comment;

            oEngine.sendOrder(oOrderParams);

            return (long)MessageTypes.OK;
        }

        bool _verbosedebug = false;
        public bool VerboseDebugging { get { return _verbosedebug; } set { _verbosedebug = value; } }

        void v(string msg)
        {
            if (VerboseDebugging)
            {
                debug(msg);
            }
        }


        void tl_newRegisterSymbols(string client, string symbols)
        {
            if (VerboseDebugging)
                debug("client subscribe request received: " + symbols);
            // get original basket
            Basket org = BasketImpl.FromString(symbols);
            Basket rem = new BasketImpl();

            // if we had something before, check if something was removed
            if (org.Count > 0)
            {
                rem = BasketImpl.Subtract(org, tl.AllClientBasket);
            }

            SubscriptionFlags flags = SubscriptionFlags.Prints;
            flags |= SubscriptionFlags.Quotes;
            flags |= SubscriptionFlags.Best;

            List<string> syms = new List<string>();
            syms.AddRange(tl.AllClientBasket.ToSymArray());

            // add current
            foreach (string sym in org.ToSymArray())
            {
                // subscribe what we don't have
                if (!syms.Contains(sym))
                    oEngine.subscribe(string.Empty,sym,flags,null);
            }

            // remove old
            foreach (Security s in rem)
            {
                oEngine.unsubscribe(string.Empty, s.Symbol);
            }

        }

        PositionTracker pt = new PositionTracker();

        Position[] tl_newPosList(string account)
        {
            return pt.ToArray();
        }

        void tl_newOrderCancelRequest(long val)
        {
            oEngine.cancelOrder(Account, val.ToString(), null, null);
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.OK);
            f.Add(MessageTypes.BROKERNAME);
            f.Add(MessageTypes.CLEARCLIENT);
            f.Add(MessageTypes.CLEARSTOCKS);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.HEARTBEATREQUEST);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.VERSION);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERLIMIT);

            return f.ToArray();
        }

        string tl_newAcctRequest()
        {
            if ((Account == null) || (Account.AccountId == null))
            {
                debug("Account not defined, responding with blank account.");
                return string.Empty;
            }
            return Account.AccountId;
        }

        public AccountInfo Account
        {
            get { return PRI_oAccount; }
        }

        /*   -----------------------------------------------------------   */

        public bool GotAccounts
        {
            get { return PRI_bGotAccounts; }
        }

        /*   -----------------------------------------------------------   */


        /*   -----------------------------------------------------------   */

        public bool LoggedIntoTs
        {
            get { return PRI_bLoggedIntoTs; }
        }

        /*   -----------------------------------------------------------   */

        public bool OrderComplete
        {
            get { return PRI_bOrderComplete; }
        }


        public bool LoggedIntoMd
               {
               get { return PRI_bLoggedIntoMd; }
               }

          public bool GotPriceIncrInfo
               {
               get { return PRI_bGotPriceIncrInfo; }
               }


          public override void Alert(AlertInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               debug(sb);

               if (oInfo.AlertType == AlertType.LoginComplete &&
     oInfo.ConnectionId == ConnectionId.MarketData)
               {
                   PRI_bLoggedIntoMd = true;
               }
               else if (oInfo.AlertType == AlertType.LoginComplete &&
                   oInfo.ConnectionId == ConnectionId.TradingSystem)
               {
                   PRI_bLoggedIntoTs = true;
               }

               else if (oInfo.AlertType == AlertType.LoginFailed)
               {
                   debug("Login failed, reason: " + oInfo.Message + " result code: " + oInfo.RpCode);
               }

               }



          public override void AskQuote(AskInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               debug(sb);
               }

          public override void BestAskQuote(AskInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               debug(sb);

               Tick k = TickImpl.NewBid(oInfo.Symbol, (decimal)oInfo.Price, oInfo.Size);
               k.be = oInfo.Exchange;
               k.date = Util.ToTLDate();
               k.time = Util.ToTLTime();
               tl.newTick(k);
               }

          public override void BestBidQuote(BidInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               debug(sb);

               Tick k = TickImpl.NewAsk(oInfo.Symbol, (decimal)oInfo.Price, oInfo.Size);
               k.be = oInfo.Exchange;
               k.date = Util.ToTLDate();
               k.time = Util.ToTLTime();
               tl.newTick(k);
               }

          public override void BidQuote(BidInfo oInfo)
               {

               }

          public override void ClosePrice(ClosePriceInfo oInfo)
               {

               }

          public override void ClosingIndicator(ClosingIndicatorInfo oInfo)
               {

               }

          public override void EndQuote(EndQuoteInfo oInfo)
               {

               }

          public override void EquityOptionList(EquityOptionListInfo oInfo)
               {

               }

          public override void EquityOptionStrategyList(EquityOptionStrategyListInfo oInfo)
               {

               }
          public override void ExchangeList(ExchangeListInfo oInfo)
               {

               }

          public override void HighPrice(HighPriceInfo oInfo)
               {

               }

          public override void LimitOrderBook(OrderBookInfo oInfo)
               {

               }

          public override void LowPrice(LowPriceInfo oInfo)
               {

               }

          public override void MarketMode(MarketModeInfo oInfo)
               {

               }

          public override void OpenPrice(OpenPriceInfo oInfo)
               {

               }

          public override void OptionList(OptionListInfo oInfo)
               {

               }

          public override void OpeningIndicator(OpeningIndicatorInfo oInfo)
               {

               }

          public override void PriceIncrUpdate(PriceIncrInfo oInfo)
               {


               if (oInfo.RpCode == 0)
                    {
                    PRI_bGotPriceIncrInfo = true;
                    }
               }

          public override void RefData(RefDataInfo oInfo)
               {

               }

          public override void SettlementPrice(SettlementPriceInfo oInfo)
               {

               }

          public override void Strategy(StrategyInfo oInfo)
               {

               }

          public override void StrategyList(StrategyListInfo oInfo)
               {

               }

          public override void TradeCondition(TradeInfo oInfo)
               {

               }

          public override void TradePrint(TradeInfo oInfo)
               {


               Tick k = TickImpl.NewTrade(oInfo.Symbol, (decimal)oInfo.Price, oInfo.Size);
               k.ex = oInfo.Exchange;
               k.date = Util.ToTLDate();
               k.time = Util.ToTLTime();
               tl.newTick(k);
               }

          public override void TradeVolume(TradeVolumeInfo oInfo)
               {

               }

          /*   ----------------------------------------------------------------   */

          public override void TimeBar(TimeBarInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               debug(sb);
               }

          public override void TimeBarReplay(TimeBarReplayInfo oInfo)
               {

               }

          public override void TradeReplay(TradeReplayInfo oInfo)
               {

               }

          /*   -----------------------------------------------------------   */

        
          

          public override void AccountList(AccountListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               debug(sb);
              PRI_bGotAccounts = true;
              if (oInfo.Accounts.Count > 0)
                    {
                    PRI_oAccount = new AccountInfo(oInfo.Accounts[0].FcmId,
                                                   oInfo.Accounts[0].IbId,
                                                   oInfo.Accounts[0].AccountId);
                    }
               
               }

          
          

          public override void PasswordChange(PasswordChangeInfo oInfo)
               {

               }

          public override void ProductRmsList(ProductRmsListInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               debug(sb);
               }

          public override void ExecutionReplay(ExecutionReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               debug(sb);
               }

          public override void LineUpdate(LineInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();

               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb);
               }
          public override void OpenOrderReplay(OrderReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               debug(sb);
               }

          public override void OrderReplay(OrderReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               oInfo.Dump(sb);
               sb.AppendFormat("\n");
               debug(sb);
               }

           public override void PnlReplay(PnlReplayInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();
               
               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb);
               }

          public override void PnlUpdate(PnlInfo oInfo)
               {
               StringBuilder sb = new StringBuilder();

               oInfo.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb);
               }

          public override void SodUpdate(SodReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb);
               }

          /*   -----------------------------------------------------------   */

          public override void BustReport(OrderBustReport oReport)
               {

               }

          public override void CancelReport(OrderCancelReport oReport)
               {

              long id = Convert.ToInt64(oReport.Tag);

              tl.newCancel(id);
              
               }

          public override void FailureReport(OrderFailureReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void FillReport(OrderFillReport oReport)
               {

                   Trade t = new TradeImpl(oReport.Symbol, (decimal)oReport.FillPrice, oReport.FillSize);
                   t.id = Convert.ToInt64(oReport.Tag);
                   t.ex = oReport.Exchange;
                   t.Account = oReport.Account.AccountId;
                   t.xdate = Util.ToTLDate();
                   t.xtime = Util.ToTLTime();
                   tl.newFill(t);
               }

          public override void ModifyReport(OrderModifyReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void NotCancelledReport(OrderNotCancelledReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void NotModifiedReport(OrderNotModifiedReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void OtherReport(OrderReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void RejectReport(OrderRejectReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void StatusReport(OrderStatusReport oReport)
               {

                   Order o = new OrderImpl(oReport.Symbol, oReport.EntryType.Contains("BUY"), oReport.ConfirmedSize);
                   o.Account = oReport.Account.AccountId;
                   o.id = Convert.ToInt64(oReport.Tag);
                   o.ex = oReport.Exchange;
                   o.date = Util.ToTLDate();
                   o.time = Util.ToTLTime();
                   tl.newOrder(o);
               }

          public override void TradeCorrectReport(OrderTradeCorrectReport oReport)
               {
               StringBuilder sb = new StringBuilder();

               oReport.Dump(sb);
               sb.AppendFormat("\n");

               debug(sb.ToString());
               }

          public override void TriggerPulledReport(OrderTriggerPulledReport oReport)
               {

               }

          public override void TriggerReport(OrderTriggerReport oReport)
               {

               }

          private bool PRI_bLoggedIntoMd;
          private bool PRI_bGotPriceIncrInfo;
          private AccountInfo PRI_oAccount;
          private bool PRI_bGotAccounts;
          private bool PRI_bLoggedIntoTs;
          private bool PRI_bOrderComplete;


        public void Stop()
        {
            try
            {
                oEngine.logout();
            }
            catch { }
        }

        protected void debug(StringBuilder sb) { debug(sb.ToString()); }
        protected void debug(string msg)
        {
            if (debs != null)
                debs(msg);
        }

        class admcallback : AdmCallbacks
        {
            ServerRithmic sr;
            internal admcallback(ServerRithmic srs)
            {
                sr = srs;
            }
            public override void Alert(AlertInfo oInfo)
            {
                StringBuilder sb = new StringBuilder();
                oInfo.Dump(sb);
                sr.debug(sb);
            }
        }

    }
}
