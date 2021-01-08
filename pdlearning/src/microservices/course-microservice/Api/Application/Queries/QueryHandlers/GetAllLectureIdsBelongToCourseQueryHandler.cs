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
    public class GetAllLectureIdsBelongToCourseQueryHandler : BaseQueryHandler<GetAllLectureIdsBelongToCourseQuery, List<Guid>>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;

        public GetAllLectureIdsBelongToCourseQueryHandler(
            IReadOnlyRepository<Lecture> lectureRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = lectureRepository;
        }

        protected override Task<List<Guid>> HandleAsync(
            GetAllLectureIdsBelongToCourseQuery query,
            CancellationToken cancellationToken)
        {
            return _readLectureRepository
                .GetAll()
                .Where(_ => _.CourseId == query.CourseId)
                .Select(_ => _.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
