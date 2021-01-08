using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Microservice.Badge.Infrastructure.Extensions
{
    public static class IAggregateFluentExtensions
    {
        public static async Task<int> CountAsync<TSource>(this IAggregateFluent<TSource> source, CancellationToken cancellationToken = default)
        {
            var result = await source.Count().FirstOrDefaultAsync(cancellationToken);
            return (int)(result?.Count ?? 0);
        }

        public static IAggregateFluent<TResult> Select<TQuery, TResult>(this IAggregateFluent<TQuery> query, Expression<Func<TQuery, TResult>> mapperModelExpr)
        {
            var projectionDefinition = Builders<TQuery>.Projection.Expression(mapperModelExpr);
            return query.Project(projectionDefinition);
        }
    }
}
