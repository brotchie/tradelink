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

namespace GCore
{
    /// <summary>
    /// Author of hte service feed entry
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Gets and sets the name of the author.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the url associated with the author.
        /// </summary>
        [XmlElement("uri")]
        public string Uri { get; set; }
    }

    /// <summary>
    /// Contents of a service feed antry object
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Gets and sets the content type.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets and sets the body of the content.
        /// </summary>
        [XmlText]
        public string Description { get; set; }
    }

    /// <summary>
    /// Generators of a Google service feed entry
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Gets and sets the version of the feed entry.
        /// </summary>
        [XmlAttribute("version")]
        public decimal Version { get; set; }

        /// <summary>
        /// Gets and sets the uri associated with the <see cref="Generator"/> object.
        /// </summary>
        [XmlAttribute("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Gets and sets the text associated with the generator object.
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }

    /// <summary>
    /// The link associated with the service feed.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Gets and sets the rel of the link.
        /// </summary>
        [XmlAttribute("rel")]
        public string Rel { get; set; }

        /// <summary>
        /// Gets and sets the type of the link.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets and sets the Href of the link.
        /// </summary>
        [XmlAttribute("href")]
        public string Href { get; set; }
    }
}