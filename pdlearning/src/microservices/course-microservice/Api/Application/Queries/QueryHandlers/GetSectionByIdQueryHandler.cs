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
    public class GetSectionByIdQueryHandler : BaseQueryHandler<GetSectionByIdQuery, SectionModel>
    {
        private readonly IReadOnlyRepository<Section> _readSectionRepository;

        public GetSectionByIdQueryHandler(
            IReadOnlyRepository<Section> readSectionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSectionRepository = readSectionRepository;
        }

        protected override async Task<SectionModel> HandleAsync(GetSectionByIdQuery query, CancellationToken cancellationToken)
        {
            var section = await _readSectionRepository.GetAsync(query.Id);

            return SectionModel.Create(section);
        }
    }
}
