using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TradeLink.Common
{
    /// <summary>
    /// tradelink servers allow tradelink clients to talk to any supported broker with common interface.
    /// this version of server supports communication with clients via windows messaging.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class TLServer_IP : TLServer
    {
        public bool SymbolSubscribed(string sym)
        {
            for (int i = 0; i < client.Count; i++)
            {
                if (client[i] == string.Empty) continue;
                else if (stocks[i].Contains(sym))
                    return true;
            }
            return false;
        }
        public Basket AllClientBasket
        {
            get
            {

                Basket b = new BasketImpl();
                for (int i = 0; i < stocks.Count; i++)
                    b.Add(BasketImpl.FromString(stocks[i]));
                return b;
            }
        }
        public string ClientName(int num) { return client[num]; }

        public string ClientSymbols(string client)
        {
            int cid = client.IndexOf(client);
            if (cid < 0) return string.Empty;
            return stocks[cid];
        }
        Providers _pn = Providers.Unknown;
        public Providers newProviderName { get { return _pn; } set { _pn = value; } }
        IPAddress _addr;

        List<Socket> _sock = new List<Socket>();


        System.ComponentModel.BackgroundWorker _at = new System.ComponentModel.BackgroundWorker();



        ~TLServer_IP()
        {
            try
            {
                Stop();

            }
            catch { }
        }



        void ReadSocket(IAsyncResult ir)
        {
            // get state
            socketinfo si = (socketinfo)ir.AsyncState;
            // get listener
            Socket list = si.sock;
            // get data from client
            Socket client = (Socket)list.EndAccept(ir);
            // get socket info for client
            socketinfo csi = new socketinfo(client,si.buffer,si.startidx);
            // get client name
            string name = client.RemoteEndPoint.ToString();
            // notify
            debug("Connection from: " + name);
            try
            {
                // receive data that arrives
                client.BeginReceive(csi.buffer, csi.startidx, csi.freebuffersize, SocketFlags.None, new AsyncCallback(ReadData), csi);
            }
            catch (SocketException ex)
            {
                v(client.RemoteEndPoint + " " + ex.SocketErrorCode + ex.Message + ex.StackTrace);
            }
            // wait for new connection
            list.BeginAccept(new AsyncCallback(ReadSocket), si);
            
        }

        public static string HexToString(byte[] buf, int len)
        {
            string Data1 = "";
            string sData = "";
            int i = 0;
            while (i < len)
            {
                //Data1 = String.Format(”{0:X}”, buf[i++]); //no joy, doesn’t pad
                Data1 = buf[i++].ToString("X").PadLeft(2, '0'); //same as “%02X” in C
                sData += Data1;
            }
            return sData;
        }


        void ReadData(IAsyncResult ir)
        {
            //get our state
            socketinfo csi = (socketinfo)ir.AsyncState;
            bool connected = csi.sock.Connected;
            try
            {
                SocketError se = SocketError.Success;
                // see how much data was read for this call
                int len = csi.sock.EndReceive(ir, out se);
                // receive any data
                if (len > 0)
                {
                    try
                    {
                        // get messages from data
                        v("srv data received: socket_status: " + se.ToString() + " data size: "+csi.freebuffersize+" data: " + HexToString(csi.buffer, len));
                        Message[] msgs = Message.gotmessages(ref csi.buffer, ref csi.startidx);
                        v("srv messages received: " + msgs.Length + " messages.  ");
                        // handle messages
                        for (int i = 0; i < msgs.Length; i++)
                        {
                            Message m = msgs[i];
                            v("srv message# " + i + " size: " + m.ByteLength + " type: " + m.Type + " tag: " + m.Tag + " data: " + m.Content);
                            handle(m.Type, m.Content, csi.sock);
                        }
                        v("srv handled " + msgs.Length + " messages.");
                    }
                    catch (Exception ex)
                    {
                        debug(ex.Message + ex.StackTrace);
                    }

                }
                else
                {
                    // implies possible disconnect, verify
                    connected = issocketconnected(csi.sock);

                }
            }
            catch (SocketException ex)
            {
                debug(ex.SocketErrorCode + ex.Message + ex.StackTrace);
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
            }
            
            try
            {
                // wait for more data to arrive
                if (connected)
                {
                    Thread.Sleep(100);
                    csi.sock.BeginReceive(csi.buffer, csi.startidx, csi.freebuffersize, SocketFlags.None, new AsyncCallback(ReadData), csi);
                }
                else
                {
                    csi.sock.Close();
                }
            }
            catch (SocketException ex)
            {
                // host likely disconnected
                v("disconnected.  msg: "+ex.SocketErrorCode + ex.Message + ex.StackTrace);
                csi.sock.Close();
            }

        }

        bool issocketconnected(Socket client) { int err; return issocketconnected(client, out err); }
        bool issocketconnected(Socket client, out int errorcode)
        {
            bool blockingState = client.Blocking;
            errorcode = 0;
            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    v("connected but send blocked.");
                    return true;
                }
                else
                {
                    
                    errorcode = e.NativeErrorCode;
                    v("disconnected, error: "+errorcode);
                    return false;
                }
            }
            finally
            {
                client.Blocking = blockingState;
            }
            return client.Connected;
        }


        Socket _list;
        IAsyncResult _myresult = null;
        public virtual void Start()
        {
            Start(3, 100, false);
        }
        public virtual void Start(int retries, int delayms, bool allowchangeport)
        {
            try
            {
                if (_started) return;
                Stop();
                debug("Starting server...");
                int attempts = 0;
                while (!_started && (attempts++ < retries))
                {
                    debug("Starting server at: " + _addr.ToString() + ":" + _port.ToString());
                    IPEndPoint end = new IPEndPoint(_addr, _port);
                    try
                    {

                        _list = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _list.Bind(end);
                        _list.Listen(MaxOustandingRequests);
                        _myresult = _list.BeginAccept(new AsyncCallback(ReadSocket), new socketinfo(_list));
                    }
                    catch (SocketException ex)
                    {
                        Stop();
                        v("start attempt #" + attempts + " failed: " + ex.Message + ex.StackTrace);
                        Thread.Sleep(delayms);
                        if (allowchangeport)
                        {
                            Random r = new Random();
                            _port += r.Next(1, 50);
                        }
                    }
                    debug("Server can handle pending requests: " + MaxOustandingRequests);
                    debug("Starting background threads to process requests and ticks.");
                    _started = _list.IsBound;
                }

                debug("Server started.");

            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
                return;
            }
            
            
        }

        bool _startedthread = false;
        void starttickthread()
        {
            if (_startedthread)
                return;

            _at = new System.ComponentModel.BackgroundWorker();
            _at.DoWork += new System.ComponentModel.DoWorkEventHandler(_at_DoWork);
            _at.WorkerSupportsCancellation = true;
            _at.RunWorkerAsync();
            _startedthread = true;
        }



        long _lastheartbeat = 0;

        void _at_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (_started)
            {
                int count = 0;
                while (tickq.hasItems)
                {
                    if (e.Cancel)
                    {
                        _started = false;
                        break;
                    }
                    Tick tick = tickq.Read();
                    sendtick(tick);
                    count++;
                    if (count % 1000 == 0)
                        checkheartbeat();
                }
                
                if (tickq.isEmpty)
                {
                    Thread.Sleep(_wait);
                    checkheartbeat();
                }
            }
        }

        void checkheartbeat()
        {
            long now = DateTime.Now.Ticks;
            long diff = (now-_lastheartbeat)*10000;
            // don't send heartbeat if not needed
            if (diff < IPUtil.SENDHEARTBEATMS)
                return;
            // otherwise attempt to send heartbeat
            for (int i = 0; i < client.Count; i++)
                if (client[i] != string.Empty)
                {
                    v("sending heartbeat to: " + client[i]+" at "+now);
                    TLSend(string.Empty, MessageTypes.HEARTBEATRESPONSE, i);
                }
            _lastheartbeat = now;
        }

        void sendtick(Tick k)
        {
            
            byte[] data = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k));
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
            {
                    if ((client[i] != null) && stocks[i].Contains(k.symbol))
                        TLSend(data, i);
            }
        }
        /// <summary>
        /// stop the server
        /// </summary>
        public virtual void Stop()
        {
            _started = false;
            _startedthread = false;
            try
            {
                for (int i = 0; i < _sock.Count; i++)
                {
                    if (_sock[i] == null)
                        continue;
                    if (!_sock[i].Connected)
                        continue;
                    _sock[i].Shutdown(SocketShutdown.Both);
                    _sock[i].Close();

                }

                if (_list.Connected)
                {
                    _list.Shutdown(SocketShutdown.Both);
                    _list.Close();
                }
                if (_at.IsBusy)
                    _at.CancelAsync();


            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
            }
            debug("Stopped: " + newProviderName);
        }

        public event StringDelegate newAcctRequest;
        public event OrderDelegateStatus newSendOrderRequest;
        public event LongDelegate newOrderCancelRequest;
        public event PositionArrayDelegate newPosList;
        public event SymbolRegisterDel newRegisterSymbols;
        public event MessageArrayDelegate newFeatureRequest;
        public event UnknownMessageDelegate newUnknownRequest;
        public event UnknownMessageDelegateSource newUnknownRequestSource;
        public event VoidDelegate newImbalanceRequest;
        public event Int64Delegate DOMRequest;

        public string Version() { return Util.TLSIdentity(); }
        protected int MinorVer = 0;

        string _name = string.Empty;
        int _wait = 5;
        public int WaitDelayMS { get { return _wait; } set { _wait = value; } }
        int _port = 0;
        public int Port { get { return _port; } }
        bool _started = false;
        public bool isStarted { get { return _started; } }
        int _maxoutstandingreq = 50;
        public int MaxOustandingRequests { get { return _maxoutstandingreq; } set { _maxoutstandingreq = value; } }

        bool _queueb4send = false;
        public bool QueueTickBeforeSend { get { return _queueb4send; } set { _queueb4send = value; } }

        RingBuffer<Tick> tickq;

        public TLServer_IP() : this(IPAddress.Loopback.ToString(), IPUtil.TLDEFAULTBASEPORT, 50,100000) { }

        public TLServer_IP(string ipaddr, int port) : this(ipaddr, port, 25, 100000) { }
        /// <summary>
        /// create an ip server
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <param name="port"></param>
        /// <param name="wait"></param>
        /// <param name="TickBufferSize">set to zero to send ticks immediately</param>
        public TLServer_IP(string ipaddr, int port, int wait, int TickBufferSize) : this(ipaddr, port, wait, TickBufferSize, null) { }
        public TLServer_IP(string ipaddr, int port, int wait, int TickBufferSize, DebugDelegate deb)
        {
            SendDebugEvent = deb;
            if (TickBufferSize == 0)
                _queueb4send = false;
            else
                tickq = new RingBuffer<Tick>(TickBufferSize);
            MinorVer = Util.ProgramBuild(Util.PROGRAM,debug);
            _wait = wait;
            if (!IPUtil.isValidAddress(ipaddr))
                debug("Not valid ip address: " + ipaddr + ", using localhost.");
            _addr = IPUtil.isValidAddress(ipaddr) ? IPAddress.Parse(ipaddr) : IPAddress.Loopback;
            _port = port;
            v("tlserver_ip wait: " + _wait);
            Start();
        }

        


        private void SrvDoExecute(string msg) // handle an order (= execute request)
        {
            Order o = OrderImpl.Deserialize(msg);
            if (newSendOrderRequest != null) 
                newSendOrderRequest(o); //request fill
            
        }


        delegate void tlneworderdelegate(OrderImpl o, bool allclients);
        public void newOrder(Order o) { newOrder(o, true); }
        public void newOrder(Order o, bool allclients)
        {
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (allclients || (stocks[i].Contains(o.symbol))))
                    TLSend(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, i);

        }

        public void newCancel(long id)
        {
            newOrderCancel(id);
        }

        // server to clients
        /// <summary>
        /// Notifies subscribed clients of a new tick.
        /// </summary>
        /// <param name="tick">The tick to include in the notification.</param>
        public void newTick(Tick tick)
        {
            if (_queueb4send)
            {
                starttickthread();
                tickq.Write(tick);
            }
            else
                sendtick(tick);

        }

        public void TLSend(string message, MessageTypes type, string clientname)
        {
            int id = client.IndexOf(clientname);
            if (id == -1) return;
            TLSend(message,type,id);
        }
        delegate void tlsenddel(string m, MessageTypes t, int dest);
        public void TLSend(string message, MessageTypes type, int  dest)
        {
            Socket s = _sock[dest];
            if (s == null) return;
            TLSend(message, type, s);
        }
        public void TLSend(string msg, MessageTypes type, Socket s)
        {
            if (s.Connected)
            {
                byte[] data = Message.sendmessage(type, msg);
#if DEBUG
                v("srv sending message type: " + type + " contents: " + msg);
                v("srv sending raw data size: " + data.Length + " data: " + HexToString(data, data.Length));
#endif

                try
                {
                    s.Send(data);
                }
                catch (Exception ex)
                {
                    debug("error sending: " + type.ToString() + " " + msg);
                    debug("exception: " + ex.Message + ex.StackTrace);
                    if (DisconnectOnError)
                    {
                        debug("disconnecting from: " + s.RemoteEndPoint.ToString());
                        s.Shutdown(SocketShutdown.Both);
                        s.Disconnect(true);
                    }
                }
            }
        }

        bool _doe = true;
        public bool DisconnectOnError { get { return _doe; } set { _doe = value; } }

        public void TLSend(byte[] data, int dest)
        {
            if ((dest<0) || (dest>=_sock.Count))
                return;

            try
            {
                Socket s = _sock[dest];
                if (s == null)
                    return;
                if (s.Connected)
                {
                    try
                    {
                        s.Send(data);
                    }
                    catch (SocketException ex)
                    {
                        debug("socket exception: " + ex.SocketErrorCode + ex.Message + ex.StackTrace);
                        handleerror(dest);
                    }
                    catch (Exception ex)
                    {
                        debug("exception sending data: " + ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (SocketException ex)
            {
                debug(ex.SocketErrorCode + ex.Message + ex.StackTrace);
                handleerror(dest);
            }


        }

        void handleerror(int dest)
        {
            if (DisconnectOnError)
            {
                debug("Disconnecting errored client: " + client[dest]);
                SrvClearClient(dest);
            }
        }

        public event DebugDelegate SendDebugEvent;

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
            
        }

        delegate void tlnewfilldelegate(TradeImpl t, bool allclients);
        /// <summary>
        /// Notifies subscribed clients of a new execution.
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        public void newFill(Trade trade) { newFill(trade, true); }
        public void newFill(Trade trade, bool allclients)
        {
            // make sure our trade is filled and initialized properly
            if (!trade.isValid)
            {
                debug("invalid trade: " + trade.ToString());
                return;
            }
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (allclients || (stocks[i].Contains(trade.symbol))))
                    TLSend(TradeImpl.Serialize(trade), MessageTypes.EXECUTENOTIFY, i);
        }

        public int NumClients { get { return client.Count; } }

        // server structures
        protected List<string> client = new List<string>();
        protected List<DateTime> heart = new List<DateTime>();
        protected List<string> stocks = new List<string>();
        protected List<string> index = new List<string>();
        

        private string SrvStocks(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return ""; // client not registered
            return stocks[cid];
        }
        private string[] SrvGetClients() { return client.ToArray(); }
        void SrvRegIndex(string cname, string idxlist)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            else index[cid] = idxlist;
            SrvBeatHeart(cname);
        }

        void SrvRegClient(string cname, Socket s)
        {
            int idx = client.IndexOf(cname) ;
            // check for already registered
            if (idx!= -1)
            {
                // update socket
                _sock[idx] = s;
                // we're done
                return; 
            }
            client.Add(cname);
            heart.Add(DateTime.Now);
            stocks.Add("");
            index.Add("");
            _sock.Add(s);
            SrvBeatHeart(cname);
        }
        
        void SrvRegStocks(string cname, string stklist)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            v("got registration request: " + stklist + " from: " + cname);
            stocks[cid] = stklist;
            SrvBeatHeart(cname);
            if (newRegisterSymbols != null)
            {
                newRegisterSymbols(cname,stklist);
            }
        }

        void SrvClearStocks(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            stocks[cid] = "";
            SrvBeatHeart(cname);
        }
        void SrvClearIdx(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            index[cid] = "";
            SrvBeatHeart(cname);
        }

        public void SrvClearClient(string client, bool niceclose)
        {
            int cid = client.IndexOf(client);
            if (cid == -1) return; // don't have this client to clear him
            SrvClearClient(cid,niceclose);
        }
        void SrvClearClient(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return; // don't have this client to clear him
            SrvClearClient(cid);
        }
        void SrvClearClient(int cid) { SrvClearClient(cid, true); }
        void SrvClearClient(int cid, bool niceclose)
        {
            if ((cid >= client.Count) || (cid < 0)) return;
            client.RemoveAt(cid);
            stocks.RemoveAt(cid);
            heart.RemoveAt(cid);
            index.RemoveAt(cid);
            try
            {
                if (niceclose)
                {
                    _sock[cid].Shutdown(SocketShutdown.Both);
                    _sock[cid].Disconnect(true);
                }
                else
                {
                    _sock[cid].Close();
                }
            }
            catch { }
            _sock.RemoveAt(cid);
        }

        void SrvBeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return; // this client isn't registered, ignore
            TimeSpan since = DateTime.Now.Subtract(heart[cid]);
            heart[cid] = DateTime.Now;
        }

        void SrvDOMReq(string cname, int depth)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            SrvBeatHeart(cname);
            if (DOMRequest!=null)
                DOMRequest(depth);
        }

        public void newOrderCancel(long orderid_being_cancled)
        {
                foreach (string c in client) // send order cancel notifcation to clients
                    TLSend(orderid_being_cancled.ToString(), MessageTypes.ORDERCANCELRESPONSE, c);
        }

        public void newImbalance(Imbalance imb)
        {
                for (int i = 0; i < client.Count; i++)
                    TLSend(ImbalanceImpl.Serialize(imb), MessageTypes.IMBALANCERESPONSE, i);
        }

        bool _noverb = true;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }
        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }

        const long NORETURNRESULT = long.MaxValue;

        long handle(Message m)
        {
            if (m.Tag == string.Empty) return 0;
            int cid = client.IndexOf(m.Tag);
            return handle(m.Type, m.Content, cid==-1 ? null : _sock[cid]);
        }

        long handle(MessageTypes type, string msg, Socket sock)
        {
            long result = NORETURNRESULT;
            v((sock!=null  ? sock.RemoteEndPoint.ToString() : string.Empty) + " " + type.ToString() + " " + msg);
            switch (type)
            {
                case MessageTypes.ACCOUNTREQUEST:
                    if (newAcctRequest == null) break;
                    string accts = newAcctRequest();
                    TLSend(accts, MessageTypes.ACCOUNTRESPONSE, sock);
                    break;
                case MessageTypes.POSITIONREQUEST:
                    if (newPosList == null) break;
                    string[] pm = msg.Split('+');
                    if (pm.Length < 2) break;
                    string client = pm[0];
                    string acct = pm[1];
                    Position[] list = newPosList(acct);
                    foreach (Position pos in list)
                        TLSend(PositionImpl.Serialize(pos), MessageTypes.POSITIONRESPONSE, client);
                    break;
                case MessageTypes.ORDERCANCELREQUEST:
                    {
                        long id = 0;
                        if (long.TryParse(msg, out id) && (newOrderCancelRequest != null))
                            newOrderCancelRequest(id);
                    }
                    break;
                case MessageTypes.SENDORDER:
                    SrvDoExecute(msg);
                    break;
                case MessageTypes.REGISTERCLIENT:
                    SrvRegClient(msg,sock);
                    break;
                case MessageTypes.REGISTERSTOCK:
                    string[] m2 = msg.Split('+');
                    SrvRegStocks(m2[0], m2[1]);
                    break;
                case MessageTypes.CLEARCLIENT:
                    SrvClearClient(msg);
                    break;
                case MessageTypes.CLEARSTOCKS:
                    SrvClearStocks(msg);
                    break;
                case MessageTypes.HEARTBEATREQUEST:
                    SrvBeatHeart(msg);
                    break;
                case MessageTypes.BROKERNAME:
                    {
                        result = (long)newProviderName;
                        sock.Send(BitConverter.GetBytes(result));
                    }
                    break;
                case MessageTypes.IMBALANCEREQUEST:
                    if (newImbalanceRequest != null) newImbalanceRequest();
                    break;
                case MessageTypes.FEATUREREQUEST:
                    string msf = "";
                    List<MessageTypes> f = new List<MessageTypes>();
                    f.Add(MessageTypes.HEARTBEATREQUEST);
                    f.Add(MessageTypes.HEARTBEATRESPONSE);
                    f.Add(MessageTypes.CLEARCLIENT);
                    f.Add(MessageTypes.CLEARSTOCKS);
                    f.Add(MessageTypes.REGISTERCLIENT);
                    f.Add(MessageTypes.REGISTERSTOCK);
                    f.Add(MessageTypes.FEATUREREQUEST);
                    f.Add(MessageTypes.FEATURERESPONSE);
                    f.Add(MessageTypes.VERSION);
                    f.Add(MessageTypes.BROKERNAME);
                    List<string> mf = new List<string>();
                    foreach (MessageTypes t in f)
                    {
                        int ti = (int)t;
                        mf.Add(ti.ToString());
                    }
                    if (newFeatureRequest != null)
                    {
                        MessageTypes[] f2 = newFeatureRequest();
                        foreach (MessageTypes t in f2)
                        {
                            int ti = (int)t;
                            mf.Add(ti.ToString());
                        }
                    }
                    msf = string.Join(",", mf.ToArray());
                    TLSend(msf, MessageTypes.FEATURERESPONSE, msg);
                    break;
                case MessageTypes.VERSION:
                    result = (long)MinorVer;
                    break;
                case MessageTypes.DOMREQUEST:
                    string[] dom = msg.Split('+');
                    SrvDOMReq(dom[0], int.Parse(dom[1]));
                    break;
                default:
                    if (newUnknownRequestSource != null)
                        result = newUnknownRequestSource(type, msg, sock.RemoteEndPoint.ToString());
                    else if (newUnknownRequest != null)
                        result = newUnknownRequest(type, msg);
                    else
                        result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
                    break;
            }

            return result;
            
        }


        internal struct socketinfo
        {
            internal socketinfo(socketinfo si)
            {
                sock = si.sock;
                startidx = si.startidx;
                buffer = si.buffer;
            }
            internal Socket sock;
            internal byte[] buffer;
            internal bool haspartial { get { return startidx != 0; } }
            internal socketinfo(Socket s)
            {
                sock = s;
                buffer = new byte[s.ReceiveBufferSize];
                startidx = 0;
            }
            internal int startidx;
            internal socketinfo(Socket s, byte[] data)
            {
                startidx = 0;
                sock = s;
                buffer = data;
            }
            internal int freebuffersize { get { return buffer.Length - startidx; } }
            internal socketinfo(Socket s, byte[] data,int offset)
            {
                startidx = offset;
                sock = s;
                buffer = data;
            }


        }





    }

    // credit : http://www.codeproject.com/KB/IP/realtimeapp.aspx
}
