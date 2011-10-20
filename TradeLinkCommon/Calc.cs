using System;
using TradeLink.API;
using System.Collections.Generic;
[assembly: CLSCompliant(true)]

namespace TradeLink.Common
{
    /// <summary>
    /// collection of calculations available in tradelink
    /// </summary>
    public static class Calc
    {
        /// <summary>
        /// round number to nearest decimal places (eg MINTICK)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="round2nearest"></param>
        /// <returns></returns>
        public static decimal Round2Decimals(decimal num, decimal round2nearest)
        {
            return Math.Round(num * (1 / round2nearest)) / (1 / round2nearest);
        }
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
        /// <summary>
        /// <summary>
        /// populate generic tracker with most recent TA-lib result
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="nb">number of elements (returned from ta lib)</param>
        /// <param name="res">result (returned from ta-lib)</param>
        /// <param name="gt"></param>
        /// <returns></returns>
        public static bool TAPopulateGT(int idx, int nb, ref double[] res, GenericTracker<decimal> gt)
        {
            return TAPopulateGT(idx, nb, 2, ref res, gt);
        }
        /// <summary>
        /// populate generic tracker with most recent TA-lib result
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="nb">number of elements (returned from ta lib)</param>
        /// <param name="dplaces">round to this many decimal places</param>
        /// <param name="res">result (returned from ta-lib)</param>
        /// <param name="gt"></param>
        /// <returns></returns>
        public static bool TAPopulateGT(int idx, int nb, int dplaces, ref double[] res, GenericTracker<decimal> gt)
        {
            if (nb <= 0) return false;
            gt[idx] = Math.Round(Convert.ToDecimal(res[nb - 1]), dplaces);
            return true;

        }

        /// <summary>
        /// convert double array to decimal
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static decimal[] Double2Decimal(ref double[] a)
        {
            decimal[] b = new decimal[a.Length];
            for (int i = 0; i < a.Length; i++)
                b[i] = (decimal)a[i];
            return b;
        }

        /// <summary>
        /// convert double array to decimal
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static decimal[] Double2Decimal(double[] a)
        {
            decimal[] b = new decimal[a.Length];
            for (int i = 0; i < a.Length; i++)
                b[i] = (decimal)a[i];
            return b;
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
            int closedsize = Math.Abs(adjust.xsize) > existing.UnsignedSize ? existing.UnsignedSize : Math.Abs(adjust.xsize);
            return ClosePT(existing, adjust) * closedsize;
        }

        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static int Norm2Min(decimal size, int MINSIZE) { return Norm2Min(size, MINSIZE, true); }
        public static int Norm2Min(decimal size, int MINSIZE, bool roundup)
        {
            int sign = size >= 0 ? 1 : -1;
            int mult = (int)Math.Floor(size / MINSIZE);
            if (roundup)
            {
                mult = (int)Math.Ceiling(size / MINSIZE);
            }
            
            int result = mult * MINSIZE;
            int final = sign * Math.Max(Math.Abs(result), MINSIZE);
            return final;
        }

