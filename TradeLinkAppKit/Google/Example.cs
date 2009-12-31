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

namespace GCodeIssueTracker
{
    /// <example>
    /// <code>
    /// 
    /// using System;
    /// using GCodeIssueTracker;
    /// using System.Net;
    /// using GCodeIssueTracker.Query;
    /// using System.Collections.Generic;
    /// using GCore;
    /// 
    /// internal class Program
    /// {
    ///    private static void Main()
    ///    {
    ///         var service = new ProjectHostingService("demo")
    ///                          {
    ///                              GUserName = "someone@gmail.com",
    ///                              GPassword = "password123"
    ///                          };
    ///         // getting all issues
    ///         IssuesFeed resultFeed = service.GetAllIssues();
    ///         if (resultFeed != null) Console.WriteLine(resultFeed.Entries.Count);
    ///         
    ///         // submitting a new issue
    ///         var newEntry = new IssuesEntry
    ///                           {
    ///                               Author = new Author { Name = "someone" },
    ///                               Content = new Content { Type = "text", Description = "Test Issue Submission" },
    ///                               Owner = new Owner { UserName = "someother" },
    ///                               Status = "New",
    ///                               Title = "Test One",
    ///                               Labels = new List&lt;string&gt; { "Priority-Medium" },
    ///                               Ccs = new List&lt;Cc&gt; { new Cc { UserName = "another" } }
    ///                           };
    ///        int id = service.SubmitNewIssue(newEntry, "demoapp").Id;
    ///        Console.WriteLine(id);
    ///        
    ///        // querying using parameters
    ///        var query = new IssueQuery();
    ///        query.AppendQuery(QueryParamType.CanAll, "");
    ///        query.AppendQuery(QueryParamType.Author, "someauthor");
    ///        query.AppendQuery(QueryParamType.Label, "High");
    ///        query.AppendQuery(QueryParamType.MaxResults, "1000");
    ///        query.AppendQuery(QueryParamType.Owner, "someowner");
    ///        query.AppendQuery(QueryParamType.PublishedDateMax, Utility.ToFriendlyDateString(DateTime.Today));
    ///        query.AppendQuery(QueryParamType.PublishedDateMin, Utility.ToFriendlyDateString(DateTime.Today));
    ///        query.AppendQuery(QueryParamType.QueryString, "blogger");
    ///        query.AppendQuery(QueryParamType.Stars, "4");
    ///        query.AppendQuery(QueryParamType.Status, "Fixed");
    ///        query.AppendQuery(QueryParamType.UpdatedDateMax, Utility.ToFriendlyDateString(DateTime.Today));
    ///        query.AppendQuery(QueryParamType.UpdatedDateMin, Utility.ToFriendlyDateString(DateTime.MinValue));
    ///        Console.WriteLine(query.GetQueryUri());
    ///        
    ///        IssuesFeed resultFeed = service.Query&lt;IssuesFeed, IssuesEntry&gt;(query);
    ///        foreach (IssuesEntry item in resultFeed.Entries)
    ///        {
    ///            Console.WriteLine(item.Id);
    ///        }
    ///        Console.WriteLine("Total feed : " + resultFeed.Entries.Count);
    ///    }
    /// }
    /// </code>
    /// </example>
    public sealed partial class ProjectHostingService
    {
    }
}
