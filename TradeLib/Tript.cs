using System;

// this namespace is intentionally different from the rest of TradeLib
namespace Tript // a special class/namespace for use only with Tript
{
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
