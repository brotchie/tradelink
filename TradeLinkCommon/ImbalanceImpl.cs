using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// receive imbalance information via tradelink
    /// </summary>
    public struct ImbalanceImpl : Imbalance
    {
        public ImbalanceImpl(string sym, string ex, int size, int time, int psize, int ptime, int info)
        {
            _time = time;
            _size = size;
            _sym = sym;
            _ex = ex;
            _ptime = ptime;
            _psize = psize;
            _info = info;
        }
        string _sym;
        string _ex;
        int _size;
        int _psize;
        int _time;
        int _ptime;
        int _info;
        public bool isValid { get { return (_sym != ""); } }
        public bool hasImbalance { get { return _size != 0; } }
        public bool hadImbalance { get { return _psize != 0; } }
        public string Symbol { get { return _sym; } }
        public string Exchange { get { return _ex; } }
        public int InfoImbalance { get { return _info; } }
        public int ThisImbalance { get { return _size; } }
        public int PrevImbalance { get { return _psize; } }
        public int ThisTime { get { return _time; } }
        public int PrevTime { get { return _ptime; } }

        public static Imbalance Deserialize(string msg)
        {
            ImbalanceImpl i = new ImbalanceImpl();
            string[] r = msg.Split(',');
            int v = 0;
            if (int.TryParse(r[(int)ImbalanceField.IF_SIZE], out v))
                i._size = v;
            if (int.TryParse(r[(int)ImbalanceField.IF_TIME], out v))
                i._time = v;
            if (int.TryParse(r[(int)ImbalanceField.IF_PSIZE], out v))
                i._psize = v;
            if (int.TryParse(r[(int)ImbalanceField.IF_PTIME], out v))
                i._ptime = v;
            if (int.TryParse(r[(int)ImbalanceField.IF_INFO], out v))
                i._info = v;
            i._sym= r[(int)ImbalanceField.IF_SYM];
            i._ex= r[(int)ImbalanceField.IF_EX];
            return i;
        }

        public static string Serialize(Imbalance i)
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(i.Symbol);
            sb.Append(d);
            sb.Append(i.Exchange);
            sb.Append(d);
            sb.Append(i.ThisImbalance);
            sb.Append(d);
            sb.Append(i.ThisTime);
            sb.Append(d);
            sb.Append(i.PrevImbalance);
            sb.Append(d);
            sb.Append(i.PrevTime);
            sb.Append(d);
            sb.Append(i.InfoImbalance);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Serialize(this);
        }
    }
}
