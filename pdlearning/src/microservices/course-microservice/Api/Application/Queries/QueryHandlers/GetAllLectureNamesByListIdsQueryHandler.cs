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
    public class GetAllLectureNamesByListIdsQueryHandler : BaseQueryHandler<GetAllLectureNamesByListIdsQuery, LectureIdMapNameModel[]>
    {
        private readonly IReadOnlyRepository<Lecture> _readLectureRepository;

        public GetAllLectureNamesByListIdsQueryHandler(
            IReadOnlyRepository<Lecture> readLectureRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLectureRepository = readLectureRepository;
        }

        protected override Task<LectureIdMapNameModel[]> HandleAsync(GetAllLectureNamesByListIdsQuery query, CancellationToken cancellationToken)
        {
            if (query.ListLectureIds != null && query.ListLectureIds.Length > 0)
            {
                return _readLectureRepository
                    .GetAll()
                    .Where(_ => query.ListLectureIds.Contains(_.Id))
                    .Select(_ => new LectureIdMapNameModel
                    {
                        LectureId = _.Id,
                        Name = _.LectureName
                    })
                    .ToArrayAsync(cancellationToken);
            }

            return Task.FromResult(new LectureIdMapNameModel[0]);
        }
    }
}
