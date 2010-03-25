using System.Collections.Generic;

namespace TradeLink.Common
{
    /// <summary>
    /// Circular buffer.   Allows reading and writing of up to BufferSize 
    /// element count in a FIFO fashion.   Useful for passing data asynchronously 
    /// between threads without requiring locking.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct RingBuffer<T>
    {
        int _bo;
        T[] _buffer;
        int _rc;
        int _wc;
        int _rflip;
        int _wflip;
        /// <summary>
        /// called when buffer overrun occurs
        /// </summary>
        public event TradeLink.API.VoidDelegate BufferOverrunEvent;
        /// <summary>
        /// number of overruns, should be zero (otherwise increase buffer size)
        /// </summary>
        public int BufferOverrun { get { return _bo; } }
        /// <summary>
        /// maximum # of unread elements the buffer can hold.
        /// </summary>
        public int BufferSize { get { return _buffer.Length; } }
        /// <summary>
        /// Count of unread elements
        /// </summary>
        public int Count { get { return System.Math.Abs(_wc - _rc); } }
        /// <summary>
        /// writes a value into the buffer
        /// </summary>
        /// <param name="val"></param>
        public void Write(T val)
        {
            System.Threading.Interlocked.Increment(ref _wc);
            if (_wc >= _buffer.Length)
            {
                System.Threading.Interlocked.Exchange(ref _wc, 0);
                System.Threading.Interlocked.Increment(ref _wflip);
                if ((_wflip - _rflip > 1))
                {
                    _bo++;
                    if (BufferOverrunEvent != null)
                        BufferOverrunEvent();
                }
            }
            _buffer[_wc] = val;
        }
        /// <summary>
        /// returns true if all written elements have been read from buffer
        /// </summary>
        public bool isEmpty { get { return (_rc >= _wc) && (_wflip== _rflip); } }
        /// <summary>
        /// returns false if there is more to be read
        /// </summary>
        public bool hasItems { get { return (_rc < _wc) || (_wflip!=_rflip); } }
        /// <summary>
        /// reads next unread element from buffer
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            System.Threading.Interlocked.Increment(ref _rc);
            if (_rc >= _buffer.Length)
            {
                System.Threading.Interlocked.Exchange(ref _rc, 0);
                System.Threading.Interlocked.Increment(ref _rflip);
            }
            T val = _buffer[_rc];

            return val;
        }
        
        /// <summary>
        /// create a buffer of fixed size in elements
        /// </summary>
        /// <param name="BufferSize"></param>
        public RingBuffer(int BufferSize)
        {
            BufferOverrunEvent = null;
            _bo = 0;
            _buffer = new T[BufferSize];
            _wc = 0;
            _rc = 0;
            _rflip = 0;
            _wflip = 0;
        }
    }
}
