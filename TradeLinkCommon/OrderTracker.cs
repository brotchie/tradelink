using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Collections;

namespace TradeLink.Common
{
    /// <summary>
    /// track status of orders
    /// </summary>
    public class OrderTracker  
    {
        /// <summary>
        /// get orders from tracker
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() { foreach (Order o in orders) yield return o; }
        /// <summary>
        /// create a tracker
        /// </summary>
        public OrderTracker()
        {
            orders.NewTxt += new TextIdxDelegate(orders_NewTxt);
        }

        void orders_NewTxt(string txt, int idx)
        {
            sent.addindex(txt, orders[idx].size);
            filled.addindex(txt, 0);
            canceled.addindex(txt, false);

            
        }


        GenericTracker<int> sent = new GenericTracker<int>();
        GenericTracker<int> filled = new GenericTracker<int>();
        GenericTracker<bool> canceled = new GenericTracker<bool>();
        GenericTracker<Order> orders = new GenericTracker<Order>();

        /// <summary>
        /// see if an order was pending
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isPending(long id) { return isPending(sent.getindex(id.ToString())); }
        bool isPending(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            if (canceled[idx])
                return false;
            return sent[idx] != filled[idx];
        }

        /// <summary>
        /// see if an order was completely filled
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isCompleted(long id) { return isCompleted(sent.getindex(id.ToString())); }
        bool isCompleted(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            return sent[idx] == filled[idx];
        }

        /// <summary>
        /// see if order was canceled
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool isCanceled(long id) { return isCanceled(sent.getindex(id.ToString())); }
        bool isCanceled(int idx)
        {
            if ((idx < 0) || (idx > sent.Count))
                return false;
            return canceled[idx];
        }

        /// <summary>
        /// returns sent size of an order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Sent(long id)
        {
            int idx = sent.getindex(id.ToString());
            if ((idx < 0) || (idx > sent.Count))
                return 0;
            return sent[idx];
        }
        /// <summary>
        /// returns filled size of an order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Filled(long id)
        {
            int idx = sent.getindex(id.ToString());
            if ((idx < 0) || (idx > sent.Count))
                return 0;
            return filled[idx];
        }



        /// <summary>
        /// get unfilled portion of order from index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int this[int idx]
        {
            get 
            { 
                if ((idx<0) || (idx>filled.Count)) return 0;
                if (canceled[idx])
                    return 0;
                return sent[idx]-filled[idx];
            }
        }

        /// <summary>
        /// get unfilled portion of order from order id
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int this[long id]
        {
            get
            {
                int idx = sent.getindex(id.ToString());
                return this[idx];
            }
        }

        /// <summary>
        /// returns true if order is known to tracker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isTracked(long id)
        {
            int idx = sent.getindex(id.ToString());
            return id != GenericTracker.UNKNOWN;
        }

        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        public virtual void GotOrder(Order o)
        {
            if (o.id == 0)
            {
                debug(o.symbol + " can't track order with blank id!: " + o.ToString());
                return;
            }
            int idx = sent.getindex(o.symbol);
            if (idx < 0)
            {
                orders.addindex(o.id.ToString(), o);
            }
        }

        public virtual void GotCancel(long id)
        {
            if (id==0) return;
            int idx = sent.getindex(id.ToString());
            if (idx<0)
            {
                debug("unknown cancelid: "+id);
                return;
            }
            canceled[idx] = true;
        }

        public virtual void GotFill(Trade f)
        {
            if (f.id == 0)
            {
                debug(f.symbol + " can't track order with blank id!: " + f.ToString());
                return;
            }
            int idx = sent.getindex(f.id.ToString());
            if (idx < 0)
            {
                debug("unknown fillid: " + f.id);
                return;
            }
            filled[idx] += f.xsize;
        }
    }
}
