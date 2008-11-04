using System;


namespace SplitEPF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("We recommend you store your tick files that need spliting in a seperate directory.");
            Console.WriteLine("Then run split EPF on this isolated directory.");
            Console.WriteLine("Enter a path to scan for EPF files to split: ");
            string PATH = Console.ReadLine();
            string p = "";
            if (args.Length>0) p = args[1];
            else p = PATH;
            Spliter s = new Spliter(p);
            s.Reduce();
        }
    }
}
