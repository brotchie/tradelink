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
using System.Serialization.Xml;
using System.Xml.Serialization;

namespace GCore
{
    /// <summary>
    /// Base Google service object. This class is xml serializable. This class is responsible for 
    /// xml serialization for Google service atom feeds. <see cref="GFeed&lt;TEntry&gt;"/> and <see cref="GEntry"/> extends
    /// this class to be serializable.
    /// </summary>
    public class GObject : IXObject
    {
        #region IXObject Members

        /// <summary>
        /// Gets and sets default xml namespace for the object.
        /// </summary>
        [XmlIgnore]
        public string DefaultNameSpace { get; set; }

        /// <summary>
        /// Gets and sets the collection of the xml namespaces for the object.
        /// </summary>
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaceCollection { get; set; }

        /// <summary>
        /// Executes routines just after deserialization.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Executes routines just before serialization.
        /// </summary>
        public virtual void Close()
        {
            if (XmlNamespaceCollection == null)
                XmlNamespaceCollection = new XmlSerializerNamespaces();
            XmlNamespaceCollection.Add("gd", "http://schemas.google.com/g/2005");
            XmlNamespaceCollection.Add("openSearch", "http://a9.com/-/spec/opensearch/1.1/");

            DefaultNameSpace = "http://www.w3.org/2005/Atom";
        }

        #endregion
    }
}