using System;
using ChapterAttachmentEntity = Microservice.Content.Domain.Entities.ChapterAttachment;

namespace Microservice.Content.Application.Models
{
    public class ChapterAttachmentModel
    {
        public ChapterAttachmentModel()
        {
        }

        public ChapterAttachmentModel(ChapterAttachmentEntity chapterAttachment)
        {
            Id = chapterAttachment.Id;
            ObjectId = chapterAttachment.Id;
            FileLocation = chapterAttachment.FileLocation;
            FileName = chapterAttachment.FileName;
            CreatedDate = chapterAttachment.CreatedDate;
            ChangedDate = chapterAttachment.ChangedDate;
            Chapter = new ChapterModel()
            {
                Id = chapterAttachment.Chapter.Id,
                CreatedBy = chapterAttachment.Chapter.CreatedBy,
                CreatedDate = chapterAttachment.Chapter.CreatedDate,
                ChangedBy = chapterAttachment.Chapter.ChangedBy,
                ChangedDate = chapterAttachment.Chapter.ChangedDate,
                ObjectId = chapterAttachment.Chapter.ObjectId,
                OriginalObjectId = chapterAttachment.Chapter.OriginalObjectId,
                Title = chapterAttachment.Chapter.Title,
                Description = chapterAttachment.Chapter.Description,
                TimeStart = chapterAttachment.Chapter.TimeStart,
                TimeEnd = chapterAttachment.Chapter.TimeEnd,
                SourceType = chapterAttachment.Chapter.SourceType
            };
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public string FileLocation { get; set; }

        public string FileName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public ChapterModel Chapter { get; set; }
    }
}
