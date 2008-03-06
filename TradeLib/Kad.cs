using System;
using System.Collections.Generic;


namespace TradeLib
{
    public delegate void KadDelegate(Kad k);
    public class Kad
    {
        private object _data;
        private object _data2;
        private Type _type;
        private Type _type2;
        public object Data { get { return _data; } set { _data = value; } }
        public Type DataType { get { return _type; } set { _type = value;  } }
        public object Data2 { get { return _data2; } set { _data2 = value; } }
        public Type Type2 { get { return _type2; } set { _type2 = value; } }
    }
}
