using System;
using System.Collections.Generic;

namespace TradeLib
{
    /// <summary>
    /// Holds collections of stock instances.
    /// </summary>
    [Serializable]
    public class MarketBasket 
    {
        public MarketBasket(string onesymbol) : this(new string[] { onesymbol }) { }
        public MarketBasket(string[] symbolist)
        {
            foreach (string s in symbolist)
                Add(new Stock(s));
        }
        public MarketBasket(Stock firststock)
        {
            Add(firststock);
        }
        public MarketBasket() { }
        public Security this [int index] { get { return symbols[index]; } set { symbols[index] = value; } }
        List<Security> symbols = new List<Security>();
        public int Count { get { return symbols.Count; } }
        public bool hasStock { get { return symbols.Count >0; } }
        public void Add(Security s) { if (s.isValid) symbols.Add((Stock)s); }
        public void Add(Stock s) { if (s.isValid) symbols.Add(s); }
        public void Add(MarketBasket mb)
        {
            for (int i = 0; i < mb.Count; i++)
                this.Add(mb[i]);
        }
        public void Subtract(MarketBasket mb)
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < symbols.Count; i++)
                for (int j = 0; j < mb.Count; j++)
                    if (symbols[i].Symbol == mb[j].Symbol)
                        remove.Add(i);
            for (int i = remove.Count - 1; i >= 0; i--)
                symbols.RemoveAt(remove[i]);
        }
        public void Remove(int i) { symbols.RemoveAt(i); }
        public void Remove(Stock s) { symbols.Remove(s); }
        public void Clear() { symbols.Clear(); }
        public Stock Get(int i) { return (Stock)symbols[i]; }
        public override string  ToString()
        {
            List<string> s = new List<string>();
            for (int i = 0; i < symbols.Count; i++) s.Add(Get(i).Symbol);
            return string.Join(",", s.ToArray());
        }
        public static MarketBasket FromString(string serialBasket)
        {
            MarketBasket mb = new MarketBasket();
            if ((serialBasket == null) || (serialBasket == "")) return mb;
            string[] r = serialBasket.Split(',');
            for (int i = 0; i < r.Length; i++)
                if ((r[i]!="") && Stock.isStock(r[i]))
                    mb.Add(new Stock(r[i]));
            return mb;
        }

    }
}
