using NUnit.Framework;
using TradeLink.Common;
using System;
using System.Collections.Generic;
using TradeLink.API;


namespace TestTradeLink
{
    [TestFixture]
    public class TestUtil
    {

        [Test]
        public void TLDatematch()
        {
            int dd = 20070601;
            int m = 20071201;
            int m2 = 20060131;
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Year));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.Month));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.None));
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Day));
            Assert.That(Util.TLDateMatch(dd, m, DateMatchType.Day | DateMatchType.Year));
            Assert.That(!Util.TLDateMatch(dd, m, DateMatchType.Day | DateMatchType.Month));
            Assert.That(!Util.TLDateMatch(dd, m2, DateMatchType.Day | DateMatchType.Month | DateMatchType.Year));
        }
        int tldate = 20070731;
        int tltime1 = 93100;
        int tltime2 = 140045;
        DateTime d;

        [Test]
        public void Date()
        {
            d = Util.TLD2DT(tldate);
            Assert.That(d.Year == 2007);
            Assert.That(d.Month == 7);
            Assert.That(d.Day == 31);
            d = Util.TLT2DT(tltime1);
            Assert.That(d.Hour == 9);
            Assert.That(d.Minute == 31);
            Assert.That(d.Second == 0);
            d = Util.ToDateTime(tldate, tltime2);
            Assert.That(d.Year == 2007);
            Assert.That(d.Month == 7);
            Assert.That(d.Day == 31);
            Assert.That(d.Hour == 14);
            Assert.That(d.Minute == 00);
            Assert.That(d.Second == 45);
        }
        [Test]
        [ExpectedException("System.Exception")]
        public void DateException()
        {
            d = Util.TLD2DT(0);
        }


        [Test]
        public void TradesToClosedPLString()
        {
            List<Trade> tradelist = new List<Trade>();
            string s = "WAG";
            tradelist.Add(new TradeImpl(s, 47.04m, 300)); // enter
            tradelist.Add(new TradeImpl(s, 47.31m, 500)); // add
            tradelist.Add(new TradeImpl(s, 47.74m, -800)); // exit

            string[] closedpl = Util.TradesToClosedPL(tradelist);
            for (int i = 0; i<closedpl.Length; i++)
            {
                string plline = closedpl[i];
                string[] plrec = plline.Split(',');

                // check record length matches expected
                int numfields = Enum.GetNames(typeof(TradePLField)).Length;
                Assert.That(numfields == plrec.Length);

                // validate the values
                switch (i)
                {
                    case 0 :
                        Assert.That(plrec[(int)TradePLField.OpenPL] == "0.00",plrec[(int)TradePLField.OpenPL] );
                        Assert.That(plrec[(int)TradePLField.ClosedPL] == "0.00",plrec[(int)TradePLField.ClosedPL] );
                        Assert.That(plrec[(int)TradePLField.OpenSize] == "300", plrec[(int)TradePLField.OpenSize]);
                        Assert.That(plrec[(int)TradePLField.ClosedSize] == "0", plrec[(int)TradePLField.ClosedSize]);
                        Assert.That(plrec[(int)TradePLField.AvgPrice] == "47.04", plrec[(int)TradePLField.AvgPrice]);
                        break;
                    case 1 :
                        Assert.That(plrec[(int)TradePLField.OpenPL] == "81.00", plrec[(int)TradePLField.OpenPL]);
                        Assert.That(plrec[(int)TradePLField.ClosedPL] == "0.00", plrec[(int)TradePLField.ClosedPL]);
                        Assert.That(plrec[(int)TradePLField.OpenSize] == "800", plrec[(int)TradePLField.OpenSize]);
                        Assert.That(plrec[(int)TradePLField.ClosedSize] == "0", plrec[(int)TradePLField.ClosedSize]);
                        Assert.That(plrec[(int)TradePLField.AvgPrice] == "47.21", plrec[(int)TradePLField.AvgPrice]);
                        break;
                    case 2 :
                        Assert.That(plrec[(int)TradePLField.OpenPL] == "0.00", plrec[(int)TradePLField.OpenPL]);
                        Assert.That(plrec[(int)TradePLField.ClosedPL] == "425.00", plrec[(int)TradePLField.ClosedPL]);
                        Assert.That(plrec[(int)TradePLField.OpenSize] == "0", plrec[(int)TradePLField.OpenSize]);
                        Assert.That(plrec[(int)TradePLField.ClosedSize] == "-800", plrec[(int)TradePLField.ClosedSize]);
                        Assert.That(plrec[(int)TradePLField.AvgPrice] == "0.00", plrec[(int)TradePLField.AvgPrice]);
                        break;
                }
            }
        }

        [Test]
        public void FastTime()
        {
            // dt2ft
            int h = 23;
            int m = 59;
            int s = 48;
            DateTime now = new DateTime(1, 1, 1, h,m,s);
            int ft = Util.DT2FT(now);
            Assert.AreEqual(235948, ft);

            // f2fts

            int span = Util.FT2FTS(ft);
            Assert.AreEqual(h * 60 * 60 + m * 60 + s, span);

            // ft2dt

            DateTime next = Util.FT2DT(ft);
            Assert.AreEqual(now.Hour, next.Hour);
            Assert.AreEqual(now.Minute, next.Minute);
            Assert.AreEqual(now.Second, next.Second);

            // diff (subtraction => fast timespan)

            int t1 = 115100;
            int t2 = 231408;
            span = Util.FTDIFF(t1, t2);
            Assert.AreEqual(11*60*60+23*60+8, span);

            // addition (addition of fastime and fasttimespan => fasttime)
            t1 = 133709;
            span = 300 * 60; // 300 minutes
            int endtime = Util.FTADD(t1, span);
            Assert.AreEqual(183709, endtime);



        }

        [Test]
        public void TickIndex()
        {

            string[,] idx = Util.TickFileIndex(Environment.CurrentDirectory + "\\", "*.epf");
            string[] syma = new string[] { "ABN", "$SPX", "FTI" };
            string syms = string.Join(",", syma);
            bool foundsym = true;
            for (int i = 0; i < idx.GetLength(0); i++)
            {
                Security s = Util.SecurityFromFileName(idx[i, 0]);
                foundsym &= syms.Contains(s.Symbol);
            }
            Assert.IsTrue(foundsym);
            Assert.AreEqual(syma.Length, idx.GetLength(0));
            Assert.AreEqual(2, idx.GetLength(1));


        }

    }



}
