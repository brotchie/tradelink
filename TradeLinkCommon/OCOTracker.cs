using System;
using System.Collections.Generic;
using TradeLink.API;


namespace TradeLink.Common
{
    public delegate void OCOGroupDel(OCOGroup group);

    public class OCOTracker : GenericTracker<OCOGroup>
    {
        IdTracker _idt;
        public OCOTracker() : this(new IdTracker()) { }
        public OCOTracker(IdTracker idt) : this("OcoTracker", 100, idt) { }
        public OCOTracker(string name, int estsymbols, IdTracker idt) : base (estsymbols,name)
        {
            _idt = idt;
        }
        Dictionary<long, int> oid2symidx = new Dictionary<long, int>();


        bool _cancelonupdate = true;
        /// <summary>
        /// when sending new oco group for same symbol, cancel any existing oco
        /// (otherwise oco will cease to function)
        /// </summary>
        public bool CancelOcoOnUpdate { get { return _cancelonupdate; } set { _cancelonupdate = value; } }
        /// <summary>
        /// get group associated with a given order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OCOGroup this[long id]
        {
            get
            {
                int idx = -1;
                if (!oid2symidx.TryGetValue(id, out idx))
                {
                    return OCOGroup.EmptyGroup();
                }
                return base[idx];
            }
        }

        public event OrderDelegate SendOrderEvent;
        public event DebugDelegate SendDebugEvent;
        public event LongDelegate SendCancelEvent;
        public event OCOGroupDel SendOCOGroupEvent;
        public event OCOGroupDel SendOCOGroupUpdateEvent;
        public event OCOGroupDel SendOCOGroupCancelEvent;

        /// <summary>
        /// determine whether given order id is member of an oco group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isGroupMember(long id)
        {
            int idx = -1;
            return oid2symidx.TryGetValue(id,out idx);
        }

        /// <summary>
        /// determine whether given order id is member of a specific oco group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool isGroupMember(long id, string sym)
        {
            int idx = -1;
            int symidx = getindex(sym);
            return oid2symidx.TryGetValue(id, out idx) && (symidx==idx);
        }

        /// <summary>
        /// determine whether given order id is member of a specific oco group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool isGroupMember(long id, int idx)
        {
            int symidx = -1;
            return oid2symidx.TryGetValue(id, out symidx) && (symidx == idx);
        }

        void cancel(long id)
        {
            if ((SendCancelEvent != null) && (id!=0))
                SendCancelEvent(id);
            else if (id!=0)
                debug("Can't cancel "+id+" because no SendCancelEvent defined!");
        }
        /// <summary>
        /// pass fills to oco tracker
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            if (!isGroupMember(f.id))
            {
                v("oco ignoring fill: " + f.id + " as not a group member.");
                return;
            }
            // get group
            OCOGroup grp = this[f.id];
            // notify
            debug(f.symbol + " for fill: " + f.id + " found ocogroup: " + grp.ToString());
            // cancel other ids
            if (grp.isValid)
            {
                v("ocogroup: " + getindex(grp.SymbolOwner) + " attempting to cancel any open orders.");
                cancelgrp(grp, f.id);
            }
            else
                v("ocogroup: " + grp.ToString() + " not valid, no cancels to send.");
        }

        /// <summary>
        /// cancel a group by index
        /// </summary>
        /// <param name="idx"></param>
        public void CancelGroup(int idx)
        {
            CancelGroup(this[idx]);
        }

