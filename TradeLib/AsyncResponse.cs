using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace TradeLib
{
    public class AsyncResponse
    {
        const uint MAXTICK = 10000;
        Tick[] tickcache = new Tick[MAXTICK];
        uint readcounter = 0;
        uint writecounter = 0;
        public event TickDelegate GotTick;

        void ReadIt()
        {

            while (true)
            {
                while (readcounter < tickcache.Length)
                {
                    if (readthread.ThreadState == ThreadState.StopRequested)
                        return;
                    if (readcounter>=writecounter)
                        break;
                    Tick k = tickcache[readcounter];
                    if (GotTick != null)
                        GotTick(k);
                    readcounter++;
                    if (readcounter >= tickcache.Length)
                        readcounter = 0;
                }
                mre.WaitOne(); // wait for a signal to continue reading
                    
            }
        }
        static ManualResetEvent mre = new ManualResetEvent(false);
        public void WriteIt(Tick k)
        {
            tickcache[writecounter] = k;
            writecounter++;
            if (writecounter >= tickcache.Length) 
                writecounter = 0;

            if ((readthread != null) && (readthread.ThreadState == ThreadState.Unstarted))
                readthread.Start();
            else if ((readthread != null) && (readthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                mre.Set(); // signal ReadIt thread to read now
            }
        }
        Thread readthread = null;
        public AsyncResponse()
        {
            readthread = new Thread(this.ReadIt);
        }

        public void Stop()
        {
            if ((readthread!=null) && ((readthread.ThreadState != ThreadState.Stopped) && (readthread.ThreadState != ThreadState.StopRequested)))
                readthread.Abort();
            tickcache = new Tick[MAXTICK];
            writecounter = 0;
            readcounter = 0;
            mre.Reset();
        }
    }
}
