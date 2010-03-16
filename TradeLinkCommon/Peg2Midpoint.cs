using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Common
{
    public class Peg2Midpoint 
    {
        public string symbol = string.Empty;
        public string ex = string.Empty;
        public long id = 0;
        public int size = 0;
        public decimal pegdiff = 0;
        public string Account = string.Empty;
        public bool isValid { get { return (symbol != string.Empty) && (size != 0); } }
        public Peg2Midpoint() { }
        public Peg2Midpoint(string Symbol, int Size, decimal Diff, string dest, long ID)
        {
            size = Size;
            ex = dest;
            id = ID;
            symbol = Symbol;
            pegdiff = Diff;
        }
        const char DELIM = ',';
        public static string Serialize(Peg2Midpoint o)
        {
            string[] r = new string[] { o.symbol, o.size.ToString(), o.ex, o.id.ToString(), o.pegdiff.ToString(),o.Account };
            return string.Join(DELIM.ToString(), r);
        }

        public static Peg2Midpoint Deserialize(string msg)
        {
            string[] r = msg.Split(DELIM);
            Peg2Midpoint pm = new Peg2Midpoint();
            if (r.Length < Enum.GetNames(typeof(MessagePeg2Midpoint)).Length)
                return pm;
            try
            {
                pm.symbol = r[(int)MessagePeg2Midpoint.Symbol];
                pm.size = Convert.ToInt32(r[(int)MessagePeg2Midpoint.Size]);
                pm.pegdiff = Convert.ToDecimal(r[(int)MessagePeg2Midpoint.PegDiff]);
                pm.id = Convert.ToInt64(r[(int)MessagePeg2Midpoint.Id]);
                pm.ex = r[(int)MessagePeg2Midpoint.Ex];
                pm.Account = r[(int)MessagePeg2Midpoint.Account];
            }
            catch { }

            return pm;

        }

        public override string ToString()
        {
            return Serialize(this);
        }
    }

    public enum MessagePeg2Midpoint
    {
        Symbol = 0,
        Size,
        Ex,
        Id,
        PegDiff,
        Account
    }


}
