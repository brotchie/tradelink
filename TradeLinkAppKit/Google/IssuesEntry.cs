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
using GCore;

namespace GCodeIssueTracker
{
    /// <summary>
    /// Issue entry class for the Google project hosting service.
    /// </summary>
    [XmlRoot("entry", Namespace = "http://www.w3.org/2005/Atom")]
    public class IssuesEntry : GEntry
    {
        /// <summary>Gets and sets the closed date.</summary>
        [XmlElement("closedDate", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public DateTime ClosedDate { get; set; }

        /// <summary>
        /// Gets and sets the id of the issue.
        /// </summary>
        [XmlElement("id", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public int Id { get; set; }

        /// <summary>Gets and sets the labels associated with the issue.</summary>
        [XmlElement("label", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public List<string> Labels { get; set; }

        /// <summary>Gets and sets the owner of the issue.</summary>
        [XmlElement("owner", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public Owner Owner { get; set; }

        /// <summary>Gets and sets the stars associated to the issue.</summary>
        [XmlElement("stars", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Stars { get; set; }

        /// <summary>Gets and sets the state of the issue.</summary>
        [XmlElement("state", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string State { get; set; }

        /// <summary>Gets and sets the status of the issue.</summary>
        [XmlElement("status", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public string Status { get; set; }

        /// <summary>
        /// Gets and sets the cc fields associated with the issue.
        /// </summary>
        [XmlElement("cc", Namespace = "http://schemas.google.com/projecthosting/issues/2009")]
        public List<Cc> Ccs { get; set; }

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