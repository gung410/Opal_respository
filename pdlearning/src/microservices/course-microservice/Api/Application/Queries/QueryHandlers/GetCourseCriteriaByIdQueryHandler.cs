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
    public class GetCourseCriteriaByIdQueryHandler : BaseQueryHandler<GetCourseCriteriaByIdQuery, CourseCriteriaModel>
    {
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;

        public GetCourseCriteriaByIdQueryHandler(
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
        }

        protected override async Task<CourseCriteriaModel> HandleAsync(GetCourseCriteriaByIdQuery query, CancellationToken cancellationToken)
        {
            var courseCriteria = await _readCourseCriteriaRepository.FirstOrDefaultAsync(p => p.Id == query.Id);
            return courseCriteria != null ? new CourseCriteriaModel(courseCriteria) : null;
        }
    }
}
