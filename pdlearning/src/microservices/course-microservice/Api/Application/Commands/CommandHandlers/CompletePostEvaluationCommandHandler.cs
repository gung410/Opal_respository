using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CompletePostEvaluationCommandHandler : BaseCommandHandler<CompletePostEvaluationCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly AutoProcessLearningProgressLogic _autoProcessLearningProgressLogic;

        public CompletePostEvaluationCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            RegistrationCudLogic registrationCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            AutoProcessLearningProgressLogic autoProcessLearningProgressLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _registrationCudLogic = registrationCudLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _autoProcessLearningProgressLogic = autoProcessLearningProgressLogic;
        }

        protected override async Task HandleAsync(CompletePostEvaluationCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistration = (await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationQuery(
                _readRegistrationRepository
                    .GetAll()
                    .Where(x => x.Id == command.RegistrationId &&
                                x.UserId == CurrentUserId &&
                                !x.PostCourseEvaluationFormCompleted),
                cancellationToken))
                .FirstOrDefault();
            aggregatedRegistration = EnsureEntityFound(aggregatedRegistration);

            EnsureBusinessLogicValid(
                aggregatedRegistration.Registration.ValidateCanCompletePostCourseEvaluation(
                    aggregatedRegistration.ClassRun,
                    aggregatedRegistration.Course));

            await _autoProcessLearningProgressLogic.CompletePostEvaluation(aggregatedRegistration.Registration, cancellationToken);

            await _registrationCudLogic.Update(aggregatedRegistration, cancellationToken);
        }
    }
}
