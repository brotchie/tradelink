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
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GCore
{
    /// <summary>
    /// The base Google Service feed entry class. Every Google Service feed entry must inherits this class.
    /// </summary>
    /// <remarks>
    /// You must add the [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")] attribute at top of each
    /// service feed entry class extending GEntry class.
    /// </remarks>
    [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")]
    public class GEntry : GObject
    {
        /// <summary>
        /// Gets and sets the Etag for the entry.
        /// </summary>
        [XmlAttribute("etag", Namespace = "http://schemas.google.com/g/2005")]
        public string Etag { get; set; }

        /// <summary>
        /// Gets and sets the id of the entry.
        /// </summary>
        [XmlElement("id", Namespace = "http://www.w3.org/2005/Atom")]
        public string IdString { get; set; }

        /// <summary>
        /// Gets and sets publish date of the entry.
        /// </summary>
        [XmlElement("published", Namespace = "http://www.w3.org/2005/Atom")]
        public DateTime Published { get; set; }

        /// <summary>
        /// Gets and sets the last update date of the entry.
        /// </summary>
        [XmlElement("updated", Namespace = "http://www.w3.org/2005/Atom")]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets and sets title of the entry.
        /// </summary>
        [XmlElement("title", Namespace = "http://www.w3.org/2005/Atom")]
        public string Title { get; set; }

        /// <summary>Gets and sets the content of the entry.</summary>
        [XmlElement("content", Namespace = "http://www.w3.org/2005/Atom")]
        public Content Content { get; set; }

        /// <summary>Gets and sets the list of links associated with the entry.</summary>
        [XmlElement("link", Namespace = "http://www.w3.org/2005/Atom")]
        public List<Link> Links { get; set; }

        /// <summary>
        /// Gets and sets the author of the entry.
        /// </summary>
        [XmlElement("author", Namespace = "http://www.w3.org/2005/Atom")]
        public Author Author { get; set; }
    }
}