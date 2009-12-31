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
using System.Collections.Generic;

namespace GCodeIssueTracker.Comments
{
    /// <summary>
    /// Updates of a particluar issue comment.
    /// </summary>
    public class Updates
    {
        /// <summary>
        /// Gets and sets updates on cc.
        /// </summary>
        [XmlElement("ccUpdate", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public List<string> CcUpdates { get; set; }

        /// <summary>Gets and sets the labels.</summary>
        [XmlElement("label", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public List<string> Labels { get; set; }

        /// <summary>Gets and sets the summary of the comment.</summary>
        [XmlElement("summary", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets and sets the owner of the update.
        /// </summary>
        [XmlElement("ownerUpdate", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string UpdateOwner { get; set; }

        /// <summary>
        /// Gets and sets the status of the comment.
        /// </summary>
        [XmlElement("status", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Status { get; set; }
    }
}