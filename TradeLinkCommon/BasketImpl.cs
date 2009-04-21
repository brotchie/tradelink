using System;
using System.Collections.Generic;
using System.Collections;
using TradeLink.API;
using System.IO;

namespace TradeLink.Common
{
    /// <summary>
    /// Holds collections of securities.
    /// </summary>
    [Serializable]
    public class BasketImpl : TradeLink.API.Basket
    {
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="onesymbol">first symbol</param>
        public BasketImpl(string onesymbol) : this(new string[] { onesymbol }) { }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="symbolist">symbols</param>
        public BasketImpl(string[] symbolist)
        {
            foreach (string s in symbolist)
                Add(new SecurityImpl(s));
        }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="firstsec">security</param>
        public BasketImpl(SecurityImpl firstsec)
        {
            Add(firstsec);
        }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="securities"></param>
        public BasketImpl(SecurityImpl[] securities)
        {
            foreach (SecurityImpl s in securities)
                Add(s);
        }
        public BasketImpl() { }
        public Security this [int index] { get { return symbols[index]; } set { symbols[index] = value; } }
        List<Security> symbols = new List<Security>();
        string _name = "";
        public string Name { get { return _name; } set { _name = value; } }
        public int Count { get { return symbols.Count; } }
        public bool hasStock { get { return symbols.Count >0; } }
        public void Add(Security s) { if (s.isValid) symbols.Add(s); }
        public void Add(Basket mb)
        {
            for (int i = 0; i < mb.Count; i++)
                this.Add(mb[i]);
        }
        public void Remove(Basket mb)
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
        public static string Serialize(Basket b)
        {
            List<string> s = new List<string>();
            for (int i = 0; i < b.Count; i++) s.Add(b[i].FullName);
            return string.Join(",", s.ToArray());
        }

        public static BasketImpl Deserialize(string serialBasket)
        {
            BasketImpl mb = new BasketImpl();
            if ((serialBasket == null) || (serialBasket == "")) return mb;
            string[] r = serialBasket.Split(',');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] == "") continue;
                SecurityImpl sec = SecurityImpl.Parse(r[i]);
                if (sec.isValid)
                    mb.Add(sec);
            }
            return mb;
        }
        public static Basket FromFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string file = sr.ReadToEnd();
            sr.Close();
            string[] syms = file.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            BasketImpl b = new BasketImpl(syms);
            return b;
        }

        public static void ToFile(Basket b, string filename) { ToFile(b, filename, false); }
        public static void ToFile(Basket b, string filename, bool append)
        {
            StreamWriter sw = new StreamWriter(filename, append);
            for (int i = 0; i < b.Count; i++)
                sw.WriteLine(b[i].Symbol);
            sw.Close();

        }
        public override string ToString() { return Serialize(this); }
        public static BasketImpl FromString(string serialbasket) { return Deserialize(serialbasket); }
        public IEnumerator GetEnumerator() { foreach (SecurityImpl s in symbols) yield return s; }

    }
}
