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
        uint _nextid = 0;
        public IdTracker() : this(OrderImpl.Unique) { }
        public IdTracker(uint initialId)
        {
            _nextid = initialId;
        }
        /// <summary>
        /// obtains a new id permanently
        /// </summary>
        public uint AssignId { get { return _nextid++; } }
        /// <summary>
        /// provides what next id will be without assigning it
        /// </summary>
        public uint NextId { get { return _nextid; } }
    }
}
