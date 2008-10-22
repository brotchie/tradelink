using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TradeLib;

namespace WinGauntlet
{
    public class SimManager
    {
        // basic idea is that the manager should take a list of relevant files
        // from user, and break up filelist so that the workload is distributed 
        // amongst all the cores on the user's system

        // also needs to take into account the data the user needs
        // this is spreadmode, because if the user needs every stock available
        // for a given day /month...  histsim needs to load this data (potentially more than once)
        // and replay it in the same thread, along with the other data feeds.


        List<FileInfo> tf = null;
        SpreadMode mode = SpreadMode.None;
        

        public SimManager(List<FileInfo> candidateio, SpreadMode Mode, Response r)
        {
            tf = candidateio;
            mode = Mode;
            
        }

        public SimWorkerArgs NextBatch()
        {
            return null;
        }
        
    }

    public enum SpreadMode
    {
        None,
        All,
        Specific,
    }
}
