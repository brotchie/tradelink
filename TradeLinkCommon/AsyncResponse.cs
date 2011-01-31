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
        const int MAXTICK = 10000;
        const int MAXIMB = 100000;
        RingBuffer<Tick> _tickcache;
        RingBuffer<Imbalance> _imbcache;

        /// <summary>
        /// fired when tick is read asychronously from buffer
        /// </summary>
        public event TickDelegate GotTick;
        /// <summary>
        ///  fired when buffer is empty
        /// </summary>
        public event VoidDelegate GotTickQueueEmpty;
        /// <summary>
        /// fired when buffer is written
        /// </summary>
        public event VoidDelegate GotTickQueued;
        /// <summary>
        /// should be zero unless buffer too small
        /// </summary>
        public int TickOverrun { get { return _tickcache.BufferOverrun; } }

        static ManualResetEvent _tickswaiting = new ManualResetEvent(false);
        Thread _readtickthread = null;

        volatile bool _readtick = true;
        int _nrt = 0;
        int _nwt = 0;

        public bool isValid { get { return _readtick; } }

        void ReadTick()
        {
            try
            {
                while (_readtick)
                {

                    if (_tickcache.hasItems && (GotTickQueued != null))
                        GotTickQueued();
                    while (_tickcache.hasItems)
                    {
                        if (!_readtick)
                            break;
                        Tick k = _tickcache.Read();
                        if (k == null)
                        {
                            _nrt++;
                            if (GotBadTick != null)
                                GotBadTick();
                            continue;
                        }
                        if (GotTick != null)
                            GotTick(k);
                    }
                    // send event that queue is presently empty
                    if (_tickcache.isEmpty && (GotTickQueueEmpty != null))
                        GotTickQueueEmpty();
                    // clear current flag signal
                    _tickswaiting.Reset();
                    // wait for a new signal to continue reading
                    _tickswaiting.WaitOne(SLEEP);

                }
            }
            catch (MissingMethodException ex)
            {
                
                System.Diagnostics.Process.Start(@"http://code.google.com/p/tradelink/wiki/MissingMethodException");
                Stop();

            }
            catch (MissingMemberException ex)
            {
                System.Diagnostics.Process.Start(@"http://code.google.com/p/tradelink/wiki/MissingMethodException");
                Stop();
            }
            catch (ThreadInterruptedException) { }
        }

        public const int SLEEPDEFAULTMS = 10;
        int _sleep = SLEEPDEFAULTMS;
        /// <summary>
        /// sleep time in milliseconds between checking read buffer
        /// </summary>
        public int SLEEP { get { return _sleep; } set { _sleep = value; } }

        /// <summary>
        /// pass new ticks through here to be processed asyncrhomously
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            if (k == null)
            {
                _nwt++;
                if (GotBadTick != null)
                    GotBadTick();
                return;
            }
            _tickcache.Write(k);
            if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.Unstarted))
            {
                _readtick = true;
                _readtickthread.Start();
            }
            else if ((_readtickthread != null) && (_readtickthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                _tickswaiting.Set(); // signal ReadIt thread to read now
            }
        }

        /// <summary>
        /// called if bad tick is written or read.
        /// check bad counters to see if written or read.
        /// </summary>
        public event VoidDelegate GotBadTick;
        /// <summary>
        /// called if bad imbalance is written or read.
        /// check bad counters to see if written or read.
        /// </summary>
        public event VoidDelegate GotBadImbalance;
        /// <summary>
        /// called if buffer set is too small
        /// </summary>
        public event VoidDelegate GotTickOverrun;
        /// <summary>
        /// called if buffer set is too small
        /// </summary>
        public event VoidDelegate GotImbalanceOverrun;
        /// <summary>
        /// # of null ticks ignored at write
        /// </summary>
        public int BadTickWritten { get { return _nwt; } }
        /// <summary>
        /// # of null ticks ignored at read
        /// </summary>
        public int BadTickRead { get { return _nrt; } }

        /// <summary>
        /// # of imbalances ignored at write
        /// </summary>
        public int BadImbalanceWritten { get { return _niw; } }
        /// <summary>
        /// # of imbalances ignored at read
        /// </summary>
        public int BadImbalanceRead { get { return _nir; } }



        /// <summary>
        /// fired when imbalance is read asynchronously from buffer
        /// </summary>
        public event ImbalanceDelegate GotImbalance;
        /// <summary>
        /// fired when buffer is written to
        /// </summary>
        public event VoidDelegate GotImbalanceQueued;
        /// <summary>
        /// fired when buffer is empty
        /// </summary>
        public event VoidDelegate GotImbalanceQueueEmpty;
        /// <summary>
        /// should be zero unless buffer too small
        /// </summary>
        public int ImbalanceOverrun { get { return _imbcache.BufferOverrun; } }

        static ManualResetEvent _imbswaiting = new ManualResetEvent(false);
        Thread _readimbthread = null;
        bool _readimb = true;
        int _nir = 0;
        int _niw = 0;

        void ReadImbs()
        {
            try
            {
                while (_readimb)
                {
                    if (_imbcache.hasItems &&(GotImbalanceQueued != null))
                        GotImbalanceQueued();
                    while (_imbcache.hasItems)
                    {
                        if (!_readimb)
                            break;
                        Imbalance imb  = _imbcache.Read();
                        if (imb == null)
                        {
                            _nir++;
                            if (GotBadImbalance != null)
                                GotBadImbalance();
                            continue;
                        }
                        if (GotImbalance != null)
                            GotImbalance(imb);
                    }
                    // send event that queue is presently empty
                    if (_imbcache.isEmpty && (GotImbalanceQueueEmpty != null) )
                        GotImbalanceQueueEmpty();

                    // clear current flag signal
                    _imbswaiting.Reset();
                    // wait for a new signal to continue reading
                    _imbswaiting.WaitOne(SLEEP);


                }
            }
            catch (ThreadInterruptedException) { }
        }
        /// <summary>
        /// write an imbalance to buffer for later processing
        /// </summary>
        /// <param name="imb"></param>
        public void newImbalance(Imbalance imb)
        {
            if (imb == null)
            {
                _niw++;
                if (GotBadImbalance != null)
                    GotBadImbalance();
                return;
            }
            _imbcache.Write(imb);

            if ((_readimbthread != null) && (_readimbthread.ThreadState == ThreadState.Unstarted))
            {
                _readimb = true;
                _readimbthread.Start();
            }
            else if ((_readimbthread != null) && (_readimbthread.ThreadState == ThreadState.WaitSleepJoin))
            {
                // signal ReadIt thread to read now
                _imbswaiting.Set();
            }
        }
        /// <summary>
        /// create an asynchronous responder
        /// </summary>
        public AsyncResponse() : this(MAXTICK, MAXIMB) { }
        /// <summary>
        /// creates asynchronous responder with specified buffer sizes
        /// </summary>
        /// <param name="maxticks"></param>
        /// <param name="maximb"></param>
        public AsyncResponse(int maxticks, int maximb)
        {
            _tickcache = new RingBuffer<Tick>(maxticks);
            _tickcache.BufferOverrunEvent += new VoidDelegate(_tickcache_BufferOverrunEvent);
            _imbcache = new RingBuffer<Imbalance>(maximb);
            _imbcache.BufferOverrunEvent += new VoidDelegate(_imbcache_BufferOverrunEvent);
            _readtickthread = new Thread(this.ReadTick);
            _readimbthread = new Thread(this.ReadImbs);
        }

        void _imbcache_BufferOverrunEvent()
        {
            if (GotImbalanceOverrun != null)
                GotImbalanceOverrun();
        }

        void _tickcache_BufferOverrunEvent()
        {
            if (GotTickOverrun != null)
                GotTickOverrun();
        }
        /// <summary>
        /// stop the read threads and shutdown (call on exit)
        /// </summary>
        public void Stop()
        {
            _readimb = false;
            _readtick = false;
            try
            {
                if ((_readtickthread != null) && ((_readtickthread.ThreadState != ThreadState.Stopped) && (_readtickthread.ThreadState != ThreadState.StopRequested)))
                    _readtickthread.Interrupt();
                if ((_readimbthread != null) && ((_readimbthread.ThreadState != ThreadState.Stopped) && (_readimbthread.ThreadState != ThreadState.StopRequested)))
                    _readimbthread.Interrupt();
            }
            catch { }
            try
            {
                _tickcache = new RingBuffer<Tick>(MAXTICK);
                _imbcache = new RingBuffer<Imbalance>(MAXIMB);
                _imbswaiting.Reset();
                _tickswaiting.Reset();
            }
            catch { }
        }

    }
}
