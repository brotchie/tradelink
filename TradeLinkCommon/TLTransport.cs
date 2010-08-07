using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Common
{

    public enum TLTransportType
    {
        None,
        IPCWM,
        TCPIP,
    }

    public class IPUtil
    {
        public const int PRIVATEPORTSTART = 49152;
        public const int PRIVATEPORTEND = 65535;
        public const int TLDEFAULTBASEPORT = 50000;
        public const int TLDEFAULTTESTPORT = 51000;
        public const string LOCALHOST = "127.0.0.1";
        public const string CLIENTNAME = "TradeLinkClient";
        public const string SERVERNAME = "TradeLinkServer";
    }
}