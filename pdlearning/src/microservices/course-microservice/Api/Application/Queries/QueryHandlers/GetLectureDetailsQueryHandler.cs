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
    public class GetLectureDetailsQueryHandler : BaseQueryHandler<GetLectureDetailsQuery, LectureModel>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;
        private readonly IReadOnlyRepository<LectureContent> _readLectureContentRepository;

        public GetLectureDetailsQueryHandler(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IReadOnlyRepository<LectureContent> readLectureContentRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
            _readLectureContentRepository = readLectureContentRepository;
        }

        protected override async Task<LectureModel> HandleAsync(GetLectureDetailsQuery query, CancellationToken cancellationToken)
        {
            var lecture = await _readLectureRepository.SingleAsync(_ => _.Id == query.LectureId);
            var lectureContent = await _readLectureContentRepository.SingleAsync(_ => _.LectureId == query.LectureId);

            return LectureModel.Create(lecture, lectureContent);
        }
    }
}
