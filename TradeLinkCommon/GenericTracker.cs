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
        List<string> _txt;
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
            _txt = new List<string>(EstCount);
        }

        /// <summary>
        /// gets array of labels tracked
        /// </summary>
        /// <returns></returns>
        public string[] ToLabelArray()
        {
            return _txt.ToArray();
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
        /// gets a label given an index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string getlabel(int idx) { return _txt[idx]; }
        /// <summary>
        /// gets index of label, adding it if it doesn't exist.
        /// </summary>
        /// <param name="txtidx">label</param>
        /// <param name="initialval">initial value to associate with label</param>
        /// <returns></returns>
        public int addindex(string txtidx, T initialval)
        {
            int idx = UNKNOWN;
            if (!_txtidx.TryGetValue(txtidx, out idx))
            {
                idx = _tracked.Count;
                _txt.Add(txtidx);
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

        public T[] ToArray()
        {
            return _tracked.ToArray();
        }

    }

    public static class GenericTracker
    {
        public const int UNKNOWN = -1;
        public static bool CSVInitGeneric<T>(string csvfile, ref GenericTracker<T> gt) { return CSVInitGeneric(csvfile, true, ref gt, 0, default(T), ',', null); }
        public static bool CSVInitGeneric<T>(string csvfile, ref GenericTracker<T> gt,  T coldefault) { return CSVInitGeneric(csvfile, true, ref gt, 0, coldefault, ',', null); }
        public static bool CSVInitGeneric<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault) { return CSVInitGeneric(csvfile, hasheader, ref gt, symcol, coldefault, ',', null); }
        public static bool CSVInitGeneric<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault, char delim) { return CSVInitGeneric(csvfile, hasheader, ref gt, symcol, coldefault, delim, null); }
        public static bool CSVInitGeneric<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault, char delim, TradeLink.API.DebugDelegate debug)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(csvfile);
                if (hasheader)
                    sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = string.Empty;
                    try
                    {
                        // get line
                        line = sr.ReadLine();
                        // get columns
                        string[] cols = line.Split(delim);
                        // get symbol
                        string sym = cols[symcol];
                        // add symbol to tracker 
                        int idx = gt.addindex(sym, coldefault);

                    }
                    catch (Exception ex)
                    {
                        if (debug != null)
                        {
                            debug("error on: " + line);
                            debug(ex.Message + ex.StackTrace);
                            continue;
                        }
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug(ex.Message + ex.StackTrace);
                }
                return false;
            }
            return true;
        }
        public static bool CSVCOL2Generic<T>(string csvfile, ref GenericTracker<T> gt, int col) { return CSVCOL2Generic<T>(csvfile, true, ref gt, 0, col,  ',', null); }
        public static bool CSVCOL2Generic<T>(string csvfile, ref GenericTracker<T> gt, int symcol, int col) { return CSVCOL2Generic<T>(csvfile, true, ref gt, symcol, col,  ',', null); }
        public static bool CSVCOL2Generic<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, int col) { return CSVCOL2Generic<T>(csvfile, hasheader, ref gt, symcol, col,  ',', null); }
        public static bool CSVCOL2Generic<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, T coldefaultOnFail) { return CSVCOL2Generic<T>(csvfile, hasheader, ref gt, symcol, col, ',', null); }
        public static bool CSVCOL2Generic<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, T coldefaultOnFail, char delim) { return CSVCOL2Generic<T>(csvfile, hasheader, ref gt, symcol, col, delim, null); }
        /// <summary>
        /// import csv column to a generic tracker value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="col"></param>
        /// <param name="coldefaultOnFail"></param>
        /// <param name="delim"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool CSVCOL2Generic<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, char delim, TradeLink.API.DebugDelegate debug)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(csvfile);
                if (hasheader)
                    sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = string.Empty;
                    try
                    {
                        // get line
                        line = sr.ReadLine();
                        // get columns
                        string[] cols = line.Split(delim);
                        // see if this is a symbol column
                        bool issym = symcol==col;
                        // get symbol
                        string sym = cols[symcol];
                        // add symbol to tracker 
                        int idx = gt.getindex(sym);
                        // skip if we don't know the symbol
                        if (idx<0)
                            continue;
                        // otherwise get column data
                        string coldata = cols[col];
                        // save it
                        try
                        {
                            gt[idx] = (T)Convert.ChangeType(coldata, typeof(T));
                        }
                        catch (InvalidCastException)
                        {

                        }
                        // thanks to : http://predicatet.blogspot.com/2009/04/c-string-to-generic-type-conversion.html

                    }
                    catch (Exception ex)
                    {
                        if (debug != null)
                        {
                            debug("error on: " + line);
                            debug(ex.Message + ex.StackTrace);
                            continue;
                        }
                    }

                }
                sr.Close();
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug(ex.Message + ex.StackTrace);
                }
                return false;
            }
            return true;
            
        }
    }
}
