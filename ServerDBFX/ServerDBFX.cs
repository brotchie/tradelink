using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace ServerDBFX
{
    public class ServerDBFX : TLServer_WM
    {
        FXCore.TradeDeskAut _tradeDesk;
        //Creates Core object
        FXCore.CoreAut core = new FXCore.CoreAut();
        // this will receive events from dbfx
        FXCore.TradeDeskEventsSinkClass sink;

        public event DebugFullDelegate SendDebug;

        int _sub = 0;

        public ServerDBFX()
        {
            // dbfx events
            sink = new FXCore.TradeDeskEventsSinkClass();
            sink.ITradeDeskEvents_Event_OnRowAddedEx += new FXCore.ITradeDeskEvents_OnRowAddedExEventHandler(sink_ITradeDeskEvents_Event_OnRowAddedEx);
            sink.ITradeDeskEvents_Event_OnRowBeforeRemoveEx += new FXCore.ITradeDeskEvents_OnRowBeforeRemoveExEventHandler(sink_ITradeDeskEvents_Event_OnRowBeforeRemoveEx);
            sink.ITradeDeskEvents_Event_OnRowChangedEx += new FXCore.ITradeDeskEvents_OnRowChangedExEventHandler(sink_ITradeDeskEvents_Event_OnRowChangedEx);
            _sub = _tradeDesk.Subscribe(sink);
            // tl events
            newOrderCancelRequest += new UIntDelegate(ServerDBFX_newOrderCancelRequest);
            newSendOrderRequest += new OrderDelegate(ServerDBFX_newSendOrderRequest);
        }

        void sink_ITradeDeskEvents_Event_OnRowChangedEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            
        }



        void sink_ITradeDeskEvents_Event_OnRowBeforeRemoveEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            
        }

        void sink_ITradeDeskEvents_Event_OnRowAddedEx(object pTableDisp, string sRowID, string sExtInfo)
        {
            
        }

        void ServerDBFX_newSendOrderRequest(Order o)
        {
            object assignedid = -1;
            object psDI;
            _tradeDesk.CreateEntryOrder(o.Account, o.symbol, o.side, o.size, 1, (double)o.stopp, (double)o.price, 0,out assignedid, out psDI);

        }

        void ServerDBFX_newOrderCancelRequest(uint number)
        {
            _tradeDesk.DeleteOrder(number.ToString());
        }

        public bool Start(string username, string password, string data, int data2)
        {
            try
            {
                _tradeDesk = (FXCore.TradeDeskAut)core.CreateTradeDesk(username);
                _tradeDesk.Login(username, password, "www.fxcorporate.com", data);
            }
            catch (Exception ex) 
            {
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create(ex.Message+ex.StackTrace));
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

        
    }


}
