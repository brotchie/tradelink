using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // base template class for issue #32
    public interface Response
    {
        event TickDelegate GotTick;
        event OrderDelegate GotOrder;
        event FillDelegate GotFill;
        event UIntDelegate GotOrderCancel;
        event OrderDelegate SendOrder;
        event UIntDelegate SendCancel;
        event DebugFullDelegate SendDebug;
    }
}
