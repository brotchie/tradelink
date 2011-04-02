
using TradeLink.API;


namespace TradeLink.Common
{
    public class RejectTracker : GenericTracker<bool>
    {
        IdTracker _idt;
        public RejectTracker() : this(new IdTracker()) { }
        public RejectTracker(IdTracker idt)
        {
            _idt = idt;
            NewTxt += new TextIdxDelegate(RejectTracker_NewTxt);
        }
        GenericTracker<bool> rejected = new GenericTracker<bool>();
        void RejectTracker_NewTxt(string txt, int idx)
        {
            ids.addindex(txt);
            times.addindex(txt);
            rejected.addindex(txt);
        }

        long gettime { get { return System.DateTime.Now.Ticks; } }
        
        public event LongDelegate SendRejectEvent;
        int _rejectwaitms = 500;
        public int RejectAfterMs { get { return _rejectwaitms; } set { _rejectwaitms = value; } }
        public void sendorder(Order o)
        {
            // assign an id if we don't have one already
            if (o.id == 0)
            {
                o.id = _idt.AssignId;
                // notify of new id
                debug(o.ToString() + " did not have id, assigned id: " + o.id);
            }
            // get index
            int idx = addindex(o.id.ToString(), false);
            // ensure we are tracking id
            ids[idx] = o.id;
            // get time
            long ctime = gettime;
            // record time order sent
            times[idx] = ctime;
            // notify
            debug(o.id + " tracking sent time: " + ctime);
            // pass order through
            if (SendOrderEvent != null)
                SendOrderEvent(o);
            // check rejects
            checkrejects(ctime);
        }

        public event DebugDelegate SendDebugEvent;
        public event LongDelegate SendCancelEvent;

        

        void cancel(long id)
        {
            if (SendCancelEvent != null)
            {
                SendCancelEvent(id);
                debug(id + " sent cancel.");
            }
            else
                debug("send cancel events not defined on reject tracker.");
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        /// <summary>
        /// if cancels are passed through, consider them acks
        /// </summary>
        /// <param name="id"></param>
        public void GotCancel(long id)
        {
            // check other rejections while we're here
            checkrejects(gettime);
            // see if we have this order
            int idx = getindex(id.ToString());
            // ignore unknown
            if (idx < 0)
                return;
            // notify
            debug(id + " got cancel ack. (not rejected)");
            // otherwise ack cancel
            this[idx] = true;
        }

        public void checkrejects() { checkrejects(gettime); }
        public void checkrejects(long ctime)
        {
            // compute allowed delay in nano-seconds that defines a reject
            long delay = RejectAfterMs*10000;
            // check every id
            for (int idx = 0; idx<this.Count; idx++)
            {
                // ignore ids which have already been acked 
                if (this[idx])
                    continue;
                // ignore reject
                if (rejected[idx])
                    continue;
                // otherwise check for delay
                if ((ctime - times[idx]) > delay)
                {
                    // get id
                    long rid = ids[idx];
                    // notify
                    debug(rid + " rejecting at " + ctime + " (" + times[idx] + ").");
                    // reject
                    reject(ids[idx]);
                }
            }
            
        }

        bool _safetycancel = true;
        public bool UseSafetyCancel { get { return _safetycancel; } set { _safetycancel = value; } }

        public bool isRejected(int idx)
        {
            if ((idx < 0) || (idx >= Count))
                return false;
            return rejected[idx];
        }
        public bool isAccepted(int idx)
        {
            if ((idx < 0) || (idx >= Count))
                return false;
            return this[idx];
        }
        public bool isAccepted(long id)
        {
            int idx = getindex(id.ToString());
            if ((idx < 0) || (idx >= Count))
                return false;
            return this[idx];
        }
        public bool isRejected(long id)
        {
            int idx = getindex(id.ToString());
            if (idx < 0)
                return false;
            return rejected[idx];
        }

        public void reject(long id) { reject(id, true); }
        public void reject(long id, bool simulatedreject)
        {
            if (simulatedreject)
            {
                // get index
                int idx = getindex(id.ToString());
                // ignore unknown
                if (idx < 0)
                {
                    debug(id + " could not reject unknown id.");
                    return;
                }
                // cancel just in case
                if (UseSafetyCancel)
                    cancel(id);

                // mark as rejected
                rejected[idx] = true;
            }
            // reject
            if (SendRejectEvent != null)
                SendRejectEvent(id);
            else
                debug("no reject event defined on reject tracker");
        }

        public event MessageDelegate GotMessageEvent;

        public virtual void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
        {
            if (GotMessageEvent!=null)
                GotMessageEvent(type,source,dest,msgid,request,ref response);
        }

        public event OrderDelegate SendOrderEvent;

        public void newTick(Tick k)
        {
            checkrejects(gettime);
        }
        GenericTracker<long> times = new GenericTracker<long>();
        GenericTracker<long> ids = new GenericTracker<long>();
        public void GotOrder(Order o)
        {

            // get index
            int idx = getindex(o.id.ToString());
            // ensure we have this order
            if (idx < 0)
            {
                // check rejects before we quit
                checkrejects(gettime);
                // we're done
                return;
            }
            // ack the order
            this[idx] = true;
            // check rejects
            checkrejects(gettime);
        }
    }

    
}