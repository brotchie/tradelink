
namespace TradeLink.API
{
    /// <summary>
    /// Ordering of fields in SENDORDER and ORDERNOTIFY messages
    /// </summary>
    public enum OrderField
    {
        Symbol = 0,
        Side,
        Size,
        Price,
        Stop,
        Comment,
        Exchange,
        Account,
        Security,
        Currency,
        LocalSymbol, // non-pretty symbol or contract symbol for futures
        OrderID,
        OrderTIF,
        oDate,
        oTime,
        oUNUSED,
        Trail,
    }
}