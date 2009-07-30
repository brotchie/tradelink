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
                SecurityImpl sec = SecurityImpl.FromFile(file);
                TikWriter tw = new TikWriter(sec.Symbol);
                while (sec.hasHistorical)
                {
                    tw.newTick(sec.NextTick());
                }
                tw.Close();
            }
        }
        /// <summary>
        /// finds a group of files with a given extension
        /// </summary>
        /// <param name="EXT"></param>
        /// <returns></returns>
        public static string[] GetFiles(string EXT)
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] fis = di.GetFiles(EXT);
            List<string> names = new List<string>();
            foreach (FileInfo fi in fis)
                names.Add(fi.FullName);
            return names.ToArray();


        }
    }
}
