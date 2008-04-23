using NUnit.Framework;
using TradeLib;
using System;
using System.Collections.Generic;


namespace TestTradeLib
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
        int tltime1 = 931;
        int tltime2 = 1400;
        int tlsec = 45;
        DateTime d;

        [Test]
        public void Date()
        {
            d = Util.ToDateTime(tldate);
            Assert.That(d.Year == 2007);
            Assert.That(d.Month == 7);
            Assert.That(d.Day == 31);
            d = Util.ToDateTime(tltime1, 0);
            Assert.That(d.Hour == 9);
            Assert.That(d.Minute == 31);
            Assert.That(d.Second == 0);
            d = Util.ToDateTime(tldate, tltime2, tlsec);
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
            d = Util.ToDateTime(0);
        }


        [Test]
        public void TradesToClosedPLString()
        {
            List<Trade> tradelist = new List<Trade>();
            string s = "WAG";
            tradelist.Add(new Trade(s, 47.04m, 300)); // enter
            tradelist.Add(new Trade(s, 47.31m, 500)); // add
            tradelist.Add(new Trade(s, 47.74m, -800)); // exit

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
    }



}
