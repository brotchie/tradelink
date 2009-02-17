using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLink.API
{
    public interface Chart
    {
        BarList Bars { get; set; }
        string Title { get;  }
        void NewBarList(BarList barlist);
        string Symbol { get; set; }
        void Show();
        void Close();
    }
}
