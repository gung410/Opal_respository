using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.BusinessLogic
{
    public abstract class BaseBusinessLogic<TEntity> where TEntity : class, IEntity
    {
        protected BaseBusinessLogic(IThunderCqrs thunderCqrs, IWriteOnlyRepository<TEntity> writeRepository)
        {
            ThunderCqrs = thunderCqrs;
            WriteRepository = writeRepository;
        }

        protected IThunderCqrs ThunderCqrs { get; set; }

        protected IWriteOnlyRepository<TEntity> WriteRepository { get; set; }
    }
}
