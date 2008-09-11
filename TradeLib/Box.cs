using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace TradeLib
{
    public delegate void DebugDelegate(string msg);
    public delegate void ObjectArrayDelegate(object[] parameters);
    /// <summary>
    /// A container for trading strategies or trading rules, and the things necessary to trade and track these rules on a single stock or instrument.
    /// </summary>
    [Serializable]
    public class Box
    {
        public event DebugFullDelegate GotDebug;
        protected event IndexDelegate GotIndex;
        public event ObjectArrayDelegate IndicatorUpdate;
        private string name = "Unnamed";
        private string ver = "$Rev: 1036 $";
        private string symbol;
        private int turns = 0;
        private int adjusts = 0;
        private bool DEBUG = false;
        private bool SHUTDOWN = false;
        private bool USELIMITS = false;
        private int MAXTRADES = Int32.MaxValue;
        private int MAXADJUSTS = Int32.MaxValue;
        private int MAXSIZE = 100;
        private int MINSIZE = 100;
        private int date = 0;
        private int time = 0;
        private int sec = 0;
        private int sday = 930;
        private int eday = 1600;
        private string loadstr = "";
        private bool _email = false;
        private bool _quickorder = true;
        private bool _multipleorders = false;
        private int _expectedpossize = 0;
        public int DayEndBuff = 2;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="ns">The NewsService that will receive news events (debugs, errors) from this box.</param>
        public Box() // constructor
        {
        }

        /// <summary>
        /// Send this box a new Index tick.
        /// </summary>
        /// <param name="i">The latest index update.</param>
        public void NewIndex(Index i)
        {
        	if (GotIndex!=null) GotIndex(i);
        }

        protected void UpdateIndicators()
        {
            if (IndicatorUpdate != null) IndicatorUpdate(Indicators);
        }

        /// <summary>
        /// Trades specified tick.  This method will call an inherited overridden member Read if QuickOrder is true or ReadOrder if QuickOrder is false.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <param name="bl">The barlist.</param>
        /// <param name="pos">The current position in the stock or instrument.</param>
        /// <param name="bi">Other box information.</param>
        /// <returns></returns>
        public Order Trade(Tick tick,BarList bl, Position pos,BoxInfo bi)
        {
            Order o = new Order();
            if (Symbol == null)
            {
                if (tick.sym != "") symbol = tick.sym;
                else throw new Exception("No symbol specified");
                _pos = new Position(tick.sym);
            }
            if (!pos.isValid) throw new Exception("Invalid Position Provided to Box" + pos.ToString());
            if (tick.sym != Symbol) return o;
            time = tick.time;
            date = tick.date;
            sec = tick.sec;

           
            if ((Time < DayStart) || (Time>DayEnd)) return o; // is market open?
            if (Off) return o; // don't trade if shutdown
            if (TradeCaps &&  pos.Flat && ((this.turns >= MAXTRADES) || (this.adjusts >= MAXADJUSTS)))
            {
                this.Shutdown("Trade limit reached.");
                return o;
            }
            _pos = pos;

            if (QuickOrder) // user providing only size adjustment
            {
                // get our adjustment
                int adjust = this.Read(tick, bl,bi);  

                // convert adjustment to an order
                o = this.Adjust(adjust); 
            }
            else // user providing a complete order, so get it
            {
                o = ReadOrder(tick, bl,bi);
            }

            if (!OrdersAllowed) // if we're not allowed, mark order as invalid
                o = new Order();

            //flat us at the close
            if (!_sentshut && (Time >= (DayEnd - DayEndBuff)))
            {
                o = this.Adjust(Flat);
                o.time = Time;
                o.date = Date;
                this.Shutdown("end-of-day");
                _sentshut = true;
            }
            if (o.isValid)
            {
                // if it's a valid order it counts as an adjustment
                adjusts++;
                // if we're going to flat from non-flat, this is a "trade"
                if ((Math.Abs(PosSize + o.SignedSize) == 0) && (PosSize != 0)) 
                    turns++;

                // final prep for good orders
                _expectedpossize += o.SignedSize;
                o.time = Time;
                o.date = Date;
            }
            
            if (o.isValid) this.D("Sent order: " + o);
            return o; // send our order
        }

        private bool _sentshut = false;

        private bool OrdersAllowed { get { return (_multipleorders || (!_multipleorders && (PosSize == _expectedpossize))); } }


        /// <summary>
        /// Adjusts the box's current position up or down by the specified number of shares.
        /// </summary>
        /// <param name="asize">The adjustment size. Zero for no change.</param>
        /// <returns>A market order for specified size</returns>
        protected Order Adjust(int asize)
        {
            if (asize == 0) return new Order();
            int size = asize;
            int ts = PosSize;
            if (Math.Abs(size) < 11) size = Norm2Min(ts * size);
            Boolean side = size > 0;
            size = NoCrossingFlat(size);
            if (Math.Abs(size + ts) > MAXSIZE) size = (MAXSIZE - Math.Abs(ts)) * (side ? 1 : -1);
            Order o = new Order(Symbol, side, size,Name);
            return o;
        }

        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        protected int Norm2Min(decimal size)
        {
            int wmult = (int)Math.Ceiling(size/MINSIZE);
            return wmult * MINSIZE;
        }

        private int NoCrossingFlat(int size) 
        {
            if ((PosSize!=0) &&
                ((PosSize+size)!=0) &&
                (((PosSize+size)*PosSize)<0))
                size = -1 * PosSize;
            return size;
        }
        protected List<uint> buyids = new List<uint>();
        protected List<uint> sellids = new List<uint>();
        public void gotOrderSink(Order o)
        {
            if (o.symbol != Symbol) return;
            if (o.side && !buyids.Contains(o.id))
                buyids.Add(o.id);
            else if (!o.side && !sellids.Contains(o.id))
                sellids.Add(o.id);
        }
        public void gotCancelSink(uint cancelid)
        {
            if (buyids.Contains(cancelid))
                buyids.Remove(cancelid);
            if (sellids.Contains(cancelid))
                sellids.Remove(cancelid);
        }
        public event UIntDelegate CancelOrderSource;
        public void CancelOrders() { CancelOrders(true); CancelOrders(false); }
        public void CancelOrders(bool side)
        {
            if (CancelOrderSource == null) return;
            if (side)
                foreach (uint id in buyids.ToArray())
                    CancelOrderSource(id);
            else
                foreach (uint id in sellids.ToArray())
                    CancelOrderSource(id);
        }

        protected List<string> _iname = new List<string>(0);
        protected List<object> _indicators = new List<object>(0);
        protected int _indicount = 0;
        /// <summary>
        /// Resets the indicators tracked by this box to zero.
        /// </summary>
        /// <param name="IndicatorCount">The indicator count.</param>
        protected void ResetIndicators(int IndicatorCount)
        {
            _indicators = new List<object>(IndicatorCount);
            _indicount = IndicatorCount;
            bool hasiname = ((_iname.Count > 0) && (_iname[0] != "i0")); 
            if (!hasiname) 
                _iname = new List<string>(IndicatorCount);
            for (int i = 0; i < IndicatorCount; i++)
            {
                _indicators.Add(0);
                if (!hasiname)
                    _iname.Add("i" + i);
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has indicators.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has indicators; otherwise, <c>false</c>.
        /// </value>
        [BrowsableAttribute(false)]
        public bool hasIndicators { get { return _indicators.Count != 0; } }
        [BrowsableAttribute(false)]
        public object[] Indicators
        {
            get { return _indicators.ToArray(); }
            set
            {
                if (value.Length != _indicators.Capacity)
                    throw new Exception("Must provide all indicator values when specifying array.");
                for (int i = 0; i < value.Length; i++)
                    _indicators[i] = value[i];
            }
        }
        /// <summary>
        /// Gets or sets the Indicator names.
        /// </summary>
        /// <value>The inames.</value>
        [BrowsableAttribute(false)]
        public string[] IndicatorNames
        {
            get { return _iname.ToArray(); }
            set
            {
                if (value.Length != _indicators.Capacity)
                    throw new Exception("Must provide all indicator values when specifying array.");
                for (int i = 0; i < value.Length; i++)
                    _iname[i] = value[i];
            }
        }
        /// <summary>
        /// Gets the indicator name.
        /// </summary>
        /// <param name="i">The indicator number.</param>
        /// <returns>the name of the indicator</returns>
        protected string GetIname(int i)
        {
            if ((i >= _indicators.Capacity) || (i < 0))
                throw new IndexOutOfRangeException("Cannot access an index beyond what was defined with ResetIndicators(int IndicatorCount)");
            return _iname[i];
        }
        /// <summary>
        /// Sets the indicator name.
        /// </summary>
        /// <param name="i">The indicator number.</param>
        /// <param name="value">The name.</param>
        protected void SetIname(int i, string value)
        {
            if ((i >= _indicators.Capacity) || (i < 0))
                throw new IndexOutOfRangeException("Cannot access an index beyond what was defined with ResetIndicators(int IndicatorCount)");
            _iname[i] = value;
        }

        /// <summary>
        /// Gets the current indicator value.
        /// </summary>
        /// <param name="i">The indicator to get the value of.</param>
        /// <returns>the value of the indicator</returns>
        protected object GetIndicator(int i)
        {
            if ((i >= _indicators.Capacity) || (i < 0))
                throw new IndexOutOfRangeException("Cannot access an index beyond what was defined with ResetIndicators(int IndicatorCount)");
            return _indicators[i];
        }
        /// <summary>
        /// Sets the indicator.
        /// </summary>
        /// <param name="i">The indicator number.</param>
        /// <param name="value">The indicator's value.</param>
        protected void SetIndicator(int i, object value)
        {
            if ((i >= _indicators.Capacity) || (i < 0))
                throw new IndexOutOfRangeException("Cannot access an index beyond what was defined with ResetIndicators(int IndicatorCount)");
            _indicators[i] = value;
        }

        /// <summary>
        /// Reset this box instance.  (eg for another box run, a new trading day, etc)
        /// </summary>
        public virtual void Reset() 
        {
            symbol = null; SHUTDOWN = false; turns = 0; adjusts = 0; 
            DayStart = 930; DayEnd = 1600;
            ResetIndicators(_indicount);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Box"/> has debugs enabled.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Enables processing of debug messages sent via D().")]
        public bool Debug { get { return DEBUG; } set { DEBUG = value; } }
        /// <summary>
        /// Shutdowns the box so no more trades occur, with the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Shutdown(string message)
        {
            this.SHUTDOWN = true;
            this.D(message);
        }

        /// <summary>
        /// Activates a box from shutdown state, with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void Activate(string message)
        {
            this.SHUTDOWN = false;
            this.D(message);
        }
        /// <summary>
        /// Sends an email alert from a box.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="msg">The MSG.</param>
        public virtual void E(string to, string from, string subject, string msg)
        {
            if (!_email) return;
            Email.Send(to, from, subject, msg);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow multiple orders].
        /// </summary>
        /// <value><c>true</c> if [allow multiple orders]; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Whether this box waits for position size to match sent orders, before new order is sent.")]
        public bool AllowMultipleOrders { get { return _multipleorders; } set { _multipleorders = value; } }

        public void S(string status)
        {
            string prefix = "[" + Name + "] " + Symbol + " " + Date + ":" + Time + " ";
            if (GotDebug != null) 
                GotDebug(TradeLib.Debug.Create(prefix + status, DebugLevel.Status)); 
        }


        /// <summary>
        /// Sends a debug message from a box.
        /// </summary>
        /// <param name="debug">The debug.</param>
        public virtual void D(string debug)
        {
            if (!this.DEBUG) return;
            d(debug);
        }
        /// <summary>
        /// Sends a debug message even when debugging is disabled.
        /// </summary>
        /// <param name="deb">The deb.</param>
        public virtual void d(string debug) { if (GotDebug != null) GotDebug(TradeLib.Debug.Create("[" + Name + "] " + Symbol + " " + Date + ":" + Time + " " + debug, DebugLevel.Debug)); }


        /// <summary>
        /// Called everytime a new tick is received by the box, if QuickOrder is true (default).  Reads from the user a position adjustment as an integer.
        /// </summary>
        /// <param name="tick">The current tick.</param>
        /// <param name="bl">The current barlist.</param>
        /// <param name="boxinfo">The boxinfo.</param>
        /// <returns>The number of shares to adjust the position up or down.</returns>
        protected virtual int Read(Tick tick, BarList bl,BoxInfo boxinfo) { D("No Read function provided"); return 0; }
        /// <summary>
        /// Called everytime a new tick is received by the box, if QuickOrder is false.  Reads from the user a position adjustment as an order.
        /// </summary>
        /// <param name="tick">The current tick.</param>
        /// <param name="bl">The current barlist.</param>
        /// <param name="boxinfo">The boxinfo.</param>
        /// <returns>The number of shares to adjust the position up or down.</returns>        protected virtual Order ReadOrder(Tick tick, BarList bl,BoxInfo boxinfo) { D("No ReadOrder function provided.");  return new Order(); }
        protected virtual Order ReadOrder(Tick tick, BarList bl, BoxInfo boxinfo) { D("No ReadOrder function provided."); return new Order(); }
        
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("User-Supplied version number.")]
        public virtual string Version { get { return ver; } set { ver = value; } }
        /// <summary>
        /// Gets or sets a value indicating whether [trade caps] are enabled.  TradeCaps will shutdown the box is MaxTurns or MaxAdjusts is reached.
        /// </summary>
        /// <value><c>true</c> if [trade caps]; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Toggle enforcing of MaxTurns and MaxAdjusts")]
        public bool TradeCaps { get { return USELIMITS; } set { USELIMITS = value; } }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Box"/> sends emails.
        /// </summary>
        /// <value><c>true</c> if emails; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Allow emails from the box to be sent.")]
        public bool Emails { get { return _email; } set { _email = value; } }
        /// <summary>
        /// Gets or sets the day start, in 24hour time as an integer (eg 1415 = 2:15PM).
        /// </summary>
        /// <value>The day start.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Sets inclusive start time for box each day.")]
        public int DayStart { get { return sday; } set { sday = value; } }
        /// <summary>
        /// Gets or sets the day end, in 24hour time as an integer (eg 1600 = 4:00PM)
        /// </summary>
        /// <value>The day end.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Sets inclusive stop time for box each day.")]
        public int DayEnd { get { if ((eday % 100) != 0) return eday - DayEndBuff; return eday - 40 - DayEndBuff; } set { eday = value; } }
        /// <summary>
        /// Gets the current date.
        /// </summary>
        /// <value>The date.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Date")]
        public int Date { get { return date; } }
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Sec")]
        public int Sec { get { return sec; } }
        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>The time.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Time")]
        public int Time { get { return time; } }
        /// <summary>
        /// Gets or sets the max roundturns before the box is shutdown.  Only used when TradeCaps is true.
        /// </summary>
        /// <value>The max turns.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("If TradeCaps is true, Shutdown box when this many roundturns is reached.")]
        public int MaxTurns { get { return MAXTRADES; } set { MAXTRADES = value; } }
        /// <summary>
        /// Gets or sets the max adjustments before the box is shutdown.  Only used when TradeCaps is true.
        /// </summary>
        /// <value>The max adjusts.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("If TradeCaps is true, Shutdown box after this many executions.")]
        public int MaxAdjusts { get { return MAXADJUSTS; } set { MAXADJUSTS = value; } }
        /// <summary>
        /// Gets or sets the size of the maximum position size allowed by this box (maxsize of a single position).
        /// </summary>
        /// <value>The size of the max.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Maximum size of a single position.")]
        public int MaxSize { get { return MAXSIZE; } set { MAXSIZE = value; } }
        /// <summary>
        /// Gets or sets the size of the minimum size of aposition allowed.
        /// </summary>
        /// <value>The size of the min.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Minimum size of a single position.")]
        public int MinSize { get { return MINSIZE; } set { MINSIZE = value; } }
        /// <summary>
        /// Gets or sets a value indicating whether [quick order] is used.  Quickorders allow the user to set size adjustments as an integer which is converted into a buy or sell market.  Otherwise a full order type is read from the user.
        /// </summary>
        /// <value><c>true</c> if [quick order]; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Whether this box returns full orders or just adjustments to position size.")]
        public bool QuickOrder { get { return _quickorder; } set { _quickorder = value; } }
        /// <summary>
        /// Gets the avg price of any current position (zero for no position).
        /// </summary>
        /// <value>The avg price.</value>
        /// 
        private Position _pos = null;
        [BrowsableAttribute(false)]
        public Position Pos { get { return _pos; } } 
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Average Position Price")]
        public decimal AvgPrice { get { return (_pos!=null) ? _pos.AvgPrice : 0; } }
        /// <summary>
        /// Gets or sets the full name of the box, as defined in the source code.
        /// </summary>
        /// <value>The full name.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Fully qualified box class name."), ReadOnlyAttribute(true)]
        public string FullName { get { return loadstr; } set { loadstr = value; } }
        /// <summary>
        /// Gets or sets the name of this box as described by the user.
        /// </summary>
        /// <value>The name.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Name of this box.")]
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// Gets or sets the currently trading symbol.
        /// </summary>
        /// <value>The symbol.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Current symbol")]
        public string Symbol { get { return symbol; } set { symbol = value; } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="Box"/> is shutdown.
        /// </summary>
        /// <value><c>true</c> if off; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink BoxInfo"), Description("True if the box is Shutdown.")]
        public bool Off { get { return SHUTDOWN; } }
        /// <summary>
        /// Gets the turns currently completed by this box.
        /// </summary>
        /// <value>The turns.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Number of round turns this box has made (doesn't take into account size of a half-turn).")]
        public int Turns { get { return turns; } }
        /// <summary>
        /// Gets the adjustments completed by this box.
        /// </summary>
        /// <value>The adjusts.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Number of position adjustments- or executions- this box has made.")]
        public int Adjusts { get { return adjusts; } }

         [BrowsableAttribute(false)]
        protected string NameVersion { get { return Name + CleanVersion; } }
        [BrowsableAttribute(false)]
        protected string CleanVersion { get { return Util.CleanVer(Version); } }
        /// <summary>
        /// Gets the size of the current position held, zero for no position.
        /// </summary>
        /// <value>The size of the pos.</value>
        [BrowsableAttribute(false)]
        public int PosSize { get { return (_pos!=null) ? _pos.Size : 0; } }
        /// <summary>
        /// Gets the position size required to flat any current position (zero if no position).
        /// </summary>
        /// <value>The flat.</value>
        [BrowsableAttribute(false)]
        protected int Flat { get { return PosSize * -1; } }
        /// <summary>
        /// Gets the position size required to half any current position (zero if none to halve).
        /// </summary>
        /// <value>The half.</value>
        [BrowsableAttribute(false)]
        protected int Half { get { return -1 * Norm2Min(PosSize / 2); } }
        /// <summary>
        /// Gets the size of a profit-taking amount, defined by the user by overriding this field.  Set to Half by default.
        /// </summary>
        /// <value>The size of the take profit.</value>
        [BrowsableAttribute(false)]
        protected virtual int TakeProfitSize { get { return Half; } }


        /// <summary>
        /// Create a box from a DLL containing Box classes.  
        /// </summary>
        /// <param name="boxname">The fully-qualified boxname (as in Box.FullName).</param>
        /// <param name="dllname">The dllname.</param>
        /// <param name="ns">The NewsService this box will use.</param>
        /// <returns></returns>
        public static Box FromDLL(string boxname, string dllname)
        {
            System.Reflection.Assembly a;
            try
            {
                a = System.Reflection.Assembly.LoadFrom(dllname);
            }
            catch (Exception ex) { Box b = new Box(); b.Name = ex.Message; return b; }
            return FromAssembly(a, boxname);
        }
        /// <summary>
        /// Create a box from an Assembly containing Box classes.
        /// </summary>
        /// <param name="a">the assembly.</param>
        /// <param name="boxname">The fully-qualified boxname.</param>
        /// <param name="ns">The NewsService where this box will send debugs and errors.</param>
        /// <returns></returns>
        public static Box FromAssembly(System.Reflection.Assembly a, string boxname)
        {
            Type type;
            object[] args;
            Box b = new Box();
            try
            {
                type = a.GetType(boxname, true, true);
            }
            catch (Exception ex) { b = new Box(); b.Name = ex.Message; return b; }
            args = new object[] { };
            try
            {
                b = (Box)Activator.CreateInstance(type, args);
            }
            catch (Exception ex) 
            {
                b = new Box(); b.Name = ex.InnerException.Message; return b;
            }
            b.FullName = boxname;
            return b;
        }
    }
}
