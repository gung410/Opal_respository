using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAnnouncementTemplateByIdQueryHandler : BaseQueryHandler<GetAnnouncementTemplateByIdQuery, AnnouncementTemplateModel>
    {
        private readonly IReadOnlyRepository<AnnouncementTemplate> _readAnnouncementTemplateRepository;

        public GetAnnouncementTemplateByIdQueryHandler(
            IReadOnlyRepository<AnnouncementTemplate> readAnnouncementTemplateRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementTemplateRepository = readAnnouncementTemplateRepository;
        }

        protected override async Task<AnnouncementTemplateModel> HandleAsync(GetAnnouncementTemplateByIdQuery query, CancellationToken cancellationToken)
        {
            var announcementTemplate = await _readAnnouncementTemplateRepository.GetAsync(query.Id);
            return new AnnouncementTemplateModel(announcementTemplate);
        }
    }
}
