using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Common.Extensions
{
    public static class AccessRightExtensions
    {
        public static IQueryable<T> CombineWithAccessRight<T>(this IQueryable<T> query, IRepository<T> repository, IRepository<AccessRight> accessRightRepository, Guid currentUserId)
            where T : class, IVersioningFields, IEntity
        {
            var accessableQuery = accessRightRepository.GetAll().Where(t => t.UserId == currentUserId).Select(x => x.ObjectId);
            query = query
                .Union(repository
                .GetAll()
                .Join(
                    accessableQuery,
                    c => c.OriginalObjectId == Guid.Empty ? c.Id : c.OriginalObjectId,
                    p => p,
                    (c, p) => c))
                .Distinct();
            return query;
        }
    }
}
