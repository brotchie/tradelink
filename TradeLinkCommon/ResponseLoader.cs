using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// used for loading responses from disk (via DLL) or from already loaded assemblies.
    /// </summary>
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
            try
            {
                int partial = fullname.IndexOf('.');
                b.Name = fullname.Substring(partial + 1, fullname.Length - partial);
            } catch (Exception ex) {}
            return b;
        }
    }

}
