using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.AppKit
{
    /// <summary>
    /// hold authentication information.
    /// </summary>
    public struct AuthInfo
    {
        /// <summary>
        /// username
        /// </summary>
        public string Username;
        /// <summary>
        /// password
        /// </summary>
        public string Password;
        /// <summary>
        /// isvalid auth information
        /// </summary>
        public bool isValid { get { return (Username != null) && (Password != null); } }
    }
}
