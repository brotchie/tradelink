using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class REGSHO_ShortTracker : OrderTracker
    {
        string _acct = string.Empty;

        public REGSHO_ShortTracker() : base()
        {
            pt.DefaultAccount = DefaultAccount;
        }

        public string DefaultAccount { get { return _acct; } set { _acct = value; pt.DefaultAccount = _acct; debug("REGSHO Short, default account: " + _acct); } }

        PositionTracker pt = new PositionTracker();

        public virtual void GotPosition(Position p)
        {
            pt.Adjust(p);
        }

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
            {
                debug(ord.symbol + " buy order can't be short: " + ord);
                return false;
            }
            // get symbol position
            Position p = pt[ord.symbol,ord.Account];
            // if we're flat return side
            if (p.isFlat)
            {
                
                debug(ord.symbol + (ord.side ? " flat+buy order can't be short: ":" flat+sell is a short: ") + ord+" pos: "+p);
                return !ord.side;
            }
            // if same side return side
            if (p.isLong && ord.side)
            {
                debug(ord.symbol + " flat+buy order can't be short: " + ord + " pos: " + p);
                return false;
            }
            else if (p.isShort && !ord.side)
            {
                debug(ord.symbol + " short+sell always short: " + ord + " pos: " + p);
                return true;
            }
            else if (p.isShort && ord.side)
            {
                debug(ord.symbol + " short+buy order can't be short: " + ord + " pos: " + p);
                return false;
            }
            // get all pending orders for symbol
            int exitsize = 0;
            List<string> ids = new List<string>();
            foreach (Order o in this)
                if (!isPending(o.id))
                    continue;
                else if (o.side != p.isLong)
                {
                    ids.Add(o.id.ToString());
                    exitsize += o.UnsignedSize;
                }
            // see if size will completely exit position
            if (exitsize >= p.UnsignedSize)
            {
                debug(ord.symbol + " marking short as pending sell size: " + exitsize + " for long: " + p + " > pos: "+p +" from orders: " + string.Join(",", ids.ToArray()));
                return true;
            }

            return false;
        }
    }
}
