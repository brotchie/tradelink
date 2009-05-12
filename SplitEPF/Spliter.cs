using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using TradeLink.Common;

namespace SplitEPF
{
    public class Spliter
    {
        public Spliter(string p) { this.PATH = p; }
        string PATH = "";
        string fname = "";
        StreamReader inf = null;
        StreamWriter of = null;
        TickImpl t = new TickImpl();
        string symbol = "";
        int lastdate = 0;
        bool firstFile = true;

        public void Reduce()
        {
            show("Starting scan of: " + PATH);
            if (PATH.Length != PATH.LastIndexOf("\\")) PATH = PATH + "\\";

            ArrayList files = TickFiles();
            for (int file = 1; file <= files.Count; file++)
            {
                lastdate = 0;
                TickFile((string)files[file-1]); // set current file
                string ofile = "";
                bool SAME = false;

                while (!SAME && !inf.EndOfStream  && t.hasTick) // while we have ticks in this file,
                {

                    if (fname.Equals(symbol + t.date + ".epf", StringComparison.CurrentCultureIgnoreCase)) 
                    { 
                        SAME = true;
                        show("Input file is the same name as the output file: " + symbol + t.date + ".epf");
                        continue; 
                    }
                    ofile = symbol + t.date+ ".EPF";
                    this.of = new StreamWriter(PATH + ofile);
                    of.Write(eSigTick.EPFheader(symbol, t.date));
                    if (firstFile == true)
                    {
                        firstFile = false;
                        lastdate = t.date;
                    }
                    while( lastdate == t.date) {
                        of.WriteLine(eSigTick.ToEPF(t));
                        t = (TickImpl)eSigTick.FromStream(symbol,inf);
                    }
                    lastdate = t.date;
                    show(symbol + t.date + ".EPF");
                    of.Flush();
                    of.Close();
                }
                inf.Close();
            }

        }

        ArrayList TickFiles()
        {
            DirectoryInfo di = new DirectoryInfo(PATH);
            FileInfo[] files = di.GetFiles("*.epf");
            ArrayList tf = new ArrayList();
            for (int i = 0; i < files.Length; i++) tf.Add(files[i].Name);
            show("Found " + tf.Count + " eSignal tick files in scan...");
            return tf;
        }
        void TickFile(string filename)
        {
            this.inf = new StreamReader(PATH + filename);
            this.of = null;
            fname = filename;
            string symline = this.inf.ReadLine();
            string dateline = this.inf.ReadLine();
            Regex se = new Regex("=[a-z-A-Z \\#]+");
            Regex dse = new Regex("=[0-9V]+");
            Regex dee = new Regex("-[0-9V]+");
            MatchCollection r = se.Matches(symline, 0);
            string l = r[0].Value;
            this.symbol = l.Substring(1, l.Length - 1);
            int rem = symbol.IndexOf(' ');
            if (rem!=-1) symbol = symbol.Remove(rem, 1);
            Regex sm = new Regex("[a-z\\#]{1,4}", RegexOptions.IgnoreCase);
            Match m = sm.Match(filename);
            this.symbol = this.symbol.ToUpper();
            if (!this.symbol.Equals(m.Value.ToUpper())) show(filename + " has symbol name " + m.Value + " but contains ticks for " + symbol);
            this.t = (TickImpl)eSigTick.FromStream(this.symbol,this.inf);
        }


        static void SHOW(string s) { Console.Write(s); }
        static void show(string s) { Console.WriteLine(s); }
    }
}
