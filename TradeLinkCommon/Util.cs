using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Net;
using TradeLink.API;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.InteropServices;

namespace TradeLink.Common
{
    /// <summary>
    /// Utility class holding commonly used properties for TradeLinkSuite
    /// </summary>
    public class Util
    {
        /// <summary>
        /// official program name of the tradelink suite
        /// </summary>
        public const string PROGRAM = "TradeLinkSuite";
        static string REGPATH = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
        static string REGPATH64 = REGPATH + @"\wow6432Node";
        static string KEY_PATH = "Path";
        static string KEY_TICKDATAPATH = "TickDataPath";
        static string KEY_VERSION = "Version";
        static string KEY_TRACKUSAGE = "TrackUsage";


        /// <summary>
        /// determines whether user has consented to application tracking for given program
        /// </summary>
        /// <returns></returns>
        public static bool TrackUsage() { return TrackUsage(PROGRAM); }
        /// <summary>
        /// determines whether user has consented to application tracking for given program
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool TrackUsage(string program) { return TrackUsage(program, null); }
        public static bool TrackUsage(string program,DebugDelegate deb)
        {
            _deb = deb;
            RegistryKey r = Registry.LocalMachine;
            string path = REGPATH + @"\" + program;
            try
            {
                debug("trying standard registry path...");
                string tus = r.OpenSubKey(path).GetValue(KEY_TRACKUSAGE).ToString();
                bool tu = tus.ToUpper() == "YES";
                debug("success.");
                return tu;
            }
            catch (NullReferenceException)
            {
                try
                {
                    debug("standard path failed, must be 64bit... trying 32bit compatibility...");
                    string tus = PlatFormInvoke.GetKeyValue(path, KEY_TRACKUSAGE);
                    bool tu = tus.ToUpper() == "YES";
                    debug("success.");
                    return tu;
                }
                catch (Exception ex)
                {
                    debug("all paths failed, path:" + path);
                    debug("error: " + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                debug("unknown error." + path);
                debug("error: " + ex.Message + ex.StackTrace);
            }
            return true;

        }
        /// <summary>
        /// gets folder where a given program is installed
        /// </summary>
        /// <param name="PROGRAM"></param>
        /// <returns></returns>
        public static string ProgramPath(string PROGRAM) { return ProgramPath(PROGRAM, null); }
        /// <summary>
        /// gets folder where given program was installed and provides debugging information
        /// </summary>
        /// <param name="PROGRAM"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static string ProgramPath(string PROGRAM,DebugDelegate deb)
        {
            _deb = deb;
            string path = string.Empty;
            RegistryKey r = Registry.LocalMachine;
            try
            {
                debug("trying standard registry path...");
                // check registry first
                path = REGPATH + @"\" + PROGRAM;
                return r.OpenSubKey(path).GetValue(KEY_PATH).ToString();
            }
            catch (NullReferenceException)
            {
                try
                {
                    debug("standard path failed, must be 64bit... trying 32bit compatibility...");
                    string ppath = PlatFormInvoke.GetKeyValue(path, KEY_PATH);
                    debug("success.");
                    return ppath;
                }
                catch (Exception ex)
                {
                    debug("all paths failed, path:" + path);
                    debug("error: " + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                // then check program files
                debug("unknown error." + path);
                debug("error: " + ex.Message + ex.StackTrace);
            }
            try
            {
                string fold = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\" + PROGRAM;
                if (Directory.Exists(fold))
                    return fold + @"\";
            }
            catch
            {

            }
            // otherwise assume current directory
            return Environment.CurrentDirectory;
        }

        static string getregvalue(string PROGRAM, string key,DebugDelegate deb)
        {
            _deb = deb;
            RegistryKey r = Registry.LocalMachine;
            string path = string.Empty;
            try
            {
                debug("trying standard registry path...");
                // check registry first
                path = REGPATH + @"\" + PROGRAM;
                return r.OpenSubKey(path).GetValue(key).ToString();
            }
            catch (NullReferenceException)
            {
                try
                {
                    debug("standard path failed, must be 64bit... trying 32bit compatibility...");
                    string ppath = PlatFormInvoke.GetKeyValue(path, key);
                    debug("success.");
                    return ppath;
                }
                catch (Exception ex)
                {
                    debug("all paths failed, path:" + path);
                    debug("error: " + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                // then check program files
                debug("unknown error." + path);
                debug("error: " + ex.Message + ex.StackTrace);
            }
            return string.Empty;
        }

        public static string ProgramDataBasePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\";
            }
        }

        /// <summary>
        /// gets folder where data for a given program should reside
        /// </summary>
        /// <param name="PROGRAM"></param>
        /// <returns></returns>
        public static string ProgramData(string PROGRAM)
        {
            string path = ProgramDataBasePath + PROGRAM;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string TLBaseDir
        {
            get
            {                                  
                string s = ProgramFilesx86() + @"\tradelink\";
                return ProgramFilesx86() + @"\tradelink\";
            }
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        /// <summary>
        /// gets folder where tradelink is installed
        /// </summary>
        public static string TLProgramDir
        {
            get
            {
                return ProgramPath(PROGRAM);
            }
        }
        /// <summary>
        /// path to tradelink tick folder
        /// </summary>
        public static string TLTickDir 
        
        { 
            get 
            {
                string path = getregvalue(PROGRAM, KEY_TICKDATAPATH, null);
                if (path == string.Empty)
                    path = Util.ProgramData("TradeLinkTicks");
                return path;
            } 
        }


        public static TickFileInfo ParseFile(string filepath)
        {
            TickFileInfo tfi;
            tfi.type = TickFileType.Invalid;
            tfi.date = DateTime.MinValue;
            tfi.symbol = "";

            try
            {
                string fn = System.IO.Path.GetFileNameWithoutExtension(filepath);
                string ext = System.IO.Path.GetExtension(filepath).Replace(".", "");
                string date = Regex.Match(fn, "[0-9]{8}$").Value;
                tfi.type = (TickFileType)Enum.Parse(typeof(TickFileType), ext.ToUpper());
                tfi.date = TLD2DT(Convert.ToInt32(date));
                tfi.symbol = Regex.Match(fn, "^[A-Z]+").Value;
            }
            catch (Exception) { tfi.type = TickFileType.Invalid; }
            return tfi;
        }
        /// <summary>
        /// Converts TradeLink date to DateTime (eg 20070926 to "DateTime.Mon = 9, DateTime.Day = 26, DateTime.ShortDate = Sept 29, 2007"
        /// </summary>
        /// <param name="TradeLinkDate"></param>
        /// <returns></returns>
        public static DateTime TLD2DT(int TradeLinkDate)
        {
            if (TradeLinkDate < 10000) throw new Exception("Not a date, or invalid date provided");
            return ToDateTime(TradeLinkDate, 0);
        }
        /// <summary>
        /// Converts TradeLink Time to DateTime.  If not using seconds, put a zero.
        /// </summary>
        /// <param name="TradeLinkTime"></param>
        /// <param name="TradeLinkSec"></param>
        /// <returns></returns>
        public static DateTime TLT2DT(int TradeLinkTime)
        {
            return ToDateTime(0, TradeLinkTime);
        }
        /// <summary>
        /// gets datetime of a tick
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static DateTime TLT2DT(Tick k)
        {
            return ToDateTime(0,k.time);
        }

        /// <summary>
        /// Converts TradeLink Date and Time format to a DateTime. 
        /// eg DateTime ticktime = ToDateTime(tick.date,tick.time);
        /// </summary>
        /// <param name="TradeLinkDate"></param>
        /// <param name="TradeLinkTime"></param>
        /// <param name="TradeLinkSec"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(int TradeLinkDate, int TradeLinkTime)
        {
            int sec = TradeLinkTime % 100;
            int hm = TradeLinkTime % 10000;
            int hour = (int)((TradeLinkTime-hm)/10000);
            int min = (TradeLinkTime - (hour*10000))/100;
            if (sec>59) { sec -= 60; min++; }
            if (min > 59) { hour++; min-=60; }
            int year = 1, day = 1, month = 1;
            if (TradeLinkDate != 0)
            {
                int ym = (TradeLinkDate % 10000);
                year = (int)((TradeLinkDate - ym)/10000);
                int mm = ym % 100;
                month = (int)((ym - mm) / 100);
                day = mm;
            }
            return new DateTime(year, month, day, hour, min, sec);
        }
        /// <summary>
        /// gets fasttime/tradelink time for now
        /// </summary>
        /// <returns></returns>
        public static int DT2FT() { return DT2FT(DateTime.Now); }
        /// <summary>
        /// converts datetime to fasttime format
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static int DT2FT(DateTime d) { return TL2FT(d.Hour, d.Minute, d.Second); }
        /// <summary>
        /// converts tradelink time to fasttime
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="min"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static int TL2FT(int hour, int min, int sec) { return hour * 10000 + min * 100 + sec; }
        /// <summary>
        /// gets fasttime from a tradelink tick
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int TL2FT(Tick t) { return t.time; }
        /// <summary>
        /// gets elapsed seconds between two fasttimes
        /// </summary>
        /// <param name="firsttime"></param>
        /// <param name="latertime"></param>
        /// <returns></returns>
        public static int FTDIFF(int firsttime, int latertime)
        {
            int span1 = FT2FTS(firsttime);
            int span2 = FT2FTS(latertime);
            return span2 - span1;
        }
        /// <summary>
        /// converts fasttime to fasttime span, or elapsed seconds
        /// </summary>
        /// <param name="fasttime"></param>
        /// <returns></returns>
        public static int FT2FTS(int fasttime)
        {
            int s1 = fasttime % 100;
            int m1 = ((fasttime - s1) / 100) % 100;
            int h1 = (int)((fasttime - (m1 * 100) - s1) / 10000);
            return h1 * 3600 + m1 * 60 + s1;
        }
        /// <summary>
        /// adds fasttime and fasttimespan (in seconds).  does not rollover 24hr periods.
        /// </summary>
        /// <param name="firsttime"></param>
        /// <param name="secondtime"></param>
        /// <returns></returns>
        public static int FTADD(int firsttime, int fasttimespaninseconds)
        {
            int s1 = firsttime % 100;
            int m1 = ((firsttime - s1) / 100) % 100;
            int h1 = (int)((firsttime - m1 * 100 - s1) / 10000);
            s1+= fasttimespaninseconds;
            if (s1 >= 60)
            {
                m1 += (int)(s1 / 60);
                s1 = s1 % 60;
            }
            if (m1 >= 60)
            {
                h1 += (int)(m1 / 60);
                m1 = m1 % 60;
            }
            int sum = h1 * 10000 + m1 * 100 + s1;
            return sum;


        }
        /// <summary>
        /// converts fasttime to a datetime
        /// </summary>
        /// <param name="ftime"></param>
        /// <returns></returns>
        public static DateTime FT2DT(int ftime)
        {
            int s = ftime % 100;
            int m = ((ftime - s) / 100) % 100;
            int h = (int)((ftime - m * 100 - s) / 10000);
            return new DateTime(1, 1, 1, h, m, s);
        }

        public const string ZEROBUILD = "0";
        /// <summary>
        /// Gets a number representing the build of an installation.
        /// Build is usually stored in VERSION.TXT and full path is presented via filepath.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        [Obsolete]
        public static int BuildFromFile(string filepath)
        {
            string builds = "";
            int build = 0;
            if (File.Exists(filepath))
            {
                try
                {
                    StreamReader sr = new StreamReader(filepath);
                    builds = sr.ReadToEnd();
                    sr.Close();
                    build = Convert.ToInt32(builds);
                }
                catch (Exception) { }
            }
            return build;
        }

        /// <summary>
        /// get build for installed tradelink
        /// </summary>
        /// <returns></returns>
        public static int TLBuild() { return BuildFromRegistry(PROGRAM); }
        /// <summary>
        /// gets build for specific installed program.
        /// returns 0 if not installed or error.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        [Obsolete]
        public static int BuildFromRegistry(string program) { return ProgramBuild(program, null); }
        [Obsolete]
        public static int BuildFromRegistry(string program, DebugDelegate d)
        {
            return ProgramBuild(program, d);

        }
        /// <summary>
        /// gets build for specific installed program.
        /// returns 0 if not installed or error.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static int ProgramBuild(string program)
        {
            return ProgramBuild(program, null);
        }
        /// <summary>
        /// get build with debugging information on errors
        /// </summary>
        /// <param name="program"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static int ProgramBuild(string program,DebugDelegate deb)
        {
            _deb = deb;
            string path = string.Empty;
            RegistryKey r = Registry.LocalMachine;
            try
            {
                debug("trying standard registry path...");
                path = REGPATH + @"\" + program;
                string ver = r.OpenSubKey(path).GetValue(KEY_VERSION).ToString();
                int build = Convert.ToInt32(ver);
                debug("success.");
                return build;
            }
            catch (NullReferenceException)
            {
                try
                {
                    debug("standard path failed, must be 64bit... trying 32bit compatibility...");
                    string ver = PlatFormInvoke.GetKeyValue(path, KEY_VERSION);
                    int build = Convert.ToInt32(ver);
                    debug("success.");
                    return build;
                }
                catch (Exception ex)
                {
                    debug("all paths failed, path:" + path);
                    debug("error: " + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                debug("unknown error." + path);
                debug("error: " + ex.Message + ex.StackTrace);
            }
            return 0;
        }

        private static DebugDelegate _deb = null;
        static void debug(string msg)
        {
            if (_deb != null)
            {
                _deb(msg);
            }
        }

        /// <summary>
        /// Gets string representing the version of this suite.
        /// </summary>
        /// <returns></returns>
        public static string TLVersion()
        {
            const string major = "0.1.";
            string build = TLBuild().ToString();
            return major + build;
        }
        /// <summary>
        /// Gets a string representing the identity of this suite.
        /// </summary>
        /// <returns></returns>
        public static string TLSIdentity()
        {
            return "TradeLinkSuite-" + TLVersion();
        }
        /// <summary>
        /// get long for current date + time
        /// </summary>
        /// <returns></returns>
        public static long ToTLDateTime() { return (ToTLDate() * 100000000) + ToTLTime(); }
        /// <summary>
        /// get long for date + time
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToTLDateTime(DateTime dt) { return (ToTLDate(dt) * 100000000) + ToTLTime(dt); }
        /// <summary>
        /// gets TradeLink date for today
        /// </summary>
        /// <returns></returns>
        public static int ToTLDate() { return ToTLDate(DateTime.Now); }
        /// <summary>
        /// Converts a DateTime to TradeLink Date (eg July 11, 2006 = 20060711)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int ToTLDate(DateTime dt)
        {
            return (dt.Year * 10000) + (dt.Month * 100) + dt.Day;
        }
        /// <summary>
        /// Converts a DateTime.Ticks values to TLDate (eg 8million milliseconds since 1970 ~= 19960101 (new years 1996)
        /// </summary>
        /// <param name="DateTimeTicks"></param>
        /// <returns></returns>
        public static int ToTLDate(long DateTimeTicks)
        {
            return ToTLDate(new DateTime(DateTimeTicks));
        }
        /// <summary>
        /// gets tradelink time for now
        /// </summary>
        /// <returns></returns>
        public static int ToTLTime() { return DT2FT(DateTime.Now); }
        /// <summary>
        /// gets tradelink time from date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ToTLTime(DateTime date)
        {
            return DT2FT(date);
        }


        /// <summary>
        /// Converts a TLDate integer format into an array of ints
        /// </summary>
        /// <param name="fulltime">The fulltime.</param>
        /// <returns>int[3] { year, month, day}</returns>
        static int[] TLDateSplit(int fulltime)
        {
            int[] splittime = new int[3]; // year, month, day
            splittime[2] = (int)((double)fulltime/10000);
            splittime[1] = (int)((double)(fulltime - (splittime[2]*10000))/100);
            double tmp = (int)((double)fulltime/100);
            double tmp2 = (double)fulltime / 100;
            splittime[0] = (int)(Math.Round(tmp2 - tmp,2, MidpointRounding.AwayFromZero) * 100);
            return splittime;
        }



        /// <summary>
        /// Tests if two dates are the same, given a mask as DateMatchType.
        /// 
        /// ie, 20070605 will match 20070705 if DateMatchType is Day or Year.
        /// </summary>
        /// <param name="fulldate">The fulldate in TLDate format (int).</param>
        /// <param name="matchdate">The matchdate to test against (int).</param>
        /// <param name="dmt">The "mask" that says what to pay attention to when matching.</param>
        /// <returns></returns>
        public static bool TLDateMatch(int fulldate,int matchdate, DateMatchType dmt)
        {
            const int d = 0, m=1,y=2;
            if (dmt == DateMatchType.None) 
                return false;
            bool matched = true;
            // if we're requesting a day match,
            if ((dmt & DateMatchType.Day) == DateMatchType.Day)
                matched &= TLDateSplit(fulldate)[d] == TLDateSplit(matchdate)[d];
            if ((dmt & DateMatchType.Month)==DateMatchType.Month)
                matched &= TLDateSplit(fulldate)[m] == TLDateSplit(matchdate)[m];
            if ((dmt & DateMatchType.Year)== DateMatchType.Year)
                matched &= TLDateSplit(fulldate)[y] == TLDateSplit(matchdate)[y];
            return matched;
        }

        /// <summary>
        /// Obtains a version out of a string that contains version + other information.
        /// </summary>
        /// <param name="ver">string containing version</param>
        /// <returns>version number</returns>
        public static int CleanVer(string ver)
        {
            Regex re = new Regex("[0-9]+");
            Match m = re.Match(ver);
            if (m.Success) return Convert.ToInt32(m.Value);
            return 0;
        }
        /// <summary>
        /// Provide date in TLDate format, returns whether market (NYSE) closes early on this day.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
		public static bool isEarlyClose(int today) 
        {
            try
            {
                return GetCloseTime().Contains(today);
            }
            catch (Exception) { return false; }
        }
        /// <summary>
        /// Gets early close time for a given date.   Returns zero if not an early close.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static int GetEarlyClose(int today)
        {
            try
            {
                return (int)GetCloseTime()[today];
            }
            catch (Exception) { return 0; }
        }
        static Hashtable GetCloseTime()
        {
            StreamReader f = new StreamReader("EarlyClose.csv");
            string[] r = new string[2];
            string line = "";
            Hashtable h = new Hashtable();
            while ((line = f.ReadLine())!=null)
            {
                r = line.Split(',');
                h.Add(Convert.ToInt32(r[0]),Convert.ToInt32(r[1]));
            }
            f.Close();
            return h; 
        }
        /// <summary>
        /// Converts list of trades to a delimited file readable by excel, R, matlab, google spreadsheets, etc.
        /// </summary>
        /// <param name="stocktrades"></param>
        /// <param name="delimiter"></param>
        /// <param name="filepath"></param>
        public static void FillsToText(List<TradeImpl> stocktrades,char delimiter,string filepath)
        { // works on a queue of Trade objects
            StreamWriter sw = new StreamWriter(filepath, false);
            sw.WriteLine("Date,Time,Symbol,Side,xSize,xPrice,Comment");
            foreach (TradeImpl t in stocktrades)
                sw.WriteLine(t.ToString(delimiter));
            sw.Close();
        }

        /// <summary>
        /// converts a trade to an array of comma-delimited string data also containing closedPL, suitable for output to file for reading by excel, R, matlab, etc.
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        public static string[] TradesToClosedPL(Trade trade)
        {
            List<Trade> trades = new List<Trade>();
            trades.Add(trade);
            return TradesToClosedPL(trades);
        }

        /// <summary>
        /// Converts a list of trades to an array of comma-delimited string data also containing closedPL, suitable for output to file for reading by excel, R, matlab, etc.
        /// </summary>
        /// <param name="tradelist"></param>
        /// <returns></returns>
        public static string[] TradesToClosedPL(List<Trade> tradelist) { return TradesToClosedPL(tradelist, ','); }
        /// <summary>
        /// Converts a list of trades to an array of delimited string data also containing closedPL, suitable for output to file for reading by excel, R, matlab, etc.
        /// </summary>
        /// <param name="tradelist"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] TradesToClosedPL(List<Trade> tradelist, char delimiter)
        {
            List<string> rowoutput = new List<string>();
            PositionTracker pt = new PositionTracker();
            
            foreach (TradeImpl t in tradelist)
            {
                string r = t.ToString(delimiter,false) + delimiter;
                string s = t.symbol;
                decimal cpl = 0;
                decimal opl = 0;
                int csize = 0;
                cpl = pt.Adjust(t);
                opl = Calc.OpenPL(t.xprice, pt[s]); // get any leftover open pl
                if (cpl != 0) csize = t.xsize; // if we closed any pl, get the size
                string[] pl = new string[] { opl.ToString("f2"), cpl.ToString("f2"), pt[s].Size.ToString(), csize.ToString(), pt[s].AvgPrice.ToString("f2") };
                r += string.Join(delimiter.ToString(), pl);
                rowoutput.Add(r);
            }
            return rowoutput.ToArray();

        }
        /// <summary>
        /// Converts a list of trades to delimited text file containing closedPL, suitable for reading by excel, R, matlab, etc.
        /// </summary>
        /// <param name="tradelist"></param>
        /// <param name="delimiter"></param>
        /// <param name="filepath"></param>
        public static void ClosedPLToText(List<Trade> tradelist, char delimiter, string filepath) { ClosedPLToText(tradelist, delimiter, filepath,false); }
        public static void ClosedPLToText(List<Trade> tradelist, char delimiter, string filepath, bool generateheaderOnEmpty) { ClosedPLToText(tradelist, delimiter, filepath, false, false); }
        public static void ClosedPLToText(Trade trade, string filepath)
        {
            List<Trade> trades = new List<Trade>();
            trades.Add(trade);

            ClosedPLToText(trades, ',', filepath, false, true);
        }
        public static void ClosedPLToText(List<Trade> tradelist, char delimiter, string filepath,bool generateheaderOnEmpty, bool append)
        {
            if ((tradelist.Count == 0) && !generateheaderOnEmpty) return;
            bool exists = File.Exists(filepath);
            StreamWriter sw = new StreamWriter(filepath, append);
            string header = string.Join(delimiter.ToString(), Enum.GetNames(typeof(TradePLField)));
            if (!append || !exists)
                sw.WriteLine(header);
            string[] lines = TradesToClosedPL(tradelist, delimiter);
            foreach (string line in lines)
                sw.WriteLine(line);
            sw.Close();           
        }
            

        static bool IsResponse(Type t) 
        {
            return typeof(Response).IsAssignableFrom(t);
        }
        /// <summary>
        /// Gets full Response names found in a given file.
        /// </summary>
        /// <param name="boxdll">The file path of the assembly containing the boxes.</param>
        /// <returns></returns>
        public static List<string> GetResponseList(string dllfilepath)
        {
            List<string> reslist = new List<string>();
            if (!File.Exists(dllfilepath)) return reslist;
            Assembly a;
            try
            {
                  a = Assembly.LoadFile(dllfilepath);
            }
            catch (Exception ex) { reslist.Add(ex.Message); return reslist; }
            return GetResponseList(a);
        }
        /// <summary>
        /// Gets all Response names found in a given assembly.  Names are FullNames, which means namespace.FullName.  eg 'BoxExamples.BigTradeUI'
        /// </summary>
        /// <param name="boxdll">The assembly.</param>
        /// <returns>list of response names</returns>
        public static List<string> GetResponseList(Assembly responseassembly) { return GetResponseList(responseassembly, null); }
        public static List<string> GetResponseList(Assembly responseassembly,DebugDelegate deb)
        {
            List<string> reslist = new List<string>();
            Type[] t;
            try
            {
                t = responseassembly.GetTypes();
                for (int i = 0; i < t.GetLength(0); i++)
                    if (IsResponse(t[i])) reslist.Add(t[i].FullName);
            }
            catch (Exception ex) 
            {
                if (deb != null)
                {
                    deb(ex.Message + ex.StackTrace);
                }
            }

            return reslist;
        }

        /// <summary>
        /// Gets a user-friendly string for Broker and TradeLink error messages.
        /// </summary>
        /// <param name="errorcode">The errorcode.</param>
        /// <returns></returns>
        public static string PrettyError(Providers provider, int errorcode)
        {
            // it's a tradelink error code
            if (errorcode<=(int)MessageTypes.SYMBOL_NOT_LOADED)
                return Enum.GetName(typeof(MessageTypes),errorcode);
            string err = "";
            switch (provider)
            {
                case Providers.Assent:
                    err = Enum.GetName(typeof(AnvilSendOrderError), errorcode);
                    break;
                case Providers.Sterling:
                    err = Enum.GetName(typeof(SterlingSubmitOrderError), errorcode);
                    break;
                case Providers.LightspeedDesktop:
                    err = Enum.GetName(typeof(LightspeedDesktopSendOrderMsg), errorcode);
                    break;
            }
            if ((err != null) && (err != "") && (err != " ")) return err;
            return "UnknownError: " + errorcode.ToString();
        }


        /// <summary>
        /// gets list of readable tickfiles in top level of a folder.
        /// 2nd dimension of list is size of file in bytes (as string)
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string[,] TickFileIndex(string folder) { return TickFileIndex(folder, TikConst.WILDCARD_EXT); }
        /// <summary>
        /// builds list of readable tick files with given extension found in top level of folder
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="tickext"></param>
        /// <returns></returns>
        public static string[,] TickFileIndex(string Folder, string tickext) { return TickFileIndex(Folder, tickext, false, null); }
        /// <summary>
        /// builds list of readable tickfiles found in given folder
        /// </summary>
        /// <param name="Folder">path containing tickfiles</param>
        /// <param name="tickext">file extension</param>
        /// <returns></returns>
        public static string[,] TickFileIndex(string Folder, string tickext, bool searchSubFolders) { return TickFileIndex(Folder, tickext, searchSubFolders, null); }
        /// <summary>
        /// builds list of readable tickfiles (and their byte-size) found in folder
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="tickext"></param>
        /// <param name="searchSubFolders"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string[,] TickFileIndex(string Folder, string tickext, bool searchSubFolders, DebugDelegate debug)
        {
            string[] _tickfiles = Directory.GetFiles(Folder, tickext);
            DirectoryInfo di = new DirectoryInfo(Folder);
            FileInfo[] fi = di.GetFiles(tickext, searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            string[,] index = new string[_tickfiles.Length, 2];
            int i = 0;
            int qtr = fi.Length / 4;
            foreach (FileInfo thisfi in fi)
            {
                if ((debug != null) && (i % qtr == 0))
                    debug((fi.Length - i).ToString("N0") + " files remaining to index...");
                index[i, 0] = thisfi.Name;
                index[i, 1] = thisfi.Length.ToString();
                i++;
            }
            return index;
        }



        public static string decode(string data)
        {
            string s = string.Empty;
            for (int i = 0; i <= data.Length - 2; i += 2)
                s += Convert.ToChar(Convert.ToUInt32(data.Substring(i, 2), 16)).ToString();
            return s;
        }

        /// <summary>
        /// dumps public properties and fields of an object as an xml string
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string DumpObjectProperties(Object o) { return DumpObjectProperties(o, null); }
        /// <summary>
        /// dumps public properties and fields as xml string, with optional debugging for errors
        /// </summary>
        /// <param name="o"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static string DumpObjectProperties(Object o,DebugDelegate deb)
        {
            try
            {
                System.Type t = o.GetType();
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(t);
                System.IO.StringWriter sw = new System.IO.StringWriter();
                xs.Serialize(sw, o);
                sw.Close();
                return sw.ToString();
            }
            catch (Exception ex)
            {
                string inner = string.Empty;
                try
                {
                    inner = (ex.InnerException == null) ? string.Empty : " inner err: " + ex.InnerException.Message + ex.InnerException.StackTrace;
                }
                catch { }
                if (deb != null)
                    deb("Error dumping: "+o.ToString()+" "+ex.Message + ex.StackTrace+inner);
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PROGRAM"></param>
        /// <returns></returns>
        public static int ProgramCount(string PROGRAM)
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            int count = 0;
            foreach (System.Diagnostics.Process p in ps)
                if (p.ProcessName.ToLower().Contains(PROGRAM.ToLower()))
                    count++;
            return count;
        }
        /// <summary>
        /// convert any structure/type to a string (can be converted back using Deserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string Serialize<T>(T o) { return Serialize(o, false, null); }
        /// <summary>
        /// convert any structure/type to a string (can be converted back using Deserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string Serialize<T>(T o, DebugDelegate debug) { return Serialize(o, false, debug); }
        /// <summary>
        /// convert any structure/type to a string (can be converted back using Deserialize)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="compress"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string Serialize<T>(T o, bool compress, DebugDelegate debug)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                System.IO.StringWriter sw = new System.IO.StringWriter(sb);
                XmlSerializer xs = new XmlSerializer(o.GetType());
                xs.Serialize(sw, o);
                sw.Close();
                if (!compress)
                    return sb.ToString();
                string r = TradeLink.Common.GZip.Compress(sb.ToString());
                return r;
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    string inner = string.Empty;
                    try
                    {
                        inner = (ex.InnerException == null) ? string.Empty : " inner err: " + ex.InnerException.Message + ex.InnerException.StackTrace;
                    }
                    catch { }
                    debug("unable to save " + TradeLink.Common.Util.DumpObjectProperties(o));
                    debug(ex.Message + ex.StackTrace+inner);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// deserialize a structure/type from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string msg) { return Deserialize<T>(msg, false, null); }
        /// <summary>
        /// deserialize a structure/type from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string msg, DebugDelegate debug) { return Deserialize<T>(msg, false, debug); }
        /// <summary>
        /// deserialize a structure/type from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="uncompress"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string msg, bool uncompress, DebugDelegate debug)
        {

            try
            {
                if (uncompress)
                    msg = TradeLink.Common.GZip.Uncompress(msg);
                System.IO.StringReader sr = new System.IO.StringReader(msg);
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T O = (T)xs.Deserialize(sr);
                sr.Close();
                return O;
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    string inner = string.Empty;
                    try
                    {
                        inner = (ex.InnerException == null) ? string.Empty : " inner err: " + ex.InnerException.Message + ex.InnerException.StackTrace;
                    }
                    catch { }
                    debug("UNABLE TO read: " + msg);
                    debug(ex.Message + ex.StackTrace+inner);
                }
            }
            return default(T);
        }

        /// <summary>
        /// create an xml file from any data structure (can be restored with FromFile)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool ToFile<T>(T o, string file) { return ToFile<T>(o, file, null); }
        /// <summary>
        /// create an xml file from any data structure (can be restored with FromFile)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="file"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool ToFile<T>(T o, string file, DebugDelegate debug)
        {
            try
            {
                string msg = Serialize(o, debug);
                if (msg == string.Empty) return false;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false);
                sw.WriteLine(msg);
                sw.Close();

            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug("error writing filename: " + file);
                    debug(ex.Message + ex.StackTrace);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// restore data structure(s) from a file (created with ToFile)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool FromFile<T>(string file, ref T o) { return FromFile(file, ref o, null); }
        /// <summary>
        /// restore data structure(s) from a file (created with ToFile)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="o"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static bool FromFile<T>(string file, ref T o, DebugDelegate debug)
        {
            try
            {
                if (!File.Exists(file))
                {
                    if (debug != null)
                    {
                        debug("file does not exist: " + file);
                        return false;
                    }
                }
                System.IO.StreamReader sr = new System.IO.StreamReader(file);
                o = Deserialize<T>(sr.ReadToEnd(), debug);
                sr.Close();
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug("error writing filename: " + file);
                    debug(ex.Message + ex.StackTrace);
                    return false;
                }
            }
            return (o != null) && (o.GetType() == typeof(T));
        }

        public static bool touch(string file) { return touch(file, string.Empty, false); }
        public static bool touch(string file, string data) { return touch(file, string.Empty, false); }
        public static bool touch(string file, string data, bool append)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, append);
                sw.WriteLine(data);
                sw.Close();
            }
            catch { return false; }
            return true;
        }

        public static string rxr(string input, string pattern, string replace) { return rxr(input, pattern, replace, false); }
        public static string rxr(string input, string pattern, string replace, bool casesens)
        {
            if (casesens)
                return Regex.Replace(input, pattern, replace);
            return Regex.Replace(input, pattern, replace, RegexOptions.IgnoreCase);
        }

        public static string rxm(string input, string pattern) { return rxm(input, pattern, false); }
        public static string rxm(string input, string pattern, bool casesens)
        {
            Match m = casesens ? Regex.Match(input, pattern) : Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (!m.Success)
                return string.Empty;
            return m.Value;
        }
        public static bool rxmok(string input, string pattern) { return rxmok(input, pattern, false); }
        public static bool rxmok(string input, string pattern, bool casesens)
        {
            if (casesens)
                return Regex.IsMatch(input, pattern);
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }


        private class PlatFormInvoke
        {
            [DllImport("advapi32.dll", EntryPoint = "RegOpenKeyEx")]
            internal static extern int RegOpenKeyEx_DllImport(
                UIntPtr hKey,
                string subKey,
                uint options,
                int sam,
                out IntPtr phkResult);


            [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
            static extern int RegQueryValueEx_DllImport(
                IntPtr hKey,
                string lpValueName,
                int lpReserved,
                out uint lpType,
                System.Text.StringBuilder lpData,
                ref uint lpcbData);


            internal static string GetKeyValue(string strSubKey, string strKey)
            {

                UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
                const int KEY_WOW64_32KEY = 0x0200;
                const int KEY_QUERY_VALUE = 0x1;

                IntPtr hKeyVal;
                uint lpType;
                uint lpcbData = 0;
                System.Text.StringBuilder pvData = new System.Text.StringBuilder(1024);
                int valueRet;



                string returnPath = String.Empty;
                unchecked
                {
                    try
                    {   //Open the required key path
                        valueRet = RegOpenKeyEx_DllImport(HKEY_LOCAL_MACHINE, strSubKey, 0, KEY_QUERY_VALUE | KEY_WOW64_32KEY, out hKeyVal);

                        //Retreive the key value
                        valueRet = RegQueryValueEx_DllImport(hKeyVal, strKey, 0, out lpType, pvData, ref lpcbData);

                        valueRet = RegQueryValueEx_DllImport(hKeyVal, strKey, 0, out lpType, pvData, ref lpcbData);

                        returnPath = pvData.ToString();
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                }
                return returnPath;
            }

            



        }
    }

    enum SterlingSubmitOrderError
    {
        No_Errors = 0,
        Invalid_Account = -1,
        Invalid_Side = -2,
        Invalid_Qty = -3,
        Invalid_Symbol = -4,
        Invalid_PriceType = -5,
        Invalid_Tif = -6,
        Invalid_Destination = -7,
        Exposure_Limit_Violation = -8,
        NYSE_Rules_Violation = -9,
        NYSE_Second_Violation = -10,
        Disable_SelectNet_Short_Sales = -11,
        Long_Sale_Position_Rules_Violation = -12,
        Short_Sale_Position_Rules_Violation = -13,
        GTC_Orders_Not_Enabled = -14,
        ActiveX_API_Not_Enabled = -15,
        Sterling_Trader_Pro_is_Offline = -16,
        Security_Not_Marked_as_Located = -17,
        Order_Size_Violation = -18,
        Position_Limit_Violation = -19,
        Buying_Power_Margin_Control_Violation = -20,
        PL_Control_Violation = -21,
        Account_Not_Enabled_for_this_Product = -22,
        Trader_Not_Enabled_for_Futures = -23,
        Minimum_Balance_Violation = -24,
        Trader_Not_Enabled_To = -25,
        Order_dollar_limit_exceeded = -26,
        Trade_Not_Enabled_for_Options = -27,
        Soft_Share_Limit_Exceeded = -28,
        For_Internal_Use_Only = -29,
        For_Internal_Use_Only2 = -30,
        Sell_to_Open_Position_Not_Enabled_for_Options = -31,
        Allow_Close_and_Cancel_Only = -32,
        Trader_not_enabled_for_security_locating = -33,
        Order_not_able_to_be_replaced_ReplaceOrder_only = -34,
        Trader_not_enabled_for_Buy_to_Cover = -35,
        Invalid_maturity_date = -36,
        Only_one_cancel_or_replace_allowed_per_order_per_second = -37,
        Account_maximum_position_value_for_this_symbol_exceeded = -38,
        Symbol_Violates_Account_MinMax_PriceSettings = -39,
        Quote_Unavailable_Order_dollar_limit = -40,
        Quote_Unavailable_Maximum_Position_Cost = -41,
        Quote_Unavailable_Buying_Power = -42 ,
        Quote_Unavailable_Margin_Control = -43 ,
        Floating_BP_Violation = -44,
        Market_order_would_remove_liquidity_frontend_setting = -45,
        Not_enabled_for_server_stop_orders = -46,
        Not_enabled_for_trail_stop_orders = -47,
        Order_would_exceed_the_Max_Open_orders_per_side_on_this_symbol_= -48
    }


    enum LightspeedDesktopSendOrderMsg
    {
        ORDER_SENT_OK,
        SOES_ORDER_DISABLED,
        MM_IN_EXCLUSION_LIST,
        ZERO_SHARES_ORDERED,
        EXECUTIONS_DISABLED,
        BUYING_POWER_EXCEEDED,
        SHORT_SELL_VIOLATION,
        STOCK_NOT_SHORTABLE,
        EXECUTOR_NOT_CONNECTED,
        MAXORDERSHARES_EXCEEDED,
        WAIT_CONSTRAINT_VIOLATION,
        STOCK_HALTED,
        MKXT_BOOK_OR_KILL,
        SMALL_CAPS_NOT_SOESABLE,
        OWN_CROSSING,
        CANNOT_TRADE_SYMBOL,
        CANNOT_TRADE_BULLETS_AFTER_MARKET,
        MAXBULLETS_EXCEEDED,
        MARKET_HALTED,
        FUTURES_MARGINABILITY_UNKNOWN,
        NO_EXEMPT_ORDERS,
        TRADINGMONITOR_BLOCKED_ORDER,
        DECLINED_AT_CONFIRM_BY_USER,
        SUPERVISOR_CREATED_NO_ORDERS,
        ROUTING_BLOCKED_ORDER,
        OTHER_REJECTION,
        EXECUTOR_NOT_LOGGED_IN,
        UNINITIALIZED_SUMMARY,
        INVALID_SUMMARY,
        INVALID_ORDER_TYPE
    }
 

    enum AnvilSendOrderError
    {
        SO_OK,
        SO_NO_ACCOUNT,
        SO_NO_SERVER_CONNECTION,
        SO_STOCK_NOT_INITIALIZED,
        SO_OPTION_TRADING_NOT_ALLOWED,
        SO_OPTION_CANNOT_SELL_UNCOVERED,
        SO_BUYING_POWER_EXCEEDED,
        SO_SIZE_ZERO,
        SO_SIZE_TOO_SMALL,
        SO_INCORRECT_PRICE,
        SO_INCORRECT_SIDE,
        SO_NO_BULLETS_FOR_CHEAP_STOCK,
        SO_NO_SHORTSELL_FOR_CHEAP_STOCK,
        SO_NO_SHORTSELL_FOR_HARD_TO_BORROW_STOCK,
        SO_NO_ONOPENORDER_FOR_NASDAQ_STOCK,
        SO_NO_ONCLOSEORDER_FOR_NASDAQ_STOCK,
        SO_NO_ONCLOSEORDER_AGAINST_IMBALANCE_AFTER_1540,
        SO_NO_ONCLOSEORDER_AFTER_1600,
        SO_NO_SIZEORDER_FOR_NON_NASDAQ_STOCK,
        SO_NO_STOPORDER_FOR_NASDAQ_STOCK,
        SO_MAX_ORDER_SIZE_EXCEEDED,
        SO_MAX_POSITION_SIZE_EXCEEDED,
        SO_MAX_POSITION_VALUE_EXCEEDED,
        SO_TRADING_LOCKED,
        SO_TRADING_HISTORY_NOT_LOADED,
        SO_NO_SOES_ORDER_WHEN_MARKET_CLOSED,
        SO_NO_SDOT_ORDER_WHEN_MARKET_CLOSED,
        SO_MAX_OPEN_POSITIONS_EXCEEDED,
        SO_MAX_POSITION_PENDING_ORDERS_EXCEEDED,
        SO_MAX_TOTAL_SHARES_EXCEEDED,
        SO_MAX_TRADED_SHARES_EXCEEDED,
        SO_AMEX_ORDER_EXECUTION_BLOCKED,
        SO_NYSE_ORDER_EXECUTION_BLOCKED,
        SO_NASDAQ_ORDER_EXECUTION_BLOCKED,
        SO_ARCA_ORDER_EXECUTION_BLOCKED,
        SO_VENUE_BLOCKED,
        SO_ARCA_ODD_LOT_ORDER_BLOCKED,
        SO_MAX_LOSS_EXCEEDED,
        SO_MAX_LOSS_PER_STOCK_EXCEEDED,
        SO_MAX_OPEN_LOSS_PER_STOCK_EXCEEDED,
        SO_NYSE_ODD_LOT_VIOLATION,
        SO_INVALID_STAGING_CONTEXT,

        SO_SHORT_EXEMPT_NOT_INSTITUTIONAL,
        SO_SELL_SIZE_GREATER_THAN_POSITION,
        SO_NO_SHORT_BEFORE_SELL_COVER_POSITION,
        SO_SHORT_CAN_EXECUTE_BEFORE_SELL,
        SO_SAME_PRICE_VENUE_OVERSELL,
        SO_STAGING_TICKET_EXCEEDED,

        SO_DESTINATION_NOT_RECOGNIZED,

        SO_HIT_OWN_ORDERS,


        SO_POTENTIAL_OVERSELL,

        SO_BLOCK_ERRONEOUS_TRADE,
        SO_WARN_ERRONEOUS_TRADE,
        SO_ALLOW_ERRONEOUS_TRADE,

        SO_SDOT_ROUTING_BLOCK,
        SO_SDOT_NEWPOS_BLOCK,
        SO_ARCA_NEWPOS_BLOCK,
    }
    public enum TickFileType
    {
        Invalid = 0,
        EPF,
        IDX,
        TIK,
    }

    public struct TickFileInfo
    {
        public string symbol;
        public DateTime date;
        public TickFileType type;
    }

    public enum DateMatchType
    {
        None = 0,
        Day = 1,
        Month = 2,
        Year = 4,
    }

    public enum TradePLField
    {
        Date = 0,
        Time,
        Symbol,
        Side,
        xSize,
        xPrice,
        Comment,
        OpenPL,
        ClosedPL,
        OpenSize,
        ClosedSize,
        AvgPrice,
    }


    
}
