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
namespace GCore.Authentication
{
    /// <summary>
    /// A class to store some important data for Google account authentication.
    /// </summary>
    public static class GoogleAuthentication
    {
        ///  <summary>Gets and sets the Google account prefix path.</summary>
        public const string ACCOUNT_PREFIX = "/accounts";

        /// <summary>Gets and sets the Google webkey identifier.</summary>
        public const string ACCOUNT_TYPE = "accountType=";

        /// <summary>Gets and sets the default value for the account type.</summary>
        public const string ACCOUNT_TYPE_DEFAULT = "HOSTED_OR_GOOGLE";

        /// <summary>Gets and sets the Google authSub authentication token.</summary>
        public const string AUTH_SUB_TOKEN = "Token";

        /// <summary>Gets and sets the Google client authentication token.</summary>
        public const string AUTH_TOKEN = "Auth";

        /// <summary>Gets and sets the captcha url token.</summary>
        public const string CAPTCHA_ANSWER = "logincaptcha";

        /// <summary>Gets and sets the default value for the account type.</summary>
        public const string CAPTCHA_TOKEN = "logintoken";

        /// <summary>Gets and sets the default authentication domain.</summary>
        public const string DEFAULT_DOMAIN = "www.google.com";

        ///  <summary>Gets and sets the Protocol.</summary>
        public const string DEFAULT_PROTOCOL = "https";

        /// <summary>Gets and sets the Google client authentication email.</summary>
        public const string EMAIL = "Email";

        /// <summary>Gets and sets the Google client header.</summary>
        public const string HEADER = "Authorization: GoogleLogin auth=";

        /// <summary>Gets and sets the Google client authentication LSID.</summary>
        public const string LSID = "LSID";

        /// <summary>Gets and sets the Google method override header.</summary>
        public const string OVERRIDE = "X-HTTP-Method-OVERRIDE";

        /// <summary>Gets and sets the Google client authentication password.</summary>
        public const string PASSWORD = "Passwd";

        /// <summary>Gets and sets the Google client authentication default service constant.</summary>
        public const string SERVICE = "service";

        /// <summary>Gets and sets the Google client authentication source constant.</summary>
        public const string SOURCE = "source";

        /// <summary>Gets and sets the Google client authentication SSID.</summary>
        public const string SSID = "SSID";

        /// <summary>Gets and sets the Google client authentication handler.</summary>
        public const string URI_HANDLER = "https://www.google.com/accounts/ClientLogin";

        /// <summary>Gets and sets the Google webkey identifier.</summary>
        public const string WEB_KEY = "X-Google-Key: key=";

        /// <summary>Gets and sets the Google YouTube client identifier.</summary>
        public const string YOU_TUBE_CLIENT_ID = "X-GData-Client:";

        /// <summary>Gets and sets the Google YouTube developer identifier.</summary>
        public const string YOU_TUBE_DEV_KEY = "X-GData-Key: key=";
    }
}