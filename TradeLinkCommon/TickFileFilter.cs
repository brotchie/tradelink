using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TradeLink.Common
{
    /// <summary>
    /// Filters tick files (EPF/IDX) based on symbol name and trading date.
    /// </summary>
    [Serializable]
    public class TickFileFilter
    {
        bool _defallowed = false;
        bool _allowedinvalid = false;
        /// <summary>
        /// Gets or sets a value indicating whether [default deny] is used when Allow and Deny are called.
        /// </summary>
        /// <value><c>true</c> if [default deny]; otherwise, <c>false</c>.</value>
        public bool DefaultDeny { get { return !_defallowed; } set { _defallowed = !value; } }
        /// <summary>
        /// Gets or sets a value indicating whether the class will [allow invalid] tickfiles, which have undefined extensions.
        /// </summary>
        /// <value><c>true</c> if [allow invalid]; otherwise, <c>false</c>.</value>
        public bool AllowInvalid { get { return _allowedinvalid; } set { _allowedinvalid = value; } }
        List<string> namelist = new List<string>();
        List<TLDateFilter> datelist = new List<TLDateFilter>();
        public List<string> SymbolList { get { return namelist; } set { namelist = value; } }
        public List<TLDateFilter> DateList { get { return datelist; } set { datelist = value; } }
        public TickFileFilter() : this(new List<string>(), new List<TLDateFilter>()) { }
        public TickFileFilter(List<string> namefilter) : this(namefilter, null) { }
        public TickFileFilter(List<TLDateFilter> datefilter) : this(null, datefilter) { }
        public TickFileFilter(List<string> namefilter, List<TLDateFilter> datefilter)
        {
            if ((namefilter != null) && (datefilter != null))
            {
                namelist = namefilter;
                datelist = datefilter;
                isSymbolDateMatchUnion = false;
            }
            else if (namefilter != null)
            {
                namelist = namefilter;
                isSymbolDateMatchUnion = true;
            }
            else if (datefilter != null)
            {
                datelist = datefilter;
                isSymbolDateMatchUnion = true;
            }
             
        }
        public TickFileFilter(string[] symbols) : this(FilterSyms(symbols), null) { }
        private static List<string> FilterSyms(string[] allowedsymbols)
        {
            List<string> f = new List<string>();
            for (int i = 0; i < allowedsymbols.Length; i++)
                f.Add(allowedsymbols[i]);
            return f;

        }
        /// <summary>
        /// Adds a symbol filter
        /// </summary>
        /// <param name="stock">The stock.</param>
        public void SymFilter(string stock)
        {
            namelist.Add(stock);
        }
        /// <summary>
        /// Adds an array of symbol filters
        /// </summary>
        /// <param name="stocks">The stocks.</param>
        public void SymFilter(string[] stocks)
        {
            for (int i = 0; i < stocks.Length; i++)
                namelist.Add(stocks[i]);
        }
        /// <summary>
        /// Adds an array of TLDateFilters
        /// </summary>
        /// <param name="filters">The filters.</param>
        public void DateFilter(TLDateFilter[] filters)
        {
            for (int i = 0; i < filters.Length; i++)
                datelist.Add(filters[i]);
        }
        /// <summary>
        /// Adds a single DateFilter
        /// </summary>
        /// <param name="datefilter">The datefilter.</param>
        public void DateFilter(TLDateFilter datefilter)
        {
            datelist.Add(datefilter);
        }
        /// <summary>
        /// Adds a single DateFilter
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="type">The type.</param>
        public void DateFilter(int date, DateMatchType type)
        {
            datelist.Add(new TLDateFilter(date, type));
        }

        bool _isDateMatchUnion = true;
        bool _isSymDateMatchUnion = true;
        /// <summary>
        /// if true, any file that matches ANY date will be allowed.  If false, all dates must match before a tick file is allowed.  default is true.
        /// </summary>
        public bool isDateMatchUnion { get { return _isDateMatchUnion; } set { _isDateMatchUnion = value; } }
        /// <summary>
        /// if true, any file matching SymbolMatch OR DateMatch will be allowed.   Otherwise, it must be allowed by the Symbol filters AND the Date filters.  default is true.
        /// </summary>
        public bool isSymbolDateMatchUnion { get { return _isSymDateMatchUnion; } set { _isSymDateMatchUnion = value; } }

        /// <summary>
        /// Allows the specified filepath, if instructed by the filter.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns>true if allowed, false otherwise</returns>
        public bool Allow(string filepath)
        {
            TickFileInfo tfi = Util.ParseFile(filepath);
            if (tfi.type == TickFileType.Invalid) return _allowedinvalid;
            // make sure the default is consistent with the set intersection requested
            bool symallowed = _defallowed;
            // see if symbols match
            for (int i = 0; i < namelist.Count; i++)
                symallowed |= (tfi.symbol == namelist[i]);

            // make sure the default is consistent with the set intersection requested
            bool dateallowed = _isDateMatchUnion ? _defallowed : !_defallowed;
            if (_isDateMatchUnion)
            {
                for (int i = 0; i < datelist.Count; i++)
                    dateallowed |= Util.TLDateMatch(Util.ToTLDate(tfi.date), datelist[i].date, datelist[i].type);
            }
            else
            {
                for (int i = 0; i < datelist.Count; i++)
                    dateallowed &= Util.TLDateMatch(Util.ToTLDate(tfi.date), datelist[i].date, datelist[i].type);
            }
            // make sure intersection between dates and symbols is what is desired
            bool allowed = _isSymDateMatchUnion ? symallowed || dateallowed : symallowed && dateallowed;
            return allowed;

        }
        /// <summary>
        /// Denies the specified filepath, if instructed by the filter.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns>true if denied, false otherwise</returns>
        public bool Deny(string filepath) { return !Allow(filepath); }
        /// <summary>
        /// Allows the specified filepaths.  Plural version of Allow.
        /// </summary>
        /// <param name="filepaths">The filepaths.</param>
        /// <returns></returns>
        public string[] Allows(string[] filepaths)
        {
            List<string> filenames = new List<string>();
            for (int i = 0; i < filepaths.Length; i++)
                if (Allow(filepaths[i]))
                    filenames.Add(filepaths[i]);
            return filenames.ToArray();
        }

        public string[] AllowsIndex(string[,] index)
        {
            List<string> keep = new List<string>();
            for (int i = 0; i < index.GetLength(0); i++)
                if (Allow(index[i, 0]))
                    keep.Add(index[i, 0]);
            return keep.ToArray();
        }

        public string[,] AllowsIndexAndSize(string[,] index)
        {
            List<int> keep = new List<int>();
            for (int i = 0; i < index.GetLength(0); i++)
                if (Allow(index[i, 0]))
                    keep.Add(i);
            string[,] allow = new string[keep.Count, 2];
            for (int i = 0; i<keep.Count; i++)
            {
                allow[i,0] = index[keep[i],0];
                allow[i,1] = index[keep[i],1];
            }
            return allow;
        }
        /// <summary>
        /// match a specific portion of a tradelink date (eg month only, year only, etc)
        /// </summary>
        public struct TLDateFilter
        {
            public TLDateFilter(int date, DateMatchType type)
            { this.date = date; this.type = type; }
            public int date;
            public DateMatchType type;
            const char dl = '+';
            public static string Serialize(TLDateFilter df)
            {
                return df.date.ToString() + dl + ((int)df.type).ToString();
            }
            public static TLDateFilter Deserialize(string msg)
            {
                string [] r = msg.Split(dl);
                TLDateFilter df = new TLDateFilter();
                int ir = 0;
                if (int.TryParse(r[1], out ir))
                    df.type = (DateMatchType)ir;
                if (int.TryParse(r[0], out ir))
                    df.date = ir;
                return df;
            }
        }
        /// <summary>
        /// get a filter that excludes everything but year from TL date
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int TLYearMask(int year) { return year * 10000; }
        /// <summary>
        /// get a filter that excludes everything but the month from TL date
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static int TLMonthMask(int month) { return month * 100; }

        const char d = ',';
        const char name = 'N';
        const char date = 'D';
        /// <summary>
        /// serialize a tradelink tick file filter
        /// </summary>
        /// <param name="tff"></param>
        /// <returns></returns>
        public static string Serialize(TickFileFilter tff)
        {
            // save everything as xml
            StringWriter fs;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TickFileFilter));
                fs = new StringWriter();
                xs.Serialize(fs, tff);
                fs.Close();
            }
            catch (FileNotFoundException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            if (fs == null) return "";
            return fs.GetStringBuilder().ToString();

        }
        /// <summary>
        /// take a serialized tickfilefilter and convert back to an object
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static TickFileFilter Deserialize(string msg)
        {
            StringReader fs;
            TickFileFilter tf = null;
            try
            {
                // prepare serializer
                XmlSerializer xs = new XmlSerializer(typeof(TickFileFilter));
                // read in message
                fs = new StringReader(msg);
                // deserialize message
                tf = (TickFileFilter)xs.Deserialize(fs);
                // close serializer
                fs.Close();
            }
            catch (FileNotFoundException) { }
            catch (Exception) { }
            return tf;

        }
        /// <summary>
        /// save tickfilefilter to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static TickFileFilter FromFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string msg = sr.ReadToEnd();
            TickFileFilter tff = TickFileFilter.Deserialize(msg);
            return tff;
        }
        /// <summary>
        /// restore a filter from a file
        /// </summary>
        /// <param name="tff"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool ToFile(TickFileFilter tff, string filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename, false);
                sw.AutoFlush = true;
                sw.WriteLine(TickFileFilter.Serialize(tff));
                sw.Close();
            }
            catch (Exception) { return false; }
            return true;
        }

        public enum MessageTickFileFilter
        {
            TFF_FILTERTYPE = 0,
            TFF_FILTERCONTENT = 1,
        }

    }
}
