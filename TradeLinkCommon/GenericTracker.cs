using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using TradeLink.API;
using System.Net;

namespace TradeLink.Common
{
    
    /// <summary>
    /// Used to track any type of item by both text label and index values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericTracker<T> : GenericTrackerI
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
        /// reset all tracked values to their default value
        /// </summary>
        public virtual void Reset()
        {
            for (int i = 0; i < _tracked.Count; i++)
                _tracked[i] = Default;
        }
        /// <summary>
        /// reset given index to it's default value
        /// </summary>
        /// <param name="idx"></param>
        public virtual void Reset(int idx)
        {
            _tracked[idx] = Default;
        }
        /// <summary>
        /// reset given label to it's default value
        /// </summary>
        /// <param name="txt"></param>
        public virtual void Reset(string txt)
        {
            int idx = getindex(txt);
            _tracked[idx] = Default;
        }

        T _defval = default(T);
        /// <summary>
        /// gets default value for a given type
        /// </summary>
        public T Default { get { return _defval; } set { _defval = value; } }

        public virtual Type TrackedType
        {
            get 
        {
            T val = default(T);
            return val.GetType();
        } 
        }
        /// <summary>
        /// attempts to convert tracked value to decimal given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual decimal ValueDecimal(string txt) { return Convert.ToDecimal(this[txt]); }
        /// <summary>
        /// attempts to convert tracked value to decimal given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual decimal ValueDecimal(int idx) { return Convert.ToDecimal(this[idx]); }

        /// <summary>
        /// gets value of given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public object Value(string txt)
        {
            return this[txt];
        }

        /// <summary>
        /// gets value of give index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public object Value(int idx)
        {
            return this[idx];
        }

