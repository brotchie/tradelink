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
        T[] _buffer;
        volatile int _rc;
        volatile int _wc;
        volatile bool _flipped;
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
            _buffer[_wc++] = val;
            if (_wc >= _buffer.Length)
            {
                _wc = 0;
                _flipped = true;
            }
        }
        /// <summary>
        /// returns true if all written elements have been read from buffer
        /// </summary>
        public bool isEmpty { get { return (_rc >= _wc) && !_flipped; } }
        /// <summary>
        /// returns false if there is more to be read
        /// </summary>
        public bool hasItems { get { return (_rc < _wc) || _flipped; } }
        /// <summary>
        /// reads next unread element from buffer
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            T val = _buffer[_rc++];
            if (_rc >= _buffer.Length)
            {
                _rc = 0;
                _flipped = false;
            }
            return val;
        }
        /// <summary>
        /// create a buffer of fixed size in elements
        /// </summary>
        /// <param name="BufferSize"></param>
        public RingBuffer(int BufferSize)
        {
            _buffer = new T[BufferSize];
            _wc = 0;
            _rc = 0;
            _flipped = false;
        }
    }
}
