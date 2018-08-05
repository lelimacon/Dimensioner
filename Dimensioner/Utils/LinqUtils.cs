using System;
using System.Collections.Generic;
using System.Linq;

namespace Dimensioner.Utils
{
    public static class LinqUtils
    {
        private class DistinctElementComparer<T, TE> : IEqualityComparer<T>
        {
            private readonly Func<T, TE> _propertyGetter;

            public DistinctElementComparer(Func<T, TE> keySelector)
            {
                _propertyGetter = keySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(_propertyGetter(x), _propertyGetter(y));
            }

            public int GetHashCode(T obj)
            {
                return _propertyGetter(obj)?.GetHashCode() ?? 0;
            }
        }

        /// <summary>
        ///     Returns distinct elements from a sequence by using a specified key selector
        ///     to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to filter.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>An IEnumerable without duplicate elements according to a key.</returns>
        /// <exception cref="ArgumentNullException">Source or keySelector is null.</exception>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.Distinct(new DistinctElementComparer<TSource, TKey>(keySelector));
        }
    }
}
