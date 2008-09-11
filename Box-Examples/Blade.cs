using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace box
{
    public class Blade : BarListIndicator
    {
        decimal _percent = 1;
        decimal _bigvolper = .33m;
        bool _zerovolavgisbig = false;
        int _minbars = 2;
        public int MinimumBarsToAvg { get { return _minbars; } set { _minbars = value; } }
        public bool ZeroAvgVolIsBig { get { return _zerovolavgisbig; } set { _zerovolavgisbig = value; } }
        public decimal BigVolumePercentage { get { return _bigvolper; } set { _bigvolper = value; } }
        public decimal BladePercentage { get { return _percent; } set { _percent = value; } }
        public Blade() { }
        public Blade(decimal BladePercent) { _percent = BladePercent; }
        public Blade(decimal BladePercent, decimal BigVolPercent) { _percent = BladePercent; _bigvolper = BigVolPercent; }
        public decimal AvgVol(BarList bl) // gets the average volume across all bars
        {
            if (!bl.Has(MinimumBarsToAvg)) return 0; // if we don't have a bar yet we can't have an avg bar volume
            int sum = 0;
            for (int i = 0; i < bl.Count; i++)
                sum += bl[i].Volume;
            return sum / (bl.Count);
        }
        public bool newBar(BarList bl)
        {
            if (!bl.isValid) return false;
            Bar c = bl.RecentBar;
            if (!c.isValid) return false;
            decimal avgvol = AvgVol(bl);
            decimal moverequired = c.Open * _percent;
            bool voltest = (c.Volume - avgvol) > (avgvol * _bigvolper);
            isBigVolume = (ZeroAvgVolIsBig && voltest) || (!ZeroAvgVolIsBig && (avgvol != 0) && voltest);
            isBladeDOWN = ((c.Open - c.Close) > moverequired);
            isBladeUP = ((c.Close-c.Open) > moverequired);
            pctChange = c.Close / c.Open - 1;
            return true;
        }

        public bool isBigVolume = false;
        public bool isBladeUP = false;
        public bool isBladeDOWN = false;
        public decimal pctChange;
    }
}
