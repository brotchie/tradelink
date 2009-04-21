using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TradeLink.Common;

namespace Replay
{
    public class Playback : BackgroundWorker
    {
        private HistSim h = null;

        public Playback(HistSim simulator) 
        { 
            h = simulator;
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (e.Cancel)
                return;
            PlayBackArgs args = (PlayBackArgs)e.Argument;
            long prevtime = 0;
            do
            {
                // if cancel was requested, quit
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                // play first tick
                long next = h.NextTickTime;
                h.PlayTo(next); 
                // adjust delay, based on user's setting of 'speed'
                // as well as daystart (no delay before daystart)
                int delay = next >= args.DayStart ?
                    Util.FTDIFF((int)prevtime, (int)next) * 1000 * args.DELAYSCALE : 0;
                // if not first tick, wait realistic time between ticks
                if (prevtime != 0) 
                    System.Threading.Thread.CurrentThread.Join(delay); 
                // save time for calculating next day
                prevtime = next; 
                // calculate progress
                double progress = 100.0 * (h.TicksProcessed / (double)h.TicksPresent);
                // report progress
                ReportProgress((int)progress); 
                
            }
            while (h.NextTickTime != HistSim.ENDSIM);
            base.OnDoWork(e);
        }

        protected override bool CanRaiseEvents
        {
            get
            {
                return true;
            }
        }


        
    }

    public class PlayBackArgs
    {
        public PlayBackArgs(int DelayScale, int StartOfDay)
        {
            DELAYSCALE = DelayScale;
            DayStart = StartOfDay;
        }
        public int DELAYSCALE = 1;
        public int DayStart;
    }
}
