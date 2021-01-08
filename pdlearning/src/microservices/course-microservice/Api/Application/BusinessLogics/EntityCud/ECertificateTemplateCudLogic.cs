using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Events;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class ECertificateTemplateCudLogic : BaseEntityCudLogic<ECertificateTemplate>
    {
        public ECertificateTemplateCudLogic(
            IWriteOnlyRepository<ECertificateTemplate> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(ECertificateTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new ECertificateTemplateChangeEvent(entity, ECertificateTemplateChangeType.Created),
                cancellationToken);
        }

        public async Task InsertMany(List<ECertificateTemplate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new ECertificateTemplateChangeEvent(x, ECertificateTemplateChangeType.Created)),
                cancellationToken);
        }

        public async Task Update(ECertificateTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new ECertificateTemplateChangeEvent(entity, ECertificateTemplateChangeType.Updated),
                cancellationToken);
        }

        public async Task UpdateMany(List<ECertificateTemplate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new ECertificateTemplateChangeEvent(x, ECertificateTemplateChangeType.Updated)),
                cancellationToken);
        }

        public async Task Delete(ECertificateTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new ECertificateTemplateChangeEvent(entity, ECertificateTemplateChangeType.Deleted),
                cancellationToken);
        }

        public async Task DeleteMany(List<ECertificateTemplate> entities, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteManyAsync(entities);

            await ThunderCqrs.SendEvents(
                entities.Select(x => new ECertificateTemplateChangeEvent(x, ECertificateTemplateChangeType.Deleted)),
                cancellationToken);
        }
    }
}
