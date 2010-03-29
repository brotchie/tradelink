using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TradeLink.Common;

namespace Replay
{
    public class Playback : BackgroundWorker
    {
        private HistSimImpl h = null;

        public Playback(HistSimImpl simulator) 
        { 
            h = simulator;
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
        }
        int lastprogress = 0;
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
                int delay = Util.FTDIFF((int)prevtime, (int)next) * 1000 * args.DELAYSCALE;
                // if not first tick, wait realistic time between ticks
                if (prevtime *delay !=0) 
                    System.Threading.Thread.CurrentThread.Join(delay); 
                // save time for calculating next day
                prevtime = next; 
                // calculate progress
                double progress = 100.0 * (h.TicksProcessed / (double)h.TicksPresent);
                int thisprogress = (int)progress;
                // report progress
                if (thisprogress > lastprogress)
                {
                    ReportProgress(thisprogress);
                    lastprogress = thisprogress;
                }
                
            }
            while (h.NextTickTime != HistSimImpl.ENDSIM);
            // reset last progress
            lastprogress = 0;
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
        public PlayBackArgs(int DelayScale)
        {
            DELAYSCALE = DelayScale;
        }
        public int DELAYSCALE = 1;
    }
}
