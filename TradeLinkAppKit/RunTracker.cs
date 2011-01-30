using System;
using System.Collections.Generic;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    public struct RunTracker
    {
        public string BuildName;
        public string Program;
        public int Build;
        public int Runs;
        public int LastRunDate;
        public bool isStable;

        public RunTracker(string program, string buildname, int build)
        {
            isStable = false;
            Program = program;
            Build = build;
            Runs = 1;
            LastRunDate = Util.ToTLDate();
            BuildName = buildname;
        }

        public RunTracker(string program, int build)
        {
            isStable = false;
            Program = program;
            Build = build;
            Runs = 1;
            LastRunDate = Util.ToTLDate();
            BuildName = program;
        }

        public const string RunTrackerFile = "_RunTracker.txt";
        public static string RunTrackerPath(string program)
        {
            return Util.ProgramData(program) + "\\" + RunTrackerFile;
        }
        /// <summary>
        /// count new instance of installed program and get most stable build
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static int CountNewGetStable(string program) { return CountNewGetStable(program, Util.ProgramBuild(program)); }
        /// <summary>
        /// count new instance of program and return number of build recognized as most used
        /// </summary>
        /// <param name="program"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static int CountNewGetStable(string program, int build)
        {
            bool ok = CountNewInstance(program, build);
            // get most used build 
            return GetMostUsedBuild(program);
        }

        /// <summary>
        /// count new run and get all runs as string
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static string CountNewGetPrettyRuns(string program) { return CountNewGetPrettyRuns(program, program); }
        public static string CountNewGetPrettyRuns(string program,string buildname)
        {
            CountNewInstance(program, Util.ProgramBuild(buildname));
            return GetPrettyRuns(program);
        }

        /// <summary>
        /// clears all run data for given program
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool ResetRunData(string program)
        {
            string fn = RunTrackerPath(program);
            if (System.IO.File.Exists(fn))
            {
                try
                {
                    System.IO.File.Delete(fn);
                    return true;
                }
                catch { return false; }
            }
            return true;
        }

        public static string GetPrettyRuns(string program) { return program+" "+string.Join(Environment.NewLine, GetPrettyRunData(program, ":", true, true)); }
        public static string GetPrettyRuns(string program, string delim) { return program+" "+string.Join(delim, GetPrettyRunData(program, ":", true, true)); }
        public static string[] GetPrettyRunData(string program) { return GetPrettyRunData(program, ":", true, true); }
        public static string[] GetPrettyRunData(string program, string delim, bool label, bool showlast)
        {
            List<RunTracker> rundata = GetRunCount(program);
            List<string> ps = new List<string>(rundata.Count);
            int current = Util.ProgramBuild(program);
            for (int i = 0; i < rundata.Count; i++)
            {
                string ctag = rundata[i].Build == current ? "*" : string.Empty;
                ps.Add((label ? "v" : string.Empty) + rundata[i].Build +ctag+ delim + rundata[i].Runs + (label ? "runs" : string.Empty) + (showlast ? " (" + rundata[i].LastRunDate + ")" : string.Empty));
            }
            return ps.ToArray();
        }
        /// <summary>
        /// count new instance of an installed program
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static bool CountNewInstance(string program) { return CountNewInstance(program, Util.ProgramBuild(program)); }
        public static bool CountNewInstance(string program, string buildname) { return CountNewInstance(program, Util.ProgramBuild(buildname)); }
        /// <summary>
        /// count new instance of program and returns true if count successful
        /// </summary>
        /// <param name="program"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static bool CountNewInstance(string program, int build) { return CountNewInstance(program, program, build); }
        public static bool CountNewInstance(string program, string buildname, int build)
        {
            if (build == 0)
                return false;
            string fn = RunTrackerPath(program);
            List<RunTracker> rundata = GetRunCount(buildname);
            // mark if we found build
            bool found = false;
            for (int i = 0; i < rundata.Count; i++)
            {
                RunTracker rt = rundata[i];
                if ((rt.Program == program) && (rt.Build == build))
                {
                    rt.Runs++;
                    rt.LastRunDate = Util.ToTLDate();
                    found = true;
                    rundata[i] = rt;
                }

            }
            // if not found, add it
            if (!found)
            {
                rundata.Add(new RunTracker(program, build));
            }
            // write data back
            bool ok = Util.ToFile<RunTracker[]>(rundata.ToArray(), fn);
            return ok;
        }

        /// <summary>
        /// identifier of a build that has never been run/counted
        /// </summary>
        public const int NEVERRUN = -1;
        /// <summary>
        /// get most run build for a given program
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static int GetMostUsedBuild(string program)
        {
            List<RunTracker> rundata = GetRunCount(program);
            int maxruns = 0;
            int maxbuild = NEVERRUN;
            for (int i = 0; i < rundata.Count; i++)
            {
                if (rundata[i].Runs > maxruns)
                {
                    maxruns = rundata[i].Runs;
                    maxbuild = rundata[i].Build;
                }
            }
            return maxbuild;
        }
        /// <summary>
        /// get run count from tracked data
        /// </summary>
        /// <param name="rundata"></param>
        /// <param name="program"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static int GetRunCount(List<RunTracker> rundata, string program, int build)
        {
            foreach (RunTracker rt in rundata)
                if ((rt.Program == program) && (rt.Build == build))
                    return rt.Runs;
            return 0;
        }
        /// <summary>
        /// get runtracker data for given program
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static List<RunTracker> GetRunCount(string program) { return GetRunCount(program, program); }
        public static List<RunTracker> GetRunCount(string program, string programbuild)
        {
            string fn = RunTrackerPath(programbuild);
            RunTracker[] data = new RunTracker[0];
            List<RunTracker> working = new List<RunTracker>();
            if (Util.FromFile<RunTracker[]>(fn, ref data))
            {
                working.AddRange(data);
            }
            return working;
        }


    }
}
