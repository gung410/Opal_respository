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
    public class CloneDigitalContentCommandHandler : BaseCommandHandler<CloneDigitalContentCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IRepository<UploadedContent> _uploadedContentRepository;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IContentUrlExtractor _contentUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IOutboxQueue _outboxQueue;
        private readonly IRepository<Chapter> _chapterRepository;

        public CloneDigitalContentCommandHandler(
            IRepository<DigitalContent> digitalContentRepository,
            IRepository<LearningContent> learningContentRepository,
            IRepository<UploadedContent> uploadedContentRepository,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<AttributionElement> attributionElemRepository,
            IContentUrlExtractor contentUrlExtractor,
            IOutboxQueue outboxQueue,
            IRepository<Chapter> chapterRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _learningContentRepository = learningContentRepository;
            _uploadedContentRepository = uploadedContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _contentUrlExtractor = contentUrlExtractor;
            _accessRightRepository = accessRightRepository;
            _outboxQueue = outboxQueue;
            _chapterRepository = chapterRepository;
        }

        protected override async Task HandleAsync(CloneDigitalContentCommand command, CancellationToken cancellationToken)
        {
            IQueryable<DigitalContent> dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasPermissionToSeeContentExpr(CurrentUserId))
               .CombineWithAccessRight(_digitalContentRepository, _accessRightRepository, CurrentUserId);

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
                    DigitalContentStatus.Draft,
                    CreateCloneTitle);

                newClonedLearningContent.DepartmentId = AccessControlContext.GetUserDepartment();
                newClonedLearningContent.ChangedDate = null;
                newClonedLearningContent.ChangedBy = Guid.Empty;
                newClonedLearningContent.OriginalObjectId = command.NewId;

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
                    DigitalContentStatus.Draft,
                    CreateCloneTitle);

                newClonedUploadedContent.ChangedDate = null;
                newClonedUploadedContent.ChangedBy = Guid.Empty;
                newClonedUploadedContent.OriginalObjectId = command.NewId;

                await _uploadedContentRepository.InsertAsync(newClonedUploadedContent);
                model = new DigitalContentModel(newClonedUploadedContent);

                await this.CloneForChapters(command.Id, command.NewId, model, command.UserId);
            }

            await this.CloneForAttributionElements(command.Id, command.NewId, model);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Cloned, model, UserContext);
        }

        private async Task CloneForAttributionElements(Guid digitalContentId, Guid newDigitalContentId, DigitalContentModel digitalContentModel)
        {
            var allElements = await _attributionElemRepository.GetAllListAsync(_ => _.DigitalContentId == digitalContentId);
            var clonedItems = CommandUtilities.CloneAttributionElement(allElements, newDigitalContentId);

            foreach (var attributionElement in clonedItems)
            {
                await _attributionElemRepository.InsertAsync(attributionElement);

                var attributionElementModel = new AttributionElementModel
                {
                    Id = attributionElement.Id,
                    DigitalContentId = attributionElement.DigitalContentId,
                    Source = attributionElement.Source,
                    Author = attributionElement.Author,
                    Title = attributionElement.Title,
                    LicenseType = attributionElement.LicenseType
                };

                digitalContentModel.AttributionElements.Add(attributionElementModel);
            }

            /* This is an example of async/await anti-pattern.
             refer: https://markheath.net/post/async-antipatterns
            clonedItems.ForEach(async attributionElement =>
            {
                await _attributionElemRepository.InsertAsync(attributionElement);
                var attributionElementModel = new AttributionElementModel
                {
                    Id = attributionElement.Id,
                    DigitalContentId = attributionElement.DigitalContentId,
                    Source = attributionElement.Source,
                    Author = attributionElement.Author,
                    Title = attributionElement.Title,
                    LicenseType = attributionElement.LicenseType
                };
                digitalContentModel.AttributionElements.Add(attributionElementModel);
            });
            */
        }

        private async Task CloneForChapters(Guid digitalContentId, Guid newDigitalContentId, DigitalContentModel digitalContentModel, Guid userId)
        {
            var chapters = await _chapterRepository
                .GetAllIncluding(_ => _.Attachments)
                .Where(_ => _.ObjectId == digitalContentId)
                .ToListAsync();
            var clonedChapters = CommandUtilities.CloneChapter(chapters, newDigitalContentId, userId);

            foreach (var chapter in clonedChapters)
            {
                await _chapterRepository.InsertAsync(chapter);

                var chapterModel = new ChapterModel
                {
                    Id = chapter.Id,
                    ObjectId = chapter.ObjectId,
                    OriginalObjectId = chapter.OriginalObjectId,
                    Title = chapter.Title,
                    Description = chapter.Description,
                    TimeStart = chapter.TimeStart,
                    TimeEnd = chapter.TimeEnd,
                    CreatedBy = chapter.CreatedBy,
                    CreatedDate = chapter.CreatedDate,
                    SourceType = chapter.SourceType,
                    Attachments = chapter.Attachments
                         .Select(attachment => new ChapterAttachmentModel
                         {
                             Id = attachment.Id,
                             ObjectId = attachment.ObjectId,
                             FileName = attachment.FileName,
                             FileLocation = attachment.FileLocation,
                             CreatedDate = attachment.CreatedDate
                         }).ToList()
                };

                digitalContentModel.Chapters.Add(chapterModel);
            }
        }

        private string CreateCloneTitle(string originTitle)
        {
            return $"Copy of {originTitle}";
        }
    }
}
