using System.Collections.Generic;

using System;
using System.Linq;

namespace Rougelike
{
    static class ArrayExtensions
    {
        public static int WeightedRandomSelect(this int[] weightTable)
        {
            int totalWeight = weightTable.Sum();
            int value = UnityEngine.Random.Range(1, totalWeight + 1);
            int retrievalIndex = -1;
            for (var i = 0; i < weightTable.Length; ++i)
            {
                if (weightTable[i] >= value)
                {
                    retrievalIndex = i;
                    break;
                }
                value -= weightTable[i];
            }
            return retrievalIndex;
        }
    }

    static class ListExtensions
    {
        public static T Pop<T>(this List<T> self)
        {
            var result = self[0];
            self.RemoveAt(0);
            return result;
        }
    }

    static class IEnumerableExtensions
    {
        public static IEnumerable<X> Map<T, X>(this IEnumerable<T> e, Func<T, X> f)
        {
            foreach (var element in e)
            {
                yield return f(element);
            }
        }
    }
}
