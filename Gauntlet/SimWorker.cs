using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace WinGauntlet
{
    public class SimWorker : BackgroundWorker
    {
        public SimWorker()
        {
            DoWork += new DoWorkEventHandler(SimWorker_DoWork);
        }

        void SimWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SimWorkerArgs arg = (SimWorkerArgs)e.Argument;
            HistSim h = arg.h;
            Response r = arg.r;
            h.PlayTo(HistSim.ENDSIM);
            
        }
    }

    public class SimWorkerArgs
    {
        public HistSim h = null;
        public Response r = null;
    }

}
