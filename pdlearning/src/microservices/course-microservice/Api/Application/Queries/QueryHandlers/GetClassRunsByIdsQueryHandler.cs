using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetClassRunsByIdsQueryHandler : BaseQueryHandler<GetClassRunsByIdsQuery, IEnumerable<ClassRunModel>>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public GetClassRunsByIdsQueryHandler(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
        }

        protected override async Task<IEnumerable<ClassRunModel>> HandleAsync(
            GetClassRunsByIdsQuery query,
            CancellationToken cancellationToken)
        {
            if (query.ClassRunIds == null || !query.ClassRunIds.Any())
            {
                return new List<ClassRunModel>();
            }

            var classRuns = await _readClassRunRepository
                .GetAll()
                .Where(x => query.ClassRunIds.Contains(x.Id) && x.Status == ClassRunStatus.Published)
                .Select(x => new ClassRunModel(x)).ToListAsync(cancellationToken);

            return classRuns;
        }
    }
}
