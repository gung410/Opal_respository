using System.Linq;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Versioning.Extensions
{
    public static class RepositotyHasVersionFieldsExtension
    {
        public static IQueryable<TEntity> IgnoreArchivedItems<TEntity>(this IQueryable<TEntity> query)
            where TEntity : class, IEntity, IVersioningFields
        {
            return query.Where(x => x.IsArchived == false);
        }
    }
}
