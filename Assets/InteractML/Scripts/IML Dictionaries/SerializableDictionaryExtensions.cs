using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace InteractML
{
    public static class SerializableDictionaryExtensions
    {
        public static bool ContainsValue<TKey, TValue>(this SerializableDictionary<TKey, TValue> dict, TValue value)
        {
            if (dict != null && value != null)
            {
                var values = dict.Values;
                if (values != null && values.Contains(value))
                    return true;
            }
            // If reached here, something is wrong
            return false;
        }

        /// <summary>
        /// Return a value by key. Not very performance friendly
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TKey GetKey<TKey, TValue>(this SerializableDictionary<TKey, TValue> dict, TValue val)
        {
            TKey key = default;
            foreach (KeyValuePair<TKey, TValue> pair in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }

}
