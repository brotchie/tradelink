using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Collections;

namespace TradeLink.Common
{
    /// <summary>
    /// track status of orders
    /// </summary>
    public class OrderTracker  : GotOrderIndicator,GotCancelIndicator,GotFillIndicator,GenericTrackerI
    {
        public void Clear()
        {
            orders.Clear();
            sent.Clear();
            filled.Clear();
            canceled.Clear();
        }
        public int addindex(string txt, int ignore)
        {
            return orders.addindex(txt);
        }
        public string Display(string txt) { return string.Empty; }
        public string Display(int idx) { return this[idx].ToString(); }
        public string getlabel(int idx) { return orders.getlabel(idx); }
        public int Count { get { return orders.Count; } }
        public decimal ValueDecimal(string txt) { int idx = getindex(txt); if (idx < 0) return 0; return this[idx]; }
        public decimal ValueDecimal(int idx) { return this[idx]; }
        public object Value(string txt) { return ValueDecimal(txt); }
        public object Value(int idx) { return ValueDecimal(idx); }
        string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; } }
        public int addindex(string txt)
        {
            return orders.addindex(txt);
        }
        public int getindex(string txt) { return orders.getindex(txt); }
        public Type TrackedType { get { return typeof(int); } }
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
        public event TextIdxDelegate NewTxt;
        void orders_NewTxt(string txt, int idx)
        {
            int sentsize = (orders[idx].side? 1 : -1) * Math.Abs(orders[idx].size);
            v(txt + " sentsize: " + sentsize + " after: " + orders[idx].ToString());
            unknownsent.addindex(txt, false);
            sent.addindex(txt, sentsize);
            filled.addindex(txt, 0);
            canceled.addindex(txt, false);
            if (NewTxt != null)
                NewTxt(txt, idx);

            
        }



        GenericTracker<bool> unknownsent = new GenericTracker<bool>();
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
            return Math.Abs(sent[idx]) != Math.Abs(filled[idx]);
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
        /// gets entire sent order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order SentOrder(long id)
        {
            if (orders.getindex(id.ToString()) != GenericTracker.UNKNOWN)
                return orders[id.ToString()];
            else
                return new OrderImpl();
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
                return Math.Abs(sent[idx])-Math.Abs(filled[idx]);
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
            return idx != GenericTracker.UNKNOWN;
        }

        public event DebugDelegate SendDebugEvent;
        protected void debug(string msg)
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
            int idx = sent.getindex(o.id.ToString());
            if (idx < 0)
            {
                idx = orders.addindex(o.id.ToString(), o);
            }
            v(o.symbol + " order ack: " + o);
        }

        public virtual void GotCancel(long id)
        {
            if (id==0) return;
            int idx = sent.getindex(id.ToString());
            if (idx<0)
            {
                debug("no existing order found with cancel id: "+id);
                idx = orders.addindex(id.ToString(), new OrderImpl());
                unknownsent[idx] = true;
                canceled[idx] = true;
                return;
            }
            canceled[idx] = true;
            if (_noverb) ;
            else
            {
                string symbol = "?";
                foreach (Order o in this)
                    if (o.id == id)
                        symbol = o.symbol;
                v(symbol + " canceled id: " + id);
            }
        }

        bool _fixsentonunknown = false;
        public bool FixSentSizeOnUnknown { get { return _fixsentonunknown; } set { _fixsentonunknown = value; } }

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
                debug("no existing order found with fillid: " + f.id);
                idx = orders.addindex(f.id.ToString(), new OrderImpl());
                unknownsent[idx] = true;
            }
            filled[idx] += (f.side ? 1 : -1) *Math.Abs(f.xsize);
            if (FixSentSizeOnUnknown && unknownsent[idx])
                sent[idx] = filled[idx];
            v(f.symbol + " filled size: " + filled[idx] + " after: " + f.ToString());
        }

        bool _noverb = true;
        public bool VerboseDebugging
        {
            get { return !_noverb; }
            set
            {
                _noverb = !value;
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
