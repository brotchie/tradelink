using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TradeLib;

namespace Replay
{
    public class Playback : BackgroundWorker
    {
        private HistSim h = null;

        public Playback(HistSim simulator) 
        { 
            h = simulator;
            h.Reset();
            h.Initialize();
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = true;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            if (e.Cancel)
                return;
            PlayBackArgs args = (PlayBackArgs)e.Argument;
            DateTime prevtime = DateTime.MinValue;
            DateTime playto = h.NextTickTime;
            while (h.NextTickTime != HistSim.ENDSIM)
            {
                if (CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // if we're past user specified DayStart, use user-specified delay otherwise no delay
                int delay = (h.NextTickTime.TimeOfDay >= args.DayStart.TimeOfDay) ? 
                    (int)h.NextTickTime.Subtract(prevtime).TotalMilliseconds * args.DELAYSCALE : 0;
                if (prevtime!=DateTime.MinValue) // if it's not first time doing this
                    System.Threading.Thread.CurrentThread.Join(delay); // wait realistic time
                prevtime = new DateTime(h.NextTickTime.Ticks); // save last time mark
                double progress = 100.0 * ((h.TickCount + h.IndexCount) / (double)h.ApproxTotalTicks);
                ReportProgress((int)progress); // report progress
                h.PlayTo(h.NextTickTime); // this will throw tick/idx events and get next time mark
            }
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
        public PlayBackArgs(int DelayScale, DateTime StartOfDay)
        {
            DELAYSCALE = DelayScale;
            DayStart = StartOfDay;
        }
        public int DELAYSCALE = 1;
        public DateTime DayStart;
    }
}
