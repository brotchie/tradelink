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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Serialization.Xml;
using System.Text;
using System.Threading;

namespace GCore
{
    /// <summary>
    /// Asynchronous operations manager class. For internal use only.
    /// </summary>
    /// <typeparam name="TFeed">Type of service feed.</typeparam>
    /// <typeparam name="TEntry">Type of service feed entry.</typeparam>
    internal class AsyncManager<TFeed, TEntry>
        where TFeed : GFeed<TEntry>, new()
        where TEntry : GEntry, new()
    {

        /// <summary>
        /// Downloads all feed asynchronously from a Google service.
        /// </summary>
        /// <param name="result"><see cref="System.IAsyncResult"/> object.</param>
        internal static void AsyncDownloadAllFeeds(IAsyncResult result)
        {
            try
            {
                var request = ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).Request as HttpWebRequest;
                if (request != null)
                {
                    WebResponse response = request.EndGetResponse(result);

                    Stream stream = response.GetResponseStream();
                    var serializer = new XObjectSerializer<TFeed>();

                    // used to build entire input 
                    var sb = new StringBuilder();

                    // used on each read operation 
                    var buffer = new byte[8192];
                    int count;

                    do
                    {
                        // fill the buffer with data 
                        count = stream.Read(buffer, 0, buffer.Length);

                        // make sure we read some data 
                        if (count != 0)
                        {
                            // translate from bytes to ASCII text 
                            string tempString = Encoding.UTF8.GetString(buffer, 0, count);

                            // continue building the string 
                            sb.Append(tempString);
                        }
                    } while (count > 0); // any more data to read? 

                    Trace.WriteLine(sb.ToString());
                    ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).FeedResult = serializer.Deserialize(sb.ToString());

                    stream.Close();
                    response.Close();
                    ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.Set();
                }
            }
            catch (Exception exception)
            {
                ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.Set();
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously submits new feed entry to Google service.
        /// </summary>
        /// <param name="result"><see cref="System.IAsyncResult"/> object.</param>
        internal static void AsyncSubmitNewEntry(IAsyncResult result)
        {
            try
            {
                var request = ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).Request as HttpWebRequest;
                if (request != null)
                {
                    WebResponse response = request.EndGetResponse(result);

                    Stream stream = response.GetResponseStream();
                    var serializer = new XObjectSerializer<TEntry>();

                    // used to build entire input 
                    var sb = new StringBuilder();

                    // used on each read operation 
                    var buffer = new byte[8192];
                    int count;

                    do
                    {
                        // fill the buffer with data 
                        count = stream.Read(buffer, 0, buffer.Length);

                        // make sure we read some data 
                        if (count != 0)
                        {
                            // translate from bytes to ASCII text 
                            string tempString = Encoding.UTF8.GetString(buffer, 0, count);

                            // continue building the string 
                            sb.Append(tempString);
                        }
                    } while (count > 0); // any more data to read? 

                    Trace.WriteLine(sb.ToString());
                    ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).EntryResult =
                        serializer.Deserialize(sb.ToString());

                    stream.Close();
                    response.Close();
                    ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.Set();
                }
            }
            catch (Exception)
            {
                ((GWebAsyncData<TFeed, TEntry>) result.AsyncState).ResetEvent.Set();
                throw;
            }
        }
    }

    /// <summary>
    /// Intermediate class to hold the state and data for asynchronous operations.
    /// For internal use only.
    /// </summary>
    /// <typeparam name="TFeed">Type of service feed.</typeparam>
    /// <typeparam name="TEntry">Type of service feed entry.</typeparam>
    internal class GWebAsyncData<TFeed, TEntry>
        where TFeed : GFeed<TEntry>, new()
        where TEntry : GEntry, new()
    {
        /// <summary>
        /// Gets and sets the <see cref="System.Net.WebRequest"/> object for the http request.
        /// </summary>
        internal WebRequest Request { get; set; }
        /// <summary>
        /// Gets and sets the service feed obtained after async operation.
        /// </summary>
        internal TFeed FeedResult { get; set; }
        /// <summary>
        /// Gets and sets the service feed entry obtained after async operation.
        /// </summary>
        internal TEntry EntryResult { get; set; }
        /// <summary>
        /// Gets and sets <seealso cref="System.Threading.ManualResetEvent"/> object for wait during async operations.
        /// </summary>
        internal ManualResetEvent ResetEvent { get; set; }
    }
}