        string _name = string.Empty;
        /// <summary>
        /// name of this tracker
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// get display-ready tracked value of a given index.
        /// For this to work, your tracked type MUST implement ToString() otherwise it will return as empty.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string Display(int idx) 
        {
            try
            {
                return _tracked[idx].ToString();
            }
            catch 
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// get display-ready tracked value of a given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public string Display(string txt) 
        {
            try
            {
                int idx = getindex(txt); 
                if (idx < 0) return string.Empty;

                return _tracked[idx].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// creates a tracker with given name
        /// </summary>
        /// <param name="name"></param>
        public GenericTracker(string name) : this(0, name) { }

        /// <summary>
        /// creates a tracker
        /// </summary>
        public GenericTracker() : this(0,string.Empty) { }

        /// <summary>
        /// creates tracker with approximate # of initial items
        /// </summary>
        /// <param name="EstCount"></param>
        public GenericTracker(int EstCount) : this(EstCount, string.Empty) { }
        /// <summary>
        /// create a tracker with an approximate # of initial items and name
        /// </summary>
        /// <param name="EstCount"></param>
        public GenericTracker(int EstCount,string name)
        {
            _name = name;
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
        /// gets index of a label, adding it if it doesn't exist.
        /// initial value associated with index will be Default
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual int addindex(string txt)
        {
            return addindex(txt, Default);
        }
        /// <summary>
        /// gets index of label, adding it if it doesn't exist.
        /// </summary>
        /// <param name="txtidx">label</param>
        /// <param name="initialval">value to associate with label</param>
        /// <returns></returns>
        public virtual int addindex(string txtidx, T val)
        {
            int idx = UNKNOWN;
            if (!_txtidx.TryGetValue(txtidx, out idx))
            {
                idx = _tracked.Count;
                _txt.Add(txtidx);
                _txtidx.Add(txtidx, idx);
                _tracked.Add(val);
                if (NewTxt != null)
                    NewTxt(txtidx, idx);
            }
            else
            {
                _tracked[idx] = val;
            }
            return idx;
        }

        /// <summary>
        /// clears all tracked values and labels
        /// </summary>
        public virtual void Clear()
        {
            _tracked.Clear();
            _txt.Clear();
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

        public virtual T[] ToArray()
        {
            return _tracked.ToArray();
        }

    }

    /// <summary>
    /// helper methods to use with GenericTracker T
    /// </summary>
    public class GenericTracker
    {
        public static bool One(int idx, params GenericTracker<bool>[] gts)
        {
            bool ok = false;
            for (int i = 0; i < gts.Length; i++)
                ok |= gts[i][idx];
            return ok;
        }
        public static bool All(int idx, params GenericTracker<bool>[] gts)
        {
            bool ok = true;
            for (int i = 0; i < gts.Length; i++)
                ok &= gts[i][idx];
            return ok;
        }
        public const int UNKNOWN = -1;
        public static void clearindicators(params GenericTrackerI[] gts)
        {
            foreach (GenericTrackerI gt in gts)
                gt.Clear();
        }

        public static GenericTrackerI[] geninds(params GenericTrackerI[] gts)
        {
            return gts;
        }
        /// <summary>
        /// import a csv file into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="gt"></param>
        /// <returns></returns>
        public static bool CSVInitGeneric<T>(string csvfile, ref GenericTracker<T> gt) { return CSVInitGeneric(csvfile, true, ref gt, 0, default(T), ',', null); }
        /// <summary>
        /// import a csv file into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="gt"></param>
        /// <param name="coldefault"></param>
        /// <returns></returns>
        public static bool CSVInitGeneric<T>(string csvfile, ref GenericTracker<T> gt,  T coldefault) { return CSVInitGeneric(csvfile, true, ref gt, 0, coldefault, ',', null); }
        /// <summary>
        /// import a csv file into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="coldefault"></param>
        /// <returns></returns>
        public static bool CSVInitGeneric<T>(string csvfile, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault) { return CSVInitGeneric(csvfile, hasheader, ref gt, symcol, coldefault, ',', null); }
        /// <summary>
        /// import a csv file into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="coldefault"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
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
                        gt.addindex(sym, coldefault);

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
                        // bool issym = symcol==col;
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

        /// <summary>
        /// gets all current values of every tracker for every symbol being tracked
        /// </summary>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string GetIndicatorPairs(params GenericTrackerI[] gts)
        {
            if (gts.Length == 0) 
                return string.Empty;
            // get first tracker
            GenericTrackerI gt = gts[0];
            // use to index every symbol
            StringBuilder all = new StringBuilder();
            for (int i = 0; i < gt.Count; i++)
                all.AppendLine(GetIndicatorPairs(i, gts));
            return all.ToString();
        }

        /// <summary>
        /// get single readable line of indicators for output when response debugging
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string GetIndicatorPairs(int idx, params GenericTrackerI[] gts) { return GetIndicatorPairs(idx, " ", gts); }
        /// <summary>
        /// get single readable line of indicators (with custom delimiter) for output when response debugging
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="delim"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string GetIndicatorPairs(int idx, string delim, params GenericTrackerI[] gts)
        {
            return string.Join(delim, GenericTracker.GetIndicatorPrettyPairs(idx, gts));
        }

        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col) { return WriteCSV<T>(filepath, gt, col, 0, true, ',', string.Empty, null); }
        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <param name="symcol"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col, int symcol) { return WriteCSV<T>(filepath, gt, col, symcol, true, ',', string.Empty, null); }
        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <param name="symcol"></param>
        /// <param name="hasheader"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col, int symcol, bool hasheader) { return WriteCSV<T>(filepath, gt, col, symcol, hasheader, ',', string.Empty, null); }
        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <param name="symcol"></param>
        /// <param name="hasheader"></param>
        /// <param name="delim"></param>
        /// <param name="stringformat"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col, int symcol, bool hasheader, char delim) { return WriteCSV<T>(filepath, gt, col, symcol, hasheader, delim, string.Empty, null); }
        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <param name="symcol"></param>
        /// <param name="hasheader"></param>
        /// <param name="delim"></param>
        /// <param name="stringformat"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col, int symcol, bool hasheader, char delim, string stringformat) { return WriteCSV<T>(filepath, gt, col, symcol, hasheader, delim, stringformat, null); }
        /// <summary>
        /// write a generic tracker to one column of a csv file, leaving rest of file untouched.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <param name="gt"></param>
        /// <param name="col"></param>
        /// <param name="symcol"></param>
        /// <param name="hasheader"></param>
        /// <param name="delim"></param>
        /// <param name="stringformat"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool WriteCSV<T>(string filepath, GenericTracker<T> gt, int col, int symcol, bool hasheader, char delim, string stringformat, DebugDelegate debug)
        {
            if (!File.Exists(filepath) && hasheader)
            {
                if (debug != null)
                {
                    debug("file does not exist: " + filepath);
                    debug("Call InitCSV first on file");
                }
                return false;
            }
            try
            {
                // slurp file in
                StreamReader sr = new StreamReader(filepath);
                string file = sr.ReadToEnd();
                string[] lines = file.Split(Environment.NewLine.ToCharArray());
                sr.Close();
                try
                {
                    // write back out
                    StreamWriter sw = new StreamWriter(filepath, false);
                    // skip first line if we have a header
                    int start = hasheader ? 1 : 0;
                    // write header back out if it exists
                    if (hasheader)
                        sw.WriteLine(lines[0]);
                    // test to see if we have custom formating
                    bool format = stringformat != string.Empty;
                    // loop through every line in the file
                    for (int i = start; i < lines.Length; i++)
                    {
                        // get all the columns
                        string[] cols = lines[i].Split(delim);
                        // get the symbol from the column
                        string sym = cols[symcol];
                        // get value from our GT and convert it for writing
                        cols[col] = format ? string.Format("{0:" + stringformat + "}", gt[sym]) : gt[sym].ToString();
                        // write the line back out
                        sw.WriteLine(string.Join(delim.ToString(), cols));
                    }
                    sw.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    if (debug != null)
                    {
                        debug("error writing file, returning to previous state: " + filepath);
                        debug(ex.Message + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug(ex.Message + ex.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// create a csv file using Name on each of an array of generic trackers
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static bool InitCSV(string filepath, GenericTrackerI[] gts)
        {
            return InitCSV(filepath, GetIndicatorNames(gts), false);
        }

        /// <summary>
        /// create a csv file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static bool InitCSV(string filepath, string[] headers) { return InitCSV(filepath, headers, false); }
        /// <summary>
        /// create a csv file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="headers"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static bool InitCSV(string filepath, string[] headers, bool overwrite) { return InitCSV(filepath, headers, overwrite, ','); }
        /// <summary>
        /// create a csv file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="headers"></param>
        /// <param name="overwrite"></param>
        /// <param name="delim"></param>
        /// <returns></returns>
        public static bool InitCSV(string filepath, string[] headers, bool overwrite, char delim) { return InitCSV(filepath, headers, overwrite, delim); }
        /// <summary>
        /// create a csv file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="headers"></param>
        /// <param name="overwrite"></param>
        /// <param name="delim"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool InitCSV(string filepath, string[] headers, bool overwrite, char delim, DebugDelegate debug)
        {
            if (File.Exists(filepath) && !overwrite)
            {
                if (debug != null)
                    debug(filepath + " already exists.");
                return false;
            }
            try
            {
                StreamWriter sw = new StreamWriter(filepath, false);
                if (headers.Length > 0)
                    sw.WriteLine(string.Join(delim.ToString(), headers));
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug(ex.Message + ex.StackTrace);
                }
            }
            return false;
        }
        /// <summary>
        /// gets indicator names from trackers
        /// </summary>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorNames(params GenericTrackerI[] gts)
        {
            List<string> names = new List<string>(gts.Length);
            for (int i = 0; i < gts.Length; i++)
            {
                names.Add(gts[i].Name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// gets indicator values from trackers
        /// </summary>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorValues(int idx, params GenericTrackerI[] gts)
        {
            if ((idx<0) || (gts.Length==0)) return new string[0];
            List<string> display = new List<string>(gts.Length);
            for (int i = 0; i < gts.Length; i++)
            {
                if (idx >= gts[i].Count) 
                    display.Add(string.Empty);
                display.Add(gts[i].Display(idx));

            }
            return display.ToArray();
        }

        /// <summary>
        /// gets indicator values from trackers
        /// </summary>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorValues(string txt, params GenericTrackerI[] gts)
        {
            if ((gts.Length == 0)) return new string[0];
            List<string> display = new List<string>(gts.Length);
            for (int i = 0; i < gts.Length; i++)
            {
                int idx = gts[i].getindex(txt);
                if ((idx<0) || (idx >= gts[i].Count))
                    display.Add(string.Empty);
                display.Add(gts[i].Display(txt));

            }
            return display.ToArray();
        }
        /// <summary>
        /// get name=>value pairs
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorPrettyPairs(string txt, params GenericTrackerI[] gts) { return GetIndicatorPrettyPairs(txt, "=>", gts); }
        /// <summary>
        /// get name=>value pairs
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="delim"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorPrettyPairs(string txt, string delim,params GenericTrackerI[] gts)
        {
            if ((gts.Length == 0)) return new string[0];
            List<string> display = new List<string>(gts.Length);
            for (int i = 0; i < gts.Length; i++)
            {
                int idx = gts[i].getindex(txt);
                if ((idx < 0) || (idx >= gts[i].Count))
                    display.Add(string.Empty);
                display.Add(gts[i].Name+delim+gts[i].Display(txt));

            }
            return display.ToArray();
        }
        /// <summary>
        /// get name=>value pairs
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorPrettyPairs(int idx, params GenericTrackerI[] gts) { return GetIndicatorPrettyPairs(idx, "=>", gts); }
        /// <summary>
        /// get name=>value pairs
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="delim"></param>
        /// <param name="gts"></param>
        /// <returns></returns>
        public static string[] GetIndicatorPrettyPairs(int idx, string delim, params GenericTrackerI[] gts)
        {
            if ((idx<0) || (gts.Length == 0)) return new string[0];
            List<string> display = new List<string>(gts.Length);
            for (int i = 0; i < gts.Length; i++)
            {
                if ((idx < 0) || (idx >= gts[i].Count))
                    display.Add(string.Empty);
                display.Add(gts[i].Name + delim + gts[i].Display(idx));

            }
            return display.ToArray();
        }

        /// <summary>
        /// test a rule made up of trackers
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="rulename"></param>
        /// <param name="booltrackers"></param>
        /// <returns></returns>
        public new static bool rulepasses(int idx, string rulename, params GenericTrackerI[] booltrackers) { return rulepasses(idx, rulename, true, null, false, booltrackers); }
        /// <summary>
        /// test a rule made up of trackers
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="rulename"></param>
        /// <param name="debug"></param>
        /// <param name="booltrackers"></param>
        /// <returns></returns>
        public new static bool rulepasses(int idx, string rulename, DebugDelegate debug, params GenericTrackerI[] booltrackers) { return rulepasses(idx, rulename, true, debug, false, booltrackers); }
        /// <summary>
        /// test a rule made up of trackers
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="rulename"></param>
        /// <param name="debug"></param>
        /// <param name="debugfails"></param>
        /// <param name="booltrackers"></param>
        /// <returns></returns>
        public new static bool rulepasses(int idx, string rulename, DebugDelegate debug, bool debugfails, params GenericTrackerI[] booltrackers) { return rulepasses(idx, rulename, true, debug, debugfails, booltrackers); }
        /// <summary>
        /// test a rule made up of trackers... optionally display the passes or failures.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="rulename"></param>
        /// <param name="debug"></param>
        /// <param name="debugfails"></param>
        /// <param name="booltrackers"></param>
        /// <returns></returns>
        public new static bool rulepasses(int idx, string rulename, bool fastmode, DebugDelegate debug, bool debugfails, params GenericTrackerI[] booltrackers)
        {
            if (idx < 0)
            {
                if (debug != null)
                    debug("??? failed rule: " + rulename + " reason: invalid index -1");
                return false;
            }
            bool ok = true;
            
            string errcode = string.Empty;
            List<GenericTrackerI> passes = new List<GenericTrackerI>(booltrackers.Length);
            List<GenericTrackerI> fails = new List<GenericTrackerI>(booltrackers.Length);
            try
            {


                debugfails &= (debug != null);
                
                for (int i = 0; i < booltrackers.Length; i++)
                {
                    // get tracker
                    GenericTrackerI gt = booltrackers[i];
                    // skip non bool types
                    if (gt.TrackedType != typeof(bool))
                        continue;
                    // test for pass
                    bool pass = gt.ValueDecimal(idx) == 1;
                    ok &= pass;
                    if (pass)
                        passes.Add(gt);
                    else if (debugfails)
                    {
                        fails.Add(gt);
                        if (fastmode)
                            break;
                    }
                    else if (fastmode)
                        break;
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errcode = ex.Message + ex.StackTrace;
            }
            
            // display if need be
            if ((debug != null) && (booltrackers.Length > 0))
            {
                string sym = booltrackers[0].getlabel(idx);
                if (ok)
                    debug(sym + " passed rule: " + rulename + " reason: " + GenericTracker.GetIndicatorPairs(idx, passes.ToArray()));
                else if (debugfails)
                    debug(sym + " failed rule: " + rulename + " reason: " + errcode+ " "+GenericTracker.GetIndicatorPairs(idx, fails.ToArray()));
            }
            return ok;
        }


        public static string get(string url) { return get(url, true, 3); }
        public static string get(string url, bool removenewlines, int retries)
        {
            bool retry = true;
            int count = 0;
            string data = string.Empty;
            while (retry && (count++ < retries))
            {
                try
                {
                    WebClient wc = new WebClient();
                    data = wc.DownloadString(url);
                    retry = data != string.Empty;
                }
                catch (Exception ex)
                {
                    retry = true;
                    System.Threading.Thread.Sleep(100);
                }
                if (removenewlines)
                {
                    data = data.Replace(Environment.NewLine, " ");
                    data = data.Replace("\n", " ");
                }

            }
            return data;
        }

        const string ROWDELIM = "rowdelimROWDELIM";
        const string COLDELIM = "coldelimCOLDELIM";

        static string[] tablerows(string table) { return tablerows(table, true); }
        static string[] tablerows(string table, bool ignorefirstrow)
        {
            string[] data = table.Split(new string[] { ROWDELIM }, StringSplitOptions.RemoveEmptyEntries);
            if (!ignorefirstrow)
                return data;
            string[] d2 = new string[data.Length - 1];
            Array.Copy(data, 1, d2, 0, data.Length - 1);
            return d2;
        }

        static string[] tablecols(string row)
        {
            return row.Split(new string[] { COLDELIM }, StringSplitOptions.RemoveEmptyEntries);
        }

        static string StripHtml(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }

        static string slice(string data, string begintag, string endtag, bool includestart, bool includeend)
        {
            int start = data.IndexOf(begintag);
            string news = data;
            if (start != -1)
                news = data.Remove(0, includestart ? start : start + begintag.Length);
            int end = news.IndexOf(endtag);
            string newe = news;

            if (end != -1)
                newe = news.Remove(end + (includeend ? endtag.Length : 0), news.Length - (includeend ? end - endtag.Length : end));
            return newe;
        }
        /// <summary>
        /// import a url into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="gt"></param>
        /// <returns></returns>
        public static bool TBLInitGeneric<T>(string url, ref GenericTracker<T> gt) { return TBLInitGeneric(url, true, ref gt, 0, default(T), ',', null); }
        /// <summary>
        /// import a url into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="gt"></param>
        /// <param name="coldefault"></param>
        /// <returns></returns>
        public static bool TBLInitGeneric<T>(string url, ref GenericTracker<T> gt, T coldefault) { return TBLInitGeneric(url, true, ref gt, 0, coldefault, ',', null); }
        /// <summary>
        /// import a url into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="coldefault"></param>
        /// <returns></returns>
        public static bool TBLInitGeneric<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault) { return TBLInitGeneric(url, hasheader, ref gt, symcol, coldefault, ',', null); }
        /// <summary>
        /// import a csv file into a generic tracker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvfile"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="coldefault"></param>
        /// <param name="delim"></param>
        /// <returns></returns>

        public static bool TBLInitGeneric<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault, char delim) { return TBLInitGeneric(url, hasheader, ref gt, symcol, coldefault, delim, null); }
        public static bool TBLInitGeneric<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, T coldefault, char delim, TradeLink.API.DebugDelegate debug)
        {
            try
            {
                string data = get(url);
                int sidx = url.LastIndexOf("/");
                string starttag = url.Substring(sidx,url.Length-sidx);
                string endtag = @"<!-- /wiki-content-body -->";
                data = slice(data, starttag, endtag, false, false);
                data = data.Replace("</tr>", ROWDELIM);
                data = data.Replace("</td>", COLDELIM);
                data = StripHtml(data);
                string[] rows = tablerows(data, hasheader);
                for (int r = 0; r < rows.Length; r++)
                {
                    string line = string.Empty;
                    try
                    {
                        // get line
                        line = rows[r];
                        // get columns
                        string[] cols = tablecols(line);
                        // get symbol
                        string sym = cols[symcol];
                        // add symbol to tracker 
                        gt.addindex(sym, coldefault);

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


        public static bool TBLCOL2Generic<T>(string url, ref GenericTracker<T> gt, int col) { return TBLCOL2Generic<T>(url, true, ref gt, 0, col, ',', null); }
        public static bool TBLCOL2Generic<T>(string url, ref GenericTracker<T> gt, int symcol, int col) { return TBLCOL2Generic<T>(url, true, ref gt, symcol, col, ',', null); }
        public static bool TBLCOL2Generic<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, int col) { return TBLCOL2Generic<T>(url, hasheader, ref gt, symcol, col, ',', null); }
        public static bool TBLCOL2Generic<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, T coldefaultOnFail) { return TBLCOL2Generic<T>(url, hasheader, ref gt, symcol, col, ',', null); }
        public static bool TBLCOL2Generic<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, T coldefaultOnFail, char delim) { return TBLCOL2Generic<T>(url, hasheader, ref gt, symcol, col, delim, null); }
        /// <summary>
        /// import url column to a generic tracker value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="hasheader"></param>
        /// <param name="gt"></param>
        /// <param name="symcol"></param>
        /// <param name="col"></param>
        /// <param name="coldefaultOnFail"></param>
        /// <param name="delim"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool TBLCOL2Generic<T>(string url, bool hasheader, ref GenericTracker<T> gt, int symcol, int col, char delim, TradeLink.API.DebugDelegate debug)
        {
            try
            {
                string data = get(url);
                int sidx = url.LastIndexOf("/");
                string starttag = url.Substring(sidx, url.Length - sidx);
                string endtag = @"<!-- /wiki-content-body -->";
                data = slice(data, starttag, endtag, false, false);
                data = data.Replace("</tr>", ROWDELIM);
                data = data.Replace("</td>", COLDELIM);
                data = StripHtml(data);
                string[] rows = tablerows(data, hasheader);
                for (int r = 0; r<rows.Length; r++)
                {
                    string line = string.Empty;
                    try
                    {
                        // get line
                        line = rows[r];
                        // get columns
                        string[] cols = tablecols(line);
                        // see if this is a symbol column
                        // bool issym = symcol==col;
                        // get symbol
                        string sym = cols[symcol];
                        // add symbol to tracker 
                        int idx = gt.getindex(sym);
                        // skip if we don't know the symbol
                        if (idx < 0)
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

    public class gt : GenericTracker
    {
    }

}
