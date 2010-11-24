using System;
using TradeLink.API;

namespace TradeLink.Common
{
    public struct Book : GotTickIndicator
    {
        public const int MAXBOOK = 40;
        public Book(Book b)
        {
            ActualDepth = b.ActualDepth;
            UpdateTime = b.UpdateTime;
            Sym = b.Sym;
            maxbook = b.maxbook;
            bidprice = new decimal[b.askprice.Length];
            bidsize = new int[b.askprice.Length];
            askprice = new decimal[b.askprice.Length];
            asksize = new int[b.askprice.Length];
            bidex = new string[b.askprice.Length];
            askex = new string[b.askprice.Length];
            Array.Copy(b.bidprice, bidprice, b.bidprice.Length);
            Array.Copy(b.bidsize, bidsize, b.bidprice.Length);
            Array.Copy(b.askprice, askprice, b.bidprice.Length);
            Array.Copy(b.asksize, asksize, b.bidprice.Length);
            for (int i = 0; i < b.ActualDepth; i++)
            {
                bidex[i] = b.bidex[i];
                askex[i] = b.askex[i];
            }
        }
        public int UpdateTime;
        public int ActualDepth;
        int maxbook;
        public Book(string sym)
        {
            UpdateTime = 0;
            ActualDepth = 0;
            maxbook = MAXBOOK;
            Sym = sym;
            bidprice = new decimal[maxbook];
            bidsize = new int[maxbook];
            askprice = new decimal[maxbook];
            asksize = new int[maxbook];
            bidex = new string[maxbook];
            askex = new string[maxbook];
        }
        public bool isValid { get { return Sym != null; } }
        public string Sym;
        public decimal[] bidprice;
        public int[] bidsize;
        public decimal[] askprice;
        public int[] asksize;
        public string[] bidex;
        public string[] askex;
        public void Reset()
        {
            ActualDepth = 0;
            for (int i = 0; i < maxbook; i++)
            {
                bidex[i] = null;
                askex[i] = null;
                bidprice[i] = 0;
                bidsize[i] = 0;
                askprice[i] = 0;
                asksize[i] = 0;
            }
        }
        public void GotTick(Tick k)
        {
            // ignore trades
            if (k.isTrade) return;
            // make sure depth is valid for this book
            if ((k.depth < 0) || (k.depth >= maxbook)) return;
            if (Sym == null)
                Sym = k.symbol;
            // make sure symbol matches
            if (k.symbol != Sym) return;
            // if depth is zero, must be a new book
            if (k.depth == 0) Reset();
            // update buy book
            if (k.hasBid)
            {
                bidprice[k.depth] = k.bid;
                bidsize[k.depth] = k.BidSize;
                bidex[k.depth] = k.be;
                if (k.depth > ActualDepth)
                    ActualDepth = k.depth;
            }
            // update sell book
            if (k.hasAsk)
            {
                askprice[k.depth] = k.ask;
                asksize[k.depth] = k.AskSize;
                askex[k.depth] = k.oe;
                if (k.depth > ActualDepth)
                    ActualDepth = k.depth;
            }
        }

        public const string EMPTYREQUESTOR = "EMPTY";
        public static string NewDOMRequest(int depthrequested) { return NewDOMRequest(EMPTYREQUESTOR, depthrequested); }
        public static string NewDOMRequest(string client, int depthrequested)
        {
            return string.Join("+", new string[] { client, depthrequested.ToString() });
        }

        public static bool ParseDOMRequest(string request, ref int depth, ref string client)
        {

            string[] r = request.Split('+');
            if (r.Length != 2) return false;
            if (!int.TryParse(r[1], out depth))
                return false;
            client = r[0];
            return true;
        }
    }
}