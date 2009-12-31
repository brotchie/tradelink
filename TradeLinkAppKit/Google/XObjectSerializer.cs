#region Copyright Notice
// 
//
// The MIT License (http://www.opensource.org/licenses/mit-license.php)
// 
// Copyright (c) 2009 Anindya Chatterjee
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 
//
#endregion
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Serialization.Xml
{
    /// <summary>
    /// Serializer tool to (de)serialize IXObject object
    /// </summary>
    /// <typeparam name="T">A class who implements <see cref="IXObject"/> interface</typeparam>
    public class XObjectSerializer<T> where T : IXObject, new()
    {
        private readonly Type _type;
        private XmlSerializer _s;

        /// <summary>
        /// Initializes a new instance of the <see cref="XObjectSerializer&lt;T&gt;"/> class.
        /// </summary>
        public XObjectSerializer()
        {
            _type = typeof (T);
            _s = new XmlSerializer(_type);
        }

        /// <summary>Deserializes a xml string to an instance of T object</summary>
        /// <param name="xml">Xml string.</param>
        /// <returns>T object</returns>
        public T Deserialize(string xml)
        {
            TextReader reader = new StringReader(xml);
            return Deserialize(reader);
        }

        /// <summary>Deserializes to an instance of Feed.</summary>
        /// <param name="doc">XmlDocument instance.</param>
        /// <returns>Feed result.</returns>
        public T Deserialize(XmlDocument doc)
        {
            TextReader reader = new StringReader(doc.OuterXml);
            return Deserialize(reader);
        }

        /// <summary>Returns an instance of T object from a <see cref="System.IO.TextReader"/> object</summary>
        /// <param name="reader"><see cref="System.IO.TextReader"/> instance.</param>
        /// <returns>T object</returns>
        public T Deserialize(TextReader reader)
        {
            var o = (T) _s.Deserialize(reader);
            o.Init();
            reader.Close();
            return o;
        }

        /// <summary>Serializes T object to an <see cref="System.Xml.XmlDocument"/> object.</summary>
        /// <param name="xObj">T object to serialize.</param>
        /// <returns>An <see cref="System.Xml.XmlDocument"/> instance.</returns>
        public XmlDocument Serialize(T xObj)
        {
            string xml = StringSerialize(xObj);
            var doc = new XmlDocument {PreserveWhitespace = true};
            doc.LoadXml(xml);
            doc = Clean(doc);
            return doc;
        }

        /// <summary>
        /// Serializes T object to a xml string
        /// </summary>
        /// <param name="xObj">T object</param>
        /// <returns>Xml string</returns>
        public string StringSerialize(T xObj)
        {
            TextWriter w = WriterSerialize(xObj);
            string xml = w.ToString();
            w.Close();
            return xml.Trim();
        }

        /// <summary>
        /// Serializes the T object and returns a <see cref="System.IO.TextWriter"/> object
        /// </summary>
        /// <param name="xObj">T object</param>
        /// <returns><see cref="System.IO.TextWriter"/> object</returns>
        public TextWriter WriterSerialize(T xObj)
        {
            TextWriter w = new StringWriter();
            xObj.Close();
            if (String.IsNullOrEmpty(xObj.DefaultNameSpace))
                _s = new XmlSerializer(_type, xObj.DefaultNameSpace);
            else
                _s = new XmlSerializer(_type);
            _s.Serialize(w, xObj);
            w.Flush();
            return w;
        }

        private static XmlDocument Clean(XmlDocument doc)
        {
            doc.RemoveChild(doc.FirstChild);
            XmlNode first = doc.FirstChild;
            foreach (XmlNode n in doc.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    first = n;
                    break;
                }
            }
            if (first.Attributes != null)
            {
                XmlAttribute a = first.Attributes["xmlns:xsd"];
                if (a != null)
                {
                    first.Attributes.Remove(a);
                }
                a = first.Attributes["xmlns:xsi"];
                if (a != null)
                {
                    first.Attributes.Remove(a);
                }
            }
            return doc;
        }

        /// <summary>Reads object data from a xml file.</summary>
        /// <param name="file">Xml file name.</param>
        /// <returns>T object</returns>
        public static T ReadFile(string file)
        {
            T xObj;
            var serializer = new XObjectSerializer<T>();
            try
            {
                string xml;
                using (var reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                xObj = serializer.Deserialize(xml);
                return xObj;
            }
            catch
            {
            }
            xObj = new T();
            xObj.Init();
            return xObj;
        }

        /// <summary>Writes object data to xml file.</summary>
        /// <param name="file">Xml file name.</param>
        /// <param name="xObj">T object.</param>
        public static bool WriteFile(string file, T xObj)
        {
            bool ok = false;
            var serializer = new XObjectSerializer<T>();
            try
            {
                string xml = serializer.Serialize(xObj).OuterXml;
                using (var writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch
            {
            }
            return ok;
        }
    }
}