using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace WinGauntlet
{
    public class SimManager
    {

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
