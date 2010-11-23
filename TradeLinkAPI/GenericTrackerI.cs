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
        /// clears all tracked values and labels
        /// </summary>
        void Clear();
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
        /// <summary>
        /// gets index associated with a given label, adding index if it doesn't exist
        /// (default value of index will be used)
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        int addindex(string txt);

        /// <summary>
        /// gets value of given index
        /// </summary>
        object Value(int idx);
        /// <summary>
        /// gets value of given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        object Value(string txt);
        /// <summary>
        /// gets type of tracked values
        /// </summary>
        Type TrackedType { get; }
        /// <summary>
        /// attempts to get decimal value of index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        decimal ValueDecimal(int idx);
        /// <summary>
        /// attempts to get decimal value of a label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        decimal ValueDecimal(string txt);

        /// <summary>
        /// called when new text label is added
        /// </summary>
        event TextIdxDelegate NewTxt;
    }

    public interface GenericTrackerDecimal
    {
        void setvalue(int idx, decimal v);
        decimal getvalue(int idx);
        decimal getvalue(string txt);
        int addindex(string txt, decimal v);
    }

    public interface GenericTrackerInt
    {
        void setvalue(int idx, int v);
        int getvalue(int idx);
        int getvalue(string txt);
        int addindex(string txt, int v);
    }

    public interface GenericTrackerBool
    {
        void setvalue(int idx, bool v);
        bool getvalue(int idx);
        bool getvalue(string txt);
        int addindex(string txt, bool v);
    }

    public interface GenericTrackerLong
    {
        void setvalue(int idx, long v);
        long getvalue(int idx);
        long getvalue(string txt);
        int addindex(string txt, long v);
    }

    public interface GenericTrackerString
    {
        void setvalue(int idx, string s);
        string getvalue(int idx);
        string getvalue(string txt);
        int addindex(string txt, string s);
    }
}
