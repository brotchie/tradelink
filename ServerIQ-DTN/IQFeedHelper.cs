using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using TradeLink.API;
using TradeLink.Common;

namespace IQFeedBroker
{
    public class IQFeedHelper
    {
        #region Variables

        internal event TickDelegate TickReceived;
        private const string IQ_FEED = "iqconnect";
        private readonly string IQ_FEED_PROGRAM = "IQConnect.exe";
        private const string IQ_FEED_REGISTRY_LOCATION = "SOFTWARE\\DTN\\IQFeed";
        private AsyncCallback m_pfnCallback;
        private Socket m_sockAdmin;
        private Socket m_sockIQConnect;
        private byte[] m_szAdminSocketBuffer = new byte[8096];
        private byte[] m_szLevel1SocketBuffer = new byte[8096];
        private Basket _basket;
        private string _user;
        private string _pswd;
        private bool _registered;

        #endregion


        #region Properties

        public bool IsConnected { get; private set; }

        private bool HaveUserCredentials
        {
            get
            {
                return !(string.IsNullOrEmpty(_user) && string.IsNullOrEmpty(_pswd));
            }
        }

        #endregion


        #region Constructors

        public IQFeedHelper()
        {
            _basket = new BasketImpl();
            
        }

        public IQFeedHelper(string username, string password)
        {
            Start(username, password);
            _basket = new BasketImpl();
        }

        public IQFeedHelper(Basket basket, string username, string password)
        {
            Start(username, password);
            _basket = basket;
        }

        public IQFeedHelper(Basket basket)
        {
            Start(string.Empty, string.Empty);
            _basket = basket;
        }

        #endregion


        #region IQFeedHelper Members

        public void Close()
        {
            Array.ForEach(System.Diagnostics.Process.GetProcessesByName(IQ_FEED), iqProcess => iqProcess.Kill());
            debug("QUITTING****************************");
        }


        internal void ConnectToAdmin()
        {
            // Establish a connection to the admin socket in IQ Feed
            Thread.Sleep(new TimeSpan(0, 0, 5));
            ConnectToAdminSocket();
        }


        internal void ConnectToLevelOne()
        {
            try
            {
                Thread.Sleep(new TimeSpan(0, 0, 5));
                ConnectToLevelOneSocket();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
            }
        }


        /// <summary>
        /// Subscribe to securities in the basket supplied as long as they aren't already in the underlying basket.
        /// </summary>
        /// <param name="basket"></param>
        internal void AddBasket(Basket basket)
        {
            foreach (Security security in basket)
            {
               /* You need to modify BaskeImpl for the contains to work*/
               // if (!((BasketImpl)_basket).contains(security.Symbol))
               // {
                    AddSecurityToBasket(security);
               // }
            }
        }


        /// <summary>
        /// Subscribe to the security supplied as long as its not in the underlying basket
        /// </summary>
        /// <param name="security"></param>
        internal void AddSecurity(Security security)
        {
            /* You need to modify BaskeImpl for the contains to work*/
            //if (!((BasketImpl)_basket).contains(security.Symbol))
            //{
                AddSecurityToBasket(security);
            //}
        }


        /// <summary>
        /// Physically adds the security to the underlying basket and connects to the socket passing
        /// this security
        /// </summary>
        /// <param name="security"></param>
        private void AddSecurityToBasket(Security security)
        {
            _basket.Add(security);

            // we form a watch command in the form of wSYMBOL\r\n
            string command = String.Format("w{0}\r\n", security.Symbol);

            // and we send it to the feed via the socket
            byte[] watchCommand = new byte[command.Length];
            watchCommand = Encoding.ASCII.GetBytes(command);
            m_sockIQConnect.Send(watchCommand, watchCommand.Length, SocketFlags.None);
        }



