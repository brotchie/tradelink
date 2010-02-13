using System;
using System.Collections;

namespace TradeLink.API
{
    public interface Basket
    {

        string Name { get; set; }
        Security this[int index] { get; set; }
        int Count { get; }
        void Add(string symbol);
        void Add(string[] symbols);
        void Add(Security newsecurity);
        void Add(Basket newbasket);
        void Remove(Basket subtractbasket);
        void Remove(int i);
        void Remove(Security s);
        void Remove(string symbol);
        void Clear();
        Security[] ToArray();
        string[] ToSymArray();
        IEnumerator GetEnumerator();
    }
}
