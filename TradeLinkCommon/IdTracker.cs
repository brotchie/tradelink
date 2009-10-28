using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// used to obtain valid ids for orders, responses, etc
    /// </summary>
    public class IdTracker
    {
        const uint DEFAULTOWNER = 0;
        const uint MAXOWNER = 512;
        // calculate mask length
        // 512 is 2^9.   32bits - 9bits for top = 23bits
        const int MASKLEN = 23;

        uint _first = 0;
        uint _nextid = 0;
        uint _owner = 0;
        uint _maxid = uint.MaxValue;
        public uint Count = 0;
        /// <summary>
        /// creates an object to assign unique order ids
        /// </summary>
        public IdTracker() : this(false,DEFAULTOWNER, OrderImpl.Unique) { }
        /// <summary>
        /// creates an object to assign unique ids
        /// </summary>
        /// <param name="OwnerId"></param>
        public IdTracker(uint OwnerId) : this(true,OwnerId, OrderImpl.Unique) { }

        /// <summary>
        /// creates an object to assign unique order ids to one or more owners.
        /// </summary>
        /// <param name="OwnerId">A unique number identifying this owner</param>
        /// <param name="initialId">Owners first order id</param>
        public IdTracker(bool virtualids,uint OwnerId, uint initialId)
        {
            _owner = OwnerId;
            if (virtualids)
            {
                // make sure valid
                if (_owner > MAXOWNER)
                    throw new Exception("You can't assign more than " + MAXOWNER + " owners.");
                unchecked
                {
                    // create a mask to strip off lower portion of id
                    const uint lowermask = (MAXOWNER - 1) << MASKLEN;
                    // get inverse to mask out top part
                    const uint topmask = ~lowermask;
                    // top mask is also the count
                    Count = (topmask + 1);
                    // get seed as lower part
                    uint seed = initialId & topmask;
                    // get high bits of first id
                    uint highbits = _owner << MASKLEN;
                    // calculate first id
                    _first = highbits + seed;
                    // calculate max value
                    _maxid = highbits + topmask;
                }
            }
            else
            {
                _first = initialId;
            }
            // assign next id
            _nextid = _first;
        }
        /// <summary>
        /// obtains a new id permanently
        /// </summary>
        public uint AssignId { get { if (NextOverflows) throw new IdTrackerOverflow(); return _nextid++; } }
        /// <summary>
        /// provides what next id will be without assigning it
        /// </summary>
        public uint NextId { get { return _nextid; } }
        /// <summary>
        /// return true if next id will overflow
        /// </summary>
        public bool NextOverflows { get { return _nextid >= _maxid; } }
    }

    public class IdTrackerOverflow : Exception
    {
        public IdTrackerOverflow() : base("The maximum # of assigned order ids was exceeded.") { }
        public IdTrackerOverflow(string msg) : base(msg) { }
    }
}

