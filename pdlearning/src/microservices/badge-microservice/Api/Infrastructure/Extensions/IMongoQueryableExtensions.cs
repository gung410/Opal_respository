using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Badge.Infrastructure.Extensions
{
    public static class IMongoQueryableExtensions
    {
        public static IMongoQueryable<T> FindFullTextIndex<T>(this IMongoQueryable<T> query, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            var filter = Builders<T>.Filter.Text(search);
            return query.Where(_ => filter.Inject());
        }

        public static IMongoQueryable<T> Paging<T>(this IMongoQueryable<T> query, PagedResultRequestDto pageInfo)
        {
            return query.Skip(pageInfo.SkipCount).Take(pageInfo.MaxResultCount);
        }
    }
}
