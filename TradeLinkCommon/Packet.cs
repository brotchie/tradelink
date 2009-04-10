using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;


namespace TradeLink.Common
{
    public class Packet
    {
        public MessageTypes Intention = 0;
        public string Data = "";
        public const int PACKETSIZE = 1024;
        public int From = 0;
        public int To = 0;
        public byte[] EncodedData = new byte[PACKETSIZE];
        public Packet(int source) { From = source; }
        public Packet()  { }
        public Packet(MessageTypes type, string msg) : this(0, 0, type, msg) { }
        public Packet(int source, int dest, MessageTypes type, string msg)
        {
            From = source;
            To = dest;
            Intention = type;
            Data = msg;
        }
        public static Packet Decode(Packet lp, int length)
        {
            Packet np = Decode(lp.EncodedData, length);
            return np;
        }
        public static Packet Decode(byte[] message, int length)
        {
            Packet lp = new Packet();
            if (length == 0) return lp;
            try
            {
                lp.From = BitConverter.ToInt32(message, (int)PacketOffset.source);
                lp.To = BitConverter.ToInt32(message, (int)PacketOffset.dest);
                lp.Intention = (MessageTypes)BitConverter.ToInt32(message, (int)PacketOffset.type);
            }
            catch (Exception) { lp.Intention = MessageTypes.UNKNOWN_MESSAGE; return lp; }
            ASCIIEncoding encoder = new ASCIIEncoding();
            lp.Data = encoder.GetString(message, (int)PacketOffset.message, length - (int)PacketOffset.message);
            return lp;

        }


        public static byte[] Encode(int source, int dest, MessageTypes type, string msg)
        {
            // prepare buffers
            byte[] buff = new byte[PACKETSIZE];
            // temp buffer used for encoding
            byte[] tmp;
            // source
            tmp = new byte[4];
            tmp = BitConverter.GetBytes(source);
            tmp.CopyTo(buff, (int)PacketOffset.source);
            // destination
            tmp = new byte[4];
            tmp = BitConverter.GetBytes(dest);
            tmp.CopyTo(buff, (int)PacketOffset.dest);
            // type
            tmp = new byte[4];
            tmp = BitConverter.GetBytes((int)type);
            tmp.CopyTo(buff, (int)PacketOffset.type);
            // data
            tmp = ASCIIEncoding.ASCII.GetBytes(msg);
            tmp.CopyTo(buff, (int)PacketOffset.message);
            return buff;
        }
        public static byte[] Encode(Packet lp)
        {
            return Encode(lp.From, lp.To, lp.Intention, lp.Data);
        }


    }

    enum PacketOffset
    {
        source = 0,
        dest = 4,
        type = 8,
        message = 12,
    }

}
