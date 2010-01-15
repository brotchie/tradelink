using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Common
{

    /// <summary>
    /// used to trackthroughput of a particular object over time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThroughputTracker<T>
    {
        public ThroughputTracker() { }
        DateTime _start;
        int _count;
        DateTime _stop;
        public int Count { get { return _count; } }
        public void Start() { _start = DateTime.Now; _count = 0; _stop = DateTime.MinValue; }
        public void Stop() { _stop = DateTime.Now; }
        /// <summary>
        /// returns throughput in items/second
        /// </summary>
        public double Throughput
        {
            get 
            {
                double elap = (_stop == DateTime.MinValue) ? 
                    DateTime.Now.Subtract(_start).TotalSeconds : 
                    _stop.Subtract(_start).TotalSeconds;
                if (elap == 0) return 0;
                return _count / elap;
            }
        }
        /// <summary>
        /// pump new items through to track their throughput
        /// </summary>
        /// <param name="item"></param>
        public void newItem(T item)
        {
            if (_stop != DateTime.MinValue) return;
            _count++;
        } 
    }
}
