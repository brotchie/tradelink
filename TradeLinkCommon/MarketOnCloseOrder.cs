using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// market on close order
    /// </summary>
    public class MOCOrder : OrderImpl
    {
        public MOCOrder(string symbol, bool side, int size)
            : base(symbol, side, System.Math.Abs(size))
        {
            this.TIF = "MOC";
            this.ex = "NYSE";
        }
    }

}
