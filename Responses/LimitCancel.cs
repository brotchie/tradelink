using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;

namespace Responses
{
    public class LimitCancel : ResponseTemplate
    {
        TickTracker _kt = new TickTracker();
        GenericTracker<long> _orderid = new GenericTracker<long>();
        GenericTracker<long> _tickcounter = new GenericTracker<long>();
        IdTracker _idt;
        bool _prompt = true;
        public LimitCancel() : this(true) { }
        public LimitCancel(bool prompt)
        {
            _prompt = prompt;
            _orderid.NewTxt += new TextIdxDelegate(_orderid_NewTxt);
        }

        void _orderid_NewTxt(string txt, int idx)
        {
            _kt.addindex(txt);
            _tickcounter.addindex(txt, 0);
        }

        public override void GotTick(Tick tick)
        {
            // track order ids
            _orderid.addindex(tick.symbol,0);
            // track ticks
            _kt.newTick(tick);
            // see if we need to send an order
            if (_orderid[tick.symbol] == 0)
            {
                // get complete last tick for this symbol
                Tick k = _kt[tick.symbol];
                // see if we have proper info to place order
                if ((Side && k.hasBid) || (!Side && k.hasAsk))
                {
                    _orderid[tick.symbol] = _idt.NextId;
                    D(tick.symbol + " sending limit order: " + _orderid[tick.symbol]);
                    sendorder(new LimitOrder(tick.symbol, Side, Ordersize, Side ? k.bid - Distance : k.ask + Distance, _idt.AssignId));
                }
            }
            else // otherwise increment counter
                _tickcounter[tick.symbol]++;
            // see if we need to cancel
            if (_tickcounter[tick.symbol] > Frequency)
            {
                D(tick.symbol + " hit frequency, canceling: " + _orderid[tick.symbol]);
                sendcancel(_orderid[tick.symbol]);
            }
        }

        public override void GotFill(Trade fill)
        {
            D(fill.symbol + " this strategy does not handle fills.");
            base.GotFill(fill);
        }

        public override void GotOrderCancel(long cancelid)
        {
            // if cancel matches an order we sent, reset the order so it can be resent
            for (int i = 0; i < _orderid.Count; i++)
                if (_orderid[i] == cancelid)
                {
                    _orderid[i] = 0;
                    _tickcounter[i] = 0;
                }
        }

        public override void Reset()
        {
            // we do this in reset so we can pickup the response id and keep our orders unique
            _idt = new IdTracker(ID);
            // prompt for parameters if specified
            ParamPrompt.Popup(this, true, !_prompt);
        }

        bool _side = true;
        [Description("true for buy, false for sell")]
        public bool Side { get { return _side; } set { _side = value; } }
        decimal _distance = .5m;
        [Description("order sent this many dollars below/above bid/ask on buy/sell")]
        public decimal Distance { get { return _distance; } set { _distance = value; } }
        int _frequency = 15;
        [Description("order resent every X ticks")]
        public int Frequency { get { return _frequency; } set { _frequency = value; } }
        int _ordersize = 100;
        [Description("size of order sent")]
        public int Ordersize { get { return _ordersize; } set { _ordersize = value; } }

    }

    public class LimitCancel_Defaults : LimitCancel
    {
        public LimitCancel_Defaults() : base(false) { }
    }
}
