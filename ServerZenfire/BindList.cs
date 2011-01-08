using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZenFireDev
{
    using Binding = KeyValuePair<String, Object>;
    
    public class Bind
    {
        static void Object<T>(T c)
        {
        
        }
        public static void List(System.Windows.Forms.ListControl control, List<Binding> src)
        {
            control.DisplayMember = "Key";
            control.ValueMember = "Value";
            control.DataSource = src;
        }
        public static void List(System.Windows.Forms.ListControl control, Object[] src)
        {
            control.DisplayMember = "Key";
            control.ValueMember = "Value";
            control.DataSource = Bind.FromArray(src);
        }

        static public List<Binding> FromEnum(System.Enum enu)
        {
            List<Binding> rv = new List<Binding>();


            Array vals = System.Enum.GetValues(enu.GetType());
            String[] keys = System.Enum.GetNames(enu.GetType());

            for (int i = 0; i < vals.Length; ++i)
                rv.Add(new Binding(keys[i], vals.GetValue(i)));
            return (rv);
        }

        static public List<Binding> FromArray(Object[] ar)
        {
            List<Binding> rv = new List<Binding>();


            foreach (Object obj in ar)
                rv.Add(new Binding(obj.ToString(), obj));
            return (rv);
        }
    }



}
