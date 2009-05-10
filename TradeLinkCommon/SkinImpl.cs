using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace TradeLink.Common
{
    /// <summary>
    /// save and restore states of specific Response instances
    /// </summary>
    public class SkinImpl : Skin
    {
        string _props = string.Empty;
        string _dll = string.Empty;
        string _class = string.Empty;
        public SkinImpl() { }
        private bool hasdefault() { return (_dll == string.Empty) && (_class == string.Empty); }
        private static bool havedll(string path) { return File.Exists(path) || File.Exists(Path.GetFileName(path)); }
        private bool havetype() { return GetType(_class,_dll)!=null; }
        /// <summary>
        /// must have provided a valid response and dll that contains said response's type
        /// </summary>
        public bool isValid { get { return !hasdefault() && havetype(); } }
        
        /// <summary>
        /// construct a new skin
        /// </summary>
        /// <param name="props"></param>
        /// <param name="classname"></param>
        /// <param name="dll"></param>
        public SkinImpl(string props, string classname, string dll) 
        { 
            _props = props; 
            _class = classname;
            if (havedll(dll))
                _dll = dll;
        }
        /// <summary>
        /// skins an existing response instance, using the response name and the dll containing the response's type
        /// </summary>
        /// <param name="t"></param>
        /// <param name="responsename"></param>
        /// <param name="dll"></param>
        public SkinImpl(object response, string responsename, string dll)
        {
            // save class name
            _class = responsename;
            // save dll
            if (havedll(dll))
                _dll = dll;
            // serialize props and save
            _props = serializeprops(GetType(_class, _dll), response);
        }
        /// <summary>
        /// create a skin with an existing response and dll
        /// </summary>
        /// <param name="response"></param>
        /// <param name="dll"></param>
        public SkinImpl(object response, string dll)
        {
            // cast to a response
            Response r = (Response)response;
            // get name
            _class = r.FullName;
            // make sure name exists
            if (r.FullName == string.Empty) 
                throw new Exception("cant import response with empty fullname.  use ResponseLoader or call a Skin method with full classname. ");
            // get dll
            if (havedll(dll))
                _dll = dll;
            // get properties
            _props = serializeprops(GetType(_class, _dll), response);
        }



        public string Properties { get { return _props; } set { _props = value; } }
        public string ResponseDLL { get { return _dll; } set { _dll = value; } }
        public string ResponseName { get { return _class; } set { _class=value; } }

        public static string Skin(object response, string classname, string dll)
        {
            return Serialize(new SkinImpl(response, classname, dll));
        }
        public static string Skin(object response, string dll)
        {
            return Serialize(new SkinImpl(response, dll));
        }

        public static object Deskin(string skin_msg)
        {
            // first get a skin
            Skin s = Deserialize(skin_msg);
            // return the deserialized properties
            return deserializeprops(GetType(s), s.Properties);
        }

        public static bool SkinFile(object response, string classname, string dll, string filename)
        {
            try
            {
                // get string data representing response
                string data = Skin(response, classname, dll);
                // prepare to write to file
                StreamWriter sw = new StreamWriter(filename, false);
                // write it
                sw.WriteLine(data);
                // clsoe file
                sw.Close();
                // report good status
                return true;
            }
            catch (Exception) { }
            // only way we got here if there was a problem
            return false;

        }


        public static object DeskinFile(string filename)
        {
            try
            {
                // open file name
                StreamReader sw = new StreamReader(filename);
                // get data
                string data = sw.ReadToEnd();
                // close file
                sw.Close();
                // deskin data
                return Deskin(data);
            }
            catch (Exception) { }
            // if we got here it's cause ane xception was thrown, so return null
            return null;
        }

        public static string Serialize(Skin skin)
        {
            if (!skin.isValid) return string.Empty;
            try
            {
                skin.Properties = GZip.Compress(skin.Properties);
                SkinImpl si = (SkinImpl)skin;
                XmlSerializer xs = new XmlSerializer(typeof(SkinImpl));
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, si);
                sw.Close();
                return sw.GetStringBuilder().ToString();
            }
            catch (Exception) { }
            return string.Empty;
        }

        public static Skin Deserialize(string msg)
        {
            try
            {
                // prepare serializer
                XmlSerializer xs = new XmlSerializer(typeof(SkinImpl));
                // read in message
                StringReader sw = new StringReader(msg);
                // deserialize message
                Skin s = (Skin)xs.Deserialize(sw);
                // close serializer
                sw.Close();
                // uncompress properties
                s.Properties = GZip.Uncompress(s.Properties);
                // return result
                return s;
            }
            catch (Exception) { }
            return new SkinImpl();
        }

        public static Type GetType(Skin skin) { return GetType(skin.ResponseName, skin.ResponseDLL); }
        public static Type GetType(string responsename, string dll)
        {
            bool fe = File.Exists(dll);
            
            // get assembly represented by this DLL
            Assembly asm = Assembly.LoadFrom(dll);
            foreach (Type t in asm.GetTypes())
            {
                // return if it's our type
                if (t.FullName == responsename)
                    return t;
            }
            return null;
        }


        private static string serializeprops(Type type, object o)
        {
            string props = string.Empty;
            try
            {
                // serialize the list
                XmlSerializer xs = new XmlSerializer(type);
                // get file to save skin
                StringWriter sw = new StringWriter();
                // save it
                xs.Serialize(sw, o);
                // get results
                props = sw.ToString();
                // close writer
                sw.Close();
            }
            catch (Exception ex) { return string.Empty; }
            return props;
        }

        private static object deserializeprops(Type type, string msg)
        {
            try
            {
                // prepare serializer
                XmlSerializer xs = new XmlSerializer(type);
                // read in message
                StringReader sw = new StringReader(msg);
                // deserialize message
                object o = xs.Deserialize(sw);
                // convert type
                object myo = Convert.ChangeType(o, type);
                // close serializer
                sw.Close();
                // return result
                return myo;
            }
            catch (Exception) { }
            return null;
        }
    }
}
