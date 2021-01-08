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
    public class GetMyLearningPackageQueryHandler : BaseQueryHandler<GetMyLearningPackageQuery, MyLearningPackageModel>
    {
        private readonly IRepository<MyLearningPackage> _myLearningPackageRepository;

        public GetMyLearningPackageQueryHandler(
            IRepository<MyLearningPackage> myLearningPackageRepository,
            IUserContext userContext) : base(userContext)
        {
            _myLearningPackageRepository = myLearningPackageRepository;
        }

        protected override async Task<MyLearningPackageModel> HandleAsync(GetMyLearningPackageQuery query, CancellationToken cancellationToken)
        {
            if (query.MyLectureId.HasValue)
            {
                var myLearningPackage = await _myLearningPackageRepository
                    .GetAll()
                    .Where(p => p.UserId == CurrentUserId)
                    .Where(p => p.MyLectureId != null && p.MyLectureId == query.MyLectureId)
                    .OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefaultAsync(cancellationToken);

                return myLearningPackage != null ? new MyLearningPackageModel(myLearningPackage) : null;
            }
            else
            {
                var myLearningPackage = await _myLearningPackageRepository
                    .GetAll()
                    .Where(p => p.UserId == CurrentUserId)
                    .Where(p => p.MyDigitalContentId != null && p.MyDigitalContentId == query.MyDigitalContentId)
                    .FirstOrDefaultAsync(cancellationToken);

                return myLearningPackage != null ? new MyLearningPackageModel(myLearningPackage) : null;
            }
        }
    }
}
