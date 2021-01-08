using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendLearningRecordEventLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly ECertificateBuilderLogic _certificateBuilder;
        private readonly GetAggregatedECertificateTemplateSharedQuery _aggregatedECertificateTemplateSharedQuery;

        public SendLearningRecordEventLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            ECertificateBuilderLogic certificateBuilder,
            GetAggregatedECertificateTemplateSharedQuery aggregatedECertificateTemplateSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readUserRepository = readUserRepository;
            _certificateBuilder = certificateBuilder;
            _aggregatedECertificateTemplateSharedQuery = aggregatedECertificateTemplateSharedQuery;
        }

        public async Task ByRegistrations(
           List<Registration> registrations,
           CancellationToken cancellationToken = default)
        {
            var correlationId = Guid.NewGuid();
            var courseIds = registrations.Select(x => x.CourseId);
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);

            var classRunIds = registrations.Select(x => x.ClassRunId);
            var classRunDict = await _readClassRunRepository.GetDictionaryByIdsAsync(classRunIds);

            var events = registrations
                .Where(x => courseDict.ContainsKey(x.CourseId) && classRunDict.ContainsKey(x.ClassRunId))
                .Select(registration => new LearningRecordEvent(registration, courseDict[registration.CourseId], classRunDict[registration.ClassRunId], correlationId));
            await _thunderCqrs.SendEvents(events, cancellationToken);

            await SendECertificates(correlationId, registrations, courseDict, cancellationToken);
        }

        private async Task SendECertificates(
            Guid correlationId,
            List<Registration> registrations,
            Dictionary<Guid, CourseEntity> courseDict,
            CancellationToken cancellationToken = default)
        {
            var userIds = registrations
                .Select(x => x.UserId)
                .Union(courseDict.Values.SelectMany(x => x.CourseFacilitatorIds))
                .Distinct()
                .ToList();
            var usersDict = await _readUserRepository.GetAll()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

            var validRegistrations = registrations
                .Where(x => x.LearningStatus == LearningStatus.Completed)
                .Where(x => courseDict.ContainsKey(x.CourseId)
                            && courseDict[x.CourseId].ECertificateTemplateId != null
                            && courseDict[x.CourseId].CourseFacilitatorIds.All(facilitatorId => usersDict.ContainsKey(facilitatorId))
                            && usersDict.ContainsKey(x.UserId))
                .ToList();

            var ecertificateTemplateIds =
                validRegistrations.SelectList(p => courseDict[p.CourseId].ECertificateTemplateId.GetValueOrDefault());
            var aggregatedECertificateTemplatesDic = (await _aggregatedECertificateTemplateSharedQuery.ByIds(ecertificateTemplateIds)).ToDictionary(p => p.Template.Id);

            foreach (var registration in validRegistrations)
            {
                var recordType = courseDict[registration.CourseId].PDActivityType == MetadataTagConstants.MicroLearningTagId ? RecordType.MicroLearning : RecordType.Course;
                var course = courseDict[registration.CourseId];

                var certificateBase64 = await _certificateBuilder.BuildForParticipantBase64(
                    registration,
                    usersDict[registration.UserId],
                    course,
                    aggregatedECertificateTemplatesDic[course.ECertificateTemplateId.GetValueOrDefault()],
                    usersDict[course.CourseFacilitatorIds.First()],
                    ReportGeneralOutputFormatType.PDF,
                    cancellationToken);

                await _thunderCqrs.SendEvent(new ECertificateEvent(registration, certificateBase64, recordType, correlationId), cancellationToken);
            }
        }
    }
}
