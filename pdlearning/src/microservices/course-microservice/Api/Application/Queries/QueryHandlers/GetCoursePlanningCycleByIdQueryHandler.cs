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
    public class GetCoursePlanningCycleByIdQueryHandler : BaseQueryHandler<GetCoursePlanningCycleByIdQuery, CoursePlanningCycleModel>
    {
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;

        public GetCoursePlanningCycleByIdQueryHandler(
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
        }

        protected override async Task<CoursePlanningCycleModel> HandleAsync(GetCoursePlanningCycleByIdQuery query, CancellationToken cancellationToken)
        {
            var coursePlanningCycle = await _readCoursePlanningCycleRepository.GetAsync(query.Id);

            return new CoursePlanningCycleModel(coursePlanningCycle);
        }
    }
}
