using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.Common
{
    /// <summary>
    /// used to obtain valid ids for orders, responses, etc
    /// </summary>
    public class IdTracker : GenericTracker<long>, IConvertible
    {
        public static implicit operator long(IdTracker idt)
        {
            return idt.AssignId;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return AssignId;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(_first, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// number of unique ids (per symbol) that can be returned from a named assignment
        /// </summary>
        public int MaxNamedAssigns = 1;

        /// <summary>
        /// otherwise the same id is returned
        /// </summary>
        public bool isMagicIdOnMaxName = true;

        public long MagicId = 0;

        const long DEFAULTOWNER = 0;
        const long MAXOWNER = 512;
        // calculate mask length
        // 512 is 2^9.   32bits - 9bits for top = 23bits
        const int MASKLEN = 55;

        long _first = 0;
        long _nextid = 0;
        long _owner = 0;
        long _maxid = long.MaxValue;
        public long Count = 0;
        /// <summary>
        /// creates an object to assign unique order ids
        /// </summary>
        public IdTracker() : this(false,DEFAULTOWNER, OrderImpl.Unique) { }
        /// <summary>
        /// creates an object to assign unique ids
        /// </summary>
        /// <param name="OwnerId"></param>
        public IdTracker(long OwnerId) : this(true,OwnerId, OrderImpl.Unique) { }

        /// <summary>
        /// creates an object to assign unique order ids to one or more owners.
        /// </summary>
        /// <param name="OwnerId">A unique number identifying this owner</param>
        /// <param name="initialId">Owners first order id</param>
        public IdTracker(bool virtualids,long OwnerId, long initialId)
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
                    const long lowermask = (MAXOWNER - 1) << MASKLEN;
                    // get inverse to mask out top part
                    const long topmask = ~lowermask;
                    // top mask is also the count
                    Count = (topmask + 1);
                    // get seed as lower part
                    long seed = initialId & topmask;
                    // get high bits of first id
                    long highbits = _owner << MASKLEN;
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
        public long AssignId 
        { 
            get 
            { 
                if (NextOverflows) 
                    throw new IdTrackerOverflow();
                long next = System.Threading.Interlocked.Increment(ref _nextid);
                return next;
            } 
        }
        /// <summary>
        /// provides what next id will be without assigning it
        /// </summary>
        public long NextId { get { return _nextid; } }
        /// <summary>
        /// return true if next id will overflow
        /// </summary>
        public bool NextOverflows { get { return _nextid >= _maxid; } }

        public event TradeLink.API.DebugDelegate SendDebugEvent;

        protected void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
            
        }

        public const string UNKNOWN_IDNAME = "UNKNOWN_IDNAME";

        /// <summary>
        /// gets an id name from an order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getidname(long id)
        {
            int idx = id2idname.getindex(id.ToString());
            if (idx < 0)
                return UNKNOWN_IDNAME;
            return id2idname[idx];

        }
        /// <summary>
        /// get count of assignments from symbol and name
        /// </summary>
        /// <param name="idname"></param>
        /// <param name="sym"></param>
        /// <returns></returns>
        public int AssignCount(string idname, string sym)
        {
            string name = getidname(sym, idname);
            return AssignCount(name);
        }

        /// <summary>
        /// get count of assignments from this name
        /// </summary>
        /// <param name="idname"></param>
        /// <returns></returns>
        public int AssignCount(string idname)
        {
            var idx = getindex(idname);
            if (idx < 0)
                return 0;
            return idnamefires[idx];
        }

        

        /// <summary>
        /// gets an id name for a symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string getidname(string sym, string name)
        {
            return sym + "." + name;
        }
        /// <summary>
        /// gets a id name for a symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string idname(string sym, string name) { return getidname(sym, name); }


        /// <summary>
        /// get a current id name by symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="idname"></param>
        /// <returns></returns>
        public long this[string sym, string idname]
        {
            get { return this[getidname(sym,idname)]; }
            set { this[getidname(sym, idname)] = value; }
        }

        /// <summary>
        /// get a current idname by number
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="idname"></param>
        /// <returns></returns>
        public long this[int idx, string idname]
        {
            get { return this[idx + idname]; }
            set { this[idx + idname] = value; }
        }

        GenericTracker<int> idnamefires = new GenericTracker<int>();
        GenericTracker<string> id2idname = new GenericTracker<string>();

        /// <summary>
        /// get or set current id by name.
        /// NOTE : generally you will want to call this with a symbol,
        /// EG idt["IBM","my entry market"]
        /// 
        /// otherwise called like :
        /// (eg idt["myentrymkt"], idt["myentrylmt"], idt["myexitprofit"])
        /// Set name to zero to force reset on next use.
        /// </summary>
        /// <param name="idname"></param>
        /// <returns></returns>
        public override long this[string idname]
        {
            get
            {
                // see if we have this idname
                int idx = getindex(idname);
                
                // if we do, return the current numeric id
                if ((idx >= 0) && (base[idx] != 0))
                {
                    // test for max fire
                    if (idnamefires[idx] >= MaxNamedAssigns)
                    {
                        if (isMagicIdOnMaxName)
                            return MagicId;
                        // otherwise return current id
                        return base[idname];
                    }
                    // otherwise reassign and fire
                    var newid = AssignId;
                    idnamefires[idx]++;
                    id2idname.addindex(newid.ToString(), idname);
                    base[idx] = newid;
                    debug("idtracker " + idname + " assigned new id: " + newid + " count: " + idnamefires[idx]);
                    return newid;
                    
                }
                else if ((idx >= 0) && (base[idx] == 0))
                {
                    // test for max fire
                    if (idnamefires[idx] >= MaxNamedAssigns)
                    {
                        if (isMagicIdOnMaxName)
                            return MagicId;
                        // otherwise return current id
                        return base[idname];
                    }
                    var newid = AssignId;
                    base[idx] = newid;
                    id2idname.addindex(newid.ToString(), idname);
                    idnamefires[idx]++;
                    debug("idtracker idname: " + idname + " was reset, assigning new id: " + newid + " count: " + idnamefires[idx]);
                    return newid;
                }
                // if we don't, assign one... save and return it
                var newnameid = AssignId;
                idx = addindex(idname, newnameid);
                id2idname.addindex(newnameid.ToString(), idname);
                idnamefires.addindex(idname, 1);
                debug("idtracker idname: " + idname + " never used, assigning new id: " + newnameid + " count: " + idnamefires[idx]);
                return newnameid;
                
            }
            set
            {
                int idx = getindex(idname);
                if (idx < 0)
                {
                    addindex(idname, value);
                    id2idname.addindex(value.ToString(), idname);
                    idnamefires.addindex(idname, 0);
                    debug("idtracker idname: "+idname+" reset to: "+value);
                    return;
                }
                id2idname[idx] = idname;
                idnamefires[idx] = 0;
                base[idx] = value;
                debug("idtracker idname: " + idname + " reset to: " + value);
            }
        }
        /// <summary>
        /// clear all named ids
        /// (subsequent ids will still be unique from previous)
        /// </summary>
        public override void Clear()
        {
            id2idname.Clear();
            idnamefires.Clear();
            base.Clear();
        }

    }

    public class IdTrackerOverflow : Exception
    {
        public IdTrackerOverflow() : base("The maximum # of assigned order ids was exceeded.") { }
        public IdTrackerOverflow(string msg) : base(msg) { }
    }
}

