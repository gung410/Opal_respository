using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Common.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<Dictionary<Guid, TEntity>> GetDictionaryByIdsAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, IEnumerable<Guid> ids) where TEntity : class, IEntity<Guid>
        {
            return (await repository.GetAllListAsync(p => ids.Contains(p.Id))).ToDictionary(p => p.Id);
        }
    }
}
