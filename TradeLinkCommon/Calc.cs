using System;
using TradeLink.API;
using System.Collections.Generic;

namespace TradeLink.Common
{
    /// <summary>
    /// collection of calculations available in tradelink
    /// </summary>
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
        public static Order PositionProfit(Position p, OffsetInfo offset) { return PositionProfit(p, offset.ProfitDist, offset.ProfitPercent); }
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
            decimal price = OffsetPrice(p, Math.Abs(offset)*-1);
            int size = !normalizesize ? (int)(p.FlatSize * percent) : Norm2Min(p.FlatSize * percent, MINSIZE);
            o = new StopOrder(p.Symbol, !p.isLong, size, price);
            return o;
        }
        public static Order PositionStop(Position p, OffsetInfo offset) { return PositionStop(p, offset.ProfitDist, offset.ProfitPercent); }
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
        public static int[] Volumes(BarList chart, int bars)
        {
            List<int> l = new List<int>();
            for (int i = chart.Count - bars; i < chart.Count; i++)
                l.Add(chart[i].Volume);
            return l.ToArray();
        }
        /// <summary>
        /// gets volumes for ALL bars, with default bar interval
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static int[] Volumes(BarList chart) { return Volumes(chart, chart.Count); }

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
        public static decimal BollingerLower(BarList bl) { return BollingerUpper(bl, 2.5m, true); }
        /// <summary>
        /// calculates lower bollinger using opening prices.  calculate yourself for faster speed
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="numStdDevs"></param>
        /// <returns></returns>
        public static decimal BollingerLower(BarList bl, decimal numStdDevs) { return BollingerUpper(bl, numStdDevs, true); }
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

    }
}
