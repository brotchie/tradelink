using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// track whether a given value flips (or crosses) from a previous state
    /// </summary>
    public class FlipTracker : GenericTracker<bool>, GenericTrackerBool
    {
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }


        GenericTracker<bool> prev;
        GenericTracker<bool> cur;
        GenericTracker<int> vcount;
        int _minvcount = 2;
        /// <summary>
        /// minimum values required to allow a flip
        /// </summary>
        public int MinFlipValueCount { get { return _minvcount; } set { _minvcount = value; } }
        public event TextIdxDelegate NewFlipEvent;
        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// create fliptracker
        /// </summary>
        public FlipTracker() : this(100) { }
        /// <summary>
        /// create fliptracker
        /// </summary>
        /// <param name="estlabels"></param>
        public FlipTracker(int estlabels) : this("FLIPPED") { }
        /// <summary>
        /// create fliptracker
        /// </summary>
        /// <param name="name"></param>
        public FlipTracker(string name) : this(name, 100) { }
        /// <summary>
        /// create fliptracker
        /// </summary>
        /// <param name="name"></param>
        /// <param name="estlabels"></param>
        public FlipTracker(string name, int estlabels)
            : base(estlabels, name)
        {
            vcount = new GenericTracker<int>(estlabels, name);
            prev = new GenericTracker<bool>(estlabels, name);
            cur = new GenericTracker<bool>(estlabels, name);
            NewTxt += new TextIdxDelegate(FlipTracker_NewTxt);
        }
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// set value to track, or check on whether given value has flipped
        /// NewFlipEvent will be thrown if value is a flip
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public new bool this[int idx]
        {
            get
            {
                return base[idx];
            }
            set
            {
                bool p = cur[idx];
                prev[idx] = p;
                cur[idx] = value;
                vcount[idx]++;
                bool flp = (value != p) && (vcount[idx] >= MinFlipValueCount);
                base[idx] = flp;
                if (flp && (NewFlipEvent != null))
                {
                    string sym = getlabel(idx);
                    debug(sym + " flipped to: " + value);
                    NewFlipEvent(sym, idx);
                }
            }
        }

        /// <summary>
        /// get or set value to track
        /// NewFlipEvent will be thrown if value is a flip
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public new bool this[string txt]
        {
            get
            {
                int idx = getindex(txt);
                return this[idx];
            }
            set
            {
                int idx = getindex(txt);
                this[idx] = value;
            }
        }

        /// <summary>
        /// clear all tracked values
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            cur.Clear();
            prev.Clear();
            vcount.Clear();
        }
        /// <summary>
        /// reset all flips
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < cur.Count; i++)
                Reset(i);
        }
        /// <summary>
        /// reset flip for particular index 
        /// </summary>
        /// <param name="idx"></param>
        public void Reset(int idx) { Reset(idx, 0); }
        /// <summary>
        /// reset flip for particular index
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="mincount"></param>
        public void Reset(int idx, int mincount)
        {
            prev[idx] = false;
            cur[idx] = false;
            vcount[idx] = mincount;
        }
        /// <summary>
        /// reset flip for particular label
        /// </summary>
        /// <param name="txt"></param>
        public void Reset(string txt)
        {
            int idx = getindex(txt);
            Reset(idx);
        }
        /// <summary>
        /// determine if a particular tracked value flipped
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Flipped(int idx) { return cur[idx]; }
        /// <summary>
        /// determine if a particular tracked value flipped
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public bool Flipped(string txt) { return cur[txt]; }
        /// <summary>
        /// get previous value
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Prev(int idx) { return prev[idx]; }
        /// <summary>
        /// get previous value
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public bool Prev(string txt) { return prev[txt]; }
        /// <summary>
        /// gets current value
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public bool Cur(string txt) { return base[txt]; }
        /// <summary>
        /// gets current value
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Cur(int idx) { return base[idx]; }



        void FlipTracker_NewTxt(string txt, int idx)
        {
            prev.addindex(txt, false);
            cur.addindex(txt, false);
            vcount.addindex(txt, 0);
        }
    }
}
