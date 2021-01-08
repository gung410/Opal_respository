using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetClassRunByIdQueryHandler : BaseQueryHandler<GetClassRunByIdQuery, ClassRunModel>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly GetHasLearnerStartedSharedQuery _getHasLearnerStartedSharedQuery;

        public GetClassRunByIdQueryHandler(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            GetHasLearnerStartedSharedQuery getHasLearnerStartedSharedQuery,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _getHasLearnerStartedSharedQuery = getHasLearnerStartedSharedQuery;
        }

        protected override async Task<ClassRunModel> HandleAsync(
            GetClassRunByIdQuery query,
            CancellationToken cancellationToken)
        {
            var classRun = await _readClassRunRepository.GetAsync(query.Id);

            var classRunModel = new ClassRunModel(classRun);

            if (query.LoadHasLearnerStarted)
            {
                var hasLearnerStarted = await _getHasLearnerStartedSharedQuery.ByClassRunId(query.Id);
                classRunModel = classRunModel.SetHasLearnerStarted(hasLearnerStarted);
            }

            return classRunModel;
        }
    }
}
