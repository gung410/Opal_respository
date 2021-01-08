using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;

namespace cxOrganization.Domain.Extensions
{
    //TODO should move to crosscutting
    public static class LinqExtension
    {
        public static IQueryable<TSource> ApplyOrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> primaryKeySelector, string orderBy = "")
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return query.OrderByDescending(primaryKeySelector);
            try
            {
                query = DynamicQueryableExtensions.OrderBy(query, orderBy);
            }
            catch (ParseException)
            {
                throw;
            }
            return query;
        }
    }
}
