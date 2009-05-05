using System;
using NUnit.Framework;
using TradeLink.Common;

namespace TestTradeLink
{
    [TestFixture]
    public class TestCalc
    {
        bool Long = true;
        bool Short = false;
        string stock = "IBM";
        decimal last = 100.45m;
        decimal entry = 99.47m;
        int lsize = 200;
        int ssize = -500;
        PositionImpl lp, sp;
        TradeImpl lc, sc;
        [SetUp]
        protected void Setup()
        {
            lp = new PositionImpl(stock, entry, lsize);
            sp = new PositionImpl(stock, entry, ssize);

            //closing trades
            lc = new TradeImpl(stock, last, lsize / -2);
            sc = new TradeImpl(stock, last, -ssize);
        }
        [Test]
        public void OpenPL()
        {
            decimal pl = .98m;
            Assert.AreEqual(pl,Calc.OpenPT(last, entry, Long));
            Assert.AreEqual(-pl,Calc.OpenPT(last, entry, Short));
            Assert.AreEqual(pl, Calc.OpenPT(last, entry, lsize));
            Assert.AreEqual(-pl,Calc.OpenPT(last, entry, ssize));
            Assert.AreEqual(pl, Calc.OpenPT(last, lp));
            Assert.AreEqual(-pl,Calc.OpenPT(last, sp));
            Assert.AreEqual(lp.Size * pl,Calc.OpenPL(last, lp) );
            Assert.AreEqual(sp.Size * pl,Calc.OpenPL(last, sp) );
        }
        [Test]
        public void ClosePL()
        {
            decimal pl = .98m;
            Assert.AreEqual(pl,Calc.ClosePT(lp, lc));
            Assert.AreEqual(-pl,Calc.ClosePT(sp, sc));
            Assert.AreEqual(pl*(lsize/2),Calc.ClosePL(lp, lc)); // matches closing size
            Assert.AreEqual((entry-last)*-ssize, Calc.ClosePL(sp, sc));
        }

        [Test]
        public void Sums()
        {
            // integer
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };
            Assert.AreEqual(15,Calc.Sum(a1, 2));
            Assert.AreEqual(16, Calc.Sum(a1));
            Assert.AreEqual(45, Calc.Sum(a1, 1, 3));

            // decimal
            decimal[] a2 = new decimal[] { 6.5m, 10.3m, 33.1m, 2, -50, 7.7m, 8 };
            Assert.AreEqual(15.7m, Calc.Sum(a2, 2));
            Assert.AreEqual(17.6, Calc.Sum(a2));
            Assert.AreEqual(45.4, Calc.Sum(a2, 1, 3));



        }

        public void SumSquares()
        {
            // integer
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };
            Assert.AreEqual(113, Calc.SumSquares(a1, 2));
            Assert.AreEqual(3842, Calc.SumSquares(a1));
            Assert.AreEqual(1193, Calc.SumSquares(a1, 1, 3));

