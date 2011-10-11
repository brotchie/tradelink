using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class REGSHO_ShortTracker : OrderTracker
    {
        string _acct = string.Empty;

        public REGSHO_ShortTracker() : this(new PositionTracker()) { }
        public REGSHO_ShortTracker(PositionTracker PT) : base()
        {
            pt = PT;
            pt.DefaultAccount = DefaultAccount;

        }

        void REGSHO_ShortTracker_SendDebugEvent(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

        public string DefaultAccount { get { return _acct; } set { _acct = value; pt.DefaultAccount = _acct; debug("REGSHO Short, default account: " + _acct); } }

        PositionTracker pt = new PositionTracker();

        public PositionTracker Positions { get { return pt; } set { pt = value; } }

        public virtual void GotPosition(Position p)
        {
            pt.Adjust(p);
            v(p.Symbol+ " position: " + p);
        }



        public override void GotFill(Trade f)
        {
            pt.Adjust(f);
            base.GotFill(f);
            v(f.symbol + " position: " + pt[f.symbol, f.Account] + " after fill: " + f);
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
            int exitsize = ord.UnsignedSize;
            List<string> ids = new List<string>();
            foreach (Order o in this)
                if (!isPending(o.id))
                {
                    v("regsho notpend: " + o);
                    continue;
                }
                else if (o.side != p.isLong)
                {
                    ids.Add(o.id.ToString());
                    exitsize += o.UnsignedSize;
                    v("regsho pend: " + o);
                }
            // see if size will more than exit position
            if (exitsize > p.UnsignedSize)
            {
                debug(ord.symbol + " marking short. reason: pending sell size: " + exitsize + " > position: " + p + "after order: " + ord + " from orders: " + string.Join(",", ids.ToArray()));
                return true;
            }

            debug(ord.symbol + " exit size: " + exitsize + " marked sell. reason: not reversing position: " + p + "after order: " + ord + " from orders: " + string.Join(",", ids.ToArray()));

            return false;
        }

        bool _noverb = true;
        public bool VerboseDebugging
        {
            get { return !_noverb; }
            set
            {
                _noverb = !value;
                base.VerboseDebugging = value;
            }
        }

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }
    }
}
