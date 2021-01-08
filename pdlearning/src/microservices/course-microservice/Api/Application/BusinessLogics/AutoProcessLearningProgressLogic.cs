using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class AutoProcessLearningProgressLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly CompleteLearningProcessOption _completeLearningProcessOption;
        private readonly ProcessJustLearningCompletedParticipantLogic _processJustLearningCompletedParticipantLogic;
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;

        public AutoProcessLearningProgressLogic(
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IOptions<CompleteLearningProcessOption> completeLearningProcessOption,
            ProcessJustLearningCompletedParticipantLogic processJustLearningCompletedParticipantLogic,
            IUserContext userContext,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery) : base(userContext)
        {
            _readSessionRepository = readSessionRepository;
            _readClassRunRepository = readClassRunRepository;
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _completeLearningProcessOption = completeLearningProcessOption.Value;
            _processJustLearningCompletedParticipantLogic = processJustLearningCompletedParticipantLogic;
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
        }

        /// <summary>
        /// This method is used for calculating learning progress when learner complete learning a content or take attendance for a session.
        /// </summary>
        /// <param name="registrationQuery">registration query which query registrations need to calculate.</param>
        /// <param name="cancellationToken">token to cancel.</param>
        /// <returns>Registrations which were updated.</returns>
        public async Task<List<Registration>> ExecuteAsync(
            IQueryable<Registration> registrationQuery,
            CancellationToken cancellationToken = default)
        {
            const int batchSize = 50;
            var skip = 0;
            var updatedRegistrations = new List<Registration>();

            while (true)
            {
                var registrations = await registrationQuery.Skip(skip).Take(batchSize).ToListAsync(cancellationToken);
                if (!registrations.Any())
                {
                    break;
                }

                await ExecuteAsync(registrations, cancellationToken: cancellationToken);
                updatedRegistrations.AddRange(registrations);

                skip += batchSize;
            }

            return updatedRegistrations;
        }

        /// <summary>
        /// This method is used for calculating learning progress when learner complete learning a content or take attendance for a session.
        /// </summary>
        /// <param name="registrations">Registrations need to calculate.</param>
        /// <param name="updatedAttendanceTrackings">List attendance tracking which updating on memory but is still not commit to database.</param>
        /// <param name="cancellationToken">token to cancel.</param>
        /// <returns>Registrations which were updated.</returns>
        public async Task<List<Registration>> ExecuteAsync(
            List<Registration> registrations,
            List<AttendanceTracking> updatedAttendanceTrackings = null,
            CancellationToken cancellationToken = default)
        {
            var inprogressLearningRegistrations = registrations.ToList(p => !p.IsLearningFinished());

            var inprogressLearningAggregatedRegistrations =
                await _aggregatedRegistrationSharedQuery.FullByRegistrations(inprogressLearningRegistrations, cancellationToken);

            var classRunWithAggregatedSessionInfoDic = await GetClassRunWithAggregatedSessionInfoDic(inprogressLearningAggregatedRegistrations);
            var presentSessionCountPerRegistrationPerClassRunDic = await GetPresentSessionCountPerRegistrationPerClassRunDic(
                updatedAttendanceTrackings,
                inprogressLearningAggregatedRegistrations,
                cancellationToken);

            foreach (var aggregatedRegistration in inprogressLearningAggregatedRegistrations)
            {
                var classRunWithAggregatedSessionInfo = classRunWithAggregatedSessionInfoDic[aggregatedRegistration.Registration.ClassRunId];
                var presentSessionCount =
                    presentSessionCountPerRegistrationPerClassRunDic[aggregatedRegistration.Registration.ClassRunId]
                        .GetValueOrDefault(aggregatedRegistration.Registration.Id, 0);

                // If class run don't have any session, attendanceClassRunRatio will be 100%.
                var attendanceRatio = classRunWithAggregatedSessionInfo.TotalSessionCount == 0
                    ? 100D
                    : (double)presentSessionCount / classRunWithAggregatedSessionInfo.TotalSessionCount;

                if (aggregatedRegistration.Course.LearningMode == MetadataTagConstants.ELearningTagId)
                {
                    ProcessForElearning(
                        aggregatedRegistration.Registration,
                        aggregatedRegistration.ClassRun.EndDateTime);
                }
                else if (aggregatedRegistration.Course.LearningMode == MetadataTagConstants.FaceToFaceTagId)
                {
                    ProcessForFaceToFace(
                        aggregatedRegistration.Registration,
                        classRunWithAggregatedSessionInfo.LastSessionEndDateTime,
                        attendanceRatio);
                }
                else if (aggregatedRegistration.Course.LearningMode == MetadataTagConstants.BlendedLearningTagId)
                {
                    ProcessForBlended(
                        aggregatedRegistration.Registration,
                        aggregatedRegistration.ClassRun.EndDateTime,
                        attendanceRatio);
                }

                aggregatedRegistration.Registration.ChangedBy = CurrentUserId;
            }

            TriggerPostCourseSurvey(registrations);
            return registrations;
        }

        public Task CompletePostEvaluation(Registration registration, CancellationToken cancellationToken = default)
        {
            registration.PostCourseEvaluationFormCompleted = true;
            if (registration.LearningStatus == LearningStatus.Passed)
            {
                registration.LearningStatus = LearningStatus.Completed;
                registration.LearningCompletedDate = Clock.Now;
                return _processJustLearningCompletedParticipantLogic.Execute(F.List(registration), cancellationToken);
            }

            return Task.CompletedTask;
        }

        private async Task<Dictionary<Guid, ClassRunWithAggregatedSessionInfo>> GetClassRunWithAggregatedSessionInfoDic(
            List<RegistrationAggregatedEntityModel> inprogressLearningAggregatedRegistrations)
        {
            var classRunIds = inprogressLearningAggregatedRegistrations.SelectListDistinct(p => p.Registration.ClassRunId);

            var classRunWithAggregatedSessionInfoDic = (await _readClassRunRepository
                    .GetAll()
                    .Where(p => classRunIds.Contains(p.Id))
                    .GroupJoin(
                        _readSessionRepository.GetAll(),
                        classRun => classRun.Id,
                        session => session.ClassRunId,
                        (classRun, session) => new { classRun, session })
                    .Select(gj => new ClassRunWithAggregatedSessionInfo
                    {
                        ClassRun = gj.classRun,
                        TotalSessionCount = gj.session.Count(),
                        LastSessionEndDateTime = gj.session.Max(x => x.EndDateTime)
                    })
                    .ToListAsync())
                .ToDictionary(p => p.ClassRun.Id);
            return classRunWithAggregatedSessionInfoDic;
        }

        private async Task<Dictionary<Guid, Dictionary<Guid, int>>> GetPresentSessionCountPerRegistrationPerClassRunDic(
           List<AttendanceTracking> updatedAttendanceTrackings,
           List<RegistrationAggregatedEntityModel> inprogressLearningAggregatedRegistrations,
           CancellationToken cancellationToken)
        {
            var classRunIds = inprogressLearningAggregatedRegistrations.SelectListDistinct(p => p.Registration.ClassRunId);
            var classRunSessionQuery = _readClassRunRepository
                .GetAll()
                .Where(p => classRunIds.Contains(p.Id))
                .GroupJoin(
                    _readSessionRepository.GetAll(),
                    classRun => classRun.Id,
                    session => session.ClassRunId,
                    (classRun, session) => new { classRun, session })
                .SelectMany(
                    p => p.session.DefaultIfEmpty(),
                    (gj, session) => new { ClassRun = gj.classRun, Session = session });

            var userIds = inprogressLearningAggregatedRegistrations.SelectListDistinct(p => p.Registration.UserId);
            var attendanceTrackingQuery = _readAttendanceTrackingRepository
                .GetAll()
                .Where(p => userIds.Contains(p.Userid));

            var classRunAttendanceInfoList = await classRunSessionQuery
                .GroupJoin(
                    attendanceTrackingQuery,
                    classRunSession => classRunSession.Session.Id,
                    attendanceTracking => attendanceTracking.SessionId,
                    (classRunSession, attendanceTrackings) => new { classRunSession, attendanceTrackings })
                .SelectMany(
                    p => p.attendanceTrackings.DefaultIfEmpty(),
                    (gj, attendanceTracking) =>
                        new
                        {
                            gj.classRunSession.ClassRun,
                            gj.classRunSession.Session,
                            AttendanceTracking = attendanceTracking
                        })
                .ToListAsync(cancellationToken);
            if (updatedAttendanceTrackings != null)
            {
                var updatedAttendanceTrackingsDic = updatedAttendanceTrackings.ToDictionary(p => p.Id);
                classRunAttendanceInfoList = classRunAttendanceInfoList.SelectList(p => new
                {
                    p.ClassRun,
                    p.Session,
                    AttendanceTracking = p.AttendanceTracking != null &&
                                         updatedAttendanceTrackingsDic.ContainsKey(p.AttendanceTracking.Id)
                        ? updatedAttendanceTrackingsDic[p.AttendanceTracking.Id]
                        : p.AttendanceTracking
                });
            }

            var classRunToPresentSessionCountPerRegistrationDic = classRunAttendanceInfoList
                .GroupBy(p => p.ClassRun.Id)
                .ToDictionary(
                    grpByClassRunAttendances => grpByClassRunAttendances.Key,
                    grpByClassRunAttendances =>
                    {
                        return grpByClassRunAttendances
                            .Where(x => x.AttendanceTracking != null && x.AttendanceTracking.IsAttendanceCheckingCompleted())
                            .GroupBy(x => x.AttendanceTracking.RegistrationId)
                            .ToDictionary(
                                grpByRegistrationAttendances => grpByRegistrationAttendances.Key,
                                grpByRegistrationAttendances => grpByRegistrationAttendances
                                    .DistinctBy(x => x.AttendanceTracking.SessionId).Count());
                    });
            return classRunToPresentSessionCountPerRegistrationDic;
        }

        private void ProcessForElearning(Registration registration, DateTime? classRunEndDateTime)
        {
            if (registration.AtleastPercentContentsCompleted(_completeLearningProcessOption.LearningContentProgress))
            {
                registration.LearningStatus = LearningStatus.Passed;
            }
            else if (classRunEndDateTime.HasValue && classRunEndDateTime.Value < Clock.Now)
            {
                registration.LearningStatus = LearningStatus.Failed;
            }
        }

        private void ProcessForFaceToFace(Registration registration, DateTime? lastSessionEndDateTime, double? attendanceClassRunRatio)
        {
            if (attendanceClassRunRatio >= _completeLearningProcessOption.AttendanceRatio)
            {
                registration.LearningStatus = LearningStatus.Passed;
            }
            else if (!lastSessionEndDateTime.HasValue || lastSessionEndDateTime.Value <= Clock.Now)
            {
                registration.LearningStatus = LearningStatus.Failed;
            }
        }

        private void ProcessForBlended(Registration registration, DateTime? classRunEndDateTime, double? attendanceClassRunRatio)
        {
            if (attendanceClassRunRatio >= _completeLearningProcessOption.AttendanceRatio
                && registration.AtleastPercentContentsCompleted(_completeLearningProcessOption.LearningContentProgress))
            {
                registration.LearningStatus = LearningStatus.Passed;
            }
            else if (classRunEndDateTime.HasValue && classRunEndDateTime.Value <= Clock.Now)
            {
                registration.LearningStatus = LearningStatus.Failed;
            }
        }

        private void TriggerPostCourseSurvey(List<Registration> registrations)
        {
            registrations.ToList(p => p.CanTriggerPostCourseEvaluation()).ForEach(x =>
            {
                x.SendPostCourseSurveyDate = Clock.Now;
            });
        }
    }

    internal class ClassRunWithAggregatedSessionInfo
    {
        public ClassRun ClassRun { get; set; }

        public int TotalSessionCount { get; set; }

        public DateTime? LastSessionEndDateTime { get; set; }
    }
}
