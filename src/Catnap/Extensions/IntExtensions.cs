using System;
using System.Collections.Generic;

namespace Catnap.Extensions
{
    public static class IntExtensions
    {
        public static IEnumerable<T> Times<T>(this int i, Func<int, T> func)
        {
            for (var j = 0; j < i; j++)
            {
                yield return func(j);
            }
        }

        public static void Times(this int i, Action<int> action)
        {
            for (var j = 0; j < i; j++)
            {
                action(j);
            }
        }
    }
}