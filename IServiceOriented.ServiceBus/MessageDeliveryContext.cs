﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IServiceOriented.ServiceBus.Collections;

using System.Runtime.Serialization;

namespace IServiceOriented.ServiceBus
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), CollectionDataContract]
    [KnownType("GetKnownTypes")]
    [Serializable]
    public class MessageDeliveryContext : IReadOnlyDictionary<string,object>
    {
        public MessageDeliveryContext()
        {
        }

        public MessageDeliveryContext(IDictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                Add(pair);
            }
        }

        public MessageDeliveryContext(IReadOnlyDictionary<string, object> dictionary)            
        {
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                Add(pair);
            }
        }

        public MessageDeliveryContext(KeyValuePair<string, object>[] pairs)            
        {
            foreach (KeyValuePair<string, object> pair in pairs)
            {
                Add(pair);
            }
        }

        private void Add(KeyValuePair<string, object> pair)
        {
            _dictionary.Add(pair);
        }

        IDictionary<string, object> _dictionary = new Dictionary<string, object>();

        #region IReadOnlyDictionary<string,object> Members

        public IEnumerable<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public IEnumerable<object> Values
        {
            get { return _dictionary.Values; }
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<string, object> value)
        {
            return _dictionary.Contains(value);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        public static Type[] GetKnownTypes()
        {
            return MessageDelivery.GetKnownTypes();
        }
    }
}
