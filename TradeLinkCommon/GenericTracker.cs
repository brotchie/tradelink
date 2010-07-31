using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using TradeLink.API;

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

        string _name = string.Empty;
        /// <summary>
        /// name of this tracker
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// get display-ready tracked value of a given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public string Display(int idx) { return _tracked[idx].ToString(); }
        /// <summary>
        /// get display-ready tracked value of a given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public string Display(string txt) { int idx = getindex(txt); if (idx < 0) return string.Empty; return _tracked[idx].ToString() ; }

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

    /// <summary>
    /// helper methods to use with GenericTracker T
    /// </summary>
    public static class GenericTracker
    {
        public const int UNKNOWN = -1;

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


    }

}
