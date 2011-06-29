using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class REGSHO_ShortTracker : OrderTracker
    {
        string _acct = string.Empty;

        public string DefaultAccount { get { return _acct; } set { _acct = value; pt.DefaultAccount = _acct; } }

        PositionTracker pt = new PositionTracker();

        public override void GotFill(Trade f)
        {
            pt.Adjust(f);
            base.GotFill(f);
        }
        /// <summary>
        /// returns true if order is a true short
        /// (versus a sell/buy)
        /// </summary>
        /// <param name="ord"></param>
        /// <returns></returns>
        public bool isOrderShort(Order ord)
        {
            // order is a buy
            if (ord.side)
                return false;
            // get symbol position
            Position p = pt[ord.symbol];
            // if we're flat return side
            if (p.isFlat)
                return !ord.side;
            // if same side return side
            if (p.isLong==ord.side)
                return ord.side;
            // get all pending orders for symbol
            int exitsize = 0;
            foreach (Order o in this)
                if (!isPending(o.id))
                    continue;
                else if (o.side != p.isLong)
                    exitsize += o.UnsignedSize;
            // see if size will completely exit position
            if (exitsize >= p.UnsignedSize)
                return true;

            return false;
        }
    }
}
