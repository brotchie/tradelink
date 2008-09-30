using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // base template class for issue #32
    public class aBox
    {
        public event TickDelegate GotTick;
        public event OrderDelegate GotOrder;
        public event FillDelegate GotFill;
        public event UIntDelegate GotOrderCancel;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event DebugFullDelegate SendDebug;

        public string Name { get { return _name; } }
        public string FullName { get { return _fullname; } set { _fullname = value; } }
        public string Symbol { get { return _sym; } }
        public bool isOn { get { return _ison; } }
        public bool isValid { get { return _name != BLANKBOX; } }
        protected Position Pos { get { return _pos; } set { _pos = value; } }
        public virtual void D(string debug) 
        {
            string Date = _dt != DateTime.MinValue ? _dt.ToString() : "";
            if (SendDebug != null) 
                SendDebug(Debug.Create("[" + Name + "] " + Symbol + " " + Date + debug, DebugLevel.Debug)); 
        }

        private DateTime _dt = DateTime.MinValue;
        private Position _pos = null;
        private bool _ison = true;
        protected string[] _iname;
        protected object[] _indicators;
        const string BLANKBOX = "Blank Box Template";
        string _name = BLANKBOX;
        string _fullname = "";
        string _sym = "";

        /// <summary>
        /// Create a box from a DLL containing Box classes.  
        /// </summary>
        /// <param name="boxname">The fully-qualified boxname (as in Box.FullName).</param>
        /// <param name="dllname">The dllname.</param>
        /// <param name="ns">The NewsService this box will use.</param>
        /// <returns></returns>
        public static Box FromDLL(string boxname, string dllname)
        {
            System.Reflection.Assembly a;
            try
            {
                a = System.Reflection.Assembly.LoadFrom(dllname);
            }
            catch (Exception ex) { Box b = new Box(); b.Name = ex.Message; return b; }
            return FromAssembly(a, boxname);
        }
        /// <summary>
        /// Create a box from an Assembly containing Box classes.
        /// </summary>
        /// <param name="a">the assembly.</param>
        /// <param name="boxname">The fully-qualified boxname.</param>
        /// <param name="ns">The NewsService where this box will send debugs and errors.</param>
        /// <returns></returns>
        public static Box FromAssembly(System.Reflection.Assembly a, string boxname)
        {
            Type type;
            object[] args;
            Box b = new Box();
            try
            {
                type = a.GetType(boxname, true, true);
            }
            catch (Exception ex) { b = new Box(); b.Name = ex.Message; return b; }
            args = new object[] { };
            try
            {
                b = (Box)Activator.CreateInstance(type, args);
            }
            catch (Exception ex)
            {
                b = new Box(); b.Name = ex.InnerException.Message; return b;
            }
            b.FullName = boxname;
            return b;
        }

        
        
        

    }
}
