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
using System.Collections;
using System.Collections.Generic;

namespace GCore.Authentication
{
    /// <summary>
    /// A collection class to store string tokens.
    /// </summary>
    public class TokenCollection : IEnumerable
    {
        private readonly string[] _elements;

        /// <summary>Constructor, takes a string and a delimiter set</summary> 
        public TokenCollection(string source, char[] delimiters)
        {
            if (source != null)
            {
                _elements = source.Split(delimiters);
            }
        }

        /// <summary>Constructor, takes a string and a delimiter set</summary> 
        public TokenCollection(string source, char delimiter,
                               bool seperateLines, int resultsPerLine)
        {
            if (source != null)
            {
                if (seperateLines)
                {
                    // first split the source into a line array
                    string[] lines = source.Split(new[] {'\n'});
                    int size = lines.Length*resultsPerLine;
                    _elements = new string[size];
                    size = 0;
                    foreach (String s in lines)
                    {
                        // do not use Split(char,int) as that one
                        // does not exist on .NET CF
                        string[] temp = s.Split(delimiter);
                        int counter = temp.Length < resultsPerLine ? temp.Length : resultsPerLine;

                        for (int i = 0; i < counter; i++)
                        {
                            _elements[size++] = temp[i];
                        }
                        for (int i = resultsPerLine; i < temp.Length; i++)
                        {
                            _elements[size - 1] += delimiter + temp[i];
                        }
                    }
                }
                else
                {
                    string[] temp = source.Split(delimiter);
                    resultsPerLine = temp.Length < resultsPerLine ? temp.Length : resultsPerLine;
                    _elements = new string[resultsPerLine];

                    for (int i = 0; i < resultsPerLine; i++)
                    {
                        _elements[i] = temp[i];
                    }
                    for (int i = resultsPerLine; i < temp.Length; i++)
                    {
                        _elements[resultsPerLine - 1] += delimiter + temp[i];
                    }
                }
            }
        }

        #region IEnumerable Members

        /// <summary>IEnumerable Interface Implementation</summary> 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TokenEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Creates a dictionary of tokens based on this tokencollection
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> CreateDictionary()
        {
            var dict = new Dictionary<string, string>();

            for (int i = 0; i < _elements.Length; i += 2)
            {
                string key = _elements[i];
                string val = _elements[i + 1];
                dict.Add(key, val);
            }
            return dict;
        }

        /// <summary>IEnumerable Interface Implementation, for the noninterface</summary> 
        public TokenEnumerator GetEnumerator() // non-IEnumerable version
        {
            return new TokenEnumerator(this);
        }

        #region Nested type: TokenEnumerator

        /// <summary>Inner class implements IEnumerator interface</summary> 
        public class TokenEnumerator : IEnumerator
        {
            private readonly TokenCollection _tokens;
            private int _position = -1;

            /// <summary>Standard constructor</summary> 
            public TokenEnumerator(TokenCollection tokens)
            {
                _tokens = tokens;
            }

            /// <summary>Current implementation, non interface, type-safe</summary> 
            public string Current
            {
                get { return _tokens._elements != null ? _tokens._elements[_position] : null; }
            }

            #region IEnumerator Members

            /// <summary>IEnumerable.MoveNext implementation.</summary> 
            public bool MoveNext()
            {
                if (_tokens._elements != null && _position < _tokens._elements.Length - 1)
                {
                    _position++;
                    return true;
                }
                return false;
            }

            /// <summary>IEnumerable.Reset implementation.</summary> 
            public void Reset()
            {
                _position = -1;
            }

            /// <summary>Current implementation, interface, not type-safe</summary> 
            object IEnumerator.Current
            {
                get { return _tokens._elements != null ? _tokens._elements[_position] : null; }
            }

            #endregion
        }

        #endregion
    }
}