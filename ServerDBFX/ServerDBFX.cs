using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using FXCore;
namespace ServerDBFX
{
    public class ServerDBFX 
    {
        FXCore.TradeDeskAut _tradeDesk;
        //Creates Core object
        FXCore.CoreAut core = new FXCore.CoreAut();
        // this will receive events from dbfx
        FXCore.TradeDeskEventsSinkClass sink;
        public event DebugDelegate SendDebug;
        int _sub = 0;
        public ServerDBFX(TLServer tls)
        {
            tl = tls;
            // dbfx events
            _tradeDesk = (FXCore.TradeDeskAut)core.CreateTradeDesk("trader");
            sink = new FXCore.TradeDeskEventsSinkClass();
            sink.ITradeDeskEvents_Event_OnRowAddedEx += new FXCore.ITradeDeskEvents_OnRowAddedExEventHandler(sink_ITradeDeskEvents_Event_OnRowAddedEx);
            sink.ITradeDeskEvents_Event_OnRowBeforeRemoveEx += new FXCore.ITradeDeskEvents_OnRowBeforeRemoveExEventHandler(sink_ITradeDeskEvents_Event_OnRowBeforeRemoveEx);
            sink.ITradeDeskEvents_Event_OnRowChangedEx += new FXCore.ITradeDeskEvents_OnRowChangedExEventHandler(sink_ITradeDeskEvents_Event_OnRowChangedEx);
            _sub = _tradeDesk.Subscribe(sink);
            // tl events
            if (tl != null)
                tl.Start();
            tl.newProviderName = Providers.DBFX;
            tl.newFeatureRequest += new MessageArrayDelegate(ServerDBFX_newFeatureRequest);
            tl.newOrderCancelRequest += new LongDelegate(ServerDBFX_newOrderCancelRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(ServerDBFX_newSendOrderRequest);
        }
        MessageTypes[] ServerDBFX_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            return f.ToArray();
        }
        void sink_ITradeDeskEvents_Event_OnRowChangedEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            FXCore.ITableAut table = pTableDisp as FXCore.ITableAut;
        }
        void sink_ITradeDeskEvents_Event_OnRowBeforeRemoveEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            FXCore.ITableAut table = pTableDisp as FXCore.ITableAut;
        }
        void sink_ITradeDeskEvents_Event_OnRowAddedEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            FXCore.ITableAut table = pTableDisp as FXCore.ITableAut;
        }
        string[] _acct = new string[0];
        Dictionary<long, string> _tl2dbfx = new Dictionary<long, string>();
        bool isunique(Order o)
        {
            foreach (long id in _tl2dbfx.Keys)
                if (o.id == id) return false;
            return true;
        }
        public TLServer tl;

        IdTracker _id = new IdTracker();
        long ServerDBFX_newSendOrderRequest(Order o)
        {
            if ((o.id != 0) && !isunique(o))
                return (long)MessageTypes.DUPLICATE_ORDERID;
            if (o.id == 0)
                o.id = _id.AssignId;
            object psOrderId;
            object psDI;
            string acct = _acct[0];
            _tradeDesk.CreateEntryOrder(acct, o.symbol, o.side, o.size, (double)o.price, (double)o.stopp, (double)o.price, 0,out psOrderId, out psDI);
            _tl2dbfx.Add(o.id, psOrderId.ToString());
            tl.newOrder(o);
            //D(psOrderId.ToString());
            return (long)MessageTypes.OK;
        }
        void D(string msg) { if (SendDebug != null) SendDebug(msg); }
        void ServerDBFX_newOrderCancelRequest(long number)
        {
            string dbfxid = string.Empty;
            if (!_tl2dbfx.TryGetValue(number, out dbfxid)) return;
            _tradeDesk.DeleteOrder(dbfxid);
        }
        const string LOGINURL = @"http://dbfx.fxcorporate.com/Hosts.jsp";
        public bool Start(string username, string password, string type, int data2)
        {

            List<string> accts = new List<string>();
            try
            {
                _tradeDesk.Login(username, password, LOGINURL, type);
                TableAut tab = (TableAut)_tradeDesk.FindMainTable("accounts");
                _acct = new string[] { tab.CellValue(1, 1).ToString() };
            }
            catch (Exception ex) 
            {
                if (SendDebug != null)
                    SendDebug(ex.Message+ex.StackTrace);
                return false;
            }
            
            
            return true;
        }
        public void Stop()
        {
            _tradeDesk.Unsubscribe(_sub);
            _tradeDesk.Logout();
        }
        public static string getOrderStatusDescr(string orderStatus)
        {
            if (orderStatus.Length != 1)
                return "Unknown";
            switch (orderStatus[0])
            {
                case 'W':
                    return "Waiting";
                case 'P':
                    return "Processing";
                case 'Q':
                    return "Requoted";
                case 'C':
                    return "Cancelled";
                case 'E':
                    return "Executing";
                case 'R':
                    return "Rejected";
                case 'T':
                    return "Expired";
                case 'F':
                    return "Executed";
                case 'I':
                    return "Dealer Intervention";
            }
            return "Unknown";
        }
        protected Dictionary<string, string> parse(string extInfo)
        {
            string[] tokens = extInfo.Split(new char[] { '=', ';' });
            Dictionary<string, string> fields = new Dictionary<string, string>();
            int tokensCount = tokens.Length % 2 == 1 ? tokens.Length - 1 : tokens.Length;
            for (int i = 0; i < tokensCount; i += 2)
                fields.Add(tokens[i], tokens[i + 1]);
            return fields;
        }
        
    }
}
