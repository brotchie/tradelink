using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace TradeLink.Common
{
    public delegate void TextIdxDelegate(string txt,int idx);
    /// <summary>
    /// Used to track any type of item by both text label and index values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericTracker<T>
    {
        int _estcount = 0;
        List<T> _tracked;
        Dictionary<string, int> _txtidx;
        /// <summary>
        /// gets count of items being tracked
        /// </summary>
        public int Count { get { return _tracked.Count; } }

        /// <summary>
        /// creates a tracker
        /// </summary>
        public GenericTracker() : this(0) { }
        /// <summary>
        /// create a tracker with an approximate # of initial items
        /// </summary>
        /// <param name="EstCount"></param>
        public GenericTracker(int EstCount)
        {
            _estcount = EstCount;
            _tracked = new List<T>(EstCount);
            _txtidx = new Dictionary<string, int>(EstCount);
        }

        /// <summary>
        /// get a tracked value from it's index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i] { get { return _tracked[i]; } set { _tracked[i] = value; } }
        /// <summary>
        /// get a tracked value from it's text label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public T this[string txt] { get { return _tracked[getindex(txt)]; } set { _tracked[getindex(txt)] = value; } }

        /// <summary>
        /// called when new text label is added
        /// </summary>
        public event TextIdxDelegate NewTxt;
        /// <summary>
        /// text label has no index
        /// </summary>
        public const int UNKNOWN = -1;

        /// <summary>
        /// gets index of text label or returns UNKNOWN if none found
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public int getindex(string txt)
        {
            int idx = UNKNOWN;
            if (_txtidx.TryGetValue(txt, out idx))
                return idx;
            return UNKNOWN;
        }
        /// <summary>
        /// gets index of label, adding it if it doesn't exist.
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public int addindex(string txtidx, T initialval)
        {
            int idx = UNKNOWN;
            if (!_txtidx.TryGetValue(txtidx, out idx))
            {
                idx = _tracked.Count;
                _txtidx.Add(txtidx, idx);
                _tracked.Add(initialval);
                if (NewTxt != null)
                    NewTxt(txtidx, idx);
            }
            return idx;
        }

        /// <summary>
        /// clears all tracked values
        /// </summary>
        public void Clear()
        {
            _tracked.Clear();
            _txtidx.Clear();
        }

        /// <summary>
        /// allows 'foreach' enumeration of each tracked element
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _tracked.Count; i++)
                yield return _tracked[i];
        }

    }
}
