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
using GCore;

namespace GCodeIssueTracker.Comments
{
    /// <summary>
    /// Issue comment entry class.
    /// </summary>
    [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")]
    public class IssueCommentsEntry : GEntry
    {
        /// <summary>
        /// Gets and sets updates on a particular issue comment.
        /// </summary>
        [XmlElement("updates", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public Updates Updates { get; set; }

        /// <summary>
        /// Adds project hosting xml namespaces before serialization.
        /// </summary>
        public override void Close()
        {
            base.Close();
            XmlNamespaceCollection.Add("issues", "http://schemas.google.com/projecthosting/issues/2009");
        }
    }
}