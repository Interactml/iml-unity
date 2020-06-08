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
    }

}
