using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using ChapterAttachmentEntity = Microservice.Content.Domain.Entities.ChapterAttachment;
using ChapterEntity = Microservice.Content.Domain.Entities.Chapter;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class SaveVideoChapterCommandHandler : BaseCommandHandler<SaveVideoChapterCommand>
    {
        private readonly IRepository<ChapterEntity> _chapterRepository;
        private readonly IRepository<ChapterAttachmentEntity> _chapterAttachmentRepository;

        public SaveVideoChapterCommandHandler(
            IRepository<ChapterEntity> chapterRepository,
            IRepository<ChapterAttachmentEntity> chapterAttachmentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _chapterRepository = chapterRepository;
            _chapterAttachmentRepository = chapterAttachmentRepository;
        }

        protected override async Task HandleAsync(SaveVideoChapterCommand command, CancellationToken cancellationToken)
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

        private async Task Create(SaveVideoChapterCommand command)
        {
            var request = command.CreationRequest;

            if (request.Chapters == null || !request.Chapters.Any())
            {
                return;
            }

            var noted = $"{command.SourceType} ";
            switch (command.SourceType)
            {
                case VideoSourceType.CSL:
                    noted += $"/ ObjectId: [VideoId]";
                    break;
                case VideoSourceType.DigitalContent:
                    noted += $"/ ObjectId: [DigitalContentId] / OriginalObjectId: [OriginalDigitalContentId]";
                    break;
                default:
                    noted += "error / none of case!!!";
                    break;
            }

            var toInsertChapters = request.Chapters
               .Select(requestChapter =>
               {
                   var chapterId = Guid.Equals(requestChapter.Id, Guid.Empty) ? Guid.NewGuid() : requestChapter.Id;
                   return new ChapterEntity
                   {
                       Id = chapterId,
                       ObjectId = request.ObjectId,
                       OriginalObjectId = request.ObjectId,
                       Title = requestChapter.Title,
                       Description = requestChapter.Description,
                       TimeStart = requestChapter.TimeStart,
                       TimeEnd = requestChapter.TimeEnd,
                       SourceType = command.SourceType,
                       Note = noted,
                       CreatedBy = command.UserId,
                       CreatedDate = Clock.Now,
                       Attachments = requestChapter.Attachments
                            .Select(attachmentRequest =>
                            {
                                var attachmentId = Guid.Equals(attachmentRequest.Id, Guid.Empty) ? Guid.NewGuid() : attachmentRequest.Id;
                                return new ChapterAttachmentEntity
                                {
                                    Id = attachmentId,
                                    ObjectId = chapterId,
                                    FileName = attachmentRequest.FileName,
                                    FileLocation = attachmentRequest.FileLocation,
                                    CreatedDate = Clock.Now
                                };
                            }).ToList()
                   };
               });

            await _chapterRepository.InsertManyAsync(toInsertChapters);
        }

        private async Task Update(SaveVideoChapterCommand command)
        {
            var request = command.UpdateRequest;

            var objectId = command.UpdateRequest.ObjectId;

            // *** allow user can delete all chapter
            var chapters = await _chapterRepository
                .GetAllIncluding(chapter => chapter.Attachments)
                .Where(chapter => chapter.ObjectId == objectId)
                .ToListAsync();

            var noted = $"{command.SourceType} ";
            switch (command.SourceType)
            {
                case VideoSourceType.CSL:
                    noted += $"/ ObjectId: [VideoId]";
                    break;
                case VideoSourceType.DigitalContent:
                    noted += $"/ ObjectId: [DigitalContentId] / OriginalObjectId: [OriginalDigitalContentId]";
                    break;
                default:
                    noted += "error / none of case!!!";
                    break;
            }

            var toInsertChapters = request.Chapters
                .FindAll(requestChapter => !chapters.Exists(chapter => chapter.Id == requestChapter.Id))
                .Select(requestChapter =>
                {
                    var chapterId = Guid.Equals(requestChapter.Id, Guid.Empty) ? Guid.NewGuid() : requestChapter.Id;

                    return new ChapterEntity
                    {
                        Id = chapterId,
                        ObjectId = objectId,
                        OriginalObjectId = objectId,
                        Title = requestChapter.Title,
                        Description = requestChapter.Description,
                        TimeStart = requestChapter.TimeStart,
                        TimeEnd = requestChapter.TimeEnd,
                        SourceType = command.SourceType,
                        Note = noted,
                        CreatedBy = command.UserId,
                        CreatedDate = Clock.Now,
                        Attachments = requestChapter.Attachments
                            .Select(attachmentRequest =>
                            {
                                var attachmentId = Guid.Equals(attachmentRequest.Id, Guid.Empty) ? Guid.NewGuid() : attachmentRequest.Id;
                                return new ChapterAttachmentEntity
                                {
                                    Id = attachmentId,
                                    ObjectId = chapterId,
                                    FileName = attachmentRequest.FileName,
                                    FileLocation = attachmentRequest.FileLocation,
                                    CreatedDate = Clock.Now
                                };
                            }).ToList()
                    };
                });

            var toUpdatedChapters = request.Chapters
                .FindAll(requestChapter => chapters.Exists(chapter => chapter.Id == requestChapter.Id))
                .Select(requestChapter =>
                {
                    var chapter = chapters.Find(chapter => chapter.Id == requestChapter.Id);
                    chapter.Title = requestChapter.Title;
                    chapter.Description = requestChapter.Description;
                    chapter.TimeStart = requestChapter.TimeStart;
                    chapter.TimeEnd = requestChapter.TimeEnd;
                    chapter.SourceType = command.SourceType;
                    chapter.Note = noted;
                    chapter.ChangedBy = command.UserId;
                    chapter.ChangedDate = Clock.Now;

                    UpdateChapterAttachments(requestChapter.Id, requestChapter);

                    return chapter;
                }).ToList();

            var toDeleteChapters = chapters.FindAll(chapter => !request.Chapters.Exists(request => request.Id == chapter.Id));

            await _chapterRepository.InsertManyAsync(toInsertChapters);
            await _chapterRepository.UpdateManyAsync(toUpdatedChapters);
            await _chapterRepository.DeleteManyAsync(toDeleteChapters);
        }

        private void UpdateChapterAttachments(Guid chapterId, UpdateChapterRequest request)
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
                    var attachmentId = Guid.Equals(request.Id, Guid.Empty) ? Guid.NewGuid() : request.Id;
                    return new ChapterAttachmentEntity
                    {
                        Id = attachmentId,
                        ObjectId = chapterId,
                        FileName = request.FileName,
                        FileLocation = request.FileLocation,
                        CreatedDate = Clock.Now
                    };
                });

            var toDeleteAttachments = attachments
                .FindAll(attachment => !request.Attachments.Exists(request => request.Id == attachment.Id));

            _chapterAttachmentRepository.InsertMany(toInsertAttachments);
            _chapterAttachmentRepository.DeleteMany(toDeleteAttachments);
        }
    }
}
