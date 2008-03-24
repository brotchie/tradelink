using System;
using System.Collections.Generic;


namespace TradeLib
{
    public class IndexBasket : Basket
    {
        List<Index> l = new List<Index>();
        public bool hasIndex { get { return l.Count > 0; } }
        public void Add(IndexBasket ib)
        {
            for (int i = 0; i<ib.Count; i++)
                Add(ib[i]);
        }
        public void Add(Index i) { if (Index.isIdx(i.Name)) l.Add(i); }
        public override void Add(Instrument item)
        {
            if (item.isValid) l.Add((Index)item);
            
        }
        public override void Remove(int index)
        {
            l.RemoveAt(index);
        }
        public void Remove(Index i) { l.Remove(i); }
        public Index this[int i] { get { return l[i]; } set { l[i] = value; } }
        public override int Count { get { return l.Count; } }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < l.Count; i++) 
                s += l[i].Name + ",";
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
