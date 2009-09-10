using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Bar
    {
        string Symbol { get; }
        ulong lHigh { get; }
        ulong lLow { get; }
        ulong lOpen { get; }
        ulong lClose { get; }
        decimal High { get; }
        decimal Low { get;  }
        decimal Open { get;  }
        decimal Close { get;  }
        long Volume { get;  }
        bool isNew { get; set;}
        int Bartime { get;  }
        int Bardate { get; }
        bool isValid { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BarInterval
    {
        /// <summary>
        /// custom volume bars
        /// </summary>
        CustomVol = -3,
        /// <summary>
        /// custom tick bars
        /// </summary>
        CustomTicks = -2,
        /// <summary>
        /// custom interval length
        /// </summary>
        CustomTime = -1,
        /// <summary>
        /// One-minute intervals
        /// </summary>
        Minute = 60,
        /// <summary>
        /// Five-minute interval
        /// </summary>
        FiveMin = 300,
        /// <summary>
        /// FifteenMinute intervals
        /// </summary>
        FifteenMin = 900,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        ThirtyMin = 1800,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        Hour = 3600,
        /// <summary>
        /// Day-long intervals
        /// </summary>
        Day = 86400,
    }
}
