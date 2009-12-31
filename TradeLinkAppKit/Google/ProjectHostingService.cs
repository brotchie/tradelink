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
using System.Net;
using GCodeIssueTracker.Comments;
using GCore;

namespace GCodeIssueTracker
{
    /// <summary>
    /// Google Code - project hosting service class.
    /// </summary>
    /// <remarks>This class cannot be extended.</remarks>
    public sealed partial class ProjectHostingService : GService
    {
        /// <summary>
        /// Instantiate a new <seealso cref="ProjectHostingService"/> object.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        public ProjectHostingService(string projectName)
        {
            ProjectName = projectName;
        }

        /// <summary>
        /// Instantiate a new <seealso cref="ProjectHostingService"/> object.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="userName">The Google account username.</param>
        /// <param name="password">The Google account password.</param>
        public ProjectHostingService(string projectName, string userName, string password)
            : base(userName, password)
        {
            ProjectName = projectName;
        }

        /// <summary>
        /// Instantiate a new <seealso cref="ProjectHostingService"/> object.
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="userName">The Google account username.</param>
        /// <param name="password">The Google account password.</param>
        /// <param name="proxy">Web proxy to be used, if behind a firewall.</param>
        public ProjectHostingService(string projectName, string userName, string password, WebProxy proxy)
            : base(userName, password, proxy)
        {
            ProjectName = projectName;
        }

        /// <summary>
        /// Gets and sets the Google Code Project name
        /// </summary>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Gets all issue for a project as a <see cref="IssuesFeed"/> object.
        /// </summary>
        /// <returns>A <see cref="IssuesFeed"/> object.</returns>
        public IssuesFeed GetAllIssues()
        {
            GUrl = "http://code.google.com/feeds/issues/p/" + ProjectName + "/issues/full";
            return base.GetAllFeed<IssuesFeed, IssuesEntry>();
        }

        /// <summary>
        /// Submits new <seealso cref="IssuesEntry"/> into the project.
        /// </summary>
        /// <param name="newEntry">A new issue entry.</param>
        /// <param name="applicationName">Application name invoking the submission.</param>
        /// <returns>A new <see cref="IssuesEntry"/> object containing some additional data.</returns>
        public IssuesEntry SubmitNewIssue(IssuesEntry newEntry, string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
                applicationName = "GCodeIssueTracker-1.0";

            GUrl = "http://code.google.com/feeds/issues/p/" + ProjectName + "/issues/full";
            return base.SubmitNewEntry<IssuesFeed, IssuesEntry>(newEntry, applicationName);
        }

        /// <summary>
        /// Queries the project service to get issues depending on certain query parameters.
        /// </summary>
        /// <param name="query">The data about the query paramteres.</param>
        /// <returns>Issues satisfying the query.</returns>
        public IssuesFeed Query(IGQuery query)
        {
            GUrl = "http://code.google.com/feeds/issues/p/" + ProjectName + "/issues/full";
            return base.Query<IssuesFeed, IssuesEntry>(query);
        }

        /// <summary>
        /// Gets all comments for a particular issue.
        /// </summary>
        /// <param name="issueId">Issue id</param>
        /// <returns>All comments of a particular issue.</returns>
        public IssueCommentsFeed GetAllIssueComments(string issueId)
        {
            GUrl = "http://code.google.com/feeds/issues/p/" + ProjectName + "/issues/" + issueId + "/comments/full";
            return base.GetAllFeed<IssueCommentsFeed, IssueCommentsEntry>();
        }

        /// <summary>
        /// Submit new comments for a particular issue.
        /// </summary>
        /// <param name="newEntry">New comment entry.</param>
        /// <param name="issueId">Id for the issue.</param>
        /// <param name="applicationName">Application which is invoking this call.</param>
        /// <returns></returns>
        public IssueCommentsEntry SubmitNewIssueComment(IssueCommentsEntry newEntry, string issueId, string applicationName)
        {
            GUrl = "http://code.google.com/feeds/issues/p/" + ProjectName + "/issues/" + issueId + "/comments/full";
            return base.SubmitNewEntry<IssueCommentsFeed, IssueCommentsEntry>(newEntry, applicationName);
        }
    }
}