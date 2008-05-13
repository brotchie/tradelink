using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace Blade
{
    public class Blade : BarListIndicator
    {
        decimal _percent = 1;
        public Blade(decimal BladePercent) { _percent = BladePercent; }
        public bool newBar(BarList bl)
        {
            Bar c = bl.Get(bl.BarZero);
            if (!c.isValid) return false;
            decimal moverequired = c.Open * _percent;
            BladesDOWN = ((c.Open - c.Close) > moverequired);
            BladesUP = ((c.Close-c.Open) > moverequired);
            return true;
        }

        public bool BladesUP = false;
        public bool BladesDOWN = false;
    }
}
