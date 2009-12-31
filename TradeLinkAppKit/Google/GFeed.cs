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
    /// The base Google Service feed class. Every Google Service feed must inherits this class.
    /// </summary>
    /// <typeparam name="TEntry">The type of Atom entry for this feed.</typeparam>
    /// <remarks>
    /// You must add the [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")] attribute at top of each
    /// service feed class extending GFeed&lt;TEntry&gt; class.
    /// </remarks>
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class GFeed<TEntry> : GObject where TEntry : GEntry, new()
    {
        /// <summary>
        /// Gets and sets feed id.
        /// </summary>
        [XmlElement("id", Namespace = "http://www.w3.org/2005/Atom")]
        public string Id { get; set; }

        /// <summary>
        /// Gets and sets date for last update.
        /// </summary>
        [XmlElement("updated", Namespace = "http://www.w3.org/2005/Atom")]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets and sets the title of the feed.
        /// </summary>
        [XmlElement("title", Namespace = "http://www.w3.org/2005/Atom")]
        public string Title { get; set; }

        /// <summary>
        /// Gets and sets the subtitle of the feeds (if any).
        /// </summary>
        [XmlElement("subtitle", Namespace = "http://www.w3.org/2005/Atom")]
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets and sets the collection of links associated with the feed.
        /// </summary>
        [XmlElement("link", Namespace = "http://www.w3.org/2005/Atom")]
        public List<Link> Links { get; set; }

        /// <summary>
        /// Gets and sets the generator of the feed.
        /// </summary>
        [XmlElement("generator", Namespace = "http://www.w3.org/2005/Atom")]
        public Generator Generator { get; set; }

        /// <summary>
        /// Gets and sets the list of <see cref="GEntry"/> objects of the feed.
        /// </summary>
        [XmlElement("entry", Namespace = "http://www.w3.org/2005/Atom")]
        public List<TEntry> Entries { get; set; }

        /// <summary>
        /// Gets and sets the total results in the feed.
        /// </summary>
        [XmlElement("totalResults", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public string TotalResults { get; set; }

        /// <summary>
        /// Gets and sets the start index of the feed.
        /// </summary>
        [XmlElement("startIndex", Namespace = "http://a9.com/-/spec/opensearch/1.1/")]
        public string StartIndex { get; set; }
    }
}