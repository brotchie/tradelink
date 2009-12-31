#region Copyright Notice
// 
// 
// Copyright (c) 2006-2008 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// 
#endregion
using System;
using System.IO;
using System.Net;
using System.Text;

namespace GCore.Authentication
{
    /// <summary>
    /// Google account authentication manager class.
    /// </summary>
    public class GAuthManager
    {
        /// <summary>
        /// Requests a client token header for a new entry submission.
        /// </summary>
        /// <param name="gc"><see cref="GDataCredentials"/> object conating the data for Google account.</param>
        /// <param name="serviceName">Google service name.</param>
        /// <param name="applicationName">Application name invoking the Google service.</param>
        /// <param name="fUseKeepAlive">Use keep alive connection.</param>
        /// <param name="clientLoginHandler">Client login url.</param>
        /// <param name="proxy">Web proxy to be used if behind a firewall.</param>
        /// <returns>A header string containing the client token required for new entry submission.</returns>
        public static string RequestClientLoginHeader(GDataCredentials gc,
                                                      string serviceName,
                                                      string applicationName,
                                                      bool fUseKeepAlive,
                                                      string clientLoginHandler,
                                                      WebProxy proxy)
        {
            gc.ClientToken = QueryClientLoginToken(gc, serviceName, applicationName, fUseKeepAlive,
                                                   new Uri(clientLoginHandler), proxy);
            string strHeader = GoogleAuthentication.HEADER + gc.ClientToken;
            return strHeader;
        }

        private static string QueryClientLoginToken(GDataCredentials gc,
                                                    string serviceName,
                                                    string applicationName,
                                                    bool fUseKeepAlive,
                                                    Uri clientLoginHandler,
                                                    IWebProxy proxy)
        {
            if (gc == null)
            {
                throw new ArgumentNullException("gc", "No credentials supplied");
            }

            HttpWebRequest authRequest = WebRequest.Create(clientLoginHandler) as HttpWebRequest;

            if (authRequest != null)
            {
                if (proxy != null)
                    authRequest.Proxy = proxy;

                authRequest.KeepAlive = fUseKeepAlive;

                string accountType = GoogleAuthentication.ACCOUNT_TYPE;
                if (!String.IsNullOrEmpty(gc.AccountType))
                {
                    accountType += gc.AccountType;
                }
                else
                {
                    accountType += GoogleAuthentication.ACCOUNT_TYPE_DEFAULT;
                }

                WebResponse authResponse;

                string authToken = null;
                try
                {
                    authRequest.ContentType = "application/x-www-form-urlencoded";
                    authRequest.Method = "POST";
                    var encoder = new ASCIIEncoding();

                    string user = gc.Username ?? "";
                    string pwd = gc.Password ?? "";

                    // now enter the data in the stream
                    string postData = GoogleAuthentication.EMAIL + "=" + Utility.UriEncodeUnsafe(user) + "&";
                    postData += GoogleAuthentication.PASSWORD + "=" + Utility.UriEncodeUnsafe(pwd) + "&";
                    postData += GoogleAuthentication.SOURCE + "=" + Utility.UriEncodeUnsafe(applicationName) + "&";
                    postData += GoogleAuthentication.SERVICE + "=" + Utility.UriEncodeUnsafe(serviceName) + "&";
                    if (gc.CaptchaAnswer != null)
                    {
                        postData += GoogleAuthentication.CAPTCHA_ANSWER + "=" +
                                    Utility.UriEncodeUnsafe(gc.CaptchaAnswer) +
                                    "&";
                    }
                    if (gc.CaptchaToken != null)
                    {
                        postData += GoogleAuthentication.CAPTCHA_TOKEN + "=" + Utility.UriEncodeUnsafe(gc.CaptchaToken) +
                                    "&";
                    }
                    postData += accountType;

                    byte[] encodedData = encoder.GetBytes(postData);
                    authRequest.ContentLength = encodedData.Length;

                    Stream requestStream = authRequest.GetRequestStream();
                    requestStream.Write(encodedData, 0, encodedData.Length);
                    requestStream.Close();
                    authResponse = authRequest.GetResponse();
                }
                catch (WebException e)
                {
                    authResponse = e.Response;
                }
                var response = authResponse as HttpWebResponse;
                if (response != null)
                {
                    // check the content type, it must be text
                    if (!response.ContentType.StartsWith("text"))
                    {
                        throw new Exception(
                            "Execution of authentication request returned unexpected content type: " +
                            response.ContentType);
                    }
                    TokenCollection tokens = Utility.ParseStreamInTokenCollection(response.GetResponseStream());
                    authToken = Utility.FindToken(tokens, GoogleAuthentication.AUTH_TOKEN);

                    if (authToken == null)
                    {
                        throw new Exception("Authentication failed. Try again later.");
                    }
                    // failsafe. if getAuthException did not catch an error...
                    var code = (int) response.StatusCode;
                    if (code != 200)
                    {
                        throw new Exception("Execution of authentication request returned unexpected result: " +
                                            code);
                    }
                }
                if (authResponse != null)
                {
                    authResponse.Close();
                }
                return authToken;
            }
            throw new OutOfMemoryException("Unable to create WebRequest.");
        }
    }
}