#pragma once

namespace TradeLibFast
{
	enum OrderInstructionType
    {
        Invalid = -2,
        /// <summary>
        /// day order
        /// </summary>
        DAY = 0,
        /// <summary>
        /// market on close
        /// </summary>
        MOC = 2,
        /// <summary>
        /// opening order
        /// </summary>
        OPG = 4,
        /// <summary>
        /// immediate or cancel
        /// </summary>
        IOC = 8,
        /// <summary>
        /// good till canceled
        /// </summary>
        GTC = 16,
        /// <summary>
        /// pegged to mid-market
        /// </summary>
        PEG2MID = 32,
        /// <summary>
        /// pegged to market
        /// </summary>
        PEG2MKT = 64,
        /// <summary>
        /// pegged to primary
        /// </summary>
        PEG2PRI = 128,
        /// <summary>
        /// pegged to best
        /// </summary>
        PEG2BST = 256,
    };
}