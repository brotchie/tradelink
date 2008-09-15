using System;
using System.Collections.Generic;
using System.Text;

namespace Picker
{
    public class RandomSymbol
    {
        public static string GetSymbol(int seed) { return GetSymbol(seed, 4); }
        public static string GetSymbol(int seed,int maxlength)
        {
            Random r = new Random(seed + DateTime.Now.DayOfYear+ DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second+ DateTime.Now.Millisecond);
            string sym = "";
            sym += (char)r.Next(65, 91);
            sym += (char)r.Next(65, 91);
            sym += (char)r.Next(65, 91);
            sym += (char)r.Next(65, 91);
            int len = r.Next(1, 4);
            return sym.Substring(0,len);
        }

        public static string[] GetSymbols(int seed)
        {
            Random r = new Random(seed);
            int symbolcount = r.Next();
            return GetSymbols(seed, 4,symbolcount);
        }
        public static string[] GetSymbols(int seed, int symlength, int symbolcount)
        {

            List<string> l = new List<string>();
            for (int i = 0; i<symbolcount; i++)
                l.Add(GetSymbol(seed+i,symlength));
            return l.ToArray();
        }
    }
}
