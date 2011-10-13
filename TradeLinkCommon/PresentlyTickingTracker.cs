using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class PresentlyTickingTracker : GenericTracker<bool>,GenericTrackerBool
    {
        public PresentlyTickingTracker() : base("ISTICKING") { }
        int _curidx = GenericTracker.UNKNOWN;
        public int PresentlyTickingIdx { get { return _curidx; } }
        public string PresentlyTickingSymbol { get { return _curidx == GenericTracker.UNKNOWN ? string.Empty : getlabel(_curidx); } }
        public bool getvalue(int idx) { return (_curidx!=GenericTracker.UNKNOWN) && (idx==_curidx); }
        public bool getvalue(string txt) { int idx = getindex(txt); return getvalue(idx); }
        public void setvalue(int idx, bool v) { }
        public int addindex(string txt, bool v) { return getindex(txt); }
        public new bool this[string txt] { get { return getvalue(txt); } }
        public new bool this[int idx] { get { return getvalue(idx); } }

        public void newTick(Tick k)
        {
            int idx = getindex(k.symbol);
            if (idx < 0)
                idx = addindex(k.symbol);
            _curidx = idx;
        }
    }
}
