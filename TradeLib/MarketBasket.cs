using System;
using System.Collections.Generic;
using System.Collections;

namespace TradeLib
{
    /// <summary>
    /// Holds collections of stock instances.
    /// </summary>
    [Serializable]
    public class MarketBasket 
    {
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="onesymbol">first symbol</param>
        public MarketBasket(string onesymbol) : this(new string[] { onesymbol }) { }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="symbolist">symbols</param>
        public MarketBasket(string[] symbolist)
        {
            foreach (string s in symbolist)
                Add(new Security(s));
        }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="firstsec">security</param>
        public MarketBasket(Security firstsec)
        {
            Add(firstsec);
        }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="securities"></param>
        public MarketBasket(Security[] securities)
        {
            foreach (Security s in securities)
                Add(s);
        }
        public MarketBasket() { }
        public Security this [int index] { get { return symbols[index]; } set { symbols[index] = value; } }
        List<Security> symbols = new List<Security>();
        string _name = "";
        public string Name { get { return _name; } set { _name = value; } }
        public int Count { get { return symbols.Count; } }
        public bool hasStock { get { return symbols.Count >0; } }
        public void Add(Security s) { if (s.isValid) symbols.Add(s); }
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
        public void Remove(Security s) { symbols.Remove(s); }
        public void Clear() { symbols.Clear(); }
        public Security Get(int i) { return symbols[i]; }
        public override string  ToString()
        {
            List<string> s = new List<string>();
            for (int i = 0; i < symbols.Count; i++) s.Add(this[i].FullName);
            return string.Join(",", s.ToArray());
        }
        public static MarketBasket FromString(string serialBasket)
        {
            MarketBasket mb = new MarketBasket();
            if ((serialBasket == null) || (serialBasket == "")) return mb;
            string[] r = serialBasket.Split(',');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] == "") continue;
                Security sec = Security.Parse(r[i]);
                if (sec.isValid)
                    mb.Add(sec);
            }
            return mb;
        }
        public IEnumerator GetEnumerator() { foreach (Security s in symbols) yield return s; }

    }
}
