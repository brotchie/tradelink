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
        public Stock this [int index] { get { return stocks[index]; } set { stocks[index] = value; } }
        List<Stock> stocks = new List<Stock>();
        public int Count { get { return stocks.Count; } }
        public bool hasStock { get { return stocks.Count >0; } }
        public void Add(Security s) { if (s.isValid) stocks.Add((Stock)s); }
        public void Add(Stock s) { if (s.isValid) stocks.Add(s); }
        public void Add(MarketBasket mb)
        {
            for (int i = 0; i < mb.Count; i++)
                this.Add(mb[i]);
        }
        public void Subtract(MarketBasket mb)
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < stocks.Count; i++)
                for (int j = 0; j < mb.Count; j++)
                    if (stocks[i].Symbol == mb[j].Symbol)
                        remove.Add(i);
            for (int i = remove.Count - 1; i >= 0; i--)
                stocks.RemoveAt(remove[i]);
        }
        public void Remove(int i) { stocks.RemoveAt(i); }
        public void Remove(Stock s) { stocks.Remove(s); }
        public void Clear() { stocks.Clear(); }
        public Stock Get(int i) { return (Stock)stocks[i]; }
        public override string  ToString()
        {
            List<string> s = new List<string>();
            for (int i = 0; i < stocks.Count; i++) s.Add(Get(i).Symbol);
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
