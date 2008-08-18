using System;
using System.Text.RegularExpressions;

namespace ResearchLib
{

    class SymbolList 
    {
        public static string NYSE() { return ResearchLib.Properties.ResearchLib.nyse; }
        public static string NASDAQ() { return ResearchLib.Properties.ResearchLib.nasdaq; }
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
