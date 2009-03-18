using System;
using TradeLink.API;


namespace TradeLink.Common
{
    public static class Calc
    {
        /// <summary>
        /// Gets the open PL on a per-share basis, ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return (PosSize == 0) ? 0 : OpenPT(LastTrade, AvgPrice, PosSize > 0);
        }
        /// <summary>
        /// Gets the open PL on a per-share basis (also called points or PT), ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="Side">if set to <c>true</c> [side].</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, bool Side)
        {
            return Side ? LastTrade - AvgPrice : AvgPrice - LastTrade;
        }
        public static decimal OpenPT(decimal LastTrade, Position Pos)
        {
            return OpenPT(LastTrade, Pos.AvgPrice, Pos.Size);
        }
        /// <summary>
        /// Gets the open PL considering all the shares held in a position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPL(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return PosSize * (LastTrade - AvgPrice);
        }
        public static decimal OpenPL(decimal LastTrade, Position Pos)
        {
            return OpenPL(LastTrade, Pos.AvgPrice, Pos.Size);
        }

        // these are for calculating closed pl
        // they do not adjust positions themselves
        /// <summary>
        /// Gets the closed PL on a per-share basis, ignoring how many shares are held.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position that's being closed/changed.</param>
        /// <returns></returns>
        public static decimal ClosePT(Position existing, Trade adjust)
        {
            if (!existing.isValid || !adjust.isValid) 
                throw new Exception("Invalid position provided. (existing:" + existing.ToString() + " adjustment:" + adjust.ToString());
            if (existing.isFlat) return 0; // nothing to close
            if (existing.isLong == adjust.side) return 0; // if we're adding, nothing to close
            return existing.isLong ? adjust.xprice- existing.AvgPrice: existing.AvgPrice- adjust.xprice;
        }

        /// <summary>
        /// Gets the closed PL on a position basis, the PL that is registered to the account for the entire shares transacted.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position being changed/closed.</param>
        /// <returns></returns>
        public static decimal ClosePL(Position existing, Trade adjust)
        {
            int closedsize = Math.Abs(adjust.xsize + existing.Size);
            return ClosePT(existing, adjust) * (closedsize==0 ? Math.Abs(adjust.xsize) : closedsize);
        }

        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static int Norm2Min(decimal size,int MINSIZE)
        {
            int wmult = (int)Math.Ceiling(size / MINSIZE);
            return wmult * MINSIZE;
        }

        /// <summary>
        /// Provides an offsetting price from a position.
        /// Positive offset will be a profit offset, negative offset will be stop.
        /// </summary>
        /// <param name="p">Position</param>
        /// <param name="offset">Offset amount</param>
        /// <returns>Offset price</returns>
        public static decimal OffsetPrice(Position p, decimal offset) { return OffsetPrice(p.AvgPrice, p.isLong, offset); }
        public static decimal OffsetPrice(decimal AvgPrice, bool side, decimal offset)
        {
            return side ? AvgPrice + offset : AvgPrice - offset;
        }
        /// <summary>
        /// Defaults to 100% of position at target.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">your target</param>
        /// <returns>profit taking limit order</returns>
        public static Order PositionProfit(Position p, decimal offset) { return PositionProfit(p, offset, 1, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent) { return PositionProfit(p, offset, percent, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <param name="normalizesize">whether to normalize order to be an even-lot trade</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new OrderImpl();
            if (!p.isValid || p.isFlat) return o;
            decimal price = OffsetPrice(p,Math.Abs(offset));
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize*percent,MINSIZE);
            o = new LimitOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price.  Defaults to 100% of position.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <returns></returns>
        public static Order PositionStop(PositionImpl p, decimal offset) { return PositionStop(p,offset,1,false,1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <returns></returns>
        public static Order PositionStop(PositionImpl p, decimal offset, decimal percent) { return PositionStop(p, offset, percent, false, 1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <param name="normalizesize">whether to normalize size to even-lots</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionStop(PositionImpl p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new OrderImpl();
            if (!p.isValid || p.isFlat) return o;
            decimal price = OffsetPrice(p, Math.Abs(offset)*-1);
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize * percent, MINSIZE);
            o = new StopOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        /// <summary>
        /// sum last elements of array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="barsback"></param>
        /// <returns></returns>
        public static decimal Sum(decimal[] array, int barsback) { return Sum(array, array.Length - barsback, barsback); }
        /// <summary>
        /// sum part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static decimal Sum(decimal[] array, int startindex, int length)
        {
            decimal sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i];
            return sum;
        }
        /// <summary>
        /// gets sum of entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Sum(decimal[] array) { return Sum(array, 0, array.Length); }
        /// <summary>
        /// gets sum of squares for end of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="barsback"></param>
        /// <returns></returns>
        public static decimal SumSquares(decimal[] array, int barsback) { return SumSquares(array, array.Length - barsback, barsback); }
        /// <summary>
        /// get sums of squares for part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static decimal SumSquares(decimal[] array, int startindex, int length)
        {
            decimal sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i]*array[i];
            return sum;
        }

        /// <summary>
        /// gets sum of squares for entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal SumSquares(decimal[] array) { return SumSquares(array, 0, array.Length); }

        /// <summary>
        /// sum last elements of array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="barsback"></param>
        /// <returns></returns>
        public static int Sum(int[] array, int barsback) { return Sum(array, array.Length - barsback, barsback); }
        /// <summary>
        /// sum part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int Sum(int[] array, int startindex, int length)
        {
            int sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i];
            return sum;
        }
        /// <summary>
        /// gets sum of entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Sum(int[] array) { return Sum(array, 0, array.Length); }
        /// <summary>
        /// gets sum of squares for end of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="barsback"></param>
        /// <returns></returns>
        public static int SumSquares(int[] array, int barsback) { return SumSquares(array, array.Length - barsback, barsback); }
        /// <summary>
        /// get sums of squares for part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int SumSquares(int[] array, int startindex, int length)
        {
            int sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i] * array[i];
            return sum;

        }

        /// <summary>
        /// gets sum of squares for entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int SumSquares(int[] array) { return SumSquares(array, 0, array.Length); }

        /// <summary>
        /// gets mean of an array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Avg(decimal[] array) { return Avg(array, true); }
        /// <summary>
        /// gets mean of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="returnzeroIfempty"></param>
        /// <returns></returns>
        public static decimal Avg(decimal[] array, bool returnzeroIfempty) { return array.Length == 0 ? 0 : Sum(array) / array.Length; }
        public static decimal Avg(int[] array ) { return Avg(array,true); }
        public static decimal Avg(int[] array, bool returnzeroIfempty) { return array.Length == 0 ? 0 : (decimal)Sum(array) / array.Length; }

        /// <summary>
        /// adds two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static int[] Add(int[] array1, int[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length ;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            Buffer.BlockCopy(array1,0,s1,0,array1.Length*4);
            Buffer.BlockCopy(array2,0,s2,0,array2.Length*4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s2[i] += s1[i];
            return s2;
        }

        /// <summary>
        /// adds two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static decimal[] Add(decimal[] array1, decimal[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = decimal2long(array1, max);
            long[] s2 = decimal2long(array2, max);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s2[i] += s1[i];
            return long2decimal(s2);
        }

        const uint D2L_MULT = 65536;
        static long[] decimal2long(decimal[] a) { return decimal2long(a, a.Length); }
        static long[] decimal2long(decimal[] a, int length)
        {
            long[] r = new long[length];
            for (int i = 0; i < length; i++)
                r[i] = (long)(a[i] * D2L_MULT);
            return r;
        }

        static decimal[] long2decimal(long[] a) { return long2decimal(a, a.Length); }
        static decimal[] long2decimal(long[] a, int length)
        {
            decimal[] r = new decimal[length];
            for (int i = 0; i < length; i++)
                r[i] = ((decimal)a[i] / D2L_MULT);
            return r;
        }

        static decimal[] long2decimalp(long[] a)
        {
            decimal[] r = new decimal[a.Length];
            long m = D2L_MULT*(long)D2L_MULT;
            for (int i = 0; i < a.Length; i++)
                r[i] = ((decimal)a[i] / m);
            return r;
        }

        /// <summary>
        /// subtracts 2nd array from first array
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static int[] Subtract(int[] array1, int[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s1[i] -= s2[i];
            return s1;
        }

        /// <summary>
        /// subtracts 2nd array from first array
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static decimal[] Subtract(decimal[] array1, decimal[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = decimal2long(array1, max);
            long[] s2 = decimal2long(array2, max);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s1[i] -= s2[i];
            return long2decimal(s1);
        }

        /// <summary>
        /// multiplies two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static int[] Product(int[] array1, int[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s2[i] *= s1[i];
            return s2;
        }

        /// <summary>
        /// multiplies two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static decimal[] Product(decimal[] array1, decimal[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = decimal2long(array1, max);
            long[] s2 = decimal2long(array2, max);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s2[i] *= s1[i];
            return long2decimalp(s2);
        }

        /// <summary>
        /// divides first array by second array.  
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static decimal[] Divide(int[] array1, int[] array2) { return Divide(array1,array2, true); }
        public static decimal[] Divide(int[] array1, int[] array2,bool zeroIfundefined)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            decimal[] r = new decimal[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                if (zeroIfundefined)
                    r[i] = s2[i] != 0 ? (decimal)s1[i] / s2[i] : 0;
                else 
                    r[i] = (decimal)s1[i] / s2[i];
            return r;
        }

        /// <summary>
        /// divides first array by second array.  
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static decimal[] Divide(decimal[] array1, decimal[] array2) { return Divide(array1, array2, true); }
        public static decimal[] Divide(decimal[] array1, decimal[] array2, bool zeroIfundefined)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = decimal2long(array1, max);
            long[] s2 = decimal2long(array2, max);
            decimal[] r = new decimal[max];
            // calculate values
            for (int i = 0; i < s1.Length; i++)
            {
                if (zeroIfundefined)
                    r[i] = s2[i] != 0 ? (decimal)s1[i] / s2[i] : 0;
                else
                    r[i] = (decimal)s1[i] / s2[i];
            }
            return r;
        }
        /// <summary>
        /// adds a constant to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Add(int[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] + val;
            return r;
        }
        public static int[] Add(int[] array, int val)
        {
            int[] r = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] + val;
            return r;
        }
        public static decimal[] Add(decimal[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] + val;
            return r;
        }

        /// <summary>
        /// subtracts constant from an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Subtract(int[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }
        public static int[] Subtract(int[] array, int val)
        {
            int[] r = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }
        public static decimal[] Subtract(decimal[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }


        /// <summary>
        /// takes product of constant and an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Product(int[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i]*val;
            return r;
        }
        public static int[] Product(int[] array, int val)
        {
            int[] r = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] * val;
            return r;
        }
        public static decimal[] Product(decimal[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] * val;
            return r;
        }
        /// <summary>
        /// divides array by a constant
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Divide(int[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = (decimal)array[i] / val;
            return r;
        }
        public static decimal[] Divide(decimal[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = (decimal)array[i] / val;
            return r;
        }
        /// <summary>
        /// gets standard deviation for values of a population
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal StdDev(int[] array)
        {
            decimal avg = Avg(array);
            decimal sq = SumSquares(array);
            decimal tmp = (sq/ array.Length) - (avg * avg);
            decimal stdev = (decimal)Math.Pow((double)tmp, .5);
            return stdev;
        }

        public static decimal StdDev(decimal[] array)
        {
            decimal avg = Avg(array);
            decimal sq = SumSquares(array);
            decimal tmp = (sq / array.Length) - (avg * avg);
            decimal stdev = (decimal)Math.Pow((double)tmp, .5);
            return stdev;

        }
        /// <summary>
        /// gets standard deviation for values of a sample
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal StdDevSam(int[] array)
        {
            decimal avg = Avg(array);
            decimal[] var = Subtract(array, avg);
            decimal[] varsq = Product(var, var);
            decimal sumvar = Sum(varsq);
            decimal stdev = (decimal)Math.Pow((double)sumvar / (array.Length - 1), .5);
            return stdev;
        }

        public static decimal StdDevSam(decimal[] array)
        {
            decimal avg = Avg(array);
            decimal[] var = Subtract(array, avg);
            decimal[] varsq = Product(var, var);
            decimal sumvar = Sum(varsq);
            decimal stdev = (decimal)Math.Pow((double)sumvar / (array.Length-1),.5);
            return stdev;
        }

    }
}
