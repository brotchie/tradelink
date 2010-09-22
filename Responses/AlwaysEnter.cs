using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;

namespace Responses
{
    /// <summary>
    /// This response will always enter long on every tick, unless he's already in a position.
    /// 
    /// If he's in a position, he will exit at pre-defined stop and loss targets.  (then get back in)
    /// 
    /// Used for testing applications that use responsees.  (Quotopia, Gauntlet, etc)
    /// </summary>
    public class AlwaysEnter : ResponseTemplate
    {
        public AlwaysEnter() : base() 
        {
            Name = "AlwaysEnter";
            _entrysignal.NewTxt += new TextIdxDelegate(_entrysignal_NewTxt);
        }


        public override void GotTick(TradeLink.API.Tick tick)
        {
            // ensure we track this symbol
            _entrysignal.addindex(tick.symbol,false);
            // track tick
            _kt.newTick(tick);
            // get current position
            Position p = _pt[tick.symbol];

            // if we're flat and no signal, enter
            if (p.isFlat && !_entrysignal[tick.symbol])
            {
                _entrysignal[tick.symbol] = true;
                D(tick.symbol+": entering long");
                O(new MarketOrder(tick.symbol, EntrySize));
            }
            else if (!p.isFlat && !_exitsignal[tick.symbol])
            {
                // get most recent tick data
                Tick k = _kt[tick.symbol];
                // estimate our exit price
                decimal exitprice = UseQuotes 
                    ? (k.hasAsk && p.isLong ? k.ask 
                    : (k.hasBid && p.isShort ? k.bid : 0))
                    : (k.isTrade ? k.trade : 0);
                // assuming we could estimate an exit, see if our exit would hit our target
                if ((exitprice != 0) && (Calc.OpenPT(exitprice, p) > ProfitTarget))
                {
                    _exitsignal[tick.symbol] = true;
                    D("hit profit target");
                    O(new MarketOrderFlat(p));
                }
                
                
            }
        }

        PositionTracker _pt = new PositionTracker();
        TickTracker _kt = new TickTracker();
        GenericTracker<bool> _entrysignal = new GenericTracker<bool>();
        GenericTracker<bool> _exitsignal = new GenericTracker<bool>();

        // link all the generic trackers together so we create 
        // proper default values for each whenever we add symbol to one
        void _entrysignal_NewTxt(string txt, int idx)
        {
            _exitsignal.addindex(txt, false);
        }


        public override void GotFill(TradeLink.API.Trade fill)
        {
            // keep track of position
            D(fill.symbol + " fill: " + fill.ToString());
            _pt.Adjust(fill);
            // ensure fill comes from this response
            int idx = _entrysignal.getindex(fill.symbol);
            if (idx < 0) return;
            // reset signals if we're flat (allows re-entry)
            if (_pt[fill.symbol].isFlat)
            {
                _entrysignal[fill.symbol] = false;
                _exitsignal[fill.symbol] = false;
            }
        }

        public override void GotPosition(TradeLink.API.Position p)
        {
            // keep track of position
            D(p.Symbol + " pos: " + p.ToString());
            _pt.Adjust(p);
        }

        int _entrysize = 100;
        [Description("size used when entering positions.  Negative numbers would be short entries.")]
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }
        decimal _profittarget = .1m;
        [Description("profit target in dollars when position is exited")]
        public decimal ProfitTarget { get { return _profittarget; } set { _profittarget = value; } }
        bool _usequotes = false;
        [Description("whether bid/ask is used to determine profitability, otherwise last trade is used")]
        public bool UseQuotes { get { return _usequotes; } set { _usequotes = value; } }

    }

    /// <summary>
    /// allows user to control alwaysenter parameters through graphical pop-up interface
    /// </summary>
    public class AlwaysEnter_Parameters : AlwaysEnter
    {
        public override void Reset()
        {
            ParamPrompt.Popup(this, true, false);
            base.Reset();
        }
    }
}
