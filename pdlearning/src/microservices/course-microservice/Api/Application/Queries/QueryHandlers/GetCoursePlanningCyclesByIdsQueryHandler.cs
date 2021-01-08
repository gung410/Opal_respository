using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetCoursePlanningCyclesByIdsQueryHandler : BaseQueryHandler<GetCoursePlanningCyclesByIdsQuery, List<CoursePlanningCycleModel>>
    {
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;

        public GetCoursePlanningCyclesByIdsQueryHandler(
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
        }

        protected override async Task<List<CoursePlanningCycleModel>> HandleAsync(GetCoursePlanningCyclesByIdsQuery query, CancellationToken cancellationToken)
        {
            var coursePlanningCycles = await _readCoursePlanningCycleRepository.GetAll().Where(x => query.Ids.Contains(x.Id)).ToListAsync(cancellationToken);

            return coursePlanningCycles.Select(x => new CoursePlanningCycleModel(x)).ToList();
        }
    }
}