        /// <summary>
        /// Provides an offsetting price from a position.
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
            decimal price = OffsetPrice(p,offset);
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize*percent,MINSIZE);
            o = new LimitOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        /// <summary>
        /// get profit order for given position given offset information
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, OffsetInfo offset) { return PositionProfit(p, offset.ProfitDist, offset.ProfitPercent,offset.NormalizeSize,offset.MinimumLotSize); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price.  Defaults to 100% of position.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset) { return PositionStop(p,offset,1,false,1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent) { return PositionStop(p, offset, percent, false, 1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <param name="normalizesize">whether to normalize size to even-lots</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new OrderImpl();
            if (!p.isValid || p.isFlat) return o;
            decimal price = OffsetPrice(p, offset*-1);
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize * percent, MINSIZE);
            o = new StopOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        /// <summary>
        /// get a stop order for a position given offset information
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Order PositionStop(Position p, OffsetInfo offset) { return PositionStop(p, offset.StopDist, offset.StopPercent, offset.NormalizeSize,offset.MinimumLotSize); }
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
        public static long Sum(int[] array, int barsback) { return Sum(array, array.Length - barsback, barsback); }
        /// <summary>
        /// sum part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long Sum(int[] array, int startindex, int length)
        {
            long sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i];
            return sum;
        }

        /// <summary>
        /// sum part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long Sum(long[] array, int startindex, int length)
        {
            long sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i];
            return sum;
        }
        /// <summary>
        /// gets sum of entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long Sum(long[] array) { return Sum(array, 0, array.Length); }

        /// <summary>
        /// gets sum of entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long Sum(int[] array) { return Sum(array, 0, array.Length); }
        /// <summary>
        /// gets sum of squares for end of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="barsback"></param>
        /// <returns></returns>
        public static long SumSquares(int[] array, int barsback) { return SumSquares(array, array.Length - barsback, barsback); }
        /// <summary>
        /// get sums of squares for part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long SumSquares(int[] array, int startindex, int length)
        {
            long sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i] * array[i];
            return sum;

        }

        /// <summary>
        /// get sums of squares for part of an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startindex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long SumSquares(long[] array, int startindex, int length)
        {
            long sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i] * array[i];
            return sum;

        }

        /// <summary>
        /// gets sum of squares for entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long SumSquares(int[] array) { return SumSquares(array, 0, array.Length); }

        /// <summary>
        /// gets sum of squares for entire array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long SumSquares(long[] array) { return SumSquares(array, 0, array.Length); }


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
        public static decimal Avg(long[] array) { return Avg(array, true); }
        public static decimal Avg(long[] array, bool returnzeroifEmpty)
        {
            if (returnzeroifEmpty)
            {
                if (array.Length == 0) return 0;
                return Sum(array) / array.Length;
            }
            return Sum(array) / array.Length;
        }

            

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
        public static long[] Add(long[] array1, long[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = new long[max];
            long[] s2 = new long[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 8);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 8);
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
        public static long[] AddBig(int[] array1, int[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            long[] s3 = new long[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s3[i] = s1[i] + s2[i];
            return s3;
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

        const long D2L_MULT = 65536;
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
        public static long[] Subtract(long[] array1, long[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = new long[max];
            long[] s2 = new long[max];
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
        public static long[] ProductBig(int[] array1, int[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            int[] s1 = new int[max];
            int[] s2 = new int[max];
            long[] s3 = new long[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s3[i] = s2[i] * s1[i];
            return s3;
        }

        /// <summary>
        /// multiplies two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static long[] ProductBig(long[] array1, long[] array2)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = new long[max];
            long[] s2 = new long[max];
            long[] s3 = new long[max];
            Buffer.BlockCopy(array1, 0, s1, 0, array1.Length * 4);
            Buffer.BlockCopy(array2, 0, s2, 0, array2.Length * 4);
            // calculate values
            for (int i = 0; i < s1.Length; i++)
                s3[i] = s2[i] * s1[i];
            return s3;
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
        public static decimal[] Divide(long[] array1, long[] array2) { return Divide(array1, array2, true); }
        public static decimal[] Divide(long[] array1, long[] array2, bool zeroIfundefined)
        {
            // normalize sizes of arrays
            bool a2bigger = array1.Length < array2.Length;
            int max = a2bigger ? array2.Length : array1.Length;
            long[] s1 = new long[max];
            long[] s2 = new long[max];
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
        /// <summary>
        /// adds a constant to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Add(long[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] + val;
            return r;
        }

        /// <summary>
        /// adds a constant to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long[] Add(long[] array, long val)
        {
            long[] r = new long[array.Length];
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
        /// <summary>
        /// subtracts constant from an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Subtract(long[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }

        /// <summary>
        /// subtracts constant from an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] Subtract(int[] array, int val)
        {
            int[] r = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }
        /// <summary>
        /// subtracts constant from an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long[] Subtract(long[] array, long val)
        {
            long[] r = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] - val;
            return r;
        }
        /// <summary>
        /// subtracts constant from an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>
        /// takes product of constant and an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal[] Product(long[] array, decimal val)
        {
            decimal[] r = new decimal[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] * val;
            return r;
        }
        /// <summary>
        /// takes product of constant and an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] Product(int[] array, int val)
        {
            int[] r = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] * val;
            return r;
        }
        /// <summary>
        /// takes product of constant and an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long[] Product(long[] array, long val)
        {
            long[] r = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
                r[i] = array[i] * val;
            return r;
        }

        /// <summary>
        /// takes product of constant and an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="val"></param>
        /// <returns></returns>
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

        /// <summary>
        /// gets standard deviation for values of a population
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal StdDev(long[] array)
        {
            decimal avg = Avg(array);
            decimal sq = SumSquares(array);
            decimal tmp = (sq / array.Length) - (avg * avg);
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

        /// <summary>
        /// gets standard deviation for values of a sample
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal StdDevSam(long[] array)
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

        /// <summary>
        /// Takes slice of last N elements of an array
        /// </summary>
        /// <param name="inputarray"></param>
        /// <param name="lastNumElements"></param>
        /// <returns></returns>
        public static decimal[] EndSlice(decimal[] inputarray, int lastNumElements)
        {
            int end = inputarray.Length >= lastNumElements ? inputarray.Length - lastNumElements : 0;
            decimal[] output = new decimal[inputarray.Length-end];
            int count = output.Length - 1;
            for (int i = inputarray.Length - 1; i >= end; i--)
                output[count--] = inputarray[i];
            return output;
        }

        /// <summary>
        /// takes slice of first N elements of array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static long[] Slice(long[] a, int count) { return Slice(a, 0, count); }
        /// <summary>
        /// takes slice of some N elements of array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static long[] Slice(long[] a, int start, int count)
        {
            long[] f = new long[count];
            for (int i = start; (i < (start + count)) && (i < a.Length); i++)
                f[i] = a[i];
            return f;
        }

        /// <summary>
        /// takes slice of first N elements of array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static decimal[] Slice(decimal[] input, int count) { return Slice(input, 0, count); }
        /// <summary>
        /// takes slice of any N elements of array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static decimal[] Slice(decimal[] input, int start, int count)
        {
            int len = count < input.Length ? count : input.Length;
            decimal[] o = new decimal[len];
            if (start > input.Length) return o;
            for (int i = start; i < start + len; i++)
                o[i - start] = input[i];
            return o;
        }

        /// <summary>
        /// Takes slice of last N elements of an array
        /// </summary>
        /// <param name="inputarray"></param>
        /// <param name="lastNumElements"></param>
        /// <returns></returns>
        public static int[] EndSlice(int[] inputarray, int lastNumElements)
        {
            int[] output = new int[lastNumElements];
            int count = lastNumElements-1;
            int end = inputarray.Length >= lastNumElements ? inputarray.Length - lastNumElements : 0;
            for (int i = inputarray.Length - 1; i >= end; i--)
                output[count--] = inputarray[i];
            return output;
        }

        /// <summary>
        /// Takes slice of last N elements of an array
        /// </summary>
        /// <param name="inputarray"></param>
        /// <param name="lastNumElements"></param>
        /// <returns></returns>
        public static long[] EndSlice(long[] inputarray, int lastNumElements)
        {
            long[] output = new long[lastNumElements];
            int count = lastNumElements - 1;
            int end = inputarray.Length >= lastNumElements ? inputarray.Length - lastNumElements : 0;
            for (int i = inputarray.Length - 1; i >= end; i--)
                output[count--] = inputarray[i];
            return output;
        }

        /// <summary>
        /// Returns a bardate as an array of ints in the form [year,month,day]
        /// </summary>
        /// <param name="bardate">The bardate.</param>
        /// <returns></returns>
        public static int[] Date(int bardate)
        {
            int day = bardate % 100;
            int month = ((bardate - day) / 100) % 100;
            int year = (bardate - (month * 100) - day) / 10000;
            return new int[] { year, month, day };
        }
        public static int[] Date(Bar bar) { return Date(bar.Bardate); }
        /// <summary>
        /// Returns the highest-high of the barlist, for so many bars back.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="barsback">The barsback to consider.</param>
        /// <returns></returns>
        public static decimal HH(BarList b, int barsback)
        {// gets highest high
            return Max(EndSlice(b.High(), barsback));
        }
        /// <summary>
        /// highest high of array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal HH(decimal[] array) { return Max(array); }
        /// <summary>
        /// lowest low of array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal LL(decimal[] array) { return Min(array); }
        /// <summary>
        /// Returns the highest high for the entire barlist.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static decimal HH(BarList b) { return Max(b.High()); }
        /// <summary>
        /// The lowest low for the barlist, considering so many bars back.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="barsback">The barsback to consider.</param>
        /// <returns></returns>
        public static decimal LL(BarList b, int barsback)
        { // gets lowest low
            return Min(EndSlice(b.Low(), barsback));
        }
        /// <summary>
        /// Lowest low for the entire barlist.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <returns></returns>
        public static decimal LL(BarList b) { return Min(b.Low()); }

        /// <summary>
        /// gets minum of an array (will return MinValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Min(decimal[] array)
        {
            decimal low = decimal.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] < low) low = array[i];
            return low;
        }
        /// <summary>
        /// gets minimum of an array (will return MinValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Min(int[] array)
        {
            int low = int.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] < low) low = array[i];
            return low;

        }

        /// <summary>
        /// gets minimum of an array (will return MinValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long Min(long[] array)
        {
            long low = long.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] < low) low = array[i];
            return low;

        }

        /// <summary>
        /// gets maximum in an array (will return MaxValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Max(decimal[] array)
        {
            decimal max = decimal.MinValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] > max) max= array[i];
            return max;
        }

        /// <summary>
        /// gets maximum in an array (will return MaxValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int Max(int[] array)
        {
            int max = int.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] > max) max = array[i];
            return max;
        }

        /// <summary>
        /// gets maximum in an array (will return MaxValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long Max(long[] array)
        {
            long max = long.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] > max) max = array[i];
            return max;
        }


        /// <summary>
        /// gets most recent number of highs from a barlist
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        public static decimal[] Highs(BarList chart, int bars)
        {
            return EndSlice(chart.High(), bars);
        }
        public static decimal[] Highs(BarList chart) { return chart.High(); }
        /// <summary>
        /// gets most recent lows from barlist, for certain number of bars
        /// (default is entire list)
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        public static decimal[] Lows(BarList chart, int bars)
        {
            return EndSlice(chart.Low(), bars);
        }
        /// <summary>
        /// gets ALL lows from barlist, at default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] Lows(BarList chart) { return chart.Low(); }
        /// <summary>
        /// gets opening prices for most recent bars, at default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        public static decimal[] Opens(BarList chart, int bars)
        {
            return EndSlice(chart.Open(), bars);
        }
        /// <summary>
        /// gets opening prices for ALL bars, at the default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] Opens(BarList chart) { return chart.Open(); }
        /// <summary>
        /// gets the most recent closing prices for a certain number of bars
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        public static decimal[] Closes(BarList chart, int bars)
        {
            return EndSlice(chart.Close(), bars);
        }
        /// <summary>
        /// gets most recent closing prices for ALL bars, default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] Closes(BarList chart) { return chart.Close(); }
        /// <summary>
        /// gets the most recent volumes from a barlist, given a certain number of bars back
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="bars"></param>
        /// <returns></returns>
        public static long[] Volumes(BarList chart, int bars)
        {
            List<long> l = new List<long>();
            for (int i = chart.Count - bars; i < chart.Count; i++)
                l.Add(chart[i].Volume);
            return l.ToArray();
        }
        /// <summary>
        /// gets volumes for ALL bars, with default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static long[] Volumes(BarList chart) { return Volumes(chart, chart.Count); }

        /// <summary>
        /// gets the high to low range of a barlist, for the default interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] HLRange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (BarImpl b in chart)
                l.Add(b.High - b.Low);
            return l.ToArray();
        }
        /// <summary>
        /// gets array of close to open ranges for default interval of a barlist
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] CORange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            foreach (BarImpl b in chart)
                l.Add(b.Close - b.Open);
            return l.ToArray();
        }
        /// <summary>
        /// gets an array of true range values representing each bar in chart
        /// (uses default bar interval)
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static decimal[] TrueRange(BarList chart)
        {
            List<decimal> l = new List<decimal>();
            for (int i = chart.Last; i > 0; i--)
            {
                Bar t = chart[i];
                Bar p = chart[i - 1];
                decimal max = t.High > p.Close ? t.High : p.Close;
                decimal min = t.Low < p.Close ? t.Low : p.Close;
                l.Add(max - min);
            }
            return l.ToArray();
        }
        /// <summary>
        /// downloads yearly charts for a list of symbols
        /// (source: google finance)
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static BarList[] FetchCharts(string[] symbols)
        {
            List<BarList> l = new List<BarList>();
            foreach (string sym in symbols)
            {
                BarList bl = BarListImpl.DayFromGoogle(sym);
                if (bl.isValid)
                    l.Add(bl);
            }
            return l.ToArray();
        }

        /// <summary>
        /// calculates upper bollinger using default # stdev of 2.5 and opening prices.
        /// Note, for speed it's faster to calculate these yourself.
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static decimal BollingerUpper(BarList bl) { return BollingerUpper(bl, 2.5m, true); }
        public static decimal BollingerUpper(BarList bl, decimal numStdDevs) { return BollingerUpper(bl, numStdDevs, true); }
        public static decimal BollingerUpper(BarList bl, decimal numStdDevs, bool useOpens)
        {
            decimal[] prices = useOpens ? bl.Open() : bl.Close();
            decimal mean = Avg(prices);
            decimal stdev = StdDev(prices);
            return mean + stdev * numStdDevs;
        }

        /// <summary>
        /// calculates lower bollinger using default # stdev of 2.5 and opening prices.
        /// Note, for speed it's faster to calculate these yourself.
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static decimal BollingerLower(BarList bl) { return BollingerLower(bl, 2.5m, true); }
        /// <summary>
        /// calculates lower bollinger using opening prices.  calculate yourself for faster speed
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="numStdDevs"></param>
        /// <returns></returns>
        public static decimal BollingerLower(BarList bl, decimal numStdDevs) { return BollingerLower(bl, numStdDevs, true); }
        /// <summary>
        /// calculates lower bollinger using open (true) or closing (false) prices, at specified # of standard deviations
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="numStdDevs"></param>
        /// <param name="useOpens"></param>
        /// <returns></returns>
        public static decimal BollingerLower(BarList bl, decimal numStdDevs, bool useOpens)
        {
            decimal[] prices = useOpens ? bl.Open() : bl.Close();
            decimal mean = Avg(prices);
            decimal stdev = StdDev(prices);
            return mean - stdev * numStdDevs;
        }

        /// <summary>
        /// computes sharpe ratio for a constant rate of risk free returns, give portfolio rate of return and portfolio volatility
        /// </summary>
        /// <param name="ratereturn"></param>
        /// <param name="stdevRate"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal SharpeRatio(decimal ratereturn, decimal stdevRate, decimal riskFreeRate)
        {
            return (ratereturn - riskFreeRate) / stdevRate;
        }

        /// <summary>
        /// computes sortinio ratio for constant rate of risk free return, give portfolio rate of return and downside volatility
        /// </summary>
        /// <param name="ratereturn"></param>
        /// <param name="stdevRateDownside"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal SortinoRatio(decimal ratereturn, decimal stdevRateDownside, decimal riskFreeRate)
        {
            return (ratereturn - riskFreeRate) / stdevRateDownside;
        }

        /// <summary>
        /// computes cost of a current position without taking into account side of position
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal PositionCost(Position p) { return PositionCost(p, true); }
        /// <summary>
        /// computes cost of current position without taking into account side
        /// </summary>
        /// <param name="p"></param>
        /// <param name="absolutecost"></param>
        /// <returns></returns>
        public static decimal PositionCost(Position p, bool absolutecost)
        {
            decimal calc = p.AvgPrice*p.Size;
            return absolutecost ? Math.Abs(calc) : calc;
        }

        /// <summary>
        /// computes money used to purchase a portfolio of positions.
        /// uses average price for position.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static decimal[] MoneyInUse(PositionTracker pt)
        {
            decimal [] miu = new decimal[pt.Count];
            for (int i = 0; i<pt.Count; i++)
                miu[i] += pt[i].AvgPrice * pt[i].UnsignedSize;
            return miu;
        }

        /// <summary>
        /// calculate a percentage return based upon a given amount of money used and the absolute return for this money, for each respective securtiy in a portfolio.
        /// </summary>
        /// <param name="MoneyInUse"></param>
        /// <param name="AbsoluteReturn"></param>
        /// <returns></returns>
        public static decimal[] RateOfReturn(decimal[] MoneyInUse, decimal[] AbsoluteReturn)
        {
            if (MoneyInUse.Length!=AbsoluteReturn.Length)
                throw new Exception("Money in use and Absolute return must have 1:1 security correspondence");
            decimal[] ror = new decimal[MoneyInUse.Length];
            for (int i = 0; i < MoneyInUse.Length; i++)
                ror[i] = AbsoluteReturn[i] / MoneyInUse[i];
            return ror;
        }

        /// <summary>
        /// gets absolute return of portfolio of positions at closing or market prices, or both
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="marketprices"></param>
        /// <param name="countClosedPL"></param>
        /// <param name="countOpenPL"></param>
        /// <returns></returns>
        public static decimal[] AbsoluteReturn(PositionTracker pt, decimal[] marketprices, bool countClosedPL, bool countOpenPL)
        {
            decimal [] aret = new decimal[pt.Count];
            if (countOpenPL && (pt.Count != marketprices.Length))
                throw new Exception("market prices must have 1:1 correspondence with positions in tracker.");
            for (int i = 0; i < pt.Count; i++)
            {
                if (countOpenPL)
                    aret[i] += Calc.OpenPL(marketprices[i],pt[i]);
                if (countClosedPL)
                    aret[i] += pt[i].ClosedPL;
            }
            return aret;
        }

        /// <summary>
        /// calculate absolute return only for closed portions of positions
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static decimal[] AbsoluteReturn(PositionTracker pt)
        {
            return AbsoluteReturn(pt, new GenericTracker<decimal>(0), true);

        }
        public static decimal[] AbsoluteReturn(PositionTracker pt, GenericTracker<decimal> marketprices)
        {
            return AbsoluteReturn(pt, marketprices, true);
        }
        /// <summary>
        /// returns absolute return of all positions in order they are listed in position tracker
        /// both closed and open pl may be included
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="marketprices"></param>
        /// <param name="countClosedPL"></param>
        /// <returns></returns>
        public static decimal[] AbsoluteReturn(PositionTracker pt, GenericTracker<decimal> marketprices, bool countClosedPL)
        {
            decimal[] aret = new decimal[pt.Count];
            bool countOpenPL = marketprices.Count >= pt.Count;
            for (int i = 0; i < pt.Count; i++)
            {
                // get position
                Position p = pt[i];
                // get index
                int idx = marketprices.getindex(p.Symbol);
                // see if we're doing open
                if (countOpenPL && (idx >= 0))
                    aret[i] += Calc.OpenPL(marketprices[idx], p);
                if (countClosedPL)
                    aret[i] += p.ClosedPL;
            }
            return aret;
        }

        /// <summary>
        /// calculate maximum drawdown from a PL stream for a given security/portfolio as a dollar value
        /// </summary>
        /// <param name="ret">array containing pl values for portfolio or security</param>
        /// <returns></returns>
        public static decimal MaxDDVal(decimal[] ret)
        {
            int maxi = 0;
            int prevmaxi = 0;
            int prevmini = 0;
            for (int i = 0; i < ret.Length; i++)
            {
                if (ret[i] >= ret[maxi])
                    maxi = i;
                else
                {
                    if ((ret[maxi] - ret[i]) > (ret[prevmaxi] - ret[prevmini]))
                    {
                        prevmaxi = maxi;
                        prevmini = i;
                    }
                }
            }
            return (ret[prevmini] - ret[prevmaxi]);
        }

        /// <summary>
        /// maximum drawdown as a percentage
        /// </summary>
        /// <param name="fills"></param>
        /// <returns></returns>
        public static decimal MaxDDPct(List<Trade> fills)
        {
            PositionTracker pt = new PositionTracker();
            List<decimal> ret = new List<decimal>();
            decimal mmiu = 0;
            for (int i = 0; i < fills.Count; i++)
            {
                pt.Adjust(fills[i]);
                decimal miu = Calc.Sum(Calc.MoneyInUse(pt));
                if (miu > mmiu)
                    mmiu = miu;
                ret.Add(Calc.Sum(Calc.AbsoluteReturn(pt, new decimal[0], true, false)));
            }
            decimal maxddval = MaxDDVal(ret.ToArray());
            decimal pct = mmiu== 0 ? 0 : maxddval / mmiu;
            return pct;
        }
        /// <summary>
        /// convert an array of decimals to less precise doubles
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] Decimal2Double(ref decimal[] array)
        {
            double[] vals = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                vals[i] = (double)array[i];
            return vals;
        }
        /// <summary>
        /// convert an array of decimals to less precise doubles
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] Decimal2Double(decimal[] array)
        {
            double[] vals = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                vals[i] = (double)array[i];
            return vals;
        }
        /// <summary>
        /// convert an array of decimals to less precise doubles
        /// </summary>
        /// <param name="array"></param>
        /// <param name="vals"></param>
        public static void Decimal2Double(decimal[] array, ref double[] vals)
        {
            for (int i = 0; i < array.Length; i++)
                vals[i] = (double)array[i];
        }

        /// <summary>
        /// convert an array of decimals to less precise doubles
        /// </summary>
        /// <param name="array"></param>
        /// <param name="vals"></param>
        public static void Decimal2Double(ref decimal[] array, ref double[] vals)
        {
            for (int i = 0; i < array.Length; i++)
                vals[i] = (double)array[i];
        }

        /// <summary>
        /// print an array
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        public static string parray(decimal[] prices) { return parray(prices, 0); }
        /// <summary>
        /// print an array
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static string parray(decimal[] prices, int back)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int start = prices.Length - back;
            if (start < 0)
                start = 0;
            for (int i = start; i < prices.Length; i++)
            {
                decimal c = prices[i];
                if (c != 0)
                    sb.Append(c.ToString("N2") + " ");
            }
            return sb.ToString();
        }
        public static string parray(int[] vals) { return parray(vals, 0); }
        /// <summary>
        /// print an array
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static string parray(int[] vals, int back)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int start = vals.Length - back;
            if (start < 0)
                start = 0;
            for (int i = start; i < vals.Length; i++)
            {
                decimal c = vals[i];
                if (c != 0)
                    sb.Append(c.ToString() + " ");
            }
            return sb.ToString();
        }
        /// <summary>
        /// fill an array with a value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static decimal[] fillarray(decimal val, int len)
        {
            decimal[] a = new decimal[len];
            for (int i = 0; i < len; i++)
                a[i] = val;
            return a;
        }
        /// <summary>
        /// fill an array with a value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int[] fillarray(int val, int len)
        {
            int[] a = new int[len];
            for (int i = 0; i < len; i++)
                a[i] = val;
            return a;
        }

    }
}
