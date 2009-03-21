using System;
using System.Collections.Generic;
using TradeLink.API;
using System.Text;

namespace TradeLink.Common
{
    public class InvalidResponse : ResponseTemplate
    {

        // Response Information
        public new bool isValid { get { return false; } set { } }
    }
}
