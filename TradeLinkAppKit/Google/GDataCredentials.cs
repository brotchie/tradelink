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
using System.Net;

namespace GCore.Authentication
{
    /// <summary>
    /// A class to store Google account credentials.
    /// </summary>
    public class GDataCredentials
    {
        /// <summary>
        /// Instantiate a new <see cref="GDataCredentials"/> object.
        /// </summary>
        /// <param name="username">The username to use.</param>
        /// <param name="password">The password to use.</param>
        public GDataCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            AccountType = GoogleAuthentication.ACCOUNT_TYPE_DEFAULT;
        }

        /// <summary>
        /// Instantiate a new <see cref="GDataCredentials"/> object.
        /// </summary>
        /// <param name="clientToken">The client login token to use.</param>
        public GDataCredentials(string clientToken)
        {
            ClientToken = clientToken;
        }

        /// <summary>Gets and sets the username used for authentication</summary> 
        /// <returns> </returns>
        public string Username { get; set; }

        /// <summary>Gets and sets the type of Account used</summary> 
        /// <returns> </returns>
        public string AccountType { get; set; }

        /// <summary>Gets and sets captcha token in case you need to handle catpcha responses for this account</summary> 
        /// <returns> </returns>
        public string CaptchaToken { get; set; }

        /// <summary>Gets and sets captcha answer in case you need to handle catpcha responses for this account</summary> 
        /// <returns> </returns>
        public string CaptchaAnswer { get; set; }

        /// <summary>Gets and sets the Google account password.</summary> 
        /// <returns> </returns>
        public string Password { internal get; set; }


        /// <summary>
        /// Gets and sets the stored client token
        /// </summary>
        /// <returns></returns>
        public string ClientToken { get; set; }

        /// <summary>
        /// Returns a Windows conforming NetworkCredential 
        /// </summary>
        public ICredentials NetworkCredential
        {
            get { return new NetworkCredential(Username, Password); }
        }
    }
}