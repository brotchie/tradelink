using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;

namespace TradeLink.Common
{
    public delegate void PacketDelgate(Packet lp);
    /// <summary>
    /// TradeLink servers provide common interface for clients to communicate with all brokers.
    /// This version supports the tradelink protocol over IP transport.
    /// (in development)
    /// </summary>
    public class TLServer_IP
    {
        Socket sock;
        Thread listenThread;
        AsyncCallback packetWorker;

        int clients = 0;
        Socket[] client = new Socket[MAXCLIENT];
        const int DEFAULTPORT = 3000;
        const int MAXCLIENT = 100;
        public int PORT = DEFAULTPORT;
        public int SERVERID = 0;

        public TLServer_IP() : this(DEFAULTPORT) { }
        public TLServer_IP(int CUSTOMPORT)
        {
            PORT = CUSTOMPORT;
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        public event PacketDelgate GotPacket;

        private void ListenForClients()
        {
            // build and bind to socket
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, PORT);
            sock.Bind(ipLocal);
            sock.Listen(4);
            // wait for clients
            sock.BeginAccept(new AsyncCallback(OnClientConnect), null);

        }
        byte[] _buff = new byte[Packet.PACKETSIZE];

        void OnClientConnect(IAsyncResult iar)
        {
            try
            {
                // client socket has arrived
                Socket workerSocket = sock.EndAccept(iar);

                // count the client
                Interlocked.Increment(ref clients);

                if (clients >= MAXCLIENT)
                    throw new Exception("maximum number of clients exceeded!");

                 // save socket for future communication
                client[clients] = workerSocket;

                // Send a welcome message to client
                //string msg = "Welcome client " + m_clientCount + "\n";
                //SendMsgToClient(msg, m_clientCount);

                // process remaning packet on this connection
                WaitSocket(clients);

                // wait for new client
                sock.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnect: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                System.Diagnostics.Debugger.Log(0,"1",se.Message);
            }

        }

        void WaitSocket(int numclient)
        {
            try
            {
                if (packetWorker == null)
                {
                    // Specify the call back function which is to be 
                    // invoked when there is any write activity by the 
                    // connected client
                    packetWorker = new AsyncCallback(OnData);
                }
                Packet lp = new Packet(numclient);
                Socket soc = client[numclient];

                soc.BeginReceive(lp.EncodedData, 0,
                    lp.EncodedData.Length,
                    SocketFlags.None,
                    packetWorker,
                    lp);
            }
            catch (SocketException se)
            {
                System.Diagnostics.Debugger.Log(0, "1", se.Message);
            }
        }


        void OnData(IAsyncResult iar)
        {
            Packet lp = (Packet)iar.AsyncState;
            try
            {
                // read in the result
                int bytesread = client[lp.From].EndReceive(iar);
                Packet read = Packet.Decode(lp, bytesread);
                read.From = lp.From;

                if (GotPacket != null) 
                    GotPacket(read);

                // wait for new data
                WaitSocket(lp.From);

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnData: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) 
                {
                    System.Diagnostics.Debugger.Log(0, "1", "\nClient "+lp.From+": connection reset by peer\n");
                    // reclaim socket
                    client[lp.From] = null;
                }
                else
                {
                    System.Diagnostics.Debugger.Log(0, "1", se.Message);
                }
            }


        }

        public void SendPacket(Packet lp)
        {
            Socket sock = client[lp.To];
            if ((sock == null) || !sock.Connected) return;
            sock.Send(Packet.Encode(lp));
        }

        List<Listener> _listeners = new List<Listener>();
        List<List<uint>> _listroute = new List<List<uint>>();
        void RegisterListener(Listener lis)
        {
            
        }
    }

    //http://www.codeguru.com/csharp/csharp/cs_network/sockets/article.php/c7695
}
