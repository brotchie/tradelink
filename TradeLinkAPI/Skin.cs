using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
    public interface Skin
    {
        /// <summary>
        /// xml encoded properties of response and their values
        /// </summary>
        string Properties { get; set; }
        /// <summary>
        /// fullname (namespace.classname) of response in question
        /// </summary>
        string ResponseName { get; set; }
        /// <summary>
        /// full path name to local dll containing classes.
        /// </summary>
        string ResponseDLL { get; set; }
        /// <summary>
        /// identifies where skin is valid
        /// </summary>
        bool isValid { get; }
    }
}
