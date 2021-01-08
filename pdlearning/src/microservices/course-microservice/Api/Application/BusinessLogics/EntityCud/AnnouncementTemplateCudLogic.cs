using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.Events;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics.EntityCud
{
    public class AnnouncementTemplateCudLogic : BaseEntityCudLogic<AnnouncementTemplate>
    {
        public AnnouncementTemplateCudLogic(
            IWriteOnlyRepository<AnnouncementTemplate> repository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(repository, thunderCqrs, userContext)
        {
        }

        public async Task Insert(AnnouncementTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.InsertAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementTemplateChangeEvent(entity, AnnouncementTemplateChangeType.Created),
                cancellationToken);
        }

        public async Task Update(AnnouncementTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.UpdateAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementTemplateChangeEvent(entity, AnnouncementTemplateChangeType.Updated),
                cancellationToken);
        }

        public async Task Delete(AnnouncementTemplate entity, CancellationToken cancellationToken = default)
        {
            await RootRepository.DeleteAsync(entity);

            await ThunderCqrs.SendEvent(
                new AnnouncementTemplateChangeEvent(entity, AnnouncementTemplateChangeType.Deleted),
                cancellationToken);
        }
    }
}
