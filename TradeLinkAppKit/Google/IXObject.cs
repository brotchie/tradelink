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
using System.Xml.Serialization;

namespace System.Serialization.Xml
{
    /// <summary>
    /// XObject base class. Any class need to extend it to be xml serializable
    /// </summary>
    /// <example>
    /// <code>
    /// using System.Serialization;
    /// using System.Xml.Serialization;
    /// using System;
    /// using System.Collections.Generic;
    /// 
    /// namespace Demo
    /// {
    ///    [XmlRoot("RootConfig")]
    ///    public class ConfigDemo : IXObject
    ///    {
    ///            
    ///        [XmlElement("TestValue")]
    ///        public string TestValue { get; set; }
    ///
    ///        [XmlArray("TestList")]
    ///        public List&lt;String&gt; TestList { get; set; }
    ///
    ///        [XmlElement("LoadingTime")]
    ///        public DateTime LoadTime { get; set; }
    ///
    ///        [XmlElement("SavingTime")]
    ///        public DateTime SaveTime { get; set; }
    ///
    ///        public void Init()
    ///        {
    ///            LoadTime = DateTime.Now;
    ///        }
    ///
    ///        void IXObject.Close()
    ///        {
    ///            
    ///        }
    ///    }
    /// }
    /// 
    /// </code> 
    /// </example>
    [XmlRoot]
    public interface IXObject
    {
        /// <summary>
        /// Default Xmlnamespace to be used for all the elements
        /// </summary>
        [XmlIgnore]
        string DefaultNameSpace { get; set; }

        /// <summary>
        /// Additional XmlSerializerNamespaces attributes
        /// Put [XmlNamespaceDeclarations] before this property in the implemented class
        /// </summary>
        [XmlNamespaceDeclarations]
        XmlSerializerNamespaces XmlNamespaceCollection { get; set; }

        /// <summary>
        /// Executes routines just after deserialization
        /// </summary>
        /// <remarks>
        /// Implements this method explicitly if you don't want to use it in the class
        /// </remarks>
        void Init();

        /// <summary>
        /// Executes routines just before serialization
        /// </summary>
        /// <remarks>
        /// Implements this explicitly if you don't want to use in the class
        /// </remarks>
        void Close();
    }
}