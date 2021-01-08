using System.Diagnostics.CodeAnalysis;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Application.SharedQueries.Abstractions
{
    public abstract class BaseSharedQuery : ISharedQuery
    {
        [return: NotNull]
        [JetBrains.Annotations.NotNull]
        protected TEntity EnsureEntityFound<TEntity>([MaybeNull] TEntity entity)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException();
            }

            return entity;
        }
    }
}
