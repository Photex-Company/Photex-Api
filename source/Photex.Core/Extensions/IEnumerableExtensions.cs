using System;
using System.Collections.Generic;

namespace Photex.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IDictionary<TKey, TValue> SafeToDictionary<TKey, TValue, TInput>(
            this IEnumerable<TInput> input,
            Func<TInput, TKey> keySelector,
            Func<TInput, TValue> valueSelector,
            Func<TValue, TValue, TValue> conflictResolver)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var current in input)
            {
                if (result.ContainsKey(keySelector(current)))
                {
                    result[keySelector(current)] = conflictResolver(
                        result[keySelector(current)], valueSelector(current));
                }
                else
                {
                    result[keySelector(current)] = valueSelector(current);
                }
            }

            return result;
        }

        public static IDictionary<TKey, TValue> SafeConcatDictionaries<TKey, TValue>(
            this IDictionary<TKey, TValue> original,
            IDictionary<TKey, TValue> toConcat,
            Func<TValue, TValue, TValue> conflictResolver)
        {
            foreach (var value in toConcat)
            {
                if (!original.ContainsKey(value.Key))
                {
                    original[value.Key] = value.Value;
                }
                else
                {
                    original[value.Key] = conflictResolver(original[value.Key], value.Value);
                }
            }

            return original;
        }
    }
}
