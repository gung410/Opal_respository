using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendRegistrationNotificationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IThunderCqrs _thunderCqrs;

        public SendRegistrationNotificationLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            WebAppLinkBuilder webAppLinkBuilder,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readUserRepository = readUserRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _thunderCqrs = thunderCqrs;
        }

        public async Task SendToLearnerWhenOfficerRejected(List<Registration> registrations, Guid currentUserId, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerRegistrationRejectedNotifyLearnerEvent(
                    currentUserId,
                    new LearnerRegistrationRejectedNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }),
                cancellationToken);
        }

        public async Task SendToLearnerWhenOfficerApproved(List<Registration> registrations, Guid currentUserId, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerRegistrationApprovedNotifyLearnerEvent(
                    currentUserId,
                    new LearnerRegistrationApprovedNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }),
                cancellationToken);
        }

        public async Task SendToLearnerWhenWaitlistConfirmed(List<Registration> registrations, Guid currentUserId, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerWaitlistConfirmedNotifyLearnerEvent(
                    currentUserId,
                    new LearnerWaitlistConfirmedNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }),
                cancellationToken);
        }

        public async Task SendToLearnerWhenRejectedByCA(List<Registration> registrations, string comment, Guid currentUserId)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new RejectedRegistrationNotifyLearnerEvent(
                    currentUserId,
                    new RejectedRegistrationNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        CourseCode = course.CourseCode,
                        Comment = comment
                    },
                    new List<Guid> { registration.UserId }));
        }

        public async Task SendToAdminWhenOfficerApproved(List<Registration> registrations, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerRegistrationNotifyAdministratorEvent(
                    registration.UserId,
                    new LearnerRegistrationNotifyAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForCAMModule(
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab,
                            CAMTabConfigurationConstant.ClassRunsTab,
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalClassrunTab,
                            CourseDetailModeConstant.View,
                            CAMTabConfigurationConstant.RegistrationsTab,
                            ClassRunDetailModeConstant.View,
                            registration.CourseId,
                            registration.ClassRunId)
                    },
                    course.GetAdministratorIds()),
                cancellationToken);
        }

        public async Task SendToAdminWhenLearnerRejectOffer(List<Registration> registrations, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new RejectedOfferNotifyAdministratorEvent(
                    user.Id,
                    new RejectedOfferNotifyAdministratorPayload { CourseTitle = course.CourseName },
                    course.GetAdministratorIds()),
                cancellationToken);
        }

        public async Task SendToAdminWhenLearnerAcceptOffer(List<Registration> registrations, CancellationToken cancellationToken)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new AcceptOfferNotifyAdministratorEvent(
                    user.Id,
                    new AcceptOfferNotifyAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForCAMModule(
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab,
                            CAMTabConfigurationConstant.ClassRunsTab,
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalClassrunTab,
                            CourseDetailModeConstant.View,
                            CAMTabConfigurationConstant.RegistrationsTab,
                            ClassRunDetailModeConstant.View,
                            registration.CourseId,
                            registration.ClassRunId)
                    },
                    course.GetAdministratorIds()),
                cancellationToken);
        }

        public async Task SendToAdminWhenWaitlistConfirmed(
            List<Registration> registrations)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerWaitlistNotifyAdministratorEvent(
                    registration.UserId,
                    new LearnerWaitlistNotifyAdministratorPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForCAMModule(
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalCourseTab,
                            CAMTabConfigurationConstant.ClassRunsTab,
                            CAMTabConfigurationConstant.HasPendingRegistrationApprovalClassrunTab,
                            CourseDetailModeConstant.View,
                            CAMTabConfigurationConstant.WaitlistTab,
                            ClassRunDetailModeConstant.View,
                            registration.CourseId,
                            registration.ClassRunId)
                    },
                    course.GetAdministratorIds()));
        }

        public async Task SendToLearnerWhenRegistrationConfirmed(
            List<Registration> registrations,
            Guid currentUserId)
        {
            await ByRegistrations(
                registrations,
                (registration, course, classRun, user) => new LearnerRegistrationConfirmedNotifyLearnerEvent(
                    currentUserId,
                    new LearnerRegistrationConfirmedNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ClassrunTitle = classRun.ClassTitle,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        public async Task SendToLearnerWhenOfferSent(List<Registration> sentOfferRegistrations, Guid currentUserId)
        {
            await ByRegistrations(
                sentOfferRegistrations,
                (registration, course, classRun, user) => new SendOfferNotifyLearnerEvent(
                    currentUserId,
                    new SendOfferNotifyLearnerPayload
                    {
                        CourseTitle = course.CourseName,
                        ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(course.Id),
                        ObjectType = TodoEventPayloadObjectTypesConstant.Course,
                        ObjectId = course.Id
                    },
                    new List<Guid> { registration.UserId }));
        }

        public async Task ByRegistrations<TMailEvent>(
            List<Registration> registrations,
            Func<Registration, CourseEntity, ClassRun, CourseUser, TMailEvent> mailEventFn,
            CancellationToken cancellationToken = default)
            where TMailEvent : BaseThunderEvent
        {
            var courseIds = registrations.Select(x => x.CourseId).Distinct();
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);

            var classRunIds = registrations.Select(x => x.ClassRunId).Distinct();
            var classRunDict = await _readClassRunRepository.GetDictionaryByIdsAsync(classRunIds);

            var validRegistrations = registrations
                .Where(r => courseDict.ContainsKey(r.CourseId) && classRunDict.ContainsKey(r.ClassRunId)).ToList();

            var userIds = validRegistrations.Select(x => x.UserId).ToList();
            var userDict = await _readUserRepository
                .GetAll()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
            var events = new List<BaseThunderEvent>();
            if (mailEventFn != null)
            {
                events.AddRange(validRegistrations.Select(p => mailEventFn(p, courseDict[p.CourseId], classRunDict[p.ClassRunId], userDict[p.UserId])));
            }

            await _thunderCqrs.SendEvents(events);
        }
    }
}
