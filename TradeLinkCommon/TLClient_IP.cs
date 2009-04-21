using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TradeLink.Common
{
    /// <summary>
    /// TradeLink Clients can connect to any supported tradelink broker.
    /// version of the tradelink protocol using IP as transport.
    /// (in development)
    /// </summary>
    public class TLClient_IP
    {
        const int DEFAULTPORT = 3000;
        public int SERVERPORT = DEFAULTPORT;
        public IPAddress SERVERADDR;
        Socket socket;
        AsyncCallback packetWorker;

        public TLClient_IP() : this(IPAddress.Loopback, DEFAULTPORT) { }
        public TLClient_IP(string IpAddress, int ServerPort) : this(IPAddress.Parse(IpAddress), ServerPort) { }
        public TLClient_IP(IPAddress ServerIpAddress, int ServerPort)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SERVERADDR = ServerIpAddress;
                SERVERPORT = ServerPort;
                IPEndPoint end = new IPEndPoint(SERVERADDR, SERVERPORT);
                socket.Connect(end);
                if (socket.Connected)
                {
                    WaitSocket();
                }
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", e.Message);
            }
        }

        void WaitSocket()
        {
            try
            {
                if (packetWorker == null)
                {
                    packetWorker = new AsyncCallback(OnData);
                }
                Packet lp = new Packet();

                // Start listening to the data asynchronously
                socket.BeginReceive(lp.EncodedData,
                                                        0, lp.EncodedData.Length,
                                                        SocketFlags.None,
                                                        packetWorker,
                                                        lp);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", e.Message);
            }

        }

        void OnData(IAsyncResult iar)
        {
            try
            {
                Packet lp = (Packet)iar.AsyncState;
                int bytesread = socket.EndReceive(iar);
                Packet read = Packet.Decode(lp.EncodedData, bytesread);
                if (GotPacket!=null)
                    GotPacket(read);
                WaitSocket();
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nSocket closed\n");
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", e.Message);
            }

        }

        public void SendPacket(Packet lp)
        {
            if ((socket == null) || !socket.Connected) return;
            try
            {
                socket.Send(Packet.Encode(lp),lp.EncodedData.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", e.Message);
            }
        }

        public event PacketDelgate GotPacket;


    }
}
