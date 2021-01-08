using System;
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
    public class GetAllDigitalContentIdsBelongCourseIdsQueryHandler : BaseQueryHandler<GetAllDigitalContentIdsBelongCourseIdsQuery, CourseIdMapListDigitalContentIdModel[]>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;

        public GetAllDigitalContentIdsBelongCourseIdsQueryHandler(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readLectureContentRepository = readLectureContentRepository;
        }

        protected override async Task<CourseIdMapListDigitalContentIdModel[]> HandleAsync(
            GetAllDigitalContentIdsBelongCourseIdsQuery query,
            CancellationToken cancellationToken)
        {
            // Filter lecture by course Ids to avoid full table scan.
            var lectureQuery = _readLectureRepository.GetAll().Where(_ => query.CourseIds.Contains(_.CourseId));

            // Filter lecture content by type and resource id has value to avoid full table scan.
            var lectureContentQuery = _readLectureContentRepository.GetAll().Where(_ => _.Type == LectureContentType.DigitalContent && _.ResourceId.HasValue);

            // Join 2 filtered tables.
            var digitalContentQuery = from lecture in lectureQuery
                                      join lectureContent in lectureContentQuery on lecture.Id equals lectureContent.LectureId
                                      select new { lecture.CourseId, DigitalContentId = lectureContent.ResourceId.Value };

            var digitalContentQueryResult = await digitalContentQuery.Distinct().ToListAsync(cancellationToken);

            var result = digitalContentQueryResult.GroupBy(_ => _.CourseId).Select(digitalContent =>

                // Current dotnet core 3.1 is not support for serialization of dictionary<Guid, Guid[]> so using 'CourseIdMapListDigitalContentIdModel[]' instead
                // ref to: https://github.com/dotnet/corefx/issues/40120
                new CourseIdMapListDigitalContentIdModel
                {
                    CourseId = digitalContent.Key,
                    ListDigitalContentId = digitalContent.Select(_ => _.DigitalContentId).ToArray()
                }).ToArray();

            var courseIdNotHaveDc = query.CourseIds.Except(result.Select(_ => _.CourseId)).Select(_ => new CourseIdMapListDigitalContentIdModel { CourseId = _, ListDigitalContentId = new Guid[0] });
            return result.Union(courseIdNotHaveDc).ToArray();
        }
    }
}
