using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    /// <summary>
    /// Abstract container for instruments
    /// </summary>
    public abstract class Basket
    {
        public abstract void Add(Instrument item);
        public abstract void Remove(int index);
        public abstract int Count { get; }
    }
}
