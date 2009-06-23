using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// used to provide ultra-fast tick processing on machines with multiple cores.
    /// takes ticks immediately on main thread, processes them on a seperate thread.
    /// </summary>
    public class AsyncResponse
    {
        const uint MAXTICK = 10000;
        Tick[] _tickcache = new Tick[MAXTICK];
        uint _readticks = 0;
        uint _writeticks = 0;
        public event TickDelegate GotTick;
        public event VoidDelegate GotTickQueueEmpty;
        public event VoidDelegate GotTickQueued;
        volatile bool _tickflip = false;
        static ManualResetEvent _tickswaiting = new ManualResetEvent(false);
        Thread _readtickthread = null;



        void ReadTick()
        {

            while (true)
            {
                if (GotTickQueued != null) GotTickQueued();
                while (_readticks < _tickcache.Length)
                {
                    if (_readtickthread.ThreadState == ThreadState.StopRequested)
                        return;
                    if ((_readticks>=_writeticks) && !_tickflip)
                        break;
                    Tick k = _tickcache[_readticks];
                    if (GotTick != null)
                        GotTick(k);
                    _readticks++;
                    if (_readticks >= _tickcache.Length)
                    {
                        _readticks = 0;
                        _tickflip = false;
                    }
                }
                // send event that queue is presently empty
                if (GotTickQueueEmpty != null) GotTickQueueEmpty();
                // clear current flag signal
                _tickswaiting.Reset();
                // wait for a new signal to continue reading
                _tickswaiting.WaitOne(-1); 
                    
            }
        }
        
        public void newTick(Tick k)
        {
            _tickcache[_writeticks] = k;
            _writeticks++;
            if (_writeticks >= _tickcache.Length)
            {
                _writeticks = 0;
                _tickflip = true;
            }

            if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.Unstarted))
                _readtickthread.Start();
            else if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                _tickswaiting.Set(); // signal ReadIt thread to read now
            }
        }

        const uint MAXIMB = 100000;
        Imbalance[] _imbcache = new Imbalance[MAXIMB];
        uint _readimbs = 0;
        uint _writeimbs = 0;
        public event ImbalanceDelegate GotImbalance;
        public event VoidDelegate GotImbalanceQueued;
        public event VoidDelegate GotImbalanceQueueEmpty;
        volatile bool _imbflip = false;
        static ManualResetEvent _imbswaiting = new ManualResetEvent(false);
        Thread _readimbthread = null;

        void ReadImbs()
        {

            while (true)
            {
                if (GotImbalanceQueued != null) GotImbalanceQueued();
                while (_readimbs < _imbcache.Length)
                {
                    if (_readimbthread.ThreadState == ThreadState.StopRequested)
                        return;
                    if ((_readimbs >= _writeimbs) && !_imbflip)
                        break;
                    Imbalance imb  = _imbcache[_readimbs];
                    if (GotImbalance != null)
                        GotImbalance(imb);
                    _readimbs++;
                    if (_readimbs >= _imbcache.Length)
                    {
                        _readimbs = 0;
                        _imbflip = false;
                    }
                }
                // send event that queue is presently empty
                if (GotImbalanceQueueEmpty != null) GotImbalanceQueueEmpty();
                // clear current flag signal
                _imbswaiting.Reset();
                // wait for a new signal to continue reading
                _imbswaiting.WaitOne(-1);

            }
        }

        public void newImbalance(Imbalance imb)
        {
            _imbcache[_writeimbs] = imb;
            _writeimbs++;
            if (_writeimbs >= _imbcache.Length)
            {
                _writeimbs = 0;
                _imbflip = true;
            }

            if ((_readimbthread != null) && (_readimbthread.ThreadState == ThreadState.Unstarted))
                _readimbthread.Start();
            else if ((_readimbthread != null) && (_readimbthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                // signal ReadIt thread to read now
                _imbswaiting.Set(); 
            }
        }

        public AsyncResponse()
        {
            _readtickthread = new Thread(this.ReadTick);
            _readimbthread = new Thread(this.ReadImbs);
        }

        public void Stop()
        {
            if ((_readtickthread!=null) && ((_readtickthread.ThreadState != ThreadState.Stopped) && (_readtickthread.ThreadState != ThreadState.StopRequested)))
                _readtickthread.Abort();
            if ((_readimbthread != null) && ((_readimbthread.ThreadState != ThreadState.Stopped) && (_readimbthread.ThreadState != ThreadState.StopRequested)))
                _readimbthread.Abort();
            _tickcache = new Tick[MAXTICK];
            _imbcache = new Imbalance[MAXIMB];
            _readimbs = 0;
            _writeimbs = 0;
            _imbswaiting.Reset();
            _writeticks = 0;
            _readticks = 0;
            _tickswaiting.Reset();
        }
    }
}
