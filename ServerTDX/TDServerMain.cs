using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;

using TradeLink.AppKit;
using AMTD_API;


namespace TDServer
{
    public partial class TDServerMain : AppTracker
    {


        private bool LOGGEDIN;
       // AmeritradeBrokerAPI api = new AmeritradeBrokerAPI();
        const string APIVER = "1";
        TLServer tl = new TLServer_WM();
        public const string PROGRAM = "ServerTDX BETA";
        Log _log = new Log(PROGRAM);
        public TDServerMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(TDServerMain_FormClosing);
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tl = new TradeLink.Common.TLServer_WM();
            else
                tl = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);


            // bindings
            tl.newProviderName = Providers.TDAmeritrade;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_gotSrvFillRequest);
            tl.newAcctRequest += new StringDelegate(tl_gotSrvAcctRequest);
            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            tl.newRegisterSymbols +=new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);

            api.OnStatusChange += new Axtdaactx.ITDAAPICommEvents_OnStatusChangeEventHandler(api_OnStatusChange);
            //api.OnL1Quote +=  rs_LevelOneStreaming = new AmeritradeBrokerAPI.RequestState();
            //api.rs_LevelOneStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_LevelOneStreaming_TickWithArgs);
            //api.rs_ActivesStreaming = new AmeritradeBrokerAPI.RequestState();
            //api.rs_ActivesStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_ActivesStreaming_TickWithArgs);
            api.OnL1Quote += new Axtdaactx.ITDAAPICommEvents_OnL1QuoteEventHandler(api_LevelOneStreaming);
            doLogin();
        }

        
        void tl_newRegisterSymbols(string client, string symbols)
        {
            debug("got symbol request: " + client + " for: " + symbols);
            Basket mb = tl.AllClientBasket;
            api.UnsubscribeAll();
            // clear idx values
            _isidx.Clear();
            //Close_Connections(false);
            foreach (Security s in mb)
            {
                //if (api.SymbTD_IsStockSymbolValid(s.Symbol))
                {
                    if (s.Symbol.Contains("$"))
                        _isidx.Add(s.Symbol, true);
                    api.Subscribe(s.Symbol, tdaactx.TxTDASubTypes.TDAPI_SUB_L1);//, service, this);

                }
            }

        }

        void rs_ActivesStreaming_TickWithArgs(DateTime time, AmeritradeBrokerAPI.ATradeArgument args)
        {
            if (args.FunctionType != AmeritradeBrokerAPI.RequestState.AsyncType.ActivesStreaming) return;
            // not sure how to activate this yet
            
        }
        
        private void api_OnStatusChange(object sender, Axtdaactx.ITDAAPICommEvents_OnStatusChangeEvent e)
        {
            debug("status chage:" +  e.newStatus +  " from " + e.oldStatus + "LoggedID=" + api.LoggedIn.ToString() + "\n");
            if (api.LoggedIn)
            {
                this.BackColor =  Color.Green;
            }
            else
            {
                this.BackColor = Color.Red;
            }

        }

        private void api_LevelOneStreaming(object sender, Axtdaactx.ITDAAPICommEvents_OnL1QuoteEvent e)
        {
            DateTime DT = new DateTime(1970, 1, 1);
            //Axtdaactx.AxTDAL1Quote quote = (Axtdaactx.AxTDAL1Quote)e.quote;

            Tick t = new TickImpl();
            t.date = Util.ToTLDate(DateTime.Now);
            t.time = Util.DT2FT(DateTime.Now);
            t.symbol = e.quote.Symbol;
            t.bid = Convert.ToDecimal(e.quote.Bid);
            t.ask = Convert.ToDecimal(e.quote.Ask);
            t.ex = e.quote.Exchange.ToString();
            t.trade = Convert.ToDecimal(e.quote.Last);
            t.size = !isidx(e.quote.Symbol) ? Convert.ToInt32(e.quote.LastSize) * 100 : -1;
            t.bs = Convert.ToInt32(e.quote.BidSize);
            t.os = Convert.ToInt32(e.quote.AskSize);
            tl.newTick(t);

        }
    
        void rs_LevelOneStreaming_TickWithArgs(DateTime time, AmeritradeBrokerAPI.ATradeArgument args)
        {
            if (args.FunctionType != AmeritradeBrokerAPI.RequestState.AsyncType.LevelOneStreaming) return;
            Tick t = new TickImpl();
            /*  don't understand the time format provided here
            int date = 0;
            int ttime = 0;
            if (int.TryParse(args.oLevelOneData[0].quotedate, out date))
                t.date = date;
            if (int.TryParse(args.oLevelOneData[0].quotetime, out ttime))
                t.time = ttime;
             */
           
            t.date = Util.ToTLDate(DateTime.Now);
            t.time = Util.DT2FT(DateTime.Now);
            t.symbol = args.oLevelOneData[0].stock;
            t.bid = Convert.ToDecimal(args.oLevelOneData[0].bid);
            t.ask = Convert.ToDecimal(args.oLevelOneData[0].ask);
            t.ex = args.oLevelOneData[0].exchange;
            t.trade = Convert.ToDecimal(args.oLevelOneData[0].last);
            t.size = Convert.ToInt32(args.oLevelOneData[0].lastsize) * 100;
            t.bs = Convert.ToInt32(args.oLevelOneData[0].bid_size);
            t.os = Convert.ToInt32(args.oLevelOneData[0].ask_size);
            tl.newTick(t);
            
        }


        Position[] tl_newPosList(string account)
        {
            AmeritradeBrokerAPI.ATradeArgument brokerAcctPosArgs = new AmeritradeBrokerAPI.ATradeArgument();
            brokerAcctPosArgs.oPositions = new List<AmeritradeBrokerAPI.Positions>();
            //api.TD_getAcctBalancesAndPositions(_user.Text, _pass.Text, AmeritradeBrokerAPI.SOURCEID, APIVER, ref brokerAcctPosArgs.oCashBalances, ref brokerAcctPosArgs.oPositions);
            Position[] plist = new Position[brokerAcctPosArgs.oPositions.Count];
            int count = 0;
            foreach (AmeritradeBrokerAPI.Positions oPosition in brokerAcctPosArgs.oPositions)
            {
                decimal price = 0;
                decimal.TryParse(oPosition.AveragePric,out price);
                int size = 0;
                int.TryParse(oPosition.Quantity,out size);
                Position p = new PositionImpl(oPosition.StockSymbol,price,size);
                plist[count++] = p;
            }
            return plist;

            
        }

        Dictionary<string, bool> _isidx = new Dictionary<string, bool>();
        bool isidx(string sym)
        {
            bool y = false;
            if (_isidx.TryGetValue(sym, out y))
                return y;
            return false;
        }


        string GetExchange(string sym)
        {
            if (ok)
            {

                //AmeritradeBrokerAPI.L1quotes oL1Quote = api.RequestSnapshotQuotes(sym);
                return "";// oL1Quote.exchange;
            }
            return string.Empty;
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            //switch (t)
            //{
              //  case MessageTypes.ISSHORTABLE:
             //       return api.TD_IsShortable(msg) ? 1 : 0;
            //}
            return (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
        }


        void tl_newOrderCancelRequest(long number)
        {
            string result ="error";
            long id;
            string orderid = "";
            if (idmap.TryGetValue(number, out id))
                orderid = id.ToString();
            
         //   api.TD_CancelOrder(api._accountid, orderid, _user.Text, _pass.Text, AmeritradeBrokerAPI.SOURCEID, APIVER, ref result);
            if (result==string.Empty)
                tl.newCancel(number);
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ISSHORTABLE);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.LIVEDATA);


            return f.ToArray();

        }

        string tl_gotSrvAcctRequest()
        {
            return (ok ? api.AccountID(0) : "");
         }
            bool ok { get { return api.LoggedIn; } }
            Dictionary<long, long> idmap = new Dictionary<long, long>();
        
        long tl_gotSrvFillRequest(Order o)
        {
            if (!ok)
            {
                debug("not logged in.");
                return (long)MessageTypes.SYMBOL_NOT_LOADED;
            }
            string action = o.side ? "buy" : "sell";
            string otype = o.isLimit ? "limit" : "market";
            if (o.id == 0)
                o.id = OrderImpl.Unique;
            string route = "auto";
            if (o.ex.ToUpper().Contains("ARCA"))
                route = "ecn_arca";
            else if (o.ex.ToUpper().Contains("INET"))
                route = "inet";
           
            AmeritradeBrokerAPI.ATradeArgument brokerReplyargs  = new AmeritradeBrokerAPI.ATradeArgument();
            string cResultMessage                               = string.Empty;
            string cEnteredOrderID                              = string.Empty;
            StringBuilder cOrderString                          = new StringBuilder();

            /*
            cOrderString.Append("action=" + api.Encode_URL(action));
            cOrderString.Append("~clientorderid=" + api.Encode_URL(o.id.ToString()));
            cOrderString.Append("~accountid=" + api.Encode_URL(api._accountid));
            cOrderString.Append("~actprice=" + api.Encode_URL(string.Empty));
            cOrderString.Append("~expire=" + api.Encode_URL(o.TIF));
            cOrderString.Append("~ordtype=" + api.Encode_URL(otype));
            cOrderString.Append("~price=" + api.Encode_URL(o.price.ToString()));
            cOrderString.Append("~quantity=" + api.Encode_URL(o.size.ToString()));
            cOrderString.Append("~spinstructions=" + api.Encode_URL("none"));
            cOrderString.Append("~symbol=" + api.Encode_URL(o.symbol));
            cOrderString.Append("~routing=" + api.Encode_URL(route));

            cOrderString.Append("~tsparam=" + api.Encode_URL(string.Empty));
            cOrderString.Append("~exmonth=" + api.Encode_URL(string.Empty));
            cOrderString.Append("~exday=" + api.Encode_URL(string.Empty));
            cOrderString.Append("~exyear=" + api.Encode_URL(string.Empty));
            cOrderString.Append("~displaysize=" + api.Encode_URL(string.Empty));
            brokerReplyargs.ResultsCode =
                api.TD_sendOrder(_user.Text, _pass.Text, AmeritradeBrokerAPI.SOURCEID, APIVER, cOrderString.ToString(), ref cResultMessage, ref cEnteredOrderID);

            if (brokerReplyargs.ResultsCode != OK)
                debug(cResultMessage);
            else
            {
                long tdid = 0;
                if (long.TryParse(cEnteredOrderID, out tdid))
                    idmap.Add(o.id, tdid);
                tl.newOrder(o);
            }*/
            debug("order not presently hooked up in TDX: "+o.ToString());

            return (long)MessageTypes.OK;
        }
        const string OK = "OK";

        void TDServerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Stop();
            Properties.Settings.Default.Save();
            if (ok)
                api.Logout();
        }

        private void doLogin()
        {
               // see if we're already logged in
            if (api.LoggedIn)
            {
               // this._login.Enabled = api.loginStatus;
                return;
            }
            // see if we have proper info
            if (_user.Text.Length + _pass.Text.Length + AmeritradeBrokerAPI.SOURCEID.Length == 0)
            {
                MessageBox.Show("You must provide SourceID, username and password.");
                return;
            }
            api.LoginName = _user.Text;
            api.LoginPassword = _pass.Text;
            api.SourceApp = AmeritradeBrokerAPI.SOURCEID;
            api.LoginSite = "apis.tdameritrade.com";
            api.STARTIT();           
            BackColor = Color.Green;
            Invalidate(true);
               
            }
            
            //this._login.Enabled = api.loginStatus;
        

        private  void _login_Click(object sender, EventArgs e)
        {
            doLogin();
        }

        public static DebugWindow _dw = new DebugWindow();

        void debug(string msg)
        {
            _log.GotDebug(msg);
            _dw.GotDebug(msg);
            
        }
        const string MESSAGES = "Messages";

        private void Close_Connections(bool lRecreateBrokerConnection)
        {
            try
            {
                     if (lRecreateBrokerConnection == true)
                        {
                            //api = new AmeritradeBrokerAPI();
                        } 
            }             
            
            catch (Exception) { }

        }

        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
           // CrashReport.Report(PROGRAM, _dw.Content);
        }

    }

}
