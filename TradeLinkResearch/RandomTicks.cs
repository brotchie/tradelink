using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Research
{
    /// <summary>
    /// create an array of ticks that is a random walk from an initial set of ticks.
    /// walk varies between +MaxMoveCents and -MaxMoveCents.
    /// at present no quotes are generated, only trades.
    /// </summary>
    public class RandomTicks
    {
        /// <summary>
        /// create random ticks for desired number of symbols
        /// </summary>
        /// <param name="symcount"></param>
        public RandomTicks(int symcount) : this(RandomSymbol.GetSymbols((int)DateTime.Now.Ticks, 4, symcount)) { }
        Tick[][] _feed = new Tick[0][];
        string[] _syms = new string[0];
        decimal[] _iprice = new decimal[0];
        /// <summary>
        /// obtain list of symbols
        /// </summary>
        public string[] Symbols { get { return _syms; } }
        /// <summary>
        /// obtain randomized ticks.  each 1st dimension array corresponds to Symbol in same-position of this.Symbols[]
        /// Ticks are listed sequentionally in the 2nd dimension.
        /// </summary>
        public Tick[][] Ticks { get { return _feed; } }
        /// <summary>
        /// gets desired number of random [initial] prices.
        /// </summary>
        /// <param name="pricecount"></param>
        /// <returns></returns>
        public static decimal[] RandomPrices(int pricecount) { return RandomPrices(pricecount,(int)DateTime.Now.Ticks); }
        /// <summary>
        /// provides a group of random prices
        /// </summary>
        /// <param name="pricecout"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static decimal[] RandomPrices(int pricecount, int seed)
        {
            decimal[] p = new decimal[pricecount];
            Random r = new Random(seed);
            for (int i = 0; i<p.Length; i++)
                p[i] = (decimal)r.NextDouble()*99+1;
            return p;
        }
        /// <summary>
        /// creates ticks from list of symbols and a seed value
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="seed"></param>
        public RandomTicks(string[] symbols, int seed) : this(symbols, RandomPrices(symbols.Length,seed), seed) { }
        /// <summary>
        /// creates random ticks from a list of symbols, with randomized initial prices
        /// </summary>
        /// <param name="symbols"></param>
        public RandomTicks(string [] symbols) : this(symbols,RandomPrices(symbols.Length),(int)DateTime.Now.Ticks) {}
        /// <summary>
        /// creates random ticks for a list of symbols and starting prices.
        /// prices should be in same order for symbol they represent.
        /// </summary>
        /// <param name="symbols">list of symbols</param>
        /// <param name="startingprices">opening trade for each symbol</param>
        public RandomTicks(string[] symbols, decimal[] startingprices, int seed)
        {
            // save symbol list
            _syms = symbols;
            // save initial prices
            _iprice = startingprices;
            // init generator
            r = new Random(seed);
        }
        int _maxmove = 3;
        int _volpertrade = 100;
        /// <summary>
        /// random walk varies between +MaxMoveCents and -MaxMoveCents
        /// </summary>
        public int MaxMoveCents { get { return _maxmove; } set { _maxmove = Math.Abs(value); } }
        /// <summary>
        /// volume to use on each tick
        /// </summary>
        public int VolPerTrade { get { return _volpertrade; } set { _volpertrade = value; } }
        Random r = null;
        /// <summary>
        /// generate Ticks per symbol using a random walk from initial prices
        /// </summary>
        /// <param name="Ticks"></param>
        public void Generate(int Ticks) { Generate(Ticks,(int)DateTime.Now.Ticks); }
        public void Generate(int Ticks, int Seed)
        {
            _feed = new Tick[_syms.Length][];

            // for each symbol
            for (int i = 0; i<_syms.Length; i++)
            {
                // generate a list of requested ticks
                _feed[i] = new Tick[Ticks];
                for (int j = 0; j<Ticks; j++)
                {
                    // by taking the initial price and moving it some amount between min and max move
                    _iprice[i] += (decimal)r.Next(_maxmove*-1,_maxmove+1)/100;
                    // make sure it's still positive
                    if (_iprice[i] < 0) _iprice[i] = 0;
                    // then store this result as a tick and continue
                    _feed[i][j] = TickImpl.NewTrade(_syms[i], _iprice[i], VolPerTrade);
                }
            }
        }
        /// <summary>
        /// generate random ticks for single symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="Ticks"></param>
        /// <returns></returns>
        public static Tick[] GenerateSymbol(string sym, int Ticks)
        {
            RandomTicks rt = new RandomTicks(new string[] { sym });
            rt.Generate(Ticks);
            return rt.Ticks[0];
        }

    }
}
