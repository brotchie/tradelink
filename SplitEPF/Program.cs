using System;


namespace SplitEPF
{
    class Program
    {
        static void Main(string[] args)
        {
//            Console.ForegroundColor = ConsoleColor.Black;
            //Console.BackgroundColor = ConsoleColor.White;
            const string PATH = "C:\\Documents and Settings\\josh.franta\\My Documents\\Notes\\Archive\\TickFiles\\";
            string p = "";
            if (args.Length>0) p = args[1];
            else p = PATH;
            Spliter s = new Spliter(p);
            s.Reduce();
        }
    }
}
