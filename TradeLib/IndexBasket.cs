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
        public Index this[int i] { get { return l[i]; } set { l[i] = value; } }
        public int Count { get { return l.Count; } }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < l.Count; i++) s += l[i].Name + ",";
            return s;
        }
        public static IndexBasket FromString(string serializedbasket)
        {
            IndexBasket ib = new IndexBasket();
            string[] name = serializedbasket.Split(',');
            for (int i = 0; i < name.Length; i++)
                ib.Add(new Index(name[i]));
            return ib;
        }
    }
}
