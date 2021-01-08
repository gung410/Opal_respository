using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyCourseByCourseIdQueryHandler : BaseQueryHandler<GetMyCourseByCourseIdQuery, MyCourseModel>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;

        public GetMyCourseByCourseIdQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
        }

        protected override async Task<MyCourseModel> HandleAsync(GetMyCourseByCourseIdQuery query, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseShared
                .GetByUserIdAndCourseId(CurrentUserIdOrDefault, query.CourseId);

            if (existingMyCourse == null)
            {
                throw new EntityNotFoundException(typeof(MyCourse), query.CourseId);
            }

            return new MyCourseModel(existingMyCourse);
        }
    }
}
