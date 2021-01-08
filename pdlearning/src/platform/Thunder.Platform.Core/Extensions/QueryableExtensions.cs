using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Thunder.Platform.Core.Application.Dtos;

namespace Thunder.Platform.Core.Extensions
{
    /// <summary>
    /// Some useful extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Skip(skipCount).Take(maxResultCount);
        }

        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, IPagedResultRequest pagedResultRequest)
        {
            return query.PageBy(pagedResultRequest.SkipCount, pagedResultRequest.MaxResultCount);
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Func<Expression<Func<T, bool>>> predicateFn)
        {
            return condition
                ? query.Where(predicateFn())
                : query;
        }

        public static IQueryable<T> WhereIfElse<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> elsePredicate)
        {
            return condition
                ? query.Where(predicate)
                : query.Where(elsePredicate);
        }

        public static IQueryable<T> WhereIfElse<T>(this IQueryable<T> query, bool condition, Func<Expression<Func<T, bool>>> predicateFn, Func<Expression<Func<T, bool>>> elsePredicateFn)
        {
            return condition
                ? query.Where(predicateFn())
                : query.Where(elsePredicateFn());
        }

        public static IQueryable<TOuter> JoinIf<TOuter, TInner, TKey>(
            this IQueryable<TOuter> outer,
            bool condition,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector)
        {
            if (condition)
            {
                return outer.Join(inner, outerKeySelector, innerKeySelector, (a, b) => a);
            }

            return outer;
        }
    }
}
