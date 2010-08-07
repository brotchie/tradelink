using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestMessage
    {
        const string sym = "TST";
        const int size = 500;
        const decimal p = 10;
        const string ex = "NYSE";
        const decimal b = 9;
        const decimal a = 11;
        const int bs = 1;
        const int os = 2;
        [Test]
        public void EncodeDecode()
        {
            // get some data
            Tick k = TickImpl.NewTrade(sym, p, size);
            // encode it
            byte[] data = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k));
            // decode it
            Message m = Message.gotmessage(data);
            Tick c = TickImpl.Deserialize(m.Content);
            // verify
            Assert.AreEqual(k.symbol, c.symbol, TickImpl.Serialize(k) + "=>" + TickImpl.Serialize(c));
            Assert.AreEqual(k.trade, c.trade, TickImpl.Serialize(k) + "=>" + TickImpl.Serialize(c));
            Assert.AreEqual(k.size, c.size, TickImpl.Serialize(k) + "=>" + TickImpl.Serialize(c));
        }

        [Test]
        public void MultipleMessages()
        {
            // get some data
            Tick k1 = TickImpl.NewTrade(sym, p, size);
            // encode it
            byte[] data1 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k1));
            // get some data
            Tick k2 = TickImpl.NewTrade(sym, p*2, size*2);
            // encode it
            byte[] data2 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k2));
            // get some data
            Tick k3 = TickImpl.NewTrade(sym, p*3, size*3);
            // encode it
            byte[] data3 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k3));
            // prepare a continuous bitstream to hold all data
            byte[] streamlike = new byte[data1.Length + data2.Length + data3.Length];
            // roll all data into stream
            Array.Copy(data1, 0, streamlike, 0, data1.Length);
            Array.Copy(data2, 0, streamlike, data1.Length, data2.Length);
            Array.Copy(data3, 0, streamlike, data1.Length+data2.Length, data3.Length);
            // decode all messages from stream
            int offset = 0;
            Message[] msgs = Message.gotmessages(ref streamlike, ref offset);
            // ensure we got correct amount/type
            Assert.AreEqual(3, msgs.Length);
            Assert.AreEqual(MessageTypes.TICKNOTIFY, msgs[0].Type);
            Assert.AreEqual(MessageTypes.TICKNOTIFY, msgs[1].Type);
            Assert.AreEqual(MessageTypes.TICKNOTIFY, msgs[2].Type);
            // decode to ticks
            Tick c1 = TickImpl.Deserialize(msgs[0].Content);
            Tick c2 = TickImpl.Deserialize(msgs[1].Content);
            Tick c3 = TickImpl.Deserialize(msgs[2].Content);
            // verify ticks
            Assert.AreEqual(k1.symbol, c1.symbol, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k1.trade, c1.trade, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k1.size, c1.size, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k2.symbol, c2.symbol, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(k2.trade, c2.trade, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(k2.size, c2.size, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(k3.symbol, c3.symbol, TickImpl.Serialize(k3) + "=>" + TickImpl.Serialize(c3));
            Assert.AreEqual(k3.trade, c3.trade, TickImpl.Serialize(k3) + "=>" + TickImpl.Serialize(c3));
            Assert.AreEqual(k3.size, c3.size, TickImpl.Serialize(k3) + "=>" + TickImpl.Serialize(c3));
            Assert.AreEqual(0, offset);



        }

        [Test]
        public void PartialMessage()
        {
            // get some data
            Tick k1 = TickImpl.NewTrade(sym, p, size);
            // encode it
            byte[] data1 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k1));
            // get some data
            Tick k2 = TickImpl.NewTrade(sym, p * 2, size * 2);
            // encode it
            byte[] data2 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k2));
            // get some data
            Tick k3 = TickImpl.NewTrade(sym, p * 3, size * 3);
            // encode it
            byte[] data3 = Message.sendmessage(MessageTypes.TICKNOTIFY, TickImpl.Serialize(k3));
            // prepare a continuous but partial bitstream to hold most of data
            const int PARTIALSIZE = 8;
            byte[] streamlike = new byte[data1.Length + data2.Length + PARTIALSIZE];
            // roll all data into stream
            Array.Copy(data1, 0, streamlike, 0, data1.Length);
            Array.Copy(data2, 0, streamlike, data1.Length, data2.Length);
            Array.Copy(data3, 0, streamlike, data1.Length + data2.Length, streamlike.Length-data1.Length-data2.Length);
            // decode all messages from stream
            int offset = 0;
            Message[] msgs = Message.gotmessages(ref streamlike, ref offset);
            // ensure we got correct amount/type
            Assert.AreEqual(2, msgs.Length);
            Assert.AreEqual(MessageTypes.TICKNOTIFY, msgs[0].Type);
            Assert.AreEqual(MessageTypes.TICKNOTIFY, msgs[1].Type);
            // decode to ticks
            Tick c1 = TickImpl.Deserialize(msgs[0].Content);
            Tick c2 = TickImpl.Deserialize(msgs[1].Content);
            // verify ticks
            Assert.AreEqual(k1.symbol, c1.symbol, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k1.trade, c1.trade, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k1.size, c1.size, TickImpl.Serialize(k1) + "=>" + TickImpl.Serialize(c1));
            Assert.AreEqual(k2.symbol, c2.symbol, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(k2.trade, c2.trade, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(k2.size, c2.size, TickImpl.Serialize(k2) + "=>" + TickImpl.Serialize(c2));
            Assert.AreEqual(PARTIALSIZE,offset);
        }

        void debug(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
