using System;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public abstract class BaseEntityCudLogic<TEntity> : BaseBusinessLogic where TEntity : class, IEntity<Guid>
    {
        protected BaseEntityCudLogic(
            IWriteOnlyRepository<TEntity> rootRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            RootRepository = rootRepository;
            ThunderCqrs = thunderCqrs;
        }

        protected IWriteOnlyRepository<TEntity> RootRepository { get; set; }

        protected IThunderCqrs ThunderCqrs { get; set; }
    }
}
