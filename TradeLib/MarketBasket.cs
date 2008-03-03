using System;
using System.Collections.Generic;

namespace TradeLib
{
    [Serializable]
    public class MarketBasket
    {
        public MarketBasket(Stock firststock)
        {
            Add(firststock);
        }
        public MarketBasket() { }
        public Stock this [int index] { get { return stocks[index]; } set { stocks[index] = value; } }
        List<Stock> stocks = new List<Stock>();
        public int Count { get { return stocks.Count; } }
        public bool hasStock { get { return stocks.Count >0; } }
        public void Add(Stock s) { stocks.Add(s); }
        public void Add(MarketBasket mb)
        {
            for (int i = 0; i < mb.Count; i++)
                this.Add(mb[i]);
        }
        public void Remove(Stock s) { stocks.Remove(s); }
        public void Clear() { stocks.Clear(); }
        public Stock Get(int i) { return (Stock)stocks[i]; }
        public override string  ToString()
        {
            string s = "";
            for (int i = 0; i < stocks.Count; i++) s += Get(i) + ",";
            return s;
        }
        public static MarketBasket FromString(string serialBasket)
        {
            MarketBasket mb = new MarketBasket();
            if ((serialBasket == null) || (serialBasket == "")) return mb;
            string[] r = serialBasket.Split(',');
            for (int i = 0; i < r.Length; i++)
                mb.Add(new Stock(r[i]));
            return mb;
        }

    }
}
