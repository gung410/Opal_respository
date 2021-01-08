using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetCoursesByCourseIdsQueryHandler : BaseQueryHandler<GetCoursesByCourseIdsQuery, List<CourseModel>>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;

        public GetCoursesByCourseIdsQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared,
            IReadUserBookmarkShared readUserBookmarkShared) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
            _readUserBookmarkShared = readUserBookmarkShared;
        }

        protected override async Task<List<CourseModel>> HandleAsync(GetCoursesByCourseIdsQuery query, CancellationToken cancellationToken)
        {
            var courseIds = query.CourseIds.Distinct().ToList();

            var myCourses = await _readMyCourseShared
                .GetByUserIdAndCourseIds(CurrentUserIdOrDefault, courseIds);

            var bookmarks = await _readUserBookmarkShared
                .GetByItemIds(CurrentUserIdOrDefault, courseIds);

            return await _readMyCourseShared
                .GetRelatedInfoOfCourses(
                    CurrentUserIdOrDefault,
                    courseIds,
                    bookmarks,
                    myCourses);
        }
    }
}
