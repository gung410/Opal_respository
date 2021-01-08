using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeRegistrationByLearnerCommandHandler : BaseCommandHandler<ChangeRegistrationByLearnerCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public ChangeRegistrationByLearnerCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _registrationCudLogic = registrationCudLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        protected override async Task HandleAsync(ChangeRegistrationByLearnerCommand command, CancellationToken cancellationToken)
        {
            // Get PendingApprovalByLearner registration of a user. Only get one because a user can only have one in progress registration at a time for a class
            var pendingApprovalByLearnerRegistration = (await _getAggregatedRegistrationSharedQuery
                .WithClassAndCourseByRegistrationQuery(
                    _readRegistrationRepository.GetAll()
                        .Where(x =>
                            x.CourseId == command.CourseId &&
                            x.ClassRunId == command.ClassRunId &&
                            x.UserId == CurrentUserId)
                        .Where(Registration.IsPendingApprovalByLearnerExpr()),
                    cancellationToken))
                .FirstOrDefault();

            pendingApprovalByLearnerRegistration = EnsureEntityFound(pendingApprovalByLearnerRegistration);

            EnsureBusinessLogicValid(pendingApprovalByLearnerRegistration.Registration
                .ValidateCanProcessPendingApprovalByLearner(pendingApprovalByLearnerRegistration.Course, command.Status));

            pendingApprovalByLearnerRegistration.Registration.Status = command.Status;
            pendingApprovalByLearnerRegistration.Registration.ChangedDate = Clock.Now;

            await _registrationCudLogic.Update(pendingApprovalByLearnerRegistration, cancellationToken: cancellationToken);
        }
    }
}
