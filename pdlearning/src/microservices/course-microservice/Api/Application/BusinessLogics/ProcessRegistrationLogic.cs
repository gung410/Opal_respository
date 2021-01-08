using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ProcessRegistrationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly IReadOnlyRepository<CourseCriteria> _readCourseCriteriaRepository;
        private readonly CheckLearnerCourseCriteriaViolationLogic _checkLearnerCourseCriteriaViolationLogic;
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;

        public ProcessRegistrationLogic(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            IReadOnlyRepository<CourseCriteria> readCourseCriteriaRepository,
            CheckLearnerCourseCriteriaViolationLogic checkLearnerCourseCriteriaViolationLogic,
            GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _readCourseCriteriaRepository = readCourseCriteriaRepository;
            _checkLearnerCourseCriteriaViolationLogic = checkLearnerCourseCriteriaViolationLogic;
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
        }

        public bool IsRegistrationClassFull(Dictionary<Guid, int> remainingSlotDict, Guid classRunId)
        {
            return remainingSlotDict[classRunId] <= 0;
        }

        public async Task RegisterNewForNotAddedByCA(
            List<Registration> registrations,
            Guid? currentUserId,
            CancellationToken cancellationToken = default)
        {
            EnsureBusinessLogicValid(registrations, p => Validation.ValidIf(p.RegistrationType != RegistrationType.AddedByCA, ExceptionMsgsConstant.InvalidData));

            var registrationToAggregatedClassRuns = await GetListOfRegistrationToAggregatedClassRun(registrations, cancellationToken);
            var existedInProgressRegistrationsByCourseAndUserIdDic = await GetExistedInProgressRegistrationsByCourseAndUserIdDic(registrations, cancellationToken);
            var classrunIdToAvailableSlotDict = await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(registrations.Select(p => p.ClassRunId), cancellationToken);

            for (int i = 0; i < registrationToAggregatedClassRuns.Count; i++)
            {
                var (registration, aggregatedClassRun) = registrationToAggregatedClassRuns[i];
                var existedSameUserAndCourseInProgressRegistrations =
                    existedInProgressRegistrationsByCourseAndUserIdDic
                        .GetValueOrDefault(
                            registration.CourseId,
                            new Dictionary<Guid, List<Registration>>())
                        .GetValueOrDefault(
                            registration.UserId,
                            new List<Registration>());

                EnsureBusinessLogicValid(registration.ValidateCanBeRegisterNewForNotAddedByCA(
                    aggregatedClassRun.Course,
                    aggregatedClassRun.ClassRun,
                    existedSameUserAndCourseInProgressRegistrations));

                if (aggregatedClassRun.Course.CanByPassApproval())
                {
                    var courseCriteriaLearnerViolation = await GetCourseCriteriaLearnerViolation(aggregatedClassRun.Course, aggregatedClassRun.ClassRun, registration, registrationToAggregatedClassRuns, i, cancellationToken);

                    // NOTE: Registration will be bypass confirmation workflow when LearningMode is a E-Learning and registration method is Public.(OP-11571)
                    if (aggregatedClassRun.Course.CanByPassCAConfirmed())
                    {
                        ByPassAdministrator(aggregatedClassRun.ClassRun, registration, currentUserId, courseCriteriaLearnerViolation, classrunIdToAvailableSlotDict);
                    }
                    else
                    {
                        ByPassApproval(registration, courseCriteriaLearnerViolation, currentUserId);
                    }
                }
            }
        }

        private async Task<CourseCriteriaLearnerViolation> GetCourseCriteriaLearnerViolation(
            CourseEntity course,
            ClassRun classRun,
            Registration registration,
            List<KeyValuePair<Registration, ClassRunAggregatedEntityModel>> registrationToAggregatedClassRuns,
            int index,
            CancellationToken cancellationToken = default)
        {
            var courseCriteriaLearnerViolation = classRun.CourseCriteriaActivated
                ? await _checkLearnerCourseCriteriaViolationLogic.Execute(
                    course.Id,
                    classRun.Id,
                    registration.UserId,
                    await _readCourseCriteriaRepository.FirstOrDefaultAsync(p => p.Id == course.Id),
                    registrationToAggregatedClassRuns.Take(index).Select(p => p.Key).ToList(),
                    cancellationToken)
                : null;

            registration.CourseCriteriaViolation = courseCriteriaLearnerViolation ?? registration.CourseCriteriaViolation;

            return courseCriteriaLearnerViolation;
        }

        private void ByPassAdministrator(
           ClassRun classRun,
           Registration registration,
           Guid? currentUserId,
           CourseCriteriaLearnerViolation courseCriteriaLearnerViolation,
           Dictionary<Guid, int> classrunIdToAvailableSlotDict)
        {
            if (classrunIdToAvailableSlotDict[classRun.Id] <= 0 || (courseCriteriaLearnerViolation != null && courseCriteriaLearnerViolation.IsViolated))
            {
                registration.Status = RegistrationStatus.WaitlistConfirmed;
            }
            else
            {
                registration.Status = RegistrationStatus.ConfirmedByCA;
                classrunIdToAvailableSlotDict[registration.ClassRunId] -= 1;
            }

            registration.ApprovingDate = Clock.Now;
            registration.ChangedBy = currentUserId;
        }

        private void ByPassApproval(Registration registration, CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, Guid? currentUserId)
        {
            registration.Status = courseCriteriaLearnerViolation != null && courseCriteriaLearnerViolation.IsViolated ? RegistrationStatus.WaitlistConfirmed : RegistrationStatus.Approved;
            registration.ApprovingDate = Clock.Now;
            registration.ChangedBy = currentUserId;
        }

        private async Task<Dictionary<Guid, Dictionary<Guid, List<Registration>>>> GetExistedInProgressRegistrationsByCourseAndUserIdDic(
            List<Registration> registrations,
            CancellationToken cancellationToken = default)
        {
            var courseIds = registrations.Select(p => p.CourseId).Distinct();
            var registrationUserIds = registrations.Select(p => p.UserId).Distinct().ToList();
            var existedInProgressLearnerRegistrations = await _readRegistrationRepository
                .GetAll()
                .Where(x => registrationUserIds.Contains(x.UserId) && courseIds.Contains(x.CourseId))
                .Where(Registration.InProgressExpr())
                .ToListAsync(cancellationToken);
            var existedInProgressRegistrationsByCourseAndUserIdDic = existedInProgressLearnerRegistrations
                .GroupBy(p => p.CourseId)
                .ToDictionary(
                    p => p.Key,
                    p => p.ToList()
                        .GroupBy(x => x.UserId)
                        .ToDictionary(x => x.Key, x => x.ToList()));
            return existedInProgressRegistrationsByCourseAndUserIdDic;
        }

        private async Task<List<KeyValuePair<Registration, ClassRunAggregatedEntityModel>>> GetListOfRegistrationToAggregatedClassRun(
            List<Registration> registrations,
            CancellationToken cancellationToken = default)
        {
            var aggregatedClassRuns = await _getAggregatedClassRunSharedQuery.ByClassRunIds(registrations.Select(p => p.ClassRunId).ToList(), false, cancellationToken);
            var aggregatedClassRunsDic = aggregatedClassRuns.ToDictionary(p => p.ClassRun.Id);
            var registrationToAggregatedClassDic = registrations.ToDictionary(p => p, p => aggregatedClassRunsDic[p.ClassRunId]);
            return registrationToAggregatedClassDic.ToList();
        }
    }
}
