using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    /// <summary>
    /// security definition
    /// </summary>
    public interface Security
    {
        /// <summary>
        /// symbol name
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// symbol with underscores represented as spaces
        /// </summary>
        string Symbol_Spaces { get; }
        /// <summary>
        /// entire representation of security
        /// </summary>
        string FullName { get;  }
        /// <summary>
        /// exchange associated with security
        /// </summary>
        string DestEx { get; set; }
        /// <summary>
        /// type associated with security
        /// </summary>
        SecurityType Type { get; set; }
        /// <summary>
        /// whether security is valid
        /// </summary>
        bool isValid { get; }
        /// <summary>
        /// whether security has an exchange
        /// </summary>
        bool hasDest { get; }
        /// <summary>
        /// whether security has an explicit type
        /// </summary>
        bool hasType { get; }
        /// <summary>
        /// date associated with security (eg expiration date)
        /// </summary>
        int Date { get; set; }
        /// <summary>
        /// details associated with security (eg put/call for options)
        /// </summary>
        string Details { get; set; }
        /// <summary>
        /// whether security is a call
        /// </summary>
        bool isCall { get; }
        /// <summary>
        /// whether security is put
        /// </summary>
        bool isPut { get; }
        /// <summary>
        /// strike price of options securities
        /// </summary>
        decimal Strike { get; set; }
    }

    public class InvalidSecurity : Exception { }
}
