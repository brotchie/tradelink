using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;

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

        public static bool IsFileWritetable(string path)
        {
            FileStream stream = null;

            try
            {
                if (!System.IO.File.Exists(path))
                    return true;
                System.IO.FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;
        }

        static void HistSource_gotTick(TradeLink.API.Tick t)
        {
            _tw.newTick((TickImpl)t);
        }

        private static TikWriter _tw;

        /// <summary>
        /// finds a group of files with a given extension
        /// </summary>
        /// <param name="EXT"></param>
        /// <returns></returns>
        public static string[] GetFiles() { return GetFiles(Util.TLTickDir, TikConst.WILDCARD_EXT); }
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

        /// <summary>
        /// get tick files created today from default folder
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFilesFromDate() { return GetFilesFromDate(Util.TLTickDir, Util.ToTLDate()); }
        /// <summary>
        /// get tick files created today
        /// </summary>
        /// <param name="tickfolder"></param>
        /// <returns></returns>
        public static List<string> GetFilesFromDate(string tickfolder) { return GetFilesFromDate(tickfolder, Util.ToTLDate()); }
        /// <summary>
        /// get tick files created on a certain date
        /// </summary>
        /// <param name="tickfolder"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<string> GetFilesFromDate(string tickfolder, int date)
        {
            string[] files = TikUtil.GetFiles(tickfolder, TikConst.WILDCARD_EXT);
            List<string> matching = new List<string>();
            foreach (string file in files)
            {
                SecurityImpl sec = SecurityImpl.SecurityFromFileName(file);
                string symfix = System.IO.Path.GetFileNameWithoutExtension(sec.Name);
                if (sec.Date == date)
                    matching.Add(file);
            }
            return matching;
        }


        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        public static bool TicksToFile(Tick[] ticks) { return TicksToFile(ticks, null); }
        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="debs"></param>
        /// <returns></returns>
        public static bool TicksToFile(Tick[] ticks, DebugDelegate debs)
        {
            try
            {
                TikWriter tw = new TikWriter();
                foreach (Tick k in ticks)
                    tw.newTick(k);
                tw.Close();
                if (debs != null)
                    debs(tw.RealSymbol + " saved " + tw.Count + " ticks to: " + tw.Filepath);

            }
            catch (Exception ex)
            {
                if (debs != null)
                    debs(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>
        /// create file from ticks
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="debs"></param>
        /// <param name="tw"></param>
        /// <returns></returns>
        public static bool TicksToFile(Tick[] ticks, DebugDelegate debs, TikWriter tw)
        {
            try
            {
                foreach (Tick k in ticks)
                    tw.newTick(k);
                tw.Close();
                if (debs != null)
                    debs(tw.RealSymbol + " saved " + tw.Count + " ticks to: " + tw.Filepath);

            }
            catch (Exception ex)
            {
                if (debs != null)
                    debs(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
        /// <summary>
        /// create ticks from bars on default interval
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public static Tick[] Barlist2Tick(BarList bl)
        {
            List<Tick> k = new List<Tick>(bl.Count * 4);
            foreach (Bar b in bl)
                k.AddRange(BarImpl.ToTick(b));
            return k.ToArray();
        }

    }
}
