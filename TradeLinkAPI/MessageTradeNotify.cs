
namespace TradeLink.API
{
    /// <summary>
    /// Order of fields in a TRADENOTIFY message
    /// </summary>
    public enum TradeField
    {
        xDate = 0,
        xTime,
        xSeconds,
        Symbol,
        Side,
        Size,
        Price,
        Comment,
        Account,
        Security,
        Currency,
        LocalSymbol,
        ID,
        Exch,
    }
}