using TradeLink.API;
using TradeLink.Common;
using System.Net;

namespace TradeLink.Common
{
    /// <summary>
    /// transport type
    /// </summary>
    public enum TLTransportType
    {
        None,
        IPCWM,
        TCPIP,
    }

    /// <summary>
    /// ip util class
    /// </summary>
    public class IPUtil
    {
        /// <summary>
        /// true if an ip address in string format is valid
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        public static bool isValidAddress(string ipaddr)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(ipaddr);
                return true;
            }
            catch {  }
            return false;
            
        }
        /// <summary>
        /// returns true if an array has at least one valid address
        /// </summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public static bool hasValidAddress(string[] servers)
        {
            bool ok = false;
            foreach (string server in servers)
                ok |= isValidAddress(server);
            return ok;
        }

        public const int PRIVATEPORTSTART = 49152;
        public const int PRIVATEPORTEND = 65535;
        public const int TLDEFAULTBASEPORT = 50000;
        public const int TLDEFAULTTESTPORT = 51000;
        public const string LOCALHOST = "127.0.0.1";
        public const string CLIENTNAME = "TradeLinkClient";
        public const string SERVERNAME = "TradeLinkServer";

        public const int SENDHEARTBEATMS = 3000;
        public const int HEARTBEATDEADMS = SENDHEARTBEATMS * 2;
    }
}