using System;
using System.Collections.Generic;
using System.Linq;

namespace Thunder.Platform.Core.Extensions
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// This is extension method which is ONLY used for data in memory.
        /// PLEASE DO NOT USE FOR QUERYABLE.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TResult">Type of result.</typeparam>
        /// <param name="source">source.</param>
        /// <param name="selector">selector predicate.</param>
        /// <returns>List of new result.</returns>
        public static List<TResult> SelectList<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).ToList();
        }

        public static List<TResult> SelectListDistinct<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector).Distinct().ToList();
        }
    }
}
