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
            int symbolinbaseten = r.Next(1, (int)Math.Pow(26,maxlength));
            int[] alphacodes = BaseTenConvert(symbolinbaseten, 26);
            sym = Alphacodes2string(alphacodes);
            return sym;
        }

        public static string Alphacodes2string(int[] codes)
        {
            string s = "";
            foreach (int c in codes)
                s += (char)(c+64);
            return s;
        }

        public static int[] BaseTenConvert(int num, int destbase)
        {
            List<int> ordinals = new List<int>();
            if (destbase==0) return ordinals.ToArray();
            int rem = num % destbase;
            int ans = (int)num / destbase;
            while (ans!=0)
            {
                ordinals.Add(rem);
                rem = ans % destbase;
                ans = (int)ans / destbase;
            }
            ordinals.Add(rem);
            return ordinals.ToArray();
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
