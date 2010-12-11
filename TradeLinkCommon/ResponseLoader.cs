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
        /// <param name="fullname"></param>
        /// <param name="dllname"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static Response FromDLL(string fullname, string dllname, DebugDelegate deb)
        {
            try
            {
                return FromDLL(fullname, dllname);
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb(ex.Message + ex.StackTrace);

                }
            }
            return null;
        }
        /// <summary>
        /// Create a single Response from a DLL containing many Responses.  
        /// </summary>
        /// <param name="fullname">The fully-qualified Response Name (as in 'BoxExamples.Name').  </param>
        /// <param name="dllname">The path and filename of DLL.</param>
        /// <returns></returns>
        public static Response FromDLL(string fullname, string dllname)
        {
            System.Reflection.Assembly a;
            
#if (DEBUG)
            a = System.Reflection.Assembly.LoadFrom(dllname);
#else
            byte[] raw = loadFile(dllname);
            a = System.Reflection.Assembly.Load(raw);
#endif
            return FromAssembly(a, fullname);
        }
        /// <summary>
        /// Create a single Response from an Assembly containing many Responses. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="fullname"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static Response FromAssembly(System.Reflection.Assembly a, string fullname, DebugDelegate deb)
        {
            try
            {
                return FromAssembly(a, fullname);
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb(ex.Message + ex.StackTrace);
                }
            }
            return null;
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
            // get class from assembly
            type = a.GetType(fullname, true, true);
            args = new object[] { };
            // create an instance of type and cast to response
            b = (Response)Activator.CreateInstance(type, args);
            // if it doesn't have a name, add one
            if (b.Name == string.Empty)
            {
                b.Name = type.Name;
            }
            if (b.FullName == string.Empty)
            {
                b.FullName = type.FullName;
            }
            return b;
        }

        static byte[] loadFile(string filename)
        {
            // get file
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open,  System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            // prepare buffer based on file size
            byte[] buffer = new byte[(int)fs.Length];
            // read file into buffer
            fs.Read(buffer, 0, buffer.Length);
            // close file
            fs.Close();
            // return buffer
            return buffer;

        }
    }

}
