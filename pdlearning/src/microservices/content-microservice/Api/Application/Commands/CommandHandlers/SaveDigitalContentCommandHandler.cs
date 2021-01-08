using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Application.Services;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Domain.ValueObject;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class SaveDigitalContentCommandHandler : BaseCommandHandler<SaveDigitalContentCommand>
    {
        private readonly IRepository<UploadedContent> _uploadedContentRepository;
        private readonly IRepository<LearningContent> _learningContentRepository;
        private readonly IRepository<AttributionElement> _attributionElemRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<ChapterAttachment> _chapterAttachmentRepository;
        private readonly IContentUrlExtractor _contentUrlExtractor;
        private readonly IOutboxQueue _outboxQueue;

        public SaveDigitalContentCommandHandler(
            IRepository<LearningContent> learningContentRepository,
            IRepository<UploadedContent> uploadedContentRepository,
            IRepository<AttributionElement> attributionElemRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<Chapter> chapterRepository,
            IRepository<ChapterAttachment> chapterAttachmentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IOutboxQueue outboxQueue,
            IUserContext userContext,
            IContentUrlExtractor contentUrlExtractor,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _learningContentRepository = learningContentRepository;
            _uploadedContentRepository = uploadedContentRepository;
            _attributionElemRepository = attributionElemRepository;
            _accessRightRepository = accessRightRepository;
            _chapterRepository = chapterRepository;
            _chapterAttachmentRepository = chapterAttachmentRepository;
            _contentUrlExtractor = contentUrlExtractor;
            _outboxQueue = outboxQueue;
        }

        protected override async Task HandleAsync(SaveDigitalContentCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await Create(command);
            }
            else
            {
                await Update(command);
            }
        }

        private async Task Update(SaveDigitalContentCommand command)
        {
            DigitalContentModel model = new DigitalContentModel();
            if (command.UpdateRequest.Type == ContentType.LearningContent)
            {
                IQueryable<LearningContent> dbQuery = _learningContentRepository
                   .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerOrApprovalPermissionExpr<LearningContent>(CurrentUserId))
                   .CombineWithAccessRight(_learningContentRepository, _accessRightRepository, CurrentUserId);

                var existedLearningContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.UpdateRequest.Id);

                if (existedLearningContent == null)
                {
                    throw new ContentAccessDeniedException();
                }

                existedLearningContent.Title = command.UpdateRequest.Title;
                existedLearningContent.Status = UpdateSubmitForApprovalStatus(command, existedLearningContent.Status);
                existedLearningContent.Type = command.UpdateRequest.Type;
                existedLearningContent.Description = HttpUtility.HtmlDecode(command.UpdateRequest.Description) != command.UpdateRequest.Description
                    ? command.UpdateRequest.Description
                    : HttpUtility.HtmlEncode(command.UpdateRequest.Description);
                existedLearningContent.HtmlContent = HttpUtility.HtmlDecode(command.UpdateRequest.HtmlContent) != command.UpdateRequest.HtmlContent
                    ? command.UpdateRequest.HtmlContent
                    : HttpUtility.HtmlEncode(command.UpdateRequest.HtmlContent);
                existedLearningContent.ChangedDate = Clock.Now;
                existedLearningContent.ChangedBy = command.UserId;
                existedLearningContent.ExpiredDate = command.UpdateRequest.ExpiredDate;
                existedLearningContent.Source = command.UpdateRequest.Source;
                existedLearningContent.Publisher = command.UpdateRequest.Publisher;
                existedLearningContent.Copyright = command.UpdateRequest.Copyright;
                existedLearningContent.TermsOfUse = command.UpdateRequest.TermsOfUse;
                existedLearningContent.LicenseType = command.UpdateRequest.LicenseType;
                existedLearningContent.StartDate = command.UpdateRequest.StartDate;
                existedLearningContent.AcknowledgementAndCredit = command.UpdateRequest.AcknowledgementAndCredit;
                existedLearningContent.IsAllowDownload = command.UpdateRequest.IsAllowDownload;
                existedLearningContent.IsAllowModification = command.UpdateRequest.IsAllowModification;
                existedLearningContent.IsAllowReusable = command.UpdateRequest.IsAllowReusable;
                existedLearningContent.LicenseTerritory = command.UpdateRequest.LicenseTerritory;
                existedLearningContent.Remarks = command.UpdateRequest.Remarks;
                existedLearningContent.Ownership = command.UpdateRequest.Ownership;
                existedLearningContent.OriginalObjectId = existedLearningContent.OriginalObjectId == Guid.Empty ? existedLearningContent.Id : existedLearningContent.OriginalObjectId;
                existedLearningContent.PrimaryApprovingOfficerId = command.UpdateRequest.PrimaryApprovingOfficerId;
                existedLearningContent.AlternativeApprovingOfficerId = command.UpdateRequest.AlternativeApprovingOfficerId;
                existedLearningContent.ArchiveDate = command.UpdateRequest.ArchiveDate;
                existedLearningContent.AutoPublishDate = command.UpdateRequest.AutoPublishDate;
                existedLearningContent.IsAutoPublish = command.UpdateRequest.IsAutoPublish;

                await _learningContentRepository.UpdateAsync(existedLearningContent);
                await _contentUrlExtractor.ExtractContentUrl(existedLearningContent);

                model = new DigitalContentModel(existedLearningContent);
            }
            else if (command.UpdateRequest.Type == ContentType.UploadedContent)
            {
                IQueryable<UploadedContent> dbQuery = _uploadedContentRepository
                   .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerOrApprovalPermissionExpr<UploadedContent>(CurrentUserId))
                   .CombineWithAccessRight(_uploadedContentRepository, _accessRightRepository, CurrentUserId);

                var existedUploadedContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.UpdateRequest.Id);

                // [OP-13494] delete annotation when user replace/delete video
                if (existedUploadedContent.FileLocation != command.UpdateRequest.FileLocation)
                {
                    command.UpdateRequest.IsReplaceVideo = true;
                }

                existedUploadedContent.Title = command.UpdateRequest.Title;
                existedUploadedContent.Status = UpdateSubmitForApprovalStatus(command, existedUploadedContent.Status);
                existedUploadedContent.Type = command.UpdateRequest.Type;
                existedUploadedContent.Description = command.UpdateRequest.Description;
                existedUploadedContent.FileName = command.UpdateRequest.FileName;
                existedUploadedContent.FileType = command.UpdateRequest.FileType;
                existedUploadedContent.FileExtension = command.UpdateRequest.FileExtension;
                existedUploadedContent.FileSize = command.UpdateRequest.FileSize;
                existedUploadedContent.FileLocation = command.UpdateRequest.FileLocation;
                existedUploadedContent.FileDuration = command.UpdateRequest.FileDuration;
                existedUploadedContent.ChangedBy = command.UserId;
                existedUploadedContent.ChangedDate = Clock.Now;
                existedUploadedContent.ExpiredDate = command.UpdateRequest.ExpiredDate;
                existedUploadedContent.Source = command.UpdateRequest.Source;
                existedUploadedContent.Publisher = command.UpdateRequest.Publisher;
                existedUploadedContent.Copyright = command.UpdateRequest.Copyright;
                existedUploadedContent.TermsOfUse = command.UpdateRequest.TermsOfUse;
                existedUploadedContent.LicenseType = command.UpdateRequest.LicenseType;
                existedUploadedContent.StartDate = command.UpdateRequest.StartDate;
                existedUploadedContent.AcknowledgementAndCredit = command.UpdateRequest.AcknowledgementAndCredit;
                existedUploadedContent.IsAllowDownload = command.UpdateRequest.IsAllowDownload;
                existedUploadedContent.IsAllowModification = command.UpdateRequest.IsAllowModification;
                existedUploadedContent.IsAllowReusable = command.UpdateRequest.IsAllowReusable;
                existedUploadedContent.LicenseTerritory = command.UpdateRequest.LicenseTerritory;
                existedUploadedContent.Remarks = command.UpdateRequest.Remarks;
                existedUploadedContent.Ownership = command.UpdateRequest.Ownership;
                existedUploadedContent.OriginalObjectId = existedUploadedContent.OriginalObjectId == Guid.Empty ? existedUploadedContent.Id : existedUploadedContent.OriginalObjectId;
                existedUploadedContent.PrimaryApprovingOfficerId = command.UpdateRequest.PrimaryApprovingOfficerId;
                existedUploadedContent.AlternativeApprovingOfficerId = command.UpdateRequest.AlternativeApprovingOfficerId;
                existedUploadedContent.ArchiveDate = command.UpdateRequest.ArchiveDate;
                existedUploadedContent.AutoPublishDate = command.UpdateRequest.AutoPublishDate;
                existedUploadedContent.IsAutoPublish = command.UpdateRequest.IsAutoPublish;

                await _uploadedContentRepository.UpdateAsync(existedUploadedContent);
                model = new DigitalContentModel(existedUploadedContent);

                // only apply for UploadedContent
                await UpdateAnnotation(command.UpdateRequest.Id, command.UserId, command.UpdateRequest, model);
            }

            await UpdateElement(command.UpdateRequest.Id, command.UpdateRequest, model);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Updated, model, UserContext);
        }

        private async Task Create(SaveDigitalContentCommand command)
        {
            DigitalContentModel model = new DigitalContentModel();
            if (command.CreationRequest.Type == ContentType.LearningContent)
            {
                var learningContent = new LearningContent
                {
                    Id = command.Id,
                    Title = command.CreationRequest.Title,
                    Status = command.CreationRequest.Status,
                    Type = command.CreationRequest.Type,
                    Description = HttpUtility.HtmlDecode(command.CreationRequest.Description) != command.CreationRequest.Description
                        ? command.CreationRequest.Description
                        : HttpUtility.HtmlEncode(command.CreationRequest.Description),
                    HtmlContent = HttpUtility.HtmlDecode(command.CreationRequest.HtmlContent) != command.CreationRequest.HtmlContent
                        ? command.CreationRequest.HtmlContent
                        : HttpUtility.HtmlEncode(command.CreationRequest.HtmlContent),
                    CreatedBy = command.UserId,
                    CreatedDate = Clock.Now,
                    OwnerId = command.UserId,
                    ExpiredDate = command.CreationRequest.ExpiredDate,
                    Source = command.CreationRequest.Source,
                    Publisher = command.CreationRequest.Publisher,
                    Copyright = command.CreationRequest.Copyright,
                    TermsOfUse = command.CreationRequest.TermsOfUse,
                    LicenseType = command.CreationRequest.LicenseType,
                    StartDate = command.CreationRequest.StartDate,
                    AcknowledgementAndCredit = command.CreationRequest.AcknowledgementAndCredit,
                    IsAllowDownload = command.CreationRequest.IsAllowDownload,
                    IsAllowModification = command.CreationRequest.IsAllowModification,
                    IsAllowReusable = command.CreationRequest.IsAllowReusable,
                    LicenseTerritory = command.CreationRequest.LicenseTerritory,
                    Remarks = command.CreationRequest.Remarks,
                    Ownership = command.CreationRequest.Ownership,
                    OriginalObjectId = command.Id,
                    PrimaryApprovingOfficerId = command.CreationRequest.PrimaryApprovingOfficerId,
                    AlternativeApprovingOfficerId = command.CreationRequest.AlternativeApprovingOfficerId,
                    DepartmentId = command.DepartmentId,
                    ArchiveDate = command.CreationRequest.ArchiveDate,
                    AutoPublishDate = command.CreationRequest.AutoPublishDate,
                    IsAutoPublish = command.CreationRequest.IsAutoPublish
                };
                await _learningContentRepository.InsertAsync(learningContent);
                model = new DigitalContentModel(learningContent);
            }
            else if (command.CreationRequest.Type == ContentType.UploadedContent)
            {
                var uploadContent = new UploadedContent
                {
                    Id = command.Id,
                    Title = command.CreationRequest.Title,
                    Status = command.CreationRequest.Status,
                    Type = command.CreationRequest.Type,
                    Description = command.CreationRequest.Description,
                    FileName = command.CreationRequest.FileName,
                    FileType = command.CreationRequest.FileType,
                    FileExtension = command.CreationRequest.FileExtension,
                    FileSize = command.CreationRequest.FileSize,
                    FileLocation = command.CreationRequest.FileLocation,
                    CreatedBy = command.UserId,
                    CreatedDate = Clock.Now,
                    OwnerId = command.UserId,
                    ExpiredDate = command.CreationRequest.ExpiredDate,
                    Source = command.CreationRequest.Source,
                    Publisher = command.CreationRequest.Publisher,
                    Copyright = command.CreationRequest.Copyright,
                    TermsOfUse = command.CreationRequest.TermsOfUse,
                    LicenseType = command.CreationRequest.LicenseType,
                    StartDate = command.CreationRequest.StartDate,
                    AcknowledgementAndCredit = command.CreationRequest.AcknowledgementAndCredit,
                    IsAllowDownload = command.CreationRequest.IsAllowDownload,
                    IsAllowModification = command.CreationRequest.IsAllowModification,
                    IsAllowReusable = command.CreationRequest.IsAllowReusable,
                    LicenseTerritory = command.CreationRequest.LicenseTerritory,
                    Remarks = command.CreationRequest.Remarks,
                    Ownership = command.CreationRequest.Ownership,
                    OriginalObjectId = command.Id,
                    PrimaryApprovingOfficerId = command.CreationRequest.PrimaryApprovingOfficerId,
                    AlternativeApprovingOfficerId = command.CreationRequest.AlternativeApprovingOfficerId,
                    DepartmentId = command.DepartmentId,
                    ArchiveDate = command.CreationRequest.ArchiveDate,
                    AutoPublishDate = command.CreationRequest.AutoPublishDate,
                    IsAutoPublish = command.CreationRequest.IsAutoPublish
                };
                await _uploadedContentRepository.InsertAsync(uploadContent);
                model = new DigitalContentModel(uploadContent);
            }

            await CreateAttributionElements(command.Id, command.CreationRequest, model);

            await _outboxQueue.QueueMessageAsync(DigitalContentChangeType.Created, model, UserContext);
        }

        private async Task UpdateElement(Guid digitalContentId, UpdateDigitalContentRequest request, DigitalContentModel model)
        {
            if (!request.AttributionElements.Any())
            {
                return;
            }

            var allElements = await _attributionElemRepository.GetAllListAsync(_ => _.DigitalContentId == digitalContentId);
            var hasNew = request.AttributionElements.Any(_ => _.Id == Guid.Empty);
            if (hasNew)
            {
                // with this solution (delete all then create from beginning) we don't care about the order of them when insert
                await this._attributionElemRepository.DeleteManyAsync(allElements);
                var newAttributionElems = request.AttributionElements.Select(_ => new AttributionElement
                {
                    Id = Guid.NewGuid(),
                    DigitalContentId = digitalContentId,
                    LicenseType = _.LicenseType,
                    Author = _.Author,
                    Source = _.Source,
                    Title = _.Title,
                    CreatedDate = Clock.Now
                });

                foreach (var attributeElem in newAttributionElems)
                {
                    var test = new AttributionElementModel
                    {
                        Id = attributeElem.Id,
                        Author = attributeElem.Author,
                        DigitalContentId = attributeElem.DigitalContentId,
                        LicenseType = attributeElem.LicenseType,
                        Source = attributeElem.Source,
                        Title = attributeElem.Title
                    };

                    model.AttributionElements.Add(test);
                }

                await _attributionElemRepository.InsertManyAsync(newAttributionElems);
                return;
            }

            // update case
            var updateElems = request.AttributionElements.Join(
                allElements,
                _ => _.Id,
                _ => _.Id,
                (a, b) => (Origin: b, Newer: a)).ToList();

            if (updateElems.Any())
            {
                foreach (var (origin, newer) in updateElems)
                {
                    var attributionElementModel = new AttributionElementModel
                    {
                        Author = origin.Author = newer.Author,
                        LicenseType = origin.LicenseType = newer.LicenseType,
                        Source = origin.Source = newer.Source,
                        Title = origin.Title = newer.Title,
                        DigitalContentId = origin.DigitalContentId,
                        Id = origin.Id
                    };

                    origin.ChangedDate = Clock.Now;
                    model.AttributionElements.Add(attributionElementModel);
                }

                await _attributionElemRepository.UpdateManyAsync(updateElems.Select(_ => _.Origin));
            }

            // delete case
            var deleteElems = allElements.FindAll(_ => !request.AttributionElements.Exists(a => a.Id == _.Id));
            if (deleteElems.Any())
            {
                await this._attributionElemRepository.DeleteManyAsync(allElements);
            }
        }

        private async Task CreateAttributionElements(Guid digitalContentId, CreateDigitalContentRequest request, DigitalContentModel model)
        {
            if (request.AttributionElements == null || !request.AttributionElements.Any())
            {
                return;
            }

            foreach (var attElem in request.AttributionElements)
            {
                var attributionElement = new AttributionElement();
                var attributionElementModel = new AttributionElementModel
                {
                    Id = attributionElement.Id = Guid.NewGuid(),
                    DigitalContentId = attributionElement.DigitalContentId = digitalContentId,
                    LicenseType = attributionElement.LicenseType = attElem.LicenseType,
                    Author = attributionElement.Author = attElem.Author,
                    Source = attributionElement.Source = attElem.Source,
                    Title = attributionElement.Title = attElem.Title
                };
                attributionElement.CreatedDate = Clock.Now;
                await _attributionElemRepository.InsertAsync(attributionElement);
                model.AttributionElements.Add(attributionElementModel);
            }
        }

        private DigitalContentStatus UpdateSubmitForApprovalStatus(SaveDigitalContentCommand command, DigitalContentStatus oldStatus)
        {
            if (oldStatus == DigitalContentStatus.Draft && command.UpdateRequest?.Status == DigitalContentStatus.ReadyToUse)
            {
                return DigitalContentStatus.ReadyToUse;
            }

            if (oldStatus == DigitalContentStatus.ReadyToUse && command.UpdateRequest?.Status == DigitalContentStatus.Draft)
            {
                return DigitalContentStatus.Draft;
            }

            if (command.IsSubmitForApproval
                && (oldStatus == DigitalContentStatus.Draft || oldStatus == DigitalContentStatus.Rejected))
            {
                return DigitalContentStatus.PendingForApproval;
            }

            if (oldStatus == DigitalContentStatus.Approved
                || oldStatus == DigitalContentStatus.Rejected
                || oldStatus == DigitalContentStatus.Unpublished)
            {
                return DigitalContentStatus.Draft;
            }

            return oldStatus;
        }

        private async Task UpdateAnnotation(Guid digitalContentId, Guid userId, UpdateDigitalContentRequest request, DigitalContentModel model)
        {
            // [OP-13494] delete annotation when user replace/delete video
            if (request.IsReplaceVideo)
            {
                request.Chapters = new List<UpdateChapterRequest>();
            }

            await UpdateChapters(digitalContentId, userId, request, model);
        }

        private async Task UpdateChapters(Guid digitalContentId, Guid userId, UpdateDigitalContentRequest request, DigitalContentModel model)
        {
            if (request.Chapters == null || !request.Chapters.Any())
            {
                return;
            }

            // *** allow user can delete all chapter
            var chapters = await _chapterRepository
                .GetAllIncluding(chapter => chapter.Attachments)
                .Where(chapter => chapter.ObjectId == digitalContentId)
                .ToListAsync();

            var toInsertChapters = request.Chapters
                .FindAll(request => !chapters.Exists(chapter => chapter.Id == request.Id))
                .Select(request =>
                {
                    var chapter = new Chapter();
                    var chapterModel = new ChapterModel
                    {
                        Id = chapter.Id = request.Id,
                        ObjectId = chapter.ObjectId = model.Id,
                        OriginalObjectId = chapter.OriginalObjectId = model.OriginalObjectId,
                        Title = chapter.Title = request.Title,
                        Description = chapter.Description = request.Description,
                        TimeStart = chapter.TimeStart = request.TimeStart,
                        TimeEnd = chapter.TimeEnd = request.TimeEnd,
                        SourceType = chapter.SourceType = VideoSourceType.DigitalContent, // [OP-13505] At this feature, Chapter function used by CCPM/LMM
                        Note = chapter.Note = $"{chapter.SourceType} / ObjectId: [DigitalContentId] / OriginalObjectId: [OriginalDigitalContentId]",
                        CreatedBy = chapter.CreatedBy = userId,
                        CreatedDate = chapter.CreatedDate = Clock.Now,
                        Attachments = request.Attachments
                         .Select(attachmentRequest =>
                         {
                             var attachment = new ChapterAttachment();
                             var attachmentModel = new ChapterAttachmentModel
                             {
                                 Id = attachment.Id = attachmentRequest.Id,
                                 ObjectId = attachment.ObjectId = chapter.Id,
                                 FileName = attachment.FileName = attachmentRequest.FileName,
                                 FileLocation = attachment.FileLocation = attachmentRequest.FileLocation,
                                 CreatedDate = attachment.CreatedDate = Clock.Now
                             };
                             chapter.Attachments.Add(attachment);
                             return attachmentModel;
                         }).ToList()
                    };
                    model.Chapters.Add(chapterModel);
                    return chapter;
                });

            var toUpdatedChapters = request.Chapters
                .FindAll(request => chapters.Exists(chapter => chapter.Id == request.Id))
                .Select(request =>
                {
                    var chapter = chapters.Find(chapter => chapter.Id == request.Id);
                    var chapterModel = new ChapterModel
                    {
                        Id = chapter.Id,
                        ObjectId = chapter.ObjectId,
                        OriginalObjectId = chapter.OriginalObjectId,
                        Title = chapter.Title = request.Title,
                        Description = chapter.Description = request.Description,
                        TimeStart = chapter.TimeStart = request.TimeStart,
                        TimeEnd = chapter.TimeEnd = request.TimeEnd,
                        SourceType = chapter.SourceType = VideoSourceType.DigitalContent, // [OP-13505] At this feature, Chapter function used by CCPM/LMM, filled by default for old data
                        Note = chapter.Note = $"{chapter.SourceType} / ObjectId: [DigitalContentId] / OriginalObjectId: [OriginalDigitalContentId]",
                        CreatedBy = chapter.CreatedBy,
                        CreatedDate = chapter.CreatedDate,
                        ChangedBy = chapter.ChangedBy = userId,
                        ChangedDate = chapter.ChangedDate = Clock.Now,
                    };
                    model.Chapters.Add(chapterModel);

                    UpdateChapterAttachments(request.Id, request, chapterModel);

                    return chapter;
                }).ToList();

            var toDeleteChapters = chapters.FindAll(chapter => !request.Chapters.Exists(request => request.Id == chapter.Id));

            await _chapterRepository.InsertManyAsync(toInsertChapters);
            await _chapterRepository.UpdateManyAsync(toUpdatedChapters);
            await _chapterRepository.DeleteManyAsync(toDeleteChapters);
        }

        private void UpdateChapterAttachments(Guid chapterId, UpdateChapterRequest request, ChapterModel model)
        {
            // *** allow user can delete all attachments
            // update case: with this flow, cant happened, user only can add/delete files
            var attachments = _chapterAttachmentRepository
                .GetAllIncluding(_ => _.Chapter)
                .Where(_ => _.ObjectId == chapterId)
                .ToList();

            var toInsertAttachments = request.Attachments
                .FindAll(request => !attachments.Exists(attachment => attachment.Id == request.Id))
                .Select(request =>
                {
                    var attachment = new ChapterAttachment();
                    var attachmentModel = new ChapterAttachmentModel
                    {
                        Id = attachment.Id = request.Id,
                        ObjectId = attachment.ObjectId = model.Id,
                        FileName = attachment.FileName = request.FileName,
                        FileLocation = attachment.FileLocation = request.FileLocation,
                        CreatedDate = attachment.CreatedDate = Clock.Now
                    };
                    model.Attachments.Add(attachmentModel);
                    return attachment;
                });

            var toDeleteAttachments = attachments
                .FindAll(attachment => !request.Attachments.Exists(request => request.Id == attachment.Id));

            _chapterAttachmentRepository.InsertMany(toInsertAttachments);
            _chapterAttachmentRepository.DeleteMany(toDeleteAttachments);
        }
    }
}
