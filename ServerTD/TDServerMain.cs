using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using AMTD_API;

namespace TDServer
{
    public partial class TDServerMain : Form
    {
        AmeritradeBrokerAPI api = new AmeritradeBrokerAPI();
        const string APIVER = "1";
        TLServer_WM tl = new TLServer_WM(TLTypes.LIVEBROKER);
        public TDServerMain()
        {
            InitializeComponent();
            _msg.BringToFront();
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(MESSAGES, new EventHandler(togglemessages));
            FormClosing += new FormClosingEventHandler(TDServerMain_FormClosing);

            // bindings
            tl.newProviderName = Providers.TDAmeritrade;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.newAcctRequest += new StringDelegate(tl_gotSrvAcctRequest);
            tl.newOrderCancelRequest += new UIntDelegate(tl_newOrderCancelRequest);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);

            api.rs_LevelOneStreaming = new AmeritradeBrokerAPI.RequestState();
            api.rs_LevelOneStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_LevelOneStreaming_TickWithArgs);
            api.rs_ActivesStreaming = new AmeritradeBrokerAPI.RequestState();
            api.rs_ActivesStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_ActivesStreaming_TickWithArgs);
            
        }

        void rs_ActivesStreaming_TickWithArgs(DateTime time, AmeritradeBrokerAPI.ATradeArgument args)
        {
            if (args.FunctionType != AmeritradeBrokerAPI.RequestState.AsyncType.ActivesStreaming) return;
            // not sure if this is for orders or trades yet
            Order o = new OrderImpl();
            o.symbol = args.oStockTradeDetails.cStockSymbol;
            o.side = args.oStockTradeDetails.TradeType.ToLower() == "buy";
            o.size = Math.Abs(Convert.ToInt32(args.oStockTradeDetails.OrderShares)) * (o.side ? 1 : -1);
            o.ex = args.oStockTradeDetails.OrderRouting;
            o.price = Convert.ToDecimal(args.oStockTradeDetails.OrderPrice);
            o.id = Convert.ToUInt32(args.oStockTradeDetails.OrderID);
            o.TIF = args.oStockTradeDetails.OrderTimeInForce;
            o.Account = api._accountid;
            tl.newOrder(o);
            
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
            api.TD_getAcctBalancesAndPositions(_user.Text, _pass.Text, _sourceid.Text, APIVER, ref brokerAcctPosArgs.oCashBalances, ref brokerAcctPosArgs.oPositions);
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

        void tl_newRegisterStocks(string msg)
        {
            Basket mb = BasketImpl.Deserialize(msg);
            //Close_Connections(false);
            foreach (Security s in mb)
            {
                if (api.TD_IsStockSymbolValid(s.Symbol))
                {
                    string service = GetExchange(s.Symbol);
                    api.TD_RequestAsyncLevel1QuoteStreaming(s.Symbol, service, this);

                }
            }

        }

        string GetExchange(string sym)
        {
            if (ok)
            {
                AmeritradeBrokerAPI.L1quotes oL1Quote = api.TD_GetLevel1Quote(sym, 0);
                return oL1Quote.exchange;
            }
            return string.Empty;
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            switch (t)
            {
                case MessageTypes.ISSHORTABLE:
                    return api.TD_IsShortable(msg) ? 1 : 0;
            }
            return (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
        }


        void tl_newOrderCancelRequest(uint number)
        {
            string result = "";
            api.TD_CancelOrder(api._accountid, number.ToString(), _user.Text, _pass.Text, _sourceid.Text, APIVER, ref result);
            if (result=="")
                tl.newOrderCancel(number);
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ISSHORTABLE);
            f.Add(MessageTypes.TICKNOTIFY);


            return f.ToArray();

        }

        string tl_gotSrvAcctRequest()
        {
            return (ok ? api._accountid : "");
        }
        bool ok { get { return api.TD_loginStatus; } }
        void tl_gotSrvFillRequest(Order o)
        {
            if (!ok) { debug("not logged in."); return; }

            string action = o.side ? "buy" : "sell";
            string otype = o.isLimit ? "limit" : "market";
            
            AmeritradeBrokerAPI.ATradeArgument brokerReplyargs  = new AmeritradeBrokerAPI.ATradeArgument();
            string cResultMessage                               = string.Empty;
            string cEnteredOrderID                              = string.Empty;
            StringBuilder cOrderString                          = new StringBuilder();

            cOrderString.Append("action="           + api.Encode_URL(action)); 
            cOrderString.Append("~clientorderid="   + api.Encode_URL(o.id.ToString()));
            cOrderString.Append("~accountid="       + api.Encode_URL(api._accountid));
            cOrderString.Append("~actprice="        + api.Encode_URL(string.Empty));
            cOrderString.Append("~expire="          + api.Encode_URL(o.TIF)); 
            cOrderString.Append("~ordtype="         + api.Encode_URL(otype));
            cOrderString.Append("~price="           + api.Encode_URL(o.price.ToString()));
            cOrderString.Append("~quantity="        + api.Encode_URL(o.size.ToString()));
            cOrderString.Append("~spinstructions="  + api.Encode_URL("none"));
            cOrderString.Append("~symbol="          + api.Encode_URL(o.symbol));
            cOrderString.Append("~routing="         + api.Encode_URL(o.ex));

            cOrderString.Append("~tsparam="         + api.Encode_URL(string.Empty));
            cOrderString.Append("~exmonth="         + api.Encode_URL(string.Empty));
            cOrderString.Append("~exday="           + api.Encode_URL(string.Empty));
            cOrderString.Append("~exyear="          + api.Encode_URL(string.Empty));
            cOrderString.Append("~displaysize="     + api.Encode_URL(string.Empty));
            brokerReplyargs.ResultsCode = 
                api.TD_sendOrder(_user.Text, _pass.Text, _sourceid.Text, APIVER, cOrderString.ToString(), ref cResultMessage, ref cEnteredOrderID);


            
        }


        void TDServerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (ok)
                api.TD_Logout(_user.Text, _pass.Text, _sourceid.Text, APIVER);
        }
        private void _login_Click(object sender, EventArgs e)
        {
            // see if we're already logged in
            if (api.loginStatus) return;
            // see if we have proper info
            if (_user.Text.Length + _pass.Text.Length + _sourceid.Text.Length == 0)
            {
                MessageBox.Show("You must provide SourceID, username and password.");
                return;
            }
            bool yes = api.TD_brokerLogin(_user.Text, _pass.Text, _sourceid.Text, APIVER);
            if (yes)
            {
                // bind events
                api.TD_GetStreamerInfo(_user.Text, _pass.Text, _sourceid.Text, APIVER);
                api.TD_KeepAlive(_user.Text, _pass.Text, _sourceid.Text, APIVER);
                BackColor = Color.Green;
                Invalidate(true);
                debug("login succeeded");
            }
            else
            {
                const string msg = "login failed.  check information.";
                MessageBox.Show(msg);
                debug(msg);
            }
        }

        void debug(string msg)
        {
            if (_msg.InvokeRequired)
                Invoke(new DebugDelegate(debug), new object[] { msg });
            else
            {
                _msg.Items.Add(DateTime.Now.ToShortTimeString()+" "+msg);
                _msg.SelectedIndex = _msg.Items.Count - 1;
            }
        }
        const string MESSAGES = "Messages";
        void togglemessages(object sender, EventArgs e)
        {
            _msg.Visible = !_msg.Visible;
            if (InvokeRequired)
                Invoke(new EventHandler(togglemessages), new object[] { null, null });
            else
            {
                for (int i = 0; i < ContextMenu.MenuItems.Count; i++)
                    if (ContextMenu.MenuItems[i].Text == MESSAGES)
                    {
                        ContextMenu.MenuItems[i].Checked = _msg.Visible;
                        return;
                    }
            }
        }

        private void Close_Connections(bool lRecreateBrokerConnection)
        {
            try
            {
                if (api != null)
                {
                    if (this.api.TD_loginStatus == true)
                    {


                        /*/
                         * Close the Level Two Streaming Response Stream
                        /*/

                        if (api.rs_LevelTwoStreaming != null)
                        {
                            if (api.rs_LevelTwoStreaming.Request != null)
                            {
                                if (api.rs_LevelTwoStreaming.Request.ServicePoint.CurrentConnections > 0)
                                {
                                    api.rs_LevelTwoStreaming.CloseStream(api.rs_LevelTwoStreaming);
                                }
                            }
                        }


                        /*/
                         * Close the Level One Snapshot Response Stream
                        /*/

                        if (api.rs_LevelOneSnapshot != null)
                        {
                            if (api.rs_LevelOneSnapshot.Request != null)
                            {
                                if (api.rs_LevelOneSnapshot.Request.ServicePoint.CurrentConnections > 0)
                                {
                                    api.rs_LevelOneSnapshot.CloseStream(api.rs_LevelOneSnapshot);
                                }
                            }
                        }


                        /*/
                         * Close the Level One Streaming Response Stream
                        /*/

                        if (api.rs_LevelOneStreaming != null)
                        {
                            if (api.rs_LevelOneStreaming.Request != null)
                            {
                                if (api.rs_LevelOneStreaming.Request.ServicePoint.CurrentConnections > 0)
                                {
                                    api.rs_LevelOneStreaming.CloseStream(api.rs_LevelOneStreaming);
                                }
                            }
                        }



                        /*/
                         * Close the Historical Chart Snapshot Response Stream
                        /*/

                        if (api.rs_ChartSnapShot != null)
                        {
                            if (api.rs_ChartSnapShot.Request != null)
                            {
                                if (api.rs_ChartSnapShot.Request.ServicePoint.CurrentConnections > 0)
                                {
                                    api.rs_ChartSnapShot.CloseStream(api.rs_ChartSnapShot);
                                }
                            }
                        }


                        /*/
                         * Close the Historical Chart Streaming Response Stream
                        /*/

                        if (api.rs_ChartStreaming != null)
                        {
                            if (api.rs_ChartStreaming.Request != null)
                            {
                                if (api.rs_ChartStreaming.Request.ServicePoint.CurrentConnections > 0)
                                {
                                    api.rs_ChartStreaming.CloseStream(api.rs_ChartStreaming);
                                }
                            }
                        }


                        if (lRecreateBrokerConnection == true)
                        {
                            api = new AmeritradeBrokerAPI();
                        }

                    }
                }
            }
            catch (Exception exc) { }

        }


    }

}
