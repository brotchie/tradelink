using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.Common;
using TWSLib;

namespace ServerIB
{
    public class ServerIB : TLServer_WM
    {
        TWSLib.TwsClass tws1 = new TwsClass();
        public ServerIB()
        {
            tws1.nextValidId += new _DTwsEvents_nextValidIdEventHandler(tws1_nextValidId);
        }

        void tws1_nextValidId(int id)
        {
            
        }
    }
}
