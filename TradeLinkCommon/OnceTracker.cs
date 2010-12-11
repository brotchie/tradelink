using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// allows a tracked value to become true only once and never again
    /// </summary>
    public class OnceTracker : GenericTracker<bool>, GenericTrackerBool
    {
        public OnceTracker() : this("ONCE") { }
        public OnceTracker(string name)
            : base(name)
        {
            NewTxt += new TextIdxDelegate(OnceTracker_NewTxt);
        }


        public new bool this[int idx]
        {
            get
            {
                return base[idx];
            }
            set
            {
                bool v = base[idx];
                if (v)
                    return;
                base[idx] = value;
            }
        }

        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(string txt, bool v) { this[txt] = v; }
        public void setvalue(int idx, bool v) { this[idx] = v; }

        void OnceTracker_NewTxt(string txt, int idx)
        {

        }
    }
}
