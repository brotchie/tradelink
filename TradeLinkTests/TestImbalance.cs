using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using NUnit.Framework;

namespace TestTradeLink
{
    [TestFixture]
    public class TestImbalance
    {
        public TestImbalance() {}

        [Test]
        public void SerializeDeserilize()
        {
            // setup imbalance
            const string s = "IBM";
            const string e = "NYSE";
            const int x = 500000;
            const int t = 1550;
            const int p = 1540;
            const int px = 400000;
            Imbalance i = new ImbalanceImpl(s, e, x, t, px, p,999);

            // verify it's valid
            Assert.IsTrue(i.isValid);

            // serialize it
            string msg = ImbalanceImpl.Serialize(i);
            // deserialize it somewhere else
            Imbalance ni = ImbalanceImpl.Deserialize(msg);

            // make sure it's valid
            Assert.IsTrue(ni.isValid);

            // verify it's the same
            Assert.AreEqual(i.Symbol, ni.Symbol);
            Assert.AreEqual(i.Exchange, ni.Exchange);
            Assert.AreEqual(i.PrevImbalance, ni.PrevImbalance);
            Assert.AreEqual(i.PrevTime, ni.PrevTime);
            Assert.AreEqual(i.ThisImbalance, ni.ThisImbalance);
            Assert.AreEqual(i.ThisTime, ni.ThisTime);
            Assert.AreEqual(i.InfoImbalance, ni.InfoImbalance);
        }

        public static Imbalance[] SampleImbalanceData(int count)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            string[] syms = TradeLink.Research.RandomSymbol.GetSymbols((int)DateTime.Now.Ticks, 4,count);
            Imbalance[] imbs = new Imbalance[syms.Length];
            for (int j = 0; j < syms.Length; j++)
            {
                imbs[j] = new ImbalanceImpl(syms[j], "NYSE", r.Next(-1000, 1000) * r.Next(1000), 1550, r.Next(-1000, 1000) * r.Next(1000), 1540, 0);
            }
            return imbs;

        }

        [Test]
        public void Performance()
        {
            
            const int OPS = 10000;

            Imbalance[] imbs = SampleImbalanceData(OPS);
            DateTime start = DateTime.Now;
            bool v = true;
            for (int i = 0; i < OPS; i++)
            {
                Imbalance im = ImbalanceImpl.Deserialize(ImbalanceImpl.Serialize(imbs[i]));
                v &= im.ThisImbalance == imbs[i].ThisImbalance;
            }

            double time = DateTime.Now.Subtract(start).TotalSeconds;
            Assert.IsTrue(v);
            Assert.LessOrEqual(time,.15);
            Console.WriteLine(string.Format("Imbalance performance: {0:n2} {1:n0}i/s",time,OPS/time));
        }



    }
}
