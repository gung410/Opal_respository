using System;
using System.Linq;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Common.Extensions
{
    public static class AccessRightExtensions
    {
        public static IQueryable<T> CombineWithAccessRight<T>(this IQueryable<T> query, IRepository<T> repository, IRepository<AccessRight> accessRightRepository, Guid currentUserId)
            where T : class, IVersioningFields, IEntity
        {
            var accessibleQuery = accessRightRepository
                .GetAll()
                .Where(t => t.UserId == currentUserId)
                .Select(x => x.ObjectId);

            query = query
                .Union(repository
                .GetAll()
                .Join(
                    accessibleQuery,
                    c => c.OriginalObjectId == Guid.Empty ? c.Id : c.OriginalObjectId,
                    p => p,
                    (c, p) => c))
                .Distinct();

            return query;
        }
    }
}
