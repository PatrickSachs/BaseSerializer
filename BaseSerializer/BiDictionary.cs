using System;
using System.Collections;
using System.Collections.Generic;

namespace BaseSerializer
{
    /// <summary>
    /// A simple & quick'n'dirty bi-directional dictionary.
    /// </summary>
    /// <typeparam name="T1">First type.</typeparam>
    /// <typeparam name="T2">Second type.</typeparam>
    internal class BiDictionary<T1, T2> : IDictionary<T1, T2>
    {
        private readonly Dictionary<T1, T2> _dic1 = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _dic2 = new Dictionary<T2, T1>();

        public void Add(T1 v1, T2 v2)
        {
            if (_dic1.ContainsKey(v1) || _dic2.ContainsKey(v2))
            {
                throw new ArgumentException("A key already exists in either dictionaries.");
            }

            _dic1.Add(v1, v2);
            _dic2.Add(v2, v1);
        }

        public bool ContainsKey(T1 key)
        {
            return _dic1.ContainsKey(key);
        }

        public bool Remove(T1 key)
        {
            if (_dic1.TryGetValue(key, out T2 value))
            {
                return _dic1.Remove(key) && _dic2.Remove(value);
            }

            return false;
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return _dic1.TryGetValue(key, out value);
        }

        public bool TryGetValue(T2 key, out T1 value)
        {
            return _dic2.TryGetValue(key, out value);
        }

        public T2 this[T1 key]
        {
            get => _dic1[key];
            set
            {
                // Remove previously assigned value from dic2 if any.
                if (_dic1.TryGetValue(key, out T2 value2))
                {
                    _dic2.Remove(value2);
                }

                _dic1[key] = value;
                _dic2[value] = key;
            }
        }

        public ICollection<T1> Keys => _dic1.Keys;
        public ICollection<T2> Values => _dic2.Keys;

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _dic1.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dic1.Clear();
            _dic2.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            if (_dic1.TryGetValue(item.Key, out T2 value))
            {
                return value.Equals(item.Value);
            }

            return false;
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            ((IDictionary<T1, T2>) _dic1).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            if (_dic1.TryGetValue(item.Key, out T2 value) && value.Equals(item.Value))
            {
                return _dic1.Remove(item.Key) && _dic2.Remove(value);
            }

            return false;
        }

        public int Count => _dic1.Count;
        public bool IsReadOnly => false;
    }
}
