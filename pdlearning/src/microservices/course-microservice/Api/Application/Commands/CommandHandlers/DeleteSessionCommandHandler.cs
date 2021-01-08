using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteSessionCommandHandler : BaseCommandHandler<DeleteSessionCommand>
    {
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly SessionCudLogic _sessionCudLogic;

        public DeleteSessionCommandHandler(
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            SessionCudLogic sessionCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _readCourseRepository = readCourseRepository;
            _sessionCudLogic = sessionCudLogic;
        }

        protected override async Task HandleAsync(DeleteSessionCommand command, CancellationToken cancellationToken)
        {
            var aggregatedSession = await _getAggregatedSessionSharedQuery.ById(command.Id, cancellationToken);

            EnsureBusinessLogicValid(aggregatedSession.Session.ValidateCanEditOrDelete(aggregatedSession.ClassRun, aggregatedSession.Course));

            EnsureValidPermission(aggregatedSession.Session.HasModifyPermission(
                CurrentUserId,
                aggregatedSession.Course,
                _readCourseRepository.GetHasAdminRightChecker(aggregatedSession.Course, AccessControlContext)(aggregatedSession.Course)));

            await _sessionCudLogic.Delete(aggregatedSession, cancellationToken);
        }
    }
}
