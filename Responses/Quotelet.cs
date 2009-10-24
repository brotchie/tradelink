using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;

namespace Responses
{
    /// <summary>
    /// this is the GUI part of the response.
    /// it interacts with the user and passes data back and forth
    /// between the actual QuoteletResponse which does the work.
    /// The QuoteletResponse class is found at bottom of this file.
    /// </summary>
    public partial class Quotelet : Form
    {
        public Quotelet()
        {
            InitializeComponent();
            // show the form by default
            Show();
            // subscribe to initial symbol
            if (NewSymbol != null) NewSymbol(_sym.Text);
            // setup proper closing
            FormClosing += new FormClosingEventHandler(Quotelet_FormClosing);
        }

        void Quotelet_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if user closes form, clear our basket
            NewSymbol(string.Empty);
        }
        /// <summary>
        /// passes ticks from a response to the form GUI elements, so user
        /// can see the tick
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            // update the screen with the tick
            // if arriving from background thread,
            // windows makes us update GUI elements by 'invoking' 
            // from the GUI's thread.  so we test for this when performing the update.
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new TickDelegate(newTick), new object[] { k });
                }
                catch (ObjectDisposedException) { return; }
            }
            else
            {
                // make sure tick matches symbol we requested
                if (k.symbol != _sym.Text) return;
                // put data in the form
                if (k.hasAsk)
                    _ask.Text = k.ask.ToString("N2");
                if (k.hasBid)
                    _bid.Text = k.bid.ToString("N2");
                if (k.isTrade)
                    _last.Text = k.trade.ToString("N2");
                // refresh form's screen area
                Invalidate(true);
            }
        }
        /// <summary>
        /// passes position updates to the user
        /// </summary>
        /// <param name="p"></param>
        public void newPosition(Position p)
        {
            if (InvokeRequired)
                Invoke(new PositionDelegate(newPosition), new object[] { p });
            else
            {
                // ignore update if doesn't match symbol we're watching
                if ((p.Symbol != _sym.Text) || !p.isValid) return;
                // otherwise update our value
                _pos.Text = p.Size.ToString();
                // refresh this box's screen area
                _pos.Invalidate();
            }
        }
        public event OrderDelegate NewOrder;
        public event DebugDelegate NewSymbol;

        private void _buy_Click(object sender, EventArgs e)
        {
            // send order to response
            if (NewOrder != null)
                NewOrder(new BuyMarket(_sym.Text, 100));
        }

        private void _new_Click(object sender, EventArgs e)
        {
            // uppercase the symbol
            _sym.Text = _sym.Text.ToUpper();
            // clear the position
            _pos.Text = "0";
            _pos.Invalidate();
            // request the response to get this data for us
            if (NewSymbol != null) NewSymbol(_sym.Text);
        }

        private void _sell_Click(object sender, EventArgs e)
        {
            // send order to response
            if (NewOrder != null)
                NewOrder(new SellMarket(_sym.Text, 100));
        }


    }

    /// <summary>
    /// this is the quotelet response, which does the work.
    /// </summary>
    public class QuoteletResponse : ResponseTemplate
    {
        // the response creates a single quotelet window
        // this window will be created when the response is created
        Quotelet q = new Quotelet();
        public QuoteletResponse()
        {
            // here we bind events the form can create to appropriate
            // response activity
            q.NewOrder += new OrderDelegate(q_NewOrder);
            q.NewSymbol += new DebugDelegate(q_NewSymbol);
        }

        void q_NewSymbol(string symbol)
        {
            // make sure symbol is valid
            if (symbol.Length == 0) return;
            // request a basket with a single symbol specified by user
            sendbasket(new string[] { symbol });
            // provide any position information to user
            q.newPosition(_pt[symbol]);
        }

        void q_NewOrder(Order o)
        {
            // send order requested by the user
            sendorder(o);
            senddebug("sent order: " + o.ToString());
        }

        public override void GotTick(Tick tick)
        {
            // pass the tick to the form
            q.newTick(tick);
        }

        /// <summary>
        /// keeps track of positions
        /// </summary>
        PositionTracker _pt = new PositionTracker();

        public override void GotFill(Trade fill)
        {
            // make sure tracker has correct position
            _pt.Adjust(fill);
            // notify user
            q.newPosition(_pt[fill.symbol]);
        }

        public override void GotPosition(Position p)
        {
            // make sure tracker has correct position
            _pt.Adjust(p);
            // notify user
            q.newPosition(_pt[p.Symbol]);

        }
    }
}
