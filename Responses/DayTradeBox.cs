using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    public class DayTradeResponse : Response
    {
        protected event TickDelegate gotTick;
        public event FillDelegate gotFill;
        protected event UIntDelegate gotOrderCancel;
        protected event OrderDelegate gotOrder;
        public event UIntDelegate SendCancel;
        public event OrderDelegate SendOrder;
        public event DebugFullDelegate SendDebug;
        public event ObjectArrayDelegate SendIndicators;

        BarListImpl _bl = new BarListImpl();
        Position _pos = new PositionImpl();
        string[] _iname = new string[0];
        string[] _syms = new string[0];
        private int _date = 0;
        private int _time = 0;
        private int _sec = 0;
        private int _sday = 930;
        private int _eday = 1600;
        protected int MAXSIZE = 100;
        protected int MINSIZE = 100;
        private string _name = "";
        private string _fullname = "";
        private int _DayEndBuff = 2;
        private bool _shut = false;
        private bool _multipleorders = false;
        private int _expectedpossize = 0;
        protected List<uint> _buyids = new List<uint>();
        protected List<uint> _sellids = new List<uint>();
        public void Indicate(object[] values) { if (SendIndicators != null) SendIndicators(values); }


        public void GotTick(Tick tick)
        {
            if (tick.symbol == "") return;
            if ((_date != 0) && (tick.date != _date))
                Reset();
            if (!string.Join(",",_syms).Contains(tick.symbol))
            {
                string[] newsym = new string[_syms.Length + 1];
                _syms.CopyTo(newsym, 0);
                newsym[_syms.Length] = tick.symbol;
                _syms = newsym;
                try
                {
                    if (Util.isEarlyClose(tick.date))
                        DayEnd = Util.GetEarlyClose(tick.date);
                }
                catch (Exception ex) { D("Exception checking for EarlyClose..."+ex.Message); }
            }
            Order o = new OrderImpl();
            _bl.newTick(tick);

            _time = tick.time;
            _date = tick.date;
            _sec = tick.sec;

            if ((Time < DayStart) || (Time > DayEnd)) return; // is market open?
            if (Off) return; // don't trade if shutdown


            o = ReadOrder(tick, _bl);

            if (!OrdersAllowed) // if we're not allowed, mark order as invalid
                o = new OrderImpl();

            //flat us at the close
            if (!_sentshut && (Time >= (DayEnd - _DayEndBuff)))
            {
                o = new MarketOrder(Symbol, -1 * _pos.Size);
                o.time = Time;
                o.date = Date;
                this.Shutdown("end-of-day");
                _sentshut = true;
            }
            if (o.isValid)
            {
                // final prep for good orders
                _expectedpossize += o.size;
                o.time = Time;
                o.date = Date;
                D("Sent order: " + o);
                if (SendOrder != null)
                    SendOrder(o); // send our order
                else 
                    D("No route for order. Dropped.");
            }

            if (gotTick != null)
                gotTick(tick);

        }

        public void GotOrderCancel(uint cancelid)
        {
            // track current ids
            if (_buyids.Contains(cancelid))
                _buyids.Remove(cancelid);
            if (_sellids.Contains(cancelid))
                _sellids.Remove(cancelid);

            // pass through as an event
            if (gotOrderCancel != null)
                gotOrderCancel(cancelid);
        }

        public void GotFill(Trade t)
        {
            // track current position
            _pos.Adjust(t);

            // pass through as an event
            if (gotFill != null)
                gotFill(t);
        }

        public void GotPosition(Position pos)
        {
            _pos = pos;
        }

        public void GotOrder(Order o)
        {
            // track orders sent from this response
            if (o.symbol != Symbol) return;
            if (o.side && !_buyids.Contains(o.id))
                _buyids.Add(o.id);
            else if (!o.side && !_sellids.Contains(o.id))
                _sellids.Add(o.id);

            //pass through as an event
            if (gotOrder != null)
                gotOrder(o);

        }

        /// <summary>
        /// Reset this response instance.  (eg for another response run, a new trading day, etc)
        /// </summary>
        public virtual void Reset()
        {
            _syms = new string[0]; _shut = false; 
            DayStart = 930; DayEnd = 1600;
            _buyids.Clear();
            _sellids.Clear();
            _pos = new PositionImpl(Symbol);
            _bl = new BarListImpl();
        }


        private bool _sentshut = false;
        private bool OrdersAllowed { get { return (_multipleorders || (!_multipleorders && (_pos.Size == _expectedpossize))); } }

        /// <summary>
        /// Cancel all orders sent from this response
        /// </summary>
        protected void CancelOrders() { CancelOrders(true); CancelOrders(false); }
        /// <summary>
        /// Cancel all orders sent from a given side of this response.
        /// </summary>
        /// <param name="side">True to cancel long orders, false to cancel short orders</param>
        protected void CancelOrders(bool side)
        {
            if (SendCancel == null) return;
            if (side)
                foreach (uint id in _buyids.ToArray())
                    SendCancel(id);
            else
                foreach (uint id in _sellids.ToArray())
                    SendCancel(id);
        }
        /// <summary>
        /// Debugging facility for response.  Send text messages here to get picked up by other programs.
        /// </summary>
        /// <param name="debug">your debugging message</param>
        protected void D(string debug) 
        {
            if (SendDebug != null)
            {
                string head = "[" + Name + "] " + Symbol + " " + Date + ":" + Time + " ";
                SendDebug(DebugImpl.Create(head + debug, DebugLevel.Debug));
            }
        }
        /// <summary>
        /// Status facility for this response.  Send status messages to get picked up by other programs.
        /// </summary>
        /// <param name="status">your status message</param>
        protected void S(string status)
        {
            if (SendDebug != null)
            {
                string head = "[" + Name + "] " + Symbol + " " + Date + ":" + Time + " ";
                SendDebug(DebugImpl.Create(head + status, DebugLevel.Status));
            }

        }

        /// <summary>
        /// Override this function to subclass this response with bar/position/shutdown features.
        /// </summary>
        /// <param name="t">Most Recent Tick</param>
        /// <param name="bl">Current BarList</param>
        /// <returns>You return an order in response to latest Bar/Tick (or invalid order to do nothing)</returns>
        protected virtual Order ReadOrder(Tick t, BarList bl) { return new OrderImpl(); }


        /// <summary>
        /// Shutdowns the response so no more trades occur, with the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Shutdown(string message)
        {
            _shut = true;
            D(message);
        }

        /// <summary>
        /// Activates a response from shutdown state, with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Activate(string message)
        {
            _shut = false;
            D(message);
        }

        /// <summary>
        /// Gets or sets the day start, in 24hour time as an integer (eg 1415 = 2:15PM).
        /// </summary>
        /// <value>The day start.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Sets inclusive start time for response each day.")]
        public int DayStart { get { return _sday; } set { _sday = value; } }
        /// <summary>
        /// Gets or sets the day end, in 24hour time as an integer (eg 1600 = 4:00PM)
        /// </summary>
        /// <value>The day end.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Sets inclusive stop time for response each day.")]
        public int DayEnd { get { if ((_eday % 100) != 0) return _eday - _DayEndBuff; return _eday - 40 - _DayEndBuff; } set { _eday = value; } }
        /// <summary>
        /// Gets the current date.
        /// </summary>
        /// <value>The date.</value>
        [BrowsableAttribute(false)]
        public int Date { get { return _date; } }
        [BrowsableAttribute(false)]
        public int Sec { get { return _sec; } }
        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>The time.</value>
        [BrowsableAttribute(false)]
        public int Time { get { return _time; } }
        /// <summary>
        /// Gets or sets the size of the maximum position size allowed by this response (maxsize of a single position).
        /// </summary>
        /// <value>The size of the max.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Maximum size of a single position.")]
        public int MaxSize { get { return MAXSIZE; } set { MAXSIZE = value; } }
        /// <summary>
        /// Gets or sets the size of the minimum size of aposition allowed.
        /// </summary>
        /// <value>The size of the min.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Minimum size of a single position.")]
        public int MinSize { get { return MINSIZE; } set { MINSIZE = value; } }
        /// <summary>
        /// Gets or sets the full name of the response, as defined in the source code.
        /// </summary>
        /// <value>The full name.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Fully qualified response class name."), ReadOnlyAttribute(true)]
        public string FullName { get { return _fullname; } set { _fullname = value; } }
        /// <summary>
        /// Gets or sets the name of this response as described by the user.
        /// </summary>
        /// <value>The name.</value>
        [CategoryAttribute("TL DayTradeInfo"), DescriptionAttribute("Name of this response.")]
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="Response"/> is shutdown.
        /// </summary>
        /// <value><c>true</c> if off; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TL DayTradeInfo"), Description("True if the response is Shutdown.")]
        public bool Off { get { return _shut; } }
        [BrowsableAttribute(false)]
        public Position Pos { get { return _pos; } }
        [BrowsableAttribute(false)]
        protected BarListImpl BL { get { return _bl; } }
        [BrowsableAttribute(false)]
        public bool isValid { get { return !_shut; } }
        [BrowsableAttribute(false)]
        public virtual string[] Indicators { get { return _iname; } set { _iname = value; } }
        [BrowsableAttribute(false)]
        public string[] Symbols { get { return _syms; } }
        [BrowsableAttribute(false)]
        public string Symbol { get { return _syms.Length == 0 ? "" : _syms[0]; } }




    }
}
