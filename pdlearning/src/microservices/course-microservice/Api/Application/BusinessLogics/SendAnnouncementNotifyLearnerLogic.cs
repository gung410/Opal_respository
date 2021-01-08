using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendAnnouncementNotifyLearnerLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly GetAggregatedAnnouncementSharedQuery _getAggregatedAnnouncementSharedQuery;

        public SendAnnouncementNotifyLearnerLogic(
            IThunderCqrs thunderCqrs,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedAnnouncementSharedQuery getAggregatedAnnouncementSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedAnnouncementSharedQuery = getAggregatedAnnouncementSharedQuery;
        }

        public async Task Execute(List<AnnouncementAggregatedEntityModel> announcements, Guid? sendBy, CancellationToken cancellationToken = default)
        {
            var validAnnouncements = announcements
                .Where(p => p.Announcement.ValidateSendAnnouncementNotifyLearner())
                .ToList();

            foreach (var announcement in validAnnouncements)
            {
                await _thunderCqrs.SendEvents(
                    announcement.Participants.Select(p =>
                        new SendAnnouncementNotifyLearnerEvent(
                            sendBy ?? announcement.Announcement.CreatedBy ?? Guid.Empty,
                            new SendAnnouncementNotifyLearnerPayload
                            {
                                UserName = announcement.ParticipantUsersDic[p.UserId].FullName(),
                                CourseTitle = announcement.Course.CourseName,
                                ClassTitle = announcement.ClassRun.ClassTitle,
                                AnnouncementTitle = announcement.Announcement.Title,
                                AnnouncementContent = announcement.Announcement.Message,
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(announcement.Course.Id)
                            },
                            new List<Guid> { p.UserId })),
                    cancellationToken);
            }
        }

        public async Task Execute(Announcement announcement, Guid? sendBy, CancellationToken cancellationToken = default)
        {
            await Execute(
                F.List(await _getAggregatedAnnouncementSharedQuery.FullByAnnouncement(announcement, cancellationToken)),
                sendBy,
                cancellationToken);
        }
    }
}
