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
namespace GCodeIssueTracker.Query
{
    /// <summary>
    /// Different query parameter types.
    /// </summary>
    public enum QueryParamType
    {
        /// <summary>
        /// Canned query identifier - All issues
        /// </summary>
        CanAll = 1,
        /// <summary>
        /// Canned query identifier - Open issues
        /// </summary>
        CanOpen,
        /// <summary>
        /// Canned query identifier - Open and owned by me
        /// </summary>
        CanOwned,
        /// <summary>
        /// Canned query identifier - Open and reported by me
        /// </summary>
        CanReported,
        /// <summary>
        /// Canned query identifier - Open and starred by me
        /// </summary>
        CanStarred,
        /// <summary>
        /// Canned query identifier - New issues
        /// </summary>
        CanNew,
        /// <summary>
        /// Canned query identifier - Issues to verify
        /// </summary>
        CanToVerify,
        /// <summary>
        /// Author of the issue
        /// </summary>
        Author,
        /// <summary>
        /// Id of the issue
        /// </summary>
        Id,
        /// <summary>
        /// Label associated with the issue
        /// </summary>
        Label,
        /// <summary>
        /// Owner of the issue
        /// </summary>
        Owner,
        /// <summary>
        /// Publish date minimum limit
        /// </summary>
        PublishedDateMin,
        /// <summary>
        /// Publish date maximum limit
        /// </summary>
        PublishedDateMax,
        /// <summary>
        /// Search string
        /// </summary>
        QueryString,
        /// <summary>
        /// Status of the issue
        /// </summary>
        Status,
        /// <summary>
        /// Stars associated with the issue
        /// </summary>
        Stars,
        /// <summary>
        /// Maximum results per query
        /// </summary>
        MaxResults,
        /// <summary>
        /// Last update date minimum limit
        /// </summary>
        UpdatedDateMin,
        /// <summary>
        /// Last update date maximum limit
        /// </summary>
        UpdatedDateMax
    }
}