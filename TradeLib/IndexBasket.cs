using System;
using System.Collections.Generic;


namespace TradeLib
{
    public class IndexBasket
    {
        List<Index> l = new List<Index>();
        public bool hasIndex { get { return l.Count > 0; } }
        public void Add(Index i) { l.Add(i); }
        public void Remove(Index i) { l.Remove(i); }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < l.Count; i++) s += l[i].Name + ",";
            return s;
        }
    }
}
