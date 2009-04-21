using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace TradeLink.Research
{
    /// <summary>
    /// get a list of verified NASDAQ or NYSE symbols.
    /// (not guaranteed to be current, static list is distributed with Research components)
    /// </summary>
    public class SymbolList 
    {
        public const int SYM = 0;
        public const int CUSIP = 1;
        public const int DESC = 2;
        public const int INDUSTRY = 3;
        public const int INDDUSTRYCODE = 4;
        public static string [] NYSE() 
        {
            // format:
            // Symbol|CUSIP|Company|Industry|IndCode|
            string nyse = Properties.Resources.nyse;
            string[] master = nyse.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> query = from line in master let x = line.Split('|') select x[SYM];
            return query.ToArray();
        }
        public static string[] NASDAQ() 
        {
            string nasdaq = Properties.Resources.nasdaq;
            string[] master = nasdaq.Split(Environment.NewLine.ToCharArray());
            return master;
        }
        public static string[] ALL() 
        {
            string[] a = NYSE();
            string[] b = NASDAQ();
            string[] c = new string[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }
    }
    public class NYSE 
    {
        private const string _name = "NYSE";
        public string Name { get { return _name; } }
        public static bool isAllowed(string symbol) { return Regex.IsMatch(symbol,"[A-Z]{3}"); }
        public static bool isListed(string symbol)
        {
            return SymbolList.NYSE().Contains(symbol + Environment.NewLine);
        }
    }

    public class NASDAQ 
    {
        private const string _name = "NASDAQ";
        public string Name { get { return _name; } }
        public static bool isAllowed(string symbol) { return Regex.IsMatch(symbol,"[A-Z]{4}"); }
        public static bool isListed(string symbol)
        {
            return SymbolList.NASDAQ().Contains(symbol + Environment.NewLine);
        }
    }



}
