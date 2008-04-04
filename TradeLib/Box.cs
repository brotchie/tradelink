using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace TradeLib
{
    public delegate void DebugDelegate(string msg);
    /// <summary>
    /// A container for trading strategies or trading rules, and the things necessary to trade and track these rules on a single stock or instrument.
    /// </summary>
    [Serializable]
    public class Box
    {
        NewsService news = null;
        protected event IndexDelegate GotIndex;
        private string name = "Unnamed";
        private string ver = "$Rev: 1036 $";
        private string symbol;
        private int tDir = 0;
        private decimal avgprice = 0;
        private int trades = 0;
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
        private int sday = 930;
        private int eday = 1600;
        private string loadstr = "";
        private bool _email = false;
        private bool _quickorder = true;
        public int DayEndBuff = 2;


        
        public Box () : this(null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="ns">The NewsService that will receive news events (debugs, errors) from this box.</param>
        public Box(NewsService ns) // constructor
        {
            this.news = ns;
        }

        /// <summary>
        /// Send this box a new Index tick.
        /// </summary>
        /// <param name="i">The latest index update.</param>
        public void NewIndex(Index i)
        {
        	if (GotIndex!=null) GotIndex(i);
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
            }
            if (!pos.isValid) throw new Exception("Invalid Position Provided to Box" + pos.ToString());
            if (tick.sym != Symbol) return o;
            time = tick.time;
            date = tick.date;

           
            if ((Time < DayStart) || (Time>DayEnd)) return o; // is market open?
            if (Off) return o; // don't trade if shutdown
            if (TradeCaps &&  pos.Flat && ((this.trades >= MAXTRADES) || (this.adjusts >= MAXADJUSTS)))
            {
                this.Shutdown("Trade limit reached.");
                return o;
            }
            if ((pos.AvgPrice != AvgPrice) || (pos.Size != PosSize))
            {
                avgprice = pos.AvgPrice;
                tDir = pos.Size;
            }

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

            //flat us at the close
            if ((Time >= (DayEnd - DayEndBuff))) 
                o = this.Adjust(Flat);
            if (o.isValid)
            {
                // send our order
                o.time = Time;
                o.date = Date;
                this.D("Sent order: " + o);
            }
            return o;
        }


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
            if (o.isValid)
            {
                adjusts++;
                // if we're going to flat from non-flat, this is a "trade"
                if ((Math.Abs(size+ts)==0) && (ts!=0)) trades++;
            }
            return o;
        }

        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        protected int Norm2Min(int size)
        {
            int wmult = (int)Math.Ceiling((decimal)size/MINSIZE);
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

        protected List<string> _iname = new List<string>(0);
        protected List<object> _indicators = new List<object>(0);
        protected int _indicount = 0;
        /// <summary>
        /// Resets the indicators tracked by this box to zero.
        /// </summary>
        /// <param name="IndicatorCount">The indicator count.</param>
        public void ResetIndicators(int IndicatorCount)
        {
            _indicators = new List<object>(IndicatorCount);
            _indicount = IndicatorCount;
            _iname = new List<string>(IndicatorCount);
            for (int i = 0; i < IndicatorCount; i++)
            {
                _indicators.Add(0);
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
        public string[] Inames
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
            symbol = null; SHUTDOWN = false; trades = 0; adjusts = 0; 
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
        /// Sends a debug message from a box.
        /// </summary>
        /// <param name="debug">The debug.</param>
        public virtual void D(string debug)
        {
            if (!this.DEBUG) return;
            if (this.news != null) this.news.newNews("["+Name+"] "+Symbol+" "+Date+":"+Time+" "+debug);
            else Console.WriteLine(debug);
            return;
        }
        /// <summary>
        /// Sends a debug message even when debugging is disabled.
        /// </summary>
        /// <param name="deb">The deb.</param>
        public virtual void d(string deb) { if (this.news != null) this.news.newNews("[" + Name + "] " + Symbol+ " " + deb); }


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
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Average Position Price")]
        public decimal AvgPrice { get { return avgprice; } }
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
        /// Gets the currently trading symbol.
        /// </summary>
        /// <value>The symbol.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Current symbol")]
        public string Symbol { get { return symbol; } }
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
        public int Turns { get { return trades; } }
        /// <summary>
        /// Gets the adjustments completed by this box.
        /// </summary>
        /// <value>The adjusts.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Number of position adjustments- or executions- this box has made.")]
        public int Adjusts { get { return adjusts; } }

        /// <summary>
        /// Gets or sets the news service that handles news events from this box, especially the debug messages.
        /// </summary>
        /// <value>The news handler.</value>
        [BrowsableAttribute(false)]
        public NewsService NewsHandler { get { return news; } set { news = value; } }
        [BrowsableAttribute(false)]
        protected string NameVersion { get { return Name + CleanVersion; } }
        [BrowsableAttribute(false)]
        protected string CleanVersion { get { return Util.CleanVer(Version); } }
        /// <summary>
        /// Gets the size of the current position held, zero for no position.
        /// </summary>
        /// <value>The size of the pos.</value>
        [BrowsableAttribute(false)]
        public int PosSize { get { return tDir; } }
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
        public static Box FromDLL(string boxname, string dllname,NewsService ns)
        {
            System.Reflection.Assembly a;
            try
            {
                a = System.Reflection.Assembly.LoadFrom(dllname);
            }
            catch (Exception ex) { ns.newNews(ex.Message); return new Box(ns); }
            return FromAssembly(a, boxname, ns);
        }
        /// <summary>
        /// Create a box from an Assembly containing Box classes.
        /// </summary>
        /// <param name="a">the assembly.</param>
        /// <param name="boxname">The fully-qualified boxname.</param>
        /// <param name="ns">The NewsService where this box will send debugs and errors.</param>
        /// <returns></returns>
        public static Box FromAssembly(System.Reflection.Assembly a, string boxname, NewsService ns)
        {
            Type type;
            object[] args;
            Box mybox = new Box(ns);
            try
            {
                type = a.GetType(boxname, true, true);
            }
            catch (Exception ex) { ns.newNews(ex.Message); return mybox; }
            args = new object[] { ns };
            try
            {
                mybox = (Box)Activator.CreateInstance(type, args);
            }
            catch (Exception ex) 
            { 
                ns.newNews(ex.Message); 
                if (ex.InnerException != null) 
                    ns.newNews(ex.InnerException.Message); 
                return mybox; 
            }
            mybox.FullName = boxname;
            return mybox;
        }
    }
}
