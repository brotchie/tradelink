using System;

// this namespace is intentionally different from the rest of TradeLink.Common
namespace Tript // a special class/namespace for use only with Tript
{
    /// <summary>
    /// a class to access certain features of tript quickly.
    /// eg type 
    /// q.help - to get list of help
    /// q.o - to print a line in tript output window
    /// q.o - to print a line with return in tript output window
    /// </summary>
    public static class q
    {
        public static void  help()
        {
            string[] h = new string[] {
                "help - tript help",
                "o - output string",
                "ol - output string with new line",
                "#include <filename> - reference another tript",
                "#using <filename.dll>; - include a dll",
                "#using <namesapce>; - reference a namespace",
            };
            foreach (string line in h)
                ol(line);
        }
        public static void h() { help(); }

        public static void o(string consoleoutput) { Console.Write(consoleoutput); }
        public static void ol(string consoleoutput) { Console.WriteLine(consoleoutput); }
    }
}
