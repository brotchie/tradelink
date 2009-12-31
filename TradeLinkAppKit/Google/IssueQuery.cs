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
using System.Diagnostics;
using GCore;

namespace GCodeIssueTracker.Query
{
    /// <summary>
    /// Class containing all the parameter information of an issue query.
    /// </summary>
    public class IssueQuery : IGQuery
    {
        private string _query;

        /// <summary>
        /// Instantiate a new <see cref="IssueQuery"/> object.
        /// </summary>
        public IssueQuery()
        {
            QueryParams = new Dictionary<QueryParamType, string>();
            ExclusionSet = new List<string>();
        }

        private Dictionary<QueryParamType, string> QueryParams { get; set; }
        private List<string> ExclusionSet { get; set; }

        #region IGQuery Members

        /// <summary>
        /// Returns the query constructed query url.
        /// </summary>
        /// <returns>Query url</returns>
        public string GetQueryUrl()
        {
            ConstructQueryUrl();
            return _query;
        }

        #endregion

        /// <summary>
        /// Appends new query parameters to the query.
        /// </summary>
        /// <param name="paramType">Type of the parameter.</param>
        /// <param name="value">Value for the query parameter.</param>
        public void AppendQuery(QueryParamType paramType, string value)
        {
            if (IsMutuallyExclusive(paramType))
                QueryParams.Add(paramType, Utility.UriEncodeUnsafe(value));
            else
                Trace.WriteLine(paramType +
                                " can not be added due to the existance of other mutually exclusive query parameter");
        }

        private bool IsMutuallyExclusive(QueryParamType paramType)
        {
            switch (paramType)
            {
                case QueryParamType.CanAll:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanOpen:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanOwned:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanReported:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanStarred:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanNew:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.CanToVerify:
                    if (!ExclusionSet.Contains("can") && !ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("can");
                        return true;
                    }
                    return false;
                case QueryParamType.Author:
                    if (!ExclusionSet.Contains("id"))
                        return true;
                    return false;
                case QueryParamType.Id:
                    if (!ExclusionSet.Contains("id") && ExclusionSet.Count == 0)
                    {
                        ExclusionSet.Add("id");
                        return true;
                    }
                    return false;
                case QueryParamType.Label:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("label");
                        return true;
                    }
                    return false;
                case QueryParamType.Owner:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("owner");
                        return true;
                    }
                    return false;
                case QueryParamType.PublishedDateMin:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("published");
                        return true;
                    }
                    return false;
                case QueryParamType.PublishedDateMax:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("published");
                        return true;
                    }
                    return false;
                case QueryParamType.QueryString:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("query");
                        return true;
                    }
                    return false;
                case QueryParamType.Status:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("status");
                        return true;
                    }
                    return false;
                case QueryParamType.Stars:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("stars");
                        return true;
                    }
                    return false;
                case QueryParamType.MaxResults:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("maxresult");
                        return true;
                    }
                    return false;
                case QueryParamType.UpdatedDateMin:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("update");
                        return true;
                    }
                    return false;
                case QueryParamType.UpdatedDateMax:
                    if (!ExclusionSet.Contains("id"))
                    {
                        ExclusionSet.Add("update");
                        return true;
                    }
                    return false;
                default:
                    return true;
            }
        }


        private void ConstructQueryUrl()
        {
            string canString = "", qString = "q=", labelString = "", idString = "", dateString = "", resultString = "";

            if (QueryParams != null)
            {
                Dictionary<QueryParamType, string>.KeyCollection keys = QueryParams.Keys;
                foreach (QueryParamType paramType in keys)
                {
                    switch (paramType)
                    {
                        case QueryParamType.Author:
                            labelString += "author:" + QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.CanAll:
                            canString = "can=all&";
                            break;
                        case QueryParamType.CanOpen:
                            canString = "can=open&";
                            break;
                        case QueryParamType.CanOwned:
                            canString = "can=owned&";
                            break;
                        case QueryParamType.CanReported:
                            canString = "can=reported&";
                            break;
                        case QueryParamType.CanStarred:
                            canString = "can=starred&";
                            break;
                        case QueryParamType.CanNew:
                            canString = "can=new&";
                            break;
                        case QueryParamType.CanToVerify:
                            canString = "can=to-verify&";
                            break;
                        case QueryParamType.Id:
                            idString = "id=" + QueryParams[paramType] + "&";
                            break;
                        case QueryParamType.Label:
                            labelString += "label:" + QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.Owner:
                            labelString += "owner:" + QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.PublishedDateMin:
                            dateString += "published-min=" + QueryParams[paramType] + "&";
                            break;
                        case QueryParamType.PublishedDateMax:
                            dateString += "published-max=" + QueryParams[paramType] + "&";
                            break;
                        case QueryParamType.QueryString:
                            qString += QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.Status:
                            labelString += "status:" + QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.Stars:
                            labelString += "stars:" + QueryParams[paramType] + "+";
                            break;
                        case QueryParamType.UpdatedDateMin:
                            dateString += "updated-min=" + QueryParams[paramType] + "&";
                            break;
                        case QueryParamType.UpdatedDateMax:
                            dateString += "updated-max=" + QueryParams[paramType] + "&";
                            break;
                        case QueryParamType.MaxResults:
                            resultString = "max-results=" + QueryParams[paramType] + "&";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                _query = canString + dateString + idString + resultString + qString + labelString;

                // remove last +
                _query = _query.Remove(_query.Length - 1);
            }
        }
    }
}