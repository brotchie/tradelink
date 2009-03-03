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
        public void Reduce()
        {
            show("Starting scan of: " + PATH);
            ArrayList files = TickFiles();
            for (int file = 1; file <= files.Count; file++)
            {
                lastdate = 0;
                int subfiles = 1;
                TickFile((string)files[file-1]); // set current file
                string ofile = "";
                bool SAME = false;

                while (!SAME && !inf.EndOfStream  && t.hasTick) // while we have ticks in this file,
                {
                    if (fname.Equals(symbol + t.date + ".epf", StringComparison.CurrentCultureIgnoreCase)) { SAME = true; continue; }
                    if (of == null)  // our first day in this input file, open new output file
                    {
                        ofile = symbol + t.date+ ".EPF";
                        this.of = new StreamWriter(PATH + ofile);
                        of.Write(eSigTick.EPFheader(symbol, t.date));
                        SHOW(fname + " ");
                    }
                    if (lastdate == 0) lastdate = t.date;
                    if (lastdate == t.date) { of.WriteLine(eSigTick.ToEPF(t)); continue; }// save first tick
                    subfiles++;
                    lastdate = t.date;
                    // otherwise we gotta close first output file and open new one
                    of.Flush();
                    of.Close();
                    ofile = symbol+t.date+".EPF";
                    SHOW(".");
                    of = new StreamWriter(PATH+ofile);
                    of.Write(eSigTick.EPFheader(symbol, t.date)); // write the header
                    of.WriteLine(eSigTick.ToEPF(t)); // save the 2nd tick and continue processing input ticks
                }
                if (subfiles!=1){ show(" into " + subfiles + " files."); of.Flush(); of.Close(); } // otherwise save and flush it
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
            of = null;
            fname = filename;
            string symline = this.inf.ReadLine();
            string dateline = this.inf.ReadLine();
            Regex se = new Regex("=[a-z-A-Z \\#]+");
            Regex dse = new Regex("=[0-9V]+");
            Regex dee = new Regex("-[0-9V]+");
            MatchCollection r = se.Matches(symline, 0);
            string t = r[0].Value;
            this.symbol = t.Substring(1, t.Length - 1);
            int rem = symbol.IndexOf(' ');
            if (rem!=-1) symbol = symbol.Remove(rem, 1);
            Regex sm = new Regex("[a-z\\#]{1,4}", RegexOptions.IgnoreCase);
            Match m = sm.Match(filename);
            this.symbol = this.symbol.ToUpper();
            if (!this.symbol.Equals(m.Value.ToUpper())) show(filename + " has symbol name " + m.Value + " but contains ticks for " + symbol);
        }


        static void SHOW(string s) { Console.Write(s); }
        static void show(string s) { Console.WriteLine(s); }
    }
}
