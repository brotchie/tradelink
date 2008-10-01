using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TradeLib
{
    public class StandardBox : Response
    {
        public event TickDelegate GotTick;
        public event FillDelegate GotFill;
        public event UIntDelegate SendCancel;
        public event OrderDelegate SendOrder;
        public event DebugFullDelegate SendDebug;
        public event UIntDelegate GotOrderCancel;
        public event OrderDelegate GotOrder;

        BarList _bl = new BarList();
        Position _pos = new Position();
        public string Symbol = null;
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
        private List<uint> _buyids = new List<uint>();
        private List<uint> _sellids = new List<uint>();


        public StandardBox()
        {
            GotTick += new TickDelegate(StandardBox_GotTick);
            GotFill += new FillDelegate(StandardBox_GotFill);
            GotOrder += new OrderDelegate(StandardBox_GotOrder);
            GotOrderCancel += new UIntDelegate(StandardBox_GotOrderCancel);
        }

        void StandardBox_GotTick(Tick tick)
        {
            _bl.newTick(tick);
            Order o = new Order();
            if (Symbol == null)
            {
                if (tick.sym != "") Symbol = tick.sym;
                else return;
                _pos = new Position(tick.sym);
            }
            if (!_pos.isValid)
            {
                D("Invalid position provided: " + _pos.ToString());
                return;
            }
            if (tick.sym != Symbol) return;
            _time = tick.time;
            _date = tick.date;
            _sec = tick.sec;

            if ((Time < DayStart) || (Time > DayEnd)) return; // is market open?
            if (Off) return; // don't trade if shutdown


            o = ReadOrder(tick, _bl);

            if (!OrdersAllowed) // if we're not allowed, mark order as invalid
                o = new Order();

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
                _expectedpossize += o.SignedSize;
                o.time = Time;
                o.date = Date;
            }

            if (o.isValid)
            {
                D("Sent order: " + o);
                if (SendOrder != null)
                    SendOrder(o); // send our order
                else D("No route for order. Dropped.");
            }

        }

        void StandardBox_GotOrderCancel(uint cancelid)
        {
            if (_buyids.Contains(cancelid))
                _buyids.Remove(cancelid);
            if (_sellids.Contains(cancelid))
                _sellids.Remove(cancelid);
        }

        void StandardBox_GotFill(Trade t)
        {
            _pos.Adjust(t);
        }

        void StandardBox_GotOrder(Order o)
        {
            if (o.symbol != Symbol) return;
            if (o.side && !_buyids.Contains(o.id))
                _buyids.Add(o.id);
            else if (!o.side && !_sellids.Contains(o.id))
                _sellids.Add(o.id);

        }


        private bool _sentshut = false;
        private bool OrdersAllowed { get { return (_multipleorders || (!_multipleorders && (_pos.Size == _expectedpossize))); } }

        /// <summary>
        /// Cancel all orders sent from this box
        /// </summary>
        protected void CancelOrders() { CancelOrders(true); CancelOrders(false); }
        /// <summary>
        /// Cancel all orders sent from a given side of this box.
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
        /// Debugging facility for box.  Send text messages here to get picked up by other programs.
        /// </summary>
        /// <param name="debug">your debugging message</param>
        void D(string debug) 
        {
            if (SendDebug != null)
            {
                string head = "[" + Name + "] " + Symbol + " " + Date + ":" + Time + " ";
                SendDebug(Debug.Create(head + debug, DebugLevel.Debug));
            }
        }
        /// <summary>
        /// Status facility for this box.  Send status messages to get picked up by other programs.
        /// </summary>
        /// <param name="status">your status message</param>
        void S(string status)
        {
            if (SendDebug != null)
            {
                string head = "[" + Name + "] " + Symbol + " " + Date + ":" + Time + " ";
                SendDebug(Debug.Create(head + status, DebugLevel.Status));
            }

        }

        /// <summary>
        /// Override this function to subclass this box with bar/position/shutdown features.
        /// </summary>
        /// <param name="t">Most Recent Tick</param>
        /// <param name="bl">Current BarList</param>
        /// <returns>You return an order in response to latest Bar/Tick (or invalid order to do nothing)</returns>
        protected virtual Order ReadOrder(Tick t, BarList bl) { return new Order(); }


        /// <summary>
        /// Shutdowns the box so no more trades occur, with the specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Shutdown(string message)
        {
            _shut = true;
            D(message);
        }

        /// <summary>
        /// Activates a box from shutdown state, with the specified message.
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
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Sets inclusive start time for box each day.")]
        public int DayStart { get { return _sday; } set { _sday = value; } }
        /// <summary>
        /// Gets or sets the day end, in 24hour time as an integer (eg 1600 = 4:00PM)
        /// </summary>
        /// <value>The day end.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Sets inclusive stop time for box each day.")]
        public int DayEnd { get { if ((_eday % 100) != 0) return _eday - _DayEndBuff; return _eday - 40 - _DayEndBuff; } set { _eday = value; } }
        /// <summary>
        /// Gets the current date.
        /// </summary>
        /// <value>The date.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Date")]
        public int Date { get { return _date; } }
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Sec")]
        public int Sec { get { return _sec; } }
        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>The time.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Time")]
        public int Time { get { return _time; } }
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
        /// Gets or sets the full name of the box, as defined in the source code.
        /// </summary>
        /// <value>The full name.</value>
        [CategoryAttribute("TradeLink BoxInfo"), DescriptionAttribute("Fully qualified box class name."), ReadOnlyAttribute(true)]
        public string FullName { get { return _fullname; } set { _fullname = value; } }
        /// <summary>
        /// Gets or sets the name of this box as described by the user.
        /// </summary>
        /// <value>The name.</value>
        [CategoryAttribute("TradeLink Box"), DescriptionAttribute("Name of this box.")]
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="Box"/> is shutdown.
        /// </summary>
        /// <value><c>true</c> if off; otherwise, <c>false</c>.</value>
        [CategoryAttribute("TradeLink BoxInfo"), Description("True if the box is Shutdown.")]
        public bool Off { get { return _shut; } }
        [BrowsableAttribute(false)]
        public Position Pos { get { return _pos; } }
        public BarList BL { get { return _bl; } }


    }
}
