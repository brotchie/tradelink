using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Bar
    {
        string Symbol { get; }
        decimal High { get; }
        decimal Low { get;  }
        decimal Open { get;  }
        decimal Close { get;  }
        int Volume { get;  }
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
        /// One-minute intervals
        /// </summary>
        Minute = 1,
        /// <summary>
        /// Five-minute interval
        /// </summary>
        FiveMin = 5,
        /// <summary>
        /// FifteenMinute intervals
        /// </summary>
        FifteenMin = 15,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        ThirtyMin = 30,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        Hour = 60,
        /// <summary>
        /// Day-long intervals
        /// </summary>
        Day = 450
    }
}
