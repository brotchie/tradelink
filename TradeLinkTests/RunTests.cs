using System;

namespace TestTradeLink
{
    // credit to blokeley for this idea :
    // http://kenthall.wordpress.com/2006/09/09/debugging-nunit-test-within-c-express/
    public class NUnitConsoleRunner
    {
        [STAThread]
        static void Main(string[] args)
        {
            NUnit.ConsoleRunner.Runner.Main(args);
        }

        internal static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        internal static void reset() { sb = new System.Text.StringBuilder(); }
        internal static void da(string msg) { debugall(msg); }
        internal static void debugall(string msg)
        {
            db(msg);
            d(msg);
        }
        internal static void db(string msg) { debugbuf(msg); }
        internal static void d(string msg) { debug(msg); }
        internal static void debug(string msg)
        {
            Console.WriteLine(msg);
        }
        internal static void debugbuf(string msg)
        {
            sb.AppendLine(msg);
        }
        public override string ToString()
        {
            return sb.ToString();
        }
        internal static string buf { get { return sb.ToString(); } }
    }

    public class rt : NUnitConsoleRunner
    {
    }
    
}
