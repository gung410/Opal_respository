using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common.Utils;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyCoursesSummaryQueryHandler : BaseQueryHandler<GetMyCoursesSummaryQuery, List<MyCoursesSummaryModel>>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;

        public GetMyCoursesSummaryQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
        }

        protected override async Task<List<MyCoursesSummaryModel>> HandleAsync(GetMyCoursesSummaryQuery query, CancellationToken cancellationToken)
        {
            var myCourseStatuses = EnumUtil.GetValues<MyCourseStatus>();

            var myCourses = await _readMyCourseShared
                .FilterByUserIdAndStatusQuery(CurrentUserIdOrDefault, myCourseStatuses)
                .GroupBy(p => new
                {
                    p.Status
                })
                .Select(p => new
                {
                    p.Key.Status,
                    Total = p.Count()
                })
                .ToListAsync(cancellationToken);

            var ongoingCoursesCount = myCourses
                .Where(p =>
                    p.Status == MyCourseStatus.InProgress
                    || p.Status == MyCourseStatus.Passed)
                .Sum(p => p.Total);

            var completedCoursesCount = myCourses
                .Where(p =>
                    p.Status == MyCourseStatus.Completed
                    || p.Status == MyCourseStatus.Failed)
                .Sum(p => p.Total);

            return query.StatusFilter.Select(p => new MyCoursesSummaryModel
            {
                StatusFilter = p,
                Total = p == MyCourseStatus.InProgress ? ongoingCoursesCount : completedCoursesCount
            }).ToList();
        }
    }
}
