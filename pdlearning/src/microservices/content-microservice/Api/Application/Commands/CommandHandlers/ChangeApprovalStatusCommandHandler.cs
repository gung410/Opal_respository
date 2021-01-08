using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Services;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using CommentEntity = Microservice.Content.Domain.Entities.Comment;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class ChangeApprovalStatusCommandHandler : BaseCommandHandler<ChangeApprovalStatusCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IConfiguration _configuration;
        private readonly IContentUrlExtractor _contentUrlExtractor;
        private readonly IOutboxQueue _outboxQueue;

        public ChangeApprovalStatusCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<LearningContent> learningContentRepository,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<AttributionElement> attributionElemRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IAccessControlContext accessControlContext,
            IOutboxQueue outboxQueue,
            IConfiguration configuration,
            IContentUrlExtractor contentUrlExtractor,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _thunderCqrs = thunderCqrs;
            _attributionElemRepository = attributionElemRepository;
            _configuration = configuration;
            _contentUrlExtractor = contentUrlExtractor;
            _outboxQueue = outboxQueue;
            _accessRightRepository = accessRightRepository;
            _learningContentRepository = learningContentRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleAsync(ChangeApprovalStatusCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerOrApprovalPermissionExpr(command.UserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var existedDigitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.ContentId, cancellationToken);

            if (existedDigitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            if (!Validate(existedDigitalContent, command.Status))
            {
                throw new InvalidStatusException();
            }

            existedDigitalContent.Status = command.Status;
            existedDigitalContent.ChangedBy = command.UserId;
            existedDigitalContent.ChangedDate = Clock.Now;

            if (command.Status == DigitalContentStatus.PendingForApproval)
            {
                existedDigitalContent.SubmitDate = Clock.Now;
            }
            else if (command.Status == DigitalContentStatus.Archived)
            {
                existedDigitalContent.ArchiveDate = Clock.Now;
                existedDigitalContent.ArchivedBy = command.UserId;
            }

            if (command.Status == DigitalContentStatus.Approved &&
                (existedDigitalContent.IsAutoPublish == true || (existedDigitalContent.AutoPublishDate.HasValue &&
                                                                 existedDigitalContent.AutoPublishDate.Value.Date <= Clock.Now.Date)))
            {
                existedDigitalContent.Status = DigitalContentStatus.Published;
            }

            await _digitalContentRepository.UpdateAsync(existedDigitalContent);

            var model = new DigitalContentModel(existedDigitalContent);

            var attributionElements = await _attributionElemRepository
                .GetAllListAsync(_ => _.DigitalContentId == command.ContentId);

            var attributionElementModel = attributionElements
                .Select(a => new AttributionElementModel
                {
                    Id = a.Id,
                    Author = a.Author,
                    DigitalContentId = a.DigitalContentId,
                    LicenseType = a.LicenseType,
                    Source = a.Source,
                    Title = a.Title
                });

            model.AttributionElements.AddRange(attributionElementModel);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Updated, model, UserContext);

            await _thunderCqrs.SendEvent(
                new NotifyContentChangeEvent(
                    "firebase subject",
                    "firebase display message",
                    CurrentUserId.ToString()),
                cancellationToken);

            if (existedDigitalContent.Type == ContentType.LearningContent)
            {
                LearningContent existedLearningContent = await _learningContentRepository.FirstOrDefaultAsync(p => p.Id == existedDigitalContent.Id);
                await _contentUrlExtractor.ExtractContentUrl(existedLearningContent);
            }

            if (command.Status == DigitalContentStatus.PendingForApproval ||
                command.Status == DigitalContentStatus.Approved ||
                command.Status == DigitalContentStatus.Rejected)
            {
                await PerformSendNotifyContentIsSubmitted(existedDigitalContent, command.Status, command.Comment, cancellationToken);
            }
        }

        private bool Validate(DigitalContent digitalContent, DigitalContentStatus newStatus)
        {
            switch (newStatus)
            {
                case DigitalContentStatus.Approved:
                case DigitalContentStatus.Rejected:
                    return digitalContent.Status == DigitalContentStatus.PendingForApproval;

                case DigitalContentStatus.Published:
                    return digitalContent.Status == DigitalContentStatus.Approved;

                case DigitalContentStatus.Archived:
                    return digitalContent.Status == DigitalContentStatus.Approved || digitalContent.Status == DigitalContentStatus.Rejected ||
                        digitalContent.Status == DigitalContentStatus.Unpublished || digitalContent.Status == DigitalContentStatus.Draft;
                default:
                    return true;
            }
        }

        private async Task PerformSendNotifyContentIsSubmitted(
            DigitalContent existedDigitalContent,
            DigitalContentStatus commandStatus,
            string comment,
            CancellationToken cancellationToken)
        {
            var ownerName = _userRepository.FirstOrDefault(user => user.Id == existedDigitalContent.OwnerId).FullName();

            var contentUrl = WebAppLinkBuilder.GetDigitalContentDetailLink(_configuration, existedDigitalContent.Id);
            string subject = null;
            string template = null;

            var payload = new NotifyContentSubmittedRequestPayload
            {
                RecipientName = ownerName,
                ContentDetailUrl = contentUrl,
                ContentName = existedDigitalContent.Title,
                CreatorName = ownerName,
                Comment = comment
            };

            switch (commandStatus)
            {
                case DigitalContentStatus.Approved:
                    subject = "OPAL2.0 - Content Approved";
                    template = "ContentApprovedByAO";
                    payload.RecipientName = ownerName;
                    payload.AOName = _userRepository.FirstOrDefault(user => user.Id == CurrentUserId).FullName();
                    await SendNotifySubmitedEvent(subject, template, payload, existedDigitalContent.Id, existedDigitalContent.OwnerId, cancellationToken);
                    break;
                case DigitalContentStatus.Rejected:
                    subject = "OPAL2.0 - Content Rejected";
                    template = "ContentRejectedByAO";
                    payload.RecipientName = ownerName;
                    payload.AOName = _userRepository.FirstOrDefault(user => user.Id == CurrentUserId).FullName();
                    await SendNotifySubmitedEvent(subject, template, payload, existedDigitalContent.Id, existedDigitalContent.OwnerId, cancellationToken);
                    break;
                case DigitalContentStatus.PendingForApproval:
                    subject = "OPAL2.0 - New content pending approval";
                    template = "NewContentRequestApproval";
                    if (existedDigitalContent.PrimaryApprovingOfficerId.HasValue)
                    {
                        payload.AOName = _userRepository.FirstOrDefault(user => user.Id == existedDigitalContent.PrimaryApprovingOfficerId.Value).FullName();
                        payload.RecipientName = payload.AOName;
                        await SendNotifySubmitedEvent(subject, template, payload, existedDigitalContent.Id, existedDigitalContent.PrimaryApprovingOfficerId.Value, cancellationToken);
                    }

                    if (existedDigitalContent.AlternativeApprovingOfficerId.HasValue)
                    {
                        payload.AOName = _userRepository.FirstOrDefault(user => user.Id == existedDigitalContent.AlternativeApprovingOfficerId.Value).FullName();
                        payload.RecipientName = payload.AOName;
                        await SendNotifySubmitedEvent(subject, template, payload, existedDigitalContent.Id, existedDigitalContent.AlternativeApprovingOfficerId.Value, cancellationToken);
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task SendNotifySubmitedEvent(string subject, string template, NotifyContentSubmittedRequestPayload payload, Guid contentId, Guid receiverId, CancellationToken cancellationToken)
        {
            await _thunderCqrs.SendEvent(
               new NotifyContentSubmittedRequestEvent(
                    payload,
                    new ReminderByDto
                    {
                        Type = ReminderByType.AbsoluteDateTimeUTC,
                        Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
                    },
                    subject,
                    template,
                    contentId,
                    receiverId),
               cancellationToken);
        }
    }
}
