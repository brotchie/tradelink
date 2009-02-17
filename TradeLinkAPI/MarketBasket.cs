using System;
using System.Collections;

namespace TradeLink.API
{
    public interface MarketBasket
    {

        string Name { get; set; }
        Security this[int index] { get; set; }
        int Count { get; }
        void Add(Security newsecurity);
        void Remove(int i);
        void Remove(Security s);
        void Clear();
        IEnumerator GetEnumerator();
    }
}
