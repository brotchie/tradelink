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
// Change history
// Added ToFriendlyDateString function - Anindya Chatterjee
#endregion
using System;
using System.IO;
using System.Text;
using GCore.Authentication;

namespace GCore
{
    /// <summary>
    /// A general helper utility class for GCore library
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Parses the stream in token collection.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The <see cref="TokenCollection"/> object</returns>
        public static TokenCollection ParseStreamInTokenCollection(Stream inputStream)
        {
            // get the body and parse it
            var encoder = new ASCIIEncoding();
            var readStream = new StreamReader(inputStream, encoder);
            String body = readStream.ReadToEnd();
            readStream.Close();
            // all we are interested is the token, so we break the string in parts
            var tokens = new TokenCollection(body, '=', true, 2);
            return tokens;
        }

        /// <summary>
        /// Finds the specified token in the given <see cref="TokenCollection"/> object
        /// </summary>
        /// <param name="tokens"><see cref="TokenCollection"/> object</param>
        /// <param name="key">Key to find</param>
        /// <returns>The value associated with the key</returns>
        public static string FindToken(TokenCollection tokens, string key)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException("tokens");
            }

            string returnValue = null;
            bool fNextOne = false;

            foreach (string token in tokens)
            {
                if (fNextOne)
                {
                    returnValue = token;
                    break;
                }
                if (key == token)
                {
                    // next one is it
                    fNextOne = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Encodes the given string to uri safe string
        /// </summary>
        /// <param name="content">The string to encode</param>
        /// <returns>The encoded string</returns>
        public static string UriEncodeUnsafe(string content)
        {
            if (content == null)
                return null;

            var returnString = new StringBuilder(256);

            foreach (char ch in content)
            {
                if (ch == ';' ||
                    ch == '/' ||
                    ch == '?' ||
                    ch == ':' ||
                    ch == '@' ||
                    ch == '&' ||
                    ch == '=' ||
                    ch == '+' ||
                    ch == '$' ||
                    ch == ',' ||
                    ch == ' ' ||
                    ch == '\'' ||
                    ch == '"' ||
                    ch == '>' ||
                    ch == '<' ||
                    ch == '#' ||
                    ch == '%')
                {
                    returnString.Append(Uri.HexEscape(ch));
                }
                else
                {
                    returnString.Append(ch);
                }
            }
            return returnString.ToString();
        }

        /// <summary>
        /// Converts a <seealso cref="DateTime"/> object into a string understandable by feed query
        /// </summary>
        /// <param name="date"><seealso cref="DateTime"/> object to format</param>
        /// <returns>Formatted string</returns>
        public static string ToFriendlyDateString(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            int hour = date.Hour;
            int minute = date.Minute;
            int second = date.Second;

            return year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00") + "T" +
                   hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
        }
    }
}