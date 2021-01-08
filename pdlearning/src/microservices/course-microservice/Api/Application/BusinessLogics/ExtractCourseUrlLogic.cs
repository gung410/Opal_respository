using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.BrokenLinkChecker;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ExtractCourseUrlLogic : BaseBusinessLogic
    {
        private readonly IAccessControlContext<CourseUser> _accessControlContext;
        private readonly IBrokenLinkChecker _brokenLinkChecker;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly IOutboxQueue _outboxQueue;
        private readonly GetAggregatedContentSharedQuery _getAggregatedContentSharedQuery;

        public ExtractCourseUrlLogic(
            IBrokenLinkChecker brokenLinkChecker,
            WebAppLinkBuilder webAppLinkBuilder,
            IAccessControlContext<CourseUser> accessControlContext,
            IOptions<RabbitMQOptions> rabbitMQOptions,
            IOutboxQueue outboxQueue,
            GetAggregatedContentSharedQuery getAggregatedContentSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _brokenLinkChecker = brokenLinkChecker;
            _webAppLinkBuilder = webAppLinkBuilder;
            _accessControlContext = accessControlContext;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _outboxQueue = outboxQueue;
            _getAggregatedContentSharedQuery = getAggregatedContentSharedQuery;
        }

        public async Task ExtractContentUrl(List<Guid> contentIds, ContentType contentType, CancellationToken cancellationToken = default)
        {
            var reportContentType = contentType switch
            {
                ContentType.Assignment => BrokenLinkContentType.Assignment,
                ContentType.Lecture => BrokenLinkContentType.Lecture,
                _ => BrokenLinkContentType.Section,
            };

            var contents = await _getAggregatedContentSharedQuery.ByIdsAndType(contentIds, contentType, true, cancellationToken);

            if (!contents.Any())
            {
                return;
            }

            foreach (var p in contents)
            {
                if (p.Owner != null)
                {
                    var contentUrl = p.ClassRunId.HasValue
                    ? _webAppLinkBuilder.GetClassRunDetailLinkForLMMModule(
                        LMMTabConfigurationConstant.CoursesTab,
                        LMMTabConfigurationConstant.ClassRunsTab,
                        LMMTabConfigurationConstant.AllClassRunsTab,
                        CourseDetailModeConstant.View,
                        LMMTabConfigurationConstant.ClassRunInfoTab,
                        ClassRunDetailModeConstant.View,
                        p.CourseId,
                        p.ClassRunId.GetValueOrDefault())
                    : _webAppLinkBuilder.GetCourseDetailLinkForLMMModule(
                        CAMTabConfigurationConstant.CoursesTab,
                        LMMTabConfigurationConstant.CourseInfoTab,
                        LMMTabConfigurationConstant.AllClassRunsTab,
                        CourseDetailModeConstant.View,
                        p.CourseId);
                    var enqueueUrlsBody = new EnqueueUrlMessage
                    {
                        ObjectId = p.Id,
                        OriginalObjectId = null,
                        ParentId = p.ClassRunId.HasValue ? p.ClassRunId : p.CourseId,
                        EmailActionName = null,
                        EmailActionUrl = contentUrl,
                        ObjectDetailUrl = contentUrl,
                        ObjectOwnerId = p.OwnerId,
                        ObjectOwnerName = p.Owner.FullName(),
                        ObjectTitle = p.Title,
                        Urls = _brokenLinkChecker.ExtractUrlFromHtml(HttpUtility.HtmlDecode(string.Join(",", p.CombinedRichText()))).ToList(),
                        Module = ModuleIdentifier.Course,
                        ContentType = reportContentType
                    };

                    var enqueueUrlsMessage = BuildMQMessage(
                            routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                            body: enqueueUrlsBody,
                            userContext: _accessControlContext.UserContext);

                    await _outboxQueue.QueueMessageAsync(
                        new QueueMessage(BrokenLinkRoutingKeys.EnqueueUrlsRoutingKey, enqueueUrlsMessage, _rabbitMQOptions.DefaultEventExchange));
                }
            }
        }

        public async Task DeleteExtractedUrls(List<Guid> contentIds)
        {
            foreach (var objectId in contentIds)
            {
                var dequeueObjectMessage = BuildMQMessage(
                    routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                    body: new DequeueObjectMessage { ObjectId = objectId },
                    userContext: _accessControlContext.UserContext);

                await _outboxQueue.QueueMessageAsync(new QueueMessage(
                    routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                    message: dequeueObjectMessage,
                    exchange: _rabbitMQOptions.DefaultEventExchange));
            }
        }

        private OpalMQMessage<object> BuildMQMessage(string routingKey, object body, IUserContext userContext)
        {
            return new OpalMQMessage<object>
            {
                Type = OpalMQMessageType.Event,
                Name = routingKey,
                Routing = new OpalMQMessageRouting
                {
                    Action = routingKey
                },
                Payload = new OpalMQMessagePayload<object>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = body
                }
            };
        }
    }
}
