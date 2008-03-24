using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public abstract class Instrument
    {
        public abstract string Name { get; set; }
        public abstract bool isValid { get; }
    }
}
