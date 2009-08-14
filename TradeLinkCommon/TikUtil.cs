using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TradeLink.Common
{
    public static class TikUtil
    {
        /// <summary>
        /// converts EPF files to tradelink tick files in current directory
        /// </summary>
        /// <param name="args"></param>
        public static void EPF2TIK(string[] args)
        {
            // get a list of epf files
            foreach (string file in args)
            {
                SecurityImpl sec = SecurityImpl.FromTIK(file);
                sec.HistSource.gotTick += new TradeLink.API.TickDelegate(HistSource_gotTick);
                _tw = new TikWriter(sec.Symbol);
                while (sec.NextTick())
                _tw.Close();
            }
        }

        static void HistSource_gotTick(TradeLink.API.Tick t)
        {
            _tw.newTick(t);
        }

        private static TikWriter _tw;

        /// <summary>
        /// finds a group of files with a given extension
        /// </summary>
        /// <param name="EXT"></param>
        /// <returns></returns>
        public static string[] GetFiles(string EXT) { return GetFiles(Environment.CurrentDirectory, EXT); }
        public static string[] GetFiles(string path, string EXT)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fis = di.GetFiles(EXT);
            List<string> names = new List<string>();
            foreach (FileInfo fi in fis)
                names.Add(fi.FullName);
            return names.ToArray();


        }
    }
}
