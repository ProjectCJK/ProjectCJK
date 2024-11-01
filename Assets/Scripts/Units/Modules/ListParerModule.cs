using System;
using System.Collections.Generic;
using ScriptableObjects.Scripts.Items;
using UnityEngine;

namespace Units.Modules
{
    public static class ListParerModule
    {
        public static Dictionary<TKey, TValue> ConvertListToDictionary<T, TKey, TValue>(List<T> items, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();

            foreach (T item in items)
            {
                var key = keySelector(item);
                var value = valueSelector(item);
                dictionary.TryAdd(key, value);
            }

            return dictionary;
        }
    }
}