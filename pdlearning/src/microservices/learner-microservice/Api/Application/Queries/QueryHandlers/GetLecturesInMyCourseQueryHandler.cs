using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetLecturesInMyCourseQueryHandler : BaseQueryHandler<GetLecturesInMyCourseQuery, List<LectureInMyCourseModel>>
    {
        private readonly IRepository<LectureInMyCourse> _lectureInMyCourseRepository;

        public GetLecturesInMyCourseQueryHandler(
            IRepository<LectureInMyCourse> lectureInMyCourseRepository,
            IUserContext userContext) : base(userContext)
        {
            _lectureInMyCourseRepository = lectureInMyCourseRepository;
        }

        protected override Task<List<LectureInMyCourseModel>> HandleAsync(GetLecturesInMyCourseQuery query, CancellationToken cancellationToken)
        {
            return _lectureInMyCourseRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.MyCourseId == query.MyCourseId)
                .Select(p => new LectureInMyCourseModel(p))
                .ToListAsync(cancellationToken);
        }
    }
}
