using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLib
{
    public class TickFileFilter
    {
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
        public static List<string> FilterSyms(string[] allowedsymbols)
        {
            List<string> f = new List<string>();
            for (int i = 0; i < allowedsymbols.Length; i++)
                f.Add(allowedsymbols[i]);
            return f;

        }
        public void DateFilter(TLDateFilter datefilter)
        {
            datelist.Add(datefilter);
        }
        public void DateFilter(int date, DateMatchType type)
        {
            datelist.Add(new TLDateFilter(date, type));
        }


        public bool Allow(string filepath)
        {
            TickFileInfo tfi = Util.ParseFile(filepath);
            if (tfi.type == TickFileType.Invalid) return false;
            bool allowed = false;
            for (int i = 0; i < namelist.Count; i++)
                allowed |= (tfi.symbol == namelist[i]);
            for (int i = 0; i < datelist.Count; i++)
                allowed |= Util.TLDateMatch(Util.ToTLDate(tfi.date), datelist[i].date,datelist[i].type);
            return allowed;

        }
        public bool Deny(string filepath) { return !Allow(filepath); }
        public string[] Match(string[] filepaths)
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

    }
}
