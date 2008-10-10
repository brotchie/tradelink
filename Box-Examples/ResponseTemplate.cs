using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace BoxExamples
{
    // A response is the most generic type of box you can have
    // responses will work in all the TradeLink programs (gauntlet/asp/kadina)
    public class ResponseTemplate : Response
    {
        /// <summary>
        /// Called when new ticks are recieved
        /// </summary>
        /// <param name="tick"></param>
        public void GotTick(Tick tick)
        {
            // here is where you respond to ticks, eg to populate a barlist

            // this.MyBarList.newTick(tick);
        }
        /// <summary>
        /// Called when new orders received
        /// </summary>
        /// <param name="order"></param>
        public void GotOrder(Order order)
        {
            // track or respond to orders here, eg:
            //
            // this.MyOrders.Add(order);
        }
        /// <summary>
        /// Called when orders are filled
        /// </summary>
        /// <param name="fill"></param>
        public void GotFill(Trade fill)
        {
            // track or respond to trades here

            // eg create a position field in this class, 
            // then adjust it with latest trade:
            //
            // this.MyPosition.Adjust(fill)
        }
        /// <summary>
        /// Called if a cancel has been processed
        /// </summary>
        /// <param name="cancelid"></param>
        public void GotOrderCancel(uint cancelid)
        {
            // order cancels dealt with here, eg:
            // int idx = -1;
            // for (int i = 0; i<this.MyOrders.Cout; i++)
            //      if (this.MyOrders[i].id==cancelid) idx = i;
            // if (idx!=-1) this.MyOrders.RemoveAt(idx);
        }
        /// <summary>
        /// Call this to reset your box parameters
        /// </summary>
        public void Reset()
        {
            // here is where you can reset your own values between runs
            // eg indicators and so forth at days end or between runs, etc:

            // MovingAverage.Reset()
        }

        public void GotPosition(Position p) { }

        string[] _inds = new string[0];
        string _name = "";
        string _full = "";

        /// <summary>
        /// Whether response can be used or not
        /// </summary>
        public bool isValid { get { return true; } }
        /// <summary>
        /// Names of the indicators used by your response.
        /// Length must correspond to actual indicator values send with SendIndicators event
        /// </summary>
        public string[] Indicators { get { return _inds; } set { _inds = value; } }
        /// <summary>
        /// Custom name of box set by you
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// Full name of this box set by programs (includes namespace)
        /// </summary>
        public string FullName { get { return _full; } set { _full = value; } }

        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event ObjectArrayDelegate SendIndicators;
    }
}
