using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.SharedQueries
{
    public abstract class BaseReadShared<TEntity> where TEntity : class, IEntity
    {
        protected BaseReadShared(IReadOnlyRepository<TEntity> readRepository)
        {
            ReadRepository = readRepository;
        }

        protected IReadOnlyRepository<TEntity> ReadRepository { get; set; }
    }
}
