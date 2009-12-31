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

namespace GCodeIssueTracker
{
    /// <summary>
    /// Owner of the issue.
    /// </summary>
    public class Owner
    {
        /// <summary>Gets and sets the uri associated with the owner.</summary>
        [XmlElement("uri", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Uri { get; set; }

        /// <summary>Gets and sets the username of the owner.</summary>
        [XmlElement("username", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// Cc field of the issue.
    /// </summary>
    public class Cc
    {
        /// <summary>Gets and sets the uri associated with the cc user.</summary>
        [XmlElement("uri", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Uri { get; set; }

        /// <summary>Gets and sets the username of the cc.</summary>
        [XmlElement("username", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string UserName { get; set; }
    }
}