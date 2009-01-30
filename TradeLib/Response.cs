using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public interface Response
    {
        // Response input
        void GotTick(Tick tick);
        void GotOrder(Order order);
        void GotFill(Trade fill);
        void GotOrderCancel(uint orderid);
        void GotPosition(Position pos);

        // Response output
        event ObjectArrayDelegate SendIndicators;
        event OrderDelegate SendOrder;
        event UIntDelegate SendCancel;
        event DebugFullDelegate SendDebug;

        // response control
        void Reset();

        // Response Information
        bool isValid { get; }
        string Name { get; set;  }
        string FullName { get; set; }
        string[] Indicators { get; set; }
    }

    public static class ResponseLoader
    {

        /// <summary>
        /// Create a single Response from a DLL containing many Responses.  
        /// </summary>
        /// <param name="fullname">The fully-qualified Response Name (as in 'BoxExamples.Name').  </param>
        /// <param name="dllname">The path and filename of DLL.</param>
        /// <returns></returns>
        public static Response FromDLL(string fullname, string dllname)
        {
            System.Reflection.Assembly a;
            try
            {
                a = System.Reflection.Assembly.LoadFrom(dllname);
            }
            catch (Exception ex) { Response b = new InvalidResponse(); b.Name = ex.Message; return b; }
            return FromAssembly(a, fullname);
        }
        /// <summary>
        /// Create a single Response from an Assembly containing many Responses. 
        /// </summary>
        /// <param name="a">the assembly object</param>
        /// <param name="boxname">The fully-qualified Response Name (as in Response.FullName).</param>
        /// <returns></returns>
        public static Response FromAssembly(System.Reflection.Assembly a, string fullname)
        {
            Type type;
            object[] args;
            Response b = null;
            try
            {
                type = a.GetType(fullname, true, true);
            }
            catch (Exception ex) { b = new InvalidResponse(); b.Name = ex.Message; return b; }
            args = new object[] { };
            try
            {
                b = (Response)Activator.CreateInstance(type, args);
            }
            catch (Exception ex)
            {
                b = new InvalidResponse(); b.Name = ex.InnerException.Message; return b;
            }
            b.FullName = fullname;
            return b;
        }
    }

    public class InvalidResponse : Response
    {
        // Response input
        public void GotTick(Tick tick) { }
        public void GotOrder(Order order) { }
        public void GotFill(Trade fill) { }
        public void GotOrderCancel(uint orderid) { }
        public void GotPosition(Position pos) { }

        // Response output
        public event ObjectArrayDelegate SendIndicators;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event DebugFullDelegate SendDebug;

        // response control
        public void Reset() { }

        string _name = "ERROR";
        string _fullname = "";

        // Response Information
        public bool isValid { get { return false; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string FullName { get { return _fullname; } set { _fullname = value; } }
        public string[] Indicators { get { return new string[0]; } set { } }
    }
}
