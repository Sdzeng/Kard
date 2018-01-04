using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            source.Add(key, default(TValue));
        }

        public static void AddIf<TKey, TValue>(this IDictionary<TKey, TValue> source, bool condition, TKey key)
        {
            if (condition)
            {
                source.Add(key, default(TValue));
            }
        }

        public static void AddIf<TKey, TValue>(this IDictionary<TKey, TValue> source, bool condition, TKey key, TValue value)
        {
            if (condition)
            {
                source.Add(key, value);
            }
        }

        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var newSource = new Dictionary<TKey, TValue>();
            foreach (var p in source)
            {
                newSource.Add(p.Key, p.Value);
            }
            return newSource;
        }
    }
}
