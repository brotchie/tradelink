using System;
using System.Collections.Generic;
using TradeLink.API;


namespace TradeLink.AppKit
{
    public class RunHelper
    {
        System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
        public bool isBusy { get { return bw.IsBusy; } }
        public RunHelper() : this(null, null, null, string.Empty) { }
        public RunHelper(VoidDelegate start, VoidDelegate end) : this(start, end, null, string.Empty) { }
        public RunHelper(VoidDelegate start, VoidDelegate end, DebugDelegate deb) : this(start, end, deb, string.Empty) { }
        public RunHelper(VoidDelegate start, VoidDelegate end, DebugDelegate deb, string name)
        {
            OnStart = start;
            OnEnd = end;
            Name = name;
            OnDebug = deb;
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            bw.WorkerReportsProgress = false;
        }

        void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
#if DEBUG
#else
            try
            {
#endif
                RunHelper rh = (RunHelper)e.Argument;
                if (rh.OnStart != null)
                    rh.OnStart();
                else
                    rh.rundebug(rh.Name + " no OnStart delegate was provided, unable to run.");
                if (rh.OnEnd != null)
                    rh.OnEnd();
                else
                    rh.rundebug(rh.Name + " no OnEnd delegate provided, run complete.");
#if DEBUG
#else
            }
            catch (Exception ex)
            {
                rundebug("error running runhelper, err: " + ex.Message + ex.StackTrace);
            }
#endif

        }

        void rundebug(string msg)
        {
            if (OnDebug != null)
                OnDebug(msg);
        }

        public VoidDelegate OnStart = null;
        public VoidDelegate OnEnd = null;
        public DebugDelegate OnDebug = null;
        public long StartTimeTicks = 0;
        public bool isStarted = false;
        public string Name = string.Empty;

        static DebugDelegate debs = null;
        static void debug(string msg)
        {
            if (debs != null)
                debs(msg);
        }

        public static RunHelper run(VoidDelegate start, VoidDelegate complete, DebugDelegate deb, string name)
        {
            debs = deb;
            RunHelper rh = new RunHelper();
            if (rh.isBusy)
            {
                debug("must wait until previous job completed. " + name);
                return rh;
            }
            if (start == null)
                throw new Exception("cannot pass a null start delegate to run helper!");
            rh = new RunHelper(start, complete, deb, name);

            rh.bw.RunWorkerAsync(rh);


            return rh;
        }
    }
}
