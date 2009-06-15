using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.Common;
using AnvilNetApi;
namespace ServerAnvil
{
    public class ServerAnvil : TLServer_WM
    {
        NAccountObserver no = new NAccountObserver();
        bool start = false;
        public ServerAnvil()
        {
            start = Anvil.s_Initialize();
            if (start)
            {
                newImbalanceRequest += new TradeLink.API.VoidDelegate(ServerAnvil_newImbalanceRequest);
                newSendOrderRequest += new TradeLink.API.OrderDelegate(ServerAnvil_newSendOrderRequest);
                newRegisterStocks += new TradeLink.API.DebugDelegate(ServerAnvil_newRegisterStocks);
                no.OnOrderNew += new DelegateOnOrderNew(no_OnOrder);

                no.OnTextMessage += new DelegateOnTextMessage(no_OnTextMessage);
                no.OnOrderAssignId += new DelegateOnOrderAssignId(no_OnOrderAssignId);
                no.OnOrderUpdate += new DelegateOnOrderUpdate(no_OnOrder);
                no.OnOrderExecution += new DelegateOnOrderExecution(no_OnOrderExecution);
            }
        }
        List<NStock> stocklist = new List<NStock>();
        void ServerAnvil_newRegisterStocks(string msg)
        {
            
        }

        void ServerAnvil_newSendOrderRequest(TradeLink.API.Order o)
        {
            
        }

        NMarketImbalanceObserver nio = null;
        void ServerAnvil_newImbalanceRequest()
        {
            nio = new NMarketImbalanceObserver();
            NObservable ob = NMarketImbalanceObserver.s_GetObservable();
            ob.Add(nio);
            nio.OnNyseImbalance += new DelegateOnNyseImbalance(nio_OnNyseImbalance);
            nio.OnNasdaqImbalance += new DelegateOnNasdaqImbalance(nio_OnNasdaqImbalance);
        }

        void nio_OnNasdaqImbalance(NObservable nobservable, string symbol, NTImbalanceFlags flags, uint matchedShares, uint imbalance, int currentReferencePrice, int nearIndicativeClearingPrice, int farIndicativeClearingPrice, sbyte noIndicativeClearingPriceFlag, sbyte priceVariationIndicator)
        {
            
        }

        void nio_OnNyseImbalance(NObservable nobservable, string symbol, NTImbalanceFlags flags, uint buyVolume, uint sellVolume)
        {
            
        }

        void Stop()
        {
            Anvil.s_Terminate();
        }

        void no_OnTextMessage(NObservable nobservable, string symbol, string message, NMessageType type)
        {
            
        }

        void no_OnOrderAssignId(NObservable nobservable, NOrder norder)
        {
            
        }

        void no_OnOrder(NObservable nobservable, NOrder norder, NTracking tracking)
        {
            
        }

        void no_OnOrder(NObservable nobservable, NOrder norder)
        {
            
        }

        void no_OnOrderExecution(NObservable nobservable, NOrder norder, NExecution nexecution)
        {
            TradeImpl t = new TradeImpl();
            t.Account = norder.Account.Name;
            t.xsize = (int)(nexecution.Shares * (nexecution.Side == 'B' ? 1 : -1));
            t.xprice = (decimal)nexecution.Price.ToDouble();
        }
    }
}
