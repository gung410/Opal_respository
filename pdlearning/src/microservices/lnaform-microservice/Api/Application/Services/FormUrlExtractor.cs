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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;
using FormQuestionEntity = Microservice.LnaForm.Domain.Entities.FormQuestion;

namespace Microservice.LnaForm.Application.Services
{
    public class FormUrlExtractor : ApplicationService, IFormUrlExtractor
    {
        private const int BatchSize = 10;

        private readonly IRepository<FormQuestionEntity> _formQuestionRepository;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IBrokenLinkChecker _brokenLinkChecker;
        private readonly IAccessControlContext _accessControlContext;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly IOutboxQueue _outboxQueue;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public FormUrlExtractor(
            IRepository<FormEntity> formRepository,
            IRepository<UserEntity> userRepository,
            IRepository<FormQuestionEntity> formQuestionRepository,
            IBrokenLinkChecker brokenLinkChecker,
            IAccessControlContext accessControlContext,
            IOptions<RabbitMQOptions> rabbitMQOptions,
            WebAppLinkBuilder webAppLinkBuilder,
            IOutboxQueue outboxQueue,
            IUnitOfWorkManager uowManager)
        {
            _formRepository = formRepository;
            _userRepository = userRepository;
            _formQuestionRepository = formQuestionRepository;
            _accessControlContext = accessControlContext;
            _outboxQueue = outboxQueue;
            _webAppLinkBuilder = webAppLinkBuilder;
            _rabbitMQOptions = rabbitMQOptions.Value;
            _brokenLinkChecker = brokenLinkChecker;
            _uowManager = uowManager;
        }

        public async Task ExtractAll()
        {
            bool continueToExtract = true;
            int skipCount = 0;

            while (continueToExtract)
            {
                var existedForms = await TakeNotExtractedForm(skipCount);
                if (!existedForms.Any())
                {
                    break;
                }

                await _uowManager.StartNewTransactionAsync(async () =>
                {
                    foreach (var form in existedForms)
                    {
                        var formQuestion = await _formQuestionRepository.GetAllListAsync(p => p.FormId == form.Id);
                        await ExtractFormUrl(form, formQuestion);
                    }
                });

                skipCount += BatchSize;
                continueToExtract = existedForms.Count == BatchSize;
            }
        }

        public async Task ExtractFormUrl(FormEntity form, List<FormQuestionEntity> formQuestion)
        {
            if (formQuestion.Count == 0)
            {
                return;
            }

            var urls = new List<string>();
            formQuestion.ForEach(question =>
            {
                var questionUrls = _brokenLinkChecker
                    .ExtractUrlFromHtml(HttpUtility.HtmlDecode(question.Title))
                    .ToList();
                urls = urls.Concat(questionUrls).ToList();
            });

            var owner = await _userRepository.GetAsync(form.OwnerId);

            var enqueueUrlsBody = new EnqueueUrlMessage
            {
                ObjectId = form.Id,
                OriginalObjectId = form.OriginalObjectId,
                EmailActionName = null,
                EmailActionUrl = _webAppLinkBuilder.GetFormDetailLink(form.Id),
                ObjectDetailUrl = _webAppLinkBuilder.GetFormDetailLink(form.Id),
                ObjectOwnerId = form.OwnerId,
                ObjectOwnerName = owner != null ? owner.FullName() : string.Empty,
                ObjectTitle = form.Title,
                Urls = urls,
                Module = ModuleIdentifier.LnaForm,
                ContentType = BrokenLinkContentType.Question
            };

            var enqueueUrlsMessage = BuildMQMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                body: enqueueUrlsBody,
                userContext: _accessControlContext.UserContext);

            await _outboxQueue.QueueMessageAsync(
                new QueueMessage(BrokenLinkRoutingKeys.EnqueueUrlsRoutingKey, enqueueUrlsMessage, _rabbitMQOptions.DefaultEventExchange));
        }

        public async Task DeleteExtractedUrls(Guid formId)
        {
            var dequeueObjectMessage = BuildMQMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                body: new DequeueObjectMessage { ObjectId = formId },
                userContext: _accessControlContext.UserContext);

            await _outboxQueue.QueueMessageAsync(new QueueMessage(
                routingKey: BrokenLinkRoutingKeys.DequeueObjectRoutingKey,
                message: dequeueObjectMessage,
                exchange: _rabbitMQOptions.DefaultEventExchange));
        }

        private Task<List<FormEntity>> TakeNotExtractedForm(int skip)
        {
            return _formRepository
                .GetAll()
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
