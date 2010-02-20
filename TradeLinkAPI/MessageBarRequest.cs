
namespace TradeLink.API
{
    /// <summary>
    /// Order of fields in a BARREQUEST message
    /// </summary>
    public enum BarRequestField
    {
        Symbol,
        BarInt,  //BarInterval Tradelink.API.BarInterval
        StartDate, //TLDate 20100131 for January 1,2010
        StartTime, //TLFastTime TL2FT(int hour, int min, int sec) { return hour * 10000 + min * 100 + sec; }
        EndDate, //TLDate
        EndTime, //TLTime
        ID, //Unique request ID
        CustomInterval, //Custom interval period
        Client,// name of requesting application
    }
}