            // decimal
            decimal[] a2 = new decimal[] { 6.5m, 10.3m, 33.1m, 2, -50, 7.7m, 8 };
            Assert.AreEqual(123.29m, Calc.SumSquares(a2, 2));
            Assert.AreEqual(3871.24m, Calc.SumSquares(a2));
            Assert.AreEqual(1205.7m, Calc.SumSquares(a2, 1, 3));
        }

        [Test]
        public void AvgAndStd()
        {
            // data
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };
            decimal[] b1 = new decimal[] { 6.4m, 10.1m, 33.33m, 2.7m, -50, 7.1m, 8 };

            // averages
            Assert.AreEqual(2.2857142857142857142857142857m, Calc.Avg(a1));
            Assert.AreEqual(2.5185714285714285714285714286m, Calc.Avg(b1));

            // stdev
            Assert.AreEqual(23.315931314473m, Calc.StdDev(a1));
            Assert.AreEqual(23.3946162479267m, Calc.StdDev(b1));

            // stdevsample
            Assert.AreEqual(25.1840788564133m, Calc.StdDevSam(a1));
            Assert.AreEqual(25.2690694320904m, Calc.StdDevSam(b1));

        }

        [Test]
        public void ArrayArrayMath()
        {
            // integer
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };
            int[] a2 = new int[] { 4,0,-3,-2,0,3,2};

            Assert.AreEqual(20, Calc.Sum(Calc.Add(a1, a2)));
            Assert.AreEqual(12, Calc.Sum(Calc.Subtract(a1, a2)));
            Assert.AreEqual(-42, Calc.Sum(Calc.Product(a1,a2)));
            Assert.AreEqual(-4.16666666666666666666666,Calc.Sum(Calc.Divide(a1,a2)));

            // decimal
            decimal [] b1 = new decimal[] { 6.4m, 10.1m, 33.33m, 2.7m, -50, 7.1m, 8 };
            decimal [] b2 = new decimal[] { 4.1m,0,-3,-2,0,3,2.7m};

            Assert.AreEqual(2243, Math.Round(Calc.Sum(Calc.Add(b1, b2))*100));
            Assert.AreEqual(1283, Math.Round(Calc.Sum(Calc.Subtract(b1, b2))*100));
            Assert.AreEqual(-3625, Math.Round(Calc.Sum(Calc.Product(b1, b2)) * 100));
            Assert.AreEqual(-557,Math.Round(Calc.Sum(Calc.Divide(b1,b2))*100));

        }

        [Test]
        public void ArrayScalarMath()
        {
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };

            Assert.AreEqual(51, Calc.Sum(Calc.Add(a1, 5)));
            Assert.AreEqual(-19, Calc.Sum(Calc.Subtract(a1, 5)));
            Assert.AreEqual(80, Calc.Sum(Calc.Product(a1, 5)));
            Assert.AreEqual(3.2m, Calc.Sum(Calc.Divide(a1, 5)));

            decimal[] b1 = new decimal[] { 6.4m, 10.1m, 33.33m, 2.7m, -50, 7.1m, 8 };
            Assert.AreEqual(56.13m, Calc.Sum(Calc.Add(b1, 5.5m)));
            Assert.AreEqual(-20.87m, Calc.Sum(Calc.Subtract(b1, 5.5m)));
            Assert.AreEqual(96.965m, Calc.Sum(Calc.Product(b1, 5.5m)));
            Assert.AreEqual(3.2054545454545454545454545454m, Calc.Sum(Calc.Divide(b1, 5.5m)));


        }

        [Test]
        public void ArraySlices()
        {
            // get input arrays
            int[] a1 = new int[] { 6, 10, 33, 2, -50, 7, 8 };
            decimal[] b1 = new decimal[] { 6.4m, 10.1m, 33.33m, 2.7m, -50, 7.1m, 8 };
            // get length of slice
            const int len = 5;
            // get slices
            int[] a2 = Calc.EndSlice(a1, len);
            decimal[] b2 = Calc.EndSlice(b1,len);
            decimal[] b3 = Calc.EndSlice(b1, 200);
            // verify lengths
            Assert.AreEqual(len, a2.Length);
            Assert.AreEqual(len, b2.Length);
            Assert.AreEqual(b1.Length, b3.Length);
            // verify last elements match
            Assert.AreEqual(a1[a1.Length - 1], a2[len - 1]);
            Assert.AreEqual(b1[b1.Length - 1], b2[len - 1]);
            // verify start elements match
            Assert.AreEqual(a1[a1.Length - len], a2[0]);
            Assert.AreEqual(b1[b1.Length - len], b2[0]);
        }

        [Test]
        public void DecimalPerformance()
        {
            DateTime start = DateTime.Now;
            const int SIZE = 250;
            const int REPS = 250;
            double EXPECT = .5;
            if (Environment.ProcessorCount == 1) EXPECT = .8;
            decimal[] a = new decimal[SIZE];
            decimal[] b = new decimal[SIZE];
            decimal[] c = new decimal[SIZE];
            for (int i = 0; i < REPS; i++)
            {
                init(ref a);
                init(ref b);
                c = Calc.Add(a, b);
                c = Calc.Subtract(a,b);
                c = Calc.Product(a,b);
                c = Calc.Divide(a,b);
                Calc.StdDev(a);
            }

            double time = DateTime.Now.Subtract(start).TotalSeconds;
            Assert.LessOrEqual(time, EXPECT);
            Console.WriteLine("DecimalPerformance (expected): " + time.ToString("N2") + "s (" + EXPECT.ToString("N2") + "), reps/sec: " + (REPS / time).ToString("N0"));
        }

        [Test]
        public void IntPerformance()
        {
            DateTime start = DateTime.Now;
            const int SIZE = 500;
            const int REPS = 500;
            const double EXPECT = .25;
            int[] a = new int[SIZE];
            int[] b = new int[SIZE];
            int[] c = new int[SIZE];
            decimal[] d = new decimal[SIZE];
            for (int i = 0; i < REPS; i++)
            {
                init(ref a);
                init(ref b);
                c = Calc.Add(a, b);
                c = Calc.Subtract(a, b);
                c = Calc.Product(a, b);
                d = Calc.Divide(a, b);
                Calc.StdDev(a);
            }

            double time = DateTime.Now.Subtract(start).TotalSeconds;
            Assert.LessOrEqual(time, EXPECT);
            Console.WriteLine("IntPerformance (expected): " + time.ToString("N2") + "s (" + EXPECT.ToString("N2") + "), reps/sec: " + (REPS / time).ToString("N0"));
        }

        void init(ref int[] a)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < a.Length; i++)
                a[i] = r.Next(0,1000);
        }

        void init(ref decimal[] a)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < a.Length; i++)
                a[i] = (decimal)r.NextDouble()*1000;
        }

    }
}
