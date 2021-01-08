using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class CheckCourseEndDateValidWithClassEndDateQueryHandler : BaseQueryHandler<CheckCourseEndDateValidWithClassEndDateQuery, bool>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public CheckCourseEndDateValidWithClassEndDateQueryHandler(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
        }

        protected override async Task<bool> HandleAsync(CheckCourseEndDateValidWithClassEndDateQuery query, CancellationToken cancellationToken)
        {
            return !await _readClassRunRepository.GetAll()
                .Where(p => p.CourseId == query.CourseId && p.EndDateTime > query.EndDate)
                .AnyAsync(cancellationToken);
        }
    }
}
