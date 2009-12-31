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
using System.IO;
using System.Net;
using System.Serialization.Xml;
using System.Threading;
using GCore.Authentication;

namespace GCore
{
    /// <summary>
    /// Base Google Service class. This class generically gets all the feeds, inserts new feed entry, queries
    /// using parameters. All Google service class must be derived from this class. The member functions are
    /// only visible to the class which inherits this class.
    /// </summary>
    public partial class GService
    {
        /// <summary>
        /// Instantiate a new <see cref="GService"/> object.
        /// </summary>
        public GService()
        {            
        }

        /// <summary>
        /// Instantiate a new <see cref="GService"/> object.
        /// </summary>
        /// <param name="userName">The Google account username.</param>
        /// <param name="password">The Google account password.</param>
        public GService(string userName, string password)            
        {
            GUserName = userName;
            GPassword = password;
        }

        /// <summary>
        /// Instantiate a new <see cref="GService"/> object.
        /// </summary>
        /// <param name="userName">The Google account username.</param>
        /// <param name="password">The Google account password.</param>
        /// <param name="proxy">Webproxy settings, if the application is behind a firewall.</param>
        public GService(string userName, string password, WebProxy proxy)
            : this(userName, password)
        {
            ProxySettings = proxy;
        }

        /// <summary>
        /// Gets and sets the proxy settings for the application.
        /// </summary>
        public WebProxy ProxySettings { get; set; }

        /// <summary>
        /// Gets and sets the Google account username.
        /// </summary>
        public string GUserName { get; set; }

        /// <summary>
        /// Gets and sets the Google account password.
        /// </summary>
        public string GPassword { private get; set; }
                
        /// <summary>
        /// Gets and sets the google feeds url.
        /// </summary>
        protected string GUrl { get; set; }

        /// <summary>
        /// Gets all feeds from <see cref="GUrl"/> specified for a particular Google service.
        /// </summary>
        /// <typeparam name="TFeed">Type of atom feed for the service.</typeparam>
        /// <typeparam name="TEntry">Type of atom feed entry for the service.</typeparam>
        /// <returns>Atom feed for the paricular Google service</returns>
        protected virtual TFeed GetAllFeed<TFeed, TEntry>()
            where TFeed : GFeed<TEntry>, new()
            where TEntry : GEntry, new()
        {
            if (String.IsNullOrEmpty(GUrl))
                throw new ArgumentNullException(GUrl,
                                                "Feed Url is not provided. Please provide Feed url first through GUrl Property.");

            var request = WebRequest.Create(GUrl) as HttpWebRequest;

            if (request != null)
            {
                if (ProxySettings != null)
                    request.Proxy = ProxySettings;
                request.Credentials = new NetworkCredential(GUserName, GPassword);
                var webData = new GWebAsyncData<TFeed, TEntry>
                                  {Request = request, ResetEvent = new ManualResetEvent(false)};
                IAsyncResult result = request.BeginGetResponse(AsyncManager<TFeed, TEntry>.AsyncDownloadAllFeeds,
                                                               webData);
                ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.WaitOne();

                return ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).FeedResult;
            }
            throw new OutOfMemoryException("Unable to create new WebRequest");
        }

        /// <summary>
        /// Submits a new atom entry to a particular Google service.
        /// </summary>
        /// <typeparam name="TFeed">Type of feed for the service.</typeparam>
        /// <typeparam name="TEntry">Type of feed entry for the service.</typeparam>
        /// <param name="entry">The atom entry object containing the data to submit.</param>
        /// <param name="applicationName">The name of the application which invokes this method.</param>
        /// <returns>A new atom entry containing some additional data like id, published date etc.</returns>
        protected virtual TEntry SubmitNewEntry<TFeed, TEntry>(TEntry entry, string applicationName)
            where TFeed : GFeed<TEntry>, new()
            where TEntry : GEntry, new()
        {
            if (String.IsNullOrEmpty(GUrl))
                throw new ArgumentNullException(GUrl,
                                                "Feed Url is not provided. Please provide Feed url first through GUrl Property.");

            var request = WebRequest.Create(GUrl) as HttpWebRequest;

            if (request != null)
            {
                var serializer = new XObjectSerializer<TEntry>();
                string xml = serializer.StringSerialize(entry);
                long length = xml.ToCharArray().LongLength;

                var gc = new GDataCredentials(GUserName, GPassword)
                             {
                                 AccountType = "GOOGLE"
                             };
                string header = GAuthManager.RequestClientLoginHeader(gc, "code", applicationName, true,
                                                                      GoogleAuthentication.URI_HANDLER, ProxySettings);
                if (ProxySettings != null)
                    request.Proxy = ProxySettings;
                request.Method = "POST";
                request.Credentials = new NetworkCredential
                                          {
                                              UserName = GUserName,
                                              Password = GPassword
                                          };
                request.ContentType = "application/atom+xml";
                request.ContentLength = length;
                request.Headers.Add(header);

                var sw = new StreamWriter(request.GetRequestStream());
                sw.Write(xml);
                sw.Close();

                var webData = new GWebAsyncData<TFeed, TEntry>
                                  {Request = request, ResetEvent = new ManualResetEvent(false)};
                IAsyncResult result = request.BeginGetResponse(AsyncManager<TFeed, TEntry>.AsyncSubmitNewEntry, webData);

                ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.WaitOne();
                return ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).EntryResult;
            }
            throw new OutOfMemoryException("Unable to create new WebRequest");
        }

        /// <summary>
        /// Queries a particular Google service using query parameters.
        /// </summary>
        /// <typeparam name="TFeed">Type of feed for the service.</typeparam>
        /// <typeparam name="TEntry">Type of feed entry for the service.</typeparam>
        /// <param name="query"><seealso cref="IGQuery"/> object, filled with data.</param>
        /// <returns>An atom feed for the particular Google service.</returns>
        protected virtual TFeed Query<TFeed, TEntry>(IGQuery query)
            where TFeed : GFeed<TEntry>, new()
            where TEntry : GEntry, new()
        {
            if (String.IsNullOrEmpty(GUrl))
                throw new ArgumentNullException(GUrl,
                                                "Feed Url is not provided. Please provide Feed url first through GUrl Property.");

            string queryUrl = GUrl + "?" + query.GetQueryUrl();
            var queryUri = new Uri(queryUrl);
            var request = WebRequest.Create(queryUri) as HttpWebRequest;

            if (request != null)
            {
                if (ProxySettings != null)
                    request.Proxy = ProxySettings;
                request.Credentials = new NetworkCredential(GUserName, GPassword);
                var webData = new GWebAsyncData<TFeed, TEntry>
                                  {Request = request, ResetEvent = new ManualResetEvent(false)};
                IAsyncResult result = request.BeginGetResponse(AsyncManager<TFeed, TEntry>.AsyncDownloadAllFeeds,
                                                               webData);
                ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.WaitOne();

                return ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).FeedResult;
            }
            throw new OutOfMemoryException("Unable to create new WebRequest");
        }
    }
}