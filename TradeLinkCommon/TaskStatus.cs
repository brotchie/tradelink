using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class TaskStatus
    {
        public string Task = string.Empty;
        public int Attempts = 0;
        public int Successes = 0;
        public int Failures { get { return Attempts - Successes; } }
        public int NetSuccess { get { return Successes - Failures; } }
        public bool isValid { get { return !string.IsNullOrWhiteSpace(Task); } }

        public static TaskStatus GetTask(string program, string sym) { return GetTask(program + sym); }
        /// <summary>
        /// gets a task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static TaskStatus GetTask(string task)
        {
            TaskStatus ts = new TaskStatus();
            string fn = GetDbPath(task);
            if (Util.FromFile<TaskStatus>(fn, ref ts))
            {
                return ts;
            }
            return ts;
        }

        public static void Reset(string program, string sym) { Reset(program + sym); }
        /// <summary>
        /// reset count for a task
        /// </summary>
        /// <param name="task"></param>
        public static void Reset(string task)
        {
            try
            {
                System.IO.File.Delete(GetDbPath(task));
            }
            catch { }
        }

        public static bool HasNetSuccess(string task) { return HasNetSuccess(task, 3); }

        public static bool hasMaxFails(string task) { return hasMaxFails(task, 2, null); }
        public static bool hasMaxFails(string program, string sym, DebugDelegate deb) { return hasMaxFails(program + sym, 2, deb); }
        public static bool hasMaxFails(string task, DebugDelegate deb) { return hasMaxFails(task, 2, deb); }
        public static bool hasMaxFails(string task, int maxfail, DebugDelegate deb)
        {
            var t = GetTask(task);
            bool max = t.isValid && (t.Successes == 0) && (t.Failures > maxfail);
            if (max && (deb != null))
                deb(task + " hit maximum failures " + t.Failures + " with no success.");

            return max;
        }

        /// <summary>
        /// see whether task has some success
        /// </summary>
        /// <param name="task"></param>
        /// <param name="minnetsuccess"></param>
        /// <returns></returns>
        public static bool HasNetSuccess(string task, int minnetsuccess)
        {
            TaskStatus ts = new TaskStatus();
            string fn = GetDbPath(task);
            if (Util.FromFile<TaskStatus>(fn, ref ts))
            {
                return ts.NetSuccess >= minnetsuccess;
            }
            return false;
        }

        public static void CountSuccess(string program, string sym) { CountSuccess(program + sym); }

        /// <summary>
        /// count successful task
        /// </summary>
        /// <param name="task"></param>
        public static void CountSuccess(string task)
        {
            TaskStatus ts = new TaskStatus();
            string fn = GetDbPath(task);
            if (Util.FromFile<TaskStatus>(fn, ref ts))
            {

            }
            ts.Task = task;
            ts.Attempts++;
            ts.Successes++;
            Util.ToFile<TaskStatus>(ts, fn);
        }

        public const string PROGRAM = "TaskDb";
        public static string TaskDbPath = Util.ProgramData(PROGRAM);
        public static string GetDbPath(string task)
        {
            return TaskDbPath + task;
        }

        public static void CountFail(string program, string sym) { CountFail(program + sym); }
        /// <summary>
        /// count an unsuccessful attempt
        /// </summary>
        /// <param name="task"></param>
        /// <param name="path"></param>
        public static void CountFail(string task)
        {
            TaskStatus ts = new TaskStatus();
            string fn = GetDbPath(task);
            if (Util.FromFile<TaskStatus>(fn, ref ts))
            {

            }
            ts.Task = task;
            ts.Attempts++;
            Util.ToFile<TaskStatus>(ts, fn);


        }
    }
}
