//=======================================================================
// Copyright (C) 2010-2013 William Hallahan
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//=======================================================================

﻿//======================================================================
// Generic Class: SparseArray
// Author: Bill Hallahan
// Date: April 9, 2010
//======================================================================

using System;
using System.Collections.Generic;

namespace SparseCollections
{
    /// <summary>
    /// This class implements a sparse array.
    /// </summary>
    /// <typeparam name="TKey">The key type used to index the array items</typeparam>
    /// <typeparam name="TValue">The type of the array values</typeparam>
    [Serializable]
    public class SparseArray<TKey, TValue>  : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        private Dictionary<TKey, TValue> m_dictionary;

        /// <summary>
        /// This property stores the default value that is returned if the key doesn't exist.
        /// </summary>
        public TValue DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get the count of items in the sparse array.
        /// </summary>
        public int Count
        {
            get
            {
                return m_dictionary.Count;
            }

            private set
            {
            }
        }

        #region Constructors
        /// <summary>
        /// Constructor - creates an empty sparse array instance.
        /// </summary>
        public SparseArray()
        {
            m_dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultValue">A default value to return if the key is not present.</param>
        public SparseArray(TValue defaultValue)
        {
            m_dictionary = new Dictionary<TKey, TValue>();
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="sparseArray">The sparse array instance to be copied</param>
        public SparseArray(SparseArray<TKey, TValue> sparseArray)
        {
            m_dictionary = new Dictionary<TKey, TValue>();
            Initialize(sparseArray);
            DefaultValue = sparseArray.DefaultValue;
        }

        #endregion

        /// <summary>
        /// Method to copy the data in another SparseArray instance to this instance.
        /// </summary>
        /// <param name="sparse2DMatrix">An instance of the SparseArray class.</param>
        private void Initialize(SparseArray<TKey, TValue> sparseArray)
        {
            m_dictionary.Clear();

            // Copy each key value pair to the new dictionary.
            foreach (KeyValuePair<TKey, TValue> pair in sparseArray)
            {
                m_dictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Method to copy the data in this SparseArray instance to another instance.
        /// </summary>
        /// <param name="sparse2DMatrix">An instance of the SparseArray class.</param>
        public void CopyTo(SparseArray<TKey, TValue> sparseArray)
        {
            sparseArray.m_dictionary.Clear();

            // Copy each key value pair to the new dictionary.
            foreach (KeyValuePair<TKey, TValue> pair in m_dictionary)
            {
                sparseArray.m_dictionary.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Property []
        /// </summary>
        /// <param name="key">The key used to index the value</param>
        /// <returns>The 'get' property returns the value at the current key</returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                if (!m_dictionary.TryGetValue(key, out value))
                {
                    value = DefaultValue;
                }

                return value;
            }

            set
            {
                m_dictionary[key] = value;
            }
        }

        /// <summary>
        /// Determines whether this sparse array contains the specified key.
        /// </summary>
        /// <param name="key">A key</param>
        /// <returns>Returns the value 'true' if and only if the key exists in this sparse array</returns>
        public bool ContainsKey(TKey key)
        {
            return m_dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether this sparse array contains the specified value.
        /// </summary>
        /// <param name="key">A value</param>
        /// <returns>Returns the value 'true' if and only if the value exists in this sparse array</returns>
        public bool ContainsValue(TValue value)
        {
            return m_dictionary.ContainsValue(value);
        }

        /// <summary>
        /// Gets the value for the associated key.
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        /// <param name="value">An out parameter that contains the value if the key exists</param>
        /// <returns>Returns the value 'true' if and only if the key exists in this sparse array</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes the value with the specified key from this sparse array instance.
        /// </summary>
        /// <param name="key">The key of the element to remove</param>
        /// <returns>The value 'true' if and only if the element is successfully found and removed.</returns>
        public bool Remove(TKey key)
        {
            return m_dictionary.Remove(key);
        }

        /// <summary>
        /// Method to clear all values in the sparse array.
        /// </summary>
        public void Clear()
        {
            m_dictionary.Clear();
        }

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        /// The Generic IEnumerator<> GetEnumerator method
        /// </summary>
        /// <returns>An enumerator to iterate over all key-value pairs in this sparse array</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.m_dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// The non-generic IEnumerator<> GetEnumerator method
        /// </summary>
        /// <returns>An enumerator to iterate over all key-value pairs in this sparse array</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_dictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// Property to get the key-value pair at the current enumerator position.
        /// </summary>
        public TValue Current
        {
            get
            {
                return m_dictionary.Values.GetEnumerator().Current;
            }
        }

        /// <summary>
        /// Method to move to the next enumerator position.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return m_dictionary.Values.GetEnumerator().MoveNext();
        }

        #endregion
    }
}
