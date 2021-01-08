using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.BrokenLinkChecker;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Application.Services
{
    public class ContentUrlExtractor : ApplicationService, IContentUrlExtractor
    {
        private const int BatchSize = 10;

        private readonly IAccessControlContext _accessControlContext;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IBrokenLinkChecker _brokenLinkChecker;
        private readonly IConfiguration _configuration;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IOutboxQueue _outboxQueue;

        public ContentUrlExtractor(
            IBrokenLinkChecker brokenLinkChecker,
            IRepository<LearningContent> learningContentRepository,
            IAccessControlContext accessControlContext,
            IConfiguration configuration,
            IRepository<UserEntity> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<RabbitMQOptions> rabbitMQOptions,
            IOutboxQueue outboxQueue)
        {
            _brokenLinkChecker = brokenLinkChecker;
            _learningContentRepository = learningContentRepository;
            _accessControlContext = accessControlContext;
            _configuration = configuration;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _userRepository = userRepository;
            _uowManager = unitOfWorkManager;
            _outboxQueue = outboxQueue;
        }

        public async Task ExtractAll()
        {
            bool continueToExtract = true;
            int skipCount = 0;

            while (continueToExtract)
            {
                var contents = await TakeNotExtractedContent(skipCount);
                if (!contents.Any())
                {
                    break;
                }

                await _uowManager.StartNewTransactionAsync(async () =>
                {
                    foreach (var learningContent in contents)
                    {
                        await ExtractContentUrl(learningContent);
                    }
                });

                skipCount += BatchSize;
                continueToExtract = contents.Count == BatchSize;
            }
        }

        public async Task ExtractContentUrl(LearningContent learningContent)
        {
            if (learningContent is null)
            {
                return;
            }

            if (learningContent.IsArchived)
            {
                return;
            }

            var urls = _brokenLinkChecker.ExtractUrlFromHtml(HttpUtility.HtmlDecode(learningContent.HtmlContent));

            var owner = await _userRepository.GetAsync(learningContent.OwnerId);

            var enqueueUrlsBody = new EnqueueUrlMessage
            {
                ObjectId = learningContent.Id,
                OriginalObjectId = learningContent.OriginalObjectId,
                EmailActionName = null,
                EmailActionUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, learningContent.Id),
                ObjectDetailUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, learningContent.Id),
                ObjectOwnerId = learningContent.OwnerId,
                ObjectOwnerName = owner.FullName(),
                ObjectTitle = learningContent.Title,
                Urls = urls.ToList(),
                Module = ModuleIdentifier.Content,
                ContentType = BrokenLinkContentType.LearningContent
            };

            var enqueueUrlsMessage = BuildMQMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                body: enqueueUrlsBody,
                userContext: _accessControlContext.UserContext);

            await _outboxQueue.QueueMessageAsync(
                new QueueMessage(BrokenLinkRoutingKeys.EnqueueUrlsRoutingKey, enqueueUrlsMessage, _rabbitMQOptions.DefaultEventExchange));
        }

        public async Task DeleteExtractedUrls(Guid learningContentId)
        {
            var dequeueObjectMessage = BuildMQMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                body: new DequeueObjectMessage { ObjectId = learningContentId },
                userContext: _accessControlContext.UserContext);

            await _outboxQueue.QueueMessageAsync(new QueueMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                message: dequeueObjectMessage,
                exchange: _rabbitMQOptions.DefaultEventExchange));
        }

        private Task<List<LearningContent>> TakeNotExtractedContent(int skip)
        {
            return _learningContentRepository
                .GetAll()
                .IgnoreArchivedItems()
                .Skip(skip)
                .Take(BatchSize)
                .ToListAsync();
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
