using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;
using TicTacTec.TA.Library; // to use TA-lib indicators

namespace Responses
{
    public class SMAResponse : ResponseTemplate
    {
        // parameters of this system
        [Description("Total Profit Target")]
        public decimal TotalProfitTarget { get { return _totalprofit; } set { _totalprofit = value; } }
        [Description("Entry size when signal is found")]
        public int EntrySize { get { return _entrysize; } set { _entrysize= value; } }
        [Description("Default bar interval for this response.")]
        public BarInterval Interval { get { return _barinterval; } set { _barinterval = value; } }
        [Description("bars back when calculating sma")]
        public int BarsBack { get { return _barsback; } set { _barsback = value; } }
        [Description("shutdown time")]
        public int Shutdown { get { return _shutdowntime; } set { _shutdowntime = value; } }

        bool _black = false;
        // this function is called the constructor, because it sets up the response
        // it is run before all the other functions, and has same name as my response.
        public SMAResponse() : this(true) { }
        public SMAResponse(bool prompt)
        {
            _black = !prompt;
            // handle when new symbols are added to the active tracker
            _active.NewTxt += new TextIdxDelegate(_active_NewTxt);

            // set our indicator names, in case we import indicators into R
            // or excel, or we want to view them in gauntlet or kadina
            Indicators = new string[] { "Time","SMA" };
        }

        public override void Reset()
        {
            // enable prompting of system parameters to user,
            // so they do not have to recompile to change things
            ParamPrompt.Popup(this, true,_black);

            // only build bars for user's interval
            blt = new BarListTracker(Interval);

            // only calculate on new bars
            blt.GotNewBar += new SymBarIntervalDelegate(blt_GotNewBar);
        }


        // wait for fill
        GenericTracker<bool> _wait = new GenericTracker<bool>();
        // track whether shutdown 
        GenericTracker<bool> _active = new GenericTracker<bool>();
        // hold last ma
        GenericTracker<decimal> _sma = new GenericTracker<decimal>();

        void _active_NewTxt(string txt, int idx)
        {
            // go ahead and notify any other trackers about this symbol
            _wait.addindex(txt, false);
            _sma.addindex(txt, 0);
        }

        void blt_GotNewBar(string symbol, int interval)
        {
            // lets do our entries.  
            
            int idx = _active.getindex(symbol);

            // calculate the SMA using closign prices for so many bars back
            decimal SMA = Calc.Avg(Calc.EndSlice(blt[symbol].Close(),_barsback));
            // uncomment to use TA-lib indicators
            //int num,si;
            //double[] result = new double[blt[symbol].Count];
            //Core.Sma(0, blt[symbol].Last,
            //    Calc.Decimal2Double(blt[symbol].Close()), 
            //    _barsback,out si, out num, result);
            //Calc.TAPopulateGT(idx, num, ref result, _sma);
            //decimal SMA = _sma[idx];

            // wait until we have an SMA
            if (SMA == 0)
                return;

            //ensure we aren't waiting for previous order to fill
            if (!_wait[symbol])
            {

                // if we're flat and not waiting
                if (pt[symbol].isFlat)
                {
                    // if our current price is above SMA, buy
                    if (blt[symbol].RecentBar.Close > SMA)
                    {
                        D("crosses above MA, buy");
                        sendorder(new BuyMarket(symbol, EntrySize));
                        // wait for fill
                        _wait[symbol] = true;
                    }
                    // otherwise if it's less than SMA, sell
                    if (blt[symbol].RecentBar.Close < SMA)
                    {
                        D("crosses below MA, sell");
                        sendorder(new SellMarket(symbol, EntrySize));
                        // wait for fill
                        _wait[symbol] = true;
                    }
                }
                else if ((pt[symbol].isLong && (blt[symbol].RecentBar.Close < SMA))
                    || (pt[symbol].isShort && (blt[symbol].RecentBar.Close > SMA)))
                {
                    D("counter trend, exit.");
                    sendorder(new MarketOrderFlat(pt[symbol]));
                    // wait for fill
                    _wait[symbol] = true;
                }
            }



            // this way we can debug our indicators during development
            // indicators are sent in the same order as they are named above
            sendindicators(new string[] { time.ToString(),SMA.ToString("N2")});

            // draw the MA as a line
            sendchartlabel(SMA, time);
        }

        // turn on bar tracking
        BarListTracker blt = new BarListTracker();
        // turn on position tracking
        PositionTracker pt = new PositionTracker();

        // keep track of time for use in other functions
        int time = 0;

        // got tick is called whenever this strategy receives a tick
        public override void GotTick(Tick tick)
        {
            // keep track of time from tick
            time = tick.time;
            // ensure response is active
            if (!isValid) return;
            // ensure we are tracking active status for this symbol
            int idx = _active.addindex(tick.symbol, true);
            // if we're not active, quit
            if (!_active[idx]) return;
            // check for shutdown time
            if (tick.time > Shutdown)
            {
                // if so shutdown
                shutdown();
                // and quit
                return;
            }
            // apply bar tracking to all ticks that enter
            blt.newTick(tick);

            // ignore anything that is not a trade
            if (!tick.isTrade) return;

            // if we made it here, we have enough bars and we have a trade.

            // exits are processed first, lets see if we have our total profit
            if (Calc.OpenPL(tick.trade, pt[tick.symbol]) + pt[tick.symbol].ClosedPL > TotalProfitTarget)
            {
                // if we hit our target, shutdown trading on this symbol
                shutdown(tick.symbol);
                // don't process anything else after this (entries, etc)
                return;
            }
            
        }

        void shutdown()
        {
            D("shutting down everything");
            foreach (Position p in pt)
                sendorder(new MarketOrderFlat(p));
            isValid = false;
        }

        void shutdown(string sym)
        {
            // notify
            D("shutting down " + sym);
            // send flat order
            sendorder(new MarketOrderFlat(pt[sym]));
            // set inactive
            _active[sym] = false;
        }

        public override void GotFill(Trade fill)
        {
            // make sure every fill is tracked against a position
            pt.Adjust(fill);
            // get index for this symbol
            int idx = _wait.getindex(fill.symbol);
            // ignore unknown symbols
            if (idx < 0) return;
            // stop waiting
            _wait[fill.symbol] = false;
            // chart fills
            sendchartlabel(fill.xprice,time,TradeImpl.ToChartLabel(fill), fill.side ? System.Drawing.Color.Green : System.Drawing.Color.Red);
        }

        public override void GotPosition(Position p)
        {
            // make sure every position set at strategy startup is tracked
            pt.Adjust(p);
        }

        // these variables "hold" the parameters set by the user above
        // also they are the defaults that show up first
        int _barsback = 2;
        BarInterval _barinterval = BarInterval.FiveMin;
        int _entrysize = 100;
        decimal _totalprofit = 200;
        int _shutdowntime = 155000;
    }

    /// <summary>
    /// this is the same as SMAResponse, except it runs without prompting user
    /// </summary>
    public class SMAResponseAuto : SMAResponse
    {
        public SMAResponseAuto() : base(false) { }
    }



#warning If you get errors about missing references to TradeLink.Common or TradeLink.Api, choose Project->Add Reference->Browse to folder where you installed tradelink (usually Program Files) and add a reference for each dll.
}
