using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Research
{
    /// <summary>
    /// used for generating random symbol names in studies.
    /// (eg if you want to randomly walk the security space)
    /// Whenever 'seed' is specified, use a randomized value... eg
    /// (int)DateTime.Now.Ticks or likewise
    /// </summary>
    public class RandomSymbol
    {
        /// <summary>
        /// gets a single random symbol.
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string GetSymbol(int seed) { return GetSymbol(seed, 4); }
        /// <summary>
        /// gets a single random symbol with a specified maximum length
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        public static string GetSymbol(int seed,int maxlength)
        {
            Random r = new Random(seed + DateTime.Now.DayOfYear+ DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second+ DateTime.Now.Millisecond);
            string sym = "";
            int symbolinbaseten = r.Next(1, (int)Math.Pow(26,maxlength));
            int[] alphacodes = BaseTenConvert(symbolinbaseten, 26);
            sym = Alphacodes2string(alphacodes);
            return sym;
        }

        /// <summary>
        /// convert a list of ASCII integers to corresponding string
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static string Alphacodes2string(int[] codes)
        {
            string s = "";
            foreach (int c in codes)
                s += (char)(c+64);
            return s;
        }
        /// <summary>
        /// convert from base ten to another number system
        /// </summary>
        /// <param name="num"></param>
        /// <param name="destbase"></param>
        /// <returns></returns>
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
        /// <summary>
        /// get a random list of symbols of a random length, given seed. (eg (int)DateTime.Now.Ticks
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static string[] GetSymbols(int seed)
        {
            Random r = new Random(seed);
            int symbolcount = r.Next();
            return GetSymbols(seed, 4,symbolcount);
        }
        /// <summary>
        /// get a random list of symbols, given seed, maximum symbol length and desired number of symbols. (seed eg (int)DateTime.Now.Ticks
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="symlength"></param>
        /// <param name="symbolcount"></param>
        /// <returns></returns>
        public static string[] GetSymbols(int seed, int symlength, int symbolcount)
        {

            List<string> l = new List<string>();
            for (int i = 0; i<symbolcount; i++)
                l.Add(GetSymbol(seed+i,symlength));
            return l.ToArray();
        }
    }
}
