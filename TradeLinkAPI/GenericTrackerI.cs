using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeLink.API
{
/// <summary>
    /// generic interface that can be used with any type of tracker
    /// </summary>
    public interface GenericTrackerI
    {
        /// <summary>
        /// name of tracker
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// display value of a tracked value for a given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        string Display(string txt);
        /// <summary>
        /// display tracked value for a given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        string Display(int idx);
        /// <summary>
        /// get label associated with an index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        string getlabel(int idx);
        /// <summary>
        /// get index associated with a given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        int getindex(string txt);
        /// <summary>
        /// get total number of labels/values
        /// </summary>
        int Count { get; }
    }
}
