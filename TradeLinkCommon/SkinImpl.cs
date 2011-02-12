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
        public SkinImpl(object response, string responsename, string dll) : this(response, responsename, dll, (DebugDelegate)null) { }
        public SkinImpl(object response, string responsename, string dll, DebugDelegate deb)
        {
            // save class name
            _class = responsename;
            // save dll
            if (havedll(dll))
                _dll = dll;
            // serialize props and save
            _props = serializeprops(GetType(_class, _dll), response,deb);
        }
        /// <summary>
        /// create a skin with an existing response and dll
        /// </summary>
        /// <param name="response"></param>
        /// <param name="dll"></param>
        public SkinImpl(object response, string dll) : this(response, dll, (DebugDelegate)null) { }
        public SkinImpl(object response, string dll,DebugDelegate deb)
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
            _props = serializeprops(GetType(_class, _dll), response,deb);
        }

        List<string> _tickfiles = new List<string>();
        public string[] TickFiles { get { return _tickfiles.ToArray(); } set { _tickfiles.Clear(); _tickfiles.AddRange(value); } }

        public string Properties { get { return _props; } set { _props = value; } }
        public string ResponseDLL { get { return _dll; } set { _dll = value; } }
        public string ResponseName { get { return _class; } set { _class=value; } }

        public static string Skin(object response, string classname, string dll) { return Skin(response, classname, dll, null); }
        public static string Skin(object response, string classname, string dll, DebugDelegate deb)
        {
            return Serialize(new SkinImpl(response, classname, dll,deb),deb);
        }
        public static string Skin(object response, string dll) { return Skin(response, dll, (DebugDelegate)null); }
        public static string Skin(object response, string dll,DebugDelegate deb)
        {
            return Serialize(new SkinImpl(response, dll),deb);
        }

        public static object Deskin(string skin_msg) { return Deskin(skin_msg, null); }
        public static object Deskin(string skin_msg, DebugDelegate deb)
        {
            // first get a skin
            Skin s = Deserialize(skin_msg,deb);
            Type t = GetType(s);
            // check for properties
            if (s.Properties.Length > 0)
                // return object with deserialized properties
                return deserializeprops(t, s.Properties, deb);
            else
            {
                try
                {
                    object o = Activator.CreateInstance(t);
                    return o;
                }
                catch (Exception ex)
                {
                    debug(deb,"Error creating object: " + t.ToString());
                    debug(deb, ex.Message + ex.StackTrace);
                }

            }
            return null;
        }

        public static bool SkinFile(object response, string classname, string dll, string filename) { return SkinFile(response,classname,dll,filename,null); }
        public static bool SkinFile(object response, string classname, string dll, string filename, DebugDelegate deb)
        {
            try
            {
                // get string data representing response
                string data = Skin(response, classname, dll,deb);
                // prepare to write to file
                StreamWriter sw = new StreamWriter(filename, false);
                // write it
                sw.WriteLine(data);
                // clsoe file
                sw.Close();
                // report good status
                return true;
            }
            catch (Exception ex)
            {
                debug(deb,"Error skinning: " + classname);
                debug(deb,ex.Message + ex.StackTrace);
            }

            // only way we got here if there was a problem
            return false;

        }


        public static object DeskinFile(string filename) { return DeskinFile(filename, null); }
        public static object DeskinFile(string filename,DebugDelegate deb)
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
                return Deskin(data,deb);
            }
            catch (Exception ex) 
            {
                debug(deb, "Error deskining: " + filename);
                debug(deb,ex.Message + ex.StackTrace);
            }
            // if we got here it's cause ane xception was thrown, so return null
            return null;
        }

        public static string Serialize(Skin skin) { return Serialize(skin, null); }
        public static string Serialize(Skin skin,DebugDelegate deb)
        {
            if (!skin.isValid) return string.Empty;
            try
            {
                skin.Properties = skin.Properties;
                SkinImpl si = (SkinImpl)skin;
                XmlSerializer xs = new XmlSerializer(typeof(SkinImpl));
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, si);
                sw.Close();
                return sw.GetStringBuilder().ToString();
            }
            catch (Exception ex) 
            { 
                debug(deb,"Error saving skin: "+ex.Message+ex.StackTrace);
            }
            return string.Empty;
        }

        public static Skin Deserialize(string msg) { return Deserialize(msg, null); }
        public static Skin Deserialize(string msg, DebugDelegate deb)
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
                s.Properties = s.Properties;
                // return result
                return s;
            }
            catch (Exception ex) 
            {
                debug(deb,"Error on: " + msg);
                debug(deb,ex.Message + ex.StackTrace);
            }
            return new SkinImpl();
        }

        public static Type GetType(Skin skin) { return GetType(skin.ResponseName, skin.ResponseDLL); }
        public static Type GetType(string responsename, string dll)
        {
            bool fe = File.Exists(dll);
			if (!fe){
                return null;				
			}
			
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


        static string serializeprops(Type type, object o) { return serializeprops(type, o, null); }
        private static string serializeprops(Type type, object o,DebugDelegate deb)
        {
            string props = string.Empty;
            try
            {
                // serialize the list
                XmlSerializer xs = new XmlSerializer(o.GetType());
                // get file to save skin
                StringWriter sw = new StringWriter();
                // save it
                xs.Serialize(sw, o);
                // get results
                props = sw.ToString();
                // close writer
                sw.Close();
            }
            catch (Exception ex) 
            {
                debug(deb, "Error saving props: " + type.ToString() + " " + o.ToString());
                debug(deb, ex.Message + ex.StackTrace);
                return string.Empty; 
            }
            return props;
        }

        static object deserializeprops(Type type, string msg) { return deserializeprops(type, msg, null); }
        private static object deserializeprops(Type type, string msg, DebugDelegate deb)
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
            catch (Exception ex) 
            {
                debug(deb, "Can't apply skin properties: " + type.ToString() + " " + msg);
                debug(deb, ex.Message + ex.StackTrace);
            }
            return null;
        }

        static void debug(DebugDelegate deb, string msg)
        {
            if (deb == null) return;
            deb(msg);
        }
        public const string SKINEXT_WILD = "*.skn";
        public const string SKINEXT_NODOT = "skn";
        public const string SKINEXT = ".skn";
        public static string SKINPATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\";

        public static string[] getskinfiles() { return getskinfiles(SKINPATH); }
        /// <summary>
        /// find skins in a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] getskinfiles(string path)
        {
            List<string> files = new List<string>();
            // get info for this directory
            DirectoryInfo di = new DirectoryInfo(path);
            // find all skins in this directory
            FileInfo[] skins = di.GetFiles("*" + SKINEXT);
            // build list of their names
            foreach (FileInfo skin in skins)
                files.Add(skin.Name);
            // return results
            return files.ToArray();
        }

        /// <summary>
        /// get skin name from filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string skinfromfile(string filename)
        {
            string name = Path.GetFileNameWithoutExtension(filename);
            string[] r = name.Split('.');
            return r[0];
        }

        public static string filefromskin(string skinname) { return filefromskin(skinname, 0, SKINPATH); }
        public static string filefromskin(string skinname, int response) { return filefromskin(skinname, response, SKINPATH); }
        public static string filefromskin(string skinname, int response, string path)
        {
            return path + "\\" + skinname + "." + response + SKINEXT;
        }

    }
}
