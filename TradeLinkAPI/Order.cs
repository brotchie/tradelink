using System;

namespace TradeLink.API
{
    /// <summary>
    /// TradeLink Order
    /// </summary>
    public interface Order
    {
        /// <summary>
        /// symbol of order
        /// </summary>
        string symbol { get; set; }
        /// <summary>
        /// time in force
        /// </summary>
        string TIF { get; set; }
        /// <summary>
        /// valid instruction
        /// </summary>
        OrderInstructionType ValidInstruct { get; set; }
        /// <summary>
        /// true if buy, otherwise sell
        /// </summary>
        bool side { get; set; }
        /// <summary>
        /// price of order. (0 for market)
        /// </summary>
        decimal price { get; set; }
        /// <summary>
        /// stop price if applicable
        /// </summary>
        decimal stopp { get; set; }
        /// <summary>
        /// trail amount if applicable
        /// </summary>
        decimal trail { get; set; }
        /// <summary>
        /// order comment
        /// </summary>
        string comment { get; set; }
        /// <summary>
        /// destination for order
        /// </summary>
        string ex { get; set; }
        /// <summary>
        /// destination for order
        /// </summary>
        string Exchange { get; set; }
        /// <summary>
        /// signed size of order (-100 = sell 100)
        /// </summary>
        int size { get; set; }
        /// <summary>
        /// unsigned size of order
        /// </summary>
        int UnsignedSize { get; }
        /// <summary>
        /// date in tradelink date format (2010/03/05 = 20100305)
        /// </summary>
        int date { get; set; }
        /// <summary>
        /// time including seconds 1:35:07PM = 133507
        /// </summary>
        int time { get; set; }
        /// <summary>
        /// whether order has been filled
        /// </summary>
        bool isFilled { get; }
        /// <summary>
        /// limit order
        /// </summary>
        bool isLimit { get; }
        /// <summary>
        /// stop order
        /// </summary>
        bool isStop { get; }
        /// <summary>
        /// trail order
        /// </summary>
        bool isTrail { get; }
        /// <summary>
        /// market order
        /// </summary>
        bool isMarket { get; }
        /// <summary>
        /// security type represented by order
        /// </summary>
        SecurityType Security { get; set; }
        /// <summary>
        /// currency with which to place order
        /// </summary>
        CurrencyType Currency { get; set; }
        /// <summary>
        /// security/contract information for order
        /// </summary>
        Security Sec { get; }
        /// <summary>
        /// account to place inventory if order is executed
        /// </summary>
        string Account { get; set; }
        /// <summary>
        /// more specific symbol name
        /// </summary>
        string LocalSymbol { get; set; }
        /// <summary>
        /// order id
        /// </summary>
        long id { get; set; }
        /// <summary>
        /// try to fill order against another order
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool Fill(Order o);
        /// <summary>
        /// try to fill order against trade
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Fill(Tick t);
        /// <summary>
        /// try to fill order against trade or bid/ask
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Fill(Tick t, bool bidask, bool fillopg);
        /// <summary>
        /// try to fill order as OPG order
        /// </summary>
        /// <param name="t"></param>
        /// <param name="fillOPG"></param>
        /// <returns></returns>
        bool Fill(Tick t, bool fillOPG);
        /// <summary>
        /// order is valid
        /// </summary>
        bool isValid { get; }
        /// <summary>
        /// owner/originator of this order
        /// </summary>
        int VirtualOwner { get; set; }

        string ToString(int decimals);
    }

    public class InvalidOrder : Exception { }
}
