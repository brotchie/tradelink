using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    /// <summary>
    /// Filters tick files (EPF/IDX) based on symbol name and trading date.
    /// </summary>
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
        public TickFileFilter() : this(new List<string>(), new List<TLDateFilter>()) { }
        public TickFileFilter(List<string> namefilter) : this(namefilter, null) { }
        public TickFileFilter(List<TLDateFilter> datefilter) : this(null, datefilter) { }
        public TickFileFilter(List<string> namefilter, List<TLDateFilter> datefilter)
        {
            if (namefilter != null)
                namelist = namefilter;
            if (datefilter != null)
                datelist = datefilter;
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


        /// <summary>
        /// Allows the specified filepath, if instructed by the filter.
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <returns>true if allowed, false otherwise</returns>
        public bool Allow(string filepath)
        {
            TickFileInfo tfi = Util.ParseFile(filepath);
            if (tfi.type == TickFileType.Invalid) return _allowedinvalid;
            bool allowed = _defallowed;
            for (int i = 0; i < namelist.Count; i++)
                allowed |= (tfi.symbol == namelist[i]);
            for (int i = 0; i < datelist.Count; i++)
                allowed |= Util.TLDateMatch(Util.ToTLDate(tfi.date), datelist[i].date,datelist[i].type);
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

        public struct TLDateFilter
        {
            public TLDateFilter(int date, DateMatchType type)
            { this.date = date; this.type = type; }
            public int date;
            public DateMatchType type;
        }

        public static int TLYearMask(int year) { return year * 10000; }
        public static int TLMonthMask(int month) { return month * 100; }

    }
}
