using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.Services;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class RollbackDigitalContentCommandHandler : BaseCommandHandler<RollbackDigitalContentCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<UploadedContent> _uploadedContentRepository;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IContentUrlExtractor _contentUrlExtractor;
        private readonly IOutboxQueue _outboxQueue;

        public RollbackDigitalContentCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<LearningContent> learningContentRepository,
            IRepository<UploadedContent> uploadedContentRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<AttributionElement> attributionElemRepository,
            IRepository<Chapter> chapterRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IOutboxQueue outboxQueue,
            IContentUrlExtractor contentUrlExtractor,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _learningContentRepository = learningContentRepository;
            _uploadedContentRepository = uploadedContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _accessRightRepository = accessRightRepository;
            _chapterRepository = chapterRepository;
            _contentUrlExtractor = contentUrlExtractor;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(RollbackDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId);

            var existingDigitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.RevertFromRecordId, cancellationToken);
            if (existingDigitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            DigitalContentModel model = new DigitalContentModel();

            if (existingDigitalContent.Type == ContentType.LearningContent)
            {
                var newClonedLearningContent = CommandUtilities.CloneLearningContent(
                    existingDigitalContent,
                    command.RevertToRecordId,
                    command.UserId,
                    existingDigitalContent.Status,
                    CreateCloneTitle);

                newClonedLearningContent.ParentId = existingDigitalContent.ParentId == Guid.Empty ? existingDigitalContent.Id : existingDigitalContent.Id;
                newClonedLearningContent.OriginalObjectId = existingDigitalContent.OriginalObjectId == Guid.Empty ? existingDigitalContent.ParentId : existingDigitalContent.OriginalObjectId;

                // Keep owner info
                newClonedLearningContent.OwnerId = existingDigitalContent.OwnerId;
                newClonedLearningContent.CreatedBy = existingDigitalContent.CreatedBy;

                await _learningContentRepository.InsertAsync(newClonedLearningContent);
                await _contentUrlExtractor.ExtractContentUrl(newClonedLearningContent);

                model = new DigitalContentModel(newClonedLearningContent);
            }
            else if (existingDigitalContent.Type == ContentType.UploadedContent)
            {
                var newClonedUploadedContent = CommandUtilities.CloneUploadedContent(
                    existingDigitalContent,
                    command.RevertToRecordId,
                    command.UserId,
                    existingDigitalContent.Status,
                    CreateCloneTitle);

                newClonedUploadedContent.ParentId = existingDigitalContent.ParentId == Guid.Empty ? existingDigitalContent.Id : existingDigitalContent.Id;
                newClonedUploadedContent.OriginalObjectId = existingDigitalContent.OriginalObjectId == Guid.Empty ? existingDigitalContent.ParentId : existingDigitalContent.OriginalObjectId;

                // Keep owner info
                newClonedUploadedContent.OwnerId = existingDigitalContent.OwnerId;
                newClonedUploadedContent.CreatedBy = existingDigitalContent.CreatedBy;

                await _uploadedContentRepository.InsertAsync(newClonedUploadedContent);
                model = new DigitalContentModel(newClonedUploadedContent);

                await this.CloneForChapters(command.RevertFromRecordId, command.RevertToRecordId, command.UserId);
            }

            await this.CloneForAttributionElements(command.RevertFromRecordId, command.RevertToRecordId);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Rollback, model, UserContext);
        }

        private async Task CloneForAttributionElements(Guid digitalContentId, Guid newDigitalContentId)
        {
            var allElements = await _attributionElemRepository.GetAllListAsync(_ => _.DigitalContentId == digitalContentId);
            var clonedItems = CommandUtilities.CloneAttributionElement(allElements, newDigitalContentId);

            foreach (var attributionElement in clonedItems)
            {
                await _attributionElemRepository.InsertAsync(attributionElement);
            }
        }

        private async Task CloneForChapters(Guid digitalContentId, Guid newDigitalContentId, Guid userId)
        {
            var chapters = await _chapterRepository
                .GetAllIncluding(_ => _.Attachments)
                .Where(_ => _.ObjectId == digitalContentId)
                .ToListAsync();
            var clonedChapters = CommandUtilities.CloneChapter(chapters, newDigitalContentId, userId);

            foreach (var chapter in clonedChapters)
            {
                await _chapterRepository.InsertAsync(chapter);
            }
        }

        private string CreateCloneTitle(string originTitle)
        {
            return originTitle;
        }
    }
}
