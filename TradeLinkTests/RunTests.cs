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
    }
}