        /// <summary>
        /// Cancel an entire group
        /// </summary>
        /// <param name="grp"></param>
        public void CancelGroup(OCOGroup grp)
        {
            
            cancelgrp(grp, 0);
        }
        void cancelgrp(OCOGroup grp, long ex)
        {
            if (!grp.isValid)
            {
                v(grp.SymbolOwner+" no oco being used. ["+grp.ToString()+"]");
                return;
            }
            int idx = getindex(grp.SymbolOwner);
            debug("Canceling group: " + grp.ToString());
            bool ok = false;
            for (int i = 0; i < grp.Count; i++)
                if (grp[i] != ex)
                {
                    ok = true;
                    v("group: " + idx + " canceling orderid: " + grp[i]);
                    cancel(grp[i]);
                }
            if (ok && (SendOCOGroupCancelEvent!=null))
            {
                SendOCOGroupCancelEvent(grp);
            }
            
        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        /// <summary>
        /// pass cancels to oco tracker
        /// </summary>
        /// <param name="id"></param>
        public void GotCancel(long id)
        {
            if (!isGroupMember(id))
                return;
            // get group
            OCOGroup grp = this[id];
            // cancel other ids
            cancelgrp(grp, id);
        }

        bool isnewid(long id, int newidx)
        {
            int idx = -1;
            if (oid2symidx.TryGetValue(id, out idx))
            {
                oid2symidx[id] = newidx;
            }
            else
            {

                oid2symidx.Add(id, newidx);
                return true;
            }
            return false;
        }

        long[] getids(ref Order[] orders)
        {
            long[] ids = new long[orders.Length];
            for (int i = 0; i < orders.Length; i++)
            {
                if (orders[i].id == 0)
                    orders[i].id = _idt.AssignId;
                ids[i] = orders[i].id;

            }
            return ids;
        }

        /// <summary>
        /// sends orders which are in same order group
        /// </summary>
        /// <param name="orders"></param>
        public void sendorder(params Order[] orders)
        {
            OCOGroup grp = new OCOGroup(getids(ref orders), orders);

            sendorder(grp);
        }

        void sendorder(OCOGroup grp)
        {
            if (SendOrderEvent != null)
            {
                debug("Send request for: " + grp.ToString());
                // see if we already had group
                int idx = getindex(grp.SymbolOwner);
                bool exists = (idx >= 0);
                if (exists)
                {
                    if (this[idx].isValid)
                    {
                        debug("Already had group for: " + grp.SymbolOwner);

                        if (CancelOcoOnUpdate)
                        {
                            CancelGroup(grp);
                        }
                    }
                }
                else
                {
                    // ensure we have group
                    idx = addindex(grp.SymbolOwner, grp);
                    // save group
                    this[grp.SymbolOwner] = grp;
                }



                // send orders
                for (int i = 0; i < grp.orders.Length; i++)
                {
                    // save relationship
                    bool newid = isnewid(grp.groupids[i], idx);
                    v("oco group id: " + idx + " sending orderid: " + grp.groupids[i] + " " + grp.orders[i].ToString());
                    // send order
                    SendOrderEvent(grp.orders[i]);
                }

                // notify
                if (!exists && (SendOCOGroupEvent != null))
                    SendOCOGroupEvent(grp);
                if (exists && (SendOCOGroupUpdateEvent != null))
                    SendOCOGroupUpdateEvent(grp);
                debug("finished sending oco group: " + idx);
                
            }
            else
                debug("Can't send group as no SendOrderEvent present.");

        }
        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }
        bool _noverb = true;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }
    }

    /// <summary>
    /// represents a group of OCO orders
    /// </summary>
    public struct OCOGroup
    {
        /// <summary>
        /// get a particular order from group by it's index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public long this[int idx] { get { return groupids[idx]; } }
        /// <summary>
        /// gets total number of orders in group (for use with indexing)
        /// </summary>
        public int Count { get { return groupids.Length; } }
        /// <summary>
        /// true if oco group is valid group
        /// </summary>
        public bool isValid { get { return (orders.Length == groupids.Length) && (orders.Length > 0) && (SymbolOwner != string.Empty); } }
        /// <summary>
        /// returns symbol of first order in group, this is the owner of the group
        /// </summary>
        public string SymbolOwner;
        /// <summary>
        /// whether a given order is a member of this group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isMember(long id)
        {
            for (int i = 0; i < groupids.Length; i++)
                if (groupids[i] == id) return true;
            return false;
        }
        public static OCOGroup EmptyGroup()
        {
            OCOGroup grp = new OCOGroup();
            grp.SymbolOwner = string.Empty;
            grp.orders = new Order[0];
            grp.groupids = new long[0];
            return grp;

        }
            
        /// <summary>
        /// create an oco group
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ords"></param>
        public OCOGroup(long[] ids, Order[] ords)
        {
            if (ords.Length > 0)
                SymbolOwner = ords[0].symbol;
            else
                SymbolOwner = string.Empty;
            groupids = ids;
            orders = ords;
        }
        public long[] groupids;
        public Order[] orders;

        public override string ToString()
        {
            string s = "OCO: [" + SymbolOwner + "] ";
            foreach (Order o in orders)
                s += o.ToString() + " ";
            return s;
        }
    }
}
