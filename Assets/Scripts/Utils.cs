using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public struct Coordinates
    {
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public struct RL
    {
        public RL(int r, int l)
        {
            R = r;
            L = l;
        }
        public int R { get; set; }
        public int L { get; set; }
    }

    static class Extentions
    {
        public struct IndexedItem2<T>
        {
            public T Element { get; }
            public int X { get; }
            public int Y { get; }
            internal IndexedItem2(T element, int x, int y)
            {
                this.Element = element;
                this.X = x;
                this.Y = y;
            }
        }

        public static IEnumerable<IndexedItem2<T>> WithIndex<T>(this T[,] self)
        {
            if (self == null)
                throw new System.ArgumentNullException(nameof(self));

            for (int x = 0; x < self.GetLength(0); x++)
                for (int y = 0; y < self.GetLength(1); y++)
                    yield return new IndexedItem2<T>(self[x, y], x, y);
        }
    }

    public static class ListExtensions
    {
        public static T Pop<T>(this IList<T> self)
        {
            var result = self[0];
            self.RemoveAt(0);
            return result;
        }
    }
}
