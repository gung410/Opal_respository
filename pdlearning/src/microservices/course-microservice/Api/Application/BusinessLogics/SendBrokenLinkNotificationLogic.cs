using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendBrokenLinkNotificationLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetContentSharedQuery _getContentSharedQuery;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public SendBrokenLinkNotificationLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetContentSharedQuery getContentSharedQuery,
            WebAppLinkBuilder webAppLinkBuilder,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _readUserRepository = readUserRepository;
            _getContentSharedQuery = getContentSharedQuery;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        public async Task NotifyBrokenLinkFound(Guid contentId, ContentType contentType)
        {
            await PerformSendEmail(contentId, contentType, "ContentBrokenLinkSystemAlert");
        }

        public async Task NotifyLearnerReport(Guid contentId, ContentType contentType)
        {
            await PerformSendEmail(contentId, contentType, "ContentBrokenLinkLearnerReport");
        }

        private async Task PerformSendEmail(Guid contentId, ContentType contentType, string templateName)
        {
            var content = await _getContentSharedQuery.ById(contentId, contentType);
            if (content == null)
            {
                return;
            }

            var user = await _readUserRepository.FirstOrDefaultAsync(content.CreatedBy);
            var payload = new NotifyContentBrokenLinkPayload
            {
                AssetOwnerName = user.FullName(),
                AssetName = content.Title,
                ActionUrl = content.ClassrunId.HasValue
                    ? _webAppLinkBuilder.GetClassRunDetailLinkForLMMModule(
                        LMMTabConfigurationConstant.CoursesTab,
                        LMMTabConfigurationConstant.ClassRunsTab,
                        LMMTabConfigurationConstant.AllClassRunsTab,
                        CourseDetailModeConstant.View,
                        LMMTabConfigurationConstant.ClassRunInfoTab,
                        ClassRunDetailModeConstant.View,
                        content.CourseId,
                        content.ClassrunId.Value)
                    : _webAppLinkBuilder.GetCourseDetailLinkForLMMModule(
                        CAMTabConfigurationConstant.CoursesTab,
                        LMMTabConfigurationConstant.CourseInfoTab,
                        LMMTabConfigurationConstant.AllClassRunsTab,
                        CourseDetailModeConstant.View,
                        content.CourseId)
            };
            var reminderByConditions = new ReminderByDto
            {
                Type = ReminderByType.AbsoluteDateTimeUTC,

                // Add 2 minutes to ensure the time is valid after the message was sent to Communication.
                Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
            };

            await _thunderCqrs.SendEvent(
                new NotifyContentBrokenLinkEvent(
                        payload,
                        new List<Guid> { content.CreatedBy },
                        templateName,
                        reminderByConditions));
        }
    }
}
