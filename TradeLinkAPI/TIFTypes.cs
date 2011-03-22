using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// list of accepted tradelink TIF values
    /// </summary>
    public enum TIFTypes
    {
        Invalid = -1,
        /// <summary>
        /// day order
        /// </summary>
        DAY,
        /// <summary>
        /// market on close
        /// </summary>
        MOC,
        /// <summary>
        /// opening order
        /// </summary>
        OPG,
        /// <summary>
        /// immediate or cancel
        /// </summary>
        IOC,
        /// <summary>
        /// good till canceled
        /// </summary>
        GTC
    }
}
