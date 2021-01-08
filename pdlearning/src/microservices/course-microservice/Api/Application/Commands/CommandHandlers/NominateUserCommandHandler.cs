using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class NominateUserCommandHandler : BaseCommandHandler<NominateUserCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetRegisteredClassRunSlotSharedQuery _registeredClassRunSlotSharedQuery;
        private readonly ValidateNominatedLearnerLogic _validateNominatedLearnerLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;
        private readonly CheckLearnerCourseCriteriaViolationLogic _checkLearnerCourseCriteriaViolationLogic;
        private readonly ProcessAutomateParticipantSelectionLogic _processAutomateParticipantSelectionLogic;

        public NominateUserCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetRegisteredClassRunSlotSharedQuery registeredClassRunSlotSharedQuery,
            RegistrationCudLogic registrationCudLogic,
            CheckLearnerCourseCriteriaViolationLogic checkLearnerCourseCriteriaViolationLogic,
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            ProcessAutomateParticipantSelectionLogic processAutomateParticipantSelectionLogic,
            ValidateNominatedLearnerLogic validateNominatedLearnerLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _registeredClassRunSlotSharedQuery = registeredClassRunSlotSharedQuery;
            _validateNominatedLearnerLogic = validateNominatedLearnerLogic;
            _registrationCudLogic = registrationCudLogic;
            _checkLearnerCourseCriteriaViolationLogic = checkLearnerCourseCriteriaViolationLogic;
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
            _processAutomateParticipantSelectionLogic = processAutomateParticipantSelectionLogic;
        }

        protected override async Task HandleAsync(NominateUserCommand command, CancellationToken cancellationToken)
        {
            var coursesDic = await GetCoursesDic(command, cancellationToken);
            var classRunsDic = await GetClassRunsDic(command, cancellationToken);

            EnsureBusinessLogicValid(classRunsDic.Select(p => p.Value).ToList(), p => p.ValidateCanBeNominated(coursesDic[p.CourseId]));

            // Process automate participant selection if classrun of each registration have available slots and course automate activated
            // before nominate learners, so that nominated learners will be moved to wait list if no available slot after process automate participant selection
            var selectedRegistrations = await _processAutomateParticipantSelectionLogic
                .ByClassRunIds(
                    classRunsDic
                        .Where(x => x.Value.CourseAutomateActivated)
                        .Select(p => p.Key).ToList(),
                    null,
                    Guid.Empty,
                    cancellationToken);

            var courseToUsersToExistedInProgressRegistrationsDic = (await _readRegistrationRepository.GetAll()
                .Where(x => command.GetRegistrationCourseIds().Contains(x.CourseId))
                .Where(Registration.InProgressExpr()).ToListAsync(cancellationToken))
                .GroupBy(x => x.CourseId)
                .ToDictionary(x => x.Key, x => x.GroupBy(p => p.UserId).ToDictionary(p => p.Key, p => p.ToList()));
            var totalRegisteredSlotPerClassRun = await _registeredClassRunSlotSharedQuery.CountByClassRunIds(command.GetRegistrationClassrunIds(), selectedRegistrations, false, cancellationToken);

            // Get all registration is nominated, if learner is nominated who has a registration in the same classrun, the registration will be updated Registration Type.
            var (newNominatedRegistrations, updateToNominatedRegistrations) = await ProcessRegistrations(command, coursesDic, classRunsDic, courseToUsersToExistedInProgressRegistrationsDic, totalRegisteredSlotPerClassRun, cancellationToken);

            await _registrationCudLogic.UpdateMany(selectedRegistrations, cancellationToken);
            await _registrationCudLogic.UpdateMany(updateToNominatedRegistrations, cancellationToken);
            await _registrationCudLogic.InsertMany(newNominatedRegistrations, cancellationToken);
        }

        private async Task<ValueTuple<List<Registration>, List<Registration>>> ProcessRegistrations(
            NominateUserCommand command,
            Dictionary<Guid, CourseEntity> coursesDic,
            Dictionary<Guid, ClassRun> classRunsDic,
            Dictionary<Guid, Dictionary<Guid, List<Registration>>> courseToUsersToExistedInProgressRegistrationsDic,
            Dictionary<Guid, int> totalRegisteredSlotPerClassRun,
            CancellationToken cancellationToken = default)
        {
            var newRegistrations = new List<Registration>();
            var updatedRegistrations = new List<Registration>();

            var classRunIds = command.Registrations.Select(x => x.ClassRunId).Distinct().ToList();
            var classRunCriteriaActivatedDict = await _readClassRunRepository.GetAll().Where(p => classRunIds.Contains(p.Id))
                .Select(x => new { x.Id, x.CourseCriteriaActivated }).ToDictionaryAsync(p => p.Id, p => p.CourseCriteriaActivated, cancellationToken);
            var courseIds = command.Registrations.Select(x => x.CourseId).Distinct().ToList();
            var courseCriteriaDict = await _readCourseCriteriaRepository.GetAll().Where(p => courseIds.Contains(p.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var nominatedRegistration in command.Registrations)
            {
                CourseCriteriaLearnerViolation courseCriteriaLearnerViolation = null;
                if (classRunCriteriaActivatedDict != null &&
                    classRunCriteriaActivatedDict.ContainsKey(nominatedRegistration.ClassRunId) &&
                    classRunCriteriaActivatedDict[nominatedRegistration.ClassRunId] &&
                    courseCriteriaDict.ContainsKey(nominatedRegistration.CourseId))
                {
                    courseCriteriaLearnerViolation = await _checkLearnerCourseCriteriaViolationLogic.Execute(
                       nominatedRegistration.CourseId,
                       nominatedRegistration.ClassRunId,
                       nominatedRegistration.UserId,
                       courseCriteriaDict[nominatedRegistration.CourseId],
                       newRegistrations,
                       cancellationToken);

                    if (courseCriteriaLearnerViolation != null && courseCriteriaLearnerViolation.CheckIsPreRequisiteCoursesViolated())
                    {
                        continue;
                    }
                }

                var course = coursesDic[nominatedRegistration.CourseId];
                var existedInProgressRegistrations = courseToUsersToExistedInProgressRegistrationsDic.GetValueOrDefault(nominatedRegistration.CourseId)?.GetValueOrDefault(nominatedRegistration.UserId);
                var validateLearnerResult = _validateNominatedLearnerLogic.Execute(course, existedInProgressRegistrations);

                switch (validateLearnerResult)
                {
                    case ValidateLearnerResultType.HasUncompleteRegistrationAddedByCA:
                    case ValidateLearnerResultType.HasPreRequisiteCoursesViolated:
                    case ValidateLearnerResultType.MaxReLearningTimes:
                        continue;
                    case ValidateLearnerResultType.HasUncompleteRegistrationAddedByLearner:
                        updatedRegistrations.AddRange(UpdateRegistrationTypeToNominated(existedInProgressRegistrations));
                        continue;
                    case ValidateLearnerResultType.Valid:
                        var classRun = classRunsDic[nominatedRegistration.ClassRunId];
                        var totalRegisteredSlot = totalRegisteredSlotPerClassRun[nominatedRegistration.ClassRunId];
                        var remainingSlot = classRun.MaxClassSize - totalRegisteredSlot;
                        var registration = new Registration
                        {
                            Id = Guid.NewGuid(),
                            UserId = nominatedRegistration.UserId,
                            ApprovingOfficer = nominatedRegistration.ApprovingOfficer,
                            AlternativeApprovingOfficer = nominatedRegistration.AlternativeApprovingOfficer,
                            ClassRunId = nominatedRegistration.ClassRunId,
                            CourseId = nominatedRegistration.CourseId,
                            RegistrationDate = Clock.Now,
                            RegistrationType = RegistrationType.Nominated,
                            CreatedBy = CurrentUserIdOrDefault,
                            CourseCriteriaViolation = courseCriteriaLearnerViolation,
                            Status = courseCriteriaLearnerViolation != null && courseCriteriaLearnerViolation.IsViolated
                                ? RegistrationStatus.WaitlistConfirmed
                                : (course.CanByPassCAConfirmed()
                                    ? (remainingSlot <= 0 ? RegistrationStatus.WaitlistConfirmed : RegistrationStatus.ConfirmedByCA)
                                    : RegistrationStatus.Approved)
                        };
                        newRegistrations.Add(registration);
                        totalRegisteredSlotPerClassRun[nominatedRegistration.ClassRunId] += 1;
                        break;
                }
            }

            return (newRegistrations, updatedRegistrations);
        }

        private async Task<Dictionary<Guid, ClassRun>> GetClassRunsDic(NominateUserCommand command, CancellationToken cancellationToken)
        {
            var classRunIds = command.Registrations
                .Select(x => x.ClassRunId)
                .Distinct()
                .ToList();
            var classRuns = await _readClassRunRepository
                .GetAll()
                .Where(x => classRunIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return classRuns.ToDictionary(p => p.Id);
        }

        private async Task<Dictionary<Guid, CourseEntity>> GetCoursesDic(NominateUserCommand command, CancellationToken cancellationToken)
        {
            var courseIds = command.Registrations
                .Select(x => x.CourseId)
                .Distinct()
                .ToList();
            var courses = await _readCourseRepository.GetAll()
                .Where(x => courseIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return courses.ToDictionary(p => p.Id);
        }

        private List<Registration> UpdateRegistrationTypeToNominated(List<Registration> existedInProgressRegistrations)
        {
            if (existedInProgressRegistrations == null)
            {
                return new List<Registration>();
            }

            var notCompleteRegistrations = existedInProgressRegistrations.Where(x => !x.IsLearningFinished());
            var appliedRegistrations = notCompleteRegistrations.Where(x => x.Status == RegistrationStatus.ConfirmedByCA || x.Status == RegistrationStatus.OfferConfirmed);
            return appliedRegistrations
                .Select(appliedRegistration =>
                {
                    appliedRegistration.RegistrationType = RegistrationType.Nominated;
                    return appliedRegistration;
                })
                .ToList();
        }
    }
}
