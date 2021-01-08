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
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class CloneDigitalContentAsNewVersionCommandHandler : BaseCommandHandler<CloneDigitalContentAsNewVersionCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<UploadedContent> _uploadedContentRepository;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IOutboxQueue _outboxQueue;
        private readonly IContentUrlExtractor _contentUrlExtractor;

        public CloneDigitalContentAsNewVersionCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<LearningContent> learningContentRepository,
            IRepository<UploadedContent> uploadedContentRepository,
            IRepository<AttributionElement> attributionElemRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Chapter> chapterRepository,
            IAccessControlContext accessControlContext,
            IOutboxQueue outboxQueue,
            IContentUrlExtractor contentUrlExtractor,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _learningContentRepository = learningContentRepository;
            _uploadedContentRepository = uploadedContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _accessRightRepository = accessRightRepository;
            _chapterRepository = chapterRepository;
            _outboxQueue = outboxQueue;
            _contentUrlExtractor = contentUrlExtractor;
        }

        protected override async Task HandleAsync(CloneDigitalContentAsNewVersionCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var existingDigitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.Id, cancellationToken);

            if (existingDigitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            DigitalContentModel model = new DigitalContentModel();

            if (existingDigitalContent.Type == ContentType.LearningContent)
            {
                var newClonedLearningContent = CommandUtilities.CloneLearningContent(
                    existingDigitalContent,
                    command.NewId,
                    command.UserId,
                    command.Status,
                    CreateCloneTitle);

                newClonedLearningContent.ChangedDate = Clock.Now;
                newClonedLearningContent.ParentId = existingDigitalContent.Id;
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
                   command.NewId,
                   command.UserId,
                   command.Status,
                   CreateCloneTitle);

                newClonedUploadedContent.ChangedDate = Clock.Now;
                newClonedUploadedContent.ParentId = existingDigitalContent.Id;
                newClonedUploadedContent.OriginalObjectId = existingDigitalContent.OriginalObjectId == Guid.Empty ? existingDigitalContent.ParentId : existingDigitalContent.OriginalObjectId;

                // Keep owner info
                newClonedUploadedContent.OwnerId = existingDigitalContent.OwnerId;
                newClonedUploadedContent.CreatedBy = existingDigitalContent.CreatedBy;

                await _uploadedContentRepository.InsertAsync(newClonedUploadedContent);
                model = new DigitalContentModel(newClonedUploadedContent);

                await this.CloneForChapters(command.Id, command.NewId, command.UserId);
            }

            await this.CloneForAttributionElements(command.Id, command.NewId);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Cloned, model, UserContext);
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
