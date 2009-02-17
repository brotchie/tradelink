using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    public class MarketResponse : DayTradeResponse
    {
        protected virtual int Read(Tick t, BarList bl) { return 0; }
        protected override Order ReadOrder(Tick t, BarList bl)
        {
            int adjust = Read(t, bl);
            OrderImpl o = Adjust(adjust);
            return o;
        }

        /// <summary>
        /// Adjusts the response's current position up or down by the specified number of shares.
        /// </summary>
        /// <param name="asize">The adjustment size. Zero for no change.</param>
        /// <returns>A market order for specified size</returns>
        protected OrderImpl Adjust(int asize)
        {
            if (asize == 0) return new OrderImpl();
            int size = asize;
            int ts = Pos.Size;
            if (Math.Abs(size) < 11) size = Calc.Norm2Min(ts * size,MINSIZE);
            Boolean side = size > 0;
            size = NoCrossingFlat(size);
            if (Math.Abs(size + ts) > MAXSIZE) size = (MAXSIZE - Math.Abs(ts)) * (side ? 1 : -1);
            OrderImpl o = new OrderImpl(Symbol, side, size, Name);
            return o;
        }

        private int NoCrossingFlat(int size)
        {
            if ((!Pos.isFlat) &&
                ((Pos.Size+ size) != 0) &&
                (((Pos.Size+ size) * Pos.Size) < 0))
                size = -1 * Pos.Size;
            return size;
        }
    }
}