        public void Start(string username, string password)
        {
            _registered = false;

            // get IQConnect Location from registry
            // IQFeed Installation directory is stored in the registry key
            RegistryKey key = Registry.LocalMachine.OpenSubKey(IQ_FEED_REGISTRY_LOCATION);
            if (key == null)
            {
                debug("IQ Feed is not installed");
                return;
            }

            // close the key since we don't need it anymore
            key.Close();
            key = null;

            _user = username;
            _pswd = password;

            try
            {
                int iqConnectProcessCount = System.Diagnostics.Process.GetProcessesByName(IQ_FEED).Length;
                switch (iqConnectProcessCount)
                {
                    case 1:
                        debug("IQ Connect price feed is already running");
                        break;
                    case 0:
                        debug("Need to start IQ Connect first");
                        string args = string.Empty;
                        if (HaveUserCredentials)
                            args += String.Format("-product {0} -version 1.0.0.0 -login {1} -password {2} -savelogininfo -autoconnect", Global.PROGRAM_NAME, _user, _pswd);
                        System.Diagnostics.Process.Start(IQ_FEED_PROGRAM, args);
                        break;
                    default:
                        throw new ApplicationException(string.Format("IQ Connect Feed has {0} instances currently running", iqConnectProcessCount));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Establishes a connection to the admin socket in the IQ Feed
        /// </summary>
        private void ConnectToAdminSocket()
        {
            try
            {
                m_sockAdmin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipLocalHost = IPAddress.Parse(Global.IP_LOCAL_HOST);
                RegistryKey key = Registry.CurrentUser.OpenSubKey(String.Format("{0}\\Startup", IQ_FEED_REGISTRY_LOCATION));
                int port = int.Parse(key.GetValue(String.Format("{0}Port", Global.ADMINISTRATION_SOCKET_NAME), "9300").ToString());
                IPEndPoint endPoint = new IPEndPoint(ipLocalHost, port);
                m_sockAdmin.Connect(endPoint);
                WaitForData(Global.ADMINISTRATION_SOCKET_NAME);
            }
            catch (Exception ex)
            {
                debug(String.Format("ADMIN SOCKET ERROR: {0}", ex.Message));
            }
        }


        /// <summary>
        /// Establishes a connection to the admin socket in IQFeed
        /// </summary>
        private void ConnectToLevelOneSocket()
        {
            try
            {
                m_sockIQConnect = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipLocalHost = IPAddress.Parse(Global.IP_LOCAL_HOST);
                RegistryKey key = Registry.CurrentUser.OpenSubKey(String.Format("{0}\\Startup", IQ_FEED_REGISTRY_LOCATION));
                int port = int.Parse(key.GetValue(String.Format("{0}Port", Global.LEVEL_ONE_SOCKET_NAME), "5009").ToString());
                IPEndPoint endPoint = new IPEndPoint(ipLocalHost, port);
                m_sockIQConnect.Connect(endPoint);
                WaitForData(Global.LEVEL_ONE_SOCKET_NAME);
            }
            catch (Exception ex)
            {
                debug(String.Format("LEVEL ONE ERROR: {0}", ex.Message));
                throw ex;
            }
        }


        private void WaitForData(string socketName)
        {
            if (m_pfnCallback == null)
                m_pfnCallback = new AsyncCallback(OnReceiveData);

            if (socketName == Global.LEVEL_ONE_SOCKET_NAME)
            {
                if (m_sockIQConnect != null)
                    m_sockIQConnect.BeginReceive(m_szLevel1SocketBuffer, 0, m_szLevel1SocketBuffer.Length, SocketFlags.None, m_pfnCallback, socketName);
            }
            else if (socketName == Global.ADMINISTRATION_SOCKET_NAME)
                m_sockAdmin.BeginReceive(m_szAdminSocketBuffer, 0, m_szAdminSocketBuffer.Length, SocketFlags.None, m_pfnCallback, socketName);
        }


        /// <summary>
        /// This is our callback that gets called by the .NET socket class when new data arrives on the socket
        /// </summary>
        /// <param name="asyn"></param>
        private void OnReceiveData(IAsyncResult result)
        {
            // first verify we received data from the correct socket.
            if (result.AsyncState.ToString().Equals(Global.ADMINISTRATION_SOCKET_NAME))
            {
                try
                {
                    debug(String.Format("Result State: {0}", result.AsyncState));
                    int bytesReceived = m_sockAdmin.EndReceive(result);
                    string rawData = Encoding.ASCII.GetString(m_szAdminSocketBuffer, 0, bytesReceived);
                    bool connectToLevelOne = _registered;

                    while (rawData.Length > 0)
                    {
                        string data = rawData.Substring(0, rawData.IndexOf("\n"));
                        debug(String.Format("ADMIN: {0}", data));
                        if (data.StartsWith("S,STATS,"))
                        {
                            #region Register this application (if necessary)...May want to extract this into a separate function as it's only called once.
                            if (!_registered)
                            {
                                string command = String.Format("S,REGISTER CLIENT APP,{0},{1}\r\n", Global.PROGRAM_NAME, Assembly.GetExecutingAssembly().GetName().Version);
                                byte[] size = new byte[command.Length];
                                int bytesToSend = size.Length;
                                size = Encoding.ASCII.GetBytes(command);
                                m_sockAdmin.Send(size, bytesToSend, SocketFlags.None);
                                _registered = true;
                            }
                            #endregion
                        }
                        else if (data.StartsWith("S,REGISTER CLIENT APP COMPLETED"))
                        {
                            #region Set the login and other attribute settings...KRJ: Not written this yet to see if can do on start up
                            string command = "S,SET LOGINID,244023\r\nS,SET PASSWORD,8488\r\nS,SET SAVE LOGIN INFO,On\r\nS,SET AUTOCONNECT,On\r\nS,CONNECT\r\n";
                            debug(String.Format("ADMIN: Register Client Completed: Command: {0}", command));
                            byte[] size = new byte[command.Length];
                            size = Encoding.ASCII.GetBytes(command);
                            int bytesToSend = size.Length;
                            m_sockAdmin.Send(size, bytesToSend, SocketFlags.None);
                            _registered = true;
                            #endregion
                        }

                        rawData = rawData.Substring(data.Length + 1);
                    }

                    WaitForData(Global.ADMINISTRATION_SOCKET_NAME);
                }
                catch (SocketException ex)
                {
                    debug(String.Format("ADMIN: Socket Exception: {0}", ex.Message));
                }
                catch (Exception ex)
                {
                    debug(String.Format("ADMIN: Full Exception: {0}", ex.Message));
                    throw ex;
                }
            }
            else if (result.AsyncState.ToString().Equals(Global.LEVEL_ONE_SOCKET_NAME))
            {
                try
                {
                    int bytesReceived = 0;
                    bytesReceived = m_sockIQConnect.EndReceive(result);
                    string rawData = Encoding.ASCII.GetString(m_szLevel1SocketBuffer, 0, bytesReceived);

                    string[] splitTickData = rawData.Split('\n');

                    Array.Reverse(splitTickData);
                    var sentTicks = new Dictionary<string, int>();
                    Array.ForEach(splitTickData, str =>
                    {
                        string[] actualData = str.Split(',');
                        int val;
                        if ((actualData.Length >= 15) && (!sentTicks.TryGetValue(actualData[1], out val)))
                        {
                            FireTick(actualData);
                            sentTicks.Add(actualData[1], 0);
                        }
                    });
                }
                catch (SocketException ex)
                {
                    debug(String.Format("LEVEL ONE: Socket Exception: {0}", ex.Message));
                }
                catch (Exception ex)
                {
                    debug(String.Format("LEVEL ONE: Full Exception: {0}", ex.Message));
                    debug(ex.ToString());
                }

                WaitForData(Global.LEVEL_ONE_SOCKET_NAME);
            }
        }


        private void FireTick(string[] actualData)
        {
            if (TickReceived != null)
            {
                try
                {
                    debug(String.Format("Got Tick: {0} - {1} - {2}", actualData[0], actualData[1], actualData[3]));
                    if ((actualData[0] == "P") || (actualData[0] == "Q"))
                    {
                        Tick tick = new TickImpl();
                        tick.date = Util.ToTLDate(DateTime.Now);
                        tick.time = Util.DT2FT(DateTime.Now);
                        tick.symbol = actualData[1];
                        tick.bid = Convert.ToDecimal(actualData[10]);
                        tick.ask = Convert.ToDecimal(actualData[11]);
                        tick.ex = actualData[2];
                        tick.trade = Convert.ToDecimal(actualData[3]);
                        tick.size = Convert.ToInt32(actualData[7]);
                        tick.bs = Convert.ToInt32(actualData[12]);
                        tick.os = Convert.ToInt32(actualData[13]);
                        
                        /* Josh does not prefer it done this way. If you need to pass High/Low,
                         * here is the suggestion from Josh...
                         * ------------------------------------
                         * whenenver you startup or reconnect, send DAYHIGH/DAYLOW request messages and take the response and replace  whatever current high/low is at this point.
                         * after this initial population, calculate new high/low from tick data. */

                        //tick.DayHigh = Convert.ToDecimal(actualData[8]);
                        //tick.DayLow = Convert.ToDecimal(actualData[9]);

                        TickReceived(tick);
                    }
                    //else if (actualData[0] == "F")
                    //{
                    //    Tick tick = new TickImpl();
                    //    tick.date = Util.ToTLDate(DateTime.Now);
                    //    tick.time = Util.DT2FT(DateTime.Now);
                    //    tick.symbol = actualData[1];
                    //    tick.bid = Convert.ToDecimal(0.00);
                    //    tick.ask = Convert.ToDecimal(0.00);
                    //    tick.ex = null;
                    //    tick.trade = Convert.ToDecimal(0.00);
                    //    tick.size = 1;
                    //    tick.bs = Convert.ToInt32(0.00);
                    //    tick.os = Convert.ToInt32(0.00);

                    //    TickReceived(tick);
                    //}
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                }
            }

                        


        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public event TradeLink.API.DebugDelegate SendDebug;

        

        #endregion
    }
    public class Global
    {
        public const string PROGRAM_NAME = "IQFEED-SERVER";
        public const string ADMINISTRATION_SOCKET_NAME = "Admin";
        public const string LEVEL_ONE_SOCKET_NAME = "Level1";
        public const string IP_LOCAL_HOST = "127.0.0.1";
    }

}