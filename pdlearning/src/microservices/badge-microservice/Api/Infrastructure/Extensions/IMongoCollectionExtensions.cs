using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.Badge.Infrastructure.Extensions
{
    public static class IMongoCollectionExtensions
    {
        public static IMongoQueryable<TSource> AsQueryableWhere<TSource>(this IMongoCollection<TSource> source, Expression<Func<TSource, bool>> predicates)
        {
            return source.AsQueryable().Where(predicates);
        }

        public static IMongoQueryable<TSource> FindFullTextIndex<TSource>(this IMongoCollection<TSource> source, string searchText)
        {
            return source.AsQueryable().FindFullTextIndex(searchText);
        }

        public static Task<TSource> FirstOrDefaultAsync<TSource>(
            this IMongoCollection<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return source.AsQueryable()
                .Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public static Task<bool> AnyAsync<TSource>(
            this IMongoCollection<TSource> source,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return source.AsQueryableWhere(predicate).AnyAsync(cancellationToken);
        }

        /// <summary>
        /// It just wrap calling ReplaceOneAsync many time into one method.
        /// Please be carefull performance when using this method.
        /// </summary>
        /// <typeparam name="TSource">Type of collection entity.</typeparam>
        /// <param name="source"> Collection entity.</param>
        /// <param name="updatedEntities">updatedEntities.</param>
        /// <returns>Task.</returns>
        public static async Task ReplaceManyAsync<TSource>(this IMongoCollection<TSource> source, IEnumerable<TSource> updatedEntities) where TSource : IHasIdEntity
        {
            // todo: research and apply update many in one query
            foreach (var updatedEntity in updatedEntities)
            {
                await source.ReplaceOneAsync(p => p.Id == updatedEntity.Id, updatedEntity);
            }
        }
    }
}
